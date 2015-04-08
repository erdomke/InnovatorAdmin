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

namespace Aras.Tools.InnovatorAdmin
{
  public class EditorControl : UserControl
  {
    private ICSharpCode.AvalonEdit.TextEditor _editor;
    private FoldingManager _foldingManager;
    private XmlFoldingStrategy _foldingStrategy = new XmlFoldingStrategy();
    private CompletionHelper _helper;
    private bool _isInitialized;

    public event EventHandler<RunRequestedEventArgs> RunRequested;

    public string SoapAction { get; set; }

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

      _helper = new CompletionHelper();

      this.SoapAction = "ApplyAML";

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
        OnRunRequested(new RunRequestedEventArgs(_helper.GetQuery(_editor.Text, _editor.CaretOffset)));
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

    public override string Text
    {
      get { return _editor.Text; }
      set { _editor.Text = value; }
    }

    private void UpdateFoldings()
    {
      _foldingStrategy.UpdateFoldings(_foldingManager, _editor.Document);
    }

    public void InitializeConnection(Func<string, XmlNode, XmlNode> applyAction)
    {
      _isInitialized = true;
      _helper.InitializeConnection(applyAction);
    }

    CompletionWindow completionWindow;

    void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      if (_isInitialized)
      {
        switch (e.Text)
        {
          case "\"":
          case " ":
          case "<":
          case ",":
          case "(":
            int overlap;
            var completionItems = _helper.GetCompletions(_editor.Text.Substring(0, _editor.CaretOffset), this.SoapAction, out overlap).Select(c => new MyCompletionData(c));
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
            break;
          case ">":
            var endTag = _helper.LastOpenTag(_editor.Text.Substring(0, _editor.CaretOffset));
            if (!string.IsNullOrEmpty(endTag))
            {
              var insert = "</" + endTag + ">";
              if (!_editor.Text.Substring(_editor.CaretOffset).StartsWith(insert))
              {
                _editor.Document.Insert(_editor.CaretOffset, "</" + endTag + ">", AnchorMovementType.BeforeInsertion);
              }
            }
            break;
        }
      }
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


    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    private class MyCompletionData : ICompletionData
    {
      public MyCompletionData(string text)
      {
        this.Text = text;
      }

      public System.Windows.Media.ImageSource Image
      {
        get { return null; }
      }

      public string Text { get; private set; }

      // Use this property if you want to show a fancy UIElement in the list.
      public object Content
      {
        get { return this.Text; }
      }

      public object Description
      {
        get { return this.Text; }
      }

      public void Complete(TextArea textArea, ISegment completionSegment,
          EventArgs insertionRequestEventArgs)
      {
        textArea.Document.Replace(completionSegment, this.Text);
      }

      public double Priority
      {
        get { return 0; }
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
