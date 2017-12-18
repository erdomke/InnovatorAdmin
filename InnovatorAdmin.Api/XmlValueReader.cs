using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class XmlValueReader : TextReader
  {
    private const int AtStart = -1;

    private int _idx = 0;
    private int _length = AtStart;
    private int _readLength = 0;
    private readonly char[] _buffer = new char[4096];
    private readonly XmlReader _reader;

    public int ReadLength { get { return _readLength; } }

    public XmlValueReader(XmlReader reader)
    {
      _reader = reader;
    }

    public override int Peek()
    {
      FillBuffer();
      if (_idx < _length)
        return (int)_buffer[_idx];
      return -1;
    }

    public override int Read()
    {
      FillBuffer();
      if (_idx < _length)
        return (int)_buffer[_idx++];
      return -1;
    }

    public override int Read(char[] buffer, int index, int count)
    {
      FillBuffer(count);

      if (_length == 0)
        return 0;

      var length = Math.Min(count, _length - _idx);
      Array.Copy(_buffer, _idx, buffer, index, length);
      _idx += length;
      return length;
    }

    private void FillBuffer(int preferred = int.MaxValue)
    {
      if (_length == 0 || _idx < _length - 1)
        return;

      _idx = 0;
      _length = _reader.ReadValueChunk(_buffer, 0, Math.Min(preferred, _buffer.Length));
      _readLength += _length;
    }
  }
}
