using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public class MergeProcessor
  {
    private Dictionary<string, string> _idChangeXref = new Dictionary<string, string>();
    private DependencySorter _sorter = new DependencySorter();

    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public DependencySorter Sorter => _sorter;

    public InstallScript Merge(IPackage baseDir, IPackage compareDir)
    {
      using (SharedUtils.StartActivity("MergeProcessor.Merge", "Calculating diffs"))
      {
        var docs = new List<MergeScript>();
        var metadata = WriteAmlMergeScripts(baseDir, compareDir, (path, progress, deleted) =>
        {
          ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Reading files", progress / 2));
          var script = new MergeScript(path, deleted);
          docs.Add(script);
          return script.Document.CreateNavigator().AppendChild();
        });

        if (_idChangeXref.Count > 0)
        {
          foreach (var doc in docs)
            ReplaceIdInTextNode(doc.Document.DocumentElement);
        }

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Performing cleanup", 50));

        var allItems = docs
          .Where(d => d.Document.DocumentElement != null)
          .SelectMany(d => d.Document.DocumentElement
            .DescendantsAndSelf(el => el.LocalName == "Item"))
          .ToArray();

        RemoveDeletesForItemsWithMultipleScripts(allItems);
        RemoveChangesToSystemProperties(allItems);

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Processing dependencies", 75));

        var installScripts = docs
          .Where(d => d.Document.DocumentElement != null && d.DeletedScript == null)
          .SelectMany(d => XmlUtils.RootItems(d.Document.DocumentElement)
            .Select(i => InstallItem.FromScript(i, d.Path)))
          .ToList();

        var lines = _sorter.SortByDependencies(installScripts, metadata)
          .Where(i => i.Type != InstallType.DependencyCheck)
          .ToList();

        var deleteLines = _sorter.SortByDependencies(docs
            .Where(d => d.Document.DocumentElement != null  && d.DeletedScript != null)
            .Select(d => d.DeletedScript), metadata)
          .Where(i => i.Type != InstallType.DependencyCheck)
          .Select(i => docs.FirstOrDefault(d => ReferenceEquals(d.DeletedScript, i)))
          .Where(d => d != null)
          .SelectMany(d => XmlUtils.RootItems(d.Document.DocumentElement)
            .Select(i => InstallItem.FromScript(i, d.Path)))
          .ToList();
        deleteLines.Reverse();
        lines.AddRange(deleteLines);

        var script = new InstallScript()
        {
          Created = DateTime.Now,
          Creator = Environment.UserName,
          Title = "MergeScript",
          Lines = lines
        };

        lines.InsertRange(0, FillInNullsOnRequiredProperties(allItems, metadata));

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Processing dependencies", 80));

        return script;
      }
    }

    private IEnumerable<InstallItem> FillInNullsOnRequiredProperties(XmlElement[] allItems, PackageMetadataProvider metadata)
    {
      var newlyRequiredProps = allItems.Where(i => i.Attribute("type") == "Property"
        && i.Attribute("action") == "edit"
        && !string.IsNullOrEmpty(i.Attribute("id"))
        && i.Element("is_required", "0") == "1"
        && i.Element("default_value").HasValue())
        .ToArray();

      if (newlyRequiredProps.Length > 0)
      {
        var doc = newlyRequiredProps[0].NewDoc();
        var root = doc.Elem("AML");
        var idx = 0;

        foreach (var prop in newlyRequiredProps)
        {
          string name;
          metadata.PropById(prop.Attribute("id"), out name);
          var typeId = prop.Parent().Parent().Attribute("id");
          var typeName = metadata.ItemTypes.Single(i => i.Id == typeId).Name;
          var script = root.Elem("Item").Attr("type", typeName).Attr("action", "edit").Attr("where", "[" + typeName + "]." + name + " is null");
          script.Elem(name, prop.Element("default_value", ""));
          yield return InstallItem.FromScript(script, "_Scripts/NewlyRequiredProp (" + (++idx) + ").xml");
        }
      }
    }

    private void ReplaceIdInTextNode(XmlNode node)
    {
      if (node == null)
      {
        return;
      }
      else if (node is XmlText text)
      {
        if (_idChangeXref.TryGetValue(text.Value, out var newId))
          text.Value = newId;
      }
      else
      {
        foreach (var child in node.ChildNodes.OfType<XmlNode>())
          ReplaceIdInTextNode(child);
      }
    }

    private void RemoveChangesToSystemProperties(XmlElement[] allItems)
    {
      var sysPropEdit = allItems.Where(i => i.Attribute("type") == "Property"
        && i.Attribute("action") == "edit"
        && (i.Attribute("where")?.Contains("name='behavior'") == true
          || i.Attribute("where")?.Contains("name='itemtype'") == true))
        .ToArray();
      foreach (var item in sysPropEdit)
      {
        var parent = item.Parent();
        item.Detach();
        CleanUp(parent);
      }
    }

    private void RemoveDeletesForItemsWithMultipleScripts(XmlElement[] allItems)
    {

      var bestActionPerItem = allItems
        .Select(el => new { Ref = ItemReference.FromElement(el), Action = el.Attribute("action") })
        .GroupBy(i => i.Ref)
        .ToDictionary(g => g.Key, g => g.Max(i => i.Action));

      var conflictingDeletesToRemove = allItems
        .Where(el => el.Attribute("action") == "delete"
          && bestActionPerItem[ItemReference.FromElement(el)] != "delete")
        .ToArray();
      foreach (var item in conflictingDeletesToRemove)
      {
        var parent = item.Parent();
        item.Detach();
        CleanUp(parent);
      }
    }

    private void CleanUp(XmlNode node)
    {
      if (node.Parent() is XmlElement && !node.Elements().Any())
      {
        var parent = node.Parent();
        node.Detach();
        CleanUp(parent);
      }
    }


    public delegate XmlWriter AmlMergeCallback(string path, int progress, XElement baseScript);

    /// <summary>
    /// Writes the difference between two directories
    /// </summary>
    /// <param name="baseDir">The base directory which needs to be transformed</param>
    /// <param name="compareDir">The comparison directory which is what the base directoy should look like after transformation</param>
    /// <param name="callback">Gets an XML writer to write the merge script given the path and current progress (integer between 0 and 100)</param>
    /// <returns>Metadata regarding the target state</returns>
    internal PackageMetadataProvider WriteAmlMergeScripts(IPackage baseDir, IPackage compareDir
      , AmlMergeCallback callback)
    {
      _idChangeXref.Clear();
      Func<IPackageFile, IComparable> keyGetter = i => string.Join("/", i.Path.ToUpperInvariant()
        .Replace('\\', '/')
        .TrimStart('/')
        .Split('/')
        .Select(n => n.Trim()));
      var basePaths = baseDir.Files().OrderBy(keyGetter).ToList();
      var comparePaths = compareDir.Files().OrderBy(keyGetter).ToList();
      var completed = 0;
      var total = basePaths.Count + comparePaths.Count;

      var baseMetadata = PackageMetadataProvider.FromPackage(baseDir);
      var compareMetadata = PackageMetadataProvider.FromPackage(compareDir);
      var metadata = new PackageMetadataProvider();

      var baseScripts = new List<AmlScript>();
      var compareScripts = new List<AmlScript>();

      Action<MergeType, AmlScript, AmlScript> mergeScripts = (type, baseScript, compareScript) =>
      {
        switch (type)
        {
          case MergeType.StartOnly: // Delete
            completed++;
            using (var writer = callback(baseScript.Path, completed * 100 / total, baseScript.Script))
            {
              var elem = AmlDiff.GetMergeScript(baseScript.Script, null);
              if (elem.Elements().Any())
                elem.WriteTo(writer);
            }
            break;
          case MergeType.DestinationOnly: // Add
            completed++;
            using (var writer = callback(compareScript.Path, completed * 100 / total, null))
            {
              compareScript.Script.WriteTo(writer);
              metadata.Add(compareScript.Script);
            }
            break;
          default:
            total--;
            completed++;

            var path = compareScript.Path;
            if (!string.Equals(baseScript.Path, compareScript.Path, StringComparison.OrdinalIgnoreCase)
              && !string.IsNullOrEmpty(compareScript.Id)
              && baseScript.Path.IndexOf(compareScript.Id) >= 0)
            {
              path = baseScript.Path;
            }
            using (var writer = callback(path, completed * 100 / total, null))
            {
              if (compareScript.Script.Elements("Item").Any(e => (string)e.Attribute("action") != "delete"))
                metadata.Add(compareScript.Script);
              var elem = AmlDiff.GetMergeScript(baseScript.Script, compareScript.Script, compareMetadata);
              if (elem.Elements().Any())
                elem.WriteTo(writer);
            }
            break;
        }
      };

      basePaths.MergeSorted(comparePaths, keyGetter, (type, baseFile, compareFile) =>
      {
        try
        {
          var path = (compareFile ?? baseFile).Path;
          if (path.EndsWith(".xslt.xml", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase))
            return;

          switch (type)
          {
            case MergeType.StartOnly: // Delete
              var baseScript = new AmlScript(baseFile, baseDir);
              if (baseScript.Key == null)
                mergeScripts(type, baseScript, null);
              else
                baseScripts.Add(baseScript);
              break;
            case MergeType.DestinationOnly: // Add
              var compareScript = new AmlScript(compareFile, compareDir);
              IdentityMergeToAdd(compareScript.Script);
              if (ItemAddToIgnore(compareScript.Script))
                return;

              if (compareScript.Key == null)
                mergeScripts(type, null, compareScript);
              else
                compareScripts.Add(compareScript);
              break;
            default: // Edit
              baseScript = new AmlScript(baseFile, baseDir);
              compareScript = new AmlScript(compareFile, compareDir);

              if (baseScript.Key == compareScript.Key
                || ChangeIdIfSameName(compareScript, compareMetadata))
              {
                if (baseScript.Key != compareScript.Key)
                {
                  _idChangeXref[compareScript.Id] = baseScript.Id;
                  compareScript.Script.DescendantsAndSelf("Item").FirstOrDefault()?.SetAttributeValue("id", baseScript.Id);
                }

                mergeScripts(type, baseScript, compareScript);
              }
              else
              {
                baseScripts.Add(baseScript);
                compareScripts.Add(compareScript);
              }
              break;
          }
        }
        catch (XmlException ex)
        {
          ex.Data["MergeType"] = type.ToString();
          if (baseFile != null)
            ex.Data["BasePath"] = baseFile.Path;
          if (compareFile != null)
            ex.Data["ComparePath"] = compareFile.Path;
          throw;
        }
      });

      baseScripts = baseScripts.OrderBy(s => s.Key).ToList();
      compareScripts = compareScripts.OrderBy(s => s.Key).ToList();

      baseScripts.MergeSorted(compareScripts, s => s.Key, mergeScripts);
      return metadata;
    }

    private bool ChangeIdIfSameName(AmlScript script, IArasMetadataProvider metadata)
    {
      if (metadata.ItemTypeByName(script.Type, out var itemType)
        && itemType.KeyedNameIsUnique)
        return true;
      else if (script.Type == "PresentationConfiguration"
        && script.KeyedName?.EndsWith("_TOC_Configuration") == true)
        return true;
      return false;
    }

    /// <summary>
    /// Change action='merge' to action='add' for identities
    /// </summary>
    private static void IdentityMergeToAdd(XElement aml)
    {
      var item = aml.DescendantsAndSelf("Item").FirstOrDefault();
      if (item?.Attribute("type")?.Value != "Identity")
        return;
      item.SetAttributeValue("action", "add");
    }

    /// <summary>
    /// Indicates that the add should be ignored.
    /// </summary>
    private static bool ItemAddToIgnore(XElement aml)
    {
      return IsAddMorphaeToFileContainerItems(aml);
    }

    private static bool IsAddMorphaeToFileContainerItems(XElement aml)
    {
      var item = aml.DescendantsAndSelf("Item").FirstOrDefault();
      if (item?.Attribute("type")?.Value != "ItemType" || item.Attribute("action")?.Value != "edit")
        return false;
      if (item.Elements().Count() != 1 || item.Elements().First().Name.LocalName != "Relationships")
        return false;
      var relationships = item.Elements().First();
      if (relationships.Elements().Count() != 1 || item.Elements().First().Name.LocalName == "Item")
        return false;
      var relation = relationships.Elements().First();
      if (relation.Attribute("type")?.Value != "Morphae" || relation.Element("source_id")?.Value != "41EF49EFD2ED4F6EAB04C047681F33AC")
        return false;
      return true;
    }

    private class AmlScript
    {
      public string KeyedName { get; }
      public string Key { get; }
      public string Path { get; }
      public string Id { get; }
      public string Type { get; }
      public XElement Script { get; }

      public AmlScript(IPackageFile file, IPackage package)
      {
        Path = file.Path;
        var doc = new XDocument();
        using (var writer = doc.CreateWriter())
          file.WriteAml(package, writer);
        Script = doc.Root;

        var item = Script.DescendantsAndSelf("Item").FirstOrDefault();
        if (item != null
          && (item.Parent == null
            || !item.Parent.Elements("Item").Skip(1).Any()))
        {
          Type = (string)item.Attribute("type");
          Id = (string)item.Attribute("id");
          KeyedName = (string)item.Attribute("_keyed_name");
          var parts = new[] {
            ((string)(item.Attribute("id") ?? item.Attribute("where")))?.ToUpperInvariant(),
            (string)item.Attribute(XmlFlags.Attr_ScriptType),
            (string)item.Attribute("action")
          };
          if (parts[2] == "add" || parts[2] == "create")
            parts[2] = "merge";
          Key = string.Join(".", parts);
        }
      }
    }

    private class MergeScript
    {
      public XmlDocument Document { get; } = new XmlDocument();
      public string Path { get; }
      public InstallItem DeletedScript { get; }

      public MergeScript(string path, XElement deletedScript)
      {
        Path = path;
        if (deletedScript != null)
        {
          var doc = new XmlDocument();
          using (var writer = doc.CreateNavigator().AppendChild())
            deletedScript.WriteTo(writer);
          DeletedScript = InstallItem.FromScript(XmlUtils.RootItems(doc.DocumentElement).First(), path);
        }
      }
    }
  }
}
