using System;
using System.Text;

namespace InnovatorAdmin.Documentation
{
  public class StringVisitor : IElementVisitor<StringVisitor>
  {
    private StringBuilder _builder = new StringBuilder();

    public StringVisitor Visit(CodeBlock codeBlock)
    {
      if (_builder.Length > 0)
        _builder.AppendLine().AppendLine();
      _builder.Append(codeBlock.Code);
      return this;
    }

    public StringVisitor Visit(DocLink docLink)
    {
      _builder.Append(docLink.Name);
      return this;
    }

    public StringVisitor Visit(Hyperlink hyperlink)
    {
      foreach (var child in hyperlink.Children)
        child.Visit(this);
      return this;
    }

    public StringVisitor Visit(List list)
    {
      if (_builder.Length > 0)
        _builder.AppendLine().AppendLine();

      for (var i = 0; i < list.Children.Count; i++)
      {
        if (list.Type == ListType.Number)
          _builder.Append(i + 1).Append(". ");
        else
          _builder.Append("- ");

        if (list.Type == ListType.Table)
        {
          foreach (var child in list.Children[i].Term)
            child.Visit(this);
          _builder.Append(": ");
        }
        foreach (var child in list.Children[i].Description)
          child.Visit(this);
      }

      return this;
    }

    public StringVisitor Visit(Paragraph paragraph)
    {
      if (_builder.Length > 0)
        _builder.AppendLine().AppendLine();

      foreach (var child in paragraph.Children)
        child.Visit(this);

      return this;
    }

    public StringVisitor Visit(Section section)
    {
      throw new NotImplementedException();
    }

    public StringVisitor Visit(TextRun run)
    {
      _builder.Append(run.Text);
      return this;
    }

    public override string ToString()
    {
      return _builder.ToString();
    }
  }
}
