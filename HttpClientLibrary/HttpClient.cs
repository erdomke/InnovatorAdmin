//
// HttpClient.cs
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

using System.Threading;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using HttpClientLibrary;

namespace System.Net.Http
{
  public class HttpClient : HttpMessageInvoker
  {
    static readonly TimeSpan TimeoutDefault = TimeSpan.FromSeconds(100);

    Uri base_address;
    CancellationTokenSource cts;
    bool disposed;
    HttpRequestHeaders headers;
    long buffer_size;
    TimeSpan timeout;

    public HttpClient()
      : this(new HttpClientHandler(), true)
    {
    }

    public HttpClient(HttpMessageHandler handler)
      : this(handler, true)
    {
    }

    public HttpClient(HttpMessageHandler handler, bool disposeHandler)
      : base(handler, disposeHandler)
    {
      buffer_size = int.MaxValue;
      timeout = TimeoutDefault;
      cts = new CancellationTokenSource();
    }

    public Uri BaseAddress
    {
      get
      {
        return base_address;
      }
      set
      {
        base_address = value;
      }
    }

    public HttpRequestHeaders DefaultRequestHeaders
    {
      get
      {
        return headers ?? (headers = new HttpRequestHeaders());
      }
    }

    public long MaxResponseContentBufferSize
    {
      get
      {
        return buffer_size;
      }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException();

        buffer_size = value;
      }
    }

    public TimeSpan Timeout
    {
      get
      {
        return timeout;
      }
      set
      {
        if (value != TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite) && value < TimeSpan.Zero)
          throw new ArgumentOutOfRangeException();

        timeout = value;
      }
    }

    public void CancelPendingRequests()
    {
      // Cancel only any already running requests not any new request after this cancellation
      // NOTE: can't call dispose due to a bug in the .NET 3.5 implementation of CancellationTokenRegistration.Dispose
      var c = Interlocked.Exchange(ref cts, new CancellationTokenSource());
      c.Cancel();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !disposed)
      {
        disposed = true;

        cts.Dispose();
      }

      base.Dispose(disposing);
    }

    public Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
    }

    public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
    }

    public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
    }

    public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
    }

    public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content });
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content });
    }

    public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, cancellationToken);
    }

    public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content });
    }

    public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, cancellationToken);
    }

    public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content });
    }

    public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
    {
      return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, cancellationToken);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
      return SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
    {
      return SendAsync(request, completionOption, CancellationToken.None);
    }

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException("request");

      if (request.SetIsUsed())
        throw new InvalidOperationException("Cannot send the same request message multiple times");

      if (request.RequestUri == null)
      {
        if (base_address == null)
          throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");

        request.RequestUri = base_address;
      }
      else if (!request.RequestUri.IsAbsoluteUri)
      {
        if (base_address == null)
          throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");

        request.RequestUri = new Uri(base_address, request.RequestUri);
      }

      if (headers != null)
      {
        request.Headers.AddHeaders(headers);
      }

      return SendAsyncWorker(request, completionOption, cancellationToken);
    }

    Task<HttpResponseMessage> SendAsyncWorker(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
      var currentSource = this.cts;
      Func<Task<CancellationTokenSource>> acquireResource = () => CompletedTask.FromResult(CancellationTokenSource.CreateLinkedTokenSource(currentSource.Token, cancellationToken));
      Func<Task<CancellationTokenSource>, Task<HttpResponseMessage>> body =
        resourceTask =>
        {
          var lcts = resourceTask.Result;
          lcts.CancelAfter(timeout);

          var task = base.SendAsync(request, lcts.Token);
          if (task == null)
            throw new InvalidOperationException("Handler failed to return a value");

          return task.Then(
            responseTask =>
            {
              var response = responseTask.Result;
              if (response == null)
                throw new InvalidOperationException("Handler failed to return a response");

              //
              // Read the content when default HttpCompletionOption.ResponseContentRead is set
              //
              if (response.Content != null && (completionOption & HttpCompletionOption.ResponseHeadersRead) == 0)
              {
                return response.Content.LoadIntoBufferAsync(MaxResponseContentBufferSize)
                  .Select(_ => response);
              }

              return CompletedTask.FromResult(response);
            })
            .Finally(_ => GC.KeepAlive(currentSource));
        };

      return TaskBlocks.Using(acquireResource, body);
    }

    public Task<byte[]> GetByteArrayAsync(string requestUri)
    {
      Func<Task<HttpResponseMessage>> resource = () => GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
      Func<Task<HttpResponseMessage>, Task<byte[]>> body =
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsByteArrayAsync();
        };

      return TaskBlocks.Using(resource, body);
    }

    public Task<byte[]> GetByteArrayAsync(Uri requestUri)
    {
      Func<Task<HttpResponseMessage>> resource = () => GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
      Func<Task<HttpResponseMessage>, Task<byte[]>> body =
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsByteArrayAsync();
        };

      return TaskBlocks.Using(resource, body);
    }

    public Task<Stream> GetStreamAsync(string requestUri)
    {
      return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).Then(
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsStreamAsync();
        });
    }

    public Task<Stream> GetStreamAsync(Uri requestUri)
    {
      return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead).Then(
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsStreamAsync();
        });
    }

    public Task<string> GetStringAsync(string requestUri)
    {
      Func<Task<HttpResponseMessage>> resource = () => GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
      Func<Task<HttpResponseMessage>, Task<string>> body =
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsStringAsync();
        };

      return TaskBlocks.Using(resource, body);
    }

    public Task<string> GetStringAsync(Uri requestUri)
    {
      Func<Task<HttpResponseMessage>> resource = () => GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
      Func<Task<HttpResponseMessage>, Task<string>> body =
        respTask =>
        {
          var resp = respTask.Result;
          resp.EnsureSuccessStatusCode();
          return resp.Content.ReadAsStringAsync();
        };

      return TaskBlocks.Using(resource, body);
    }
  }
}
