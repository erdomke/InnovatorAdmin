using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Globalization;
using System.IO;
using System.Xml;
using GotDotNet.XPath;
using Mvp.Xml.XPointer;
using System.Xml.XPath;

namespace InnovatorAdmin.Editor
{
  public partial class FindReplace : Form
  {
    private TextEditor _editor;
    private SearchResultBackgroundRenderer _renderer;
    private ISearchStrategy _strategy;
    private SearchMode _mode = SearchMode.Normal;
    private bool _programChangingCheckState;
    private TextDocument _currentDoc;
    private Control _positionParent;

    public enum SearchMode
    {
      /// <summary>Standard search</summary>
      Normal,
      /// <summary>RegEx search</summary>
      RegEx,
      /// <summary>Wildcard search</summary>
      Wildcard,
      _FirstCustom,
      Extended,
      XPath
    }

    public FindReplace(EditorControl editor)
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      _editor = editor.Editor;
      _positionParent = editor;
      _renderer = new SearchResultBackgroundRenderer();

      txtFind.TextChanged += txtFind_TextChanged;
      DisplayReplace(false);

      this.StartPosition = FormStartPosition.Manual;

      var offset = Properties.Settings.Default.FindReplace_Location;
      if (offset == Point.Empty)
      {
        offset = new Point(editor.Width - this.DesktopBounds.Width, 0);
      }

      SearchMode newMode;
      if (Enum.TryParse<SearchMode>(Properties.Settings.Default.FindReplace_LastMode, out newMode))
      {
        SetMode(newMode);
      }

      var relative = editor.PointToScreen(editor.Location);
      relative.Offset(offset);
      var screenDim = SystemInformation.VirtualScreen;
      var newX = Math.Min(Math.Max(relative.X, 0), screenDim.Width - this.DesktopBounds.Width);
      var newY = Math.Min(Math.Max(relative.Y, 0), screenDim.Height - this.DesktopBounds.Height);
      this.DesktopLocation = new Point(newX, newY);
    }

