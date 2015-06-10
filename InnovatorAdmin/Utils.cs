using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Aras.Tools.InnovatorAdmin
{
  public static class Utils
  {
    public static string GroupConcat<T>(this IEnumerable<T> values, string separator, Func<T, string> renderer)
    {
      if (values.Any())
      {
        if (renderer == null)
        {
          return values.Select(v => v.ToString()).Aggregate((p, c) => p + separator + c);
        }
        return values.Select(renderer).Aggregate((p, c) => p + separator + c);
      }
      else
      {
        return string.Empty;
      }
    }

    public static void HandleError(Exception ex)
    {
      MessageBox.Show(ex.Message);
    }

    //public static IEnumerable<Item> AsEnum(this Item item)
    //{
    //  for (var i = 0; i < item.getItemCount(); i++)
    //  {
    //    yield return item.getItemByIndex(i);
    //  }
    //}

    public static void UiThreadInvoke(this Control control, Action code)
    {
      if (control.InvokeRequired && control.Parent != null)
      {
        control.Invoke(code);
      }
      else
      {
        code.Invoke();
      }
    }

    public static string GetAppFilePath(AppFileType fileType)
    {
      string path = string.Empty;
      switch (fileType)
      {
        case AppFileType.ImportExtractor:
          path = @"{0}\{1}\last_extractor.xml";
          break;
        case AppFileType.ImportLog:
          path = @"{0}\{1}\import.log";
          break;
        case AppFileType.XsltAutoSave:
          path = @"{0}\{1}\autosave.xslt";
          break;
        default:
          throw new NotSupportedException();
      }
      return string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);
    }

    public static string FormatXml(XmlNode node)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      using (var writer = new System.IO.StringWriter())
      {
        using (var xml = XmlTextWriter.Create(writer, settings))
        {
          node.WriteTo(xml);
        }
        return writer.ToString();
      }
    }

    public static T AddAndReturn<T>(this XElement parent, T content)
    {
      parent.Add(content);
      return content;
    }

    public static void RemoveFilter<T>(this IList<T> list, Func<T, bool> predicate)
    {
      var i = 0;
      while (i < list.Count)
      {
        if (predicate(list[i]))
        {
          list.RemoveAt(i);
        }
        else
        {
          i++;
        }
      }
    }

    public static string AttributeValue(this XElement element, string attributeName)
    {
      var attr = element.Attribute(attributeName);
      if (attr == null) return null;
      return attr.Value;
    }
  }
}
