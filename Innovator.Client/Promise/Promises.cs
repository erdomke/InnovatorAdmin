using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if NET4
using System.Threading.Tasks;
#endif

namespace Innovator.Client
{
  public static class Promises
  {
    public static IPromise<T1> All<T1>(IPromise<T1> promise1)
    {
      return promise1;
    }
    public static IPromise<PromiseResult<T1, T2>> All<T1, T2>(IPromise<T1> promise1, IPromise<T2> promise2)
    {
      return new AggregatePromise<T1, T2>(promise1, promise2);
    }
    public static IPromise<PromiseResult<T1, T2, T3>> All<T1, T2, T3>(IPromise<T1> promise1, IPromise<T2> promise2, IPromise<T3> promise3)
    {
      return new AggregatePromise<T1, T2, T3>(promise1, promise2, promise3);
    }
    public static IPromise<PromiseResult<T1, T2, T3, T4>> All<T1, T2, T3, T4>(IPromise<T1> promise1, IPromise<T2> promise2, IPromise<T3> promise3, IPromise<T4> promise4)
    {
      return new AggregatePromise<T1, T2, T3, T4>(promise1, promise2, promise3, promise4);
    }
    public static IPromise<IList<object>> All(params IPromise[] promises)
    {
      return new AggregatePromise(promises);
    }
    /// <summary>
    /// Return a promise that resolves upon completion of all the contained promises.  Only allow
    /// a select number of promises to be active at a given time.
    /// </summary>
    /// <param name="poolSize">Number of promises that can be active simultaneously</param>
    /// <param name="promises">Factory for creating the promises</param>
    public static IPromise<IList<object>> Pooled(int poolSize, params Func<IPromise>[] promises)
    {
      if (poolSize < promises.Length)
        return new PooledPromise(poolSize, promises);
      return new AggregatePromise(promises.Select(p => p.Invoke()).ToArray());
    }
    /// <summary>
    /// Attach a callback that runs when the promise is canceled
    /// </summary>
    public static IPromise<T> Cancelled<T>(this IPromise<T> promise, Action<OperationCanceledException> callback)
    {
      return promise.Fail(ex => {
        var cancel = ex as OperationCanceledException;
        if (cancel != null) callback.Invoke(cancel);
      });
    }
    /// <summary>
    /// Continue a promise chain by acting on the previous promise's result and returning a new promise
    /// </summary>
    /// <param name="promise">Previous promise</param>
    /// <param name="callback">Code to execute on resolution of the promise and returning a new promise</param>
    /// <returns>A new promise</returns>
    public static IPromise<S> Continue<T, S>(this IPromise<T> promise, Func<T, IPromise<S>> callback)
    {
      var result = new Promise<S>();
      result.CancelTarget(
        promise
          .Progress((p, m) => result.Notify(p / 50, m))
          .Done(t =>
          {
            try
            {
              result.CancelTarget(callback(t)
                .Progress((p, m) => result.Notify(50 + p / 50, m))
                .Done(s => result.Resolve(s))
                .Fail(exS => result.Reject(exS)));
            }
            catch (Exception ex)
            {
              result.Reject(ex);
            }
          }).Fail(exT => result.Reject(exT)));
      return result;
    }
    /// <summary>
    /// Convert a promise using a simple transformation to go from one type to another
    /// </summary>
    public static IPromise<S> Convert<T, S>(this IPromise<T> promise, Func<T, S> callback)
    {
      var result = new Promise<S>();
      result.CancelTarget(
        promise
        .Progress((p, m) => result.Notify(p, m))
        .Done(d =>
        {
          try
          {
            result.Resolve(callback.Invoke(d));
          }
          catch (Exception ex)
          {
            result.Reject(ex);
          }
        }).Fail(ex => result.Reject(ex)));
      return result;
    }
    /// <summary>
    /// Convert a promise by modifying the done/fail logic in addition to transforming the type.
    /// </summary>
    public static IPromise<S> Convert<T, S>(this IPromise<T> promise, Action<T, Promise<S>> doneCallback, Action<Exception, Promise<S>> failCallback)
    {
      var result = new Promise<S>();
      result.CancelTarget(
        promise
        .Progress((p, m) => result.Notify(p, m))
        .Done(d =>
        {
          doneCallback.Invoke(d, result);
        }).Fail(ex => failCallback.Invoke(ex, result)));
      return result;
    }
    /// <summary>
    /// Attach a callback that runs an error (other than a cancellation error) occurs
    /// </summary>
    public static IPromise<T> Error<T>(this IPromise<T> promise, Action<Exception> callback)
    {
      return promise.Fail(ex => { if (!(ex is OperationCanceledException)) callback.Invoke(ex); });
    }
    /// <summary>
    /// Attach a callback that runs an error (other than a cancellation error) occurs
    /// </summary>
    public static IPromise<T> Error<TErr, T>(this IPromise<T> promise, Action<TErr> callback) where TErr : Exception
    {
      return promise.Fail(ex => {
        var err = ex as TErr;
        if (err != null) callback.Invoke(err);
      });
    }
    public static IPromise<T> FailOver<T>(this IPromise<T> promise, Func<IPromise<T>> callback)
    {
      var result = new Promise<T>();
      result.CancelTarget(
        promise
          .Progress((p, m) => result.Notify(p / 50, m))
          .Done(t => result.Resolve(t))
          .Fail(exT =>
          {
            try
            {
              result.CancelTarget(
                callback()
                  .Progress((p, m) => result.Notify(50 + p / 50, m))
                  .Done(s => result.Resolve(s))
                  .Fail(exS => result.Reject(exS)));
            }
            catch (Exception ex)
            {
              result.Reject(ex);
            }
          }));
      return result;
    }
    /// <summary>
    /// Create a new promise which is already rejected
    /// </summary>
    public static IPromise<T> Rejected<T>(Exception ex)
    {
      var result = new Promise<T>();
      result.Reject(ex);
      return result;
    }
    /// <summary>
    /// Create a new promise which is already resolved
    /// </summary>
    public static IPromise<T> Resolved<T>(T data)
    {
      var result = new Promise<T>();
      result.Resolve(data);
      return result;
    }
    /// <summary>
    /// Changes the invokation logic of the promise (e.g. to trigger invokation on the UI thread)
    /// </summary>
    /// <returns>The original promise with the new invokation logic</returns>
    public static IPromise<T> WithInvoker<T>(this IPromise<T> promise, Action<Delegate, object[]> invoker)
    {
      var impl = promise as Promise<T>;
      if (impl == null) throw new NotSupportedException();
      impl.Invoker = invoker;
      return impl;
    }

#if NET4
    /// <summary>
    /// Convert a promise to a .Net 4.0 Task
    /// </summary>
    public static Task<T> ToTask<T>(this IPromise<T> promise, CancellationToken ct = default(CancellationToken))
    {
      var tcs = new TaskCompletionSource<T>();
      promise.Done(v =>
      {
        try
        {
          tcs.TrySetResult(v);
        }
        catch (Exception ex)
        {
          tcs.TrySetException(ex);
        }
      }).Fail(ex =>
      {
        var cancel = ex as OperationCanceledException;
        if (cancel == null)
        {
          tcs.TrySetException(ex);
        }
        else
        {
          tcs.TrySetCanceled();
        }
      });

      if (ct != default(CancellationToken))
      {
        ct.Register(() =>
        {
          promise.Cancel();
        });
      }

      return tcs.Task;
    }

