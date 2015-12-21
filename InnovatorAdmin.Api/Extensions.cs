using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public static class Extensions
  {
    /// <summary>
    /// Determines if a string is an Aras-styled GUID
    /// </summary>
    public static bool IsGuid(this string value)
    {
      if (value == null || value.Length != 32) return false;
      for (int i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
          case 'F':
            // Do Nothing
            break;
          default:
            return false;
        }
      }
      return true;
    }

    public static void WriteTo(this IEnumerable<InstallItem> exports, TextWriter writer)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      // Render the output
      using (var xml = XmlTextWriter.Create(writer, settings))
      {
        xml.WriteStartElement("AML");
        foreach (var item in exports.Where(i => i.Script != null))
        {
          item.Script.WriteTo(xml);
        }
        xml.WriteEndElement();
      }
    }
    public static string WriteToString(this IEnumerable<InstallItem> exports)
    {
      using (var writer = new StringWriter())
      {
        WriteTo(exports, writer);
        return writer.ToString();
      }
    }

    public static DataTable GetItemTable(XmlNode node)
    {
      var result = new DataTable();
      try
      {
        result.BeginLoadData();
        while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
        if (node != null)
        {
          var items = node.ParentNode.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "Item");
          var props = new HashSet<string>();
          if (items.Any(i => i.HasAttribute("type") || i.Element("id") != null)) props.Add("type");
          if (items.Any(i => i.HasAttribute("typeId") || i.Element("itemtype") != null)) props.Add("itemtype");
          if (items.Any(i => i.Element("keyed_name") != null || i.Element("id") != null)) props.Add("keyed_name");
          if (items.Any(i => i.HasAttribute("id") || i.Element("id") != null)) props.Add("id");


          foreach (var elem in items.SelectMany(i => i.Elements()))
          {
            if (elem.LocalName != "id" && elem.LocalName != "config_id")
            {
              if (elem.HasAttribute("type")) props.Add(elem.LocalName + "/type");
              if (elem.HasAttribute("keyed_name")) props.Add(elem.LocalName + "/keyed_name");
            }
            props.Add(elem.LocalName);
          }

          foreach (var prop in props)
          {
            result.Columns.Add(prop, typeof(string));
          }

          DataRow row;
          int split;
          foreach (var item in items)
          {
            row = result.NewRow();
            row.BeginEdit();
            foreach (var prop in props)
            {
              switch (prop)
              {
                case "id":
                  row[prop] = item.Attribute("id", item.Element("id", null));
                  break;
                case "keyed_name":
                  row[prop] = item.Element("keyed_name", item.Element("id").Attribute("keyed_name"));
                  break;
                case "type":
                  row[prop] = item.Attribute("type", item.Element("id").Attribute("type"));
                  break;
                case "itemtype":
                  row[prop] = item.Attribute("typeId", item.Element("itemtype", null));
                  break;
                default:
                  split = prop.IndexOf('/');
                  if (split > 0)
                  {
                    row[prop] = item.Element(prop.Substring(0, split)).Attribute(prop.Substring(split + 1));
                  }
                  else
                  {
                    row[prop] = item.Element(prop, null);
                  }
                  break;
              }
            }
            row.EndEdit();
            row.AcceptChanges();
            result.Rows.Add(row);
          }
        }
      }
      finally
      {
        result.EndLoadData();
        result.AcceptChanges();
      }
      return result;
    }

    /// <summary>
    /// Groups a list of items and pages the results by batch size
    /// </summary>
    public static IEnumerable<IGrouping<TKey, T>> PagedGroupBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelector, int batchSize)
    {
      var elems = new Dictionary<TKey, Grouping<TKey, T>>();
      TKey key;
      Grouping<TKey, T> list;

      foreach (var item in items)
      {
        key = keySelector(item);
        if (!elems.TryGetValue(key, out list) || list == null)
        {
          list = new Grouping<TKey, T>();
          list.Key = key;
          elems[key] = list;
        }
        list.Values.Add(item);
        if (list.Values.Count >= batchSize)
        {
          yield return list;
          elems[key] = null;
        }
      }

      foreach (var kvp in elems.Where(k => k.Value != null))
      {
        yield return kvp.Value;
      }
    }

    private class Grouping<TKey, T> : IGrouping<TKey, T>
    {
      private List<T> _values = new List<T>();

      public TKey Key { get; internal set; }
      internal IList<T> Values { get { return _values; } }

      public IEnumerator<T> GetEnumerator()
      {
        return _values.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }
    }

    /// <summary>
    /// Whether the enumerable has more than one item or more than one item matching the criteria
    /// if criteria is provided
    /// </summary>
    public static bool HasMultiple<T>(this IEnumerable<T> items, Func<T, bool> predicate = null)
    {
      var count = 0;
      foreach (var item in items)
      {
        if (predicate == null || predicate(item))
        {
          count++;
          if (count > 1) return true;
        }
      }
      return false;
    }


    public static IEnumerable<XElement> Parents(this XElement x)
    {
      var curr = x.Parent;
      while (curr != null)
      {
        yield return curr;
        curr = curr.Parent;
      }
    }
    public static IEnumerable<XElement> ParentsAndSelf(this XElement x)
    {
      return Enumerable.Repeat(x, 1).Concat(Parents(x));
    }
    public static XElement ReplaceWithElement(this XElement x, XElement replacement)
    {
      x.ReplaceWith(replacement);
      return replacement;
    }

    public static void MergeSorted<T, TKey>(this IList<T> start, IList<T> dest,
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
  }
}
