using System;
using System.Threading;

namespace InnovatorAdmin
{
  /// <summary>
  /// Application Instance Manager
  /// </summary>
  public static class SingleInstance
  {
    /// <summary>
    /// Creates the single instance.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static bool IsFirstInstance(string name)
    {
      EventWaitHandle eventWaitHandle = null;
      string eventName = string.Format("{0}-{1}", Environment.MachineName, name);

      var isFirstInstance = false;

      try
      {
        // try opening existing wait handle
        eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
      }
      catch
      {
        // got exception = handle wasn't created yet
        isFirstInstance = true;
      }

      if (isFirstInstance)
      {
        // init handle
        eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);

        // register wait handle for this instance (process)
        ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, null, Timeout.Infinite, false);
        eventWaitHandle.Close();
      }

      return isFirstInstance;
    }


    /// <summary>
    /// Wait Or Timer Callback Handler
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="timedOut">if set to <c>true</c> [timed out].</param>
    private static void WaitOrTimerCallback(object state, bool timedOut)
    {
      // Do nothing for now
    }
  }
}
