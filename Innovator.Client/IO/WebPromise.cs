using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Innovator.Client
{
  internal class WebPromise : IPromise<WebResponse>
  {
    private Promise<WebResponse> _internal = new Promise<WebResponse>();
    private WebRequest _request;
    
    public int PercentComplete
    {
      get { return _internal.PercentComplete; }
    }

    public WebPromise(WebRequest request)
    {
      _request = request;
    }

    public void Execute(Action<IStreamWriter> requestWriter)
    {
      ExecuteAsync(_request.Core.Method == "GET" ? null : requestWriter);
    }

    /// <summary>
    /// Execute the request asynchronously
    /// </summary>
    /// <param name="requestWriter">Writes the request data to the stream</param>
    /// <remarks>Using the asynchronous pattern is better than threads for I/O bound tasks as documented 
    /// http://stackoverflow.com/questions/4299101/thread-startwebrequest-getresponse-vs-webrequest-begingetresponse and 
    /// http://www.matlus.com/httpwebrequest-asynchronous-programming/ among other places</remarks>
    [System.Diagnostics.DebuggerStepThrough()]
    private void ExecuteAsync(Action<IStreamWriter> requestWriter)
    {
      // Use the async invoke since some synchronouse work is still done at the initial setup of methods
      // such as BeginGetResponse (http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetresponse.aspx)
      Utils.AsyncInvoke(() =>
      {
        try
        {
          if (requestWriter == null)
          {
            if (_request.Core.Method == "POST") _request.Core.ContentLength = 0;
            Utils.AsyncInvoke(ResponseHandler, _request.Core);
          }
          else
          {
            var compStream = new SmartCompressionStream(_request.Compression, _request.Core.ContentType);
            var memWriter = new AsyncStreamWriter(compStream,
              () =>
              {
                try
                {
                  _internal.Notify(5, "Request written to compressed stream");
                  var memStream = compStream.BaseStream;
                  _request.ContentLength = memStream.Position;
                  if (compStream.Compression != CompressionType.none)
                  {
                    _request.Core.Headers.Add("Content-Encoding", compStream.Compression.ToString());
                  }
                  memStream.Position = 0;

                  var asyncResult = _request.Core.BeginGetRequestStream((AsyncCallback)(ar =>
                  {
                    var req = (WebRequest)ar.AsyncState;
                    try
                    {
                      var stream = req.Core.EndGetRequestStream(ar);
                      var writer = new AsyncStreamWriter(stream,
                        () =>
                        {
                          stream.Dispose();
                          memStream.Dispose();
                          _internal.Notify(10, "Request written");
                          Utils.AsyncInvoke(ResponseHandler, req.Core);
                        },
                        ex => _internal.Reject(ex));
                      writer.Write(memStream);
                      writer.Close();
                    }
                    catch (WebException webEx)
                    {
                      if (webEx.Status == WebExceptionStatus.RequestCanceled)
                      {
                        _internal.Cancel();
                      }
                      else
                      {
                        _internal.Reject(new HttpException(webEx, _request.Core));
                      }
                    }
                    catch (Exception ex)
                    {
                      _internal.Reject(ex);
                    }
                  }), _request);

                  // Implement a timeout mechanism for the async call
                  ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, (state, timedOut) =>
                  {
                    if (timedOut)
                    {
                      var timeOutReq = state as WebPromise;
                      if (timeOutReq != null && !_internal.IsComplete)
                      {
                        timeOutReq._internal.Reject(new HttpTimeoutException());
                        timeOutReq._request.Core.Abort();
                      }
                    }
                  }, this, _request.ReadWriteTimeout, true);
                }
                catch (Exception ex)
                {
                  _internal.Reject(ex);
                }
              },
              ex => _internal.Reject(ex));
            requestWriter.Invoke(memWriter);
            memWriter.Close();
          }
        }
        catch (Exception ex)
        {
          _internal.Reject(ex);
        }
      });
    }

    [System.Diagnostics.DebuggerStepThrough()]
    private void ResponseHandler(HttpWebRequest req)
    {
      try
      {
        var asyncResult = req.BeginGetResponse((AsyncCallback)(ar =>
        {
          try
          {
            var request = (WebRequest)ar.AsyncState;
            _internal.Resolve(new WebResponse(_request.Core, (HttpWebResponse)request.Core.EndGetResponse(ar)));
          }
          catch (WebException webEx)
          {
            if (webEx.Status == WebExceptionStatus.RequestCanceled)
            {
              _internal.Cancel();
            }
            else
            {
              _internal.Reject(new HttpException(webEx, _request.Core));
            }
          }
          catch (Exception ex)
          {
            _internal.Reject(ex);
          }
        }), _request);

        // Implement a timeout mechanism for the async call
        ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, (state, timedOut) =>
        {
          if (timedOut)
          {
            var timeOutReq = state as WebPromise;
            if (timeOutReq != null && !_internal.IsComplete)
            {
              timeOutReq._internal.Reject(new HttpTimeoutException());
              timeOutReq._request.Core.Abort();
            }
          }
        }, this, _request.Timeout, true);
      }
      catch (Exception ex)
      {
        _internal.Reject(ex);
      }
    }

    #region IPromise
    public IPromise<WebResponse> Done(Action<WebResponse> callback)
    {
      _internal.Done(callback);
      return this;
    }

    public IPromise<WebResponse> Fail(Action<Exception> callback)
    {
      _internal.Fail(callback);
      return this;
    }

    public IPromise<WebResponse> Always(Action callback)
    {
      _internal.Always(callback);
      return this;
    }

    public IPromise<WebResponse> Progress(Action<int, string> callback)
    {
      _internal.Progress(callback);
      return this;
    }

    public void Resolve(WebResponse data)
    {
      _internal.Resolve(data);
    }

    public void Reject(Exception error)
    {
      _internal.Reject(error);
    }

    public void Notify(int progress, string message)
    {
      _internal.Notify(progress, message);
    }

    public WebResponse Value
    {
      get { return _internal.Value; }
    }

    public bool IsRejected
    {
      get { return _internal.IsRejected; }
    }

    public bool IsResolved
    {
      get { return _internal.IsResolved; }
    }

    public bool IsComplete
    {
      get { return _internal.IsComplete; }
    }

    object IPromise.Value
    {
      get { return _internal.Value; }
    }

    IPromise IPromise.Always(Action callback)
    {
      return _internal.Always(callback);
    }

    IPromise IPromise.Done(Action<object> callback)
    {
      return ((IPromise)_internal).Done(callback);
    }

    IPromise IPromise.Fail(Action<Exception> callback)
    {
      return _internal.Fail(callback);
    }

    IPromise IPromise.Progress(Action<int, string> callback)
    {
      return _internal.Progress(callback);
    }

    #endregion

    public void Cancel()
    {
      if (!_internal.IsComplete)
        _request.Core.Abort();
    }
  }
}
