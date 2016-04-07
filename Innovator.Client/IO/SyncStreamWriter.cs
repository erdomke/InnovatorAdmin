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
    private Promise<bool> _promise;

    public IPromise<bool> Promise { get { return _promise; } }

    public SyncStreamWriter(Stream stream)
    {
      _stream = stream;
      _promise = new Promise<bool>();
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
        _promise.Resolve(true);
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
      value.Dispose();
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
