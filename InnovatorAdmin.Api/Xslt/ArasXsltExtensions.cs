using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using Mvp.Xml.Common.Xsl;
using Mvp.Xml.Common.XPath;
using System.Reflection;
using Mvp.Xml.Exslt;

namespace Aras.Tools.InnovatorAdmin
{
  public class ArasXsltExtensions
  {
    public static IEnumerable<MethodInfo> GetExtensionMethods(string namespaceUri)
    {
      Type type = null;
      switch (namespaceUri)
      {
        case Namespace:
          type = typeof(ArasXsltExtensions);
          break;
        case ExsltNamespaces.DatesAndTimes:
          type = typeof(ExsltDatesAndTimes);
          break;
        case ExsltNamespaces.Math:
          type = typeof(ExsltMath);
          break;
        case ExsltNamespaces.Random:
          type = typeof(ExsltRandom);
          break;
        case ExsltNamespaces.RegularExpressions:
          type = typeof(ExsltRegularExpressions);
          break;
        case ExsltNamespaces.Sets:
          type = typeof(ExsltSets);
          break;
        case ExsltNamespaces.Strings:
          type = typeof(ExsltStrings);
          break;
        case ExsltNamespaces.GDNDatesAndTimes:
          type = typeof(GDNDatesAndTimes);
          break;
        case ExsltNamespaces.GDNDynamic:
          type = typeof(GDNDynamic);
          break;
        case ExsltNamespaces.GDNMath:
          type = typeof(GDNMath);
          break;
        case ExsltNamespaces.GDNRegularExpressions:
          type = typeof(GDNRegularExpressions);
          break;
        case ExsltNamespaces.GDNSets:
          type = typeof(GDNSets);
          break;
        case ExsltNamespaces.GDNStrings:
          type = typeof(GDNStrings);
          break;
      }

      if (type == null) return Enumerable.Empty<MethodInfo>();
      return type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
    }
    public static string Transform(string xslt, string xml, IArasConnection conn)
    {
      var xal = new XsltArgumentList();
      xal.AddExtensionObject(Namespace, new ArasXsltExtensions(conn));

      var xsl = new MvpXslTransform();
      xsl.SupportedFunctions = Mvp.Xml.Exslt.ExsltFunctionNamespace.All;
      using (var reader = new StringReader(xslt))
      {
        using (var xmlReader = XmlReader.Create(reader))
        {
          xsl.Load(xmlReader);
        }
      }
      using (var writer = new System.IO.StringWriter())
      {
        xsl.Transform(new XmlInput(new StringReader(xml)), xal, new XmlOutput(writer));
        return writer.ToString();
      }
    }

    public const string Namespace = "http://www.aras.com/XsltExtensions/1.0";

    private IArasConnection _conn;

    public ArasXsltExtensions(IArasConnection conn)
    {
      _conn = conn;
    }

    public double Abs(double x)
    {
      return Math.Abs(x);
    }

    public XPathNavigator CallAction(string action, XPathNodeIterator input)
    {
      var inputDoc = new XmlDocument();
      XmlNode mainElem = inputDoc.Elem("AML");
      while(input.MoveNext())
      {
        mainElem.AppendChild(inputDoc.ReadNode(input.Current.ReadSubtree()));
      }

      if (mainElem.ChildNodes.Count == 1 && mainElem.ChildNodes[0].NodeType == XmlNodeType.Text)
      {
        inputDoc.LoadXml(mainElem.ChildNodes[0].InnerText);
        mainElem = inputDoc.DocumentElement;
      }

      if (mainElem.ChildNodes.Count == 1)
      {
        switch (mainElem.ChildNodes[0].LocalName)
        {
          case "AML":
          case "SQL":
          case "sql":
            mainElem = mainElem.ChildNodes[0];
            break;
        }
      }

      return XmlUtils.DocFromXml(_conn.CallAction(action, mainElem.OuterXml)).CreateNavigator();
    }

    public bool IsArasFault(XPathNodeIterator input)
    {
      while (input.MoveNext())
      {
        var result = XPathCache.Select("/*[local-name()='Envelope' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Body' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Fault' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]", input.Current);
        if (result.MoveNext()) return true;
      }
      return false;
    }

    public double Ceiling(double x)
    {
      return Math.Ceiling(x);
    }

    public int Compare(string x, string y)
    {
      return string.Compare(x, y);
    }

    public int Compare(string x, string y, StringComparison comparison)
    {
      return string.Compare(x, y, comparison);
    }

    public bool Contains(string input, string value)
    {
      return Contains(input, value, StringComparison.CurrentCulture);
    }

    public bool Contains(string input, string value, StringComparison comparison)
    {
      return input.IndexOf(value, comparison) >= 0;
    }

    public bool EndsWith(string input, string value)
    {
      return EndsWith(input, value, StringComparison.CurrentCulture);
    }

    public bool EndsWith(string input, string value, StringComparison comparison)
    {
      return input.EndsWith(value, comparison);
    }

    public bool FileCopy(string source, string destination, bool overwrite)
    {
      File.Copy(source, destination, overwrite);
      return true;
    }

    public double Floor(double x)
    {
      return Math.Floor(x);
    }

    public string FormatDate(string date, string format)
    {
      return DateTime.Parse(date).ToString(format);
    }

    public string GetDirectoryName(string path)
    {
      return Path.GetDirectoryName(path);
    }

    public string GetExtension(string path)
    {
      return Path.GetExtension(path);
    }

    public string GetFileName(string path)
    {
      return Path.GetFileName(path);
    }

