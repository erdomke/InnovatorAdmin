using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using System.Xml;
using System.IO;
using ICSharpCode.AvalonEdit;

namespace InnovatorAdmin.Editor
{
  public class EditorControl : UserControl
  {
    //private CompletionHelper _helper;
    //private bool _isInitialized;
    private ExtendedEditor _extEditor;
    private FindReplace _findReplace;

    public event EventHandler<RunRequestedEventArgs> RunRequested;

    public IEditorHelper Helper {
      get { return _extEditor.Helper; }
      set { _extEditor.Helper = value; }
    }
    public ICSharpCode.AvalonEdit.TextEditor Editor { get { return _extEditor.Editor; } }
    //public string SoapAction { get; set; }

    public EditorControl()
    {
      var host = new ElementHost();
      host.Size = new Size(200, 100);
      host.Location = new Point(100, 100);
      host.Dock = DockStyle.Fill;

      _extEditor = new ExtendedEditor();
      _extEditor.Host = this;

      var editor = _extEditor.editor;
      editor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
      editor.FontSize = 12.0;
      editor.Options.ConvertTabsToSpaces = true;
      editor.Options.EnableRectangularSelection = true;
      editor.Options.IndentationSize = 2;
      editor.ShowLineNumbers = true;
      editor.TextArea.TextEntering += TextArea_TextEntering;
      editor.TextArea.TextEntered += TextArea_TextEntered;
      editor.TextArea.KeyDown += TextArea_KeyDown;
      host.Child = _extEditor;



      editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

      this.Controls.Add(host);
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
      // F5 or F9
      if (e.Key == System.Windows.Input.Key.F9 || e.Key == System.Windows.Input.Key.F5)
      {
        OnRunRequested(new RunRequestedEventArgs(Editor.Text));
      }
      // Ctrl+Enter or Ctrl+Shift+E
      else if ((e.Key == System.Windows.Input.Key.Enter && IsControlDown(e.KeyboardDevice))
        || (e.Key == System.Windows.Input.Key.E && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice)))
      {
        OnRunRequested(new RunRequestedEventArgs(GetCurrentQuery()));
      }
      // Ctrl+Shift+Up
      else if ((e.Key == System.Windows.Input.Key.Up && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
        || (e.Key == System.Windows.Input.Key.Up && IsAltDown(e.KeyboardDevice)))
      {
        MoveLineUp();
      }
      // Ctrl+Shift+Down
      else if ((e.Key == System.Windows.Input.Key.Down && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
        || (e.Key == System.Windows.Input.Key.Down && IsAltDown(e.KeyboardDevice)))
      {
        MoveLineDown();
      }
      // Ctrl+T
      else if (e.Key == System.Windows.Input.Key.T && IsControlDown(e.KeyboardDevice)) // Indent the code
      {
        TidyXml();
      }
      // Ctrl+Shift+U
      else if (e.Key == System.Windows.Input.Key.U && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice))
      {
        TransformUppercase();
      }
      // Ctrl+U
      else if (e.Key == System.Windows.Input.Key.U && IsControlDown(e.KeyboardDevice))
      {
        TransformLowercase();
      }
      // Ctrl+F
      else if (e.Key == System.Windows.Input.Key.F && IsControlDown(e.KeyboardDevice))
      {
        if (_findReplace == null)
          _findReplace = new FindReplace(this);
        if (_findReplace.IsDisposed)
          _findReplace = new FindReplace(this);
        _findReplace.Show(this);

      }

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
      if (start.Line > 1)
      {
        var doc = Editor.Document;
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

    //private void TransformSelection(Func<string, string> transform)
    //{
    //  var doc = Editor.Document;
    //  using (doc.RunUpdate())
    //  {
    //    string text;
    //    foreach (var segment in Editor.TextArea.Selection.Segments)
    //    {
    //      text = doc.GetText(segment);
    //      doc.Replace(segment, transform.Invoke(text));
    //    }
    //  }
    //}

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

    public bool ReadOnly
    {
      get { return Editor.IsReadOnly; }
      set { Editor.IsReadOnly = value; }
    }
    public override string Text
    {
      get { return Editor.Text; }
      set { Editor.Text = value; }
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
        if (_findReplace != null)
          _findReplace.Dispose();
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
}
