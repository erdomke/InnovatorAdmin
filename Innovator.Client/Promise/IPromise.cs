using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Represents a promise that a result will be provided at some point in the future
  /// </summary>
  public interface IPromise : ICancelable
  {
    /// <summary>Whether an error occurred causing the promise to be rejected</summary>
    bool IsRejected { get; }
    /// <summary>Whether the promise completed successfully</summary>
    bool IsResolved { get; }
    /// <summary>The progress of the promise represented as an integer from 0 to 100</summary>
    int PercentComplete { get; }
    /// <summary>The result of the promise.  Only valid if <see cref="IPromise.IsResolved"/> is <c>true</c></summary>
    object Value { get; }

    /// <summary>Callback to be executed when the promise completes regardless of whether an error occurred</summary>
    IPromise Always(Action callback);
    /// <summary>Callback to be executed when the promise completes successfully</summary>
    IPromise Done(Action<object> callback);
    /// <summary>Callback to be executed when the promise encounters an error</summary>
    IPromise Fail(Action<Exception> callback);
    /// <summary>Callback to be executed when the reported progress changes</summary>
    IPromise Progress(Action<int, string> callback);
  }
  /// <summary>
  /// Represents a promise that a result of the specified type will be provided at some point in the future
  /// </summary>
  public interface IPromise<T> : IPromise
  {
    /// <summary>The result of the promise.  Only valid if <see cref="IPromise.IsResolved"/> is <c>true</c></summary>
    new T Value { get; }

    /// <summary>Callback to be executed when the promise completes regardless of whether an error occurred</summary>
    new IPromise<T> Always(Action callback);
    /// <summary>Callback to be executed when the promise encounters an error</summary>
    new IPromise<T> Fail(Action<Exception> callback);
    /// <summary>Callback to be executed when the reported progress changes</summary>
    new IPromise<T> Progress(Action<int, string> callback);
    /// <summary>Callback to be executed when the promise completes successfully</summary>
    IPromise<T> Done(Action<T> callback);
  }
}
