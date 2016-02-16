using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class DelimitedTextReader : IPipeInput<System.IO.TextReader>, IPipeOutput<String>
  {
    private enum QuoteState
    {
      none, start, end
    }

    private int _bufferSize = 512;
    private char _delim = '\0';
    private Func<char[], int, int> _delimMatcher = null;
    private char _enclosingCharEscape;
    private char _fieldEnclosingChar = '\0';
    private bool _newLineDelim = false;
    private bool _skipEmpty = false;
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
    public char FieldEnclosingChar
    { 
      get { return _fieldEnclosingChar; } 
      set { _fieldEnclosingChar = value; }
    }
    public char EnclosingCharEscape
    {
      get
      {
        return _enclosingCharEscape;
      }
      set
      {
        _enclosingCharEscape = value;
      }
    }
    public bool SkipEmpty
    {
      get
      {
        return _skipEmpty;
      }
      set
      {
        _skipEmpty = value;
      }
    }

    public void AddDelim(char delim)
    {
      if (delim == '\0') return;

      if (delim == '\r' || delim == '\n')
      {
        _newLineDelim = true;
      } 
      else if (_delim == '\0')
      {
        _delim = delim;
      }
      else
      {
        if (_delimMatcher == null)
        {
          _delimMatcher = (buffer, endIndex) =>
          {
            if (buffer[endIndex] == delim)
            {
              return endIndex;
            }
            else
            {
              return -1;
            }
          };
        }
        else
        {
          var currMatcher = _delimMatcher;
          _delimMatcher = (buffer, endIndex) =>
          {
            if (buffer[endIndex] == delim)
            {
              return endIndex;
            }
            else if (currMatcher == null)
            {
              return -1;
            }
            else
            {
              return currMatcher(buffer, endIndex);
            }
          };
        }
      }
    }
    public void AddDelim(string delim)
    {
      if (string.IsNullOrEmpty(delim))
      {
        return;
      }
      else
      {
        if (delim.Length == 1)
        {
          AddDelim(delim[0]);
        }
        else if (delim == "\r\n")
        {
          _newLineDelim = true;
        }
        else
        {
          var chars = delim.ToCharArray();
          if (_delimMatcher == null)
          {
            _delimMatcher = (buffer, endIndex) =>
            {
              endIndex -= chars.Length - 1;
              if (endIndex < 0)
              {
                return -1;
              }
              else
              {
                for (var i = 0; i < chars.Length; i++)
                {
                  if (chars[i] != buffer[endIndex + i]) return -1;
                }
                return endIndex;
              }
            };
          }
          else
          {
            var currMatcher = _delimMatcher;
            _delimMatcher = (buffer, endIndex) =>
            {
              endIndex -= chars.Length - 1;
              var result = endIndex;
              if (endIndex >= 0)
              {
                for (var i = 0; i < chars.Length; i++)
                {
                  if (chars[i] != buffer[endIndex + i])
                  {
                    result = -1;
                    break;
                  }
                }
              }
              else
              {
                result = -1;
              }

              if (result < 0 && currMatcher != null)
              {
                return currMatcher(buffer, endIndex);
              }
              else
              {
                return result;
              }
            };
          }
        }
      }
    }

    public IEnumerator<string> GetEnumerator()
    {
      return new DelimitedEnumerator(this);
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Initialize(IEnumerable<System.IO.TextReader> source)
    {
      _source = source;
    }

    private interface IMatcher
    {
      IMatcher NextCheck { get; set; }
      int EndingDelimIndex(char[] buffer, int endIndex, string prevValue);
    }

    public class DelimitedEnumerator : IEnumerator<string>
    {
      private DelimitedTextReader _parent;
      private IEnumerator<System.IO.TextReader> _enum;
      private string _curr;
      private System.IO.TextReader _reader;
      private char _lastMatch;

      private char[] buffer;
      private char[] newBuffer;
      private int lastDelim = 0;
      private int bufferLength = 0;
      private QuoteState quotes = QuoteState.none;
      private int i = 0;
      private int delimMatch;
      private int fieldEnd = int.MaxValue;
      private int lastEscape = -2;

      public DelimitedEnumerator(DelimitedTextReader parent)
      {
        _parent = parent;
        _enum = _parent._source.GetEnumerator();
        buffer = new char[_parent._bufferSize * 2];
      }

      public string Current
      {
        get { return _curr; }
      }
      object System.Collections.IEnumerator.Current
      {
        get { return _curr; }
      }
      public char LastMatch
      {
        get { return _lastMatch; }
      }

      public void Dispose()
      {
        // Do Nothing
      }

      public bool MoveNext()
      {
        while (true)
        {
          _curr = null;
          if (_reader == null)
          {
            if (_enum.MoveNext())
            {
              _lastMatch = '\0';
              _reader = _enum.Current;

              if (_parent._delim == '\0' && _parent._delimMatcher == null && !_parent._newLineDelim)
              {
                _curr = _reader.ReadToEnd();
                _reader = null;
                return true;
              }
              else
              {
                bufferLength = _reader.ReadBlock(buffer, 0, _parent._bufferSize);
                i = 0;
                lastDelim = 0;
              }
            }
            else
            {
              _reader = null;
              return false;
            }
          }

          if (i > 0 && bufferLength == i) ReloadBuffer();

          while (bufferLength > i)
          {
            while (i < bufferLength)
            {
              if (_parent._fieldEnclosingChar > 0 && buffer[i] == _parent._fieldEnclosingChar)
              {
                if (_parent._enclosingCharEscape > 0 && i > 0 && buffer[i - 1] == _parent._enclosingCharEscape && i > (lastEscape + 1))
                {
                  Array.Copy(buffer, i, buffer, i - 1, bufferLength - i);
                  i--;
                  bufferLength--;
                  quotes = QuoteState.start;
                  lastEscape = i;
                }
                else if (quotes == QuoteState.start)
                {
                  fieldEnd = i;
                  quotes = QuoteState.end;
                }
                else
                {
                  if (quotes == QuoteState.none && i == lastDelim) lastDelim++;
                  quotes = QuoteState.start;
                }
              }
              else if (quotes != QuoteState.start)
              {
                delimMatch = -1;
                if (_parent._delim > 0) delimMatch = (buffer[i] == _parent._delim ? i : -1);
                if (_parent._newLineDelim && delimMatch < 0) delimMatch = (buffer[i] == '\r' || buffer[i] == '\n' ? i : -1);
                if (_parent._delimMatcher != null && delimMatch < 0) delimMatch = _parent._delimMatcher(buffer, i);
                if (delimMatch >= 0)
                {
                  if (i == lastDelim && _lastMatch == '\r' && buffer[i] == '\n')
                  {
                    // Do Nothing
                  }
                  else
                  {
                    if (fieldEnd < delimMatch) delimMatch = fieldEnd;
                    if (delimMatch == lastDelim)
                    {
                      if (!_parent._skipEmpty)
                      {
                        _curr = string.Empty;
                      }
                    }
                    else
                    {
                      _curr = new string(buffer, lastDelim, delimMatch - lastDelim);
                    }
                  }
                  _lastMatch = buffer[i];
                  fieldEnd = int.MaxValue;
                  lastDelim = i + 1;
                  quotes = QuoteState.none;
                }
              }
              i++;
              if (_curr != null) return true;
            }

            ReloadBuffer();
          }

          _reader = null;
          if (i == lastDelim)
          {
            if (!_parent._skipEmpty)
            {
              _curr = string.Empty;
              return true;
            }
          }
          else
          {
            _curr = new string(buffer, 0, i);
            return true;
          }
        }
      }
      private void ReloadBuffer()
      {
        i = bufferLength - lastDelim;
        if (bufferLength > lastDelim && lastDelim > 0)
        {
          newBuffer = new char[bufferLength - lastDelim + _parent._bufferSize];
          Array.Copy(buffer, lastDelim, newBuffer, 0, bufferLength - lastDelim);
          buffer = newBuffer;
        }
        else if ((bufferLength - lastDelim + _parent._bufferSize) > buffer.Length)
        {
          Array.Resize(ref buffer, buffer.Length + _parent._bufferSize);
        }
        bufferLength = _reader.ReadBlock(buffer, i, _parent._bufferSize) + i;
        lastDelim = 0;
      }

      public void Reset()
      {
        _enum = _parent._source.GetEnumerator();
      }
    }

    private class CharMatcher : IMatcher
    {
      private char _value;

      public IMatcher NextCheck { get; set; }

      public CharMatcher(char value)
      {
        _value = value;
      }
      public int EndingDelimIndex(char[] buffer, int endIndex, string prevValue)
      {
        if (buffer[endIndex] == _value)
        {
          return endIndex;
        }
        else if (NextCheck == null)
        {
          return -1;
        }
        else
        {
          return NextCheck.EndingDelimIndex(buffer, endIndex, prevValue);
        }
      }
    }
  }
}
