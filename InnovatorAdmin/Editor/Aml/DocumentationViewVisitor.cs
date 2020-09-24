using ICSharpCode.AvalonEdit;
using InnovatorAdmin.Documentation;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace InnovatorAdmin.Editor
{
  internal class DocumentationViewVisitor : BaseWpfDocumentationVisitor
  {
    private static double[] _headingScales = new[] { 2.25, 2, 1.75, 1.5, 1.25, 1.125 };
    private int _listLevel;

    public int HeadingLevel { get; set; }
    public double BaseFontSize { get; set; } = 16;

    public override DependencyObject Visit(Documentation.CodeBlock codeBlock)
    {
      var editor = new TextEditor
      {
        Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(244, 244, 244)),
        Text = codeBlock.Code,
        FontFamily = new System.Windows.Media.FontFamily("Consolas"),
        FontSize = BaseFontSize,
        HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
        VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
        IsReadOnly = true,
        SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension("." + codeBlock.Language)
      };
      return new BlockUIContainer
      {
        Child = editor,
        Margin = new Thickness(0, BaseFontSize, 0, BaseFontSize)
      };
    }

    public override DependencyObject Visit(Documentation.List list)
    {
      if (list.Type == ListType.Table)
      {
        var result = new Table();
        result.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
        result.Columns.Add(new TableColumn() { Width = new GridLength(3, GridUnitType.Star) });
        result.RowGroups.Add(new TableRowGroup());
        var header = new TableRow();

        var termHeader = new TableCell();
        if (list.Header?.Term.Count > 0)
          termHeader.Blocks.AddRange(GetBlocks(list.Header?.Term.Select(e => e.Visit(this))));
        else
          termHeader.Blocks.Add(new System.Windows.Documents.Paragraph(new Run("Term") { FontWeight = FontWeight.FromOpenTypeWeight(700) }));
        header.Cells.Add(termHeader);

        var descripHeader = new TableCell();
        if (list.Header?.Description.Count > 0)
          descripHeader.Blocks.AddRange(GetBlocks(list.Header?.Description.Select(e => e.Visit(this))));
        else
          descripHeader.Blocks.Add(new System.Windows.Documents.Paragraph(new Run("Description") { FontWeight = FontWeight.FromOpenTypeWeight(700) }));
        header.Cells.Add(descripHeader);
        result.RowGroups[0].Rows.Add(header);

        foreach (var item in list.Children)
        {
          var row = new TableRow();
          row.Cells.Add(new TableCell());
          row.Cells.Add(new TableCell());
          if (item.Term.Count > 0)
            row.Cells[0].Blocks.AddRange(GetBlocks(item.Term.Select(e => e.Visit(this))));
          if (item.Description.Count > 0)
            row.Cells[1].Blocks.AddRange(GetBlocks(item.Description.Select(e => e.Visit(this))));
          result.RowGroups[0].Rows.Add(row);
        }
        return result;
      }
      else
      {
        var result = new System.Windows.Documents.List
        {
          MarkerStyle = list.Type == ListType.Number ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc
        };
        if (_listLevel > 0)
          result.Margin = new Thickness(0);
        try
        {
          _listLevel++;
          result.ListItems.AddRange(list.Children.Select(i =>
          {
            var item = new System.Windows.Documents.ListItem();
            item.Blocks.AddRange(GetBlocks(i.Description.Select(e => e.Visit(this))));
            return item;
          }));
          return result;
        }
        finally
        {
          _listLevel--;
        }
      }
    }

    public override DependencyObject Visit(Documentation.Section section)
    {
      var result = new System.Windows.Documents.Section();
      var scale = _headingScales[Math.Min(HeadingLevel, _headingScales.Length - 1)];
      result.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(section.Header) { FontSize = scale * BaseFontSize }));
      try
      {
        HeadingLevel++;
        result.Blocks.AddRange(GetBlocks(section.Children.Select(e => e.Visit(this))));
        return result;
      }
      finally
      {
        HeadingLevel--;
      }
    }

    protected override DependencyObject GetParagraph()
    {
      return new System.Windows.Documents.Paragraph();
    }

    protected override InlineCollection GetParagraphInlines(DependencyObject paragraph)
    {
      return ((System.Windows.Documents.Paragraph)paragraph).Inlines;
    }
  }
}
