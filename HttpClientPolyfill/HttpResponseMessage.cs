//
// HttpResponseMessage.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2011 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
  public class HttpResponseMessage : IDisposable
  {
    HttpResponseHeaders headers;
    string reasonPhrase;
    HttpStatusCode statusCode;
    Version version;
    bool disposed;

    public HttpResponseMessage()
      : this(HttpStatusCode.OK)
    {
    }

    public HttpResponseMessage(HttpStatusCode statusCode)
    {
      StatusCode = statusCode;
    }

    public HttpContent Content { get; set; }

    public HttpResponseHeaders Headers
    {
      get
      {
        return headers ?? (headers = new HttpResponseHeaders());
      }
    }

    public bool IsSuccessStatusCode
    {
      get
      {
        // Successful codes are 2xx
        return statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.MultipleChoices;
      }
    }

    public string ReasonPhrase
    {
      get
      {
        return reasonPhrase ?? GetStatusDescription((int)statusCode);
      }
      set
      {
        reasonPhrase = value;
      }
    }

    public HttpRequestMessage RequestMessage { get; set; }

    public HttpStatusCode StatusCode
    {
      get
      {
        return statusCode;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException();

        statusCode = value;
      }
    }

    public Version Version
    {
      get
      {
        return version ?? HttpVersion.Version11;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("Version");

        version = value;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && !disposed)
      {
        disposed = true;

        if (Content != null)
          Content.Dispose();
      }
    }

    public HttpResponseMessage EnsureSuccessStatusCode()
    {
      if (IsSuccessStatusCode)
        return this;

      throw new HttpRequestException(string.Format("{0} ({1})", (int)statusCode, ReasonPhrase));
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("StatusCode: ").Append((int)StatusCode);
      sb.Append(", ReasonPhrase: '").Append(ReasonPhrase ?? "<null>");
      sb.Append("', Version: ").Append(Version);
      sb.Append(", Content: ").Append(Content != null ? Content.ToString() : "<null>");
      sb.Append(", Headers:\r\n{\r\n").Append(Headers);
      if (Content != null)
        sb.Append(Content.Headers);

      sb.Append("}");

      return sb.ToString();
    }

    private static string GetStatusDescription(int code)
    {
      switch (code)
      {
        case 100: return "Continue";
        case 101: return "Switching Protocols";
        case 102: return "Processing";
        case 200: return "OK";
        case 201: return "Created";
        case 202: return "Accepted";
        case 203: return "Non-Authoritative Information";
        case 204: return "No Content";
        case 205: return "Reset Content";
        case 206: return "Partial Content";
        case 207: return "Multi-Status";
        case 300: return "Multiple Choices";
        case 301: return "Moved Permanently";
        case 302: return "Found";
        case 303: return "See Other";
        case 304: return "Not Modified";
        case 305: return "Use Proxy";
        case 307: return "Temporary Redirect";
        case 400: return "Bad Request";
        case 401: return "Unauthorized";
        case 402: return "Payment Required";
        case 403: return "Forbidden";
        case 404: return "Not Found";
        case 405: return "Method Not Allowed";
        case 406: return "Not Acceptable";
        case 407: return "Proxy Authentication Required";
        case 408: return "Request Timeout";
        case 409: return "Conflict";
        case 410: return "Gone";
        case 411: return "Length Required";
        case 412: return "Precondition Failed";
        case 413: return "Request Entity Too Large";
        case 414: return "Request-Uri Too Long";
        case 415: return "Unsupported Media Type";
        case 416: return "Requested Range Not Satisfiable";
        case 417: return "Expectation Failed";
        case 422: return "Unprocessable Entity";
        case 423: return "Locked";
        case 424: return "Failed Dependency";
        case 500: return "Internal Server Error";
        case 501: return "Not Implemented";
        case 502: return "Bad Gateway";
        case 503: return "Service Unavailable";
        case 504: return "Gateway Timeout";
        case 505: return "Http Version Not Supported";
        case 507: return "Insufficient Storage";
      }
      return "";
    }
  }
}
