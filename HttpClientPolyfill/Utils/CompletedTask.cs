using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientLibrary
{
  using System.Threading.Tasks;

  /// <summary>
  /// Provides static methods to create completed <see cref="Task"/> and <see cref="Task{TResult}"/> instances.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  static class CompletedTask
  {
    /// <summary>
    /// Gets a completed <see cref="Task"/>.
    /// </summary>
    /// <value>A completed <see cref="Task"/>.</value>
    public static Task Default
    {
      get
      {
        return FromResult(default(VoidResult));
      }
    }

    /// <summary>
    /// Gets a completed <see cref="Task{TResult}"/> with the specified result.
    /// </summary>
    /// <typeparam name="TResult">The task result type.</typeparam>
    /// <param name="result">The result of the completed task.</param>
    /// <returns>A completed <see cref="Task{TResult}"/>, whose <see cref="Task{TResult}.Result"/> property returns the specified <paramref name="result"/>.</returns>
    public static Task<TResult> FromResult<TResult>(TResult result)
    {
#if NET45PLUS
            return Task.FromResult(result);
#else
      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
      completionSource.SetResult(result);
      return completionSource.Task;
#endif
    }

    /// <summary>
    /// Gets a canceled <see cref="Task"/>.
    /// </summary>
    /// <returns>A canceled <see cref="Task"/>.</returns>
    public static Task Canceled()
    {
      return Canceled<VoidResult>();
    }

    /// <summary>
    /// Gets a canceled <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The task result type.</typeparam>
    /// <returns>A canceled <see cref="Task{TResult}"/>.</returns>
    public static Task<TResult> Canceled<TResult>()
    {
      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
      completionSource.SetCanceled();
      return completionSource.Task;
    }
  }
}
