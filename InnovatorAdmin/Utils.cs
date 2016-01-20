using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public static class Utils
  {
    public static string Replace(this string source, string oldValue, string newValue, StringComparison comparisonType)
    {
      if (source.Length == 0 || oldValue.Length == 0)
        return source;

      var result = new System.Text.StringBuilder();
      int startingPos = 0;
      int nextMatch;
      while ((nextMatch = source.IndexOf(oldValue, startingPos, comparisonType)) > -1)
      {
        result.Append(source, startingPos, nextMatch - startingPos);
        result.Append(newValue);
        startingPos = nextMatch + oldValue.Length;
      }
      result.Append(source, startingPos, source.Length - startingPos);

      return result.ToString();
    }

    public static void CopyTo(this TextReader reader, TextWriter writer)
    {
      var buffer = new char[4096];
      var cnt = reader.Read(buffer, 0, buffer.Length);
      while (cnt > 0)
      {
        writer.Write(buffer, 0, cnt);
        cnt = reader.Read(buffer, 0, buffer.Length);
      }
    }
    public static async Task CopyToAsync(this TextReader reader, TextWriter writer)
    {
      var buffer = new char[4096];
      var cnt = await reader.ReadAsync(buffer, 0, buffer.Length);
      while (cnt > 0)
      {
        await writer.WriteAsync(buffer, 0, cnt);
        cnt = await reader.ReadAsync(buffer, 0, buffer.Length);
      }
    }
    public static Control FindFocusedControl(this IContainerControl container)
    {
      Control control = null;
      while (container != null)
      {
        control = container.ActiveControl;
        container = control as IContainerControl;
      }
      return control;
    }

    public static IEnumerable<Control> ParentsAndSelf(this Control control)
    {
      var parent = control;
      while (parent != null)
      {
        yield return parent;
        parent = parent.Parent;
      }
    }

    public static string IndentXml(string xml)
    {
      string result;
      if (IndentXml(xml, out result) == null)
        return result;
      return xml;
    }

    /// <summary>
    /// Tidy the xml of the editor by indenting
    /// </summary>
    /// <param name="xml">Unformatted XML string</param>
    /// <returns>Formatted Xml String</returns>
    public static Exception IndentXml(string xml, out string formattedString)
    {
      try
      {
        var readerSettings = new XmlReaderSettings();
        readerSettings.IgnoreWhitespace = true;

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CheckCharacters = true;
        settings.CloseOutput = true;

        using (var reader = new StringReader(xml))
        using (var xmlReader = XmlReader.Create(reader, readerSettings))
        using (var writer = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
          xmlWriter.WriteNode(xmlReader, true);
          xmlWriter.Flush();
          formattedString = writer.ToString();
        }

        return null;
      }
      catch (Exception ex)
      {
        Dialog.MessageDialog.Show(ex.Message);
        formattedString = string.Empty;
        return ex;
      }
    }

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
      using (var dialog = new Dialog.MessageDialog())
      {
        dialog.Message = ex.Message;
        dialog.Details = ex.ToString();
        dialog.OkText = "&Keep Going";
        dialog.Caption = "Oops, that error wasn't expected";
        dialog.CaptionColor = System.Drawing.Color.Red;
        dialog.ShowDialog();
      }
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
      if (control.InvokeRequired && (control.Parent != null || control is Form))
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
          throw new NotSupportedException("Unexpected app file path");
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


    public static IPromise<T> UiPromise<T>(this IPromise<T> promise, Control ctrl)
    {
      return promise.WithInvoker((d, a) => {
        if (ctrl.InvokeRequired)
          ctrl.Invoke(d, a);
        else
          d.DynamicInvoke(a);
      });
    }

    public static bool CellIsNull(this DataRow row, DataColumn col)
    {
      if (row.RowState == DataRowState.Deleted)
        return row.IsNull(col, DataRowVersion.Original);
      return row.IsNull(col);
    }
    public static bool CellIsNull(this DataRow row, string col)
    {
      return row.CellIsNull(row.Table.Columns[col]);
    }
    public static object CellValue(this DataRow row, DataColumn col)
    {
      if (row.RowState == DataRowState.Deleted)
        return row[col, DataRowVersion.Original];
      return row[col];
    }
    public static object CellValue(this DataRow row, string col)
    {
      if (row.RowState == DataRowState.Deleted)
        return row[col, DataRowVersion.Original];
      return row[col];
    }

    /// <summary>
    /// Call a method and wait synchronously for completion for at most <see cref="timeout"/>
    /// milliseconds
    /// </summary>
    /// <param name="timeout">Maximum time to wait in milliseconds</param>
    /// <param name="method">Method to execute</param>
    public static bool CallWithTimeout(int timeout, Action method)
    {
      var wait = new System.Threading.ManualResetEvent(false);
      Exception exception = null;
      method.BeginInvoke(iar =>
      {
        try
        {
          ((Action)iar.AsyncState).EndInvoke(iar);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        wait.Set();
      }, method);

      if (wait.WaitOne(timeout))
      {
        if (exception != null)
          throw exception;
        return true;
      }
      return false;
    }
  }
}
