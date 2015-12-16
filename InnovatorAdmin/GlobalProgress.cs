using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class GlobalProgress
  {
    private TaskbarManager prog = TaskbarManager.Instance;

    public GlobalProgress Start()
    {
      try
      {
        prog.SetProgressState(TaskbarProgressBarState.Normal);
      }
      catch (Exception) { }
      return this;
    }
    public GlobalProgress Value(int progress)
    {
      try
      {
        prog.SetProgressValue(Math.Min(Math.Max(0, progress), 100), 100);
      }
      catch (Exception) { }
      return this;
    }
    public GlobalProgress Error()
    {
      try
      {
        prog.SetProgressState(TaskbarProgressBarState.Error);
      }
      catch (Exception) { }
      return this;
    }
    public GlobalProgress Stop()
    {
      try
      {
        prog.SetProgressState(TaskbarProgressBarState.NoProgress);
      }
      catch (Exception) { }
      return this;
    }
    public GlobalProgress Marquee()
    {
      try
      {
        prog.SetProgressState(TaskbarProgressBarState.Indeterminate);
      }
      catch (Exception) { }
      return this;
    }

    private static GlobalProgress _instance = new GlobalProgress();

    public static GlobalProgress Instance { get { return _instance; } }
  }
}
