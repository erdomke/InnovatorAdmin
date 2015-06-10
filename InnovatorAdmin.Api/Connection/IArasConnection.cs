using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IArasConnection //: IDisposable
  {
    string CallAction(string action, string input, IProgressCallback progressReporter);
    string CallAction(string action, string input);
    string GetDatabaseName();
    string GetIomVersion();
    string GetUserId();
  }
}
