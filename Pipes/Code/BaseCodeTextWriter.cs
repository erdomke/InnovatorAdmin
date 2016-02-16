using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Code
{
  public abstract class BaseCodeTextWriter : TextWriter, ICodeTextWriter
  {
    private bool _writeValue = false;
    private TextWriter _writer;

    public override Encoding Encoding
    {
      get { return _writer.Encoding; }
    }

    public BaseCodeTextWriter(TextWriter writer)
    {
      _writer = writer;
    }

    public TextWriter Raw()
    {
      _writeValue = false;
      return this;
    }

    public ICodeTextWriter RawEnd()
    {
      _writeValue = false;
      return this;
    }

    public virtual TextWriter StringValue()
    {
      Write('\'');
      _writeValue = true;
      return this;
    }

    public virtual ICodeTextWriter StringValueEnd()
    {
      _writeValue = false;
      Write('\'');
      return this;
    }

    public Code.IBaseCodeWriter Comment(string value)
    {
      _writer.Write("/* ");
      _writer.Write(value);
      _writer.Write("*/");
      return this;
    }

    public Code.IBaseCodeWriter CommentLine(string value)
    {
      _writer.Write("-- ");
      _writer.WriteLine(value);
      return this;
    }

    public abstract Code.IBaseCodeWriter DateValue(object value);

    public Code.IBaseCodeWriter Line()
    {
      _writer.WriteLine();
      return this;
    }

    public Code.IBaseCodeWriter Line(string value)
    {
      _writer.WriteLine(value);
      return this;
    }

    public new Code.IBaseCodeWriter Null()
    {
      _writer.Write("null");
      return this;
    }

    public Code.IBaseCodeWriter NumberValue(object value)
    {
      double dblValue;
      long lngValue;
      if (value is byte ||
          value is Int16 || value is UInt16 ||
          value is Int32 || value is UInt32 ||
          value is Int64 || value is UInt64 ||
          value is float || value is double)
      {
        _writer.Write(value);
      }
      else if (value == null || value is DBNull)
      {
        return Null();
      }
      else
      {
        if (long.TryParse(value.ToString(), out lngValue))
        {
          _writer.Write(lngValue);
        }
        else if (double.TryParse(value.ToString(), out dblValue))
        {
          _writer.Write(dblValue);
        }
        else
        {
          return Null();
        }
      }
      return this;
    }

    public Code.IBaseCodeWriter Raw(string value)
    {
      _writeValue = false;
      Write(value);
      return this;
    }

    public Code.IBaseCodeWriter StringValue(object value)
    {
      if (value == null || value is DBNull)
      {
        return Null();
      }
      else
      {
        try
        {
          StringValue();
          Write(value.ToString());
        }
        finally
        {
          StringValueEnd();
        }
        return this;
      }
    }

    public override void Write(char value)
    {
      Write(value.ToString());
    }
    public override void Write(char[] buffer, int index, int count)
    {
      Write(new String(buffer, index, count));
    }
    public override void Write(string value)
    {
      if (_writeValue)
      {
        _writer.Write(value.Replace("'", "''"));
      }
      else 
      {
        _writer.Write(value);
      }
    }

    public virtual Code.IBaseCodeWriter Identifier(object value)
    {
      try
      {
        Write('[');
        Write(value.ToString().Replace("[","").Replace("]","").Replace(";","").Replace("\"",""));
      }
      finally
      {
        Write(']');
      }
      return this;

    }

  }
}
