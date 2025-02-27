using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public class AmlDiff
  {
    private enum KeyMode
    {
      Undefined,
      ItemId,
      ElementName
    }

    //public static bool IsDifferent(string start, string dest)
    //{
    //  return GetMergeScript(start, dest).Elements().Any();
    //}

    public static bool IsDifferent(XmlReader start, XmlReader dest)
    {
      return GetMergeScript(start, dest).Elements().Any();
    }

    public static XElement GetMergeScript(XmlReader start, XmlReader dest)
    {
      var startElem = XElement.Load(start);
      var result = new XElement(startElem.Name);

      // Create merges/deletes as necessary
      var destElem = XElement.Load(dest);
      GetMergeScript(startElem, destElem, result);
      Cleanup(result);
      return result;
    }

    private static void Cleanup(XElement element)
    {
      element.RemoveAnnotations<ElementKey>();
      if (element.Elements("Item")
        .Where(i => !string.IsNullOrEmpty((string)i.Attribute("id")))
        .Skip(1)
        .Any())
      {
        var xref = element.Elements("Item")
          .Where(i => !string.IsNullOrEmpty((string)i.Attribute("id")))
          .ToDictionary(i => (string)i.Attribute("id"));

        var sorted = element.Elements().DependencySort(e =>
          (e.Annotation<DiffAnnotation>()?.Original ?? e).Elements()
            .Select(e => xref.TryGetValue((string)e ?? "", out var d) ? d : null)
            .Where(d => d != null))
          .ToList();
        element.RemoveNodes();
        foreach (var child in sorted
          .Where(i => (string)i.Attribute("action") != "delete"))
        {
          element.Add(child);
        }
        foreach (var child in sorted
          .Where(i => (string)i.Attribute("action") == "delete")
          .Reverse())
        {
          element.Add(child);
        }
      }

      var origRel = element.Annotation<OriginalRelationship>();
      if (origRel != null
        && (string)element.Attribute("action") == "edit"
        && (string)element.Attribute("id") == null
        && (string)element.Attribute("where") == null)
      {
        var updates = new HashSet<string>(element.Elements().Select(e => e.Name.LocalName));
        var tableName = $"[{((string)element.Attribute("type")).Replace(' ', '_')}]";
        foreach (var matchProp in origRel.Original.Elements()
          .Where(e => !updates.Contains(e.Name.LocalName) && !string.IsNullOrEmpty((string)e)))
        {
          element.SetAttributeValue(XmlFlags.Attr_DiffRelMatchPrefix + matchProp.Name.LocalName, matchProp.Value);
        }
      }

      foreach (var child in element.Elements())
      {
        Cleanup(child);
      }
    }

    public static XElement GetMergeScript(Stream start, Stream dest)
    {
      var startElem = Utils.LoadXml(start);
      if (start == null)
        return new XElement("AML");

      var destElem = Utils.LoadXml(dest);
      return GetMergeScript(startElem, destElem);
    }

    public static XElement GetMergeScript(XElement start, XElement dest, IArasMetadataProvider destMetadata = null)
    {
      var result = new XElement(start.Name);
      if (dest == null)
      {
        var itemTag = start.DescendantsAndSelf().First(e => e.Name.LocalName == "Item");
        var startItems = itemTag.Parent.Elements("Item").Where(e => e.Attribute("action") != null
          && (e.Attribute("action").Value == "merge" || e.Attribute("action").Value == "add"
            || e.Attribute("action").Value == "create"));
        XElement newItem;
        foreach (var startItem in startItems)
        {
          newItem = new XElement(startItem.Name, startItem.Attributes().Where(IsAttributeToCopy));
          newItem.SetAttributeValue("action", "delete");
          newItem.AddAnnotation(new DiffAnnotation(startItem, false));
          result.Add(newItem);
        }
        return result;
      }

      // Create merges/deletes as necessary
      GetMergeScript(start, dest, result, destMetadata);
      Cleanup(result);
      return result;
    }
    
    private static void GetMergeScript(XElement start, XElement dest, XElement result, IArasMetadataProvider destMetadata = null)
    {
      var startList = SetKeys(start.Elements()).OrderBy(t => t.Annotation<ElementKey>().Key).ToArray();
      var destList = SetKeys(dest.Elements()).OrderBy(t => t.Annotation<ElementKey>().Key).ToArray();
      XElement res;

      startList.MergeSorted(destList, t => t.Annotation<ElementKey>().Key, (i, s, d) =>
      {
        switch (i)
        {
          case MergeType.StartOnly: // delete
            if ((string)s.Attribute("action") == "edit")
              return;

            res = EnsurePath(s, result);
            if (res.Name.LocalName == "Item")
            {
              res.SetAttributeValue("action", "delete");
              res.AddAnnotation(new DiffAnnotation(s, false));
            }
            else if (res.Name.LocalName == "Relationships")
            {
              foreach (var item in s.Elements("Item"))
              {
                if ((string)item.Attribute("action") == "add"
                  || (string)item.Attribute("action") == "merge")
                {
                  var childDelete = new XElement(item);
                  childDelete.RemoveNodes();
                  childDelete.SetAttributeValue("action", "delete");
                  childDelete.AddAnnotation(new DiffAnnotation(item, false));
                  res.Add(childDelete);
                }
              }
              res.Parent.SetAttributeValue("action", "edit");
            }
            else
            {
              res.SetAttributeValue("is_null", "1");
              res.AddAnnotation(new DiffAnnotation(s, true));
              res.Parent.SetAttributeValue("action", "edit");
            }
            break;
          case MergeType.DestinationOnly: // add
            if (destMetadata != null
              && (string)d == "0"
              && d.Parent?.Name.LocalName == "Item"
              && destMetadata.ItemTypeByName((string)d.Parent.Attribute("type"), out var itemType)
              && itemType.Properties.TryGetValue(d.Name.LocalName, out var property)
              && property.Type == PropertyType.boolean)
            {
              // Don't add boolean properties
            }
            else
            {
              res = EnsurePath(d, result);
              var finalAdd = new XElement(d);
              finalAdd.AddAnnotation(new DiffAnnotation(null, true));
              SetKeys(SetItemEdits(res.ReplaceWithElement(finalAdd)).DescendantsAndSelf());
            }
            break;
          default:
            if (TextDiffers(s, d))
            {
              res = EnsurePath(s, result);
              var final = new XElement(d);
              final.AddAnnotation(new DiffAnnotation(s, true));
              SetItemEdits(res.ReplaceWithElement(final));
            }
            else
            {
              GetMergeScript(s, d, result, destMetadata);
            }
            break;
        }
      });
    }

    private static XElement SetItemEdits(XElement child)
    {
      foreach (var item in child.Parents().Where(e => e.Name.LocalName == "Item" && e.Attribute("action") == null))
      {
        item.SetAttributeValue("action", "edit");
      }
      return child;
    }

    private static bool TextDiffers(XElement x, XElement y)
    {
      // CData nodes inherit from XText, so this covers both
      var xText = x.Nodes().OfType<XText>().FirstOrDefault()?.Value;
      if (!x.Nodes().Any())
        xText = (string)x.Attribute(XmlFlags.Attr_ConfigId);
      var yText = y.Nodes().OfType<XText>().FirstOrDefault()?.Value;
      if (!y.Nodes().Any())
        yText = (string)y.Attribute(XmlFlags.Attr_ConfigId);

      if (xText == null && yText == null)
        return false;
      if (xText == null || yText == null)
        return true;

      if (IsProbableJsonObjectString(xText) && IsProbableJsonObjectString(yText))
      {
        try
        {
          using (var xDoc = JsonDocument.Parse(xText))
          using (var yDoc = JsonDocument.Parse(yText))
          {
            var comparer = new JsonElementComparer();
            return !comparer.Equals(xDoc.RootElement, yDoc.RootElement);
          }
        }
        catch (JsonException) { }
      }

      return !string.Equals(xText, yText);
    }

    private static bool IsProbableJsonObjectString(string value)
    {
      var foundOpenBrace = false;
      for (var i = 0; i < value.Length; i++)
      {
        if (value[i] == '{')
          foundOpenBrace = true;
        else if (foundOpenBrace && value[i] == '"')
          break;
        else if (!char.IsWhiteSpace(value[i]))
          return false;
      }

      for (var i = value.Length - 1; i >= 0; i--)
      {
        if (value[i] == '}')
          return true;
        else if (!char.IsWhiteSpace(value[i]))
          return false;
      }

      return false;
    }

    private static XElement EnsurePath(XElement path, XElement result)
    {
      var pathList = path.ParentsAndSelf().Reverse().ToList();
      if (pathList.Count == 0)
        throw new ArgumentException();
      var curr = result;
      if (curr.Name != pathList[0].Name)
        throw new ArgumentException();

      XElement match;

      for (var i = 1; i < pathList.Count; i++)
      {
        var elem = pathList[i];
        match = curr.Elements().Where(e => PathMatches(elem, e)).FirstOrDefault();
        if (match == null)
        {
          match = new XElement(elem.Name, elem.Attributes().Where(IsAttributeToCopy));
          match.AddAnnotation(elem.Annotation<ElementKey>());
          if (i >= 2
            && elem.Name == "Item"
            && pathList[i - 1].Name == "Relationships"
            && pathList[i - 2].Attribute(XmlFlags.Attr_ConfigId) != null)
          {
            match.AddAnnotation(new OriginalRelationship(elem));
          }
          curr.Add(match);
        }
        curr = match;
      }
      
      return curr;
    }

    private record class OriginalRelationship(XElement Original) { }

    private static bool IsAttributeToCopy(XAttribute a)
    {
      switch (a.Name.LocalName)
      {
        case "id":
        case "where":
        case "idlist":
        case "type":
        case XmlFlags.Attr_ConfigId:
        case XmlFlags.Attr_IsScript:
        case XmlFlags.Attr_ScriptType:
        case "_keyed_name":
          return true;
        case "action":
          return a.Value == "get";
      }
      return false;
    }

    private static bool PathMatches(XElement canonical, XElement compare)
    {
      if (!canonical.Name.Equals(compare.Name))
        return false;
      if (canonical.Name.LocalName == "Item")
      {
        if (canonical.Attribute("id") != null
          && (compare.Attribute("id") == null
          || compare.Attribute("id").Value != canonical.Attribute("id").Value)) return false;
        if (canonical.Attribute("where") != null
          && (compare.Attribute("where") == null
          || compare.Attribute("where").Value != canonical.Attribute("where").Value)) return false;
        if (canonical.Attribute("idlist") != null
          && (compare.Attribute("idlist") == null
          || compare.Attribute("idlist").Value != canonical.Attribute("idlist").Value)) return false;

        if (canonical.Attribute("id") == null
          && canonical.Attribute("where") == null
          && compare.Attribute("idlist") == null
          && canonical.Annotation<ElementKey>().Key != compare.Annotation<ElementKey>().Key)
          return false;
      }
      return true;
    }

    private static IEnumerable<XElement> SetKeys(IEnumerable<XElement> elems)
    {
      var mode = KeyMode.Undefined;
      var results = new List<Tuple<string, XElement>>();

      foreach (var elem in elems)
      {
        if (mode == KeyMode.Undefined)
        {
          if (elem.Name.LocalName == "Item"
            && (elem.Attribute("action") == null || elem.Attribute("action").Value != "get")
            && elem.Attribute("type") != null)
          {
            mode = KeyMode.ItemId;
          }
          else
          {
            mode = KeyMode.ElementName;
          }
        }

        elem.AddAnnotation(ElementKey.FromElement(elem, mode));
      }

      return elems;
    }

    private class ElementKey
    {
      public string Key { get; set; }

      private ElementKey(string key)
      {
        this.Key = key;
      }

      public static ElementKey FromElement(XElement elem, KeyMode mode)
      {
        if (mode == KeyMode.ItemId)
        {
          if ((string)elem.Attribute("type") == "Morphae"
            && TryGetId(elem.Element("related_id"), out var relatedId)
            && TryGetId(elem.Element("source_id"), out var sourceId))
          {
            // Don't worry about the ID with Morphae as this can change (especially with `FileContainerItems`)
            return new ElementKey($"Morphae:{sourceId}-{relatedId}");
          }

          if ((string)elem.Attribute("type") == "View")
          {
            var formName = (string)elem.Element("related_id")?.Element("Item")?.Element("name")
              ?? (string)elem.Element("related_id")?.Element("Item")?.Attribute("id")
              ?? (string)elem.Element("related_id");

            var role = (string)elem.Element("role")?.Element("Item")?.Element("name")
                ?? (string)elem.Element("role")?.Element("Item")?.Attribute("id")
                ?? (string)elem.Element("role");

            // Don't worry about the ID with View as this can change.
            if (!string.IsNullOrEmpty(formName) && !string.IsNullOrEmpty(role))
              return new ElementKey($"View:{formName}-{role}");
          }


          if (elem.Attribute("id") != null)
          {
            return new ElementKey(elem.Attribute("id").Value);
          }
          else if (elem.Attribute("idlist") != null)
          {
            return new ElementKey(elem.Attribute("idlist").Value);
          }
          else if (elem.Attribute("where") != null)
          {
            return new ElementKey(elem.Attribute("where").Value);
          }
          else if (elem.Element("related_id") != null
            && TryGetId(elem.Element("related_id"), out relatedId))
          {
            return new ElementKey(relatedId);
          }

          return new ElementKey(elem.Name.LocalName + "[" + ElementIndex(elem).ToString() + "]");
        }
        else
        {
          var lang = elem.Attribute(XNamespace.Xml + "lang");
          return new ElementKey(elem.Name.LocalName + (lang == null ? "" : "[" + lang.Value + "]"));
        }
      }

      private static bool TryGetId(XElement elem, out string id)
      {
        id = elem.Nodes().OfType<XText>().FirstOrDefault(t => t.Value.IsGuid())?.Value
          ?? (string)elem.Element("Item")?.Attribute("id");
        return !string.IsNullOrEmpty(id);
      }

      private static int ElementIndex(XElement elem)
      {
        var curr = elem.PreviousNode;
        var i = 0;
        while (curr != null)
        {
          i++;
          curr = curr.PreviousNode;
        }
        return i;
      }
    }
  }
}
