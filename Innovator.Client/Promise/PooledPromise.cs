using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Innovator.Client
{
  internal class PooledPromise : Promise<IList<object>>
  {
    protected Func<IPromise>[] _promiseFactories;
    protected IPromise[] _promises;
    protected int _active = 0;        // The number of active items
    protected int _nextStart = -1;    // The index of the current promise being operated on
    protected bool _isLoading = true;
    protected int _poolSize;          // The number of active promises allowed at a time
    protected object[] values;        // The return values

    public override bool IsRejected
    {
      get { return !_isLoading && _promises.All(p => p != null && p.IsRejected); }
    }
    public override bool IsResolved
    {
      get { return !_isLoading && _promises.All(p => p != null && p.IsResolved); }
    }
    public override bool IsComplete
    {
      get { return !_isLoading && _promises.All(p => p != null && (p.IsRejected || p.IsResolved)); }
    }

    public PooledPromise(int poolSize, Func<IPromise>[] promiseFactories)
    {
      _poolSize = poolSize;
      values = new object[promiseFactories.Length];
      try
      {
        _promiseFactories = promiseFactories;
        _promises = new IPromise[promiseFactories.Length];

        LaunchPromises();
      }
      finally
      {
        _isLoading = false;
      }
      if (this.IsResolved) BaseResolve(values);
    }

    private void LaunchPromises()
    {
      int nextPromise;
      while (TryReserveActive(out nextPromise)
        && nextPromise < _promiseFactories.Length)
      {
        var i = nextPromise; // Capture the variable so the lambda function works properly
        _promises[i] = _promiseFactories[i].Invoke()
          .Progress(OnProgress)
          .Done(o =>
          {
            Interlocked.Decrement(ref _active);
            values[i] = o;
            if (this.IsResolved)
              BaseResolve(values);
            else
              LaunchPromises();
          }).Fail(OnFail);

      }
    }

    protected void OnProgress(int progress, string message)
    {
      base.Notify((int)_promises.Average(r => r == null ? 0.0 : (double)r.PercentComplete), message);
    }
    protected void OnFail(Exception ex)
    {
      base.Reject(ex);
    }

    /// <summary>
    /// If there is availability in the pool, return true as well as the promise that should be
    /// generated.  This should be thread-safe
    /// </summary>
    private bool TryReserveActive(out int promiseIndex)
    {
      promiseIndex = -1;
      var currActive = _active;
      var newActive = currActive + 1;
      while (currActive < _poolSize &&
        currActive != Interlocked.CompareExchange(ref _active, newActive, currActive))
      {
        currActive = _active;
        newActive = currActive + 1;
      }

      if (newActive <= _poolSize)
      {
        promiseIndex = Interlocked.Increment(ref _nextStart);
        return true;
      }
      return false;
    }

    protected void BaseResolve(IList<object> data)
    {
      base.Resolve(data);
    }

    public override void Resolve(IList<object> data)
    {
      throw new NotSupportedException();
    }

  }

}
