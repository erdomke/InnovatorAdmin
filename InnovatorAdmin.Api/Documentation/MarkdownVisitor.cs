using System.IO;

namespace InnovatorAdmin.Documentation
{
  public class MarkdownVisitor : IElementVisitor<object>
  {
    private enum WriteState
    {
      DocStart,
      NewLine,
      Text
    }

    private WriteState _state;
    private readonly TextWriter _writer;
    private int _headerLevel = 1;
    private int _indent = 0;

    public MarkdownVisitor(TextWriter writer)
    {
      _writer = writer;
    }

    public object Visit(Document document)
    {
      StartNewParagraph();
      if (!string.IsNullOrEmpty(document.Title))
      {
        _writer.Write(new string('#', _headerLevel));
        _writer.Write(' ');
        Escape(document.Title, _writer);
        _writer.WriteLine();
      }
      if (!string.IsNullOrEmpty(document.SubTitle))
      {
        _writer.WriteLine();
        _writer.Write('*');
        Escape(document.SubTitle, _writer);
        _writer.WriteLine('*');
      }
      _state = WriteState.NewLine;
      StartNewParagraph();
      try
      {
        if (!string.IsNullOrEmpty(document.Title))
          _headerLevel++;
        foreach (var child in document.Content)
          child.Visit(this);
        return null;
      }
      finally
      {
        if (!string.IsNullOrEmpty(document.Title))
          _headerLevel--;
      }
    }

    public object Visit(CodeBlock codeBlock)
    {
      StartNewParagraph();
      _writer.Write("```");
      if (!string.IsNullOrEmpty(codeBlock.Language))
        _writer.Write(codeBlock.Language);
      _writer.WriteLine();
      if (_indent > 0)
      {
        var lines = codeBlock.Code.Replace("\r\n", "\n").Replace('\r', '\n').Trim('\n').Split('\n');
        foreach (var line in lines)
        {
          _writer.Write(new string(' ', _indent));
          _writer.WriteLine(line);
        }
        _writer.Write(new string(' ', _indent));
        _writer.WriteLine("```");
      }
      else
      {
        _writer.Write(codeBlock.Code.Trim('\r', '\n'));
        _writer.WriteLine();
        _writer.WriteLine("```");
      }
      _state = WriteState.NewLine;
      return null;
    }

    public object Visit(DocLink docLink)
    {
      _writer.Write('[');
      Escape(docLink.Name, _writer);
      _writer.Write(']');
      _writer.Write("(#");
      _writer.Write(docLink.Type);
      _writer.Write('.');
      _writer.Write(docLink.Name ?? docLink.Id);
      _writer.Write(')');
      _state = WriteState.Text;
      return null;
    }

    public object Visit(Hyperlink hyperlink)
    {
      _writer.Write('[');
      foreach (var child in hyperlink.Children)
        child.Visit(this);
      _writer.Write(']');
      _writer.Write("(");
      _writer.Write(hyperlink.Href);
      _writer.Write(')');
      _state = WriteState.Text;
      return null;
    }

    public object Visit(List list)
    {
      if (_indent < 1)
      {
        StartNewParagraph();
      }
      else
      {
        if (_state == WriteState.Text)
        {
          _writer.WriteLine();
          _state = WriteState.NewLine;
        }
        _writer.Write(new string(' ', _indent));
      }

