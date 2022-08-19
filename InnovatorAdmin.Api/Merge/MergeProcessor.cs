using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public class MergeProcessor
  {
    private DependencySorter _sorter = new DependencySorter();

    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public HashSet<string> FirstOfGroup => _sorter.FirstOfGroup;

    public InstallScript Merge(IPackage baseDir, IPackage compareDir)
    {
      using (SharedUtils.StartActivity("MergeProcessor.Merge", "Calculating diffs"))
      {
        var docs = new List<MergeScript>();
        var metadata = baseDir.WriteAmlMergeScripts(compareDir, (path, progress, deleted) =>
        {
          ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Reading files", progress / 2));
          var script = new MergeScript(path, deleted);
          docs.Add(script);
          return script.Document.CreateNavigator().AppendChild();
        });

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
