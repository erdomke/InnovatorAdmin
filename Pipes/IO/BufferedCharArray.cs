using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Pipes.IO
{
  public class BufferedCharArray
  {
    private int _bookmark = 0;
    private char[] _buffer;
    private int _bufferSize = 512;
    private int _bufferLength = 0;
    private int _eof = 1;
    private int _index = 0;
    private System.IO.TextReader _reader;

    public BufferedCharArray(System.IO.TextReader reader)
    {
      _reader = reader;
      _buffer = new char[_bufferSize * 2];
    }

    public char MoveNext()
    {
      if (_index >= _eof) return '\0';
      if (_index >= _bufferLength) ReloadBuffer();
      // If the buffer load found zero new items, be sure to bail appropriately
      if (_index >= _eof) return '\0';
      return _buffer[_index++];
    }
    public void MoveNext(int count)
    {
      for (int i = 0; i < count; i++)
      {
        MoveNext();
      }
    }
    public void MovePrev(int count)
    {
      var i = 0;
      while (_index > 0 && i < count)
      {
        _index--;
        i++;
      }
      if (_bookmark > _index) _bookmark = _index;
    }
    public void MovePrev()
    {
      MovePrev(1);
    }
    public string ReadFromLast()
    {
      var result = new string(_buffer, _bookmark, _index - _bookmark);
      _bookmark = _index;
      return result;
    }

    private void ReloadBuffer()
    {
      if (_index > _bookmark)
      {
        var newBuffer = new char[Math.Max(_bufferSize * 2, _bufferSize + _index - _bookmark)];
        Array.Copy(_buffer, _bookmark, newBuffer, 0, _index - _bookmark);
        _buffer = newBuffer;
        _index -= _bookmark;
        _bookmark = 0;
      }
      else
      {
        _index = 0;
        _bookmark = 0;
      }
      _bufferLength = _reader.ReadBlock(_buffer, _index, _bufferSize);
      _eof = _bufferLength + _index + (_bufferLength < _bufferSize ? 0 : 1);
      _bufferLength += _index;
    }
  }
}
