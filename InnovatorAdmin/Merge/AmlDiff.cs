using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static XElement GetMergeScript(string start, string dest)
    {
      if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(dest))
        return new XElement("AML");

      var startElem = XElement.Parse(start);
      var destElem = XElement.Parse(dest);
      var result = new XElement(startElem.Name);
      GetMergeScript(startElem, destElem, result);
      return result;
    }

    private static void GetMergeScript(XElement start, XElement dest, XElement result)
    {
      var startList = GetKeys(start.Elements()).OrderBy(t => t.Item1).ToArray();
      var destList = GetKeys(dest.Elements()).OrderBy(t => t.Item1).ToArray();
      XElement res;
      XElement clone;

      MergeSorted(startList, destList, t => t.Item1, (i, s, d) =>
      {
        switch (i)
        {
          case -1:
            res = EnsurePath(s.Item2, result);
            if (res.Name.LocalName == "Item")
            {
              res.SetAttributeValue("action", "delete");
            }
            else if (res.Name.LocalName != "Relationships")
            {
              res.SetAttributeValue("is_null", "1");
              res.Parent.SetAttributeValue("action", "edit");
            }
            break;
          case 1:
            res = EnsurePath(d.Item2, result);
            SetItemEdits(res.ReplaceWithElement(new XElement(d.Item2)));
            break;
          default:
            if (TextDiffers(s.Item2, d.Item2))
            {
              res = EnsurePath(s.Item2, result);
              SetItemEdits(res.ReplaceWithElement(new XElement(d.Item2)));
            }
            else
            {
              GetMergeScript(s.Item2, d.Item2, result);
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
      var xText = x.Nodes().OfType<XText>().FirstOrDefault();
      var yText = y.Nodes().OfType<XText>().FirstOrDefault();

      if (xText == null || yText == null)
        return false;
      return !string.Equals(xText.Value, yText.Value);
    }

    private static XElement EnsurePath(XElement path, XElement result)
    {
      var pathList = path.ParentsAndSelf().Reverse().ToArray();
      if (!pathList.Any())
        throw new ArgumentException();
      var curr = result;
      if (curr.Name != pathList[0].Name)
        throw new ArgumentException();

      XElement match;

      foreach (var elem in pathList.Skip(1))
      {
        match = curr.Elements().Where(e => PathMatches(elem, e)).FirstOrDefault();
        if (match == null)
        {
          match = new XElement(elem.Name, elem.Attributes()
            .Where(a => a.Name == "id" || a.Name == "where" || a.Name == "idlist" || a.Name == "type"));
          curr.Add(match);
        }
        curr = match;
      }

      return curr;
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
      }
      return true;
    }

    private static void MergeSorted<T, TKey>(IList<T> start, IList<T> dest,
      Func<T, TKey> keyGetter, Action<int, T, T> callback) where TKey : IComparable
    {
      var startPtr = 0;
      var destPtr = 0;
      int status;

      while (startPtr < start.Count && destPtr < dest.Count)
      {
        status = keyGetter(start[startPtr]).CompareTo(keyGetter(dest[destPtr]));
        switch (status)
        {
          case -1:
            callback(status, start[startPtr], default(T));
            startPtr++;
            break;
          case 1:
            callback(status, default(T), dest[destPtr]);
            destPtr++;
            break;
          default:
            callback(0, start[startPtr], dest[destPtr]);
            startPtr++;
            destPtr++;
            break;
        }
      }
      while (startPtr < start.Count)
      {
        callback(-1, start[startPtr], default(T));
        startPtr++;
      }
      while (destPtr < dest.Count)
      {
        callback(1, default(T), dest[destPtr]);
        destPtr++;
      }
    }

    private static IList<Tuple<string, XElement>> GetKeys(IEnumerable<XElement> elems)
    {
      var mode = KeyMode.Undefined;
      var results = new List<Tuple<string, XElement>>();

      foreach (var elem in elems)
      {
        if (mode == KeyMode.Undefined)
        {
          if (elem.Name.LocalName == "Item"
            && (elem.Attribute("action") == null || elem.Attribute("action").Value != "get")
            && elem.Attribute("type") != null
            && (elem.Attribute("id") != null || elem.Attribute("where") != null || elem.Attribute("idlist") != null))
          {
            mode = KeyMode.ItemId;
          }
          else
          {
            mode = KeyMode.ElementName;
          }
        }

        if (mode == KeyMode.ItemId)
        {
          if (elem.Attribute("id") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("id").Value, elem));
          }
          else if (elem.Attribute("idlist") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("idlist").Value, elem));
          }
          else if (elem.Attribute("where") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("where").Value, elem));
          }
        }
        else
        {
          results.Add(Tuple.Create(elem.Name.LocalName, elem));
        }
      }

      //results.Sort((x, y) => x.Item1.CompareTo(y.Item1));
      return results;
    }
  }
}
