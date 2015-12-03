using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
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
  public partial class ExtendedEditor : UserControl
  {
    private IEditorHelper _helper;
    private FoldingManager _foldingManager;
    private IFoldingStrategy _foldingStrategy;
    
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
    public System.Windows.Forms.Control Host { get; set; }

    public ICSharpCode.AvalonEdit.TextEditor Editor
    {
      get { return this.editor; }
    }

    public ExtendedEditor()
    {
      InitializeComponent();

      _foldingManager = FoldingManager.Install(this.editor.TextArea);
      UpdateFoldings();

      var foldingUpdateTimer = new DispatcherTimer();
      foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
      foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
      foldingUpdateTimer.Start();

      this.editor.TextArea.MouseRightButtonDown += TextArea_MouseRightButtonDown;
    }

    void TextArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var position = this.editor.GetPositionFromPoint(e.GetPosition(this.editor));
      if (position.HasValue)
      {
        this.editor.TextArea.Caret.Position = position.Value;
      }
    }

    public void CollapseAll()
    {
      UpdateFoldings();
      if (_foldingManager.AllFoldings.Any(IsAmlFolding))
      {
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

    private void CollapseAll_Click(object sender, RoutedEventArgs e)
    {
      CollapseAll();
    }
    private bool IsAmlFolding(FoldingSection f)
    {
      return f.Title.StartsWith("<Item") || f.Title.StartsWith("<Properties ") || f.Title.StartsWith("<Relationships");
    }
    private void ExpandAll_Click(object sender, RoutedEventArgs e)
    {
      ExpandAll();
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
      if (this.Helper == null)
      {
        amlQuery = doc.Text;
      }
      else
      {
        amlQuery = this.Helper.GetCurrentQuery(doc.Text, caret.Offset);
        var loc = doc.GetLocation(doc.Text.IndexOf(amlQuery));
        settings.LineNumberOffset = loc.Line;
      }

      string lastItemId = null;
      string lastId = null;
      var elems = new Stack<string>();


      using (var reader = new System.IO.StringReader(amlQuery))
      {
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
      }

      return null;
    }

    private void OpenWith_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var file = io.Path.Combine(io.Path.GetTempPath(), io.Path.GetRandomFileName() + ".xml");
        io.File.WriteAllText(file, this.editor.Text);
        ShellHelper.OpenAs(this.Host.FindForm().Handle, file);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
