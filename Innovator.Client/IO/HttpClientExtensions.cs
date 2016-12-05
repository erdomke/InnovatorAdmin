using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Innovator.Client
{
  internal static class HttpClientExtensions
  {
    private static IPromise<T> ToHttpPromise<T>(this Task<T> task, TimeoutSource timeout)
    {
      var result = new Promise<T>();
      result.CancelTarget(timeout);
      task
        .ContinueWith(t =>
        {
          if (t.IsFaulted)
          {
            Exception ex = t.Exception;
            if (ex != null && ex.InnerException != null)
              ex = ex.InnerException;
            result.Reject(ex);
          }
          else if (t.IsCanceled)
          {
            result.Reject(new HttpTimeoutException(string.Format("A response was not received after waiting for {0:m' minutes, 's' seconds'}", TimeSpan.FromMilliseconds(timeout.TimeoutDelay))));
          }
          else
          {
            result.Resolve(t.Result);
          }
        });
      return result;
    }

    public static IPromise<IHttpResponse> PostPromise(this HttpClient service, Uri uri, bool async, HttpRequest req)
    {
      req.RequestUri = uri;
      req.Method = HttpMethod.Post;
      req.Async = async;

#if HTTPSYNC
      if (!async && req.Content is ISyncContent && service is SyncHttpClient)
        return SendSync((SyncHttpClient)service, req);
#endif

      var timeout = new TimeoutSource();
      timeout.CancelAfter(req == null ? HttpRequest.DefaultTimeout : req.Timeout);

      var result = service.SendAsync(req, timeout.Source.Token)
        .ContinueWith((Func<Task<HttpResponseMessage>, Task<IHttpResponse>>)HttpResponse.Create, TaskScheduler.Default)
        .Unwrap()
        .ToHttpPromise(timeout);
      if (!async)
        result.Wait();
      return result;
    }

    public static IPromise<IHttpResponse> GetPromise(this HttpClient service, Uri uri, bool async, HttpRequest req = null)
    {
      if (req == null)
        req = new HttpRequest();
      req.RequestUri = uri;
      req.Method = HttpMethod.Get;
      req.Async = async;

#if HTTPSYNC
      if (!async && service is SyncHttpClient)
        return SendSync((SyncHttpClient)service, req);
#endif

      var timeout = new TimeoutSource();
      timeout.CancelAfter(req == null ? HttpRequest.DefaultTimeout : req.Timeout);
      var respTask = service.SendAsync(req, timeout.Source.Token);

      var result = respTask
        .ContinueWith((Func<Task<HttpResponseMessage>, Task<IHttpResponse>>)HttpResponse.Create, TaskScheduler.Default)
        .Unwrap()
        .ToHttpPromise(timeout);
      if (!async)
        result.Wait();
      return result;
    }

#if HTTPSYNC
    private static IPromise<IHttpResponse> SendSync(SyncHttpClient service, HttpRequest req)
    {
      try
      {
        return Promises.Resolved((IHttpResponse)service.Send(req));
      }
      catch (System.Net.WebException webex)
      {
        switch (webex.Status)
        {
          case System.Net.WebExceptionStatus.RequestCanceled:
          case System.Net.WebExceptionStatus.Timeout:
            return Promises.Rejected<IHttpResponse>(new HttpTimeoutException(string.Format("A response was not received after waiting for {0:m' minutes, 's' seconds'}", TimeSpan.FromMilliseconds(req.Timeout))));
          default:
            return Promises.Rejected<IHttpResponse>(webex);
        }
      }
      catch (Exception ex)
      {
        return Promises.Rejected<IHttpResponse>(ex);
      }
    }
#endif

    private class TimeoutSource : IDisposable, ICancelable
    {
      private CancellationTokenSource _source = new CancellationTokenSource();
      private int _timeoutDelay;

      public CancellationTokenSource Source { get { return _source; } }
      public int TimeoutDelay { get { return _timeoutDelay; } }

      public void CancelAfter(int millisecondsDelay)
      {
        _timeoutDelay = millisecondsDelay;

#if TASKS
        _source.CancelAfter(millisecondsDelay);
#else
        if (_source.IsCancellationRequested)
          return;

        if (_timer == null)
        {
          var timer = new Timer(_timerCallback, this, -1, -1);
          if (Interlocked.CompareExchange<Timer>(ref _timer, timer, null) != null)
          {
            timer.Dispose();
          }
        }
        try
        {
          this._timer.Change(millisecondsDelay, -1);
        }
        catch (ObjectDisposedException) { }
#endif
      }

#if !TASKS
      private Timer _timer;
      private static readonly TimerCallback _timerCallback = new TimerCallback(TimerCallbackLogic);

      private static void TimerCallbackLogic(object obj)
      {
        var source = (TimeoutSource)obj;
        try
        {
          source._source.Cancel();
        }
        catch (ObjectDisposedException) { }
      }
#endif

      public void Dispose()
      {
        if (_source != null)
        {
          _source.Dispose();
          _source = null;
        }
      }

      public void Cancel()
      {
        _source.Cancel();
      }
    }
  }
}
