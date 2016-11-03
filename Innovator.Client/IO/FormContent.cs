using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  internal class FormContent : MultipartFormDataContent
  {
    private CompressionType _compression;

    public CompressionType Compression
    {
      get { return _compression; }
      set
      {
        _compression = value;
        Headers.ContentEncoding.Clear();
        if (_compression != CompressionType.none)
          Headers.ContentEncoding.Add(_compression.ToString());
      }
    }

    public FormContent() : this(GetBoundaryLine()) { }
    public FormContent(string boundary) : base(boundary)
    {
      var mediaTypeHeaderValue = new MediaTypeHeaderValue("multipart/form-data");
      mediaTypeHeaderValue.Parameters.Add(new NameValueHeaderValue("boundary", boundary));
      base.Headers.ContentType = mediaTypeHeaderValue;
    }

    public void Add(string name, string value)
    {
      var content = new ByteArrayContent(Encoding.UTF8.GetBytes(value));
      content.Headers.Add("Content-Disposition", "form-data; name=\"" + name + "\"");
      base.Add(content);
    }

    protected override bool TryComputeLength(out long length)
    {
      if (Compression == CompressionType.none)
        return base.TryComputeLength(out length);

      length = -1;
      return false;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      Stream compressedStream = null;
      if (Compression == CompressionType.none)
        return base.SerializeToStreamAsync(stream, context);
      else if (Compression == CompressionType.gzip)
        compressedStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
      else if (Compression == CompressionType.deflate)
        compressedStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true);
      else
        throw new NotSupportedException();

      return base.SerializeToStreamAsync(compressedStream, context).ContinueWith(tsk =>
      {
        if (compressedStream != null)
          compressedStream.Dispose();
      });
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
