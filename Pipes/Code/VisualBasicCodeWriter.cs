using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Code
{
  public class VisualBasicCodeWriter : IImperativeCodeWriter
  {
    private TextWriter _writer;

    public VisualBasicCodeWriter(TextWriter writer)
    {
      _writer = writer;
    }

    public IImperativeCodeWriter LineEnd()
    {
      Line();
      return this;
    }

    public IImperativeCodeWriter LineContinue()
    {
      _writer.WriteLine(" _");
      return this;
    }

    public void Flush()
    {
      _writer.Flush();
    }

    public IBaseCodeWriter Comment(string value)
    {
      var reader = new IO.StringTextSource(value).Pipe(new Data.DelimitedTextReader());
      reader.AddDelim('\r');
      foreach (var line in reader)
      {
        _writer.Write("' ");
        _writer.WriteLine(line);
      }
      return this;
    }

    public IBaseCodeWriter CommentLine(string value)
    {
      return Comment(value);
    }

    public IBaseCodeWriter DateValue(object value)
    {
      if (value == null || value == DBNull.Value)
      {
        return Null();
      }
      else
      {
        _writer.Write("#");
        _writer.Write(value.ToString());
        _writer.Write("#");
        return this;
      }
    }

    public IBaseCodeWriter Line()
    {
      _writer.WriteLine();
      return this;
    }

    public IBaseCodeWriter Line(string value)
    {
      _writer.WriteLine(value);
      return this;
    }

    public IBaseCodeWriter Null()
    {
      _writer.Write("Nothing");
      return this;
    }

    public IBaseCodeWriter NumberValue(object value)
    {
      if (value == null || value == DBNull.Value)
      {
        return Null();
      }
      else
      {
        _writer.Write(value.ToString());
        return this;
      }
    }

    public IBaseCodeWriter Raw(string value)
    {
      _writer.Write(value);
      return this;
    }

    public IBaseCodeWriter StringValue(object value)
    {
      if (value == null || value == DBNull.Value)
      {
        return Null();
      }
      else
      {
        var chars = value.ToString().ToCharArray();

        int runIndex = -1;
        char lastChar = '\0';

        for (var index = 0; index < chars.Length; ++index)
        {
          var c = chars[index];

          if (c != '\t' && c != '\n' && c != '\r' && c != '"')
          {
            if (runIndex == -1)
              runIndex = index;

            continue;
          }

          if (runIndex != -1)
          {
            WriteStringStart();
            _writer.Write(chars, runIndex, index - runIndex);
            runIndex = -1;
          }

          switch (c)
          {
            case '\t':
              WriteStringEnd(false);
              _writer.Write(" & vbTab");
              break;
            case '\r':
              WriteStringEnd(false);
              _writer.Write(" & vbCr");
              break;
            case '\n':
              WriteStringEnd(false);
              if (lastChar == '\r')
              {
                _writer.Write("Lf");
              }
              else
              {
                _writer.Write(" & vbLf");
              }
              break;
            case '"':
              WriteStringStart();
              _writer.Write("\"\"");
              break;
            default:
              WriteStringStart();
              _writer.Write(c);
              break;
          }
          lastChar = c;
        }

        if (runIndex != -1)
        {
          WriteStringStart();
          _writer.Write(chars, runIndex, chars.Length - runIndex);
        }

        WriteStringEnd(true);
        return this;
      }
    }

    private enum StringPart { None, Inside, Outside }
    private StringPart _currStringPart = StringPart.None;
    private void WriteStringStart()
    {
      if (_currStringPart == StringPart.None)
      {
        _writer.Write("\"");
      }
      else if (_currStringPart == StringPart.Outside)
      {
        _writer.Write(" & \"");
      }
      _currStringPart = StringPart.Inside;
    }
    private void WriteStringEnd(bool final)
    {
      if (_currStringPart == StringPart.Inside) _writer.Write("\"");
      if (final)
      {
        _currStringPart = StringPart.None;
      }
      else
      {
        _currStringPart = StringPart.Outside;
      }
    }


    public IBaseCodeWriter Identifier(object value)
    {
      throw new NotImplementedException();
    }
  }
}
