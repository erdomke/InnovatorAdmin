using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Innovator.Client;

namespace InnovatorAdmin
{
  public class RecoverableErrorEventArgs : EventArgs
  {
    private string _message;

    public InstallItem Line { get; }
    public string Message => _message;
    public ServerException Exception { get; }
    public XmlNode NewQuery { get; set; }
    public RecoveryOption RecoveryOption { get; set; }
    /// <summary>
    /// Whether the recovery decision was made automatically by code (as opposed to by the user)
    /// </summary>
    public bool IsAutomatic { get; set; }
    public Dictionary<RecoveryOption, string> Labels { get; } = new Dictionary<RecoveryOption, string>();


    internal RecoverableErrorEventArgs(string message)
    {
      _message = message;
      RecoveryOption = RecoveryOption.Abort;
    }

    internal RecoverableErrorEventArgs(string message, ServerException exception, InstallItem line)
      : this(message ?? exception.Message)
    {
      Exception = exception;
      Line = line;
    }
  }
}
