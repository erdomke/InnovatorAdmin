using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientLibrary
{
  using System;
  using System.Threading.Tasks;

#if NET45PLUS
    using System.Reflection;
#endif

  /// <summary>
  /// Provides extension methods for efficiently creating <see cref="Task"/> continuations,
  /// with automatic handling of faulted and canceled antecedent tasks.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  static class CoreTaskExtensions
  {
    /// <summary>
    /// Synchronously execute a continuation when a task completes successfully.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// await task.ConfigureAwait(false);
    /// return continuationFunction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled or faulted, the status of the antecedent is
    /// directly applied to the task returned by this method; it is not wrapped in an additional
    /// <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Then{TResult}(Task, Func{Task, Task{TResult}})"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Select<TResult>(this Task task, Func<Task, TResult> continuationFunction)
    {
      return task.Select(continuationFunction, false);
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// return continuationFunction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
    /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Then{TResult}(Task, Func{Task, Task{TResult}}, bool)"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Select<TResult>(this Task task, Func<Task, TResult> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes successfully.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// TSource source = await task.ConfigureAwait(false);
    /// return continuationFunction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled or faulted, the status of the antecedent is
    /// directly applied to the task returned by this method; it is not wrapped in an additional
    /// <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use <see cref="Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Select<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction)
    {
      return task.Select(continuationFunction, false);
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     TSource source = await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// return continuationFunction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
    /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use <see cref="Then{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}}, bool)"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Select<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes successfully.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// await task.ConfigureAwait(false);
    /// continuationAction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled or faulted, the status of the antecedent is
    /// directly applied to the task returned by this method; it is not wrapped in an additional
    /// <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation actions. For non-trivial continuation actions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Then(Task, Func{Task, Task})"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationAction">The continuation action to execute when <paramref name="task"/> completes successfully.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Select(this Task task, Action<Task> continuationAction)
    {
      return task.Select(continuationAction, false);
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// continuationAction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
    /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation actions. For non-trivial continuation actions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Then(Task, Func{Task, Task}, bool)"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationAction">The continuation action to execute when <paramref name="task"/> completes.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationAction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Select(this Task task, Action<Task> continuationAction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationAction == null)
        throw new ArgumentNullException("continuationAction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationAction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes successfully.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// TSource source = await task.ConfigureAwait(false);
    /// continuationAction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled or faulted, the status of the antecedent is
    /// directly applied to the task returned by this method; it is not wrapped in an additional
    /// <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation actions. For non-trivial continuation actions, use <see cref="Then{TSource}(Task{TSource}, Func{Task{TSource}, Task})"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationAction">The continuation action to execute when <paramref name="task"/> completes successfully.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Select<TSource>(this Task<TSource> task, Action<Task<TSource>> continuationAction)
    {
      return task.Select(continuationAction, false);
    }

    /// <summary>
    /// Synchronously execute a continuation when a task completes. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     TSource source = await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// continuationAction(task);
    /// </code>
    ///
    /// <para>If the antecedent task is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
    /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation actions. For non-trivial continuation actions, use <see cref="Then{TSource}(Task{TSource}, Func{Task{TSource}, Task}, bool)"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationAction">The continuation action to execute when <paramref name="task"/> completes.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationAction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Select<TSource>(this Task<TSource> task, Action<Task<TSource>> continuationAction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationAction == null)
        throw new ArgumentNullException("continuationAction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationAction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the
    /// antecedent task completes successfully, or if the <see cref="Task.Exception"/> it
    /// provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, the status of the antecedent is directly applied to
    /// the task returned by this method. Otherwise, the status of the cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex)
    /// {
    ///     handler(task, ex);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent
    /// tasks when <typeparamref name="TException"/> is assignable from
    /// <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight exception handler methods. For non-trivial exception handlers, use a
    /// <see cref="Task"/> for the exception handling operation and call
    /// <see cref="Catch{TException}(Task, Func{Task, TException, Task})"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by <paramref name="handler"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="handler">The exception handler continuation action to execute when <paramref name="task"/> completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Catch<TException>(this Task task, Action<Task, TException> handler)
        where TException : Exception
    {
      return Catch(task, ex => true, handler);
    }

    /// <summary>
    /// Synchronously execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the
    /// antecedent task completes successfully, or if the <see cref="Task.Exception"/> it
    /// provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, the status of the antecedent is directly applied to
    /// the task returned by this method. Otherwise, the status of the cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex)
    /// {
    ///     return handler(task, ex);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent
    /// tasks when <typeparamref name="TException"/> is assignable from
    /// <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight exception handler methods. For non-trivial exception handlers, use a
    /// <see cref="Task"/> for the exception handling operation and call
    /// <see cref="Catch{TException, TResult}(Task{TResult}, Func{Task{TResult}, TException, Task{TResult}})"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by <paramref name="handler"/>.</typeparam>
    /// <typeparam name="TResult">The result type of the antecedent <paramref name="task"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/> completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="task"/>
    /// if it completed successfully, or the result of <paramref name="handler"/> if it resulted in an error condition
    /// which was handled.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Catch<TException, TResult>(this Task<TResult> task, Func<Task<TResult>, TException, TResult> handler)
        where TException : Exception
    {
      return Catch(task, ex => true, handler);
    }

    /// <summary>
    /// Execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the
    /// antecedent task completes successfully, or if the <see cref="Task.Exception"/> it
    /// provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, the status of the antecedent is directly applied to
    /// the task returned by this method. Otherwise, the status of the cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex)
    /// {
    ///     await handler(task, ex).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent
    /// tasks when <typeparamref name="TException"/> is assignable from
    /// <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="handler"/> function is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="handler"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by <paramref name="handler"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/> completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Catch<TException>(this Task task, Func<Task, TException, Task> handler)
        where TException : Exception
    {
      return Catch(task, ex => true, handler);
    }

    /// <summary>
    /// Execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the
    /// antecedent task completes successfully, or if the <see cref="Task.Exception"/> it
    /// provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, the status of the antecedent is directly applied to
    /// the task returned by this method. Otherwise, the status of the cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex)
    /// {
    ///     return await handler(task, ex).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent
    /// tasks when <typeparamref name="TException"/> is assignable from
    /// <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="handler"/> function is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="handler"/> itself, not to the
    /// <see cref="Task{TResult}"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by <paramref name="handler"/>.</typeparam>
    /// <typeparam name="TResult">The result type of the antecedent <paramref name="task"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/> completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="task"/>
    /// if it completed successfully, or the result of <paramref name="handler"/> if it resulted in an error condition
    /// which was handled.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Catch<TException, TResult>(this Task<TResult> task, Func<Task<TResult>, TException, Task<TResult>> handler)
        where TException : Exception
    {
      return Catch(task, ex => true, handler);
    }

    /// <summary>
    /// Synchronously execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the antecedent task
    /// completes successfully, or if the <see cref="Task.Exception"/> it provides is not a
    /// <typeparamref name="TException"/> wrapped in an <see cref="AggregateException"/>, or if the specified filter
    /// does not match the exception instance, the status of the antecedent is directly applied to the task returned
    /// by this method. Otherwise, the status of the cleanup operation is directly applied to the task returned by
    /// this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex) when (filter(ex))
    /// {
    ///     handler(task, ex);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent tasks when
    /// <typeparamref name="TException"/> is assignable from <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled task is not wrapped
    /// in an additional <see cref="AggregateException"/>.</para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for lightweight
    /// exception handler methods. For non-trivial exception handlers, use a <see cref="Task"/> for the exception
    /// handling operation and call
    /// <see cref="Catch{TException}(Task, Predicate{TException}, Func{Task, TException, Task})"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by
    /// <paramref name="handler"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="filter">A predicate defining the condition under which the exception is handled.</param>
    /// <param name="handler">The exception handler continuation action to execute when <paramref name="task"/>
    /// completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="filter"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <preliminary/>
    public static Task Catch<TException>(this Task task, Predicate<TException> filter, Action<Task, TException> handler)
        where TException : Exception
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (filter == null)
        throw new ArgumentNullException("filter");
      if (handler == null)
        throw new ArgumentNullException("handler");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      bool matchType = false;
      bool match = false;
      Action<Task> handlerWrapper =
          t =>
          {
            TException exception = TryGetException<TException>(t);
            if (exception != null)
            {
              matchType = true;
              if (filter(exception))
              {
                match = true;
                handler(task, exception);
              }
            }
          };

      task
          .ContinueWith(handlerWrapper, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion)
          .ContinueWith(
              t =>
              {
                if (match)
                {
                        // this is the only case where the handler executes
                        completionSource.SetFromTask(t, default(VoidResult));
                }
                else if (matchType && t.Status != TaskStatus.RanToCompletion)
                {
                        // an exception was thrown while evaluating the filter itself
                        completionSource.SetFromFailedTask(t);
                }
                else
                {
                        // otherwise propagate the antecedent
                        completionSource.SetFromTask(task, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute an exception handling continuation when a task completes in the
    /// <see cref="TaskStatus.Canceled"/> or <see cref="TaskStatus.Faulted"/> state. If the antecedent task
    /// completes successfully, or if the <see cref="Task.Exception"/> it provides is not a
    /// <typeparamref name="TException"/> wrapped in an <see cref="AggregateException"/>, or if the specified filter
    /// does not match the exception instance, the status of the antecedent is directly applied to the task returned
    /// by this method. Otherwise, the status of the cleanup operation is directly applied to the task returned by
    /// this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex) when (filter(ex))
    /// {
    ///     return handler(task, ex);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent tasks when
    /// <typeparamref name="TException"/> is assignable from <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled task is not wrapped
    /// in an additional <see cref="AggregateException"/>.</para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for lightweight
    /// exception handler methods. For non-trivial exception handlers, use a <see cref="Task"/> for the exception
    /// handling operation and call
    /// <see cref="Catch{TException, TResult}(Task{TResult}, Predicate{TException}, Func{Task{TResult}, TException, Task{TResult}})"/>
    /// instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by
    /// <paramref name="handler"/>.</typeparam>
    /// <typeparam name="TResult">The result type of the antecedent <paramref name="task"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="filter">A predicate defining the condition under which the exception is handled.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/>
    /// completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the
    /// <paramref name="task"/> if it completed successfully, or the result of <paramref name="handler"/> if it
    /// resulted in an error condition which was handled.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="filter"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <preliminary/>
    public static Task<TResult> Catch<TException, TResult>(this Task<TResult> task, Predicate<TException> filter, Func<Task<TResult>, TException, TResult> handler)
        where TException : Exception
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (filter == null)
        throw new ArgumentNullException("filter");
      if (handler == null)
        throw new ArgumentNullException("handler");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      bool matchType = false;
      bool match = false;
      Func<Task<TResult>, TResult> handlerWrapper =
          t =>
          {
            TException exception = TryGetException<TException>(t);
            if (exception != null)
            {
              matchType = true;
              if (filter(exception))
              {
                match = true;
                return handler(task, exception);
              }
            }

            return default(TResult);
          };

      task
          .ContinueWith(handlerWrapper, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion)
          .ContinueWith(
              t =>
              {
                if (match)
                {
                        // this is the only case where the handler executes
                        completionSource.SetFromTask(t);
                }
                else if (matchType && t.Status != TaskStatus.RanToCompletion)
                {
                        // an exception was thrown while evaluating the filter itself
                        completionSource.SetFromFailedTask(t);
                }
                else
                {
                        // otherwise propagate the antecedent
                        completionSource.SetFromTask(task);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute an exception handling continuation when a task completes in the <see cref="TaskStatus.Canceled"/> or
    /// <see cref="TaskStatus.Faulted"/> state. If the antecedent task completes successfully, or if the
    /// <see cref="Task.Exception"/> it provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, or if the specified filter does not match the exception instance, the
    /// status of the antecedent is directly applied to the task returned by this method. Otherwise, the status of
    /// the cleanup operation is directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex) when (filter(ex))
    /// {
    ///     await handler(task, ex).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent tasks when
    /// <typeparamref name="TException"/> is assignable from <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled task is not wrapped
    /// in an additional <see cref="AggregateException"/>.</para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="handler"/> function is executed synchronously, this method should only be
    /// used for lightweight continuation functions. This restriction applies only to <paramref name="handler"/>
    /// itself, not to the <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by
    /// <paramref name="handler"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="filter">A predicate defining the condition under which the exception is handled.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/>
    /// completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="filter"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <preliminary/>
    public static Task Catch<TException>(this Task task, Predicate<TException> filter, Func<Task, TException, Task> handler)
        where TException : Exception
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (filter == null)
        throw new ArgumentNullException("filter");
      if (handler == null)
        throw new ArgumentNullException("handler");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      bool matchType = false;
      bool match = false;
      Func<Task, Task> handlerWrapper =
          t =>
          {
            TException exception = TryGetException<TException>(t);
            if (exception != null)
            {
              matchType = true;
              if (filter(exception))
              {
                match = true;
                return handler(task, exception);
              }
            }

            return CompletedTask.Default;
          };

      task
          .ContinueWith(handlerWrapper, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (match)
                {
                        // this is the only case where the handler executes
                        completionSource.SetFromTask(t, default(VoidResult));
                }
                else if (matchType && t.Status != TaskStatus.RanToCompletion)
                {
                        // an exception was thrown while evaluating the filter itself
                        completionSource.SetFromFailedTask(t);
                }
                else
                {
                        // otherwise propagate the antecedent
                        completionSource.SetFromTask(task, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute an exception handling continuation when a task completes in the <see cref="TaskStatus.Canceled"/> or
    /// <see cref="TaskStatus.Faulted"/> state. If the antecedent task completes successfully, or if the
    /// <see cref="Task.Exception"/> it provides is not a <typeparamref name="TException"/> wrapped in an
    /// <see cref="AggregateException"/>, or if the specified filter does not match the exception instance, the
    /// status of the antecedent is directly applied to the task returned by this method. Otherwise, the status of
    /// the cleanup operation is directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// catch (TException ex) when (filter(ex))
    /// {
    ///     return await handler(task, ex).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method is capable of handling <see cref="TaskStatus.Canceled"/> antecedent tasks when
    /// <typeparamref name="TException"/> is assignable from <see cref="TaskCanceledException"/>.</para>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled task is not wrapped
    /// in an additional <see cref="AggregateException"/>.</para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="handler"/> function is executed synchronously, this method should only be
    /// used for lightweight continuation functions. This restriction applies only to <paramref name="handler"/>
    /// itself, not to the <see cref="Task{TResult}"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TException">The type of exception which is handled by
    /// <paramref name="handler"/>.</typeparam>
    /// <typeparam name="TResult">The result type of the antecedent <paramref name="task"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="filter">A predicate defining the condition under which the exception is handled.</param>
    /// <param name="handler">The exception handler continuation function to execute when <paramref name="task"/>
    /// completes with an exception of type <typeparamref name="TException"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the
    /// <paramref name="task"/> if it completed successfully, or the result of <paramref name="handler"/> if it
    /// resulted in an error condition which was handled.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="filter"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="handler"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <preliminary/>
    public static Task<TResult> Catch<TException, TResult>(this Task<TResult> task, Predicate<TException> filter, Func<Task<TResult>, TException, Task<TResult>> handler)
        where TException : Exception
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (filter == null)
        throw new ArgumentNullException("filter");
      if (handler == null)
        throw new ArgumentNullException("handler");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      bool matchType = false;
      bool match = false;
      Func<Task<TResult>, Task<TResult>> handlerWrapper =
          t =>
          {
            TException exception = TryGetException<TException>(t);
            if (exception != null)
            {
              matchType = true;
              if (filter(exception))
              {
                match = true;
                return handler(task, exception);
              }
            }

            return CompletedTask.FromResult(default(TResult));
          };

      task
          .ContinueWith(handlerWrapper, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (match)
                {
                        // this is the only case where the handler executes
                        completionSource.SetFromTask(t);
                }
                else if (matchType && t.Status != TaskStatus.RanToCompletion)
                {
                        // an exception was thrown while evaluating the filter itself
                        completionSource.SetFromFailedTask(t);
                }
                else
                {
                        // otherwise propagate the antecedent
                        completionSource.SetFromTask(task);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute a cleanup continuation when a task completes, regardless of the
    /// final <see cref="Task.Status"/> of the task. If the cleanup action completes
    /// successfully, the status of the antecedent is directly applied to the task returned by
    /// this method. Otherwise, the status of the faulted or canceled cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// finally
    /// {
    ///     cleanupAction(task);
    /// }
    /// </code>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Finally(Task, Func{Task, Task})"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="cleanupAction">The cleanup continuation function to execute when <paramref name="task"/> completes.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="cleanupAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Finally(this Task task, Action<Task> cleanupAction)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (cleanupAction == null)
        throw new ArgumentNullException("cleanupAction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      task
          .ContinueWith(cleanupAction, TaskContinuationOptions.ExecuteSynchronously)
          .ContinueWith(
              t =>
              {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                  completionSource.SetFromTask(task, default(VoidResult));
                }
                else
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromFailedTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Synchronously execute a cleanup continuation when a task completes, regardless of the
    /// final <see cref="Task.Status"/> of the task. If the cleanup action completes
    /// successfully, the status of the antecedent is directly applied to the task returned by
    /// this method. Otherwise, the status of the faulted or canceled cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// finally
    /// {
    ///     cleanupAction(task);
    /// }
    /// </code>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the continuation is executed synchronously, this method should only be used for
    /// lightweight continuation functions. For non-trivial continuation functions, use a
    /// <see cref="Task"/> for the continuation operation and call
    /// <see cref="Finally(Task, Func{Task, Task})"/> instead.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="cleanupAction">The cleanup continuation function to execute when <paramref name="task"/> completes.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task
    /// completes successfully, the <see cref="Task{TResult}.Result"/> property will return
    /// the result of the antecedent <paramref name="task"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="cleanupAction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Finally<TResult>(this Task<TResult> task, Action<Task<TResult>> cleanupAction)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (cleanupAction == null)
        throw new ArgumentNullException("cleanupAction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      task
          .ContinueWith(cleanupAction, TaskContinuationOptions.ExecuteSynchronously)
          .ContinueWith(
              t =>
              {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                  completionSource.SetFromTask(task);
                }
                else
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromFailedTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a cleanup continuation task when a task completes, regardless of the final
    /// <see cref="Task.Status"/> of the antecedent task. If the cleanup action completes
    /// successfully, the status of the antecedent is directly applied to the task returned by
    /// this method. Otherwise, the status of the faulted or canceled cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// finally
    /// {
    ///     await cleanupFunction(task).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="cleanupFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="cleanupFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="cleanupFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task"/> representing the asynchronous cleanup operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="cleanupFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Finally(this Task task, Func<Task, Task> cleanupFunction)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (cleanupFunction == null)
        throw new ArgumentNullException("cleanupFunction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      task
          .ContinueWith(cleanupFunction, TaskContinuationOptions.ExecuteSynchronously)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                  completionSource.SetFromTask(task, default(VoidResult));
                }
                else
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromFailedTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a cleanup continuation task when a task completes, regardless of the final
    /// <see cref="Task.Status"/> of the antecedent task. If the cleanup action completes
    /// successfully, the status of the antecedent is directly applied to the task returned by
    /// this method. Otherwise, the status of the faulted or canceled cleanup operation is
    /// directly applied to the task returned by this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     return await task.ConfigureAwait(false);
    /// }
    /// finally
    /// {
    ///     await cleanupFunction(task).ConfigureAwait(false);
    /// }
    /// </code>
    ///
    /// <para>This method ensures that exception information provided by a faulted or canceled
    /// task is not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="cleanupFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="cleanupFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="cleanupFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task"/> representing the asynchronous cleanup operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task
    /// completes successfully, the <see cref="Task{TResult}.Result"/> property will return
    /// the result of the antecedent <paramref name="task"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="cleanupFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Finally<TResult>(this Task<TResult> task, Func<Task<TResult>, Task> cleanupFunction)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (cleanupFunction == null)
        throw new ArgumentNullException("cleanupFunction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      task
          .ContinueWith(cleanupFunction, TaskContinuationOptions.ExecuteSynchronously)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                  completionSource.SetFromTask(task);
                }
                else
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromFailedTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a continuation task when a task completes successfully. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// await task.ConfigureAwait(false);
    /// return await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled or faulted, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
    /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Then<TResult>(this Task task, Func<Task, Task<TResult>> continuationFunction)
    {
      return task.Then(continuationFunction, false);
    }

    /// <summary>
    /// Execute a continuation task when a task completes. The continuation task is synchronously
    /// created by a continuation function, and then unwrapped to form the result of this method.
    /// The <paramref name="supportsErrors"/> parameter specifies whether the continuation is
    /// executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// return await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
    /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Then<TResult>(this Task task, Func<Task, Task<TResult>> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a continuation task when a task completes successfully. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// TSource source = await task.ConfigureAwait(false);
    /// return await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled or faulted, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
    /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Then<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, Task<TResult>> continuationFunction)
    {
      return task.Then(continuationFunction, false);
    }

    /// <summary>
    /// Execute a continuation task when a task completes. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     TSource source = await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// return await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
    /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
    /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task<TResult> Then<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, Task<TResult>> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t);
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a continuation task when a task completes successfully. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// await task.ConfigureAwait(false);
    /// await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled or faulted, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task"/> which provides the final result of the continuation.</param>
    /// <returns>A <see cref="Task"/> representing the unwrapped asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Then(this Task task, Func<Task, Task> continuationFunction)
    {
      return task.Then(continuationFunction, false);
    }

    /// <summary>
    /// Execute a continuation task when a task completes. The continuation task is synchronously
    /// created by a continuation function, and then unwrapped to form the result of this method.
    /// The <paramref name="supportsErrors"/> parameter specifies whether the continuation is
    /// executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task"/> which provides the final result of the continuation.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the unwrapped asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Then(this Task task, Func<Task, Task> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// Execute a continuation task when a task completes successfully. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of
    /// <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// TSource source = await task.ConfigureAwait(false);
    /// await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled or faulted, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task"/> which provides the final result of the continuation.</param>
    /// <returns>A <see cref="Task"/> representing the unwrapped asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Then<TSource>(this Task<TSource> task, Func<Task<TSource>, Task> continuationFunction)
    {
      return task.Then(continuationFunction, false);
    }

    /// <summary>
    /// Execute a continuation task when a task completes. The continuation
    /// task is synchronously created by a continuation function, and then unwrapped to
    /// form the result of this method. The <paramref name="supportsErrors"/>
    /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
    /// </summary>
    /// <remarks>
    /// <para>This code implements support for the following construct without requiring the use of <see langword="async/await"/>.</para>
    ///
    /// <code language="cs">
    /// try
    /// {
    ///     TSource source = await task.ConfigureAwait(false);
    /// }
    /// catch
    /// {
    ///     if (!supportsErrors)
    ///         throw;
    /// }
    ///
    /// await continuationFunction(task).ConfigureAwait(false);
    /// </code>
    ///
    /// <para>If the antecedent <paramref name="task"/> is canceled, or faulted with <paramref name="supportsErrors"/>
    /// set to <see langword="false"/>, the status
    /// of the antecedent is directly applied to the task returned by this method; it is
    /// not wrapped in an additional <see cref="AggregateException"/>.
    /// </para>
    ///
    /// <note type="caller">
    /// <para>Since the <paramref name="continuationFunction"/> is executed synchronously, this
    /// method should only be used for lightweight continuation functions. This restriction
    /// applies only to <paramref name="continuationFunction"/> itself, not to the
    /// <see cref="Task"/> returned by it.</para>
    /// </note>
    /// </remarks>
    /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes. The continuation function returns a <see cref="Task"/> which provides the final result of the continuation.</param>
    /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task"/> representing the unwrapped asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
    /// <para>-or-</para>
    /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
    /// </exception>
    public static Task Then<TSource>(this Task<TSource> task, Func<Task<TSource>, Task> continuationFunction, bool supportsErrors)
    {
      if (task == null)
        throw new ArgumentNullException("task");
      if (continuationFunction == null)
        throw new ArgumentNullException("continuationFunction");

      TaskCompletionSource<VoidResult> completionSource = new TaskCompletionSource<VoidResult>();

      TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
      task
          .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
          .Unwrap()
          .ContinueWith(
              t =>
              {
                if (task.Status == TaskStatus.RanToCompletion || (supportsErrors && task.Status == TaskStatus.Faulted))
                {
                  IgnoreAntecedentExceptions(task);
                  completionSource.SetFromTask(t, default(VoidResult));
                }
              }, TaskContinuationOptions.ExecuteSynchronously);

      TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
      task
          .ContinueWith(t => completionSource.SetFromFailedTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

      return completionSource.Task;
    }

    /// <summary>
    /// This method ensures the exception for a faulted task is "observed", i.e. the <see cref="Task.Exception"/>
    /// property will be accessed if the task enters the <see cref="TaskStatus.Faulted"/> state.
    /// </summary>
    /// <remarks>
    /// <para>Prior to .NET 4.5, failing to observe the <see cref="Task.Exception"/> property for a faulted task
    /// would terminate the process.</para>
    /// </remarks>
    /// <typeparam name="TTask">The specific type of task.</typeparam>
    /// <param name="task">The antecedent task.</param>
    /// <returns>The input argument <paramref name="task"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
    public static TTask ObserveExceptions<TTask>(this TTask task)
        where TTask : Task
    {
      if (task == null)
        throw new ArgumentNullException("task");

      task.ContinueWith(
          t =>
          {
            Exception ignored = t.Exception;
          }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

      return task;
    }

    /// <summary>
    /// Attempts to gets the first unwrapped exception from a faulted or canceled task
    /// as an instance of <typeparamref name="TException"/>.
    /// </summary>
    /// <typeparam name="TException">The desired exception type.</typeparam>
    /// <param name="task">The completed task.</param>
    /// <returns>
    /// <para>An instance of <typeparamref name="TException"/> if the <paramref name="task"/> is in the
    /// <see cref="TaskStatus.Faulted"/> state and the first exception in
    /// <see cref="AggregateException.InnerExceptions"/> is an instance of
    /// <typeparamref name="TException"/>.</para>
    /// <para>-or-</para>
    /// <para>An instance of <typeparamref name="TException"/> if the <paramref name="task"/> is in
    /// the <see cref="TaskStatus.Canceled"/> state, and a call to <see cref="Task.Wait()"/> results
    /// in an <see cref="AggregateException"/>, and the first unwrapped exception is an instance of
    /// <typeparamref name="TException"/>.</para>
    /// <para>-or-</para>
    /// <para>Otherwise, <see langword="null"/> if the <paramref name="task"/> completed
    /// successfully or the unwrapped exception was not an instance of
    /// <typeparamref name="TException"/>.</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
    private static TException TryGetException<TException>(Task task)
        where TException : Exception
    {
      if (task == null)
        throw new ArgumentNullException("task");

      TException exception = null;
      if (task.Exception != null && task.Exception.InnerExceptions.Count >= 1)
        exception = task.Exception.InnerExceptions[0] as TException;

      if (exception == null && task.Status == TaskStatus.Canceled && CouldHandleCancellation<TException>.Value)
      {
        try
        {
          task.Wait();
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count >= 1)
            exception = ex.InnerExceptions[0] as TException;
        }
      }

      return exception;
    }

    /// <summary>
    /// Observe any exceptions from the specified <paramref name="task"/>, since its result is being ignored due to
    /// another condition or task.
    /// </summary>
    /// <remarks>
    /// <para>This is generally called when a continuation task's faulted or canceled state is about to be
    /// propagated instead of the final state of <paramref name="task"/>. This is similar to the case in C# where an
    /// exception is thrown in an exception block while another exception has already been thrown. The first
    /// exception is silently dropped. In the Task Parallel Library, the equivalent is "observing" the exception
    /// from the antecedent task by accessing its <see cref="Task.Exception"/> property.
    /// </para>
    /// </remarks>
    /// <param name="task">The antecedent task.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
    private static void IgnoreAntecedentExceptions(Task task)
    {
      if (task == null)
        throw new ArgumentNullException("task");

      if (!task.IsFaulted)
        return;

      Exception ignored = task.Exception;
    }

    /// <summary>
    /// This utility class provides efficient access to a value indicating whether
    /// a particular exception type might be able to handle the unwrapped exception
    /// from a <see cref="TaskStatus.Canceled"/> task.
    /// </summary>
    /// <typeparam name="TException">The desired exception type.</typeparam>
    private static class CouldHandleCancellation<TException>
        where TException : Exception
    {
      static CouldHandleCancellation()
      {
#if NET45PLUS
                Value = typeof(TException).GetTypeInfo().IsAssignableFrom(typeof(TaskCanceledException).GetTypeInfo())
                    || typeof(TaskCanceledException).GetTypeInfo().IsAssignableFrom(typeof(TException).GetTypeInfo());
#else
        Value = typeof(TException).IsAssignableFrom(typeof(TaskCanceledException))
            || typeof(TaskCanceledException).IsAssignableFrom(typeof(TException));
#endif
      }

      /// <summary>
      /// Gets a value indicating whether the unwrapped exception from a <see cref="TaskStatus.Canceled"/>
      /// task could be an instance of <typeparamref name="TException"/>.
      /// </summary>
      /// <value>
      /// <para><see langword="true"/> if <typeparamref name="TException"/> is assignable from <see cref="TaskCanceledException"/>.</para>
      /// <para>-or-</para>
      /// <para><see langword="true"/> if <see cref="TaskCanceledException"/> is assignable from <typeparamref name="TException"/>.</para>
      /// <para>-or-</para>
      /// <para>Otherwise, <see langword="false"/>.</para>
      /// </value>
      public static bool Value
      {
        get; private set;
      }
    }
  }
}
