using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ADiff
{
  public class LineWriter : StringWriter
  {
    int _currLine = 0;

    public int Line { get { return _currLine; } }

    public override Encoding Encoding
    {
      get { return Encoding.UTF8; }
    }

    public override void Write(char[] buffer, int index, int count)
    {
      if (_currLine == 0) _currLine++;
      base.Write(buffer, index, count);
      for (int i = index; i < count; i++)
      {
        if (buffer[i] == '\r' || (buffer[i] == '\n' && (i == 0 || buffer[i-1] != '\r')))
        {
          _currLine++;
        }
      }
    }

    public override void Write(string value)
    {
      this.Write(value.ToCharArray());
    }
  }
}
