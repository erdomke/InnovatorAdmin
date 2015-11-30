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

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public enum EditorMode
  {
    Xml,
    Text
  }

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
      editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".xml");
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

    void TextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.F9 || e.Key == System.Windows.Input.Key.F5)
      {
        OnRunRequested(new RunRequestedEventArgs(Editor.Text));
      }
      else if ((e.Key == System.Windows.Input.Key.Enter && IsControlDown(e.KeyboardDevice))
        || (e.Key == System.Windows.Input.Key.E && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice)))
      {
        OnRunRequested(new RunRequestedEventArgs(Helper.GetCurrentQuery(Editor.Text, Editor.CaretOffset)));
      }
      else if (e.Key == System.Windows.Input.Key.T && IsControlDown(e.KeyboardDevice)) // Indent the code
      {
        TidyXml();
      }
      else if (e.Key == System.Windows.Input.Key.F && IsControlDown(e.KeyboardDevice))
      {
        if (_findReplace == null)
          _findReplace = new FindReplace(this);
        if (_findReplace.IsDisposed)
          _findReplace = new FindReplace(this);
        _findReplace.Show();

      }

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
      if (e.Text.Length > 0 && completionWindow != null)
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
        if (IndentXml(buffer, out buffer) == null)
        {
          Editor.TextArea.Document.Text = buffer;
        }
      }
    }

    /// <summary>
    /// Tidy the xml of the editor by indenting
    /// </summary>
    /// <param name="xml">Unformatted XML string</param>
    /// <returns>Formatted Xml String</returns>
    public Exception IndentXml(string xml, out string formattedString)
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
        MessageBox.Show(ex.Message);
        formattedString = string.Empty;
        return ex;
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
