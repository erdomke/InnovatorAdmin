using Innovator.Client;
using InnovatorAdmin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Navigation;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// Interaction logic for DocumentViewer.xaml
  /// </summary>
  public partial class DocumentViewer : System.Windows.Controls.UserControl
  {
    private Documentation.Document _document;
    private ArasMetadataProvider _metadata;
    private int _historyIndex = -1;
    private List<DocumentUrl> _history = new List<DocumentUrl>();
    private static List<string> _categories = new List<string>() { "Help", "ItemType", "Method" };
    private static Dictionary<string, Func<FlowDocument>> _topics = new Dictionary<string, Func<FlowDocument>>(StringComparer.OrdinalIgnoreCase);

    public DocumentViewer()
    {
      InitializeComponent();
      this.flowViewer.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(flowViewer_RequestNavigate));
      this.FontFamily = new FontFamily("Segoe UI");
      this.FontSize = 14;
    }

    public void AddTopic(string title, Func<FlowDocument> generator)
    {
      _topics[title] = generator;
    }

    public void SetProxy(ArasEditorProxy proxy)
    {
      if (proxy == null)
      {
        _metadata = null;
      }
      else
      {
        _metadata = ArasMetadataProvider.Cached(proxy.Connection);
        //_metadata.ReloadPromise().Done(_ => TryNavigate("Method", "ans_FileSys_UploadFiles"));
      }
    }

    public bool TryNavigate(string category, string name)
    {
      while (_history.Count > Math.Max(_historyIndex + 1, 0))
        _history.RemoveAt(_history.Count - 1);
      var url = new DocumentUrl(category, name);
      if (_historyIndex >= 0 && _history[_historyIndex].Equals(url))
        return false;

      _history.Add(url);
      SetHistoryIndex(_historyIndex + 1);
      
      Navigate(url);
      return true;
    }

    public bool TryGoForward()
    {
      if (_historyIndex >= _history.Count - 1)
        return false;

      SetHistoryIndex(_historyIndex + 1);
      Navigate(_history[_historyIndex]);
      return true;
    }

    public bool TryGoBackward()
    {
      if (_historyIndex <= 0)
        return false;

      SetHistoryIndex(_historyIndex - 1);
      Navigate(_history[_historyIndex]);
      return true;
    }

    private void SetHistoryIndex(int index)
    {
      _historyIndex = index;
      BackButton.IsEnabled = _historyIndex > 0;
      ForwardButton.IsEnabled = _historyIndex < _history.Count - 1;
    }

    public FlowDocument CreateDocument()
    {
      return new FlowDocument()
      {
        FontFamily = this.FontFamily,
        FontSize = this.FontSize,
        TextAlignment = TextAlignment.Left
      };
    }

    private async Task<bool> Navigate(DocumentUrl documentUrl)
    {
      SetMessage("Loading...");
      try
      {
        UpdateAddressBar(documentUrl);

        var options = new Documentation.DocumentOptions();
        switch (documentUrl.Category.ToUpperInvariant())
        {
          case "HELP":
            if (_topics.TryGetValue(documentUrl.Name, out var generator))
            {
              _document = null;
              Display(generator());
              return true;
            }
            break;
          case "ITEMTYPE":
            await _metadata.ReloadTask();
            if (_metadata.ItemTypeByName(documentUrl.Name, out var itemType))
            {
              await Task.WhenAll(_metadata.GetClassPaths(itemType).ToTask(), _metadata.GetProperties(itemType).ToTask());
              Display(Documentation.Document.FromItemType(itemType, options));
              return true;
            }
            break;
          case "METHOD":
            await _metadata.ReloadTask();
            var method = _metadata.Methods.FirstOrDefault(m => string.Equals(m.KeyedName, documentUrl.Name, StringComparison.OrdinalIgnoreCase));
            if (method != null)
            {
              Display(Documentation.Document.FromMethod(method, options));
              return true;
            }
            break;
        }
        SetMessage("Can't load document");
      }
      catch (Exception ex)
      {
        SetMessage("Can't load document\r\n" + ex.ToString());
      }

      return false;
    }

    private void UpdateAddressBar(DocumentUrl documentUrl)
    {
      if (!Dispatcher.CheckAccess())
      {
        Dispatcher.Invoke(() => UpdateAddressBar(documentUrl));
        return;
      }

      CategoryButton.Content = documentUrl.Category;
      NameButton.Content = documentUrl.Name;
    }

    private void SetMessage(string message)
    {
      if (!Dispatcher.CheckAccess())
      {
        Dispatcher.Invoke(() => SetMessage(message));
        return;
      }

      _document = null;
      var flowDocument = CreateDocument();
      flowDocument.Blocks.Add(new Paragraph(new Run(message)));
      this.flowViewer.Document = flowDocument;
    }

    private void Display(FlowDocument document)
    {
      if (!Dispatcher.CheckAccess())
      {
        Dispatcher.Invoke(() => Display(document));
        return;
      }

      this.flowViewer.Document = document;
    }

    private void Display(Documentation.Document document)
    {
      _document = document;
      var flowDocument = CreateDocument();
      var visitor = new DocumentationViewVisitor()
      {
        HeadingLevel = 1,
        BaseFontSize = flowDocument.FontSize
      };
      if (!string.IsNullOrEmpty(document.Title))
        flowDocument.Blocks.Add(new Paragraph(new Run(document.Title)) {
          FontSize = flowDocument.FontSize * 2.25,
          Margin = new Thickness(0)
        });
      if (!string.IsNullOrEmpty(document.SubTitle))
        flowDocument.Blocks.Add(new Paragraph(new Run(document.SubTitle)) { Foreground = Brushes.Gray } );
      flowDocument.Blocks.AddRange(visitor.GetBlocks(document.Content.Select(e => e.Visit(visitor))));
      Display(flowDocument);
    }

    private void flowViewer_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      try
      {
        e.Handled = true;
        if (e.Uri.IsAbsoluteUri)
        {
          Process.Start(e.Uri.ToString());
        }
        else
        {
          var match = Regex.Match(e.Uri.OriginalString ?? "", @"StartItem=(?<type>[A-Za-z0-9_% ]+):(?<name>[A-Za-z0-9_% ]+)");
          if (match.Success)
            TryNavigate(match.Groups["type"].Value, match.Groups["name"].Value);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private struct DocumentUrl : IEquatable<DocumentUrl>
    {
      public string Category { get; }
      public string Name { get; }

      public DocumentUrl(string category, string name)
      {
        Category = category;
        Name = name;
      }

      public override bool Equals(object obj)
      {
        if (obj is DocumentUrl url)
          return Equals(url);
        return false;
      }

      public bool Equals(DocumentUrl other)
      {
        return string.Equals(Category, other.Category, StringComparison.OrdinalIgnoreCase)
          && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
      }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        TryGoBackward();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        TryGoForward();
      }
      catch(Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var category = default(string);
        using (var categoryDialog = new FilterSelect<string>())
        {
          categoryDialog.DataSource = _categories;
          categoryDialog.Message = "Select a category";
          if (categoryDialog.ShowDialog(System.Windows.Forms.Application.OpenForms[0]) == DialogResult.OK && categoryDialog.SelectedItem != null)
            category = categoryDialog.SelectedItem;
        }

        if (!string.IsNullOrEmpty(category))
          PromptName(category);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void NameButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (_historyIndex >= 0 && _historyIndex < _history.Count)
          PromptName(_history[_historyIndex].Category);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void PromptName(string category)
    {
      switch (category.ToUpperInvariant())
      {
        case "ITEMTYPE":
          using (var nameDialog = new FilterSelect<ItemType>())
          {
            nameDialog.DataSource = _metadata.ItemTypes;
            nameDialog.DisplayMember = "Name";
            nameDialog.Message = "Select an item type";
            if (nameDialog.ShowDialog(System.Windows.Forms.Application.OpenForms[0]) == DialogResult.OK && nameDialog.SelectedItem != null)
            {
              TryNavigate(category, nameDialog.SelectedItem.Name);
            }
          }
          break;
        case "METHOD":
          using (var nameDialog = new FilterSelect<Method>())
          {
            nameDialog.DataSource = _metadata.Methods;
            nameDialog.DisplayMember = "KeyedName";
            nameDialog.Message = "Select a method";
            if (nameDialog.ShowDialog(System.Windows.Forms.Application.OpenForms[0]) == DialogResult.OK && nameDialog.SelectedItem != null)
            {
              TryNavigate(category, nameDialog.SelectedItem.KeyedName);
            }
          }
          break;
        default: // "General",
          using (var nameDialog = new FilterSelect<string>())
          {
            nameDialog.DataSource = _topics.Keys;
            nameDialog.Message = "Select a topic";
            if (nameDialog.ShowDialog(System.Windows.Forms.Application.OpenForms[0]) == DialogResult.OK && nameDialog.SelectedItem != null)
            {
              TryNavigate(category, nameDialog.SelectedItem);
            }
          }
          break;
      }
    }

    private void mniExportMarkdown_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (_document != null)
        {
          using (var saveFile = new SaveFileDialog())
          {
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
              using (var writer = new StreamWriter(saveFile.FileName))
              {
                var markdown = new Documentation.MarkdownVisitor(writer);
                markdown.Visit(_document);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
