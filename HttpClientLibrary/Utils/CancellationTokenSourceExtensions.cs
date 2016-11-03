using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientLibrary
{
  using System;
  using System.Threading;

#if NET40PLUS
    using System.Runtime.CompilerServices;
#else
  using System.Collections.Concurrent;
  using System.Collections.Generic;
#endif

#if NET45PLUS && PORTABLE
    using System.Threading.Tasks;
#endif

  /// <summary>
  /// Provides extension methods for the <see cref="CancellationTokenSource"/> class.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  static class CancellationTokenSourceExtensions
  {
#if NET40PLUS
        /// <summary>
        /// This map prevents <see cref="Timer"/> instances from being garbage collected prior to the
        /// <see cref="CancellationTokenSource"/> with which they are associated.
        /// </summary>
        private static readonly ConditionalWeakTable<CancellationTokenSource, TimerHolder> _timers =
            new ConditionalWeakTable<CancellationTokenSource, TimerHolder>();
#else
    /// <summary>
    /// This set prevents <see cref="Timer"/> instances from being garbage collected prior to the
    /// <see cref="CancellationTokenSource"/> with which they are associated.
    /// </summary>
    private static readonly ConcurrentDictionary<HashedWeakReference<CancellationTokenSource>, Timer> _timers =
        new ConcurrentDictionary<HashedWeakReference<CancellationTokenSource>, Timer>();
#endif

    /// <summary>
    /// Schedules a <see cref="CancellationTokenSource.Cancel()"/> operation on a <see cref="CancellationTokenSource"/>
    /// after the specified time span.
    /// </summary>
    /// <remarks>
    /// <para>If a previous call to this method scheduled a cancellation, the cancellation time is
    /// reset to the new <paramref name="delay"/> value. This method has no effect if the
    /// <see cref="CancellationTokenSource"/> has already been canceled (i.e. the
    /// <see cref="CancellationTokenSource.IsCancellationRequested"/> property returns
    /// <see langword="true"/>.</para>
    /// <para>
    /// In all versions of .NET, requesting cancellation of a <see cref="CancellationTokenSource"/> will
    /// not prevent the instance from becoming eligible for garbage collection prior to the timer expiring.
    /// In .NET 4 and newer, any associated <see cref="T:System.Threading.Timer"/> instance will become eligible for
    /// garbage collection at the same time as the associated <see cref="CancellationTokenSource"/>.
    /// </para>
    /// </remarks>
    /// <param name="cts">The <see cref="CancellationTokenSource"/> to cancel after a delay.</param>
    /// <param name="delay">The time span to wait before canceling the <see cref="CancellationTokenSource"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="cts"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">If <paramref name="cts"/> has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the <see cref="TimeSpan.TotalMilliseconds"/> property of <paramref name="delay"/> is less than -1 or greater than <see cref="int.MaxValue"/>.</exception>
    public static void CancelAfter(this CancellationTokenSource cts, TimeSpan delay)
    {
      if (cts == null)
        throw new ArgumentNullException("cts");

      if (delay.TotalMilliseconds < -1 || delay.TotalMilliseconds > int.MaxValue)
        throw new ArgumentOutOfRangeException("delay");

      if (cts.IsCancellationRequested)
        return;

#if NET40PLUS
            TimerHolder holder = _timers.GetOrCreateValue(cts);
            try
            {
                holder.GetOrCreateTimer(cts).Change(delay, TimeSpan.FromMilliseconds(-1));
            }
            catch (ObjectDisposedException)
            {
            }
#else
      Timer timer = _timers.GetOrAdd(new HashedWeakReference<CancellationTokenSource>(cts), CreateTimer);
      timer.Change(delay, TimeSpan.FromMilliseconds(-1));
#endif
    }

#if !NET40PLUS
    private static Timer CreateTimer(HashedWeakReference<CancellationTokenSource> key)
    {
      CancellationTokenSource cts = key.Target;
      if (cts == null)
        throw new InvalidOperationException();

      TimerState state = new TimerState(cts);
      Timer timer = new Timer(TimeElapsed, state, Timeout.Infinite, Timeout.Infinite);
      state.Timer = timer;
      return timer;
    }
#endif

    private static void TimeElapsed(object state)
    {
      TimerState timerState = (TimerState)state;
#if !NET40PLUS
      foreach (HashedWeakReference<CancellationTokenSource> key in _timers.Keys)
      {
        if (key.IsAlive)
          continue;

        Timer timer;
        if (_timers.TryRemove(key, out timer))
          timer.Dispose();
      }
#endif

      try
      {
        CancellationTokenSource cts = timerState.CancellationTokenSource;
        if (cts != null)
          cts.Cancel();

        timerState.Timer.Dispose();
      }
      catch (ObjectDisposedException)
      {
      }
    }

#if NET40PLUS
        private sealed class TimerHolder
        {
            private readonly object _lock = new object();
            private Timer _timer;

            public Timer GetOrCreateTimer(CancellationTokenSource cts)
            {
                if (cts == null)
                    throw new ArgumentNullException("cts");

                if (_timer != null)
                    return _timer;

                lock (_lock)
                {
                    if (_timer == null)
                    {
                        TimerState state = new TimerState(cts);
                        _timer = new Timer(TimeElapsed, state, Timeout.Infinite, Timeout.Infinite);
                        state.Timer = _timer;
                    }

                    return _timer;
                }
            }
        }
#else
    private class HashedWeakReference : WeakReference
    {
      private readonly int _hashCode;

      public HashedWeakReference(object target)
          : base(target)
      {
        _hashCode = EqualityComparer<object>.Default.GetHashCode(target);
      }

      public override object Target
      {
        get
        {
          return base.Target;
        }

        set
        {
          throw new NotSupportedException();
        }
      }

      public override bool Equals(object obj)
      {
        if (obj == this)
          return true;
        if (obj == null)
          return false;

        HashedWeakReference other = obj as HashedWeakReference;
        if (other == null)
          return false;

        if (GetHashCode() != other.GetHashCode())
          return false;

        return EqualityComparer<object>.Default.Equals(Target, other.Target);
      }

      public override int GetHashCode()
      {
        return _hashCode;
      }
    }

    private class HashedWeakReference<T> : HashedWeakReference
        where T : class
    {
      public HashedWeakReference(T target)
          : base(target)
      {
      }

      public virtual new T Target
      {
        get
        {
          return (T)base.Target;
        }
      }

      public override bool Equals(object obj)
      {
        if (obj == this)
          return true;
        if (obj == null)
          return false;

        HashedWeakReference<T> other = obj as HashedWeakReference<T>;
        if (other == null)
          return false;

        if (GetHashCode() != other.GetHashCode())
          return false;

        return EqualityComparer<T>.Default.Equals(Target, other.Target);
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }
    }
#endif

    private sealed class TimerState
    {
      private readonly WeakReference _cts = new WeakReference(null);

      public TimerState(CancellationTokenSource cancellationTokenSource)
      {
        if (cancellationTokenSource == null)
          throw new ArgumentNullException("cancellationTokenSource");

        CancellationTokenSource = cancellationTokenSource;
      }

      public CancellationTokenSource CancellationTokenSource
      {
        get
        {
          return (CancellationTokenSource)_cts.Target;
        }

        private set
        {
          _cts.Target = value;
        }
      }

      public Timer Timer
      {
        get;
        set;
      }
    }

#if NET45PLUS && PORTABLE
        /// <summary>
        /// Represents the method that handles calls from a <see cref="Timer"/>.
        /// </summary>
        /// <param name="state">An object containing application-specific information relevant to the method invoked by
        /// this delegate, or <see langword="null"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Too many preprocessor directives to make ordering straightforward.")]
        private delegate void TimerCallback(object state);

        /// <summary>
        /// This class implements the Timer functionality required for this class in cases where the PCL does not
        /// provide the standard <see cref="T:System.Threading.Timer"/> class.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Timer : IDisposable
        {
            private readonly object _lock = new object();
            private readonly TimerCallback _callback;
            private readonly object _state;

            private CancellationTokenSource _cancellationTokenSource;
            private Task _task;

            /// <summary>
            /// Initializes a new instance of the <see cref="Timer"/> class, using a 32-bit signed integer to specify
            /// the time interval.
            /// </summary>
            /// <param name="callback">A <see cref="TimerCallback"/> delegate representing a method to be executed.</param>
            /// <param name="state">An object containing information to be used by the <paramref name="callback"/> method, or <see langword="null"/>.</param>
            /// <param name="dueTime">The amount of time to delay before callback is invoked, in milliseconds. Specify <see cref="Timeout.Infinite"/> to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
            /// <param name="period">The time interval between invocations of <paramref name="callback"/>, in milliseconds. Specify <see cref="Timeout.Infinite"/> to disable periodic signaling.</param>
            public Timer(TimerCallback callback, object state, int dueTime, int period)
            {
                if (callback == null)
                    throw new ArgumentNullException("callback");
                if (dueTime < -1)
                    throw new ArgumentOutOfRangeException("dueTime");
                if (period < -1)
                    throw new ArgumentOutOfRangeException("period");

                _callback = callback;
                _state = state;

                Change(dueTime, period);
            }

            /// <summary>
            /// Changes the start time and the interval between method invocations for a timer, using <see cref="TimeSpan"/> values to measure time intervals.
            /// </summary>
            /// <param name="delay">A <see cref="TimeSpan"/> representing the amount of time to delay before invoking the callback method specified when the <see cref="Timer"/> was constructed. Specify negative one (-1) milliseconds to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
            /// <param name="period">The time interval between invocations of the callback method specified when the <see cref="Timer"/> was constructed. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
            /// <returns><see langword="true"/> if the timer was successfully updated; otherwise, <see langword="false"/>.</returns>
            public bool Change(TimeSpan delay, TimeSpan period)
            {
                return Change((int)delay.TotalMilliseconds, (int)period.TotalMilliseconds);
            }

            public bool Change(int delay, int period)
            {
                if (delay < -1)
                    throw new ArgumentOutOfRangeException("delay");
                if (period < -1)
                    throw new ArgumentOutOfRangeException("period");

                lock (_lock)
                {
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource.Dispose();
                    }

                    if (delay >= 0)
                    {
                        _cancellationTokenSource = new CancellationTokenSource();
                        Task task = DelayedTask.Delay(TimeSpan.FromMilliseconds(delay), _cancellationTokenSource.Token).Select(BeginInvokeCallback);
                        if (period > 0)
                        {
                            Func<bool> trueFunction = () => true;
                            Func<Task> delayAndSend = () => DelayedTask.Delay(TimeSpan.FromMilliseconds(period), _cancellationTokenSource.Token).Select(BeginInvokeCallback);
                            task = task.Then(_ => TaskBlocks.While(trueFunction, delayAndSend));
                        }

                        _task = task;
                    }
                }

                return true;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                GC.SuppressFinalize(this);
            }

            private void BeginInvokeCallback(Task task)
            {
                Task.Factory.StartNew(() => _callback(_state));
            }
        }
#endif
  }
}
