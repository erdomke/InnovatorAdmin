// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System.Diagnostics;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// Holds information about the start of a fold in an xml string.
  /// </summary>
  sealed class AmlFoldStart : NewFolding
  {
    private IList<KeyValuePair<string, string>> _attributes = new List<KeyValuePair<string, string>>();
    private bool keyedNameSet = false;

    private List<AmlFoldStart> _properties = new List<AmlFoldStart>();

    internal int EndLine;
    internal int StartLine;
    internal IEnumerable<AmlFoldStart> Properties
    {
      get
      {
        var attr = _attributes.SingleOrDefault(k => k.Key == "keyed_name");
        var keyedName = (attr.Equals(default(KeyValuePair<string, string>)) ? "" : attr.Value);

        foreach (var prop in _properties.Where(p => string.IsNullOrEmpty(p.Name)).ToArray())
        {
          prop.Name = "<Properties keyed_name=\"" + keyedName + "\" ...>";
        }
        return _properties.Where(p => p.StartLine != p.EndLine);
      }
    }

    public void AddAttr(XmlReader reader)
    {
      for (int i = 0; i < reader.AttributeCount; ++i)
      {
        reader.MoveToAttribute(i);

        _attributes.Add(new KeyValuePair<string, string>(reader.Name, reader.Value));
      }
    }

    public void Flush()
    {
      if (!this.Name.StartsWith("<"))
      {
        if (_attributes.Any())
        {
          this.Name = String.Concat("<", this.Name, " ", GetAttributeFoldText(), ">");
        }
        else
        {
          this.Name = String.Concat("<", this.Name, ">");
        }
      }
    }

    public void SetEndPropOffset(TextDocument document, XmlReader reader)
    {
      if (_properties.Any())
      {
        var prop = _properties.Last();
        IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
        prop.EndLine = lineInfo.LineNumber;
        prop.EndOffset = document.GetOffset(lineInfo.LineNumber, lineInfo.LinePosition + reader.Name.Length + 1);
      }
    }

    public void SetKeyedName(string keyedName)
    {
      if (!keyedNameSet && !string.IsNullOrEmpty(keyedName) && !keyedName.IsGuid() && keyedName != "Related: ")
      {
        _attributes.Add(new KeyValuePair<string, string>("keyed_name", keyedName));
        keyedNameSet = true;
      }
    }

    public void SetStartPropOffset(TextDocument document, XmlReader reader)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
      if (!_properties.Any() || lineInfo.LineNumber > _properties.Last().EndLine + 1)
      {
        var prop = new AmlFoldStart();
        prop.StartLine = lineInfo.LineNumber;
        prop.StartOffset = document.GetOffset(lineInfo.LineNumber, lineInfo.LinePosition - 1);
        _properties.Add(prop);
      }
    }
    private string GetAttributeFoldText()
    {
      IEnumerable<KeyValuePair<string, string>> attrEnum = _attributes;
      if (this.Name == "Item")
      {
        attrEnum = _attributes.Where(a => a.Key == "type")
          .Concat(_attributes.Where(a => a.Key == "keyed_name"))
          .Concat(_attributes.Where(a => a.Key == "id"))
          .Concat(_attributes.Where(a => a.Key != "type" && a.Key != "keyed_name" && a.Key != "id").OrderBy(a => a.Key));
      }
      var text = new StringBuilder();
      var first = true;
      foreach (var kvp in attrEnum)
      {
        if (!first)
          text.Append(' ');

        text.Append(kvp.Key);
        text.Append("=\"");
        text.Append(AmlFoldingStrategy.XmlEncodeAttributeValue(kvp.Value, '\"'));
        text.Append('\"');

        first = false;
      }

      return text.ToString();
    }
  }

  /// <summary>
  /// Determines folds for an xml string in the editor.
  /// </summary>
  public class AmlFoldingStrategy : IFoldingStrategy
  {
    /// <summary>
    /// Flag indicating whether attributes should be displayed on folded
    /// elements.
    /// </summary>
    public bool ShowAttributesWhenFolded { get; set; }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document.
    /// </summary>
    public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
    {
      firstErrorOffset = 0;
      if (document.TextLength < 1)
        return Enumerable.Empty<NewFolding>();

      try
      {
        if (document.IndexOf('<', 0, document.TextLength) < 0)
          return Enumerable.Empty<NewFolding>();
        var reader = new XmlTextReader(document.CreateReader());
        reader.XmlResolver = null; // don't resolve DTDs
        return CreateNewFoldings(document, reader, out firstErrorOffset);
      }
      catch (XmlException)
      {
        return Enumerable.Empty<NewFolding>();
      }
    }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document.
    /// </summary>
    [DebuggerStepThrough()]
    public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, XmlReader reader, out int firstErrorOffset)
    {
      Stack<AmlFoldStart> stack = new Stack<AmlFoldStart>();
      List<NewFolding> foldMarkers = new List<NewFolding>();
      try
      {
        while (reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
              if (stack.Any() && stack.Peek().Name == "Item")
              {
                if (this.ShowAttributesWhenFolded && reader.LocalName == "id")
                {
                  stack.Peek().SetKeyedName(reader.GetAttribute("keyed_name"));
                }
                else if (this.ShowAttributesWhenFolded && reader.LocalName == "keyed_name")
                {
                  stack.Peek().SetKeyedName(reader.Value);
                }
                else if (this.ShowAttributesWhenFolded && reader.LocalName == "related_id")
                {
                  stack.Peek().SetKeyedName("Related: " + (reader.GetAttribute("keyed_name") ?? ""));
                }

                if (reader.LocalName != "Relationships" && reader.LocalName != "related_id")
                {
                  stack.Peek().SetStartPropOffset(document, reader);
                  if (reader.IsEmptyElement) stack.Peek().SetEndPropOffset(document, reader);
                }
              }
              if (!reader.IsEmptyElement)
              {
                var newFoldStart = CreateElementFoldStart(document, reader);
                stack.Push(newFoldStart);
              }
              break;

            case XmlNodeType.EndElement:
              var foldStart = stack.Pop();
              foldStart.Flush();
              CreateElementFold(document, foldMarkers, reader, foldStart);
              if (stack.Any() && stack.Peek().Name == "Item" && reader.LocalName != "Relationships" && reader.LocalName != "related_id")
              {
                stack.Peek().SetEndPropOffset(document, reader);
              }
              break;

            case XmlNodeType.Comment:
              CreateCommentFold(document, foldMarkers, reader);
              break;
          }
        }
        firstErrorOffset = -1;
      }
      catch (XmlException ex)
      {
        // ignore errors at invalid positions (prevent ArgumentOutOfRangeException)
        if (ex.LineNumber >= 1 && ex.LineNumber <= document.LineCount)
          firstErrorOffset = document.GetOffset(ex.LineNumber, ex.LinePosition);
        else
          firstErrorOffset = 0;
      }
      foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
      return foldMarkers;
    }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document and updates the folding manager with them.
    /// </summary>
    public void UpdateFoldings(FoldingManager manager, TextDocument document)
    {
      int firstErrorOffset;
      IEnumerable<NewFolding> foldings = CreateNewFoldings(document, out firstErrorOffset);
      manager.UpdateFoldings(foldings, firstErrorOffset);
    }
    static int GetOffset(TextDocument document, XmlReader reader)
    {
      IXmlLineInfo info = reader as IXmlLineInfo;
      if (info != null && info.HasLineInfo())
      {
        return document.GetOffset(info.LineNumber, info.LinePosition);
      }
      else
      {
        throw new ArgumentException("XmlReader does not have positioning information.");
      }
    }

    /// <summary>
    /// Creates a comment fold if the comment spans more than one line.
    /// </summary>
    /// <remarks>The text displayed when the comment is folded is the first
    /// line of the comment.</remarks>
    static void CreateCommentFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader)
    {
      string comment = reader.Value;
      if (comment != null)
      {
        int firstNewLine = comment.IndexOf('\n');
        if (firstNewLine >= 0)
        {

          // Take off 4 chars to get the actual comment start (takes
          // into account the <!-- chars.

          int startOffset = GetOffset(document, reader) - 4;
          int endOffset = startOffset + comment.Length + 7;

          string foldText = String.Concat("<!--", comment.Substring(0, firstNewLine).TrimEnd('\r'), "-->");
          foldMarkers.Add(new NewFolding(startOffset, endOffset) { Name = foldText });
        }
      }
    }

    /// <summary>
    /// Create an element fold if the start and end tag are on
    /// different lines.
    /// </summary>
    static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader, AmlFoldStart foldStart)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
      int endLine = lineInfo.LineNumber;
      if (endLine > foldStart.StartLine)
      {
        int endCol = lineInfo.LinePosition + reader.Name.Length + 1;
        foldStart.EndOffset = document.GetOffset(endLine, endCol);

        if (foldStart.Properties.Any())
        {
          foreach (var prop in foldStart.Properties)
          {
            foldMarkers.Add(prop);
          }
        }

        foldMarkers.Add(foldStart);
      }
    }

    /// <summary>
    /// Creates an XmlFoldStart for the start tag of an element.
    /// </summary>
    AmlFoldStart CreateElementFoldStart(TextDocument document, XmlReader reader)
    {
      // Take off 1 from the offset returned
      // from the xml since it points to the start
      // of the element name and not the beginning
      // tag.
      //XmlFoldStart newFoldStart = new XmlFoldStart(reader.Prefix, reader.LocalName, reader.LineNumber - 1, reader.LinePosition - 2);
      AmlFoldStart newFoldStart = new AmlFoldStart();

      IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
      newFoldStart.StartLine = lineInfo.LineNumber;
      newFoldStart.StartOffset = document.GetOffset(newFoldStart.StartLine, lineInfo.LinePosition - 1);

      if (this.ShowAttributesWhenFolded && reader.HasAttributes)
      {
        newFoldStart.Name = reader.Name;
        newFoldStart.AddAttr(reader);
      }
      else
      {
        newFoldStart.Name = reader.Name;
      }

      return newFoldStart;
    }
    /// <summary>
    /// Gets the element's attributes as a string on one line that will
    /// be displayed when the element is folded.
    /// </summary>
    /// <remarks>
    /// Currently this puts all attributes from an element on the same
    /// line of the start tag.  It does not cater for elements where attributes
    /// are not on the same line as the start tag.
    /// </remarks>
    static string GetAttributeFoldText(XmlReader reader)
    {
      StringBuilder text = new StringBuilder();

      for (int i = 0; i < reader.AttributeCount; ++i)
      {
        reader.MoveToAttribute(i);

        text.Append(reader.Name);
        text.Append("=");
        text.Append(reader.QuoteChar.ToString());
        text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
        text.Append(reader.QuoteChar.ToString());

        // Append a space if this is not the
        // last attribute.
        if (i < reader.AttributeCount - 1)
        {
          text.Append(" ");
        }
      }

      return text.ToString();
    }

    /// <summary>
    /// Xml encode the attribute string since the string returned from
    /// the XmlTextReader is the plain unencoded string and .NET
    /// does not provide us with an xml encode method.
    /// </summary>
    internal static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
    {
      StringBuilder encodedValue = new StringBuilder(attributeValue);

      encodedValue.Replace("&", "&amp;");
      encodedValue.Replace("<", "&lt;");
      encodedValue.Replace(">", "&gt;");

      if (quoteChar == '"')
      {
        encodedValue.Replace("\"", "&quot;");
      }
      else
      {
        encodedValue.Replace("'", "&apos;");
      }

      return encodedValue.ToString();
    }
  }
}
