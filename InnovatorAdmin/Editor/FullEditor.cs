using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin.Editor
{
  public partial class FullEditor : UserControl
  {

    private enum FindReplaceState
    {
      None,
      Find,
      Replace
    }

    public event EventHandler<RunRequestedEventArgs> RunRequested;
    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    private TextDocument _currentDoc;
    private SearchResultBackgroundRenderer _renderer;
    private ISearchStrategy _strategy;
    private SearchMode _mode = SearchMode.Normal;
    private bool _programChangingCheckState;

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<VisualLineElementGenerator> ElementGenerators
    {
      get { return editor.ElementGenerators; }
    }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEditorHelper Helper
    {
      get { return editor.Helper; }
      set { editor.Helper = value; }
    }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextEditor Editor { get { return editor.Editor; } }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextDocument Document
    {
      get { return editor.Document; }
      set { editor.Document = value; }
    }
    public bool ReadOnly
    {
      get { return editor.ReadOnly; }
      set { editor.ReadOnly = value; }
    }
    public override string Text
    {
      get { return editor.Text; }
      set { editor.Text = value; }
    }

    private FindReplaceState FindReplaceMode
    {
      get
      {
        if (txtReplace.Visible)
          return FindReplaceState.Replace;
        if (findReplacePanel.Visible)
          return FindReplaceState.Find;
        return FindReplaceState.None;
      }
      set
      {
        if (_currentDoc != null)
        {
          _currentDoc.TextChanged -= new EventHandler(this.textArea_Document_TextChanged);
        }

        findReplacePanel.Visible = (value != FindReplaceState.None);
        txtReplace.Visible = (value == FindReplaceState.Replace);
        btnReplaceAll.Visible = txtReplace.Visible;
        btnReplaceNext.Visible = txtReplace.Visible;

        if (value == FindReplaceState.None)
        {
          _currentDoc = null;
          editor.Focus();
        }
        else
        {
          _currentDoc = editor.Document;
          if (_currentDoc != null)
          {
            _currentDoc.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
            this.DoSearch(false);
          }

          SearchMode newMode;
          if (Enum.TryParse<SearchMode>(Properties.Settings.Default.FindReplace_LastMode, out newMode))
          {
            SetMode(newMode);
          }

          txtFind.Focus();
          var selection = editor.Editor.SelectedText;
          if (!string.IsNullOrEmpty(selection))
          {
            txtFind.Text = selection;
            txtFind.Editor.SelectionStart = 0;
            txtFind.Editor.SelectionLength = selection.Length;
          }
        }
      }
    }

    public FullEditor()
    {
      InitializeComponent();

      _renderer = new SearchResultBackgroundRenderer();
      editor.Editor.TextArea.TextView.BackgroundRenderers.Add(_renderer);

      editor.KeyDown += editor_KeyDown;
      editor.RunRequested += editor_RunRequested;
      editor.SelectionChanged += editor_SelectionChanged;
      editor.TextChanged += editor_TextChanged;

      txtFind.SingleLine = true;
      txtFind.TextChanged += txtFind_TextChanged;
      txtFind.RunRequested += txtFind_RunRequested;
      txtFind.KeyDown += textbox_KeyDown;
      txtReplace.SingleLine = true;
      txtReplace.RunRequested += txtReplace_RunRequested;
      txtReplace.KeyDown += textbox_KeyDown;

      FindReplaceMode = FindReplaceState.None;
    }

    public void Find()
    {
      FindReplaceMode = FindReplaceState.Find;
    }
    public void Replace()
    {
      FindReplaceMode = FindReplaceState.Replace;
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
      _currentDoc = editor.Document;
      if (_currentDoc != null)
      {
        _currentDoc.TextChanged += new EventHandler(this.textArea_Document_TextChanged);
        this.DoSearch(false);
      }
    }

    public void BindToolStripItem(ToolStripItem item, System.Windows.Input.RoutedCommand command)
    {
      editor.BindToolStripItem(item, command);
    }
    public void CleanUndoStack()
    {
      Document.UndoStack.ClearAll();
    }
    public void CollapseAll() { editor.CollapseAll(); }
    public void ReplaceSelectionSegments(Func<string, string> insert) { editor.ReplaceSelectionSegments(insert); }
    public void MoveLineDown() { editor.MoveLineDown(); }
    public void MoveLineUp() { editor.MoveLineUp(); }
    public void TidyXml() { editor.TidyXml(); }
    public void TransformLowercase() { editor.TransformLowercase(); }
    public void TransformUppercase() { editor.TransformUppercase(); }

    void textbox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape)
      {
        FindReplaceMode = FindReplaceState.None;
      }
      HandleGlobalKeys(e);
    }

    private bool HandleGlobalKeys(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.F && e.Control)
      {
        FindReplaceMode = FindReplaceState.Find;
        e.Handled = true;
        return true;
      }
      else if (e.KeyCode == Keys.H && e.Control)
      {
        FindReplaceMode = FindReplaceState.Replace;
        e.Handled = true;
        return true;
      }
      else if (e.KeyCode == Keys.F3 && e.Shift)
      {
        if (FindReplaceMode != FindReplaceState.None)
          FindPrevious();
      }
      else if (e.KeyCode == Keys.F3)
      {
        if (FindReplaceMode != FindReplaceState.None)
          FindNext();
      }
      return false;
    }

    void txtReplace_RunRequested(object sender, RunRequestedEventArgs e)
    {
      Replace();
    }

    void txtFind_TextChanged(object sender, EventArgs e)
    {
      UpdateSearch();
    }

    void txtFind_RunRequested(object sender, RunRequestedEventArgs e)
    {
      FindNext();
    }

    void editor_KeyDown(object sender, KeyEventArgs e)
    {
      if (!HandleGlobalKeys(e))
        OnKeyDown(e);
    }

    void editor_RunRequested(object sender, RunRequestedEventArgs e)
    {
      if (RunRequested != null)
        RunRequested.Invoke(this, e);
    }

    void editor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged.Invoke(this, e);
    }

    void editor_TextChanged(object sender, EventArgs e)
    {
      OnTextChanged(e);
    }

    private TextSegment GetSelectionSegment()
    {
      if (editor.Editor.SelectionLength < 1) return null;
      var segment = editor.Editor.TextArea.Selection.Segments.First();
      return _renderer.CurrentResults.FirstOrDefault(s => s.StartOffset == segment.StartOffset && s.EndOffset == segment.EndOffset);
    }

    public void FindNext()
    {
      var currResult = GetSelectionSegment();
      var searchResult = currResult != null
        ? _renderer.CurrentResults.GetNextSegment(currResult)
        : _renderer.CurrentResults.FindFirstSegmentWithStartAfter(editor.Editor.TextArea.Caret.Offset + 1);
      if (searchResult == null)
      {
        searchResult = _renderer.CurrentResults.FirstSegment;
      }
      if (searchResult != null)
      {
        this.SelectResult(searchResult);
      }
    }

    public void FindPrevious()
    {
      var currResult = GetSelectionSegment();
      var searchResult = currResult != null
        ? _renderer.CurrentResults.GetPreviousSegment(currResult)
        : _renderer.CurrentResults.FindFirstSegmentWithStartAfter(editor.Editor.TextArea.Caret.Offset + 1);
      if (searchResult == null)
      {
        searchResult = _renderer.CurrentResults.LastSegment;
      }
      if (searchResult != null)
      {
        this.SelectResult(searchResult);
      }
    }

    private void ReplaceNext()
    {
      var searchResult = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(editor.Editor.TextArea.Caret.Offset) as ISearchResult;
      if (searchResult == null)
      {
        searchResult = _renderer.CurrentResults.FirstSegment as ISearchResult;
      }
      if (searchResult != null)
      {
        var newText = ReplacementText(searchResult);
        _currentDoc.Replace(searchResult, newText);
        this.SelectResult(new TextSegment() { StartOffset = searchResult.Offset, Length = newText.Length });
        _renderer.CurrentResults.Remove((TextSegment)searchResult);
      }
    }

    private string ReplacementText(ISearchResult result)
    {
      var replacement = txtReplace.Text;
      if (_mode == SearchMode.Extended || _mode == SearchMode.RegEx)
        replacement = StringHelper.StringFromCSharpLiteral(replacement);
      return result.ReplaceWith(replacement);
    }

    private void ReplaceAll()
    {
      using (_currentDoc.RunUpdate())
      {
        var offset = 0;
        string newText;
        foreach (var result in _renderer.CurrentResults.OfType<ISearchResult>().ToList())
        {
          newText = ReplacementText(result);
          _currentDoc.Replace(result.Offset + offset, result.Length, newText);
          offset += newText.Length - result.Length;
        }
      }
      _renderer.CurrentResults.Clear();
      editor.Editor.SelectionLength = 0;
    }

    private void UpdateSearch()
    {
      try
      {
        this._strategy = SearchFactory.Create(txtFind.Text ?? ""
          , !chkCaseSensitive.Checked, false, _mode);
        this.DoSearch(true);
      }
      catch (SearchPatternException)
      {
        // Eat any regex parse errors
      }
    }

    private void DoSearch(bool changeSelection)
    {
      if (this.IsDisposed)
        return;

      _renderer.CurrentResults.Clear();
      if (!string.IsNullOrEmpty(txtFind.Text))
      {
        var offset = editor.Editor.TextArea.Caret.Offset;
        if (changeSelection)
          editor.Editor.TextArea.ClearSelection();

        foreach (var searchResult in _strategy.FindAll(_currentDoc, 0
          , _currentDoc.TextLength).OfType<TextSegment>())
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
      editor.Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
    }

    private void SelectResult(ISegment result)
    {
      editor.Editor.TextArea.Caret.Offset = result.Offset;
      editor.Editor.TextArea.Selection = Selection.Create(editor.Editor.TextArea, result.Offset, result.EndOffset);
      editor.Editor.TextArea.Caret.BringCaretToView();
      editor.Editor.TextArea.Caret.Show();
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
          txtFind.Helper = null;
          switch (mode)
          {
            case SearchMode.Extended:
              chkExtended.Checked = true;
              txtFind.Helper = new StringLiteralHelper();
              txtReplace.Helper = new StringLiteralHelper();
              break;
            case SearchMode.Normal:
              chkNormal.Checked = true;
              break;
            case SearchMode.RegEx:
              chkRegExp.Checked = true;
              txtFind.Helper = new RegexHelper();
              txtReplace.Helper = new RegexReplacementHelper();
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

    private void btnFind_Click(object sender, EventArgs e)
    {
      try
      {
        FindNext();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnReplaceNext_Click(object sender, EventArgs e)
    {
      try
      {
        ReplaceNext();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnReplaceAll_Click(object sender, EventArgs e)
    {
      try
      {
        ReplaceAll();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      try
      {
        FindReplaceMode = FindReplaceState.None;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

  }
}
