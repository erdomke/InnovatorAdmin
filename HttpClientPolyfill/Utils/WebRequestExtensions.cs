using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientLibrary
{
  using System;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using Stream = System.IO.Stream;

#if PORTABLE && NET45PLUS
    using Microsoft.CSharp.RuntimeBinder;
#endif

  /// <summary>
  /// Provides extension methods for asynchronous operations on
  /// <see cref="WebRequest"/> objects.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  static class WebRequestExtensions
  {
    /// <summary>
    /// Returns a <see cref="Stream"/> for writing data to the Internet resource as an asynchronous operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>This operation will not block. The returned <see cref="Task{TResult}"/> object will complete when the <see cref="Stream"/> for writing data to the Internet resource is available.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
    public static Task<Stream> GetRequestStreamAsync(this WebRequest request)
    {
      if (request == null)
        throw new ArgumentNullException("request");

      return Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
    }

    /// <summary>
    /// Returns a response to an Internet request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// <para>This operation will not block. The returned <see cref="Task{TResult}"/> object will
    /// complete after a response to an Internet request is available.</para>
    /// </remarks>
    /// <param name="request">The request.</param>
    /// <returns>A <see cref="Task"/> object which represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
    public static Task<WebResponse> GetResponseAsync(this WebRequest request)
    {
      if (request == null)
        throw new ArgumentNullException("request");

#if PORTABLE
            return Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
#else
      return GetResponseAsync(request, CancellationToken.None);
#endif
    }

    /// <summary>
    /// Returns a response to an Internet request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// <para>This operation will not block. The returned <see cref="Task{TResult}"/> object will
    /// complete after a response to an Internet request is available.</para>
    /// </remarks>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that will be assigned to the new <see cref="Task"/>.</param>
    /// <returns>A <see cref="Task"/> object which represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="request"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="WebException">
    /// <para>If <see cref="WebRequest.Abort"/> was previously called.</para>
    /// <para>-or-</para>
    /// <para>If the timeout period for the request expired.</para>
    /// <para>-or-</para>
    /// <para>If an error occurred while processing the request.</para>
    /// </exception>
    public static Task<WebResponse> GetResponseAsync(this WebRequest request, CancellationToken cancellationToken)
    {
      return GetResponseAsync(request, true, cancellationToken);
    }

    /// <summary>
    /// Returns a response to an Internet request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// <para>This operation will not block. The returned <see cref="Task{TResult}"/> object will
    /// complete after a response to an Internet request is available.</para>
    /// </remarks>
    /// <param name="request">The request.</param>
    /// <param name="throwOnError"><see langword="true"/> to throw a <see cref="WebException"/> if the <see cref="HttpWebResponse.StatusCode"/> of the response is greater than 400; otherwise, <see langword="false"/> to return the <see cref="WebResponse"/> in the result for these cases.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that will be assigned to the new <see cref="Task"/>.</param>
    /// <returns>A <see cref="Task"/> object which represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="request"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="WebException">
    /// <para>If <see cref="WebRequest.Abort"/> was previously called.</para>
    /// <para>-or-</para>
    /// <para>If the timeout period for the request expired.</para>
    /// <para>-or-</para>
    /// <para>If an error occurred while processing the request.</para>
    /// </exception>
    public static Task<WebResponse> GetResponseAsync(this WebRequest request, bool throwOnError, CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException("request");

#if PORTABLE
            bool timeout = false;

            CancellationTokenRegistration cancellationTokenRegistration;
            if (cancellationToken.CanBeCanceled)
            {
                Action cancellationAction = request.Abort;
                cancellationTokenRegistration = cancellationToken.Register(cancellationAction);
            }
            else
            {
                cancellationTokenRegistration = default(CancellationTokenRegistration);
            }

            CancellationTokenSource noRequestTimeoutTokenSource = new CancellationTokenSource();
            WebExceptionStatus timeoutStatus;
            if (!Enum.TryParse("Timeout", out timeoutStatus))
                timeoutStatus = WebExceptionStatus.UnknownError;

            int requestTimeout;
#if NET45PLUS
            try
            {
                // hack to work around PCL limitation in .NET 4.5
                dynamic dynamicRequest = request;
                requestTimeout = dynamicRequest.Timeout;
            }
            catch (RuntimeBinderException)
            {
                requestTimeout = Timeout.Infinite;
            }
#else
            // hack to work around PCL limitation in .NET 4.0
            var propertyInfo = request.GetType().GetProperty("Timeout", typeof(int));
            if (propertyInfo != null)
            {
                requestTimeout = (int)propertyInfo.GetValue(request, null);
            }
            else
            {
                requestTimeout = Timeout.Infinite;
            }
#endif

            if (requestTimeout >= 0)
            {
                Task timeoutTask = DelayedTask.Delay(TimeSpan.FromMilliseconds(requestTimeout), noRequestTimeoutTokenSource.Token).Select(
                    _ =>
                    {
                        timeout = true;
                        request.Abort();
                    });
            }

            TaskCompletionSource<WebResponse> completionSource = new TaskCompletionSource<WebResponse>();

            AsyncCallback completedCallback =
                result =>
                {
                    try
                    {
                        noRequestTimeoutTokenSource.Cancel();
                        noRequestTimeoutTokenSource.Dispose();
                        cancellationTokenRegistration.Dispose();
                        completionSource.TrySetResult(request.EndGetResponse(result));
                    }
                    catch (WebException ex)
                    {
                        if (timeout)
                            completionSource.TrySetException(new WebException("No response was received during the time-out period for a request.", timeoutStatus));
                        else if (cancellationToken.IsCancellationRequested)
                            completionSource.TrySetCanceled();
                        else if (ex.Response != null && !throwOnError)
                            completionSource.TrySetResult(ex.Response);
                        else
                            completionSource.TrySetException(ex);
                    }
                    catch (Exception ex)
                    {
                        completionSource.TrySetException(ex);
                    }
                };

            IAsyncResult asyncResult = request.BeginGetResponse(completedCallback, null);
            return completionSource.Task;
#else
      bool timeout = false;
      TaskCompletionSource<WebResponse> completionSource = new TaskCompletionSource<WebResponse>();

      RegisteredWaitHandle timerRegisteredWaitHandle = null;
      RegisteredWaitHandle cancellationRegisteredWaitHandle = null;
      AsyncCallback completedCallback =
          result =>
          {
            try
            {
              if (cancellationRegisteredWaitHandle != null)
                cancellationRegisteredWaitHandle.Unregister(null);

              if (timerRegisteredWaitHandle != null)
                timerRegisteredWaitHandle.Unregister(null);

              completionSource.TrySetResult(request.EndGetResponse(result));
            }
            catch (WebException ex)
            {
              if (timeout)
                completionSource.TrySetException(new WebException("No response was received during the time-out period for a request.", WebExceptionStatus.Timeout));
              else if (cancellationToken.IsCancellationRequested)
                completionSource.TrySetCanceled();
              else if (ex.Response != null && !throwOnError)
                completionSource.TrySetResult(ex.Response);
              else
                completionSource.TrySetException(ex);
            }
            catch (Exception ex)
            {
              completionSource.TrySetException(ex);
            }
          };

      IAsyncResult asyncResult = request.BeginGetResponse(completedCallback, null);
      if (!asyncResult.IsCompleted)
      {
        if (request.Timeout != Timeout.Infinite)
        {
          WaitOrTimerCallback timedOutCallback =
              (object state, bool timedOut) =>
              {
                if (timedOut)
                {
                  timeout = true;
                  request.Abort();
                }
              };

          timerRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, timedOutCallback, null, request.Timeout, true);
        }

        if (cancellationToken.CanBeCanceled)
        {
          WaitOrTimerCallback cancelledCallback =
              (object state, bool timedOut) =>
              {
                if (cancellationToken.IsCancellationRequested)
                  request.Abort();
              };

          cancellationRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(cancellationToken.WaitHandle, cancelledCallback, null, Timeout.Infinite, true);
        }
      }

      return completionSource.Task;
#endif
    }
  }
}
