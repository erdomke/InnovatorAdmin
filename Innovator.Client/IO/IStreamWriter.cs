using System;

namespace Innovator.Client
{
  public interface IStreamWriter
  {
    void Close();
    IStreamWriter Write(params byte[] value);
    IStreamWriter Write(byte[] buffer, int offset, int count);
    IStreamWriter Write(global::System.IO.Stream value);
    IStreamWriter Write(string value);
  }
}
