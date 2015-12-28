using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  public class EditorWinForm : UserControl
  {
    private static System.Windows.Media.FontFamily _fixedWidth = new System.Windows.Media.FontFamily("Consolas");
    private static System.Windows.Media.FontFamily _sansSerif = new System.Windows.Media.FontFamily("Helvetica");

    private System.Windows.Controls.ToolTip _toolTip;
    private ExtendedEditor _extEditor;
    private List<IDisposable> _itemsToDispose = new List<IDisposable>();
    private bool _singleLine;
    private Placeholder _placeholder;

    public event EventHandler<RunRequestedEventArgs> RunRequested;
    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEditorHelper Helper {
      get { return _extEditor.Helper; }
      set { _extEditor.Helper = value; }
    }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ICSharpCode.AvalonEdit.TextEditor Editor { get { return _extEditor.Editor; } }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextDocument Document
    {
      get { return _extEditor.Editor.Document; }
      set
      {
        _extEditor.Editor.Document = value;
        _extEditor.ResetFoldingManager();
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
      get { return _extEditor.Editor.ShowLineNumbers; }
      set
      {
        _extEditor.Editor.ShowLineNumbers = value;
        if (!value)
          _extEditor.Editor.TextArea.LeftMargins.Clear();
      }
    }
    public bool ShowScrollbars
    {
      get { return _extEditor.Editor.HorizontalScrollBarVisibility != System.Windows.Controls.ScrollBarVisibility.Hidden; }
      set
      {
        _extEditor.editor.HorizontalScrollBarVisibility = value
          ? System.Windows.Controls.ScrollBarVisibility.Auto
          : System.Windows.Controls.ScrollBarVisibility.Hidden;
        _extEditor.editor.VerticalScrollBarVisibility = _extEditor.editor.HorizontalScrollBarVisibility;
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
    public override string Text
    {
      get { return Editor.Text; }
      set
      {
        Editor.Document.Text = (value ?? string.Empty);
        Editor.CaretOffset = 0;
      }
    }

    public EditorWinForm()
    {
      var host = new ElementHost();
      host.Size = new Size(200, 100);
      host.Location = new Point(100, 100);
      host.Dock = DockStyle.Fill;

      _extEditor = new ExtendedEditor();
      _extEditor.Host = this;

      var editor = _extEditor.editor;
      editor.FontFamily = _fixedWidth;
      editor.FontSize = 12.0;
      editor.Options.ConvertTabsToSpaces = true;
      editor.Options.EnableRectangularSelection = true;
      editor.Options.IndentationSize = 2;
      editor.ShowLineNumbers = true;
      editor.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
      editor.TextArea.TextEntering += TextArea_TextEntering;
      editor.TextArea.TextEntered += TextArea_TextEntered;
      editor.TextArea.KeyDown += TextArea_KeyDown;
      editor.TextArea.TextView.MouseHover += TextView_MouseHover;
      editor.TextArea.TextView.MouseHoverStopped += TextView_MouseHoverStopped;
      editor.TextArea.TextView.VisualLinesChanged += TextView_VisualLinesChanged;
      editor.TextArea.SelectionChanged += TextArea_SelectionChanged;
      editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
      editor.TextChanged += editor_TextChanged;

      host.Child = _extEditor;

      editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

      this.Controls.Add(host);
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


    void editor_TextChanged(object sender, EventArgs e)
    {
      OnTextChanged(e);
    }

    void TextArea_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Enter && this.SingleLine && !IsControlDown(e.KeyboardDevice) && !IsAltDown(e.KeyboardDevice) && !IsShiftDown(e.KeyboardDevice))
      {
        OnRunRequested(new RunRequestedEventArgs(this.Text));
        e.Handled = true;
      }
      else if (e.Key == System.Windows.Input.Key.Tab && this.SingleLine && !IsControlDown(e.KeyboardDevice) && !IsAltDown(e.KeyboardDevice))
      {
        e.Handled = true;
        var frm = this.FindForm();
        frm.SelectNextControl(this, !IsShiftDown(e.KeyboardDevice), true, true, true);
      }
    }

    void Caret_PositionChanged(object sender, EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged.Invoke(sender, new SelectionChangedEventArgs(_extEditor.Editor.TextArea));
    }

    void TextArea_SelectionChanged(object sender, EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged.Invoke(sender, new SelectionChangedEventArgs(_extEditor.Editor.TextArea));
    }

    void TextView_VisualLinesChanged(object sender, EventArgs e)
    {
      if (_toolTip != null)
      {
        _toolTip.IsOpen = false;
      }
    }

    void TextView_MouseHoverStopped(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (_toolTip != null)
      {
        _toolTip.IsOpen = false;
        e.Handled = true;
      }
    }

    public new void Focus()
    {
      _extEditor.Editor.Focus();
    }

    void TextView_MouseHover(object sender, System.Windows.Input.MouseEventArgs e)
    {
      var pos = _extEditor.Editor.GetPositionFromPoint(e.GetPosition(_extEditor.Editor));
      if (pos.HasValue)
      {
        var visLine = _extEditor.Editor.TextArea.TextView.GetVisualLine(pos.Value.Line);
        if (visLine != null)
        {
          var link = visLine.Elements.OfType<VisualLineLinkText>()
            .FirstOrDefault(l => l.VisualColumn <= pos.Value.VisualColumn
              && pos.Value.VisualColumn < (l.VisualColumn + l.VisualLength));
          if (link != null && _toolTip == null)
          {
            _toolTip = new System.Windows.Controls.ToolTip();
            _toolTip.Closed += _toolTip_Closed;
            _toolTip.PlacementTarget = _extEditor.Editor;
            _toolTip.Content = "CTRL+click to follow link";
            _toolTip.IsOpen = true;
            e.Handled = true;
          }
        }
      }
    }

    void _toolTip_Closed(object sender, System.Windows.RoutedEventArgs e)
    {
      _toolTip.Closed -= _toolTip_Closed;
      _toolTip = null;
    }

    public IList<VisualLineElementGenerator> ElementGenerators
    {
      get { return _extEditor.Editor.TextArea.TextView.ElementGenerators; }
    }

    public void CollapseAll()
    {
      _extEditor.CollapseAll();
    }


    protected virtual void OnRunRequested(RunRequestedEventArgs e)
    {
      if (RunRequested != null) RunRequested.Invoke(this, e);
    }

    private string GetCurrentQuery()
    {
      if (Editor.TextArea.Selection.IsEmpty)
      {
        var query = Helper.GetCurrentQuery(Editor.Text, Editor.CaretOffset);
        return string.IsNullOrEmpty(query) ? Editor.Text : query;
      }
      var doc = Editor.Document;
      var start = doc.GetOffset(Editor.TextArea.Selection.StartPosition.Location);
      var end = doc.GetOffset(Editor.TextArea.Selection.EndPosition.Location);
      return doc.GetText(start, end - start);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
    }

    void TextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      var key = (e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key);

      // F5 or F9
      if (key == System.Windows.Input.Key.F9 || key == System.Windows.Input.Key.F5)
      {
        OnRunRequested(new RunRequestedEventArgs(Editor.Text));
      }
      // Ctrl+Enter or Ctrl+Shift+E
      else if ((key == System.Windows.Input.Key.Enter && IsControlDown(e.KeyboardDevice))
        || (key == System.Windows.Input.Key.E && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice)))
      {
        OnRunRequested(new RunRequestedEventArgs(GetCurrentQuery()));
      }
      // Ctrl+Shift+Up
      else if (!SingleLine
        && ((key == System.Windows.Input.Key.Up && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
          || (key == System.Windows.Input.Key.Up && IsAltDown(e.KeyboardDevice))))
      {
        MoveLineUp();
      }
      // Ctrl+Shift+Down
      else if (!SingleLine
        && ((key == System.Windows.Input.Key.Down && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
          || (key == System.Windows.Input.Key.Down && IsAltDown(e.KeyboardDevice))))
      {
        MoveLineDown();
      }
      // Ctrl+T
      else if (key == System.Windows.Input.Key.T && IsControlDown(e.KeyboardDevice)) // Indent the code
      {
        TidyXml();
      }
      // Ctrl+Shift+U
      else if (key == System.Windows.Input.Key.U && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
      {
        TransformUppercase();
      }
      // Ctrl+U
      else if (key == System.Windows.Input.Key.U && IsControlDown(e.KeyboardDevice))
      {
        TransformLowercase();
      }

      OnKeyDown(WinFormsKey(e));
    }

    private KeyEventArgs WinFormsKey(System.Windows.Input.KeyEventArgs e)
    {
      var key = (e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key);
      var keys = (System.Windows.Forms.Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(key);
      if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftAlt)
        || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightAlt)) keys |= Keys.Alt;
      if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl)
        || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl)) keys |= Keys.Control;
      if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftShift)
        || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightShift)) keys |= Keys.Shift;
      return new KeyEventArgs(keys);
    }

    public void ReplaceSelectionSegments(Func<string, string> insert)
    {
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

    private bool IsAltDown(System.Windows.Input.KeyboardDevice keyboard)
    {
      return keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) || keyboard.IsKeyDown(System.Windows.Input.Key.RightAlt);
    }
    private bool IsControlDown(System.Windows.Input.KeyboardDevice keyboard)
    {
      return keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
    }
    private bool IsShiftDown(System.Windows.Input.KeyboardDevice keyboard)
    {
      return keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
    }

    CompletionWindow completionWindow;

    public void ShowCompletionWindow(IEnumerable<ICompletionData> completionItems, int overlap)
    {
      if (completionItems.Any())
      {
        completionWindow = new CompletionWindow(Editor.TextArea);
        completionWindow.StartOffset -= overlap;
        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
        foreach (var item in completionItems)
        {
          data.Add(item);
        }
        completionWindow.Show();
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
      if (this.Helper != null) this.Helper.HandleTextEntered(this, e.Text);
    }


    void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      if (e.Text.Length > 0 && completionWindow != null
        && !completionWindow.CompletionList.CompletionData.OfType<SqlGeneralCompletionData>().Any())
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
      // Do not set e.Handled=true.
      // We still want to insert the character that was typed.
    }

    /// <summary>
    /// Tidy the xml of the editor by indenting
    /// </summary>
    /// <returns>Tidied Xml</returns>
    public void TidyXml()
    {
      string buffer = Editor.TextArea.Document.Text;
      if (buffer.Length < 30000
        || MessageBox.Show("Validating large requests may take several moments.  Continue?", "AML Studio", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        if (Utils.IndentXml(buffer, out buffer) == null)
        {
          Editor.TextArea.Document.Text = buffer;
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        foreach (var itemToDispose in _itemsToDispose)
        {
          itemToDispose.Dispose();
        }
        _itemsToDispose.Clear();

        var elementHost = this.Controls.OfType<ElementHost>().FirstOrDefault();
        if (elementHost != null)
          elementHost.Child = null;

      }
    }

    public void BindToolStripItem(ToolStripItem item, System.Windows.Input.RoutedCommand command)
    {
      _itemsToDispose.Add(new ToolStripBinding(item, command, Editor.TextArea));
    }

    private class ToolStripBinding : IDisposable
    {
      private ToolStripItem _item;
      private System.Windows.Input.RoutedCommand _command;
      private System.Windows.IInputElement _input;

      public ToolStripBinding(ToolStripItem item,
        System.Windows.Input.RoutedCommand command,
        System.Windows.IInputElement input)
      {
        _item = item;
        _command = command;
        _input = input;

        _item.Click += Invoke;
        _command.CanExecuteChanged += EnableChanged;
      }

      private void EnableChanged(object sender, EventArgs e)
      {
        _item.Enabled = _command.CanExecute(null, _input);
      }
      private void Invoke(object sender, EventArgs e)
      {
        _command.Execute(null, _input);
      }

      public void Dispose()
      {
        _item.Click -= Invoke;
        _command.CanExecuteChanged -= EnableChanged;
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
