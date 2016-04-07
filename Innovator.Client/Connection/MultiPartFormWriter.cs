using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  internal class MultiPartFormWriter
  {
    private bool _async;
    private MemoryStream _content = new MemoryStream();
    private StreamWriter _writer;
    private string _boundaryLine = GetBoundaryLine();
    private long _contentLength = -1;
    private UploadCommand _upload;
    private byte[] _finalBytes;
    private IServerContext _context;

    public string ContentType
    {
      get { return "multipart/form-data; boundary=" + _boundaryLine.Substring(2); }
    }

    public MultiPartFormWriter(bool async, IServerContext context)
    {
      _async = async;
      _context = context;
      _writer = new StreamWriter(_content);
    }

    public void AddFiles(UploadCommand upload)
    {
      _upload = upload;
    }
    public long GetLength()
    {
      if (_contentLength < 0)
      {
        _writer.Flush();
        _contentLength = _content.Length;
        foreach (var file in _upload.Files)
        {
          _contentLength += file.SetHeader(_boundaryLine, _upload, _context) + file.Length + 2;
        }
        _finalBytes = Encoding.UTF8.GetBytes(_boundaryLine + "--\r\n");
        _contentLength += _finalBytes.Length;
      }
      return _contentLength;
    }
    public void WriteToRequest(IStreamWriter writer)
    {
      if (_contentLength < 0) GetLength();
      _content.Position = 0;
      writer.Write(_content);
      foreach (var file in _upload.Files)
      {
        writer.Write(file.Header, 0, file.Header.Length);
        var stream = file.GetStream(_async);
        writer.Write(stream);
        writer.Promise.Always(() => stream.Dispose());
        writer.Write(13, 10);
      }
      writer.Write(_finalBytes, 0, _finalBytes.Length);
    }
    public void WriteFormField(string name, string value)
    {
      _writer.Write(_boundaryLine);
      _writer.Write("\r\n");
      _writer.Write("Content-Disposition: form-data; name=\"");
      _writer.Write(name);
      _writer.Write("\"\r\n\r\n");
      _writer.Write(value);
      _writer.Write("\r\n");
    }

    private const int SessionHashLen = 14;
    private const string Symbols = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
    private static readonly Random _rnd = new Random();
    private static string GetBoundaryLine()
    {
      var sb = new StringBuilder("---------------------------", 27 + SessionHashLen);
      for (int i = 0; i < SessionHashLen; i++)
      {
        sb.Append(Symbols[(int)(_rnd.NextDouble() * (double)(Symbols.Length - 1))]);
      }
      return sb.ToString();
    }
  }
}