    void txtFind_TextChanged(object sender, EventArgs e)
    {
      UpdateSearch();
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
      this.Opacity = 1.0;
      txtFind.Focus();
    }
    protected override void OnDeactivate(EventArgs e)
    {
      base.OnDeactivate(e);
      this.Opacity = 0.3;
    }
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      _editor.TextArea.TextView.BackgroundRenderers.Add(_renderer);
      _currentDoc = _editor.Document;
      if (_currentDoc != null)
      {
        _currentDoc.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
      }
      _editor.TextArea.DocumentChanged += new EventHandler(this.textArea_DocumentChanged);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      SaveFormBounds();
      _editor.TextArea.TextView.BackgroundRenderers.Remove(_renderer);
      _editor.TextArea.DocumentChanged -= new EventHandler(this.textArea_DocumentChanged);
      if (_currentDoc != null)
      {
        _currentDoc.TextChanged -= new EventHandler(this.textArea_Document_TextChanged);
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnFind_Click(object sender, EventArgs e)
    {
      FindNext();
    }

    private void FindNext()
    {
      var searchResult = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(_editor.TextArea.Caret.Offset + 1);
      if (searchResult == null)
      {
        searchResult = _renderer.CurrentResults.FirstSegment;
      }
      if (searchResult != null)
      {
        this.SelectResult(searchResult);
      }
    }

    private void UpdateSearch()
    {
      var mode = _mode;
      var searchText = txtFind.Text;

      if (mode == SearchMode.Extended)
      {
        mode = SearchMode.Normal;
        searchText = StringHelper.StringFromCSharpLiteral(searchText);
      }

      if (mode == SearchMode.XPath)
      {
        this._strategy = new XPathSearchStrategy(searchText);
      }
      else
      {
        this._strategy = SearchStrategyFactory.Create(searchText ?? ""
          , !chkCaseSensitive.Checked, false
          , (ICSharpCode.AvalonEdit.Search.SearchMode)mode);
      }
      this.DoSearch(true);
    }

    private void DisplayReplace(bool visible)
    {
      txtReplace.Visible = visible;
      btnReplaceAll.Visible = visible;
      btnReplaceNext.Visible = visible;
      this.Height = visible ? 120 : 90;
      btnShowReplace.Text = visible ? "▲" : "▼";
    }

    private void DoSearch(bool changeSelection)
    {
      if (this.IsDisposed)
        return;

      _renderer.CurrentResults.Clear();
      if (!string.IsNullOrEmpty(txtFind.Text))
      {
        var offset = _editor.TextArea.Caret.Offset;
        if (changeSelection)
          _editor.TextArea.ClearSelection();

        foreach (var searchResult in _strategy.FindAll(_editor.TextArea.Document, 0
          , _editor.TextArea.Document.TextLength).OfType<TextSegment>())
        {
          if (changeSelection && searchResult.StartOffset >= offset)
          {
            this.SelectResult(searchResult);
            changeSelection = false;
          }
          _renderer.CurrentResults.Add(searchResult);
        }
        //if (!_renderer.CurrentResults.Any<SearchResult>())
        //{
        //  this.messageView.IsOpen = true;
        //  this.messageView.Content = this.Localization.NoMatchesFoundText;
        //  this.messageView.PlacementTarget = this.searchTextBox;
        //}
        //else
        //{
        //  this.messageView.IsOpen = false;
        //}
      }
      _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
    }

    private void SelectResult(TextSegment result)
    {
      _editor.TextArea.Caret.Offset = result.StartOffset;
      _editor.TextArea.Selection = Selection.Create(_editor.TextArea, result.StartOffset, result.EndOffset);
      _editor.TextArea.Caret.BringCaretToView();
      _editor.TextArea.Caret.Show();
    }

    private void chkNormal_CheckedChanged(object sender, EventArgs e)
    {
      SetMode(SearchMode.Normal);
    }

    private void chkExtended_CheckedChanged(object sender, EventArgs e)
    {
      SetMode(SearchMode.Extended);
    }

    private void chkRegExp_CheckedChanged(object sender, EventArgs e)
    {
      SetMode(SearchMode.RegEx);
    }

    private void chkXPath_CheckedChanged(object sender, EventArgs e)
    {
      SetMode(SearchMode.XPath);
    }

    private void SetMode(SearchMode mode)
    {
      if (!_programChangingCheckState)
      {
        _programChangingCheckState = true;
        try
        {
          chkNormal.Checked = false;
          chkExtended.Checked = false;
          chkRegExp.Checked = false;
          chkXPath.Checked = false;
          switch (mode)
          {
            case SearchMode.Extended:
              chkExtended.Checked = true;
              break;
            case SearchMode.Normal:
              chkNormal.Checked = true;
              break;
            case SearchMode.RegEx:
              chkRegExp.Checked = true;
              break;
            case SearchMode.XPath:
              chkXPath.Checked = true;
              break;
          }
          _mode = mode;
          UpdateSearch();
        }
        finally
        {
          _programChangingCheckState = false;
        }
      }

    }

    private void txtFind_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Enter:
          FindNext();
          break;
      }
    }

    private void textArea_Document_TextChanged(object sender, EventArgs e)
    {
      this.DoSearch(false);
    }

    private void textArea_DocumentChanged(object sender, EventArgs e)
    {
      if (_currentDoc != null)
      {
        _currentDoc.TextChanged -= new EventHandler(this.textArea_Document_TextChanged);
      }
      _currentDoc = _editor.TextArea.Document;
      if (_currentDoc != null)
      {
        _currentDoc.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
        this.DoSearch(false);
      }
    }

    private class XPathSearchResult : TextSegment, ISearchResult
    {
      public string ReplaceWith(string replacement)
      {
        return replacement;
      }
    }

    private class XPathSearchStrategy : ISearchStrategy
    {
      private string _xPath;

      public XPathSearchStrategy(string xPath)
      {
        _xPath = xPath;
      }


      public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
      {
        try
        {
          return FindAllValidating(document, offset, length).ToList();
        }
        catch (XmlException)
        {
          return FindAllForwardOnly(document, offset, length);
        }
        catch (Exception)
        {
          return Enumerable.Empty<ISearchResult>();
        }
      }

      public IEnumerable<ISearchResult> FindAllValidating(ITextSource document, int offset, int length)
      {
        using (var stream = document.CreateReader())
        {
          var doc = (IDocument)document;
          var xmlDoc = new XPathDocument(stream);;
          var navigator = xmlDoc.CreateNavigator();

          XPathExpression expr = null;
          XPathNodeIterator iterator;
          try
          {
            expr = navigator.Compile(_xPath);
            iterator = navigator.Select(expr);
          }
          catch (System.Xml.XPath.XPathException)
          {
            yield break;
          }

          while (iterator.MoveNext())
          {
            var current = iterator.Current;
            var segment = XmlSegment(doc, ((IXmlLineInfo)current).LineNumber, ((IXmlLineInfo)current).LinePosition);
            if (segment != null && segment.Offset >= offset && segment.EndOffset <= (offset + length))
            {
              yield return new XPathSearchResult()
              {
                StartOffset = segment.Offset,
                Length = segment.Length
              };
            }
          }
        }
      }

      public IEnumerable<ISearchResult> FindAllForwardOnly(ITextSource document, int offset, int length)
      {
        var xc = new XPathCollection();
        try
        {
          xc.Add(_xPath);
        }
        catch (Exception)
        {
          yield break;
        }
        using (var reader = new XmlTextReader(document.CreateReader()))
        using (var xpathReader = new XPathReader(reader, xc))
        {
          var lineInfo = xpathReader as IXmlLineInfo;
          var doc = (IDocument)document;
          ISegment segment;

          while (Read(xpathReader))
          {
            if (xpathReader.Match(0) && xpathReader.NodeType != XmlNodeType.EndElement)
            {
              segment = null;
              try
              {
                segment = XmlSegment(doc, lineInfo.LineNumber, lineInfo.LinePosition);
              }
              catch (Exception) { }
              if (segment != null && segment.Offset >= offset && segment.EndOffset <= (offset + length))
              {
                yield return new XPathSearchResult()
                {
                  StartOffset = segment.Offset,
                  Length = segment.Length
                };
              }
            }
          }
        }
      }

      private bool Read(XmlReader reader)
      {
        try
        {
          return reader.Read();
        }
        catch (Exception)
        {
          return false;
        }
      }

      private ISegment XmlSegment(IDocument doc, int lineNumber, int linePosition)
      {
        if (lineNumber < 1) return null;
        var offset = doc.GetOffset(lineNumber, linePosition) - 1;
        using (var stream = new StringReader(doc.GetText(offset, doc.TextLength - offset)))
        using (var reader = new XmlTextReader(stream))
        {
          while (reader.Read())
          {
            try
            {
              reader.ReadOuterXml();
            }
            catch (XmlException)
            {
              return null;
            }

            if (reader.LineNumber == 1)
            {
              return new TextSegment()
              {
                StartOffset = offset,
                Length = reader.LinePosition
              };
            }
            else
            {
              return new TextSegment()
              {
                StartOffset = offset,
                Length = doc.GetOffset(reader.LineNumber + lineNumber - 1, reader.LinePosition) - offset
              };
            }
          }
        }

        return null;
      }

      public ISearchResult FindNext(ITextSource document, int offset, int length)
      {
        return this.FindAll(document, offset, length).FirstOrDefault<ISearchResult>();
      }

      public bool Equals(ISearchStrategy other)
      {
        var xPath = other as XPathSearchStrategy;
        return xPath != null && xPath._xPath == this._xPath;
      }
    }

    /// <summary>
    /// This static class involves helper methods that use strings.
    /// </summary>
    private static class StringHelper
    {
      /// <summary>
      /// Converts a C# literal string into a normal string.
      /// </summary>
      /// <param name="source">Source C# literal string.</param>
      /// <returns>
      /// Normal string representation.
      /// </returns>
      public static string StringFromCSharpLiteral(string source)
      {
        StringBuilder sb = new StringBuilder(source.Length);
        int pos = 0;
        while (pos < source.Length)
        {
          char c = source[pos];
          if (c == '\\')
          {
            // --- Handle escape sequences
            pos++;
            if (pos >= source.Length) throw new ArgumentException("Missing escape sequence");
            switch (source[pos])
            {
              // --- Simple character escapes
              case '\'': c = '\''; break;
              case '\"': c = '\"'; break;
              case '\\': c = '\\'; break;
              case '0': c = '\0'; break;
              case 'a': c = '\a'; break;
              case 'b': c = '\b'; break;
              case 'f': c = '\f'; break;
              case 'n': c = ' '; break;
              case 'r': c = ' '; break;
              case 't': c = '\t'; break;
              case 'v': c = '\v'; break;
              case 'x':
                // --- Hexa escape (1-4 digits)
                StringBuilder hexa = new StringBuilder(10);
                pos++;
                if (pos >= source.Length)
                  throw new ArgumentException("Missing escape sequence");
                c = source[pos];
                if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                  hexa.Append(c);
                  pos++;
                  if (pos < source.Length)
                  {
                    c = source[pos];
                    if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                    {
                      hexa.Append(c);
                      pos++;
                      if (pos < source.Length)
                      {
                        c = source[pos];
                        if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                          (c >= 'A' && c <= 'F'))
                        {
                          hexa.Append(c);
                          pos++;
                          if (pos < source.Length)
                          {
                            c = source[pos];
                            if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                              (c >= 'A' && c <= 'F'))
                            {
                              hexa.Append(c);
                              pos++;
                            }
                          }
                        }
                      }
                    }
                  }
                }
                c = (char)Int32.Parse(hexa.ToString(), NumberStyles.HexNumber);
                pos--;
                break;
              case 'u':
                // Unicode hexa escape (exactly 4 digits)
                pos++;
                if (pos + 3 >= source.Length)
                  throw new ArgumentException("Unrecognized escape sequence");
                try
                {
                  uint charValue = UInt32.Parse(source.Substring(pos, 4),
                    NumberStyles.HexNumber);
                  c = (char)charValue;
                  pos += 3;
                }
                catch (SystemException)
                {
                  throw new ArgumentException("Unrecognized escape sequence");
                }
                break;
              case 'U':
                // Unicode hexa escape (exactly 8 digits, first four must be 0000)
                pos++;
                if (pos + 7 >= source.Length)
                  throw new ArgumentException("Unrecognized escape sequence");
                try
                {
                  uint charValue = UInt32.Parse(source.Substring(pos, 8),
                    NumberStyles.HexNumber);
                  if (charValue > 0xffff)
                    throw new ArgumentException("Unrecognized escape sequence");
                  c = (char)charValue;
                  pos += 7;
                }
                catch (SystemException)
                {
                  throw new ArgumentException("Unrecognized escape sequence");
                }
                break;
              default:
                throw new ArgumentException("Unrecognized escape sequence");
            }
          }
          pos++;
          sb.Append(c);
        }
        return sb.ToString();
      }

      /// <summary>
      /// Converts a C# verbatim literal string into a normal string.
      /// </summary>
      /// <param name="source">Source C# literal string.</param>
      /// <returns>
      /// Normal string representation.
      /// </returns>
      public static string StringFromVerbatimLiteral(string source)
      {
        StringBuilder sb = new StringBuilder(source.Length);
        int pos = 0;
        while (pos < source.Length)
        {
          char c = source[pos];
          if (c == '\"')
          {
            // --- Handle escape sequences
            pos++;
            if (pos >= source.Length) throw new ArgumentException("Missing escape sequence");
            if (source[pos] == '\"') c = '\"';
            else throw new ArgumentException("Unrecognized escape sequence");
          }
          pos++;
          sb.Append(c);
        }
        return sb.ToString();
      }

      /// <summary>
      /// Converts a C# literal string into a normal character..
      /// </summary>
      /// <param name="source">Source C# literal string.</param>
      /// <returns>
      /// Normal char representation.
      /// </returns>
      public static char CharFromCSharpLiteral(string source)
      {
        string result = StringFromCSharpLiteral(source);
        if (result.Length != 1)
          throw new ArgumentException("Invalid char literal");
        return result[0];
      }
    }

    private void btnShowReplace_Click(object sender, EventArgs e)
    {
      DisplayReplace(!txtReplace.Visible);
    }


    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      SaveFormBounds();
    }
    protected override void OnResizeEnd(EventArgs e)
    {
      base.OnResizeEnd(e);
      SaveFormBounds();
    }
    private void SaveFormBounds()
    {
      Properties.Settings.Default.FindReplace_LastMode = _mode.ToString();
      var relative = _positionParent.PointToScreen(_positionParent.Location);
      Properties.Settings.Default.FindReplace_Location = new Point(
        this.DesktopLocation.X - relative.X, this.DesktopLocation.Y - relative.Y
      );
      Properties.Settings.Default.Save();
    }

  }
}