    /// <summary>
    /// Convert a .Net 4.0 Task to a promise
    /// </summary>
    public static IPromise<T> ToPromise<T>(this Task<T> task, CancellationTokenSource cts = null)
    {
      var result = new Promise<T>();
      if (cts != null)
        result.CancelTarget(new CancelTarget() { Source = cts });
      task.ContinueWith(t =>
      {
        if (t.IsFaulted)
        {
          Exception ex = t.Exception;
          if (ex != null && ex.InnerException != null)
            ex = ex.InnerException;
          result.Reject(ex);
        }
        else if (!t.IsCanceled)
        {
          result.Resolve(t.Result);
        }
      });
      return result;
    }

    private class CancelTarget : ICancelable
    {
      public CancellationTokenSource Source { get; set; }

      public void Cancel()
      {
        Source.Cancel();
      }
    }
#endif

    /// <summary>
    /// Block the current thread waiting for a promise to complete
    /// </summary>
    /// <param name="promise">The promise to wait for</param>
    /// <returns>The resolved value of the promise</returns>
    public static T Wait<T>(this IPromise<T> promise)
    {
      if (!SpinWait(promise))
      {
        var mre = new ManualResetEvent(false);
        promise.Always(() => { mre.Set(); });
        if (!promise.IsRejected && !promise.IsResolved)
        {
          mre.WaitOne();
        }
      }
      return promise.Value;
    }


    private static bool SpinWait(IPromise promise)
    {
      if (promise.IsRejected || promise.IsResolved) return true;

      int num = PlatformHelper.IsSingleProcessor ? 1 : 10;
      for (int i = 0; i < num; i++)
      {
        if (promise.IsRejected || promise.IsResolved) return true;

        if (i == num / 2)
        {
          Thread.Sleep(0);
        }
        else
        {
          Thread.SpinWait(PlatformHelper.ProcessorCount * (4 << i));
        }
      }
      return promise.IsRejected || promise.IsResolved;
    }

