using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace InnovatorAdmin.Editor
{
  internal class TooltipElementVisitor : BaseWpfDocumentationVisitor
  {
    public override DependencyObject Visit(Documentation.List list)
    {
      var grid = new Grid
      {
        Margin = new Thickness(0, 9, 0, 9),
      };
      grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
      grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
      var offset = (list.Type == Documentation.ListType.Table ? 1 : 0);
      var rowCount = list.Children.Count + offset;
      for (var i = 0; i < rowCount; i++)
        grid.RowDefinitions.Add(new RowDefinition());

      if (list.Type == Documentation.ListType.Table)
      {
        var termHeader = default(UIElement);
        if (list.Header?.Term.Count > 0)
          termHeader = GetBlock(list.Header?.Term.Select(e => e.Visit(this)));
        else
          termHeader = new TextBlock(new Run("Term")) { TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.FromOpenTypeWeight(700) };
        termHeader = WithMargin(termHeader);
        Grid.SetRow(termHeader, 0);
        Grid.SetColumn(termHeader, 0);
        grid.Children.Add(termHeader);

        var descripHeader = default(UIElement);
        if (list.Header?.Description.Count > 0)
          descripHeader = GetBlock(list.Header?.Description.Select(e => e.Visit(this)));
        else
          descripHeader = new TextBlock(new Run("Description")) { TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.FromOpenTypeWeight(700) };
        descripHeader = WithMargin(descripHeader);
        Grid.SetRow(descripHeader, 0);
        Grid.SetColumn(descripHeader, 1);
        grid.Children.Add(descripHeader);
      }

      for (var i = 0; i < list.Children.Count; i++)
      {
        var term = default(UIElement);
        switch (list.Type)
        {
          case Documentation.ListType.Number:
            term = new TextBlock(new Run($"{i + 1}."));
            break;
          case Documentation.ListType.Table:
            term = GetBlock(list.Children[i].Term.Select(e => e.Visit(this)));
            break;
          default:
            term = new TextBlock(new Run($"●"));
            break;
        }
        term = WithMargin(term);
        Grid.SetRow(term, offset + i);
        Grid.SetColumn(term, 0);
        grid.Children.Add(term);

        var description = GetBlock(list.Children[i].Description.Select(e => e.Visit(this)));
        description = WithMargin(description);
        Grid.SetRow(description, offset + i);
        Grid.SetColumn(description, 1);
        grid.Children.Add(description);
      }
      return grid;
    }

    private Border WithMargin(UIElement element)
    {
      return new Border()
      {
        Child = element,
        Margin = new Thickness(3)
      };
    }

    public override DependencyObject Visit(Documentation.Section section)
    {
      throw new NotImplementedException();
    }

    protected override DependencyObject GetParagraph()
    {
      return new TextBlock()
      {
        TextWrapping = TextWrapping.Wrap
      };
    }

    protected override InlineCollection GetParagraphInlines(DependencyObject paragraph)
    {
      return ((TextBlock)paragraph).Inlines;
    }

    public UIElement GetBlock(IEnumerable<DependencyObject> elements)
    {
      var blocks = GetBlocks(elements).OfType<UIElement>().ToList();
      if (blocks.Count == 1)
        return blocks[0];
      var result = new StackPanel();
      foreach (var block in blocks)
        result.Children.Add(block);
      return result;
    }
  }
}
