using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IConnectionFactory
  {
    IArasConnection Login(string url, string database, string username, string password, out string message);
    IEnumerable<string> AvailableDatabases(string url);
  }
}
