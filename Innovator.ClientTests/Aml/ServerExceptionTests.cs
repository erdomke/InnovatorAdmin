using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class ServerExceptionTests
  {
    [TestMethod()]
    public void VerifySerialization()
    {
      var factory = ElementFactory.Local;

      var ex = new ServerException("A bad exception");
      var expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());

      ex = factory.NoItemsFoundException("Part", "<query />");
      expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());

      ex = factory.ValidationException("Missing Properties", factory.Item(factory.Type("Method")), "owned_by_id", "managed_by_id");
      expected = ex.ToString();
      ex = SerializeException<ServerException>(ex);
      Assert.AreEqual(expected, ex.ToString());
    }

    [TestMethod()]
    public void ExceptionToAml()
    {
      var aml = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
  <SOAP-ENV:Body>
    <SOAP-ENV:Fault xmlns:af=""http://www.aras.com/InnovatorFault"">
      <faultcode>0</faultcode>
      <faultstring>No items of type SavedSearch found.</faultstring>
      <detail>
        <af:legacy_detail>No items of type SavedSearch found.</af:legacy_detail>
        <af:legacy_faultstring>No items of type 'SavedSearch' found using the criteria:
&lt;Item type=""SavedSearch"" action=""get""&gt;
  &lt;is_email_subscription&gt;1&lt;/is_email_subscription&gt;
  &lt;itname&gt;asdfasdfasdf&lt;/itname&gt;
&lt;/Item&gt;
</af:legacy_faultstring>
        <af:legacy_faultactor>   at System.Environment.GetStackTrace(Exception e, Boolean needFileInfo)</af:legacy_faultactor>
      </detail>
    </SOAP-ENV:Fault>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
      var exAml = ElementFactory.Local.FromXml(aml).Exception.ToAml();
      var expected = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""><SOAP-ENV:Body><SOAP-ENV:Fault xmlns:af=""http://www.aras.com/InnovatorFault""><faultcode>0</faultcode><faultstring>No items of type SavedSearch found.</faultstring><detail><af:legacy_detail>No items of type SavedSearch found.</af:legacy_detail><af:legacy_faultstring>No items of type 'SavedSearch' found using the criteria:
&lt;Item type=""SavedSearch"" action=""get""&gt;
  &lt;is_email_subscription&gt;1&lt;/is_email_subscription&gt;
  &lt;itname&gt;asdfasdfasdf&lt;/itname&gt;
&lt;/Item&gt;
</af:legacy_faultstring><af:legacy_faultactor>   at System.Environment.GetStackTrace(Exception e, Boolean needFileInfo)</af:legacy_faultactor></detail></SOAP-ENV:Fault></SOAP-ENV:Body></SOAP-ENV:Envelope>";
      Assert.AreEqual(expected, exAml);
    }

    private T SerializeException<T>(Exception ex) where T : Exception
    {
      var bf = new BinaryFormatter();
      using (var ms = new MemoryStream())
      {
        // "Save" object state
        bf.Serialize(ms, ex);

        // Re-use the same stream for de-serialization
        ms.Seek(0, 0);

        // Replace the original exception with de-serialized one
        return (T)bf.Deserialize(ms);
      }
    }
  }
}