    internal static class PlatformHelper
    {
      private const int PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000;
      private static volatile int s_processorCount;
      private static volatile int s_lastProcessorCountRefreshTicks;
      internal static int ProcessorCount
      {
        get
        {
          int tickCount = Environment.TickCount;
          int num = PlatformHelper.s_processorCount;
          if (num == 0 || tickCount - PlatformHelper.s_lastProcessorCountRefreshTicks >= 30000)
          {
            num = (PlatformHelper.s_processorCount = Environment.ProcessorCount);
            PlatformHelper.s_lastProcessorCountRefreshTicks = tickCount;
          }
          return num;
        }
      }
      internal static bool IsSingleProcessor
      {
        get
        {
          return PlatformHelper.ProcessorCount == 1;
        }
      }
    }

    private abstract class AggregatePromise<T> : Promise<T>
    {
      protected IPromise[] _promises;
      protected bool _isLoading = true;

      public override bool IsRejected
      {
        get { return !_isLoading && _promises.All(p => p.IsRejected); }
      }
      public override bool IsResolved
      {
        get { return !_isLoading && _promises.All(p => p.IsResolved); }
      }
      public override bool IsComplete
      {
        get { return !_isLoading && _promises.All(p => p.IsRejected || p.IsResolved); }
      }

      protected void OnProgress(int progress, string message)
      {
        base.Notify((int)_promises.Average(r => (double)r.PercentComplete), message);
      }
      protected void OnFail(Exception ex)
      {
        base.Reject(ex);
      }

      public override void Cancel()
      {
        base.Cancel();
        foreach (var promise in _promises)
        {
          promise.Cancel();
        }
      }

      protected void BaseResolve(T data)
      {
        base.Resolve(data);
      }

      public override void Resolve(T data)
      {
        throw new NotSupportedException();
      }
    }
    private class AggregatePromise : AggregatePromise<IList<object>>
    {
      public AggregatePromise(params IPromise[] promises)
      {
        var values = new object[promises.Length];
        try
        {
          _promises = promises;

          for (int i = 0; i < promises.Length; i++)
          {
            var j = i;
            promises[i].Progress(OnProgress)
            .Done(o =>
            {
              values[j] = o;
              if (this.IsResolved) base.BaseResolve(values);
            }).Fail(OnFail);
          }
        }
        finally
        {
          _isLoading = false;
        }
        if (this.IsResolved) base.BaseResolve(values);
      }
    }

    private class AggregatePromise<T1, T2> : AggregatePromise<PromiseResult<T1, T2>>
    {
      public AggregatePromise(IPromise<T1> promise1, IPromise<T2> promise2)
      {
        var result = new PromiseResult<T1, T2>();
        try
        {
          _promises = new IPromise[] { promise1, promise2 };

          promise1.Progress(OnProgress).Done(o =>
          {
            result.Result1 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise2.Progress(OnProgress).Done(o =>
          {
            result.Result2 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

        }
        finally
        {
          _isLoading = false;
        }
        if (this.IsResolved) base.BaseResolve(result);
      }
    }

    private class AggregatePromise<T1, T2, T3> : AggregatePromise<PromiseResult<T1, T2, T3>>
    {
      public AggregatePromise(IPromise<T1> promise1, IPromise<T2> promise2, IPromise<T3> promise3)
      {
        var result = new PromiseResult<T1, T2, T3>();
        try
        {
          _promises = new IPromise[] { promise1, promise2, promise3 };

          promise1.Progress(OnProgress).Done(o =>
          {
            result.Result1 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise2.Progress(OnProgress).Done(o =>
          {
            result.Result2 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise3.Progress(OnProgress).Done(o =>
          {
            result.Result3 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);
        }
        finally
        {
          _isLoading = false;
        }
        if (this.IsResolved) base.BaseResolve(result);
      }
    }

    private class AggregatePromise<T1, T2, T3, T4> : AggregatePromise<PromiseResult<T1, T2, T3, T4>>
    {
      public AggregatePromise(IPromise<T1> promise1, IPromise<T2> promise2, IPromise<T3> promise3, IPromise<T4> promise4)
      {
        var result = new PromiseResult<T1, T2, T3, T4>();
        try
        {
          _promises = new IPromise[] { promise1, promise2, promise3, promise4 };

          promise1.Progress(OnProgress).Done(o =>
          {
            result.Result1 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise2.Progress(OnProgress).Done(o =>
          {
            result.Result2 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise3.Progress(OnProgress).Done(o =>
          {
            result.Result3 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);

          promise4.Progress(OnProgress).Done(o =>
          {
            result.Result4 = o;
            if (this.IsResolved) base.BaseResolve(result);
          }).Fail(OnFail);
        }
        finally
        {
          _isLoading = false;
        }
        if (this.IsResolved) base.BaseResolve(result);
      }
    }
  }
}
