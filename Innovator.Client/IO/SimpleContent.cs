using System;
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
  internal class SimpleContent : StreamContent
  {
    private const int CompressedCutoff = 1400;

    private CompressionType _compression;
    private bool _forceCompressionOff;

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

    public SimpleContent(Stream content, string mediaType) : base(content)
    {
      if (!string.IsNullOrEmpty(mediaType))
        Headers.ContentType = new MediaTypeHeaderValue(mediaType);
      _forceCompressionOff = DisableCompression(mediaType);
    }
    public SimpleContent(string content, string mediaType) : base(new MemoryStream(Encoding.UTF8.GetBytes(content)))
    {
      var header = new MediaTypeHeaderValue((mediaType == null) ? "text/plain" : mediaType);
      header.CharSet = Encoding.UTF8.WebName;
      Headers.ContentType = header;
      _forceCompressionOff = DisableCompression(mediaType);
    }

    protected override bool TryComputeLength(out long length)
    {
      if (Compression == CompressionType.none || _forceCompressionOff)
        return base.TryComputeLength(out length);

      length = -1;
      return false;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
      Stream compressedStream = null;
      if (Compression == CompressionType.none || _forceCompressionOff)
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

    /// <summary>
    /// Don't compress content that is already compressed or content that is too small to make
    /// an appreciable difference
    /// </summary>
    private bool DisableCompression(string mediaType)
    {
      switch ((mediaType ?? "").ToLowerInvariant())
      {
        case "application/java-archive":
        case "application/ogg":
        case "application/pdf":
        case "application/vnd.ms-cab-compressed":
        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
        case "application/vnd.openxmlformats-officedocument.presentationml.slideshow":
        case "application/vnd.openxmlformats-officedocument.presentationml.template":
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.template":
        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
        case "application/vnd.openxmlformats-officedocument.wordprocessingml.template":
        case "application/x-7z-compressed":
        case "application/x-apple-diskimage":
        case "application/x-bzip2":
        case "application/x-compress":
        case "application/x-compressed-zip":
        case "application/x-gtar":
        case "application/x-gzip":
        case "application/x-rar-compressed":
        case "application/x-tar":
        case "application/zip":
        case "audio/mpeg":
        case "audio/ogg":
        case "audio/webm":
        case "image/gif":
        case "image/jpeg":
        case "image/pjpeg":
        case "image/png":
        case "image/tiff":
        case "multipart/x-zip":
        case "video/ogg":
        case "video/webm":
          return true;
        default:
          long length;
          return base.TryComputeLength(out length) && length < CompressedCutoff;
      }
    }
  }
}
