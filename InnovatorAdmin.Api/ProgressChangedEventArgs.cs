using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class ProgressChangedEventArgs : EventArgs
  {
    private string _message;
    private int _progress;

    public string Message { get { return _message; } }
    public int Progress { get { return _progress; } }

    public ProgressChangedEventArgs(string message, int progress)
    {
      _message = message;
      _progress = progress;
    }
  }
}