    public string GetFileNameWithoutExtension(string path)
    {
      return Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// If the input is null or empty return the default value, otherwise, return the input as a string
    /// </summary>
    public string IfNullOrEmpty(string input, string defaultValue)
    {
      if (string.IsNullOrEmpty(input)) return defaultValue;
      return input;
    }

    public int IndexOf(string input, string value)
    {
      return IndexOf(input, value, StringComparison.CurrentCulture);
    }

    public int IndexOf(string input, string value, StringComparison comparison)
    {
      return input.IndexOf(value, comparison);
    }

    public int LastIndexOf(string input, string value)
    {
      return LastIndexOf(input, value, StringComparison.CurrentCulture);
    }

    public int LastIndexOf(string input, string value, StringComparison comparison)
    {
      return input.LastIndexOf(value, comparison);
    }

    public double Max(double x, double y)
    {
      return Math.Max(x, y);
    }

    public double Min(double x, double y)
    {
      return Math.Min(x, y);
    }

    public string NewGuid()
    {
      return NewGuid("N", true);
    }

    public string NewGuid(string format, bool uppercase)
    {
      var result = Guid.NewGuid().ToString(format);
      if (uppercase) return result.ToUpperInvariant();
      return result;
    }

    public string Now()
    {
      return DateTime.Now.ToString("s");
    }

    public string PadLeft(string input, int totalWidth, string pad)
    {
      if (string.IsNullOrEmpty(pad))
      {
        return input.PadLeft(totalWidth);
      }
      else
      {
        return input.PadLeft(totalWidth, pad[0]);
      }
    }

    public string PadRight(string input, int totalWidth, string pad)
    {
      if (string.IsNullOrEmpty(pad))
      {
        return input.PadRight(totalWidth);
      }
      else
      {
        return input.PadRight(totalWidth, pad[0]);
      }
    }

    public double Pow(double x, double y)
    {
      return Math.Pow(x, y);
    }

    public XPathNavigator RegExMatch(string input, string pattern, string options)
    {
      var opts = RegexOptions.None;
      if (options.IndexOf('i') >= 0) opts |= RegexOptions.IgnoreCase;
      if (options.IndexOf('m') >= 0) opts |= RegexOptions.Multiline;

      var match = Regex.Match(input, pattern);
      var doc = new XmlDocument();
      var result = doc.CreateDocumentFragment();
      
      RenderCaptures(match.Captures, result);
      foreach (Group group in match.Groups)
      {
        var elem = result.Elem("Group");
        RenderCaptures(group.Captures, elem);
        elem.Elem("Index").InnerText = group.Index.ToString();
        elem.Elem("Length").InnerText = group.Length.ToString();
        elem.Elem("Success").InnerText = group.Success ? "1" : "0";
        elem.Elem("Value").InnerText = group.Value;
      }
      result.Elem("Index").InnerText = match.Index.ToString();
      result.Elem("Length").InnerText = match.Length.ToString();
      result.Elem("Success").InnerText = match.Success ? "1" : "0";
      result.Elem("Value").InnerText = match.Value;

      return result.CreateNavigator();
    }

    public string Replace(string input, string oldValue, string newValue)
    {
      return Replace(input, oldValue, newValue, StringComparison.OrdinalIgnoreCase);
    }

    public string Replace(string input, string oldValue, string newValue, StringComparison comparison)
    {
      if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue)) return input;

      var sb = new StringBuilder(input.Length);
      var previousIndex = 0;
      var index = input.IndexOf(oldValue, comparison);
      while (index != -1)
      {
        sb.Append(input.Substring(previousIndex, index - previousIndex));
        sb.Append(newValue);
        index += oldValue.Length;

        previousIndex = index;
        index = input.IndexOf(oldValue, index, comparison);
      }
      sb.Append(input.Substring(previousIndex));

      return sb.ToString();
    }

    public double Sign(double x)
    {
      return Math.Sign(x);
    }

    public XPathNodeIterator Split(string input, string splitString)
    {
      var values = input.Split(new string[] { splitString }, StringSplitOptions.None);

      var doc = new XmlDocument();
      var result = doc.CreateDocumentFragment();
      for (var i = 0; i < values.Length; i++)
      {
        result.Elem("value").InnerText = values[i];
      }
      return XPathCache.Select("/*", result.CreateNavigator());
    }

    public double Sqrt(double x)
    {
      return Math.Sqrt(x);
    }

    public bool StartsWith(string input, string value)
    {
      return StartsWith(input, value, StringComparison.CurrentCulture);
    }

    public bool StartsWith(string input, string value, StringComparison comparison)
    {
      return input.StartsWith(value, comparison);
    }

    public string Today()
    {
      return DateTime.Today.ToString("s");
    }

    public string ToLower(string input)
    {
      return input.ToLower();
    }

    public string ToLowerInvariant(string input)
    {
      return input.ToLowerInvariant();
    }

    public string ToUpper(string input)
    {
      return input.ToUpper();
    }

    public string ToUpperInvariant(string input)
    {
      return input.ToUpperInvariant();
    }

    public string Trim(string input)
    {
      return input.Trim();
    }

    public string TrimEnd(string input)
    {
      return input.TrimEnd();
    }

    public string TrimEnd(string input, string chars)
    {
      return input.TrimEnd(chars.ToCharArray());
    }

    public string TrimStart(string input)
    {
      return input.TrimStart();
    }

    public string TrimStart(string input, string chars)
    {
      return input.TrimStart(chars.ToCharArray());
    }

    private void RenderCaptures(CaptureCollection captures, XmlNode node)
    {
      foreach (Capture capture in captures)
      {
        var elem = node.Elem("Capture");
        elem.Elem("Index").InnerText = capture.Index.ToString();
        elem.Elem("Length").InnerText = capture.Length.ToString();
        elem.Elem("Value").InnerText = capture.Value;
      }
    }

  }
}
