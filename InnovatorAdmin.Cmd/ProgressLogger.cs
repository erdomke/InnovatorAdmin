using Microsoft.Extensions.Logging;
using System;

namespace InnovatorAdmin.Cmd
{
  internal class ProgressLogger : IProgress<int>
  {
    private int _lastProgress;
    private DateTime _lastProgressTime;
    private ILogger _logger;

    public ProgressLogger(ILogger logger)
    {
      _logger = logger;
      Reset();
    }

    public void Report(int value)
    {
      Report(value, null);
    }

    public void Report(int value, string message)
    {
      if ((value - _lastProgress) >= 10
        || (DateTime.UtcNow - _lastProgressTime).TotalSeconds >= 5)
      {
        if (string.IsNullOrEmpty(message))
          message = "Processing";
        _logger.LogInformation(message + $" @ {value}%");
        _lastProgress = value;
        _lastProgressTime = DateTime.UtcNow;
      }
    }

    public void Report(object sender, ProgressChangedEventArgs args)
    {
      Report(args.Progress, args.Message);
    }

    public void Reset()
    {
      _lastProgress = 0;
      _lastProgressTime = DateTime.UtcNow.AddHours(-1);
    }
  }
}
