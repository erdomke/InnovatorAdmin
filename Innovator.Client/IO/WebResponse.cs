using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Innovator.Client
{
  internal class WebResponse : IHttpResponse
  {
    private string _requestContentType;
    private HttpWebResponse _response;
    private NameValueDictionary _headers;
    private MemoryStream _content;

    public Stream AsStream
    {
      get { return _content; }
    }
    public string AsString
    {
      get 
      {
        var reader = new StreamReader(_content, GuessDownloadEncoding());
        var result = reader.ReadToEnd();
        _content.Position = 0;
        return result;
      }
    }
    internal HttpWebResponse Core
    {
      get { return _response; }
    }
    public IDictionary<string, string> Headers
    {
      get { return _headers; }
    }
    public HttpStatusCode StatusCode
    {
      get { return _response.StatusCode; }
    }

    public WebResponse(HttpWebRequest request, HttpWebResponse response)
    {
      _requestContentType = request.ContentType;
      _response = response;
      _headers = new NameValueDictionary(response == null 
        ? new System.Collections.Specialized.NameValueCollection() 
        : response.Headers);

      _content = new MemoryStream();
      if (_response != null)
      {
        using (var stream = _response.GetResponseStream())
        {
          stream.CopyTo(_content);
        }
        _content.Position = 0;
        _response.Close();
      }

#if !NET4
      if (request.CookieContainer != null && response.Cookies.Count > 0)
      {
        request.CookieContainer.BugFix_CookieDomain();
      }
#endif
    }

    private Encoding GuessDownloadEncoding()
    {
      try
      {
        string text;
        if ((text = _response.ContentType ?? _requestContentType) == null) return Encoding.UTF8;

        bool flag = false;
        string[] parts = text.ToLower(CultureInfo.InvariantCulture).Split(new char[] { ';', '=', ' ' });

        foreach (var part in parts)
        {
          if (part == "charset")
          {
            flag = true;
          }
          else if (flag)
          {
            return Encoding.GetEncoding(part);
          }
        }
      }
      catch (Exception ex)
      {
        if (ex is System.Threading.ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
        {
          throw;
        }
      }
      return Encoding.UTF8;
    }

    
  }
}
