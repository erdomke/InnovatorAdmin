using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface IHttpRequest
  {
    int ReadWriteTimeout { get; set; }
    int Timeout { get; set; }
    string UserAgent { get; set; }

    void ConfigureForFileUpload();
    void SetContent(Action<IStreamWriter> writer, string contentType = null);
    void SetHeader(string name, string value);
  }
}
