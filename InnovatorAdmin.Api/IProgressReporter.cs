using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IProgressReporter
  {
    event EventHandler<ActionCompleteEventArgs> ActionComplete;
    event EventHandler<ProgressChangedEventArgs> ProgressChanged;
  }
}
