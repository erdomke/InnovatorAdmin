using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innovator.Telemetry
{
  public class TextWrapper : TextWriter
  {
    private Stack<string> _indent = new Stack<string>();
    private int _position;
    private int _spaces;
    private TextWriter _writer;
    private bool _allowBreakNext = true;
    private char _onlyAllowBreakAfter = '\0';

    public override Encoding Encoding => _writer.Encoding;
    public int Indent => _indent.Sum(i => i.Length);
    public int MaxWidth { get; set; } = int.MaxValue;
    /// <summary>
    /// Whether to allow breaks in the middle of a word
    /// </summary>
    public bool BreakInWord { get; set; } = true;
    /// <summary>
    /// If set, breaks are only allowed after this individual character is written
    /// </summary>
    public char OnlyAllowBreakAfter
    {
      get { return _onlyAllowBreakAfter; }
      set
      {
        _allowBreakNext = value == '\0';
        _onlyAllowBreakAfter = value;
      }
    }

    public TextWrapper(TextWriter writer)
    {
      _writer = writer;
    }

    public void IncreaseIndent(int indent)
    {
      _indent.Push(new string(' ', indent));
    }

    public void IncreaseIndent(string value)
    {
      _indent.Push(value);
    }

    public void DecreaseIndent()
    {
      _indent.Pop();
    }

    public override void Flush()
    {
      _writer.Flush();
    }

    public override void Write(char value)
    {
      if (value == ' ')
      {
        WriteSpace(1);
      }
      else
      {
        if (_allowBreakNext && _position >= MaxWidth)
        {
          WriteLine();
          WriteIndent();
        }
        _writer.Write(value);
        _position++;
      }

      if (OnlyAllowBreakAfter != '\0')
        _allowBreakNext = value == OnlyAllowBreakAfter;
    }

    public override void Write(char[] buffer, int index, int count)
    {
      var start = index;
      var lineIndent = 0;

      for (var i = index; i < count; i++)
      {
        if (buffer[i] == '\r' || buffer[i] == '\n')
        {
          if (!IsBreakingWhitespace(buffer[start]))
            WriteWord(buffer, start, i - start);

          WriteLine();

          if (buffer[i] == '\r'
              && i + 1 < count
              && buffer[i + 1] == '\n')
            i++;
          start = i + 1;
          if (lineIndent > 0)
          {
            _indent.Pop();
            lineIndent = 0;
          }
        }
        else if (IsBreakingWhitespace(buffer[i]))
        {
          if (!IsBreakingWhitespace(buffer[start]))
          {
            WriteWord(buffer, start, i - start);
            start = i;
          }
        }
        else
        {
          if (IsBreakingWhitespace(buffer[start]))
          {
            if (_position == 0)
            {
              lineIndent = i - start;
              _indent.Push(new string(' ', lineIndent));
            }
            else
            {
              WriteSpace(i - start);
            }
            start = i;
          }
        }
      }

      if (start < count)
      {
        if (IsBreakingWhitespace(buffer[start]))
          WriteSpace(count - start);
        else
          WriteWord(buffer, start, count - start);
      }

      if (lineIndent > 0)
      {
        _indent.Pop();
      }
    }

    private static bool IsBreakingWhitespace(char ch)
    {
      return ch != 160 && char.IsWhiteSpace(ch);
    }

    public override void WriteLine()
    {
      _writer.WriteLine();
      _position = 0;
      _spaces = 0;
    }

    public TextWrapper WriteWord(string word)
    {
      return WriteWord(word.ToCharArray());
    }

    public TextWrapper WriteWord(char[] word, int start = 0, int length = -1)
    {
      if (length < 0)
        length = word.Length;

      if (_position < Indent)
      {
        WriteIndent();
      }
      else if (_allowBreakNext
        && _position > 0
        && length + _spaces + _position > Math.Max(MaxWidth, 1))
      {
        WriteLine();
        WriteIndent();
      }
      else
      {
        for (var i = 0; i < _spaces; i++)
          _writer.Write(' ');
        _position += _spaces;
      }
      _spaces = 0;
      var allowedLength = Math.Max(MaxWidth - Indent, 1);
      if (length > allowedLength && BreakInWord && _allowBreakNext)
        WriteChunks(word, start, length, allowedLength);
      else
        _writer.Write(word, start, length);
      _position += length;

      if (OnlyAllowBreakAfter != '\0')
        _allowBreakNext = false;

      return this;
    }

    private void WriteChunks(char[] word, int start, int length, int allowedLength)
    {
      var end = start + length;
      var first = true;
      while (start < end && char.IsWhiteSpace(word[start]))
        start++;

      while (start < end)
      {
        var chunkEnd = GetChunkEnd(word, start, Math.Min(allowedLength, end - start));
        if (first)
        {
          first = false;
        }
        else
        {
          WriteLine();
          WriteIndent();
        }
        _writer.Write(word, start, chunkEnd + 1 - start);
        _position += chunkEnd + 1 - start;
        start = chunkEnd + 1;
        while (start < end && char.IsWhiteSpace(word[start]))
          start++;
      }
    }

    private int GetChunkEnd(char[] word, int start, int length)
    {
      var result = start + length - 1;
      while (result >= start && !char.IsWhiteSpace(word[result]))
        result--;
      while (result >= start && char.IsWhiteSpace(word[result]))
        result--;

      if (result >= start)
        return result;

      result = start + length - 1;
      while (result >= start && char.IsLetterOrDigit(word[result]))
        result--;

      if (result >= start)
        return result;
      else
        return start + length - 1;
    }

    private void WriteIndent()
    {
      if (_indent.Count > 0)
      {
        var indentStr = string.Join("", _indent.Reverse());
        var maxLength = Math.Max(MaxWidth, 1) - 1;
        if (maxLength < indentStr.Length)
          indentStr = indentStr.Substring(0, maxLength);
        _writer.Write(indentStr.Substring(_position));
        _position = indentStr.Length;
      }
    }

    public TextWrapper WriteSpace(int count)
    {
      _spaces = count;
      return this;
    }

    public override string ToString()
    {
      return _writer.ToString();
    }
  }
}
