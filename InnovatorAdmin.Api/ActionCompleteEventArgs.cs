using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class ActionCompleteEventArgs : EventArgs
  {
    public Exception Exception { get; set; }
  }
}
