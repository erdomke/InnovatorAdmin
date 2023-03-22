using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public class DiffProcessor
  {
    private Dictionary<string, string> _idChangeXref = new Dictionary<string, string>();
    private DependencySorter _sorter = new DependencySorter();

    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public DependencySorter Sorter => _sorter;

    public InstallScript Diff(IPackage baseDir, IPackage compareDir)
    {
      using (SharedUtils.StartActivity("DiffProcessor.Diff", "Calculating diffs"))
      {
        var scripts = new Dictionary<string, XElement>(StringComparer.OrdinalIgnoreCase);
        var metadata = GetAmlMergeScripts(baseDir, compareDir, scripts, progress => 
        {
          ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Reading files", progress / 2));
        });

        if (_idChangeXref.Count > 0)
        {
          foreach (var diffScript in scripts.Values)
            ReplaceIdInTextNode(diffScript);
        }

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Performing cleanup", 50));

        var allItems = scripts.Values
          .Where(d => d != null)
          .SelectMany(d => d.DescendantsAndSelf("Item"))
          .ToList();

        RemoveDeletesForItemsWithMultipleScripts(allItems);
        RemoveChangesToSystemProperties(allItems);

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Processing dependencies", 75));

        var installScripts = scripts
          .SelectMany(s => RootItems(s.Value)
            .Select(e =>
            {
              var doc = new XmlDocument();
              using (var writer = doc.CreateNavigator().AppendChild())
                DiffAnnotation.WriteTo(e, writer, DiffVersion.Both);
              return InstallItem.FromScript(doc.DocumentElement, s.Key);
            }))
          .ToList();

        var lines = _sorter.SortByDependencies(installScripts, metadata)
          .Where(i => i.Type != InstallType.DependencyCheck)
          .ToList();

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

    private IEnumerable<XElement> RootItems(XElement element)
    {
      var curr = element;
      while (curr != null && curr.Name.LocalName != "Item")
        curr = curr.Elements().FirstOrDefault();

      if (curr == null
        || curr.Name.LocalName != "Item")
      {
        yield break;
      }
      else if (curr.Parent == null)
      {
        yield return curr;
      }
      else
      {
        foreach (var item in curr.Parent.Elements("Item"))
          yield return item;
      }
    }

    private IEnumerable<InstallItem> FillInNullsOnRequiredProperties(IList<XElement> allItems, PackageMetadataProvider metadata)
    {
      var newlyRequiredProps = allItems.Where(i => (string)i.Attribute("type") == "Property"
        && (string)i.Attribute("action") == "edit"
        && !string.IsNullOrEmpty((string)i.Attribute("id"))
        && (string)i.Element("is_required") == "1"
        && !string.IsNullOrEmpty((string)i.Element("default_value")))
        .ToList();

      if (newlyRequiredProps.Count > 0)
      {
        var doc = new XmlDocument();
        var root = doc.Elem("AML");
        var idx = 0;

        foreach (var prop in newlyRequiredProps)
        {
          metadata.PropById((string)prop.Attribute("id"), out var name);
          var typeId = (string)prop.Parent.Parent.Attribute("id");
          var typeName = metadata.ItemTypes.Single(i => i.Id == typeId).Name;
          var script = root.Elem("Item").Attr("type", typeName).Attr("action", "edit").Attr("where", "[" + typeName + "]." + name + " is null");
          script.Elem(name, (string)prop.Element("default_value") ?? "");
          yield return InstallItem.FromScript(script, "_Scripts/NewlyRequiredProp (" + (++idx) + ").xml");
        }
      }
    }

    private void ReplaceIdInTextNode(XNode node)
    {
      if (node == null)
      {
        return;
      }
      else if (node is XText text)
      {
        if (_idChangeXref.TryGetValue(text.Value, out var newId))
          text.Value = newId;
      }
      else if (node is XElement element)
      {
        if (_idChangeXref.TryGetValue((string)element.Attribute("where") ?? "", out var newWhere))
          element.SetAttributeValue("where", newWhere);
        foreach (var child in element.Nodes())
          ReplaceIdInTextNode(child);
      }
    }

    private void RemoveChangesToSystemProperties(IList<XElement> allItems)
    {
      var sysPropEdit = allItems.Where(i => (string)i.Attribute("type") == "Property"
        && (string)i.Attribute("action") == "edit"
        && (((string)i.Attribute("where"))?.Contains("name='behavior'") == true
          || ((string)i.Attribute("where"))?.Contains("name='itemtype'") == true))
        .ToList();
      foreach (var item in sysPropEdit)
      {
        var parent = item.Parent;
        item.Remove();
        CleanUp(parent);
      }
    }

    private void RemoveDeletesForItemsWithMultipleScripts(IList<XElement> allItems)
    {

      var bestActionPerItem = allItems
        .Select(el => new { Ref = ItemReference.FromElement(el), Action = (string)el.Attribute("action") })
        .GroupBy(i => i.Ref)
        .ToDictionary(g => g.Key, g => g.Max(i => i.Action));

      var conflictingDeletesToRemove = allItems
        .Where(el => (string)el.Attribute("action") == "delete"
          && bestActionPerItem[ItemReference.FromElement(el)] != "delete")
        .ToArray();
      foreach (var item in conflictingDeletesToRemove)
      {
        var parent = item.Parent;
        item.Remove();
        CleanUp(parent);
      }
    }

    private void CleanUp(XNode node)
    {
      if (node != null
        && node.Parent is XElement element
        && !element.Elements().Any())
      {
        var parent = element.Parent;
        element.Remove();
        CleanUp(parent);
      }
    }

    /// <summary>
    /// Returns the difference between two directories
    /// </summary>
    /// <param name="baseDir">The base directory which needs to be transformed</param>
    /// <param name="compareDir">The comparison directory which is what the base directoy should look like after transformation</param>
    /// <param name="scripts">A list of scripts to run and their paths.</param>
    /// <param name="progress">A callback for the current progress (integer between 0 and 100).</param>
    /// <returns>Metadata regarding the target state</returns>
    internal PackageMetadataProvider GetAmlMergeScripts(IPackage baseDir, IPackage compareDir
      , Dictionary<string, XElement> scripts, Action<int> progress)
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
            progress?.Invoke(completed * 100 / total);
            var deleteScript = AmlDiff.GetMergeScript(baseScript.Script, null);
            if (deleteScript.Elements().Any())
              scripts.Add(baseScript.Path, deleteScript);
            break;
          case MergeType.DestinationOnly: // Add
            completed++;
            progress?.Invoke(completed * 100 / total);
            scripts.Add(compareScript.Path, compareScript.Script);
            metadata.Add(compareScript.Script);
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
            progress?.Invoke(completed * 100 / total);
            if (compareScript.Script.Elements("Item").Any(e => (string)e.Attribute("action") != "delete"))
              metadata.Add(compareScript.Script);
            var editScript = AmlDiff.GetMergeScript(baseScript.Script, compareScript.Script, compareMetadata);
            if (editScript.Elements().Any())
              scripts.Add(path, editScript);
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
                  _idChangeXref[compareScript.Id ?? (string)compareScript.Item.Attribute("where")]
                    = baseScript.Id ?? (string)baseScript.Item.Attribute("where");
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
      else if (script.Type == "CommandBarSection"
        && script.KeyedName?.EndsWith("_TOC_Content") == true)
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
      public XElement Item { get; }

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
          Item = item;
          Type = (string)item.Attribute("type");
          Id = (string)item.Attribute("id");
          KeyedName = (string)item.Attribute("_keyed_name");
          var parts = new[] {
            ((string)(item.Attribute("id")
              ?? item.Attribute("_config_id")
              ?? item.Attribute("where")))?.ToUpperInvariant(),
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
