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
using System.Threading.Tasks;

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

    /// <summary>Returns the absolute value of a double-precision floating-point number.</summary>
    /// <returns>A double-precision floating-point number, x, such that 0 ≤ x ≤<see cref="F:System.Double.MaxValue" />.</returns>
    /// <param name="value">A number that is greater than or equal to <see cref="F:System.Double.MinValue" />, but less than or equal to <see cref="F:System.Double.MaxValue" />.</param>
    /// <filterpriority>1</filterpriority>
    public double Abs(double value)
    {
      return Math.Abs(value);
    }

    /// <summary>
    /// Call an action on the current Aras database
    /// </summary>
    /// <param name="action">SOAP action</param>
    /// <param name="input">AML to process</param>
    /// <returns>AML returned by the server</returns>
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

    /// <summary>Returns the smallest integral value that is greater than or equal to the specified double-precision floating-point number.</summary>
    /// <returns>The smallest integral value that is greater than or equal to <paramref name="a" />. If <paramref name="a" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned. Note that this method returns a <see cref="T:System.Double" /> type instead of an integral type.</returns>
    /// <param name="a">A double-precision floating-point number. </param>
    /// <filterpriority>1</filterpriority>
    public double Ceiling(double a)
    {
      return Math.Ceiling(a);
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects and returns an integer that indicates their relative position in the sort order.</summary>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.Value Condition Less than zero <paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />. Greater than zero <paramref name="x" /> is greater than <paramref name="y" />. </returns>
    /// <param name="x">The first string to compare. </param>
    /// <param name="y">The second string to compare. </param>
    /// <filterpriority>1</filterpriority>
    public int Compare(string x, string y)
    {
      return string.Compare(x, y);
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects using the specified rules, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.Value Condition Less than zero <paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />. Greater than zero <paramref name="x" /> is greater than <paramref name="y" />. </returns>
    /// <param name="x">The first string to compare.</param>
    /// <param name="y">The second string to compare. </param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison. </param>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value. </exception>
    /// <exception cref="T:System.NotSupportedException">
    ///   <see cref="T:System.StringComparison" /> is not supported.</exception>
    /// <filterpriority>1</filterpriority>
    public int Compare(string x, string y, StringComparison comparisonType)
    {
      return string.Compare(x, y, comparisonType);
    }

    /// <summary>Returns a value indicating whether the specified <see cref="T:System.String" /> object occurs within this string.</summary>
    /// <returns>true if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, false.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <filterpriority>1</filterpriority>
    public bool Contains(string input, string value)
    {
      return Contains(input, value, StringComparison.CurrentCulture);
    }

    /// <summary>Returns a value indicating whether the specified <see cref="T:System.String" /> object occurs within this string.</summary>
    /// <returns>true if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, false.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <filterpriority>1</filterpriority>
    public bool Contains(string input, string value, StringComparison comparisonType)
    {
      return input.IndexOf(value, comparisonType) >= 0;
    }

    /// <summary>Determines whether the end of this string instance matches the specified string.</summary>
    /// <returns>true if <paramref name="value" /> matches the end of this instance; otherwise, false.</returns>
    /// <param name="input">The string to test</param>
    /// <param name="value">The string to compare to the substring at the end of this instance. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <filterpriority>1</filterpriority>
    public bool EndsWith(string input, string value)
    {
      return EndsWith(input, value, StringComparison.CurrentCulture);
    }

    /// <summary>Determines whether the end of this string instance matches the specified string when compared using the specified comparison option.</summary>
    /// <returns>true if the <paramref name="value" /> parameter matches the end of this string; otherwise, false.</returns>
    /// <param name="input">The string to test</param>
    /// <param name="value">The string to compare to the substring at the end of this instance. </param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    public bool EndsWith(string input, string value, StringComparison comparisonType)
    {
      return input.EndsWith(value, comparisonType);
    }

    /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
    /// <param name="sourceFileName">The file to copy. </param>
    /// <param name="destFileName">The name of the destination file. This cannot be a directory. </param>
    /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false. </param>
    /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-<paramref name="destFileName" /> is read-only.</exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.-or- <paramref name="sourceFileName" /> or <paramref name="destFileName" /> specifies a directory. </exception>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is null. </exception>
    /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is invalid (for example, it is on an unmapped drive). </exception>
    /// <exception cref="T:System.IO.FileNotFoundException">
    ///   <paramref name="sourceFileName" /> was not found. </exception>
    /// <exception cref="T:System.IO.IOException">
    ///   <paramref name="destFileName" /> exists and <paramref name="overwrite" /> is false.-or- An I/O error has occurred. </exception>
    /// <exception cref="T:System.NotSupportedException">
    ///   <paramref name="sourceFileName" /> or <paramref name="destFileName" /> is in an invalid format. </exception>
    /// <filterpriority>1</filterpriority>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public bool FileCopy(string sourceFileName, string destFileName, bool overwrite)
    {
      File.Copy(sourceFileName, destFileName, overwrite);
      return true;
    }

    /// <summary>Returns the largest integer less than or equal to the specified double-precision floating-point number.</summary>
    /// <returns>The largest integer less than or equal to <paramref name="d" />. If <paramref name="d" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned.</returns>
    /// <param name="d">A double-precision floating-point number. </param>
    /// <filterpriority>1</filterpriority>
    public double Floor(double d)
    {
      return Math.Floor(d);
    }

    /// <summary>
    /// Attempts to convert the <paramref name="value"/> parameter to a <see cref="T:System.DateTime"/> and render it to a <see cref="T:System.String" /> using the given format specifier.
    /// </summary>
    /// <param name="value">String value to format</param>
    /// <param name="format">Format specifier</param>
    /// <returns>If <paramref name="value"/> represents a valid <see cref="T:System.DateTime"/>, the <see cref="T:System.DateTime"/> is rendered using the <paramref name="format"/> specifier. Otherwise, an exception is thrown</returns>
    public string FormatDate(string value, string format)
    {
      if (string.IsNullOrEmpty(value)) return string.Empty;
      return DateTime.Parse(value).ToString(format);
    }

    /// <summary>
    /// Attempts to convert the <paramref name="value"/> parameter to a <see cref="T:System.Integer"/> and render it to a <see cref="T:System.String" /> using the given format specifier.
    /// </summary>
    /// <param name="value">String value to format</param>
    /// <param name="format">Format specifier</param>
    /// <returns>If <paramref name="value"/> represents a valid <see cref="T:System.Integer"/>, the <see cref="T:System.Integer"/> is rendered using the <paramref name="format"/> specifier. Otherwise, an exception is thrown</returns>
    public string FormatInt(string value, string format)
    {
      if (string.IsNullOrEmpty(value)) return string.Empty;
      return int.Parse(value).ToString(format);
    }

    /// <summary>Returns the directory information for the specified path string.</summary>
    /// <returns>A <see cref="T:System.String" /> containing directory information for <paramref name="path" />, or null if <paramref name="path" /> denotes a root directory or is null. Returns <see cref="F:System.String.Empty" /> if <paramref name="path" /> does not contain directory information.</returns>
    /// <param name="path">The path of a file or directory. </param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="path" /> parameter contains invalid characters, is empty, or contains only white spaces. </exception>
    /// <exception cref="T:System.IO.PathTooLongException">The <paramref name="path" /> parameter is longer than the system-defined maximum length.</exception>
    /// <filterpriority>1</filterpriority>
    public string GetDirectoryName(string path)
    {
      return Path.GetDirectoryName(path);
    }

    /// <summary>Returns the extension of the specified path string.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the extension of the specified path (including the "."), null, or <see cref="F:System.String.Empty" />. If <paramref name="path" /> is null, GetExtension returns null. If <paramref name="path" /> does not have extension information, GetExtension returns Empty.</returns>
    /// <param name="path">The path string from which to get the extension. </param>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.  </exception>
    /// <filterpriority>1</filterpriority>
    public string GetExtension(string path)
    {
      return Path.GetExtension(path);
    }

    /// <summary>Returns the file name and extension of the specified path string.</summary>
    /// <returns>A <see cref="T:System.String" /> consisting of the characters after the last directory character in <paramref name="path" />. If the last character of <paramref name="path" /> is a directory or volume separator character, this method returns <see cref="F:System.String.Empty" />. If <paramref name="path" /> is null, this method returns null.</returns>
    /// <param name="path">The path string from which to obtain the file name and extension. </param>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />. </exception>
    /// <filterpriority>1</filterpriority>
    public string GetFileName(string path)
    {
      return Path.GetFileName(path);
    }

    /// <summary>Returns the file name of the specified path string without the extension.</summary>
    /// <returns>A <see cref="T:System.String" /> containing the string returned by <see cref="M:System.IO.Path.GetFileName(System.String)" />, minus the last period (.) and all characters following it.</returns>
    /// <param name="path">The path of the file. </param>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.</exception>
    /// <filterpriority>1</filterpriority>
    public string GetFileNameWithoutExtension(string path)
    {
      return Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Given the set of values (paths) specified, return a structure with each path grouped underneath its checksum and size.
    /// </summary>
    /// <param name="input">Set of values</param>
    /// <returns>
    /// Structure takes the form
    /// <code><![CDATA[
    /// <Result>
    ///   <Item type="File" id="{new GUID}">
    ///     <checksum></checksum>
    ///     <size></size>
    ///     <paths>
    ///       <!-- one or more paths -->
    ///       <path></path>
    ///     </paths>
    ///   </Item>
    ///   <!-- Additional items based on the unique combination of checksum and size -->
    /// </Result>
    /// ]]></code>
    /// </returns>
    public XPathNavigator GetUniqueFiles(XPathNodeIterator input)
    {
      var paths = new List<string>();
      while (input.MoveNext())
      {
        paths.Add(input.Current.Value);
      }
      var data = new Tuple<string, long>[paths.Count];
      Parallel.For(0, data.Length, j =>
      {
        data[j] = Tuple.Create(Utils.GetFileChecksum(paths[j]), new FileInfo(paths[j]).Length);
      });

      var uniqueFiles = from d in data.Select((d, i) => new
                                              {
                                                Unique = d,
                                                Path = paths[i]
                                              })
                        group d by d.Unique into grp
                        select grp;
      var doc = new XmlDocument();
      var root = doc.Elem("Result");
      XmlElement file;
      foreach (var unique in uniqueFiles)
      {
        file = root.Elem("Item").Attr("type", "File").Attr("id", Guid.NewGuid().ToString("N").ToUpperInvariant());
        file.Elem("checksum", unique.Key.Item1);
        file.Elem("size", unique.Key.Item2.ToString());
        file = file.Elem("paths");
        foreach (var path in unique)
        {
          file.Elem("path", path.Path);
        }
      }
      return doc.CreateNavigator();
    }

    /// <summary>
    /// If the input is null or empty return the default value, otherwise, return the input as a string
    /// </summary>
    /// <param name="input">Value to check</param>
    /// <param name="defaultValue">Value to return if <paramref name="input"/> is null or empty</param>
    /// <returns><paramref name="input" /> if it is not null nor empty, otherwise <paramref name="defaultValue" /></returns>
    public string IfNullOrEmpty(string input, string defaultValue)
    {
      if (string.IsNullOrEmpty(input)) return defaultValue;
      return input;
    }

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance.</summary>
    /// <returns>The zero-based index position of <paramref name="value" /> if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <filterpriority>1</filterpriority>
    public int IndexOf(string input, string value)
    {
      return IndexOf(input, value, StringComparison.CurrentCulture);
    }

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
    /// <returns>The index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    public int IndexOf(string input, string value, StringComparison comparisonType)
    {
      return input.IndexOf(value, comparisonType);
    }

    /// <summary>
    /// Indicates if the AML represents an Aras fault SOAP message
    /// </summary>
    /// <param name="input">The AML to test</param>
    /// <returns><c>true</c> if the AML represents an Aras fault SOAP message, otherwise <c>false</c></returns>
    public bool IsArasFault(XPathNodeIterator input)
    {
      while (input.MoveNext())
      {
        var result = XPathCache.Select("/*[local-name()='Envelope' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Body' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]/*[local-name()='Fault' and (namespace-uri()='http://schemas.xmlsoap.org/soap/envelope/' or namespace-uri()='')]", input.Current);
        if (result.MoveNext()) return true;
      }
      return false;
    }

    /// <summary>
    /// Indicates if the given string can be parsed as an integ
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <returns><c>true</c> if the string can be parsed, <c>false</c> otherwise</returns>
    public bool IsInteger(string value)
    {
      int result;
      return int.TryParse(value, out result);
    }

    /// <summary>
    /// Indicates if the given string can be parsed as a floating-point number
    /// </summary>
    /// <param name="value">Value test</param>
    /// <returns><c>true</c> if the string can be parsed, <c>false</c> otherwise</returns>
    public bool IsNumeric(string value)
    {
      double result;
      return double.TryParse(value, out result);
    }

    /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance.</summary>
    /// <returns>The zero-based index position of <paramref name="value" /> if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the last index position in this instance.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <filterpriority>1</filterpriority>
    public int LastIndexOf(string input, string value)
    {
      return LastIndexOf(input, value, StringComparison.CurrentCulture);
    }

    /// <summary>Reports the zero-based index of the last occurrence of a specified string within the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
    /// <returns>The index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is the last index position in this instance.</returns>
    /// <param name="input">The string to look in</param>
    /// <param name="value">The string to seek. </param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    public int LastIndexOf(string input, string value, StringComparison comparisonType)
    {
      return input.LastIndexOf(value, comparisonType);
    }

    /// <summary>Returns the larger of two double-precision floating-point numbers.</summary>
    /// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger. If <paramref name="val1" />, <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NaN" /> is returned.</returns>
    /// <param name="val1">The first of two double-precision floating-point numbers to compare. </param>
    /// <param name="val2">The second of two double-precision floating-point numbers to compare. </param>
    /// <filterpriority>1</filterpriority>
    public double Max(double val1, double val2)
    {
      return Math.Max(val1, val2);
    }

    /// <summary>Returns the smaller of two double-precision floating-point numbers.</summary>
    /// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller. If <paramref name="val1" />, <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NaN" /> is returned.</returns>
    /// <param name="val1">The first of two double-precision floating-point numbers to compare. </param>
    /// <param name="val2">The second of two double-precision floating-point numbers to compare. </param>
    /// <filterpriority>1</filterpriority>
    public double Min(double val1, double val2)
    {
      return Math.Min(val1, val2);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Guid" /> class.</summary>
    /// <returns>A new <see cref="T:System.Guid" /> object rendered in uppercase using the <c>"N"</c> format specifier.</returns>
    /// <filterpriority>1</filterpriority>
    public string NewGuid()
    {
      return NewGuid("N", true);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Guid" /> class.</summary>
    /// <param name="format">Format specifier</param>
    /// <param name="uppercase">Whether to convert the result to uppercase</param>
    /// <returns>A new <see cref="T:System.Guid" /> object using the <paramref name="format"/> specifier.</returns>
    public string NewGuid(string format, bool uppercase)
    {
      var result = Guid.NewGuid().ToString(format);
      if (uppercase) return result.ToUpperInvariant();
      return result;
    }

    /// <summary>Gets a <see cref="T:System.DateTime" /> object that is set to the current date and time on this computer, expressed as the local time.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> whose value is the current local date and time.</returns>
    /// <filterpriority>1</filterpriority>
    public string Now()
    {
      return DateTime.Now.ToString("s");
    }

    /// <summary>Returns a new string that right-aligns the characters in this instance by padding them on the left with a specified Unicode character, for a specified total length.</summary>
    /// <returns>A new string that is equivalent to this instance, but right-aligned and padded on the left with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    /// <param name="input">The string to pad</param>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
    /// <param name="paddingChar">A Unicode padding character. </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="totalWidth" /> is less than zero. </exception>
    /// <filterpriority>2</filterpriority>
    public string PadLeft(string input, int totalWidth, string paddingChar)
    {
      if (string.IsNullOrEmpty(paddingChar))
      {
        return input.PadLeft(totalWidth);
      }
      else
      {
        return input.PadLeft(totalWidth, paddingChar[0]);
      }
    }

    /// <summary>Returns a new string that left-aligns the characters in this string by padding them on the right with a specified Unicode character, for a specified total length.</summary>
    /// <returns>A new string that is equivalent to this instance, but left-aligned and padded on the right with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />.  However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    /// <param name="input">The string to pad</param>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. </param>
    /// <param name="paddingChar">A Unicode padding character. </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="totalWidth" /> is less than zero. </exception>
    /// <filterpriority>2</filterpriority>
    public string PadRight(string input, int totalWidth, string paddingChar)
    {
      if (string.IsNullOrEmpty(paddingChar))
      {
        return input.PadRight(totalWidth);
      }
      else
      {
        return input.PadRight(totalWidth, paddingChar[0]);
      }
    }

    /// <summary>Returns a specified number raised to the specified power.</summary>
    /// <returns>The number <paramref name="x" /> raised to the power <paramref name="y" />.</returns>
    /// <param name="x">A double-precision floating-point number to be raised to a power. </param>
    /// <param name="y">A double-precision floating-point number that specifies a power. </param>
    /// <filterpriority>1</filterpriority>
    public double Pow(double x, double y)
    {
      return Math.Pow(x, y);
    }

    /// <summary>Searches the specified input string for the first occurrence of the regular expression supplied in the <paramref name="pattern" /> parameter.</summary>
    /// <returns>An object that contains information about the match.  The format of this object is
    /// <code><![CDATA[
    ///   <Group> <!-- One or more -->
    ///     <Capture> <!-- One or more -->
    ///       <Index />
    ///       <Length />
    ///       <Value />
    ///     </Capture>
    ///     <Index />
    ///     <Length />
    ///     <Success />
    ///     <Value />
    ///   </Group>
    ///   <Index />
    ///   <Length />
    ///   <Success />
    ///   <Value />
    /// ]]></code>
    /// </returns>
    /// <param name="input">The string to search for a match. </param>
    /// <param name="pattern">The regular expression pattern to match. </param>
    /// <param name="options">Options. <c>i</c> indicates to ignore case. <c>m</c> indicates to ^ and $ should match a line within <paramref name="input"/>, not just the start and end.</param>
    /// <exception cref="T:System.ArgumentException">A regular expression parsing error has occurred.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="input" /> is null. -or-<paramref name="pattern" /> is null.</exception>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
    /// </PermissionSet>
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

    /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.</summary>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />.</returns>
    /// <param name="input">The string to replace content within</param>
    /// <param name="oldValue">The string to be replaced. </param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="oldValue" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="oldValue" /> is the empty string (""). </exception>
    /// <filterpriority>1</filterpriority>
    public string Replace(string input, string oldValue, string newValue)
    {
      return Replace(input, oldValue, newValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.  The search takes place using the rules of the <paramref name="comparisonType" /></summary>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />.</returns>
    /// <param name="input">The string to replace content within</param>
    /// <param name="oldValue">The string to be replaced. </param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />. </param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="oldValue" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="oldValue" /> is the empty string (""). </exception>
    /// <filterpriority>1</filterpriority>
    public string Replace(string input, string oldValue, string newValue, StringComparison comparisonType)
    {
      if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue)) return input;

      var sb = new StringBuilder(input.Length);
      var previousIndex = 0;
      var index = input.IndexOf(oldValue, comparisonType);
      while (index != -1)
      {
        sb.Append(input.Substring(previousIndex, index - previousIndex));
        sb.Append(newValue);
        index += oldValue.Length;

        previousIndex = index;
        index = input.IndexOf(oldValue, index, comparisonType);
      }
      sb.Append(input.Substring(previousIndex));

      return sb.ToString();
    }

    /// <summary>Returns a value indicating the sign of a double-precision floating-point number.</summary>
    /// <returns>A number indicating the sign of <paramref name="value" />.Number Description -1 <paramref name="value" /> is less than zero. 0 <paramref name="value" /> is equal to zero. 1 <paramref name="value" /> is greater than zero. </returns>
    /// <param name="value">A signed number. </param>
    /// <exception cref="T:System.ArithmeticException">
    ///   <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />. </exception>
    /// <filterpriority>1</filterpriority>
    public double Sign(double value)
    {
      return Math.Sign(value);
    }

    /// <summary>Returns a string array that contains the substrings in this string that are delimited by elements of a specified string array. A parameter specifies whether to return empty array elements.</summary>
    /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more strings in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    /// <param name="input">The string to split</param>
    /// <param name="separator">A string that delimits the substrings in this string</param>
    /// <filterpriority>1</filterpriority>
    public XPathNodeIterator Split(string input, string separator)
    {
      var values = input.Split(new string[] { separator }, StringSplitOptions.None);

      var doc = new XmlDocument();
      var result = doc.CreateDocumentFragment();
      for (var i = 0; i < values.Length; i++)
      {
        result.Elem("value").InnerText = values[i];
      }
      return XPathCache.Select("/*", result.CreateNavigator());
    }

    /// <summary>Returns the square root of a specified number.</summary>
    /// <returns>Value of <paramref name="d" />Returns Zero, or positive The positive square root of <paramref name="d" />. Negative <see cref="F:System.Double.NaN" />If <paramref name="d" /> is equal to <see cref="F:System.Double.NaN" /> or <see cref="F:System.Double.PositiveInfinity" />, that value is returned.</returns>
    /// <param name="d">A number. </param>
    /// <filterpriority>1</filterpriority>
    public double Sqrt(double d)
    {
      return Math.Sqrt(d);
    }

    /// <summary>Determines whether the beginning of this string instance matches the specified string.</summary>
    /// <returns>true if <paramref name="value" /> matches the beginning of this string; otherwise, false.</returns>
    /// <param name="input">The string to test</param>
    /// <param name="value">The string to compare. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    public bool StartsWith(string input, string value)
    {
      return StartsWith(input, value, StringComparison.CurrentCulture);
    }

    /// <summary>Determines whether the beginning of this string instance matches the specified string when compared using the specified comparison option.</summary>
    /// <returns>true if this instance begins with <paramref name="value" />; otherwise, false.</returns>
    /// <param name="input">The string to test</param>
    /// <param name="value">The string to compare. </param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="value" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    public bool StartsWith(string input, string value, StringComparison comparison)
    {
      return input.StartsWith(value, comparison);
    }

    /// <summary>Gets the current date.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> set to today's date, with the time component set to 00:00:00.</returns>
    /// <filterpriority>1</filterpriority>
    public string Today()
    {
      return DateTime.Today.ToString("s");
    }

    /// <summary>Returns a copy of this <see cref="T:System.String" /> converted to lowercase, using the casing rules of the current culture.</summary>
    /// <param name="input">String value to transform</param>
    /// <returns>A <see cref="T:System.String" /> in lowercase.</returns>
    /// <filterpriority>1</filterpriority>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public string ToLower(string input)
    {
      return input.ToLower();
    }

    /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to lowercase using the casing rules of the invariant culture.</summary>
    /// <param name="input">String value to transform</param>
    /// <returns>A <see cref="T:System.String" /> object in lowercase.</returns>
    /// <filterpriority>2</filterpriority>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public string ToLowerInvariant(string input)
    {
      return input.ToLowerInvariant();
    }

    /// <summary>Returns a copy of this <see cref="T:System.String" /> converted to uppercase, using the casing rules of the current culture.</summary>
    /// <returns>A <see cref="T:System.String" /> in uppercase.</returns>
    /// <param name="input">String value to transform</param>
    /// <filterpriority>1</filterpriority>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public string ToUpper(string input)
    {
      return input.ToUpper();
    }

    /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to uppercase using the casing rules of the invariant culture.</summary>
    /// <returns>A <see cref="T:System.String" /> object in uppercase.</returns>
    /// <param name="input">String value to transform</param>
    /// <filterpriority>2</filterpriority>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
    /// </PermissionSet>
    public string ToUpperInvariant(string input)
    {
      return input.ToUpperInvariant();
    }

    /// <summary>Removes all leading and trailing white-space characters from the current <see cref="T:System.String" /> object.</summary>
    /// <param name="input">String value to transform</param>
    /// <returns>The string that remains after all white-space characters are removed from the start and end of the current <see cref="T:System.String" /> object.</returns>
    /// <filterpriority>1</filterpriority>
    public string Trim(string input)
    {
      return input.Trim();
    }

    /// <summary>Removes all trailing white-space characters from the current <see cref="T:System.String" /> object.</summary>
    /// <param name="input">String value to transform</param>
    /// <returns>The string that remains after all white-space characters are removed from the end of the current <see cref="T:System.String" /> object.</returns>
    /// <filterpriority>1</filterpriority>
    public string TrimEnd(string input)
    {
      return input.TrimEnd();
    }

    /// <summary>Removes all trailing occurrences of a set of characters specified in an array from the current <see cref="T:System.String" /> object.</summary>
    /// <returns>The string that remains after all occurrences of the characters in the <paramref name="trimChars" /> parameter are removed from the end of the current <see cref="T:System.String" /> object. If <paramref name="trimChars" /> is null or an empty array, white-space characters are removed instead.</returns>
    /// <param name="input">String value to transform</param>
    /// <param name="trimChars">A string of Unicode characters to remove or null. </param>
    /// <filterpriority>2</filterpriority>
    public string TrimEnd(string input, string trimChars)
    {
      return input.TrimEnd(trimChars.ToCharArray());
    }

    /// <summary>Removes all leading white-space characters from the current <see cref="T:System.String" /> object.</summary>
    /// <param name="input">String value to transform</param>
    /// <returns>The string that remains after all white-space characters are removed from the start of the current <see cref="T:System.String" /> object.</returns>
    /// <filterpriority>1</filterpriority>
    public string TrimStart(string input)
    {
      return input.TrimStart();
    }

    /// <summary>Removes all leading occurrences of a set of characters specified in an array from the current <see cref="T:System.String" /> object.</summary>
    /// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars" /> parameter are removed from the start of the current <see cref="T:System.String" /> object. If <paramref name="trimChars" /> is null or an empty array, white-space characters are removed instead.</returns>
    /// <param name="input">String value to transform</param>
    /// <param name="trimChars">An array of Unicode characters to remove or null. </param>
    /// <filterpriority>2</filterpriority>
    public string TrimStart(string input, string trimChars)
    {
      return input.TrimStart(trimChars.ToCharArray());
    }

    /// <summary>
    /// Attempts to convert the <paramref name="value"/> parameter to a <see cref="T:System.DateTime"/> and render it to a <see cref="T:System.String" /> using the given format specifier.
    /// </summary>
    /// <param name="value">String value to format</param>
    /// <param name="format">Format specifier</param>
    /// <returns>If <paramref name="value"/> represents a valid <see cref="T:System.DateTime"/>, the <see cref="T:System.DateTime"/> is rendered using the <paramref name="format"/> specifier. Otherwise, <paramref name="value"/> is returned</returns>
    public string TryFormatDate(string value, string format)
    {
      if (string.IsNullOrEmpty(value)) return string.Empty;
      DateTime parsed;
      if (!DateTime.TryParse(value, out parsed)) return value;
      return parsed.ToString(format);
    }

    /// <summary>
    /// Attempts to convert the <paramref name="value"/> parameter to a <see cref="T:System.Integer"/> and render it to a <see cref="T:System.String" /> using the given format specifier.
    /// </summary>
    /// <param name="value">String value to format</param>
    /// <param name="format">Format specifier</param>
    /// <returns>If <paramref name="value"/> represents a valid <see cref="T:System.Integer"/>, the <see cref="T:System.Integer"/> is rendered using the <paramref name="format"/> specifier. Otherwise, <paramref name="value"/> is returned</returns>
    public string TryFormatInt(string value, string format)
    {
      if (string.IsNullOrEmpty(value)) return string.Empty;
      int parsed;
      if (!int.TryParse(value, out parsed)) return value;
      return parsed.ToString(format);
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
