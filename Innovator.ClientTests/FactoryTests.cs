using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class FactoryTests
  {
    private const string SECURELY_STORED_PASSWORD = "cognition";
    private IRemoteConnection conn;

    [TestInitialize()]
    public void Initialize()
    {
      // Get the connection
      conn = Factory.GetConnection("http://ct.gentex.com/gentexinnovator", "MY_CLIENT_USER_AGENT");

      // Log in
      conn.Login(new ExplicitCredentials("DEV", "catusr", SECURELY_STORED_PASSWORD));
    }

    [TestMethod()]
    public void GetConnectionTest()
    {
      // Perform a query
      var result = conn.Process(new Command("<Item type=\"User\" action=\"get\" select=\"login_name\" id=\"@0\" />", conn.UserId)).AsString();

      // Verify the results
      Assert.AreEqual("<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body><Result><Item type=\"User\" typeId=\"45E899CD2859442982EB22BB2DF683E5\" id=\"2057C8BB950B43B291057BE7421CC1E6\"><id keyed_name=\"_Catia User\" type=\"User\">2057C8BB950B43B291057BE7421CC1E6</id><login_name>catusr</login_name><itemtype>45E899CD2859442982EB22BB2DF683E5</itemtype></Item></Result></SOAP-ENV:Body></SOAP-ENV:Envelope>", result);
    }

    [TestMethod()]
    public void PictureDownloadTest()
    {
      var result = conn.Apply("<Item type='ItemType' action='get' select='open_icon'><name>ItemType</name></Item>");
      var data = result.AssertItem().Property("open_icon").AsFile(conn);
      var img = new Bitmap(data);
      Assert.AreEqual(16, img.Width);
    }
  }
}
