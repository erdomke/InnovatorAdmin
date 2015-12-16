using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using Innovator.Client;
using System.Threading.Tasks;

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

    private IAsyncConnection _conn;
    private DependencyAnalyzer _dependAnalyzer;
    private ArasMetadataProvider _metadata;
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
      _metadata = ArasMetadataProvider.Cached(conn);
      _metadata.Reset();
      _dependAnalyzer = new DependencyAnalyzer(_metadata);
    }

    /// <summary>
    /// Fill an install script with exports of the specified items
    /// </summary>
    public void Export(InstallScript script, IEnumerable<ItemReference> items
      , bool checkDependencies = true)
    {
      ReportProgress(0, "Loading system data");
      _metadata.Wait();

      var uniqueItems = new HashSet<ItemReference>(items);
      if (script.Lines != null) uniqueItems.ExceptWith(script.Lines.Select(l => l.Reference));
      var itemList = uniqueItems.ToList();

      ItemType metaData;
      var outputDoc = new XmlDocument();
      outputDoc.AppendChild(outputDoc.CreateElement("AML"));
      XmlElement queryElem;
      var whereClause = new StringBuilder();

      ConvertPolyItemReferencesToActualType(itemList);
      string itemType;

      var groups = itemList.PagedGroupBy(i => new { Type = i.Type, Levels = i.Levels }, 25);
      foreach (var typeItems in groups)
      {
        whereClause.Length = 0;
        itemType = typeItems.Key.Type;

        // For versionable item types, get the latest generation
        if (_metadata.ItemTypeByName(typeItems.Key.Type.ToLowerInvariant(), out metaData) && metaData.IsVersionable)
        {
          queryElem = outputDoc.CreateElement("Item")
            .Attr("action", "get")
            .Attr("type", typeItems.Key.Type);

          if (typeItems.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[")
              .Append(typeItems.Key.Type.Replace(' ', '_'))
              .Append("].[config_id] in (select config_id from innovator.[")
              .Append(typeItems.Key.Type.Replace(' ', '_'))
              .Append("] where id in ('")
              .Append(typeItems.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + "','" + c))
              .Append("'))");
          }
          if (typeItems.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              typeItems.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + " or " + c));
          }
          queryElem.SetAttribute("where", whereClause.ToString());
        }
        else if (typeItems.Key.Type == "ItemType")
        {
          // Make sure relationship item types aren't accidentally added
          queryElem = outputDoc.CreateElement("Item").Attr("action", "get").Attr("type", typeItems.Key.Type);

          if (typeItems.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[ItemType].[id] in ('")
              .Append(typeItems.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + "','" + c))
              .Append("')");
          }
          if (typeItems.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              typeItems.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + " or " + c));
          }
          queryElem.SetAttribute("where", "(" + whereClause.ToString() + ") and [ItemType].[is_relationship] = '0'");

          SetQueryAttributes(queryElem, typeItems.Key.Type, typeItems.Key.Levels, typeItems);
          outputDoc.DocumentElement.AppendChild(queryElem);

          queryElem = outputDoc.CreateElement("Item")
            .Attr("action", "get")
            .Attr("type", "RelationshipType")
            .Attr("where", "[RelationshipType].[relationship_id] in (select id from innovator.[ItemType] where " + whereClause.ToString() + ")");
          itemType = "RelationshipType";
        }
        else if (typeItems.Key.Type == "List")
        {
          // Filter out auto-generated lists for polymorphic item types
          queryElem = outputDoc.CreateElement("Item")
            .Attr("action", "get")
            .Attr("type", typeItems.Key.Type);

          if (typeItems.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[List].[id] in ('")
              .Append(typeItems.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + "','" + c))
              .Append("')");
          }
          if (typeItems.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              typeItems.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + " or " + c));
          }

          queryElem.SetAttribute("where", "(" + whereClause.ToString() + @") and not [List].[id] in (
            select l.id
            from innovator.LIST l
            inner join innovator.PROPERTY p
            on l.id = p.DATA_SOURCE
            and p.name = 'itemtype'
            inner join innovator.ITEMTYPE it
            on it.id = p.SOURCE_ID
            and it.IMPLEMENTATION_TYPE = 'polymorphic'
            )");
        }
        else
        {
          queryElem = outputDoc.CreateElement("Item")
            .Attr("action", "get")
            .Attr("type", typeItems.Key.Type);

          if (typeItems.Any(i => i.Unique.IsGuid()))
          {
            whereClause.Append("[")
              .Append(typeItems.Key.Type.Replace(' ', '_'))
              .Append("].[id] in ('")
              .Append(typeItems.Where(i => i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + "','" + c))
              .Append("')");
          }
          if (typeItems.Any(i => !i.Unique.IsGuid()))
          {
            whereClause.AppendSeparator(" or ",
              typeItems.Where(i => !i.Unique.IsGuid()).Select(i => i.Unique).Aggregate((p, c) => p + " or " + c));
          }

          queryElem.SetAttribute("where", whereClause.ToString());
        }

        SetQueryAttributes(queryElem, itemType, typeItems.Key.Levels, typeItems);
        outputDoc.DocumentElement.AppendChild(queryElem);
      }

      try
      {
        ReportProgress(0, "Loading system data");

        FixFederatedRelationships(outputDoc.DocumentElement);
        var result = ExecuteExportQuery(ref outputDoc, items);

        // Add warnings for embedded relationships
        var warnings = new HashSet<ItemReference>();
        ItemReference warning;
        foreach (var relType in result.ElementsByXPath("/Result/Item[@type='ItemType']/Relationships/Item[@type='RelationshipType']"))
        {
          warning = ItemReference.FromFullItem(relType as XmlElement, true);
          warning.KeyedName = "* Possible missing relationship: " + warning.KeyedName;
          warnings.Add(warning);
        }

        //RemoveRelatedItems(result, items);
        //CleanUpSystemProps(result.DocumentElement.Elements(), items, false);
        FixPolyItemReferences(result);
        FloatVersionableRefs(result);
        var doc = TransformResults(ref result);
        NormalizeClassStructure(doc);
        FixFormFieldsPointingToSystemProperties(doc);
        RemoveKeyedNameAttributes(doc.DocumentElement);
        ExpandSystemIdentities(doc);
        RemoveVersionableRelIds(doc);
        //TODO: Replace references to poly item lists

        Export(script, doc, warnings, checkDependencies);
        CleanUpSystemProps(doc.DocumentElement.Elements(), items.ToDictionary(i => i), true);
        ConvertFloatProps(doc);

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
    }

    /// <summary>
    /// Fill an install script with exports of the items in the XmlDocument
    /// </summary>
    public void Export(InstallScript script, XmlDocument doc
      , HashSet<ItemReference> warnings = null, bool checkDependencies = true)
    {
      try
      {
        _metadata.Wait();
        FixPolyItemReferences(doc);
        FixPolyItemListReferences(doc);
        FixForeignProperties(doc);
        FixCyclicalWorkflowLifeCycleRefs(doc);
        FixCyclicalWorkflowItemTypeRefs(doc);

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

            results = GetDependencyList((script.Lines ?? Enumerable.Empty<InstallItem>())
                                          .Concat(newInstallItems), out state).ToList();
            loops++;
          }
        }
        else
        {
          var newInstallItems = (from e in itemNode.ParentNode.Elements()
                                 where e.LocalName == "Item" && e.HasAttribute("type")
                                 select InstallItem.FromScript(e)).ToList();
          //foreach (var item in newInstallItems)
          //{
          //  item.Script.SetAttribute(XmlFlags.Attr_DependenciesAnalyzed, "1");
          //}

          results = (script.Lines ?? Enumerable.Empty<InstallItem>())
            .Concat(newInstallItems).ToList();
        }


        if (warnings == null) warnings = new HashSet<ItemReference>();
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
              yield return new ItemReference("Life Cycle Map", "[Life_Cycle_Map].[id] in (select source_id from [innovator].[Life_Cycle_State] where id = '" + item.Unique +"')");
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
        node.Detatch();
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
                    prop.Detatch();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detatch();
                }
                break;
              case "behavior":
                if (item.Attribute("type") != "RelationshipType")
                {
                  if ((group & SystemPropertyGroup.State) == SystemPropertyGroup.State)
                  {
                    if (cleanAll)
                    {
                      prop.Detatch();
                    }
                    else
                    {
                      updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                    }
                  }
                  else
                  {
                    prop.Detatch();
                  }
                }
                break;
              case "permission_id":
                if ((group & SystemPropertyGroup.Permission) == SystemPropertyGroup.Permission)
                {
                  if (cleanAll)
                  {
                    prop.Detatch();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detatch();
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
                    prop.Detatch();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detatch();
                }
                break;
              case "major_rev":
              case "minor_rev":
                if ((group & SystemPropertyGroup.Versioning) == SystemPropertyGroup.Versioning)
                {
                  if (cleanAll)
                  {
                    prop.Detatch();
                  }
                  else
                  {
                    updateQuery.Append(prop.LocalName).Append(" = '").Append(prop.InnerText).Append("', ");
                  }
                }
                else
                {
                  prop.Detatch();
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

      var promises = queryItems.Select(q => (Func<IPromise>)(
          () => _conn.Process(q, true)
            .Convert(s => {
              return (IEnumerable<XmlElement>)(Items(result, s, itemDic).ToArray());
            })
          )
        ).ToArray();
      query = null; // Release the query memory as soon as possible;
      var items = Promises.Pooled(30, promises)
        .Progress((i, m) =>
        {
          ReportProgress(4 + (int)(i * 0.9), "Searching for data...");
        }).Wait();

      ReportProgress(95, "Loading results into memory...");
      var elems = items.SelectMany(r => (IEnumerable<XmlElement>)r);
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
      var settings = new XmlReaderSettings();
      settings.NameTable = doc.NameTable ?? new NameTable();
      using (var reader = XmlReader.Create(stream, settings))
      {
        while(!reader.EOF)
        {
          if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Item")
          {
            var elem = (XmlElement)doc.ReadNode(reader);
            RemoveRelatedItems(elem, itemDict);
            CleanUpSystemProps(Enumerable.Repeat(elem, 1), itemDict, false);
            yield return elem;
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
    private void FixForeignProperties(XmlDocument doc)
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
          foreignProp.Detatch();
          fix.AppendChild(foreignProp);
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
          propType.Detatch();
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
            fullType = _metadata.TypeById(item.Property("itemtype").AsString(""));
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
          fullType = _metadata.TypeById(item.Property("itemtype").AsString(""));
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
                               .ToDictionary(e => ItemReference.FromFullItem(e, false), e => e.ConfigId().AsString(""));
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

    private IEnumerable<InstallItem> GetDependencyList(IEnumerable<InstallItem> values, out CycleState cycleState)
    {
      cycleState = CycleState.NoCycle;
      var lookup = (from i in values
                    group i by i.Reference into refGroup
                    select refGroup)
                  .ToDictionary(i => i.Key, i => (IEnumerable<InstallItem>)i);

      IEnumerable<ItemReference> sorted = null;
      IList<ItemReference> cycle = new List<ItemReference>() { null };
      List<XmlNode> refs;

      sorted = lookup.Keys.DependencySort<ItemReference>(d =>
      {
        IEnumerable<InstallItem> res = null;
        if (lookup.TryGetValue(d, out res)) return res.SelectMany(r =>
        {
          var ii = r as InstallItem;
          if (ii == null) return Enumerable.Empty<ItemReference>();
          return _dependAnalyzer.GetDependencies(ii.Reference);
        });
        return Enumerable.Empty<ItemReference>();
      }, ref cycle, false);

      // Attempt to remove cycles by identifying a Relationships tag in one of the cycles
      // and moving the Relationships to the top level
      if (cycle.Count > 0 && (cycle[0] != null || cycle.Count > 1))
      {
        cycleState = CycleState.UnresolvedCycle;
        for (int i = (cycle[0] == null ? 2 : 1); i < cycle.Count; i++)
        {
          refs = _dependAnalyzer.GetReferences(cycle[i - 1], cycle[i]).ToList();
          if (refs.Count == 1)
          {
            var relTag = refs[0].Parents().FirstOrDefault(e => e.LocalName == "Relationships");
            if (relTag != null)
            {
              var parentTag = refs[0].Parents().Last(e => e.LocalName == "Item").Parent();
              foreach (var child in relTag.Elements().ToList())
              {
                child.Detatch();
                parentTag.AppendChild(child);
                var sourceId = (XmlElement)child.AppendChild(child.OwnerDocument.CreateElement("source_id"));
                sourceId.SetAttribute("type", relTag.Parent().Attribute("type"));
                sourceId.SetAttribute("keyed_name", relTag.Parent().Attribute("_keyed_name"));
                sourceId.InnerText = relTag.Parent().Attribute("id");
              }
              relTag.Detatch();
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
              relTag.Detatch();
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
                relTag.Detatch();
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
    /// Remove empty &lt;Relationship/&gt; tags that weren't dealt with by the xslt template
    /// </summary>
    private void RemoveEmptyRelationshipTags(XmlDocument doc)
    {
      var elements = doc.Descendants(e => e.LocalName == "Relationships" && e.IsEmpty).ToList();
      foreach (var elem in elements)
      {
        elem.Detatch();
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
          child.Element.Detatch();
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

      foreach (var elem in doc.ElementsByXPath(".//Relationships/Item/related_id[Item/@id!='']").ToList())
      {
        parents = elem.Parents().Where(e => e.LocalName == "Item").Skip(1).ToList();
        levels = 1;
        if (itemDict.TryGetValue(ItemReference.FromFullItem(parents.Last(), false), out itemRefOpts))
        {
          levels = Math.Min(itemRefOpts.Levels, 1);
        }

        Debug.Print(parents.GroupConcat(" > ", p => ItemReference.FromFullItem(p, true).ToString()) + " > " + ItemReference.FromFullItem(elem.Element("Item"), true).ToString());
        if (parents.Count >= levels && _metadata.ItemTypeByName(elem.Element("Item").Attribute("type").ToLowerInvariant(), out itemType) && !itemType.IsDependent)
        {
          Debug.Print("Removing");
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
          queryElem.SetAttribute("levels", "2");
          queryElem.SetAttribute("config_path", "Property|RelationshipType|View|Server Event|Item Action|ItemType Life Cycle|Allowed Workflow|TOC Access|TOC View|Client Event|Can Add|Allowed Permission|Item Report|Morphae");
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
          levels = 1;
          break;
        case "Grid":
        case "User":
        case "Preference":
        case "Property":
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
      ReportProgress(94, "Transforming the results");
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
