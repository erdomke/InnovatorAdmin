using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class SmartCompressionStream : Stream
  {
    private const int CompressedCutoff = 1400;

    private CompressionType _compression;
    private MemoryStream _raw = new MemoryStream();
    private Stream _compressed;
    private MemoryStream _compressedRaw = new MemoryStream();
    private bool _disposed = false;
    private bool _checkedZipSignature = false;

    public MemoryStream BaseStream
    {
      get { return _raw ?? _compressedRaw; }
    }
    public CompressionType Compression
    {
      get { return _raw == null ? _compression : CompressionType.none; }
    }

    public SmartCompressionStream(CompressionType compression)
    {
      _compression = compression;
      switch (compression)
      {
        case CompressionType.deflate:
          _compressed = new DeflateStream(_compressedRaw, CompressionMode.Compress, true);
          break;
        case CompressionType.gzip:
          _compressed = new GZipStream(_compressedRaw, CompressionMode.Compress, true);
          break;
      }
    }
    public SmartCompressionStream(CompressionType compression, string contentType)
      : this(GetCompression(contentType, compression)) { }

    /// <summary>
    /// Returns how to compress content based on the content type.  The goal is to not compress
    /// content that is already compressed.
    /// </summary>
    private static CompressionType GetCompression(string contentType, CompressionType compression)
    {
      switch (contentType.ToLowerInvariant())
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
          return CompressionType.none;
        default:
          return compression;
      }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (_disposed) throw new InvalidOperationException();

      if (_raw != null)
      {
        _raw.Write(buffer, offset, count);

        // Check to see if the file being written in a PKZip file using the signature indicated
        // at https://users.cs.jmu.edu/buchhofp/forensics/formats/pkzip.html.  If so, don't bother
        // compressing the content
        if (!_checkedZipSignature && _raw.Position >= 4)
        {
          _checkedZipSignature = true;
          var origPos = _raw.Position;
          try
          {
            _raw.Position = 0;
            var signature = new byte[4];
            if (_raw.Read(signature, 0, signature.Length) == 4
              && signature[0] == 0x50
              && signature[1] == 0x4b
              && signature[2] == 0x03
              && signature[3] == 0x04)
            {
              _compressed = null;
            }
          }
          finally
          {
            _raw.Position = origPos;
          }
        }
        // If compression is allowed, and the raw content is greater than the content, stop 
        // recording the raw content
        if (_compressed != null && _raw.Position > CompressedCutoff) 
          _raw = null;
      }
      if (_compressed != null) 
        _compressed.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !_disposed)
      {
        if (_compressed != null) _compressed.Dispose();
        _disposed = true;
      }
    }

    #region "Interface"

    public override bool CanRead
    {
      get { return false; }
    }

    public override bool CanSeek
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override long Length
    {
      get { throw new NotSupportedException(); }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }
    #endregion
  }
}
