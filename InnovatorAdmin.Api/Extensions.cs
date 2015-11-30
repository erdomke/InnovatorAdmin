using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public static class Extensions
  {
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
  }
}
