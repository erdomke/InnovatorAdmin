using InnovatorAdmin.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  internal abstract class BaseWpfDocumentationVisitor : IElementVisitor<DependencyObject>
  {
    public virtual DependencyObject Visit(CodeBlock codeBlock)
    {
      var result = GetParagraph();
      GetParagraphInlines(result).Add(new Run(codeBlock.Code)
      {
        FontFamily = new FontFamily("Consolas")
      });
      return result;
    }

    public virtual DependencyObject Visit(DocLink docLink)
    {
      var result = new System.Windows.Documents.Hyperlink()
      {
        NavigateUri = new Uri($"#StartItem={docLink.Type}:{docLink.Id ?? docLink.Name}", UriKind.Relative)
      };
      result.Inlines.Add(new Run(docLink.Name));
      return result;
    }

    public virtual DependencyObject Visit(Documentation.Hyperlink hyperlink)
    {
      var result = new System.Windows.Documents.Hyperlink()
      {
        NavigateUri = new Uri(hyperlink.Href)
      };
      result.Inlines.AddRange(hyperlink.Children
        .Select(c => c.Visit(this))
        .OfType<Inline>());
      return result;
    }

    public abstract DependencyObject Visit(Documentation.List list);

    public IEnumerable<DependencyObject> GetBlocks(IEnumerable<DependencyObject> elements)
    {
      var lastParagraph = default(DependencyObject);
      foreach (var child in elements)
      {
        if (child is Inline inline)
        {
          if (lastParagraph == null)
            lastParagraph = GetParagraph();
          GetParagraphInlines(lastParagraph).Add(inline);
        }
        else 
        {
          if (lastParagraph != null)
          {
            yield return lastParagraph;
            lastParagraph = null;
          }
          yield return child;
        }
      }
      if (lastParagraph != null)
        yield return lastParagraph;
    }

    public virtual DependencyObject Visit(Documentation.Paragraph paragraph)
    {
      var result = GetParagraph();
      GetParagraphInlines(result).AddRange(paragraph.Children
        .Select(c => c.Visit(this))
        .OfType<Inline>());
      return result;
    }

    public abstract DependencyObject Visit(Documentation.Section section);

    public DependencyObject Visit(TextRun run)
    {
      var result = new Run(run.Text);
      if ((run.Style & RunStyle.Bold) > 0)
        result.FontWeight = FontWeight.FromOpenTypeWeight(700);
      if ((run.Style & RunStyle.Italic) > 0)
        result.FontStyle = FontStyles.Italic;
      if ((run.Style & RunStyle.Code) > 0)
      {
        result.FontFamily = new FontFamily("Consolas");
        result.Foreground = Brushes.DarkGray;
      }
      return result;
    }

    protected abstract DependencyObject GetParagraph();
    protected abstract InlineCollection GetParagraphInlines(DependencyObject paragraph);
  }
}
