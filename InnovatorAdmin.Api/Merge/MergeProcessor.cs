using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin
{
  public class MergeProcessor
  {
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public bool SortDependencies { get; set; } = true;

    public InstallScript Merge(IDiffDirectory baseDir, IDiffDirectory compareDir)
    {
      var docs = new List<Tuple<XmlDocument, string>>();
      var metadata = baseDir.WriteAmlMergeScripts(compareDir, (path, prog) =>
      {
        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Reading files", prog / 2));
        var doc = new XmlDocument();
        docs.Add(Tuple.Create(doc, path));
        return new XmlNodeWriter(doc);
      });

      ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Performing cleanup", 50));

      var allItems = docs
        .Where(d => d.Item1.DocumentElement != null)
        .SelectMany(d => d.Item1.DocumentElement
          .DescendantsAndSelf(el => el.LocalName == "Item"))
        .ToArray();

      RemoveDeletesForItemsWithMultipleScripts(allItems);
      RemoveChangesToSystemProperties(allItems);

      var installScripts = docs
        .Where(d => d.Item1.DocumentElement != null)
        .SelectMany(d => XmlUtils.RootItems(d.Item1.DocumentElement)
          .Select(i => InstallItem.FromScript(i, d.Item2)))
        .ToArray();

      ProgressChanged?.Invoke(this, new ProgressChangedEventArgs("Processing dependencies", 75));

      var lines = (SortDependencies
        ? ExportProcessor.SortByDependencies(installScripts, metadata)
        : installScripts).ToList();
      lines.RemoveWhere(i => i.Type == InstallType.DependencyCheck);

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
        item.Detatch();
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
        item.Detatch();
        CleanUp(parent);
      }
    }

    private void CleanUp(XmlNode node)
    {
      if (node.Parent() is XmlElement && !node.Elements().Any())
      {
        var parent = node.Parent();
        node.Detatch();
        CleanUp(parent);
      }
    }
  }
}
