using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface IPromise : ICancelable
  {
    bool IsRejected { get; }
    bool IsResolved { get; }
    int PercentComplete { get; }
    object Value { get; }

    IPromise Always(Action callback);
    IPromise Done(Action<object> callback);
    IPromise Fail(Action<Exception> callback);
    IPromise Progress(Action<int, string> callback);
  }
  public interface IPromise<T> : IPromise
  {
    new T Value { get; }

    new IPromise<T> Always(Action callback);
    new IPromise<T> Fail(Action<Exception> callback);
    new IPromise<T> Progress(Action<int, string> callback);
    IPromise<T> Done(Action<T> callback);
  }
}
