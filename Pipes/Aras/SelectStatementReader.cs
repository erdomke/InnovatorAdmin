using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class SelectStatementReader : IPipeOutput<SelectColumn>, IPipeInput<System.IO.TextReader>
  {
    private int _bufferSize = 512;
    private IEnumerable<System.IO.TextReader> _source;

    public int BufferSize
    {
      get
      {
        return _bufferSize;
      }
      set
      {
        _bufferSize = value;
      }
    }

    public IEnumerator<SelectColumn> GetEnumerator()
    {
      var buffer = new char[_bufferSize * 2];
      char[] newBuffer;
      int lastDelim = 0;
      int bufferLength = 0;
      int i = 0;
      int parenDepth = 0;
      int parenStart = -1;
      
      foreach (var textReader in _source)
      {
        bufferLength = textReader.ReadBlock(buffer, 0, _bufferSize);
        i = 0;
        while (bufferLength > i)
        {
          lastDelim = 0;
          while (i < bufferLength)
          {
            if (buffer[i] == '(')
            {
              if (parenDepth <= 0)
              {
                parenStart = i;
                parenDepth = 1;
              }
              else
              {
                parenDepth++;
              }
            }
            else if (buffer[i] == ')')
            {
              parenDepth--;
            }
            else if (buffer[i] == ',' && parenDepth <= 0)
            {
              if (i != lastDelim)
              {
                if (parenStart < 0)
                {
                  yield return new SelectColumn(new string(buffer, lastDelim, i - lastDelim));
                }
                else
                {
                  if (buffer[i - 1] == ')')
                  {
                    yield return new SelectColumn(new string(buffer, lastDelim, parenStart - lastDelim), new string(buffer, parenStart + 1, i - parenStart - 2));
                  }
                  else
                  {
                    yield return new SelectColumn(new string(buffer, lastDelim, parenStart - lastDelim), new string(buffer, parenStart, i - parenStart));
                  }
                }
              }
              lastDelim = i + 1;
              parenStart = -1;
            }

            i++;
          }

          i = bufferLength - lastDelim;
          if (bufferLength > lastDelim && lastDelim > 0)
          {
            newBuffer = new char[bufferLength - lastDelim + _bufferSize];
            Array.Copy(buffer, lastDelim, newBuffer, 0, bufferLength - lastDelim);
            buffer = newBuffer;
          }
          else if ((bufferLength - lastDelim + _bufferSize) > buffer.Length)
          {
            Array.Resize(ref buffer, buffer.Length + _bufferSize);
          }
          bufferLength = textReader.ReadBlock(buffer, i, _bufferSize) + i;
          parenStart -= lastDelim;
          lastDelim = 0;
        }

        if (i != 0)
        {
          if (buffer[i - 1] == ')')
          {
            yield return new SelectColumn(new string(buffer, lastDelim, parenStart - lastDelim), new string(buffer, parenStart + 1, i - parenStart - 2));
          }
          else
          {
            yield return new SelectColumn(new string(buffer, 0, i));
          }
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Initialize(IEnumerable<System.IO.TextReader> source)
    {
      _source = source;
    }

    public static IEnumerable<SelectColumn> GetColumns(string input)
    {
      return new IO.StringTextSource(input).Pipe(new SelectStatementReader());
    }
  }
}
