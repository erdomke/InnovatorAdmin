using Aras.AutoComplete;
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
    private ICSharpCode.AvalonEdit.TextEditor _editor;
    private FoldingManager _foldingManager;
    private XmlFoldingStrategy _foldingStrategy = new XmlFoldingStrategy();
    //private CompletionHelper _helper;
    //private bool _isInitialized;
    private EditorMode _mode = EditorMode.Xml;

    public event EventHandler<RunRequestedEventArgs> RunRequested;

    public IEditorHelper Helper { get; set; }
    public ICSharpCode.AvalonEdit.TextEditor Editor { get { return _editor; } }
    //public string SoapAction { get; set; }

    public EditorControl()
    {
      var host = new ElementHost();
      host.Size = new Size(200, 100);
      host.Location = new Point(100, 100);
      host.Dock = DockStyle.Fill;

      _editor = new ICSharpCode.AvalonEdit.TextEditor();
      _editor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
      _editor.FontSize = 12.0;
      _editor.Options.ConvertTabsToSpaces = true;
      _editor.Options.EnableRectangularSelection = true;
      _editor.Options.IndentationSize = 2;
      _editor.ShowLineNumbers = true;
      _editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".xml");
      _editor.TextArea.TextEntering += TextArea_TextEntering;
      _editor.TextArea.TextEntered += TextArea_TextEntered;
      _editor.TextArea.KeyDown += TextArea_KeyDown;
      host.Child = _editor;

      _editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
      _foldingManager = FoldingManager.Install(_editor.TextArea);
      UpdateFoldings();

      var foldingUpdateTimer = new DispatcherTimer();
      foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
      foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
      foldingUpdateTimer.Start();
      
      this.Controls.Add(host);
    }

    protected virtual void OnRunRequested(RunRequestedEventArgs e)
    {
      if (RunRequested != null) RunRequested.Invoke(this, e);
    }

    void TextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.F9 || e.Key == System.Windows.Input.Key.F5)
      {
        OnRunRequested(new RunRequestedEventArgs(_editor.Text));
      }
      else if ((e.Key == System.Windows.Input.Key.Enter && IsControlDown(e.KeyboardDevice))
        || (e.Key == System.Windows.Input.Key.E && IsControlDown(e.KeyboardDevice) && IsShiftDown(e.KeyboardDevice)))
      {
        OnRunRequested(new RunRequestedEventArgs(Helper.GetCurrentQuery(_editor.Text, _editor.CaretOffset)));
      }
      else if (e.Key == System.Windows.Input.Key.T && IsControlDown(e.KeyboardDevice)) // Indent the code
      {
        TidyXml();
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
      get { return _editor.IsReadOnly; }
      set { _editor.IsReadOnly = value; }
    }
    public override string Text
    {
      get { return _editor.Text; }
      set { _editor.Text = value; }
    }

    private void UpdateFoldings()
    {
      switch (_mode)
      {
        case EditorMode.Xml:
          _foldingStrategy.UpdateFoldings(_foldingManager, _editor.Document);
          break;
      }
    }

    CompletionWindow completionWindow;

    public void ShowCompletionWindow(IEnumerable<ICompletionData> completionItems, int overlap)
    {
      if (completionItems.Any())
      {
        completionWindow = new CompletionWindow(_editor.TextArea);
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
          case ' ':
          case '>':
            // Whenever a non-letter is typed while the completion window is open,
            // insert the currently selected element.
            completionWindow.CompletionList.RequestInsertion(e);
            break;
        }
      }
      // Do not set e.Handled=true.
      // We still want to insert the character that was typed.
    }

    /// <summary>
    /// Tidy the xml of the editor by adding AML tags and indenting
    /// </summary>
    /// <returns>Tidied Xml</returns>
    void TidyXml()
    {
            //_editor.TextArea.Document.Text = "Hallo";
            string buffer = _editor.TextArea.Document.Text;
            if (buffer.Length < 30000 || MessageBox.Show("Validating large requests may take several moments.  Continue?","AML Requestor", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (IndentXml(buffer, out buffer) == null)
                {
                    _editor.TextArea.Document.Text = buffer;
                }
            }
    }

    /// <summary>
    /// Tidy the xml of the editor by adding AML tags and indenting
    /// </summary>
    /// <param name="xml">Unformatted XML string</param>
    /// <returns>Formatted Xml String</returns>
    public Exception IndentXml(string xml, out string formattedString)
    {
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(xml);
                return IndentXml(doc, out formattedString);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                formattedString = string.Empty;
                return ex;
            }
    }

    /// <summary>
    /// Takes an Xml Document and renders it as a formatted (indented) string
    /// </summary>
    /// <param name="doc">XML Document</param>
    /// <returns>Formatted (indented) XML string</returns>
    public Exception IndentXml(XmlDocument doc, out string formattedString)
    {
        try
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.IndentChars = "  ";
                settings.CheckCharacters = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    doc.WriteContentTo(xmlWriter);
                    xmlWriter.Flush();
                    formattedString = writer.ToString();
                }

            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            formattedString = string.Empty;
            return ex;
        }
        return null;
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
