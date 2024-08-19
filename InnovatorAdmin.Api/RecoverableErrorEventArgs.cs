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
    public InstallItem Line { get; set; }
    public string Message { get; set; }
    public ServerException Exception { get; set; }
    public XmlNode NewQuery { get; set; }
    public RecoveryOption RecoveryOption { get; set; }
    /// <summary>
    /// Whether the recovery decision was made automatically by code (as opposed to by the user)
    /// </summary>
    public bool IsAutomatic { get; set; }


    public RecoverableErrorEventArgs()
    {
      this.RecoveryOption = RecoveryOption.Abort;
    }
  }
}
