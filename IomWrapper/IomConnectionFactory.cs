using Aras.Tools.InnovatorAdmin;
using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IomWrapper
{
  public class IomConnectionFactory : MarshalByRefObject, IConnectionFactory
  {

    public IArasConnection Login(string url, string database, string username, string password, out string message)
    {
      var conn = IomFactory.CreateHttpServerConnection(url, database, username, password);
      var loginResult = conn.Login();
      if (loginResult.isError())
      {
        //get details of error
        var errorStr = loginResult.getErrorString();

        //Interpret message string  - remove header text before : symbol
        var pos = errorStr.IndexOf(':') + 1;
        if (pos > 0) errorStr = errorStr.Substring(pos);

        //If error contains keyword clean up message text
        if (errorStr.Contains("Authentication")) errorStr = Properties.Resources.InvalidCredentials;
        if (errorStr.Contains("Database")) errorStr = Properties.Resources.DatabaseUnavailable;

        message = string.Format(Properties.Resources.LoginFailed, errorStr);
        return null;
      }

      message = Properties.Resources.LoginSuccess;

      return new IomConnection(IomFactory.CreateInnovator(conn));
    }

    public IEnumerable<string> AvailableDatabases(string url)
    {
      return IomFactory.CreateHttpServerConnection(url).GetDatabases();
    }
  }
}
