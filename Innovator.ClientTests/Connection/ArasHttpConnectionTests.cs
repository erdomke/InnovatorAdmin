using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Connection.Tests
{
  [TestClass()]
  public class ArasHttpConnectionTests
  {
    [TestMethod()]
    public async Task ProcessTest()
    {
      try
      {
        var service = new DefaultHttpService();
        var conn = new ArasHttpConnection(service, "http://www.aras.com/subscriberportal/");
        var cmd = new Command("<Item type='User' action='get' id='1234' />");
        cmd.Settings = r => r.Timeout = 1;
        var result = await conn.Process(cmd, true).ToTask();
        Assert.Fail();
      }
      catch (HttpException) { }
      catch (HttpTimeoutException) { }
    }
  }
}