      if (list.Type == ListType.Table)
      {
        _writer.Write('|');
        if (list.Header?.Term == null)
        {
          _writer.Write("Term");
        }
        else
        {
          foreach (var child in list.Header.Term)
            child.Visit(this);
        }
        _writer.Write('|');
        if (list.Header?.Description == null)
        {
          _writer.Write("Description");
        }
        else
        {
          foreach (var child in list.Header.Description)
            child.Visit(this);
        }
        _writer.WriteLine('|');
        if (_indent > 0)
          _writer.Write(new string(' ', _indent));
        _writer.WriteLine("|--|--|");

        foreach (var item in list.Children)
        {
          if (_indent > 0)
            _writer.Write(new string(' ', _indent));
          _writer.Write('|');
          foreach (var child in item.Term)
            child.Visit(this);
          _writer.Write('|');
          foreach (var child in item.Description)
            child.Visit(this);
          _writer.WriteLine('|');
        }
        _state = WriteState.NewLine;
      }
      else
      {
        var prefix = list.Type == ListType.Number ? "1. " : "- ";
        var first = true;
        foreach (var item in list.Children)
        {
          if (first)
            first = false;
          else
            _writer.Write(new string(' ', _indent));
          _writer.Write(prefix);
          _indent += prefix.Length;
          try
          {
            foreach (var child in item.Description)
              child.Visit(this);
            if (_state != WriteState.NewLine)
            {
              _writer.WriteLine();
              _state = WriteState.NewLine;
            }
          }
          finally
          {
            _indent -= prefix.Length;
          }
        }
      }
      return null;
    }

    public object Visit(Paragraph paragraph)
    {
      StartNewParagraph();
      foreach (var child in paragraph.Children)
        child.Visit(this);
      return null;
    }

    public object Visit(Section section)
    {
      StartNewParagraph();
      _writer.Write(new string('#', _headerLevel));
      _writer.Write(' ');
      Escape(section.Header, _writer);
      _writer.WriteLine();
      _state = WriteState.NewLine;
      try
      {
        _headerLevel++;
        foreach (var child in section.Children)
          child.Visit(this);
        return null;
      }
      finally
      {
        _headerLevel--;
      }
    }

    public object Visit(TextRun run)
    {
      if (string.IsNullOrEmpty(run.Text))
        return null;

      var prefix = run.Style.HasFlag(RunStyle.Code)
        ? "`"
        : ((run.Style.HasFlag(RunStyle.Bold) ? "**" : "")
          + (run.Style.HasFlag(RunStyle.Italic) ? "*" : ""));

      if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(run.Text))
      {
        Escape(run.Text, _writer);
      }
      else
      {
        var firstNonWhitespace = 0;
        while (char.IsWhiteSpace(run.Text[firstNonWhitespace]))
          firstNonWhitespace++;
        var lastNonWhitespace = run.Text.Length - 1;
        while (char.IsWhiteSpace(run.Text[lastNonWhitespace]))
          lastNonWhitespace--;

        if (firstNonWhitespace > 0)
          _writer.Write(run.Text.Substring(0, firstNonWhitespace));
        _writer.Write(prefix);
        if (prefix == "`")
          _writer.Write(run.Text.Substring(firstNonWhitespace, lastNonWhitespace - firstNonWhitespace + 1));
        else
          Escape(run.Text.Substring(firstNonWhitespace, lastNonWhitespace - firstNonWhitespace + 1), _writer);
        _writer.Write(prefix);
        if (lastNonWhitespace < run.Text.Length - 1)
          _writer.Write(run.Text.Substring(lastNonWhitespace + 1));
      }
      _state = WriteState.Text;
      return this;
    }

    private void StartNewParagraph()
    {
      switch (_state)
      {
        case WriteState.NewLine:
          _writer.WriteLine();
          break;
        case WriteState.Text:
          _writer.WriteLine();
          _writer.WriteLine();
          _state = WriteState.NewLine;
          break;
      }
      if (_indent > 0)
        _writer.Write(new string(' ', _indent));
    }


    public static string Escape(string value)
    {
      using (var writer = new StringWriter())
      {
        Escape(value, writer);
        return writer.ToString();
      }
    }

    public static void Escape(string value, TextWriter writer)
    {
      if (value == null)
        return;

      foreach (var ch in value)
      {
        switch (ch)
        {
          case '\\':
          case '`':
          case '*':
          case '_':
          case '[':
          case ']':
          case '|':
          case '#':
            writer.Write('\\');
            break;
        }
        writer.Write(ch);
      }
    }
  }
}
