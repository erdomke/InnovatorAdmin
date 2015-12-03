using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public interface IProgressReporter
  {
    event EventHandler<ActionCompleteEventArgs> ActionComplete;
    event EventHandler<ProgressChangedEventArgs> ProgressChanged;
  }

  public interface ICancelableProgressReporter : IProgressReporter
  {
    bool Cancel { get; set; }
  }
}
