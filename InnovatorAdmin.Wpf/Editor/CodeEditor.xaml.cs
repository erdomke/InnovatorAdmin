using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using io = System.IO;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// Interaction logic for ExtendedEditor.xaml
  /// </summary>
  public partial class CodeEditor : UserControl, IDisposable
  {
    private static System.Windows.Media.FontFamily _fixedWidth = new System.Windows.Media.FontFamily("Consolas");
    private static System.Windows.Media.FontFamily _sansSerif = new System.Windows.Media.FontFamily("Helvetica");

    private FoldingManager _foldingManager;
    private IFoldingStrategy _foldingStrategy;
    private DispatcherTimer _foldingUpdateTimer = new DispatcherTimer();
    private IEditorHelper _helper;
    private Placeholder _placeholder;
    private bool _singleLine;

    public event EventHandler<RunRequestedEventArgs> RunRequested;
    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    public TextDocument Document
    {
      get { return Editor.Document; }
      set
      {
        Editor.Document = value;
        ResetFoldingManager();
      }
    }
    public IEditorHelper Helper
    {
      get { return _helper; }
      set
      {
        _helper = value;
        _foldingStrategy = value == null ? null : value.FoldingStrategy;
        this.editor.SyntaxHighlighting = value == null ? null : value.GetHighlighting();
      }
    }
    public string PlaceholderText
    {
      get { return _placeholder == null ? string.Empty : (_placeholder.PlaceholderText ?? string.Empty); }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
        {
          Editor.TextArea.TextView.BackgroundRenderers.Remove(_placeholder);
          _placeholder = null;
        }
        else
        {
          if (_placeholder == null)
          {
            _placeholder = new Placeholder();
            Editor.TextArea.TextView.BackgroundRenderers.Add(_placeholder);
          }
          _placeholder.PlaceholderText = value;
        }
      }
    }
    public bool ShowLineNumbers
    {
      get { return Editor.ShowLineNumbers; }
      set
      {
        Editor.ShowLineNumbers = value;
        if (!value)
          Editor.TextArea.LeftMargins.Clear();
      }
    }
    public bool ShowScrollbars
    {
      get { return Editor.HorizontalScrollBarVisibility != System.Windows.Controls.ScrollBarVisibility.Hidden; }
      set
      {
        Editor.HorizontalScrollBarVisibility = value
          ? System.Windows.Controls.ScrollBarVisibility.Auto
          : System.Windows.Controls.ScrollBarVisibility.Hidden;
        Editor.VerticalScrollBarVisibility = Editor.HorizontalScrollBarVisibility;
      }
    }
    public bool SingleLine
    {
      get { return _singleLine; }
      set
      {
        _singleLine = value;
        Editor.FontFamily = _singleLine
          ? _sansSerif
          : _fixedWidth;
      }
    }
    public bool ReadOnly
    {
      get { return Editor.IsReadOnly; }
      set { Editor.IsReadOnly = value; }
    }
    public string Text
    {
      get { return Editor.Text; }
      set
      {
        Editor.Document.Text = (value ?? string.Empty);
        Editor.CaretOffset = 0;
      }
    }

    public void ResetFoldingManager()
    {
      if (_foldingManager != null)
        FoldingManager.Uninstall(_foldingManager);
      _foldingManager = FoldingManager.Install(this.editor.TextArea);
      UpdateFoldings();
    }

    public ICSharpCode.AvalonEdit.TextEditor Editor
    {
      get { return this.editor; }
    }

    public CodeEditor()
    {
      InitializeComponent();

      _foldingManager = FoldingManager.Install(this.editor.TextArea);
      UpdateFoldings();

      _foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
      _foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
      _foldingUpdateTimer.Start();

      Editor.FontFamily = _fixedWidth;
      Editor.FontSize = 12.0;
      Editor.Options.ConvertTabsToSpaces = true;
      Editor.Options.EnableRectangularSelection = true;
      Editor.Options.IndentationSize = 2;
      Editor.ShowLineNumbers = true;
      Editor.TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
      Editor.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
      Editor.TextArea.TextEntering += TextArea_TextEntering;
      Editor.TextArea.TextEntered += TextArea_TextEntered;
      Editor.TextArea.KeyDown += TextArea_KeyDown;
      Editor.TextArea.TextView.MouseHover += TextView_MouseHover;
      //Editor.TextArea.TextView.MouseHoverStopped += TextView_MouseHoverStopped;
      //Editor.TextArea.TextView.VisualLinesChanged += TextView_VisualLinesChanged;
      Editor.TextArea.SelectionChanged += TextArea_SelectionChanged;
      Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
      //Editor.TextChanged += editor_TextChanged;

      Editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
    }

    public void CollapseAll()
    {
      UpdateFoldings();
      if (_foldingManager.AllFoldings.FirstOrDefault(f => f.Title == "<TestSuite>") != null)
      {
        // Unit test results
        foreach (var fm in _foldingManager.AllFoldings)
        {
          fm.IsFolded = !(fm.Title == "<TestSuite>" || fm.Title == "<Results>");
        }
      }
      else if (_foldingManager.AllFoldings.Any(IsAmlFolding))
      {
        // AML folding
        foreach (var fm in _foldingManager.AllFoldings)
        {
          fm.IsFolded = IsAmlFolding(fm);
        }
      }
      else
      {
        foreach (var fm in _foldingManager.AllFoldings)
        {
          fm.IsFolded = true;
        }
      }
    }
    public void ExpandAll()
    {
      UpdateFoldings();
      foreach (var fm in _foldingManager.AllFoldings)
      {
        fm.IsFolded = false;
      }
    }

    private void UpdateFoldings()
    {
      if (_foldingStrategy != null)
        _foldingStrategy.UpdateFoldings(_foldingManager, this.editor.Document);
    }

    private bool IsAmlFolding(FoldingSection f)
    {
      return f.Title.StartsWith("<Item") || f.Title.StartsWith("<Properties ") || f.Title.StartsWith("<Relationships");
    }

    private void CopyId_Click(object sender, RoutedEventArgs e)
    {
      var id = GetId(this.editor.Document, this.editor.TextArea.Caret);
      if (string.IsNullOrEmpty(id))
      {
        System.Windows.Clipboard.Clear();
      }
      else
      {
        System.Windows.Clipboard.SetText(id);
      }
    }

    private string GetId(ICSharpCode.AvalonEdit.Document.IDocument doc, ICSharpCode.AvalonEdit.Editing.Caret caret)
    {
      string amlQuery;
      var settings = new System.Xml.XmlReaderSettings();
      System.IO.TextReader reader;

      if (this.Helper == null)
      {
        reader = doc.CreateReader();
      }
      else
      {
        amlQuery = this.Helper.GetCurrentQuery(doc, caret.Offset);
        var loc = doc.GetLocation(doc.IndexOf(amlQuery, 0, doc.TextLength, StringComparison.Ordinal));
        reader = new System.IO.StringReader(amlQuery);
        settings.LineNumberOffset = loc.Line;
      }

      string lastItemId = null;
      string lastId = null;
      var elems = new Stack<string>();

      using (reader)
      using (var xmlReader = System.Xml.XmlReader.Create(reader))
      {
        var lineInfo = (System.Xml.IXmlLineInfo)xmlReader;
        while (xmlReader.Read())
        {
          switch (xmlReader.NodeType)
          {
            case System.Xml.XmlNodeType.Element:
            case System.Xml.XmlNodeType.EndElement:
              if (lineInfo.LineNumber > this.editor.TextArea.Caret.Line
                || (lineInfo.LineNumber == this.editor.TextArea.Caret.Line && (lineInfo.LinePosition - 1) > this.editor.TextArea.Caret.Column))
              {
                return lastId ?? lastItemId;
              }
              break;
          }

          switch (xmlReader.NodeType)
          {
            case System.Xml.XmlNodeType.Element:
              switch (xmlReader.LocalName)
              {
                case "Item":
                  lastItemId = xmlReader.GetAttribute("id");
                  lastId = xmlReader.GetAttribute("id");
                  break;
              }
              if (!xmlReader.IsEmptyElement) elems.Push(xmlReader.LocalName);
              break;
            case System.Xml.XmlNodeType.Text:
              if (xmlReader.Value.IsGuid())
              {
                switch (elems.Peek())
                {
                  case "id":
                    lastItemId = xmlReader.Value;
                    break;
                  default:
                    lastId = xmlReader.Value;
                    break;
                }
              }
              break;
            case System.Xml.XmlNodeType.EndElement:
              lastId = null;
              if (elems.Pop() == "Item")
              {
                lastItemId = null;
              }
              break;
          }
        }
      }

      return null;
    }

    public void Dispose()
    {
      _helper = null;
      _foldingManager = null;
      _foldingStrategy = null;
      if (_foldingUpdateTimer != null)
        _foldingUpdateTimer.Stop();
      _foldingUpdateTimer = null;
    }


    private void TextArea_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      try
      {
        var position = this.editor.GetPositionFromPoint(e.GetPosition(this.editor));
        if (position.HasValue)
        {
          this.editor.TextArea.Caret.Position = position.Value;
        }

        //var pos = e.GetPosition(Editor.TextArea);
        //var pt = new Point((int)pos.X, (int)pos.Y);
        //conMenu.Show(this.PointToScreen(pt));
        //while (conMenu.Items.Count > 9)
        //  conMenu.Items.RemoveAt(conMenu.Items.Count - 1);

        //if (Helper != null)
        //  EditorScript.BuildMenu(conMenu.Items
        //    , Helper.GetScripts(Document, Editor.CaretOffset)
        //    , (script) =>
        //    {
        //      var ide = this.FindForm() as EditorWindow;
        //      if (ide != null)
        //        ide.Execute(script);
        //    });
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private class Placeholder : IBackgroundRenderer
    {
      public string PlaceholderText { get; set; }

      public void Draw(TextView textView, DrawingContext drawingContext)
      {
        if (textView.Document.TextLength <= 0)
        {
          var text = new FormattedText(this.PlaceholderText
            , CultureInfo.CurrentCulture
            , System.Windows.FlowDirection.LeftToRight
            , new Typeface(_sansSerif
              , System.Windows.FontStyles.Normal
              , System.Windows.FontWeights.Normal
              , System.Windows.FontStretches.Normal)
            , textView.DefaultLineHeight - 2
            , System.Windows.Media.Brushes.DimGray);
          drawingContext.DrawText(text, new System.Windows.Point(1, 1));
        }
      }

      public KnownLayer Layer
      {
        get { return KnownLayer.Background; }
      }
    }

    void TextArea_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      try
      {
        if (e.Key == System.Windows.Input.Key.Enter && this.SingleLine
          && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
        {
          OnRunRequested(new RunRequestedEventArgs(this.Text));
          e.Handled = true;
        }
        else if (e.Key == System.Windows.Input.Key.Tab && this.SingleLine &&
          (e.KeyboardDevice.Modifiers & ~(System.Windows.Input.ModifierKeys.Shift)) == System.Windows.Input.ModifierKeys.None)
        {
          e.Handled = true;
          MoveToNextUIElement(e);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    void MoveToNextUIElement(KeyEventArgs e)
    {
      // Creating a FocusNavigationDirection object and setting it to a
      // local field that contains the direction selected.
      FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

      // MoveFocus takes a TraveralReqest as its argument.
      TraversalRequest request = new TraversalRequest(focusDirection);

      // Gets the element with keyboard focus.
      UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

      // Change keyboard focus.
      if (elementWithFocus != null)
      {
        if (elementWithFocus.MoveFocus(request)) e.Handled = true;
      }
    }

    void Caret_PositionChanged(object sender, EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged.Invoke(sender, new SelectionChangedEventArgs(Editor.TextArea));
    }

    void TextArea_SelectionChanged(object sender, EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged.Invoke(sender, new SelectionChangedEventArgs(Editor.TextArea));
    }


    void TextView_MouseHover(object sender, System.Windows.Input.MouseEventArgs e)
    {
      try
      {
        //var pos = _extEditor.Editor.GetPositionFromPoint(e.GetPosition(_extEditor.Editor));
        //if (pos.HasValue)
        //{
        //  var visLine = _extEditor.Editor.TextArea.TextView.GetVisualLine(pos.Value.Line);
        //  if (visLine != null)
        //  {
        //    var link = visLine.Elements.OfType<VisualLineLinkText>()
        //      .FirstOrDefault(l => l.VisualColumn <= pos.Value.VisualColumn
        //        && pos.Value.VisualColumn < (l.VisualColumn + l.VisualLength));
        //    if (link != null && _toolTip == null)
        //    {
        //      _toolTip = new System.Windows.Controls.ToolTip();
        //      _toolTip.Closed += _toolTip_Closed;
        //      _toolTip.PlacementTarget = _extEditor.Editor;
        //      _toolTip.Content = "CTRL+click to follow link";
        //      _toolTip.IsOpen = true;
        //      e.Handled = true;
        //    }
        //  }
        //}
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    //void _toolTip_Closed(object sender, System.Windows.RoutedEventArgs e)
    //{
    //  _toolTip.Closed -= _toolTip_Closed;
    //  _toolTip = null;
    //}

    //void TextView_VisualLinesChanged(object sender, EventArgs e)
    //{
    //  if (_toolTip != null)
    //  {
    //    _toolTip.IsOpen = false;
    //  }
    //}

    //void TextView_MouseHoverStopped(object sender, System.Windows.Input.MouseEventArgs e)
    //{
    //  if (_toolTip != null)
    //  {
    //    _toolTip.IsOpen = false;
    //    e.Handled = true;
    //  }
    //}


    public IList<VisualLineElementGenerator> ElementGenerators
    {
      get { return Editor.TextArea.TextView.ElementGenerators; }
    }

    protected virtual void OnRunRequested(RunRequestedEventArgs e)
    {
      if (RunRequested != null) RunRequested.Invoke(this, e);
    }

    private string GetCurrentQuery()
    {
      if (Editor.TextArea.Selection.IsEmpty)
      {
        var query = Helper.GetCurrentQuery(Editor.Document, Editor.CaretOffset);
        return string.IsNullOrEmpty(query) ? Editor.Text : query;
      }
      var doc = Editor.Document;
      var start = doc.GetOffset(Editor.TextArea.Selection.StartPosition.Location);
      var end = doc.GetOffset(Editor.TextArea.Selection.EndPosition.Location);
      if (end < start)
      {
        var buffer = end;
        end = start;
        start = buffer;
      }
      return doc.GetText(start, end - start);
    }


    void TextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      try
      {
        var key = (e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key);

        // F5 or F9
        if ((key == System.Windows.Input.Key.F9 || key == System.Windows.Input.Key.F5)
          && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
        {
          OnRunRequested(new RunRequestedEventArgs(Editor.Text));
          e.Handled = true;
        }
        // Ctrl+Enter or Ctrl+Shift+E
        else if ((key == System.Windows.Input.Key.Enter && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control)
          || (key == System.Windows.Input.Key.E
            && e.KeyboardDevice.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)))
        {
          OnRunRequested(new RunRequestedEventArgs(GetCurrentQuery()));
          e.Handled = true;
        }
        // Ctrl+Shift+Up
        else if (!SingleLine
          && ((key == System.Windows.Input.Key.Up
              && e.KeyboardDevice.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
            || (key == System.Windows.Input.Key.Up && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Alt)))
        {
          MoveLineUp();
          e.Handled = true;
        }
        // Ctrl+Shift+Down
        else if (!SingleLine
          && ((key == System.Windows.Input.Key.Down
              && e.KeyboardDevice.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
            || (key == System.Windows.Input.Key.Down && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Alt)))
        {
          MoveLineDown();
          e.Handled = true;
        }
        // Ctrl+Shift+U
        else if (key == System.Windows.Input.Key.U
          && e.KeyboardDevice.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
        {
          TransformUppercase();
          e.Handled = true;
        }
        // Ctrl+U
        else if (key == System.Windows.Input.Key.U && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control)
        {
          TransformLowercase();
          e.Handled = true;
        }
        // Ctrl+Space
        else if (key == System.Windows.Input.Key.Space && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Control && Helper != null)
        {
          Helper.ShowCompletions(this);
          e.Handled = true;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


    public void ReplaceSelectionSegments(Func<string, string> insert)
    {
      if (Editor.IsReadOnly) return;

      if (Editor.TextArea.Selection is RectangleSelection)
      {
        var doc = Editor.Document;
        using (doc.RunUpdate())
        {
          var sel = Editor.TextArea.Selection;
          var segs = sel.Segments.Select(s => Tuple.Create(s.StartOffset, s.Length)).ToArray();
          var start = sel.StartPosition;
          var end = sel.EndPosition.Location;
          var delta = 0;
          var lastDiff = 0;
          string replacement;
          foreach (var seg in segs)
          {
            replacement = insert.Invoke(doc.GetText(seg.Item1 + delta, seg.Item2));
            doc.Replace(seg.Item1 + delta, seg.Item2, replacement);
            lastDiff = replacement.Length - seg.Item2;
            delta += lastDiff;
          }
          Editor.TextArea.Selection = new RectangleSelection(Editor.TextArea, start,
            new TextViewPosition(end.Line, end.Column + lastDiff));
        }
      }
      else if (Editor.TextArea.Selection.IsEmpty)
      {
        Editor.Document.Insert(Editor.CaretOffset, insert.Invoke(string.Empty));
      }
      else
      {
        Editor.SelectedText = insert.Invoke(Editor.SelectedText);
      }
    }

    private void OffsetLines(int offset)
    {
      offset = Math.Sign(offset);
      if (offset == 0) return;

      var loc = Editor.TextArea.Caret.Location;
      var start = Editor.TextArea.Selection.IsEmpty ? loc : Editor.TextArea.Selection.StartPosition.Location;
      var end = Editor.TextArea.Selection.IsEmpty ? loc : Editor.TextArea.Selection.EndPosition.Location;
      var isRectangle = Editor.TextArea.Selection is RectangleSelection;
      var doc = Editor.Document;

      if ((offset < 0 && start.Line > 1)
        || (offset > 0 && start.Line < doc.LineCount))
      {
        using (doc.RunUpdate())
        {
          var firstLine = doc.GetLineByNumber(start.Line);
          var lastLine = end.Line == start.Line
            ? firstLine
            : (end.Line == start.Line + 1
              ? firstLine.NextLine
              : doc.GetLineByNumber(end.Line));
          var text = doc.GetText(firstLine.Offset, (lastLine.Offset + lastLine.TotalLength) - firstLine.Offset);

          if (offset > 0)
          {
            var nextLine = lastLine.NextLine;
            doc.Remove(firstLine.Offset, text.Length);
            doc.Insert(firstLine.Offset + nextLine.TotalLength, text);
          }
          else
          {
            var prevLine = firstLine.PreviousLine;
            doc.Remove(firstLine.Offset, text.Length);
            doc.Insert(prevLine.Offset, text);
          }

          if (!start.Equals(end))
          {
            if (isRectangle)
            {
              Editor.TextArea.Selection = new RectangleSelection(Editor.TextArea
                , new TextViewPosition(start.Line + offset, start.Column)
                , new TextViewPosition(end.Line + offset, end.Column));
            }
            else
            {
              var startOffset = doc.GetOffset(start.Line + offset, start.Column);
              var endOffset = doc.GetOffset(end.Line + offset, end.Column);
              Editor.SelectionStart = startOffset;
              Editor.SelectionLength = endOffset - startOffset;
            }
          }
          Editor.TextArea.Caret.Location = new TextLocation(loc.Line + offset, loc.Column);
        }
      }
    }

    public void MoveLineDown()
    {
      OffsetLines(1);
    }

    public void MoveLineUp()
    {
      OffsetLines(-1);
    }

    public void TransformUppercase()
    {
      ReplaceSelectionSegments(t => t.ToUpperInvariant());
    }
    public void TransformLowercase()
    {
      ReplaceSelectionSegments(t => t.ToLowerInvariant());
    }


    CompletionWindowEx completionWindow;

    public void ShowCompletionWindow(IEnumerable<ICompletionData> completionItems, int overlap)
    {
      if (completionItems.Any())
      {
        //if (completionWindow != null)
        //  completionWindow.Close();

        completionWindow = new CompletionWindowEx(Editor.TextArea);
        completionWindow.StartOffset -= overlap;
        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
        foreach (var item in completionItems)
        {
          data.Add(item);
        }
        completionWindow.Show();
        completionWindow.AutoGrow();
        completionWindow.CompletionList.IsFiltering = false;
        completionWindow.CompletionList.SelectItem(completionItems.First().Text);
        completionWindow.CompletionList.IsFiltering = true;
        completionWindow.Closed += delegate
        {
          completionWindow = null;
        };
      }
    }
    public void HideCompletionWindow()
    {
      if (completionWindow != null)
        completionWindow.Close();
    }

    void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      try
      {
        if (this.Helper != null) this.Helper.HandleTextEntered(this, e.Text);
      }
      // hide this exception as a problem in completions should cause the use issues
      catch (Exception) { }
    }


    void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      if (e.Text.Length > 0 && completionWindow != null)
      {
        if (!completionWindow.CompletionList.CompletionData.OfType<SqlGeneralCompletionData>().Any())
        {
          switch (e.Text[0])
          {
            case '"':
            case '\'':
              // Whenever a non-letter is typed while the completion window is open,
              // insert the currently selected element.
              completionWindow.CompletionList.RequestInsertion(e);
              break;
            case '>':
              if (Editor.CaretOffset > 0)
              {
                var prev = Editor.Document.GetCharAt(Editor.CaretOffset - 1);
                if (prev != ' ')
                  completionWindow.CompletionList.RequestInsertion(e);
              }
              break;
            case ' ':
              if (!completionWindow.CompletionList.CompletionData.Any(d => d.Text.IndexOf(' ') >= 0))
                completionWindow.CompletionList.RequestInsertion(e);
              break;
          }
        }
      }
      // Do not set e.Handled=true.
      // We still want to insert the character that was typed.
    }


    private class CompletionWindowEx : ICSharpCode.AvalonEdit.CodeCompletion.CompletionWindow
    {
      private bool _allowEnter = false;
      private double _minWidth = 0;

      public CompletionWindowEx(TextArea textArea) : base(textArea) { }

      protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
      {
        switch (e.Key)
        {
          case System.Windows.Input.Key.Prior:
          case System.Windows.Input.Key.Next:
          case System.Windows.Input.Key.Up:
          case System.Windows.Input.Key.Down:
            _allowEnter = true;
            break;
        }

        if (!_allowEnter &&
          (e.Key == System.Windows.Input.Key.Enter
          || e.Key == System.Windows.Input.Key.End
          || e.Key == System.Windows.Input.Key.Home))
        {
          this.Close();
        }
        else
        {
          base.OnKeyDown(e);
        }
        AutoGrow();
      }

      public void AutoGrow()
      {
        var addition = this.CompletionList.ScrollViewer.ExtentWidth - this.CompletionList.ScrollViewer.ViewportWidth;
        var client = this.TextArea.PointFromScreen(new System.Windows.Point(this.Left, this.Top));
        addition = Math.Max(0, Math.Min(addition, this.TextArea.ActualWidth - this.Width - client.X));
        this.Width += addition;
      }
    }
  }


  public class RunRequestedEventArgs : EventArgs
  {
    private string _query;
    public string Query
    {
      get { return _query; }
    }

    public RunRequestedEventArgs(string query)
    {
      _query = query;
    }
  }

  public class SelectionChangedEventArgs : EventArgs
  {
    private TextArea _area;

    public int CaretLine
    {
      get { return _area.Caret.Line; }
    }
    public int CaretColumn
    {
      get { return _area.Caret.Column; }
    }
    public int SelectionLength
    {
      get { return _area.Selection.Length; }
    }
    public string GetText(int length = -1)
    {
      if (length < 0)
        return _area.Selection.GetText();
      if (!_area.Selection.Segments.Any())
        return string.Empty;

      var ch = new char[length];
      var i = 0;
      foreach (var segment in _area.Selection.Segments)
      {
        using (var reader = _area.Document.CreateReader(segment.StartOffset, segment.Length))
        {
          i += reader.Read(ch, i, length - i);
          if (i >= length)
            return new string(ch, 0, i);
        }
      }

      return new string(ch, 0, i);
    }

    public SelectionChangedEventArgs(TextArea area)
    {
      _area = area;
    }
  }
}
