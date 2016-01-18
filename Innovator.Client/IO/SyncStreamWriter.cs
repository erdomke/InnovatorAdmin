using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  internal class SyncStreamWriter : IStreamWriter
  {
    private Stream _stream;

    public SyncStreamWriter(Stream stream)
    {
      _stream = stream;
    }

    public void Close()
    {
      if (_stream != null)
      {
        if (_stream.ShouldClose()) 
          _stream.Dispose();
        else
          _stream.Flush();
        _stream = null;
      }
    }

    public IStreamWriter Write(params byte[] value)
    {
      Write(value, 0, value.Length);
      return this;
    }

    public IStreamWriter Write(byte[] buffer, int offset, int count)
    {
      _stream.Write(buffer, offset, count);
      return this;
    }

    public IStreamWriter Write(System.IO.Stream value)
    {
      value.CopyTo(_stream);
      return this;
    }

    public IStreamWriter Write(string value)
    {
      var bytes = Encoding.UTF8.GetBytes(value);
      Write(bytes, 0, bytes.Length);
      return this;
    }
  }
}
