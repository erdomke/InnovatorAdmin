using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Xml;

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
    private ContextMenuStrip conMenu;
    private IContainer components;
    private ToolStripMenuItem mniCut;
    private ToolStripMenuItem mniCopy;
    private ToolStripMenuItem mniPaste;
    private ToolStripMenuItem mniCollapseAll;
    private ToolStripMenuItem mniExpandAll;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem mniOpenWith;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem mniCompareTo;
    private Placeholder _placeholder;

    public event EventHandler<FileOpeningEventArgs> FileOpening;
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
      InitializeComponent();

      var host = new ElementHost();
      host.Size = new Size(200, 100);
      host.Location = new Point(100, 100);
      host.Dock = DockStyle.Fill;

      _extEditor = new ExtendedEditor();
      _extEditor.Host = this;
      _extEditor.FileOpening += (s, e) =>
      {
        if (FileOpening != null)
          FileOpening.Invoke(this, e);
      };

      var editor = _extEditor.editor;
      editor.FontFamily = _fixedWidth;
      editor.FontSize = 12.0;
      editor.Options.ConvertTabsToSpaces = true;
      editor.Options.EnableRectangularSelection = true;
      editor.Options.IndentationSize = 2;
      editor.ShowLineNumbers = true;
      editor.TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
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

      BindToolStripItem(mniCut, System.Windows.Input.ApplicationCommands.Cut);
      BindToolStripItem(mniCopy, System.Windows.Input.ApplicationCommands.Copy);
      BindToolStripItem(mniPaste, System.Windows.Input.ApplicationCommands.Paste);
    }

    void TextArea_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      try
      {
        var pos = e.GetPosition(Editor.TextArea);
        var pt = new Point((int)pos.X, (int)pos.Y);
        conMenu.Show(this.PointToScreen(pt));
        while (conMenu.Items.Count > 9)
          conMenu.Items.RemoveAt(conMenu.Items.Count - 1);

        if (Helper != null)
          EditorScript.BuildMenu(conMenu.Items
            , Helper.GetScripts(Document, Editor.CaretOffset)
            , (script) =>
            {
              var ide = this.FindForm() as EditorWindow;
              if (ide != null)
                ide.Execute(script);
            });

        OnMouseDown(new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, e.ClickCount, pt.X, pt.Y, 0));
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


    void editor_TextChanged(object sender, EventArgs e)
    {
      OnTextChanged(e);
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
          var frm = this.FindForm();
          frm.SelectNextControl(this, e.KeyboardDevice.Modifiers != System.Windows.Input.ModifierKeys.Shift, true, true, true);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
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
      try
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
      catch (Exception ex)
      {
        Utils.HandleError(ex);
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

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
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

        var winArgs = WinFormsKey(e);
        OnKeyDown(winArgs);
        e.Handled = e.Handled || winArgs.Handled;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
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

    //private bool IsAltDown(System.Windows.Input.KeyboardDevice keyboard)
    //{
    //  return keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) || keyboard.IsKeyDown(System.Windows.Input.Key.RightAlt);
    //}
    //private bool IsControlDown(System.Windows.Input.KeyboardDevice keyboard)
    //{
    //  return keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
    //}
    //private bool IsShiftDown(System.Windows.Input.KeyboardDevice keyboard)
    //{
    //  return keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
    //}

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
          ( e.Key == System.Windows.Input.Key.Enter
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
        if (null == this.CompletionList.ScrollViewer) return;
        var addition = this.CompletionList.ScrollViewer.ExtentWidth - this.CompletionList.ScrollViewer.ViewportWidth;
        var client = this.TextArea.PointFromScreen(new System.Windows.Point(this.Left, this.Top));
        addition = Math.Max(0, Math.Min(addition, this.TextArea.ActualWidth - this.Width - client.X));
        this.Width += addition;
      }
    }

    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.conMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mniCut = new System.Windows.Forms.ToolStripMenuItem();
      this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
      this.mniPaste = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.mniCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
      this.mniExpandAll = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mniOpenWith = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.mniCompareTo = new System.Windows.Forms.ToolStripMenuItem();
      this.conMenu.SuspendLayout();
      this.SuspendLayout();
      //
      // conMenu
      //
      this.conMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCut,
            this.mniCopy,
            this.mniPaste,
            this.toolStripSeparator2,
            this.mniCollapseAll,
            this.mniExpandAll,
            this.toolStripSeparator1,
            this.mniCompareTo,
            this.mniOpenWith,
            this.toolStripSeparator3});
      this.conMenu.Name = "contextMenuStrip1";
      this.conMenu.Size = new System.Drawing.Size(153, 198);
      //
      // mniCut
      //
      this.mniCut.Name = "mniCut";
      this.mniCut.ShortcutKeyDisplayString = "Ctrl+X";
      this.mniCut.Size = new System.Drawing.Size(152, 22);
      this.mniCut.Text = "Cut";
      //
      // mniCopy
      //
      this.mniCopy.Name = "mniCopy";
      this.mniCopy.ShortcutKeyDisplayString = "Ctrl+C";
      this.mniCopy.Size = new System.Drawing.Size(152, 22);
      this.mniCopy.Text = "Copy";
      //
      // mniPaste
      //
      this.mniPaste.Name = "mniPaste";
      this.mniPaste.ShortcutKeyDisplayString = "Ctrl+V";
      this.mniPaste.Size = new System.Drawing.Size(152, 22);
      this.mniPaste.Text = "Paste";
      //
      // toolStripSeparator2
      //
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
      //
      // mniCollapseAll
      //
      this.mniCollapseAll.Name = "mniCollapseAll";
      this.mniCollapseAll.Size = new System.Drawing.Size(152, 22);
      this.mniCollapseAll.Text = "Collapse All";
      this.mniCollapseAll.Click += new System.EventHandler(this.mniCollapseAll_Click);
      //
      // mniExpandAll
      //
      this.mniExpandAll.Name = "mniExpandAll";
      this.mniExpandAll.Size = new System.Drawing.Size(152, 22);
      this.mniExpandAll.Text = "Expand All";
      this.mniExpandAll.Click += new System.EventHandler(this.mniExpandAll_Click);
      //
      // toolStripSeparator1
      //
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
      //
      // mniOpenWith
      //
      this.mniOpenWith.Name = "mniOpenWith";
      this.mniOpenWith.Size = new System.Drawing.Size(152, 22);
      this.mniOpenWith.Text = "Open With...";
      this.mniOpenWith.Click += new System.EventHandler(this.mniOpenWith_Click);
      //
      // toolStripSeparator3
      //
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
      //
      // mniCompareTo
      //
      this.mniCompareTo.Name = "mniCompareTo";
      this.mniCompareTo.Size = new System.Drawing.Size(152, 22);
      this.mniCompareTo.Text = "Compare To...";
      this.mniCompareTo.DropDownOpening += new System.EventHandler(this.mniCompareTo_DropDownOpening);
      //
      // EditorWinForm
      //
      this.Name = "EditorWinForm";
      this.conMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    private void mniCollapseAll_Click(object sender, EventArgs e)
    {
      try { _extEditor.CollapseAll(); }
      catch (Exception ex) { Utils.HandleError(ex); }
    }

    private void mniExpandAll_Click(object sender, EventArgs e)
    {
      try { _extEditor.ExpandAll(); }
      catch (Exception ex) { Utils.HandleError(ex); }
    }

    private void mniOpenWith_Click(object sender, EventArgs e)
    {
      try
      {
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xml");
        File.WriteAllText(file, Editor.Text);
        ShellHelper.OpenAs(this.FindForm().Handle, file);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void mniCompareTo_DropDownOpening(object sender, EventArgs e)
    {
      try
      {
        mniCompareTo.DropDownItems.Clear();
        if (!this.SingleLine)
        {
          var newItems = new List<ToolStripMenuItem>();
          var currEditor = this.ParentsAndSelf().OfType<FullEditor>().FirstOrDefault();
          var currForm = this.FindForm();
          var isInput = currEditor != null && currEditor.Name.StartsWith("input", StringComparison.OrdinalIgnoreCase);
          var currName = isInput ? "Input" : "Output";

          foreach (var editor in Application.OpenForms.OfType<EditorWindow>()
            .SelectMany(f => new FullEditor[] { f.InputEditor, f.OutputEditor })
            .Where(c => c != currEditor))
          {
            var compareForm = editor.FindForm();
            var isCompareInput = editor.Name.StartsWith("input", StringComparison.OrdinalIgnoreCase);
            var isSameForm = compareForm == currForm;
            var name = isCompareInput ? "Input" : "Output";
            if (!isSameForm)
              name += ": " + compareForm.Text.Replace(" [Innovator Admin]", "");
            newItems.Add(new ToolStripMenuItem(name, null, (s, ea) =>
            {
              if (!isInput && isCompareInput)
                Settings.Current.PerformDiff(name, editor.Document, currName, this.Document);
              else
                Settings.Current.PerformDiff(currName, this.Document, name, editor.Document);
            })
            {
              Tag = isSameForm ? 1 : 2
            });
          }

          newItems.OrderBy(i => (int)i.Tag).ThenBy(i => i.Name);
          mniCompareTo.DropDownItems.AddRange(newItems.ToArray());
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
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
