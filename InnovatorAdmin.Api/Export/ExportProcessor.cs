using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace InnovatorAdmin
{
  public class ExportProcessor : IProgressReporter
  {

    private enum CycleState
    {
      NoCycle,
      UnresolvedCycle,
      ResolvedCycle
    }

    private Version _arasVersion;
    private readonly IAsyncConnection _conn;
    private readonly DependencyAnalyzer _dependAnalyzer;
    private readonly ArasMetadataProvider _metadata;
    XslCompiledTransform _resultTransform;
    public event EventHandler<ActionCompleteEventArgs> ActionComplete;
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    /// <summary>
    /// Construct a new <see cref="ExportProcessor"/> from a connection to Aras
    /// </summary>
    /// <param name="conn"></param>
    public ExportProcessor(IAsyncConnection conn)
    {
      _conn = conn;
      if (_conn is Innovator.Client.Connection.IArasConnection arasConn)
        arasConn.DefaultSettings.Add(r => r.Timeout = TimeSpan.FromMinutes(3));
      _metadata = ArasMetadataProvider.Cached(conn);
      _metadata.Reset();
      _dependAnalyzer = new DependencyAnalyzer(_metadata);
    }

    /// <summary>
    /// Fill an install script with exports of the specified items
    /// </summary>
    public async Task Export(InstallScript script, IEnumerable<ItemReference> items
      , bool checkDependencies = true)
    {
      _arasVersion = (await _conn.FetchVersion(true)) ?? new Version(9, 3);
      ReportProgress(0, "Loading system data");
      await _metadata.ReloadTask().ConfigureAwait(false);

      // On each scan, reload everything from the database.  Even if this might degrade
      // performance, it is much more reliable
      var uniqueItems = new HashSet<ItemReference>(items);
      uniqueItems.UnionWith(script.Lines.Where(l => l.Type == InstallType.Create).Select(l => l.Reference));
      script.Lines = null;
      var itemList = uniqueItems.ToList();

      var outputDoc = new XmlDocument();
      outputDoc.AppendChild(outputDoc.CreateElement("AML"));
      XmlElement queryElem;
      var whereClause = new StringBuilder();

      ConvertPolyItemReferencesToActualType(itemList);
      itemList = (await NormalizeReferences(itemList).ConfigureAwait(false)).ToList();

      var groups = itemList.GroupBy(i => new { Type = i.Type, Levels = i.Levels });
      var queries = new List<XmlElement>();
      foreach (var typeItems in groups)
      {
        var pageSize = GroupSize(typeItems.Key.Type, typeItems.Key.Levels);
        var pageCount = (int)Math.Ceiling((double)typeItems.Count() / pageSize);
        for (var p = 0; p < pageCount; p++)
        {
          var refs = typeItems.Skip(p * pageSize).Take(pageSize);
          whereClause.Length = 0;
          queryElem = outputDoc.CreateElement("Item")
            .Attr("action", "get")
            .Attr("type", typeItems.Key.Type);

          if (refs.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[")
              .Append(typeItems.Key.Type.Replace(' ', '_'))
              .Append("].[id] in ('")
              .Append(refs.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).GroupConcat("','"))
              .Append("')");
          }
          if (refs.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              refs.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).GroupConcat(" or "));
          }

          queryElem.SetAttribute("where", whereClause.ToString());
          SetQueryAttributes(queryElem, typeItems.Key.Type, typeItems.Key.Levels, refs);
          queries.Add(queryElem);
        }
      }

      queries.Shuffle();
      foreach (var query in queries)
      {
        outputDoc.DocumentElement.AppendChild(query);
      }

      try
      {
        ReportProgress(0, "Loading system data");

        FixFederatedRelationships(outputDoc.DocumentElement);
        var result = ExecuteExportQuery(ref outputDoc, items);
        RemoveXPropertyFlattenRelationships(result);
        RemoveGeneratedItemTypeRelationships(result);
        RemoveCmfGeneratedTypes(result);
        RemoveVaultUrl(result);
        FixCmfComputedPropDependencies(result);
        FixCmfTabularViewMissingWarning(result);

        // Add warnings for embedded relationships
        var warnings = new HashSet<ItemReference>();
        ItemReference warning;
        foreach (var relType in result.ElementsByXPath("/Result/Item[@type='ItemType']/Relationships/Item[@type='RelationshipType']"))
        {
          warning = ItemReference.FromFullItem(relType as XmlElement, true);
          warning.KeyedName = "* Possible missing relationship: " + warning.KeyedName;
          warnings.Add(warning);
        }

        //Add warnings for cmf linked itemtypes
        foreach (var item in result.ElementsByXPath("/Result/Item[@type='ItemType']"))
        {
          var id = item.Attribute("id", "");
          if (_metadata.CmfLinkedTypes.TryGetValue(id, out var reference))
          {
            warning = reference.Clone();
            warning.KeyedName = "* Possible missing ContentType: " + warning.KeyedName;
            warnings.Add(warning);
          }
        }

        //RemoveRelatedItems(result, items);
        //CleanUpSystemProps(result.DocumentElement.Elements(), items, false);
        FixPolyItemReferences(result);
        FloatVersionableRefs(result);
        var doc = TransformResults(ref result);
        SetDoGetItemForCmf(doc);
        NormalizeClassStructure(doc);
        FixFormFieldsPointingToSystemProperties(doc);
        ExpandSystemIdentities(doc);
        RemoveVersionableRelIds(doc);
        //TODO: Replace references to poly item lists

        await Export(script, doc, warnings, checkDependencies).ConfigureAwait(false);
        CleanUpSystemProps(doc.DocumentElement.Elements(), items.ToDictionary(i => i), true);
        ConvertFloatProps(doc);
        RemoveKeyedNameAttributes(doc.DocumentElement);

        if (string.IsNullOrWhiteSpace(script.Title))
        {
          if (script.Lines.Count(l => l.Type == InstallType.Create) == 1)
          {
            script.Title = script.Lines.Single(l => l.Type == InstallType.Create).Reference.ToString();
          }
          else if (items.Count() == 1)
          {
            script.Title = items.First().ToString();
          }
        }

        // Rename the FileTypes and identities to make comparing easier
        ItemReference newRef;
        foreach (var line in script.Lines.Where(l => l.Reference.Type == "FileType" || l.Reference.Type == "Identity"))
        {
          newRef = ItemReference.FromFullItem(line.Script.DescendantsAndSelf(e => e.LocalName == "Item").First(), true);
          if (newRef.Type == line.Reference.Type && !String.Equals(newRef.KeyedName, line.Reference.KeyedName))
          {
            line.Reference.KeyedName = newRef.KeyedName;
          }
        }
      }
      catch (Exception ex)
      {
        this.OnActionComplete(new ActionCompleteEventArgs() { Exception = ex });
      }
#if DEBUG
      // This is useful for debugging and making sure duplicate items are not present
      if (script.Lines != null)
      {
        var grps = script.Lines.Where(l => l.Type == InstallType.Create)
          .GroupBy(l => l.Reference.Unique);
        var duplicates = grps.Where(g => g.Skip(1).Any()).ToArray();
        if (duplicates.Length > 0)
          throw new InvalidOperationException("The package has duplicate entries for the following items: " + duplicates.GroupConcat(", ", g => g.Key));
      }
#endif
    }

    private int GroupSize(string itemType, int levels)
    {
      switch (itemType)
      {
        case "ItemType":
        case "RelationshipType":
        case "cmf_ContentType":
        case "cmf_TabularView":
        case "cmf_ElementType":
        case "cmf_PropertyType":
        case "Form":
          return 6;
      }
      return levels > 1 ? 10 : 40;
    }

    /// <summary>
    /// Normalize the list of items to be exported
    /// </summary>
    /// <remarks>
    /// Get the latest version of versionable items, switch item type references to relationship types, and don't export polyitem lists
    /// </remarks>
    private async Task<IEnumerable<ItemReference>> NormalizeReferences(IEnumerable<ItemReference> references)
    {
      var groups = references.PagedGroupBy(i => new { i.Type, IsId = i.Unique.IsGuid() }, 250);
      var results = new List<ItemReference>();
      foreach (var group in groups)
      {
        if (_metadata.ItemTypeByName(group.Key.Type.ToLowerInvariant(), out var metaData) && metaData.IsVersionable && group.Key.IsId)
        {
          // For versionable item types, get the latest generation
          var toResolve = (await _conn.ApplyAsync(@"<Item type='@0' action='get' select='config_id'>
  <id condition='in'>@1</id>
  <is_current>0</is_current>
</Item>", true, false, group.Key.Type, group.Select(i => i.Unique).ToList()).ConfigureAwait(false)).Items()
            .ToDictionary(i => i.Id(), i => i.ConfigId().Value);
          results.AddRange(group.Where(i => !toResolve.ContainsKey(i.Unique)));
          if (toResolve.Count > 0)
          {
            results.AddRange((await _conn.ApplyAsync(@"<Item type='@0' action='get' select='id'>
  <config_id condition='in'>@1</config_id>
</Item>", true, false, group.Key.Type, toResolve.Values).ConfigureAwait(false)).Items()
              .Select(i => ItemReference.FromFullItem(i, true)));
          }
        }
        else if (group.Key.Type == "ItemType")
        {
          // Make sure relationship item types aren't accidentally added
          if (group.Key.IsId)
          {
            var relations = (await _conn.ApplyAsync(@"<Item type='RelationshipType' action='get' select='relationship_id'>
  <relationship_id condition='in'>@0</relationship_id>
</Item>", true, false, group.Select(i => i.Unique).ToList()).ConfigureAwait(false)).Items()
              .ToDictionary(i => i.Property("relationship_id").Value, i => ItemReference.FromFullItem(i, true));
            results.AddRange(group.Where(i => !relations.ContainsKey(i.Unique)));
            results.AddRange(relations.Values);
          }
          else
          {
            foreach (var itemType in group)
            {
              var relation = (await _conn.ApplyAsync(@"<Item type='RelationshipType' action='get' select='id'>
  <relationship_id><Item type='ItemType' action='get' where='@0'></Item></relationship_id>
</Item>", true, false, itemType.Unique).ConfigureAwait(false)).Items().FirstOrNullItem();
              if (relation.Exists)
                results.Add(ItemReference.FromFullItem(relation, true));
              else
                results.Add(itemType);
            }
          }
        }
        else if (group.Key.Type == "List")
        {
          var refs = default(List<ItemReference>);
          if (group.Key.IsId)
          {
            refs = group.ToList();
          }
          else
          {
            refs = (await _conn.ApplyAsync(@"<Item type='List' action='get' where='@0'></Item>", true, false, group.Select(i => i.Unique).GroupConcat(" or ")).ConfigureAwait(false))
              .Items()
              .Select(i => ItemReference.FromFullItem(i, true))
              .ToList();
          }

          // Filter out auto-generated lists for polymorphic item types
          var polyLists = new HashSet<string>((await _conn.ApplyAsync(@"<Item type='Property' action='get' select='data_source'>
  <source_id>
    <Item type='ItemType' action='get'>
      <implementation_type>polymorphic</implementation_type>
    </Item>
  </source_id>
  <name>itemtype</name>
  <data_source condition='in'>@0</data_source>
</Item>", true, false, refs.Select(i => i.Unique).ToList()).ConfigureAwait(false)).Items().Select(i => i.Property("data_source").Value));
          results.AddRange(refs.Where(i => !polyLists.Contains(i.Unique)));
        }
        else
        {
          results.AddRange(group);
        }
      }
      return results;
    }

    /// <summary>
    /// Fill an install script with exports of the items in the XmlDocument
    /// </summary>
    public async Task Export(InstallScript script, XmlDocument doc
      , HashSet<ItemReference> warnings = null, bool checkDependencies = true)
    {
      try
      {
        await _metadata.ReloadTask().ConfigureAwait(false);
        FixPolyItemReferences(doc);
        FixPolyItemListReferences(doc);
        await FixForeignPropertyReferences(doc).ConfigureAwait(false);
        FixForeignPropertyDependencies(doc);
        FixCyclicalWorkflowLifeCycleRefs(doc);
        FixCyclicalWorkflowItemTypeRefs(doc);
        FixCyclicalContentTypeTabularViewRefs(doc);

        RemoveEmptyRelationshipTags(doc);
        RemoveVersionableRelIds(doc);
        SortRelationshipTags(doc);

        MoveFormRefsInline(doc, (script.Lines ?? Enumerable.Empty<InstallItem>()).Where(l => l.Type == InstallType.Create || l.Type == InstallType.Script));

        // Sort the resulting nodes as appropriate
        ReportProgress(98, "Sorting the results");
        XmlNode itemNode = doc.DocumentElement;
        while (itemNode != null && itemNode.LocalName != "Item") itemNode = itemNode.Elements().FirstOrDefault();
        if (itemNode == null) throw new InvalidOperationException(); //TODO: Give better error information here (e.g. interpret an error item if present)

        IEnumerable<InstallItem> results = null;
        if (checkDependencies)
        {
          int loops = 0;
          CycleState state = CycleState.ResolvedCycle;
          while (loops < 10 && state == CycleState.ResolvedCycle)
          {
            // Only reset if this is not a rescan
            if (script.Lines != null)
            {
              _dependAnalyzer.Reset(from l in script.Lines where l.Type == InstallType.Create || l.Type == InstallType.Script select l.Reference);
            }
            else
            {
              _dependAnalyzer.Reset();
            }
            var newInstallItems = (from e in itemNode.ParentNode.Elements()
                                   where e.LocalName == "Item" && e.HasAttribute("type")
                                   select InstallItem.FromScript(e)).ToList();

            foreach (var newInstallItem in newInstallItems)
            {
              _dependAnalyzer.GatherDependencies(newInstallItem.Script, newInstallItem.Reference, newInstallItem.CoreDependencies);
            }
            _dependAnalyzer.CleanDependencies();

            results = GetDependencyList(_dependAnalyzer
              , (script.Lines ?? Enumerable.Empty<InstallItem>()).Concat(newInstallItems)
              , out state).ToList();
            loops++;
          }
        }
        else
        {
          var newInstallItems = (from e in itemNode.ParentNode.Elements()
                                 where e.LocalName == "Item" && e.HasAttribute("type")
                                 select InstallItem.FromScript(e))
            .OrderBy(l => DefaultInstallOrder(l.Reference))
            .ThenBy(l => l.Reference.Type.ToLowerInvariant())
            .ThenBy(l => l.Reference.Unique)
            .ToList();
          //foreach (var item in newInstallItems)
          //{
          //  item.Script.SetAttribute(XmlFlags.Attr_DependenciesAnalyzed, "1");
          //}

          results = (script.Lines ?? Enumerable.Empty<InstallItem>())
            .Concat(newInstallItems).ToList();
        }


        warnings = warnings ?? new HashSet<ItemReference>();
        warnings.ExceptWith(results.Select(r => r.Reference));

        script.Lines = warnings.Select(w => InstallItem.FromWarning(w, w.KeyedName))
          .Concat(results.Where(r => r.Type == InstallType.DependencyCheck || r.Type == InstallType.Warning))
          .OrderBy(r => r.Name)
          .ToList()
          .Concat(results.Where(r => r.Type != InstallType.DependencyCheck && r.Type != InstallType.Warning))
          .ToList();

        RemoveInjectedDependencies(doc);

        this.OnActionComplete(new ActionCompleteEventArgs());
      }
      catch (Exception ex)
      {
        this.OnActionComplete(new ActionCompleteEventArgs() { Exception = ex });
      }
    }

    public static IEnumerable<InstallItem> SortByDependencies(IEnumerable<InstallItem> items, IArasMetadataProvider metadata)
    {
      int loops = 0;
      var state = CycleState.ResolvedCycle;
      var results = items ?? Enumerable.Empty<InstallItem>();
      var analyzer = new DependencyAnalyzer(metadata);
      while (loops < 10 && state == CycleState.ResolvedCycle)
      {
        if (loops > 0)
          analyzer.Reset();

        foreach (var newInstallItem in items)
        {
          analyzer.GatherDependencies(newInstallItem.Script, newInstallItem.Reference, newInstallItem.CoreDependencies);
        }
        analyzer.CleanDependencies();

        results = GetDependencyList(analyzer, items, out state).ToList();
        loops++;
      }

      results = results
        .Where(i => !IsDelete(i))
        .Concat(results
          .Where(IsDelete)
          .OrderByDescending(DefaultInstallOrder)
        ).ToArray();

      return results;
    }

    private static bool IsDelete(InstallItem item)
    {
      return item.Type == InstallType.Script && item.Name.Split(' ').Contains("Delete");
    }

    public static async Task<IEnumerable<InstallItem>> SortByDependencies(IEnumerable<InstallItem> items, IAsyncConnection conn)
    {
      var metadata = ArasMetadataProvider.Cached(conn);
      await metadata.ReloadTask().ConfigureAwait(false);
      return SortByDependencies(items, metadata);
    }

    /// <summary>
    /// Normalize a list of item references.  Used in the resolution step to know which parent items
    /// to add given a dependency on a child.
    /// </summary>
    public IEnumerable<ItemReference> NormalizeRequest(IEnumerable<ItemReference> items)
    {
      foreach (var item in items)
      {
        if (item.Unique.IsGuid())
        {
          switch (item.Type)
          {
            case "Life Cycle State":
              yield return new ItemReference("Life Cycle Map", "[Life_Cycle_Map].[id] in (select source_id from [innovator].[Life_Cycle_State] where id = '" + item.Unique + "')");
              break;
            case "Life Cycle Transition":
              yield return new ItemReference("Life Cycle Map", "[Life_Cycle_Map].[id] in (select source_id from [innovator].[Life_Cycle_Transition] where id = '" + item.Unique + "')");
              break;
            case "Property":
              yield return new ItemReference("ItemType", "[ItemType].[id] in (select source_id from [innovator].[Property] where id = '" + item.Unique + "')");
              break;
            case "Activity":
              yield return new ItemReference("Workflow Map", "[Workflow_Map].[id] in (select source_id from [innovator].[Workflow_Map_Activity] where related_id = '" + item.Unique + "')");
              break;
            default:
              yield return item;
              break;
          }
        }
        else
        {
          yield return item;
        }
      }
    }

    /// <summary>
    /// Removes all Items that reference the given item
    /// </summary>
    public void RemoveReferencingItems(InstallScript script, ItemReference itemRef)
    {
      var nodes = _dependAnalyzer.RemoveDependencyContexts(itemRef);
      script.Lines = script.Lines.Where(l => !(l.Type == InstallType.Create || l.Type == InstallType.Script) || !nodes.Contains(l.Script)).ToList();
    }

    /// <summary>
    /// Removes all the properties that reference the given item
    /// </summary>
    public void RemoveReferences(InstallScript script, ItemReference itemRef)
    {
      var nodes = _dependAnalyzer.RemoveDependencyReferences(itemRef);
      script.Lines = script.Lines.Where(l => !(l.Type == InstallType.Create || l.Type == InstallType.Script) || !nodes.Contains(l.Script)).ToList();
    }

    /// <summary>
    /// Invoke the ProgressChanged event
    /// </summary>
    protected virtual void ReportProgress(int progress, string message)
    {
      OnProgressChanged(new ProgressChangedEventArgs(message, progress));
    }

    /// <summary>
    /// Invoke the ActionComplete event
    /// </summary>
    protected virtual void OnActionComplete(ActionCompleteEventArgs e)
    {
      if (this.ActionComplete != null) ActionComplete(this, e);
    }

    /// <summary>
    /// Invoke the ProgressChanged event
    /// </summary>
    protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
    {
      if (this.ProgressChanged != null) ProgressChanged(this, e);
    }

    private void RemoveInjectedDependencies(XmlDocument doc)
    {
      foreach (var node in doc.SelectNodes("//*[@_is_dependency = '1']").OfType<XmlElement>())
      {
        node.Detach();
      }
    }

    /// <summary>
    /// This method removes system properties (e.g. created_by_id, state, etc.).  However, if the user wants to persist
    /// some of these properties, they are preserved as a post-import SQL script.
    /// </summary>
    /// <param name="doc">Xml to cleanup</param>
    /// <param name="items">Data regarding which system properties to persist as sql scripts</param>
    /// <param name="cleanAll">Whether to preserve the required data for dependency checking, or remove it all</param>
    /// <remarks>
    /// This method is called twice. Once with <paramref name="cleanAll"/> set to <c>false</c> where the SQL scripts are written an unnecessary
    /// properties are removed.  Properties included in the SQL script are kept for dependency checking.  The run with <paramref name="cleanAll"/>
    /// set to <c>true</c> removes the remaining system properties.
    /// </remarks>
    private void CleanUpSystemProps(IEnumerable<XmlElement> elems
      , IDictionary<ItemReference, ItemReference> itemDict, bool cleanAll)
    {
      ItemReference itemRef;
      ItemReference itemRefOpts;
      SystemPropertyGroup group;
      var updateQuery = new StringBuilder(128);
      IEnumerable<XmlElement> itemElems;

      foreach (var elem in elems)
      {
        itemRef = ItemReference.FromFullItem(elem, false);
        group = SystemPropertyGroup.None;
        if (itemDict.TryGetValue(itemRef, out itemRefOpts))
        {
          group = itemRefOpts.SystemProps;
        }

        itemElems = elem.DescendantsAndSelf(i => i.LocalName == "Item" && i.HasAttribute("type") && i.Attribute("action") != "get");

        foreach (var item in itemElems)
        {
          updateQuery.Append("update innovator.[").Append(item.Attribute("type").Replace(' ', '_')).Append("] set ");

          foreach (var prop in item.Elements().ToList())
          {
            switch (prop.LocalName)
            {
              case "is_released":
              case "not_lockable":
              case "state":
              case "current_state":
              case "release_date":
              case "effective_date":
              case "superseded_date":
                if ((group & SystemPropertyGroup.State) == SystemPropertyGroup.State)
                {
                  if (cleanAll)
                  {
                    prop.Detach();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detach();
                }
                break;
              case "behavior":
                if (item.Attribute("type") != "RelationshipType")
                {
                  if ((group & SystemPropertyGroup.State) == SystemPropertyGroup.State)
                  {
                    if (cleanAll)
                    {
                      prop.Detach();
                    }
                    else
                    {
                      updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                    }
                  }
                  else
                  {
                    prop.Detach();
                  }
                }
                break;
              case "permission_id":
                if ((group & SystemPropertyGroup.Permission) == SystemPropertyGroup.Permission)
                {
                  if (cleanAll)
                  {
                    prop.Detach();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detach();
                }
                break;
              case "created_by_id":
              case "created_on":
              case "modified_by_id":
              case "modified_on":
                if ((group & SystemPropertyGroup.History) == SystemPropertyGroup.History)
                {
                  if (cleanAll)
                  {
                    prop.Detach();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detach();
                }
                break;
              case "major_rev":
              case "minor_rev":
                if ((group & SystemPropertyGroup.Versioning) == SystemPropertyGroup.Versioning)
                {
                  if (cleanAll)
                  {
                    prop.Detach();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detach();
                }
                break;
            }
          }

          if (group > 0 && !cleanAll)
          {
            updateQuery.Remove(updateQuery.Length - 2, 2);
            updateQuery.Append(" where config_id = '").Append(item.Attribute(XmlFlags.Attr_ConfigId, item.Attribute("id"))).Append("' and is_current = '1';");
            item.Attr(XmlFlags.Attr_SqlScript, updateQuery.ToString());
          }
        }
      }
    }

    /// <summary>
    /// Convert properties marked as float to a search using a where clause for more convenient execution
    /// </summary>
    private void ConvertFloatProps(XmlDocument doc)
    {
      doc.VisitDescendantsAndSelf(e =>
      {
        if (e.HasAttribute(XmlFlags.Attr_Float))
        {
          e.RemoveAttribute(XmlFlags.Attr_Float);
          if (!string.IsNullOrEmpty(e.Attribute("id"))
            && !string.IsNullOrEmpty(e.Attribute("type")))
          {
            e.SetAttribute(XmlFlags.Attr_ConfigId, e.Attribute("id"));
            e.SetAttribute("where", string.Format("[{0}].[config_id] = '{1}'", e.Attribute("type").Replace(' ', '_'), e.Attribute("id")));
            e.RemoveAttribute("id");
          }
          else if (!e.Elements().Any())
          {
            var configId = e.InnerText;
            e.InnerText = "";
            e.Elem("Item")
              .Attr("type", e.Attribute("type"))
              .Attr("action", "get")
              .Attr(XmlFlags.Attr_ConfigId, configId)
              .Attr("where", string.Format("[{0}].[config_id] = '{1}'", e.Attribute("type").Replace(' ', '_'), configId));
          }
        }
      });
    }

    /// <summary>
    /// Convert polyitems to their actual item types
    /// </summary>
    private void ConvertPolyItemReferencesToActualType(List<ItemReference> itemList)
    {
      ItemType metaData;
      ItemType realType;
      ItemReference newRef;
      var whereClause = new StringBuilder();

      foreach (var typeItems in (from i in itemList
                                 group i by i.Type into typeGroup
                                 select typeGroup).ToList())
      {
        if (_metadata.ItemTypeByName(typeItems.Key.ToLowerInvariant(), out metaData) && metaData.IsPolymorphic)
        {
          whereClause.Length = 0;

          if (typeItems.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[")
              .Append(typeItems.Key.Replace(' ', '_'))
              .Append("].[id] in ('")
              .Append(typeItems.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + "','" + c))
              .Append("')");
          }
          if (typeItems.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              typeItems.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + " or " + c));
          }

          // Remove the items from the list
          var idx = 0;
          while (idx < itemList.Count)
          {
            if (itemList[idx].Type == typeItems.Key)
            {
              itemList.RemoveAt(idx);
            }
            else
            {
              idx++;
            }
          }

          var polyItems = _conn.Apply("<Item action='get' type='@0' where='@1' />", typeItems.Key, whereClause.ToString()).Items();
          foreach (var polyItem in polyItems)
          {
            newRef = ItemReference.FromFullItem(polyItem, true);
            realType = _metadata.ItemTypes.FirstOrDefault(t => t.Id == polyItem.Property("itemtype").AsString(""));
            if (realType != null) newRef.Type = realType.Name;
            itemList.Add(newRef);
          }
        }
      }
    }
    /// <summary>
    /// Execute the query to get all the relevant data from the database regarding the items to export.
    /// </summary>
    private XmlDocument ExecuteExportQuery(ref XmlDocument query, IEnumerable<ItemReference> itemRefs)
    {
      var itemDic = itemRefs.ToDictionary(i => i);
      var queryItems = query.DocumentElement.Elements("Item");
      var result = query.NewDoc();
      ReportProgress(4, "Searching for data...");

      var promises = queryItems.Select(q => (Func<Task<IEnumerable<XmlElement>>>)(
        async () =>
        {
          var stream = await _conn.Process(q.OuterXml, true).ConfigureAwait(false);
          return Items(result, stream, itemDic).ToList();
        })
      ).ToArray();
      query = null; // Release the query memory as soon as possible;
      var items = SharedUtils.TaskPool(10, (i, m) =>
      {
        ReportProgress(4 + (int)(i * 0.9), "Searching for data...");
      }, promises).Result;

      ReportProgress(95, "Loading results into memory...");
      var elems = items.SelectMany(r => r);
      var root = result.Elem("Result");

      foreach (var elem in elems)
      {
        root.AppendChild(elem);
      }
      return result;
    }

    private IEnumerable<XmlElement> Items(XmlDocument doc, Stream stream
      , IDictionary<ItemReference, ItemReference> itemDict)
    {
      var settings = new XmlReaderSettings
      {
        NameTable = doc.NameTable ?? new NameTable()
      };
      using (var reader = XmlReader.Create(stream, settings))
      {
        while (!reader.EOF)
        {
          if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Item")
          {
            var elem = (XmlElement)doc.ReadNode(reader);
            RemoveRelatedItems(elem, itemDict);
            CleanUpSystemProps(Enumerable.Repeat(elem, 1), itemDict, false);
            yield return elem;
          }
          else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Fault"
            && string.Equals(reader.NamespaceURI, "http://schemas.xmlsoap.org/soap/envelope/", StringComparison.OrdinalIgnoreCase))
          {
            var fault = (XmlElement)doc.ReadNode(reader);
            var env = doc.CreateElement("SOAP-ENV", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            var body = doc.CreateElement("SOAP-ENV", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            env.AppendChild(body);
            body.AppendChild(fault);
            var result = ElementFactory.Local.FromXml(env);
            if (!(result.Exception is NoItemsFoundException))
              result.AssertNoError();
          }
          else
          {
            reader.Read();
          }
        }
      }
    }

    /// <summary>
    /// Match system identities by name instead of ID
    /// </summary>
    private void ExpandSystemIdentities(XmlDocument doc)
    {
      ItemReference ident;
      foreach (var elem in doc.ElementsByXPath("//self::node()[local-name() != 'Item' and local-name() != 'id' and local-name() != 'config_id' and local-name() != 'source_id' and @type='Identity' and not(Item)]").ToList())
      {
        ident = _metadata.GetSystemIdentity(elem.InnerText);
        if (ident != null)
        {
          elem.InnerXml = "<Item type=\"Identity\" action=\"get\" select=\"id\"><name>" + ident.KeyedName + "</name></Item>";
        }
      }
    }
    private void EnsureTransforms()
    {
      if (_resultTransform == null)
      {
        _resultTransform = new XslCompiledTransform();
        using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("InnovatorAdmin.Export.ExportResult.xslt"))
        {
          using (var xslt = XmlReader.Create(stream))
          {
            _resultTransform.Load(xslt);
          }
        }
      }
    }
    /// <summary>
    /// Fix cyclical workflow-itemtype references by creating an edit script
    /// </summary>
    private void FixCyclicalWorkflowItemTypeRefs(XmlDocument doc)
    {
      var workflowRefs = doc.ElementsByXPath("//Item/Relationships/Item[@type='Allowed Workflow' and (@action='add' or @action='merge' or @action='create')]").ToList();
      XmlElement type;
      XmlElement sourceId;
      foreach (var workflowRef in workflowRefs)
      {
        if (!workflowRef.Elements("source_id").Any())
        {
          type = (XmlElement)workflowRef.ParentNode.ParentNode;
          sourceId = workflowRef.Elem("source_id");
          sourceId.SetAttribute("type", type.Attribute("type"));
          sourceId.SetAttribute("keyed_name", type.Element("id").Attribute("keyed_name", ""));
          sourceId.InnerText = type.Attribute("id");
        }
        workflowRef.Attr(XmlFlags.Attr_ScriptType, "3");
        type = workflowRef.Parents().Last(e => e.LocalName == "Item");
        while (type.NextSibling.Attribute("action") == "edit") type = (XmlElement)type.NextSibling;
        workflowRef.ParentNode.RemoveChild(workflowRef);
        type.ParentNode.InsertAfter(workflowRef, type);
        workflowRef.Attr(XmlFlags.Attr_IsScript, "1");
      }
    }
    /// <summary>
    /// Fix cyclical ContentType references by creating an edit script
    /// </summary>
    private void FixCyclicalContentTypeTabularViewRefs(XmlDocument doc)
    {
      var viewRefs = doc.ElementsByXPath("//Item/Relationships/Item[@type='cmf_ContentTypeView' and (@action='add' or @action='merge' or @action='create')]").ToList();
      XmlElement type;
      XmlElement sourceId;
      foreach (var viewRef in viewRefs)
      {
        if (!viewRef.Elements("source_id").Any())
        {
          type = (XmlElement)viewRef.ParentNode.ParentNode;
          sourceId = viewRef.Elem("source_id");
          sourceId.SetAttribute("type", type.Attribute("type"));
          sourceId.SetAttribute("keyed_name", type.Element("id").Attribute("keyed_name", ""));
          sourceId.InnerText = type.Attribute("id");
        }
        viewRef.Attr(XmlFlags.Attr_ScriptType, "3");
        type = viewRef.Parents().Last(e => e.LocalName == "Item");
        //while (type.NextSibling.Attribute("action") == "edit") type = (XmlElement)type.NextSibling;
        viewRef.ParentNode.RemoveChild(viewRef);
        type.ParentNode.InsertAfter(viewRef, type);
        viewRef.Attr(XmlFlags.Attr_IsScript, "1");
      }
    }
    /// <summary>
    /// Fix cyclical workflow-life cycle references by creating an edit script
    /// </summary>
    private void FixCyclicalWorkflowLifeCycleRefs(XmlDocument doc)
    {
      var workflowRefs = doc.ElementsByXPath("//Item[@type='Life Cycle State' and (@action='add' or @action='merge' or @action='create')]/workflow").ToList();
      XmlElement map;
      XmlElement fix;
      foreach (var workflowRef in workflowRefs)
      {
        fix = (XmlElement)workflowRef.ParentNode.CloneNode(false);
        fix.SetAttribute("action", "edit");
        fix.SetAttribute(XmlFlags.Attr_ScriptType, "4");
        fix.IsEmpty = true;
        fix.AppendChild(workflowRef.CloneNode(true));
        map = workflowRef.Parents().First(e => e.LocalName == "Item" && e.Attribute("type", "") == "Life Cycle Map");
        map.ParentNode.InsertAfter(fix, map);
        workflowRef.ParentNode.RemoveChild(workflowRef);
      }
    }
    /// <summary>
    /// Fix the queries for items with federated relationships by making sure that federated relationships
    /// (which might cause errors with the export) are not retrieved.  Only export real relationships
    /// </summary>
    private void FixFederatedRelationships(XmlNode query)
    {
      ItemType itemType;
      string relQuery;
      foreach (var item in query.ChildNodes.OfType<XmlElement>())
      {
        if (item.Attribute("levels") == "1"
          && _metadata.ItemTypeByName(item.Attribute("type").ToLowerInvariant(), out itemType)
          && itemType.Relationships.Any(r => r.IsFederated))
        {
          item.RemoveAttribute("levels");
          if (itemType.Relationships.Any(r => !r.IsFederated))
          {
            relQuery = (from r in itemType.Relationships
                        where !r.IsFederated
                        select string.Format("<Item type=\"{0}\" action=\"get\" />", r.Name))
                        .Aggregate((p, c) => p + c);
            item.InnerXml = "<Relationships>" + relQuery + "</Relationships>";
          }
        }
      }
    }

    /// <summary>
    /// Move all foreign properties to a script.  This is because the other properties must be created first before these can
    /// be added.
    /// </summary>
    private void FixForeignPropertyDependencies(XmlDocument doc)
    {
      var itemTypes = doc.ElementsByXPath("//Item[@type='ItemType' and Relationships/Item[@type = 'Property' and data_type = 'foreign']]").ToList();
      XmlElement fix = null;
      foreach (var itemType in itemTypes)
      {
        fix = (XmlElement)itemType.CloneNode(false);
        fix.SetAttribute("action", "edit");
        fix.SetAttribute(XmlFlags.Attr_ScriptType, "5");
        fix.IsEmpty = true;
        fix = (XmlElement)fix.AppendChild(doc.CreateElement("Relationships"));
        var uppermostItem = itemType.Parents().LastOrDefault(e => e.LocalName == "Item" && !string.IsNullOrEmpty(e.Attribute("type", ""))) ?? itemType;
        uppermostItem.ParentNode.InsertAfter(fix.ParentNode, uppermostItem);

        foreach (var foreignProp in itemType.ElementsByXPath(".//Relationships/Item[@type = 'Property' and data_type = 'foreign']").ToList())
        {
          foreignProp.Detach();
          fix.AppendChild(foreignProp);
        }
      }
    }

    /// <summary>
    /// Handle foreign properties correctly even if they are not being exported as part of a larger item type.
    /// </summary>
    private async Task FixForeignPropertyReferences(XmlDocument doc)
    {
      var props = doc.ElementsByXPath("//Item[@type='Property'][data_type='foreign']").ToList();
      foreach (var prop in props)
      {
        var dataSource = prop.Element("data_source");
        var sourceIdElem = prop.Element("source_id");
        var sourceId = sourceIdElem == null ? prop.Parent().Parent().Attribute("id") : sourceIdElem.InnerText;
        var parentId = dataSource.InnerText;
        if (parentId.IsGuid())
        {
          dataSource.IsEmpty = true;
          var dataSourceQuery = dataSource.Elem("Item").Attr("type", "Property").Attr("action", "get").Attr("select", "id");
          dataSourceQuery.Elem("source_id", sourceId);
          prop.Element("foreign_property").IsEmpty = true;
          var foreignPropQuery = prop.Element("foreign_property")
            .Elem("Item").Attr("type", "Property").Attr("action", "get").Attr("select", "id");
          foreignPropQuery.Elem("keyed_name", prop.Element("foreign_property").Attribute("keyed_name"));
          var itemTypeQuery = foreignPropQuery.Elem("source_id").Elem("Item").Attr("type", "ItemType").Attr("action", "get").Attr("select", "id");

          var refProp = prop.Parent().Elements(e => e.Attribute("type") == "Property" && e.Attribute("id") == parentId).FirstOrDefault();
          if (refProp != null)
          {
            dataSourceQuery.Elem("name", refProp.Element("name").InnerText);
            itemTypeQuery.Elem("name", refProp.Element("data_source").Attribute("name"));
          }
          else
          {
            var parentProp = (await _metadata.GetPropertiesByTypeId(sourceId).ToTask()).FirstOrDefault(p => p.Id == parentId);
            if (parentProp != null)
            {
              dataSourceQuery.Elem("name", parentProp.Name);
              itemTypeQuery.Elem("name", _metadata.ItemTypeById(parentProp.DataSource).Name);
            }
          }
        }
      }
    }

    /// <summary>
    /// Convert form fields pointing to a system property from an ID reference to a search
    /// </summary>
    private void FixFormFieldsPointingToSystemProperties(XmlDocument doc)
    {
      const string sysProps = "|behavior|classification|config_id|created_by_id|created_on|css|current_state|generation|history_id|id|is_current|is_released|keyed_name|release_date|effective_date|locked_by_id|major_rev|managed_by_id|minor_rev|modified_by_id|modified_on|new_version|not_lockable|owned_by_id|permission_id|related_id|sort_order|source_id|state|itemtype|superseded_date|team_id|";
      var fields = doc.ElementsByXPath("//Item[@type='Field'][contains($p0,concat('|',propertytype_id/@keyed_name,'|'))]", sysProps).ToList();
      if (fields.Count < 1) return;

      var query = "<Item type=\"Property\" action=\"get\" idlist=\"" + fields.GroupConcat(",", f => f.Element("propertytype_id", "")) + "\" select=\"name,source_id\" />";

      // Get all the property information from the database
      var results = _conn.Apply(query).Items().ToDictionary(e => e.Id(), e => new Command(
        @"<Item type='@0' action='get' select='id'>
            <name>@1</name>
            <source_id>@2</source_id>
        </Item>", e.Type().Value, e.Property("name").Value, e.SourceId().Value)
                .ToNormalizedAml(_conn.AmlContext.LocalizationContext));

      // Update the export the the proper edit scripts
      string propData;
      XmlDocument tempDoc;
      XmlElement propType;
      XmlElement parentItem;
      XmlElement script;
      foreach (var field in fields)
      {
        if (results.TryGetValue(field.Element("propertytype_id", ""), out propData))
        {
          propType = field.Element("propertytype_id");
          propType.RemoveAll();

          tempDoc = doc.NewDoc();
          tempDoc.LoadXml(propData);

          propType.AppendChild(propType.OwnerDocument.ImportNode(tempDoc.DocumentElement, true));
          propType.Detach();
          parentItem = field.Parents().Last(e => e.LocalName == "Item");
          script = parentItem.OwnerDocument.CreateElement("Item")
            .Attr("type", field.Attribute("type"))
            .Attr("id", field.Attribute("id"))
            .Attr("action", "edit")
            .Attr(XmlFlags.Attr_ScriptType, "6");
          script.AppendChild(propType);
          parentItem.Parent().InsertAfter(script, parentItem);
        }
      }
    }

    /// <summary>
    /// Convert references to poly itemtypes to the correct itemtype so that dependency checking works properly
    /// </summary>
    private void FixPolyItemReferences(XmlDocument doc)
    {
      var polyQuery = "//*["
        + (from i in _metadata.ItemTypes
           where i.IsPolymorphic
           select "@type='" + i.Name + "'")
          .Aggregate((p, c) => p + " or " + c)
        + "]";
      var elements = doc.ElementsByXPath(polyQuery);
      var elementsByRef =
        (from e in elements
         group e by ItemReference.FromElement(e) into newGroup
         select newGroup)
         .ToDictionary(g => g.Key, g => g.ToList());

      // Fix items referenced by ID
      var queries = (from i in elementsByRef.Keys
                     where i.Unique.IsGuid()
                     group i by i.Type into newGroup
                     select "<Item type=\""
                       + newGroup.Key
                       + "\" idlist=\""
                       + newGroup.Select(r => r.Unique).Aggregate((p, c) => p + "," + c)
                       + "\" select=\"itemtype\" action=\"get\" />");
      IEnumerable<IReadOnlyItem> items;
      List<XmlElement> fixElems;
      ItemType fullType;

      foreach (var query in queries)
      {
        items = _conn.Apply(query).Items();
        foreach (var item in items)
        {
          if (elementsByRef.TryGetValue(ItemReference.FromFullItem(item, false), out fixElems))
          {
            fullType = _metadata.ItemTypeById(item.Property("itemtype").AsString(""));
            if (fullType != null)
            {
              foreach (var elem in fixElems)
              {
                elem.SetAttribute("type", fullType.Name);
              }
            }
          }
        }
      }

      // Fix items referenced with a where clause
      var whereQueries = (from i in elementsByRef.Keys
                          where !i.Unique.IsGuid() && !string.IsNullOrEmpty(i.Unique)
                          select new
                          {
                            Ref = i,
                            Query = "<Item type=\""
                              + i.Type
                              + "\" where=\""
                              + i.Unique
                              + "\" select=\"itemtype\" action=\"get\" />"
                          });

      foreach (var whereQuery in whereQueries)
      {
        var item = _conn.Apply(whereQuery.Query).Items().FirstOrDefault();
        if (elementsByRef.TryGetValue(whereQuery.Ref, out fixElems))
        {
          fullType = _metadata.ItemTypeById(item.Property("itemtype").AsString(""));
          if (fullType != null)
          {
            foreach (var elem in fixElems)
            {
              elem.SetAttribute("type", fullType.Name);
            }
          }
        }
      }
    }

    /// <summary>
    /// Convert references to polyitem lists (which are not exported as they are auto-generated)
    /// to AML gets
    /// </summary>
    private void FixPolyItemListReferences(XmlDocument doc)
    {
      var query = "//*["
        + (from i in _metadata.PolyItemLists
           select "text()='" + i.Unique + "'")
          .Aggregate((p, c) => p + " or " + c)
        + "]";
      var elements = doc.ElementsByXPath(query);

      ItemReference itemRef;
      foreach (var elem in elements)
      {
        itemRef = _metadata.PolyItemLists.Single(i => i.Unique == elem.InnerText);
        elem.InnerXml = "<Item type='List' action='get'><name>" + itemRef.KeyedName + "</name></Item>";
      }
    }

    /// <summary>
    /// For references to versionable types that should float, mark them for floating
    /// </summary>
    private void FloatVersionableRefs(XmlDocument result)
    {
      var refs = new List<Tuple<ItemType, string>>();
      const string _needs_float = "_needs_float";

      result.VisitDescendantsAndSelf(e =>
      {
        switch (e.LocalName)
        {
          case "id":
          case "config_id":
          case "source_id":
          case "related_id":
            return;
        }

        var type = e.Attribute("type");
        ItemType itemType;

        // Find all of the elements referencing versionable item types
        if (!e.HasAttribute(XmlFlags.Attr_Float)
          && !string.IsNullOrEmpty(type)
          && _metadata.ItemTypeByName(type.ToLowerInvariant(), out itemType)
          && itemType.IsVersionable)
        {
          ItemType parent;
          switch (e.LocalName)
          {
            case "Item":
              // For item tags that are float properties, or root level item tags, set them to float
              if (e.Parent().Parent() != null
                && e.Parent().Parent().LocalName == "Item"
                && e.Attribute("action", "") != "get")
              {
                if (_metadata.ItemTypeByName(e.Parent().Parent().Attribute("type").ToLowerInvariant(), out parent)
                  && parent.FloatProperties.Contains(e.Parent().LocalName))
                {
                  e.SetAttribute(XmlFlags.Attr_Float, "1");
                  e.SetAttribute("id", e.Element("config_id", e.Attribute("id")));
                }
              }
              else
              {
                e.SetAttribute(XmlFlags.Attr_Float, "1");
                e.SetAttribute("id", e.Element("config_id", e.Attribute("id")));
              }
              break;
            default:
              // For item properties (with no Item tag) set to float, make sure they float
              if (!e.Elements().Any()
                && e.Parent().LocalName == "Item"
                && _metadata.ItemTypeByName(e.Parent().Attribute("type").ToLowerInvariant(), out parent)
                && parent.FloatProperties.Contains(e.LocalName))
              {
                refs.Add(Tuple.Create(itemType, e.InnerText));
                e.SetAttribute(_needs_float, "1");
              }
              break;
          }
        }
      });

      var queries = (from r in refs
                     group r by r.Item1 into g
                     select "<Item type=\"" + g.Key.Name + "\" action=\"get\" select=\"config_id\" idlist=\""
                              + g.Select(i => i.Item2).Distinct().Aggregate((p, c) => p + "," + c)
                              + "\" />");
      var resultItems = queries.SelectMany(q => _conn.Apply(q).Items())
                               .ToDictionary(e => ItemReference.FromFullItem(e, false), e => e.ConfigId().Value ?? "");
      result.VisitDescendantsAndSelf(e =>
      {
        string configId;
        if (e.HasAttribute(_needs_float) && resultItems.TryGetValue(ItemReference.FromItemProp(e), out configId))
        {
          e.RemoveAttribute(_needs_float);
          e.SetAttribute(XmlFlags.Attr_Float, "1");
          e.InnerText = configId;
        }
      });
    }

    private static IEnumerable<InstallItem> GetDependencyList(DependencyAnalyzer dependAnalyzer
      , IEnumerable<InstallItem> values, out CycleState cycleState)
    {
      cycleState = CycleState.NoCycle;
      var lookup = (from i in values
                    group i by i.Reference into refGroup
                    select refGroup)
                  .ToDictionary(i => i.Key, i => (IEnumerable<InstallItem>)i);

      IEnumerable<ItemReference> sorted = null;
      IList<ItemReference> cycle = new List<ItemReference>() { null };
      List<XmlNode> refs;

      // Bias the install order initially to try and help the dependency sort properly handle anything
      // where explicit dependencies don't exist.  Then, perform the dependency sort.
      var initialSort = lookup.Keys
        .OrderBy(DefaultInstallOrder)
        .ThenBy(i => i.Type.ToLowerInvariant())
        .ThenBy(i => i.Unique)
        .ToList();
      sorted = initialSort.DependencySort<ItemReference>(d =>
      {
        IEnumerable<InstallItem> res = null;
        if (lookup.TryGetValue(d, out res))
        {
          return res.SelectMany(r =>
          {
            var ii = r as InstallItem;
            if (ii == null) return Enumerable.Empty<ItemReference>();
            return dependAnalyzer.GetDependencies(ii.Reference);
          });
        }

        return Enumerable.Empty<ItemReference>();
      }, ref cycle, false);

      // Attempt to remove cycles by identifying a Relationships tag in one of the cycles
      // and moving the Relationships to the top level
      if (cycle.Count > 0 && (cycle[0] != null || cycle.Count > 1))
      {
        cycleState = CycleState.UnresolvedCycle;
        for (int i = (cycle[0] == null ? 2 : 1); i < cycle.Count; i++)
        {
          refs = dependAnalyzer.GetReferences(cycle[i - 1], cycle[i]).ToList();
          if (refs.Count == 1)
          {
            var relTag = refs[0].Parents().FirstOrDefault(e => e.LocalName == "Relationships");
            if (relTag != null)
            {
              var parentTag = refs[0].Parents().Last(e => e.LocalName == "Item").Parent();
              foreach (var child in relTag.Elements().ToList())
              {
                child.Detach();
                parentTag.AppendChild(child);
                var sourceId = (XmlElement)child.AppendChild(child.OwnerDocument.CreateElement("source_id"));
                sourceId.SetAttribute("type", relTag.Parent().Attribute("type"));
                sourceId.SetAttribute("keyed_name", relTag.Parent().Attribute("_keyed_name"));
                sourceId.InnerText = relTag.Parent().Attribute("id");
              }
              relTag.Detach();
              cycleState = CycleState.ResolvedCycle;
              return Enumerable.Empty<InstallItem>();
            }
          }
        }
      }

      var result = new List<InstallItem>();
      IEnumerable<InstallItem> items = null;
      foreach (var sort in sorted)
      {
        if (lookup.TryGetValue(sort, out items))
        {
          foreach (var item in items)
            result.Add(item);
        }
        else
        {
          result.Add(InstallItem.FromDependency(sort));
        }
      }

      return result;
    }

    private static int DefaultInstallOrder(InstallItem line)
    {
      var itemRef = line.Reference;
      if (line.Reference.Type == "*Script")
        itemRef = ItemReference.FromElement(line.Script);
      return DefaultInstallOrder(itemRef);
    }

    private static HashSet<string> _coreItemTypeIds = new HashSet<string>()
    {
      "483228BE6B9A4C0E99ACD55FDF328DEC",
      "937CE47DE2854308BE6FF5AB1CFB19D4",
      "7A3EFE7242DB4403965890C053A57A0B",
      "6B9057491021453ABA0A425570CC10D2",
      "E8335FB6E4834AFF9BA8F9D4C6E3DE2C",
      "1DD844998D3941D1BB8680612D9B4B0C",
      "5D9A5583B5A54CA78586FDBF9716E53A",
      "FC1A83F673B542C7AC8EEC29F25C602E",
      "EFC5E36233D541FDA5B23B7BD374653C",
      "D003393AB1E447B3A8D3B274CAA70F69",
      "3538F62649F3477EA1F8990EB20F88B9",
      "1675E79E847F4D3E9408B57EE7DA69EF",
      "B228811F241240FE8962E14AD584F2AD",
      "94E345F15EB94D86ADE8FF1B6AE2B439",
      "550DEE262619410A82F8F7378D6184D0",
      "D3F830490AB143E4ACC00D16538C6DB0",
      "2C95B39863F346EFB224DCB85390F465",
      "7BFF05B795FA4C1293C0B68C282F78F1",
      "BF62E8B6450447D89EF86B539F49F992",
      "31392C4909A249BF9EA143915F3F9553",
      "B5175846B27145EEA5653DA35ED78BE4",
      "51F1815885E3477D8B6385A69236C5AA",
      "A16A77E9E9B84559AA435361BAB057F7",
      "6DF95CF17D824F5DAC6A996FFC34D5F8",
      "837D345236384B5DAE4B52042419F966",
      "05DF56FF833542F98251528F3FFE2FA0",
      "02FA838247DF47C2BB85AAB299E646B2",
      "0300B828CBEE4610B77C41377209C900",
      "0DE14E76AA794A039DA8D2CDC34E6B1D",
      "645774D6072F41FD8F998C861E211741",
      "4DAD707F62B54823AE2E4730BB00C649",
      "09586A35D3534D5EAD92B94CB1036AEF",
      "F91DE6AE038A4CC8B53D47D5A7FA49FC",
      "CE0C4143D35E46CDA3874C4339F159BE",
      "8052A558B9084D41B9F11805E464F443",
      "41EF49EFD2ED4F6EAB04C047681F33AC",
      "45F28B3088E74905913152E9BE3B9B12",
      "68D1EEEAA8B84B75962D3008C00E2280",
      "2A69EFFD543B4F74AB5AE964FE83203D",
      "B8940FB948604232B27FC1263FC7E203",
      "13EC84A626F1457BB5F60A13DA03580B",
      "47573682FB7549F59ADECD4BFE04F1DE",
      "2E25B49E218A45D28D0C7D3C0633710C",
      "8AB5CEB9824B4CF7920AFED29F662C66",
      "8EABD91B465443F0A4995418F483DC51",
      "1718DC9FB25043B9A3F0B76DB5DC6637",
      "B2D9F59116B944CE9151C7C05F4D946C",
      "06E0660816FE40A2BF1411B2280062B3",
      "BA053C68D62E4E5293039323F10E116E",
      "38CAFF4302AC45EBAF91EE4DCE4948C7",
      "E582AB17663F4EF28460015B2BE9E094",
      "BC7977377FFF40D59FF14205914E9C71",
      "450906E86E304F55A34B3C0D65C097EA",
      "030019FA30FE40FEB5E32AD2FC9B1F20",
      "AC32527D85604A4D9FC9107C516AEF47",
      "5736C479A8CB49BCA20138514C637266",
      "137D24DFD9AC4D0CA2ABF8D90346AABB",
      "F8F19489113C487B860733E7F7D5B12D",
      "87879A09B8044DE380D59DF22DE1867F",
      "1C5BA0A1491843378E48FF481F6F1DF1",
      "0B9D641B40D24036A117D911558CBDCE",
      "F312562D6AD948DCBCCCCF6A615EE0EA",
      "C6A89FDE1294451497801DF78341B473",
      "7C63771EBC8D46FE8E902C5188033515",
      "AD09B1279AC246BB9EE39BD153D28586",
      "80881F5852BC439E9F3CF0AEC03ABE2A",
      "2DAC2B407B0043A692905CF6A94296A8",
      "98FF7C1BFDFA43448B1EC5A95EA13AEA",
      "F0834BBA6FB64394B78DF5BB725532DD",
      "F81CDEF9FE324D01947CC9023BC38317",
      "18C15AB147F84834874F2E0CB6B8B4C0",
      "E5BC8090E82D4F8D8E3F389C95316433",
      "DA581F2EAD1641CF976A1F1211E1ADBA",
      "B5048D604A6A4D53B3FC6C3BF3D81157",
      "BB3394EA2B014A6493267E7867B4ABD7",
      "920AAA3EAE684F6E99A0EAB95516D064",
      "A46890D3535C41D4A5D79240B8C373B0",
      "DF17056D3AC1479DBC7196255105D04B",
      "2B46201802CE46708C269667DB4798AC",
      "42EA5C9FEC6B49CD8E2784E9E846EAFE",
      "E5326B5A2B93464A9795B1F8A6E6B666",
      "FC3E32F18F804FD9BE4B175973D29112",
      "B7DF834246F24F10BC9B91056D828538",
      "E7A68175B2024FFB876E87D0081071A9",
      "B30032741C894BB086148DDB551D3BEE",
      "2F4E2B53BFBA4351BFFCEE0E438ECF97",
      "FF53A19A424D4B2F80938A5A5C1A29EA",
      "83EBCDAE9D834E169ABC95CC0C7CCB28",
      "CC23F9130F574E7D99DF9659F27590A6",
      "63EC2E6B69FC4FB09859077EE073D9A5",
      "ED9F61ADA4334D7D94361F426C081DB5",
      "E4A23B0AC84D4155BF4C1E44B84CBD45",
      "DE828FBA99FF4ABB9E251E8A4118B397",
      "DD54C11BF6004B09A9E152AFD61ABEA9",
      "45E899CD2859442982EB22BB2DF683E5",
      "122BF06C8B8E423A9931604DD939172F",
      "6DAB4ACC09E6471DB4BDD15F36C3482B",
      "8FC29FEF933641A09CEE13A604A9DC74",
      "602D9828174C48EBA648B1D261C54E43",
      "B19D349CC6FC44BC97D50A6D70AE79CB",
      "261EAC08AE9144FC95C49182ACE0D3FE",
      "321BD822949149C597FD596B1212B85C",
      "EDC5F1D5759D4C7CBDC7F8C20D76087D",
    };

    private static HashSet<string> _coreRelationshipTypeIds = new HashSet<string>()
    {
      "AEFCD3D2DC1D4E3EA126D49D68041EB6",
      "85924010F3184E77B24E9142FDBB481B",
      "90D8880C29CD45C3AA8DAFF1DAEBC60E",
      "BF3EAD31AAA2403592CAAC2446FF7797",
      "46BDE53304404C28B5C45610E41C1DD5",
      "BD4A250787A742A484C7B174A4AED1E2",
      "96C238700CD840DEBE512EE85D440AF3",
      "67735DF455F54736A9D51CB53AB129E3",
      "05A68B0BC74C47A6A2FD4404A73C815F",
      "E32DB4E6E3B64F98A12B550C578E6A01",
      "F88A91D29C4F446BB309BEEE925AADD1",
      "FA1755A31ACF4EDFBBFFCD6A8C6F7AF8",
      "EB4ADB2BC83C410FB265CB42ED5C633B",
      "8B58025F8E504DBDADF0E1176D3CE178",
      "CB7696CC33EC4D1BB98716465F1AD580",
      "4E355E04444B4676AE723B43DECA37DC",
      "DB54505FA3E9419DA3C1E1AFB7A48C1C",
      "10B8BB84EEE9413AAD071C8341BBAB04",
      "BF60433C7E924BE6B78D901809F8FEF6",
      "3A65F41FF1FC42518A702FDA164AF420",
      "6EA4299D271743BFB50DBB14C08AC55B",
      "432E29895A994D0DBC9DF9B0918E189F",
      "421A0EC1E68C4661AA9274C297BD410C",
      "D4C8D8008DAE4799A426518C2B6D0889",
      "4E24BC6E89394C8D8D05D3F9871EA5D8",
      "103F7DC6DDEA415389587AC6D37C135C",
      "21E1EF3C68744A53BF5FAC205A1B51CF",
      "8EC6FCD4C5344652A2C751023B66B889",
      "7FAFAF8FFCE143A0985010F83DACDA88",
      "0CFC2324C1A141C59B2B4612442D0433",
      "64DA1324EE1D4A6FA84783D93CEE0EA2",
      "91D179E2AC9C4BDA92E0CB877D45E051",
      "A008CBA8749B405C9A3FE02904EBA067",
      "DC80041DD3544835BDB50A8CAA535903",
      "692D9D11B6524E54BC96AE8244EE6AE3",
      "9CB248B6DD5F4EBD9D56A75B30DBEBCB",
      "B5C980B0A6494F1FBEDE80F96F96BEBE",
      "1F5F9158F3ED435CAD9757BCFA3A4453",
      "8F3CDD9B7F8B4B8FB6A35BB39DED6EC1",
      "C779CBB024A04E519D2806435882F7F9",
      "0EA142227FAE40148A4BDD8CF1D450EF",
      "621D9BA267174927B0E6A08601BDCF08",
      "8E7749E9F3C84E11A5F7F5E4E7D51B22",
      "17F8471D9F0346379B33D94E3A90689B",
      "52E7E19747864AB5900BBABF82E66382",
      "617069E6F5C04687B0526750978CFA51",
      "39C52F7B48ED4CEA824196BE9DBE6784",
      "E45E85D702F14A37B0E65F3C770D1195",
      "C5283EF5760B424C974E2E2F81CE02CE",
      "6733DCABB12E49E7A1FA6A76863DDA95",
      "70E817CC22644F538032177DEC7B6ECD",
      "95D6B96F155543CF86ED9CC9989A1059",
      "0BDF3A9BA8D94A18A016541653EA9097",
      "241944ED565940B5BC4987C3D9EAB6C7",
      "91ABEDE7189D4F509795396D3C646ADA",
      "D3F7714E036040BA8B24F0B8F5601452",
      "D3E6C97153E1408FB218FA34D8B65A3D",
      "CB10A07383CC4C35A22B1577B7712D38",
      "789104C736664FAA9748352CBBF86BCA",
      "3E85D9DD379643F8A207B99A9DFB72C2",
      "9C0541DDA9644E4EBA9CBC68BBD37C52",
      "C64BA744050149A1A32F402ED965CB1A",
      "ED06F0AEA37242349F2C499B7FEA6A26",
      "1549FB139D6B4AD99FDC1ED4B8011DB9",
      "721F8FFCBA8448D8BA61E9AD980A52C1",
      "4697179F1FC94E25A8274E586EEF2F39",
      "0BB5B81FEB37475BB9C779408080DB61",
      "3572C4D1479445D9959C413624A9FFF6",
      "EA473F6BF22F4F34807F2FA03497147F",
      "F03364FA841641EF82C8D893CA2C6727",
      "EEC427EA9E754FD9BE93D1C6C72F2F2E",
      "92BC895A020E452D926B6F64E985A197",
      "15FA7E0A9F2A41269B8B6CF7C52BD11D",
      "BCC8053D365143A18B033850EFE56F3C",
      "96730C617F4C40729D730D97395FD620",
      "2578D0D1427A4FE384544CA498301E40",
      "0A871CCD40364A03AD2F0FD24AEAB4E5",
      "640C685B4AE14A239A138C5A8876A1C9",
      "A7C8B9475B884B9593E3B9ED3F3B828C",
      "3844E4C9C5FA4ABAB3034FD6D4BE596E",
      "19193BC7942C41AEBA8147BD1778F35E",
      "AC218DF3AE43488C9E64E1AA551D2522",
      "CA289ED4F1A84A9EB6CDD822846FD745",
      "C24A87B33E3740B3B01254DC776F1EFE",
      "E2DECCA6300E4815B466C62C66E9D3AF",
      "ACD5116613FF40C9BC5EF7447CBEBBCB",
      "213B74E721ED457C9BE735C038C7CB95",
      "B70DE4074ABC49C69B0D8729D9212982",
      "6323EE2C82E94CC4BB83779DDFDDD6F5",
      "EECAA9639E054096B4A0F98E6845E963",
      "74F9BBD18C9143AE8547D078C9FAB456",
      "B1B179DB316A4178B0A5A57F00185A27",
      "E1F075B555A24EA8BF0E945CE580254F",
      "6C280692633D4498BD9CFEA7989138AB",
      "5BD6BED0CD794A078AA42476F47ECF46",
      "A2294A4CD0B14DE4912C1B530218AB57",
      "797EE3B1B9624E369B3E45FA0C10757C",
      "5EFB53D35BAE468B851CD388BEA46B30",
      "1E764495A5134823B30060D83FD6A2F9",
      "5698BACD2A7A45D6AC3FA60EAB3E6566",
      "8EF041900FE2428AABD404063AB979B6",
      "7348E620D27E40D1868C54247B5DE8D1",
      "E0849D6A48B84B8D9C135488C4C1DDF4",
      "A7AE79CD9E144A15B5B34B455AFB66D9",
      "B2A302B6DCD54F92AD0912A37A1D4850",
      "5ED4936039F64E9E99F703F8F46A1DB1",
      "E4F3367A4A6C4956875CAEF580692564",
      "5D6C6E00F2114D678570FC877B6F44BC",
      "D1AEF724041942BCAAC8CADBBDFD5EF4",
      "51CE2CDC8D3F49C7BF707B8E0A4BD15F",
      "CFACA4085F044622A1D4E00F7DB9C71E",
      "0EB9440796664EEBBAD9BE8B4DAE660F",
      "5CDF0A9CB3FC49BEB83D361B4CEE4082",
      "929D27BAADFE4F2F9194EF8E29B8B869",
      "26D7CD4E033242148E2724D3D054B4D3",
      "8CFAF78BCFFB41E6A3ED838D9EC2FD7C",
      "FF5EFC69BB8F4E56839A43A8477AE58F",
      "C57EC6AAFE22490082F06FD8DCBE2E1C",
      "471932C33B604C3099070F4106EE5024",
      "A274904C583C4290BB734B7F1875AB82",
      "9501CD77346746B39A98460EF44376D2",
      "A93B9C603CAA498AA19454C75B0826F8",
      "A1B184A401B546EEB7D09CDD35C47911",
      "D2ECF1B2A7FC45DB9D916996CE9BE9C9",
      "F0A8BC4265E44C47A127AEC1975F4C89",
      "56C80A4B7A774848824BA91BBB1065D5",
      "FA745E2F3D0B406FA6C0F3ECE4C5F5D4",
      "13558E4C883D4E1788160C87AB6F61BC",
      "9F7E6D76DB664D44AE13769DBF007571",
      "9344B56F71AA4BE1BB0A5DB6A75825B8",
      "9FC5E87F173747AB840D966D4238290C",
      "475E03B3E64740C5856F6E33EE8301FA",
      "8214ECDE53F04AFC95243E10B2C7BBD4",
      "4625FCFBF49747ACA31F31FA3101F1D5",
      "A3605A9B20E74483A7F4B8EF64AB72D6",
      "3D7E7F7C1487472A9690CDD361C6B26F",
      "D2794EA7FB7B4B52BA2CE4681E2D9DFB",
      "F0B2EAE5414249748F2986CC1EE78340",
      "F0CC0E7AEC0A401A94E8A63C9AC4F4D3",
      "73B2E56B742C40398F649727233DBD87",
      "B3441B45957C4BDFABBC2EA1E37FD31D",
      "D9520A9B3DEA453CB8F6A3A5CA1C9FEB",
      "C455246EFC1C402296B1C2249D00B04D",
      "3C3A0C482534454884151CCD97FCD561",
      "38C9CE2A4E06401DABF942E1D0224E87",
      "F356C4CED1584EBF812912F2D926066B",
      "E5B978ACDB914BA0AE53ED501B6F2600",
      "C3A6C9422EB64F5A8B9F82C4AE4FC928",
      "2F80CC4282644DDEB6DD5E57F6BDF9C1",
      "FBA0C040106A44CA93F48A6E112FB14E",
      "46A05975EE8A43DB82BA9D6C477D5756",
      "AABF8B3AD8AC4839BF103B1BFB3E9473",
      "5804B3F85FAB49B6B0B8B2686F4F5512",
      "1DA22B3D6F12458290F8549165B490EC",
      "8E5FA57F5168436BA998A70CB2C7F259",
      "8651DCAB4D714EF6AA747BB8F50719BA",
      "8EFCE40BCB74478B8254CEB594CE8774",
      "2D700440AC084B99AD123528BAE67D29",
      "9E212D4ED3C64493B631EE15D0A62AF7",
      "42A80AD3F88443F785C005BAF2121E01",
      "97F00180EC8442B3A1CB67E6349D7BDE",
      "AE7AC22E64D746B69F970EA1EC65DB05",
      "7937E5F0640240FDBBF9B158F45F4F6C",
      "FE597F427BF84EC783435F4471520403",
      "F3F07BFCCDDF48E79ED239F0111E4710",
      "34682D3EB66141ECACC8796C9D3A42B8",
      "4245947FE6244E37982F46D2FA46D74E",
      "30F30E1181BA43FE99706038200EFEBF",
      "40F45DEED2D84BA58B223F556FA25617",
      "DA0772F930654164AEC517CC6CDC5DBA",
      "3D1EF44A78D34058A32CB1C658AE90FE",
      "DCF6BD55DE71421CA49C6FF4F3B2D1FC",
      "EED63D7ABCB24E7E9AC1C4AA7C3ADC40",
      "D98433ED30FF48CDA8D2A84E846EC2DF",
    };

    // Ids for the property and itemtype item types
    private static HashSet<string> _dataModelIds = new HashSet<string>()
    {
      "D98433ED30FF48CDA8D2A84E846EC2DF",
      "26D7CD4E033242148E2724D3D054B4D3",
      "450906E86E304F55A34B3C0D65C097EA"
    };

    private static int DefaultInstallOrder(ItemReference itemRef)
    {
      switch (itemRef.Type)
      {
        case "List": return 1;
        case "Sequence": return 2;
        case "Revision": return 3;
        case "Variable": return 4;
        case "Method": return 5;
        case "Identity": return 6;
        case "Member": return 7;
        case "User": return 8;
        case "Permission": return 9;
        case "EMail Message": return 10;
        case "Action": return 11;
        case "Report": return 12;
        case "Form": return 13;
        case "Workflow Map": return 14;
        case "Life Cycle Map": return 15;
        case "Grid": return 16;
        case "ItemType":
          // Prioritize core types (e.g. ItemType, List, etc.)
          if (_dataModelIds.Contains(itemRef.Unique))
            return -20;
          else if (_coreItemTypeIds.Contains(itemRef.Unique))
            return -10;
          return 17;
        case "RelationshipType":
          // Prioritize core metadata types (e.g. Member)
          if (_dataModelIds.Contains(itemRef.Unique))
            return -20;
          else if (_coreRelationshipTypeIds.Contains(itemRef.Unique))
            return -10;
          return 18;
        case "Field": return 19;
        case "Property": return 20;
        case "View": return 21;
        case "SQL": return 22;
        case "Metric": return 23;
        case "Chart": return 24;
        case "Dashboard": return 25;
        case "*Script":
          if (itemRef.Unique.Split(' ').Concat(itemRef.KeyedName.Split(' ')).Any(p => _dataModelIds.Contains(p)))
            return -15;
          if (itemRef.Unique.Split(' ').Concat(itemRef.KeyedName.Split(' ')).Any(p => _coreItemTypeIds.Contains(p) || _coreRelationshipTypeIds.Contains(p)))
            return -5;
          return 50;

      }
      return 99;
    }


    /// <summary>
    /// Move the form nodes inline with the itemtype create script
    /// </summary>
    private void MoveFormRefsInline(XmlDocument doc, IEnumerable<InstallItem> lines)
    {
      ItemReference formItemRef = null;

      foreach (var form in doc.ElementsByXPath("//Item[@type='Form' and @action and @id]").ToList())
      {
        var references = doc.ElementsByXPath(".//related_id[@type='Form' and not(Item) and text() = $p0]", form.Attribute("id", ""))
          //.Concat(otherDocs.SelectMany(d => d.ElementsByXPath("//self::node()[local-name() != 'Item' and local-name() != 'id' and local-name() != 'config_id' and @type='Form' and not(Item) and text() = $p0]", form.Attribute("id", "")))).ToList();
          .ToList();
        if (references.Any())
        {
          foreach (var formRef in references)
          {
            formRef.InnerXml = form.OuterXml;
            foreach (var relTag in formRef.Elements("Item").Elements("Relationships").ToList())
            {
              relTag.Detach();
            }
          }
        }

        foreach (var line in lines)
        {
          references = line.Script.ElementsByXPath(".//related_id[@type='Form' and not(Item) and text() = $p0]", form.Attribute("id", "")).ToList();
          if (references.Any())
          {
            if (formItemRef == null) formItemRef = ItemReference.FromFullItem(form, false);

            foreach (var formRef in references)
            {
              formRef.InnerXml = form.OuterXml;
              foreach (var relTag in formRef.Elements("Item").Elements("Relationships").ToList())
              {
                relTag.Detach();
              }
            }

            _dependAnalyzer.RemoveDependency(line.Reference, formItemRef);
          }

        }
      }
    }

    /// <summary>
    /// Normalize the formatting of class structure nodes to aid in manual line-based diff-ing
    /// </summary>
    private void NormalizeClassStructure(XmlDocument doc)
    {
      ReportProgress(97, "Transforming the results");
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      var classStructs = (from e in doc.ElementsByXPath("//Item[@type='ItemType']/class_structure")
                          where !string.IsNullOrEmpty(e.InnerText)
                          select e);
      XmlDocument classDoc;
      foreach (var classStruct in classStructs)
      {
        classDoc = doc.NewDoc();
        var text = classStruct.InnerText;
        classDoc.LoadXml(text);
        var sb = new StringBuilder((int)(text.Length * 1.4));
        using (var output = new StringWriter(sb))
        {
          using (var writer = XmlTextWriter.Create(output, settings))
          {
            WriteClassStruct(Enumerable.Repeat((XmlNode)classDoc.DocumentElement, 1), writer);
          }
          classStruct.IsEmpty = true;
          classStruct.AppendChild(doc.CreateCDataSection(output.ToString()));
        }
      }
    }
    /// <summary>
    /// Remove keyed_name attributes to make diffing easier
    /// </summary>
    private void RemoveKeyedNameAttributes(XmlElement node)
    {
      if (node.HasAttribute("keyed_name"))
        node.RemoveAttribute("keyed_name");
      foreach (var elem in node.Elements())
        RemoveKeyedNameAttributes(elem);
    }

    /// <summary>
    /// Remove the calculated vault URL
    /// </summary>
    private void RemoveVaultUrl(XmlDocument doc)
    {
      var query = "//Item[@type='Vault']/vault_url";
      var elements = doc.ElementsByXPath(query).ToList();
      foreach (var elem in elements)
      {
        elem.Parent().RemoveChild(elem);
      }
    }

    /// <summary>
    /// Remove the ID attribute from relationships from versionable items.  This will improve the
    /// compare process and the import process should handle the import of these items without an
    /// ID.
    /// </summary>
    private void RemoveVersionableRelIds(XmlDocument doc)
    {
      var query = "//Item["
        + (from i in _metadata.ItemTypes
           where i.IsVersionable
           select "@type='" + i.Name + "'")
          .Aggregate((p, c) => p + " or " + c)
        + "]/Relationships/Item[@id]";
      var elements = doc.ElementsByXPath(query).ToList();
      foreach (var elem in elements)
      {
        elem.RemoveAttribute("id");
      }
    }

    /// <summary>
    /// Set <c>doGetItem="0"</c> for cmf_ContentType elements
    /// </summary>
    private void SetDoGetItemForCmf(XmlDocument doc)
    {
      var elements = doc.Descendants(e => e.Attribute("type", "") == "cmf_ContentType")
        .ToArray();
      foreach (var elem in elements)
      {
        elem.Attr("doGetItem", "0");
      }
    }

    private void RemoveGeneratedItemTypeRelationships(XmlDocument doc)
    {
      var elemsToRemove = doc.ElementsByXPath("//Relationships/Item[@type='Client Event' and related_id[@keyed_name='cmf_OnShowContainer']]").ToList();
      foreach (var elem in elemsToRemove)
      {
        elem.Detach();
      }
    }

    private void RemoveXPropertyFlattenRelationships(XmlDocument doc)
    {
      var elemsToRemove = doc.ElementsByXPath("//Item[@type='xClass_xProperty_Flatten']").ToList();
      foreach (var elem in elemsToRemove)
      {
        elem.Detach();
      }
    }

    /// <summary>
    /// The CMF generated types will get recreated by Aras.  Don't export them here.
    /// </summary>
    private void RemoveCmfGeneratedTypes(XmlDocument doc)
    {
      foreach (var element in doc.Descendants(x => x.Attribute("type", "") == "cmf_ElementType" || x.Attribute("type", "") == "cmf_PropertyType"))
      {
        var generatedTypeProps = element.Descendants(e => e.LocalName == "generated_type")
        .ToArray();
        foreach (var generatedTypeProp in generatedTypeProps)
        {
          var itemType = generatedTypeProp.Elements().Single();
          var parentItem = generatedTypeProp.Parents().Last(e => e.LocalName == "Item");
          itemType.Attr("action", "merge");
          itemType.Attr("where", "[ItemType].name = '" + itemType.Element("name").InnerText + "'");
          itemType.RemoveAttribute("id");
          itemType.Element("id").Detach();
          itemType.Attr("_cmf_generated", "1");
          itemType.Attr("keyed_name", generatedTypeProp.Attribute("keyed_name", ""));
          // The following referenceElement is for dependency sorting
          var referenceElement = itemType.Elem("___cmf_content_type_ref___").Attr("type", parentItem.Attribute("type"));
          referenceElement.InnerText = parentItem.Attribute("id");

          itemType.Detach();
          parentItem.ParentNode.InsertAfter(itemType, parentItem);
          generatedTypeProp.Detach();
        }
      }

      var elems = doc.Descendants(e => e.Attribute("type", "") == "RelationshipType"
        && e.Elements(x => x.LocalName == "relationship_id"
        && x.Attribute("type", "") == "ItemType"
        && _metadata.CmfGeneratedTypes.Contains(x.InnerText)).Any()
      ).ToArray();

      foreach (var elem in elems)
      {
        elem.Detach();
      }
    }

    /// <summary>
    /// The CMF computed property dependencies are dependent on inter-item properties. Extract and insert them later.
    /// </summary>
    private void FixCmfComputedPropDependencies(XmlDocument doc)
    {
      var existingComputedDeps = doc.ElementsByXPath("/Result/Item[@type='cmf_ComputedPropertyDependency']").Select(x => x.Attribute("id", "")).ToList();
      foreach (var contentType in doc.ElementsByXPath("/Result/Item[@type='cmf_ContentType']"))
      {
        var computedDependencies = contentType.Descendants(e => e.LocalName == "Item" && e.Attribute("type", "") == "cmf_ComputedPropertyDependency")
          .ToArray();
        foreach (var computedDep in computedDependencies)
        {
          var computedDepID = computedDep.Attribute("id", "");
          if (existingComputedDeps.Contains(computedDepID))
          {
            computedDep.Detach();
            continue;
          }
          var parentItem = computedDep.Parents().Last(e => e.LocalName == "Item");
          computedDep.Attr("action", "merge");
          computedDep.Detach();
          parentItem.ParentNode.InsertAfter(computedDep, parentItem);
          existingComputedDeps.Add(computedDepID);
        }
      }
    }

    /// <summary>
    /// The CMF TabularView Item is expanded by default, need to replace with only related_id. This will allow processor to notice possible missing items.
    /// </summary>
    private void FixCmfTabularViewMissingWarning(XmlDocument doc)
    {
      foreach (var contentType in doc.ElementsByXPath("/Result/Item[@type='cmf_ContentType']"))
      {
        var relatedBaseViews = contentType.Descendants(e => e.LocalName == "related_id" && e.Parent().Attribute("type", "") == "cmf_ContentTypeView"
        && (e.Attribute("type", "") == "cmf_BaseView" || e.Attribute("type", "") == "cmf_TabularView"))
          .ToArray();
        foreach (var relatedBaseView in relatedBaseViews)
        {
          var tabularView = relatedBaseView.Element("Item");
          relatedBaseView.InnerText = tabularView.Attribute("id", "");
          relatedBaseView.Attr("type", tabularView.Attribute("type", ""));
          tabularView.Detach();
        }
      }
    }

    /// <summary>
    /// Remove empty &lt;Relationship/&gt; tags that weren't dealt with by the xslt template
    /// </summary>
    private void RemoveEmptyRelationshipTags(XmlDocument doc)
    {
      var elements = doc.Descendants(e => e.LocalName == "Relationships" && e.IsEmpty).ToArray();
      foreach (var elem in elements)
      {
        elem.Detach();
      }
    }

    /// <summary>
    /// Reorder the Relationship tags in cases where the ItemTypes are not intrinsically sorted on
    /// gets (e.g. the Property Item Type is intrinsically sorted).
    /// </summary>
    private void SortRelationshipTags(XmlDocument doc)
    {
      var unsortedTypes = new HashSet<string>(_metadata.ItemTypes.Where(i => !i.IsSorted).Select(i => i.Name));
      var elements = doc.Descendants(e =>
        e.LocalName == "Relationships"
        && e.Elements().HasMultiple(c => c.HasAttribute("id")
          && c.HasAttribute("type")
          && unsortedTypes.Contains(c.Attribute("type")))).ToList();
      foreach (var elem in elements)
      {
        var children = elem.Elements().Select((e, i) => new
        {
          Element = e,
          Type = e.Attribute("type"),
          SortKey = e.HasAttribute("id") && e.HasAttribute("type")
            && unsortedTypes.Contains(e.Attribute("type"))
            ? e.Attribute("id") : i.ToString("D5")
        })
        .OrderBy(e => e.Type)
        .ThenBy(e => e.SortKey)
        .ToArray();
        foreach (var child in children)
        {
          child.Element.Detach();
        }
        foreach (var child in children)
        {
          elem.AppendChild(child.Element);
        }
      }

      // Sort list values by the text of the value instead of the sort order to ease merging
      var sortByValueTypes = new HashSet<string>(new string[] { "Value", "Filter Value" });
      elements = doc.Descendants(e =>
        e.LocalName == "Relationships"
        && e.Elements().HasMultiple(c => c.HasAttribute("id")
          && c.HasAttribute("type")
          && sortByValueTypes.Contains(c.Attribute("type")))).ToList();
      foreach (var elem in elements)
      {
        var children = elem.Elements().Select((e, i) => new
        {
          Element = e,
          Type = e.Attribute("type"),
          SortKey = e.HasAttribute("id") && e.HasAttribute("type")
            && sortByValueTypes.Contains(e.Attribute("type"))
            ? (e.Element("value", null) ?? e.Attribute("id")) : i.ToString("D5")
        })
        .OrderBy(e => e.Type)
        .ThenBy(e => e.SortKey)
        .ToArray();
        foreach (var child in children)
        {
          child.Element.Detach();
        }
        foreach (var child in children)
        {
          elem.AppendChild(child.Element);
        }
      }
    }



    /// <summary>
    /// Remove related items from relationships to non-dependent itemtypes (they should be exported separately).
    /// </summary>
    /// <remarks>
    /// Floating relationships to versionable items are flagged to float.
    /// </remarks>
    private void RemoveRelatedItems(XmlNode doc, IDictionary<ItemReference, ItemReference> itemDict)
    {
      ItemReference itemRefOpts;
      ItemType itemType;
      List<XmlElement> parents;
      int levels;

      // get the related_id of the current relationship (if it is a relationship) along with the
      // related_id inside any Relationships tags
      var relateds = doc.ElementsByXPath("./related_id[Item/@id!='']")
        .Concat(doc.ElementsByXPath(".//Relationships/Item/related_id[Item/@id!='']"))
        .ToList();

      foreach (var elem in relateds)
      {
        parents = elem.Parents().Where(e => e.LocalName == "Item").Skip(1).ToList();
        levels = 1;

        if (parents.Any(e => e.Attribute("type", "").StartsWith("cmf_", StringComparison.OrdinalIgnoreCase))
          && !parents.Any(e => e.Attribute("type", "") == "ItemType"))
          continue;

        if (parents.Count > 0 && itemDict.TryGetValue(ItemReference.FromFullItem(parents.Last(), false), out itemRefOpts))
        {
          levels = Math.Min(itemRefOpts.Levels, 1);
        }

        var parentCount = parents.Count;
        if (parentCount < 1) parentCount = 1;
        if (parentCount >= levels && _metadata.ItemTypeByName(elem.Element("Item").Attribute("type").ToLowerInvariant(), out itemType) && !itemType.IsDependent)
        {
          if (itemType.IsVersionable && elem.Parent().Element("behavior", "").IndexOf("float") >= 0)
          {
            elem.Attr(XmlFlags.Attr_Float, "1");
            elem.InnerXml = elem.Element("Item").Element("config_id", elem.Element("Item").Attribute("id"));
          }
          else
          {
            elem.InnerXml = elem.Element("Item").Attribute("id");
          }
        }
      }
    }

    private void SetQueryAttributes(XmlElement queryElem, string type, int levels, IEnumerable<ItemReference> refs)
    {
      switch (type)
      {
        case "ItemType":
          //queryElem.SetAttribute("levels", "2");
          //queryElem.SetAttribute("config_path", "Property|RelationshipType|View|Server Event|Item Action|ItemType Life Cycle|Allowed Workflow|TOC Access|TOC View|Client Event|Can Add|Allowed Permission|Item Report|Morphae");
          if (_arasVersion.Major < 11)
          {
            queryElem.InnerXml = @"<Relationships>
  <Item type='Property' action='get' levels='1' />
  <Item type='RelationshipType' action='get' related_expand='0' />
  <Item type='View' action='get' />
  <Item type='Server Event' action='get' />
  <Item type='Item Action' action='get' related_expand='0' />
  <Item type='ItemType Life Cycle' action='get' related_expand='0' />
  <Item type='Allowed Workflow' action='get' />
  <Item type='TOC Access' action='get' related_expand='0' />
  <Item type='TOC View' action='get'  related_expand='0' />
  <Item type='Client Event' action='get' />
  <Item type='Can Add' action='get' related_expand='0' />
  <Item type='Allowed Permission' action='get' related_expand='0' />
  <Item type='Item Report' action='get' related_expand='0' />
  <Item type='Morphae' action='get' related_expand='0' />
</Relationships>";
          }
          else
          {
            queryElem.InnerXml = @"<Relationships>
  <Item type='DiscussionTemplate' action='get'>
    <Relationships>
      <Item type='FeedTemplate' action='get'>
        <Relationships>
          <Item type='FileSelectorTemplate' action='get' />
        </Relationships>
      </Item>
      <Item type='DiscussionTemplateView' action='get' >
        <related_id>
          <Item type='SSVCPresentationConfiguration' action='get'/>
        </related_id>
      </Item>
    </Relationships>
  </Item>
  <Item type='ITPresentationConfiguration' action='get' related_expand='0' />
  <Item type='Property' action='get' levels='1' />
  <Item type='RelationshipType' action='get' related_expand='0' />
  <Item type='View' action='get' />
  <Item type='Server Event' action='get' />
  <Item type='Item Action' action='get' related_expand='0' />
  <Item type='ItemType Life Cycle' action='get' related_expand='0' />
  <Item type='Allowed Workflow' action='get' />
  <Item type='TOC Access' action='get' related_expand='0' />
  <Item type='TOC View' action='get'  related_expand='0' />
  <Item type='Client Event' action='get' />
  <Item type='Can Add' action='get' related_expand='0' />
  <Item type='Allowed Permission' action='get' related_expand='0' />
  <Item type='Item Report' action='get' related_expand='0' />
  <Item type='Morphae' action='get' related_expand='0' />
</Relationships>";
          }
          levels = 1;
          break;
        case "Item Report":
          queryElem.SetAttribute("levels", "0");
          queryElem.SetAttribute("related_expand", "0");
          levels = 0;
          break;
        case "RelationshipType":
          queryElem.SetAttribute("levels", "1");
          var relId = queryElem.AppendChild(queryElem.OwnerDocument.CreateElement("relationship_id"));
          relId.InnerXml = "<Item type=\"ItemType\" action=\"get\" levels=\"2\" config_path=\"Property|RelationshipType|View|Server Event|Item Action|ItemType Life Cycle|Allowed Workflow|TOC Access|TOC View|Client Event|Can Add|Allowed Permission|Item Report|Morphae\" />";
          levels = 1;
          break;
        case "Action":
          queryElem.SetAttribute("levels", "1");
          levels = 1;
          break;
        case "Report":
          queryElem.SetAttribute("levels", "1");
          levels = 1;
          break;
        case "Identity":
        case "List":
        case "Team":
        case "Method":
        case "Permission":
        case "Sequence":
        case "UserMessage":
        case "Workflow Promotion":
          queryElem.SetAttribute("levels", "1");
          queryElem.SetAttribute("related_expand", "0");
          levels = 1;
          break;
        case "Grid":
        case "User":
        case "Preference":
        case "Property":
        case "qry_QueryDefinition":
        case "rb_TreeGridViewDefinition":
        case "xClassificationTree":
        case "DiscussionTemplate":
          queryElem.SetAttribute("levels", "2");
          levels = 2;
          break;
        case "Form":
          queryElem.SetAttribute("levels", "3");
          levels = 3;
          break;
        case "Life Cycle Map":
        case "Workflow Map":
          queryElem.SetAttribute("levels", "3");
          levels = 3;
          break;
        case "cmf_ContentType":
          queryElem.InnerXml = @"<Relationships>
  <Item type='cmf_ContentTypeView' action='get'>
  </Item>
  <Item type='cmf_ElementType' action='get'>
    <generated_type>
      <Item action='get' type='ItemType'>
        <Relationships>
          <Item action='get' type='Allowed Permission'>
          </Item>
          <Item action='get' type='Server Event'>
          </Item>
          <Item action='get' type='Can Add'>
          </Item>
        </Relationships>
      </Item>
    </generated_type>
    <Relationships>
      <Item type='cmf_ElementAllowedPermission' action='get'>
      </Item>
      <Item type='cmf_ElementBinding' action='get'>
        <Relationships>
          <Item type='cmf_PropertyBinding' action='get'>
          </Item>
        </Relationships>
      </Item>
      <Item type='cmf_PropertyType' action='get'>
        <generated_type>
          <Item action='get' type='ItemType'>
            <Relationships>
              <Item action='get' type='Allowed Permission'>
              </Item>
              <Item action='get' type='Server Event'>
              </Item>
              <Item action='get' type='Can Add'>
              </Item>
            </Relationships>
          </Item>
        </generated_type>
        <Relationships>
          <Item type='cmf_ComputedProperty' action='get'>
            <Relationships>
              <Item action='get' type='cmf_ComputedPropertyDependency' related_expand='0'>
              </Item>
            </Relationships>
          </Item>
          <Item type='cmf_PropertyAllowedPermission' action='get'>
          </Item>
        </Relationships>
      </Item>
    </Relationships>
  </Item>
  <Item type='cmf_ContentTypeExportRel' action='get'>
  </Item>
  <Item type='cmf_DocumentLifeCycleState' action='get'>
  </Item>
</Relationships>";
          levels = 0;
          break;
        case "cmf_TabularView":
          queryElem.InnerXml = @"<Relationships>
  <Item type='cmf_TabularViewColumn' action='get'>
    <Relationships>
      <Item type='cmf_AdditionalPropertyType' action='get'>
      </Item>
    </Relationships>
  </Item>
  <Item type='cmf_TabularViewHeaderRows' action='get'>
    <related_id>
      <Item type='cmf_TabularViewHeaderRow' action='get'>
        <Relationships>
          <Item type='cmf_TabularViewColumnGroups' action='get'>
          </Item>
        </Relationships>
      </Item>
    </related_id>
  </Item>
  <Item type='cmf_TabularViewTree' action='get'>
  </Item>
</Relationships>";
          levels = 0;
          break;
        case "cmf_ElementType":
          queryElem.InnerXml = @"
<generated_type>
  <Item action='get' type='ItemType'>
    <Relationships>
      <Item action='get' type='Allowed Permission'>
      </Item>
      <Item action='get' type='Server Event'>
      </Item>
      <Item action='get' type='Can Add'>
      </Item>
    </Relationships>
  </Item>
</generated_type>
<Relationships>
  <Item type='cmf_ElementAllowedPermission' action='get'>
  </Item>
  <Item type='cmf_ElementBinding' action='get'>
    <Relationships>
      <Item type='cmf_PropertyBinding' action='get'>
      </Item>
    </Relationships>
  </Item>
  <Item type='cmf_PropertyType' action='get'>
    <generated_type>
      <Item action='get' type='ItemType'>
        <Relationships>
          <Item action='get' type='Allowed Permission'>
          </Item>
          <Item action='get' type='Server Event'>
          </Item>
          <Item action='get' type='Can Add'>
          </Item>
        </Relationships>
      </Item>
    </generated_type>
    <Relationships>
      <Item type='cmf_ComputedProperty' action='get'>
        <Relationships>
          <Item action='get' type='cmf_ComputedPropertyDependency' related_expand='0'>
          </Item>
        </Relationships>
      </Item>
      <Item type='cmf_PropertyAllowedPermission' action='get'>
      </Item>
    </Relationships>
  </Item>
</Relationships>";
          levels = 0;
          break;
        case "cmf_PropertyType":
          queryElem.InnerXml = @"
<generated_type>
  <Item action='get' type='ItemType'>
    <Relationships>
      <Item action='get' type='Allowed Permission'>
      </Item>
      <Item action='get' type='Server Event'>
      </Item>
      <Item action='get' type='Can Add'>
      </Item>
    </Relationships>
  </Item>
</generated_type>
<Relationships>
  <Item type='cmf_ComputedProperty' action='get'>
    <Relationships>
      <Item action='get' type='cmf_ComputedPropertyDependency' related_expand='0'>
      </Item>
    </Relationships>
  </Item>
  <Item type='cmf_PropertyAllowedPermission' action='get'>
  </Item>
</Relationships>";
          levels = 0;
          break;
        case "cmf_TabularViewColumn":
          queryElem.InnerXml = @"
<Relationships>
  <Item action='get' type='cmf_AdditionalPropertyType'>
  </Item>
</Relationships>";
          levels = 0;
          break;
        default:
          levels = (levels < 0 ? 1 : levels);
          queryElem.SetAttribute("levels", levels.ToString());
          break;
      }

      foreach (var itemRef in refs)
      {
        itemRef.Levels = levels;
      }
    }

    /// <summary>
    /// Transform the results to normalize the output using the XSLT
    /// </summary>
    private XmlDocument TransformResults(ref XmlDocument results)
    {
      EnsureTransforms();

      var doc = results.NewDoc();
      // Transform the output
      ReportProgress(95, "Transforming the results");
      using (var memStream = new MemoryStream(results.DocumentElement.ChildNodes.Count * 1000))
      using (var output = new StreamWriter(memStream))
      {
        using (var reader = new XmlNodeReader(results.DocumentElement))
        {
          _resultTransform.Transform(reader, null, output);
        }
        // Nullify the original document immediately to reclaim the memory ASAP
        results = null;

        output.Flush();
        memStream.Position = 0;
        doc.Load(memStream);

      }

      // Make sure empty elements truly empty
      var emptyElements = doc.Descendants(e => !e.HasChildNodes && !e.IsEmpty).ToArray();
      foreach (var emptyElem in emptyElements)
      {
        emptyElem.IsEmpty = true;
      }

      return doc;
    }
    /// <summary>
    /// Normalize the class structure to make it easier to handle in a text compare
    /// </summary>
    /// <param name="nodes">Nodes to render</param>
    /// <param name="writer">Writer to render the nodes to</param>
    private void WriteClassStruct(IEnumerable<XmlNode> nodes, XmlWriter writer)
    {
      // Reorder the elements
      var nodeList = nodes.ToList();
      var indices = (from n in nodeList.Select((n, i) => new { Node = n, Index = i })
                     where n.Node is XmlElement
                     orderby n.Index
                     select n.Index);
      var elems = (from n in nodeList.OfType<XmlElement>()
                   orderby (n.Attributes["id"] == null ? string.Empty : n.Attributes["id"].Value)
                   select n).ToList();
      var indiceEnum = indices.GetEnumerator();
      var elemEnum = elems.GetEnumerator();
      while (indiceEnum.MoveNext() && elemEnum.MoveNext())
      {
        nodeList[indiceEnum.Current] = elemEnum.Current;
      }

      XmlElement elem;
      foreach (var node in nodeList)
      {
        elem = node as XmlElement;
        if (elem == null)
        {
          node.WriteTo(writer);
        }
        else
        {
          writer.WriteStartElement(elem.Prefix, elem.LocalName, elem.NamespaceURI);
          if (elem.HasAttributes)
          {
            // Reorder the attributes
            foreach (var attr in (from a in elem.Attributes.OfType<XmlAttribute>()
                                  orderby a.Name
                                  select a))
            {
              attr.WriteTo(writer);
            }
          }
          WriteClassStruct(elem.ChildNodes.OfType<XmlNode>(), writer);
          writer.WriteEndElement();
        }
        switch (node.NodeType)
        {
          case XmlNodeType.Element:

            break;
          default:
            node.WriteTo(writer);
            break;
        }
      }
    }

    internal static void EnsureSystemData(IAsyncConnection _conn, ref Dictionary<string, ItemType> _itemTypes)
    {
      if (_itemTypes == null)
      {
        _itemTypes = new Dictionary<string, ItemType>();
        var itemTypes = _conn.Apply(@"<Item type='ItemType' action='get' select='is_versionable,is_dependent,implementation_type,core,name'></Item>").Items();
        ItemType result;
        foreach (var itemTypeData in itemTypes)
        {
          result = new ItemType();
          result.Id = itemTypeData.Id();
          result.IsCore = itemTypeData.Property("core").AsBoolean(false);
          result.IsDependent = itemTypeData.Property("is_dependent").AsBoolean(false);
          result.IsFederated = itemTypeData.Property("implementation_type").Value == "federated";
          result.IsPolymorphic = itemTypeData.Property("implementation_type").Value == "polymorphic";
          result.IsVersionable = itemTypeData.Property("is_versionable").AsBoolean(false);
          result.Name = itemTypeData.Property("name").Value;
          result.Reference = ItemReference.FromFullItem(itemTypeData, true);
          _itemTypes[result.Name.ToLowerInvariant()] = result;
        }

        var relationships = _conn.Apply(@"<Item action='get' type='RelationshipType' related_expand='0' select='related_id,source_id,relationship_id,name' />").Items();
        ItemType relType;
        foreach (var rel in relationships)
        {
          if (rel.SourceId().Attribute("name").HasValue()
            && _itemTypes.TryGetValue(rel.SourceId().Attribute("name").Value.ToLowerInvariant(), out result)
            && rel.Property("relationship_id").Attribute("name").HasValue()
            && _itemTypes.TryGetValue(rel.Property("relationship_id").Attribute("name").Value.ToLowerInvariant(), out relType))
          {
            result.Relationships.Add(relType);
          }
        }

        var floatProps = _conn.Apply(@"<Item type='Property' action='get' select='source_id,item_behavior,name' related_expand='0'>
                                        <data_type>item</data_type>
                                        <data_source>
                                          <Item type='ItemType' action='get'>
                                            <is_versionable>1</is_versionable>
                                          </Item>
                                        </data_source>
                                        <item_behavior>float</item_behavior>
                                        <name condition='not in'>'config_id','id'</name>
                                      </Item>").Items();
        foreach (var floatProp in floatProps)
        {
          if (_itemTypes.TryGetValue(floatProp.SourceId().Attribute("name").Value.ToLowerInvariant(), out result))
          {
            result.FloatProperties.Add(floatProp.Property("name").AsString(""));
          }
        }
      }
    }
  }
}
