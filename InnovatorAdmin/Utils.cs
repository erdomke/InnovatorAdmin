using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public static class Utils
  {
    public static bool EndsWith(this IList<string> values, params string[] predicate)
    {
      if (predicate == null || predicate.Length < 1)
        return true;
      if (values.Count < predicate.Length)
        return false;
      var offset = values.Count - predicate.Length;
      for (var i = 0; i < predicate.Length; i++)
      {
        if (!string.Equals(values[i + offset], predicate[i]))
          return false;
      }
      return true;
    }

    public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
    {
      TSource[] bucket = null;
      var count = 0;

      foreach (var item in source)
      {
        if (bucket == null)
          bucket = new TSource[size];

        bucket[count++] = item;
        if (count != size)
          continue;

        yield return bucket;

        bucket = null;
        count = 0;
      }

      if (bucket != null && count > 0)
        yield return bucket.Take(count);
    }

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

    public static void HandleError(Exception ex)
    {
      if (ex == null) return;
      using (var dialog = new Dialog.MessageDialog())
      {
        if (ex is AggregateException agg
          && agg.Flatten().InnerExceptions.Count == 1)
        {
          ex = agg.Flatten().InnerExceptions[0];
        }

        dialog.Message = ex.Message;
        dialog.Details = ex.ToString();

        if (ex.Data.Count > 0)
        {
          dialog.Details += Environment.NewLine;
          foreach (var key in ex.Data.Keys)
          {
            dialog.Details += Environment.NewLine + $"{key} = {ex.Data[key]}";
          }
        }

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
      return promise.WithInvoker((d, a) =>
      {
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
      using (var cts = new CancellationTokenSource())
      {
        cts.CancelAfter(timeout);
        Task.Run(method, cts.Token).Wait();
        return true;
      }
    }

    public static string PathEllipse(string path)
    {
      return Ellipsis(path, 400, new System.Drawing.Font("Arial", 9), TextFormatFlags.PathEllipsis);
    }
    public static string Ellipsis(string path, int width, float fontSize, TextFormatFlags FormatFlags)
    {
      return Ellipsis(path, width, new System.Drawing.Font("Arial", fontSize), FormatFlags);
    }
    public static string Ellipsis(string path, int width, System.Drawing.Font Font, TextFormatFlags formatFlags)
    {
      var result = string.Copy(path);
      TextRenderer.MeasureText(result, Font, new System.Drawing.Size(width, 0), formatFlags | TextFormatFlags.ModifyString);

      for (int index = 0; index <= result.Length - 1; index++)
      {
        if (result[index] == '\0')
        {
          return result.Substring(0, index);
        }
      }

      return result;
    }
    public static string URLEllipse(string path)
    {
      return Ellipsis(path.Replace('/', '\\'), 300, new System.Drawing.Font("Arial", 9), TextFormatFlags.PathEllipsis).Replace('\\', '/');
    }

  }
}
