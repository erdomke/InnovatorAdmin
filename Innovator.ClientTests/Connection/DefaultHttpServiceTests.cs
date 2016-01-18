using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Innovator.Client.Connection.Tests
{
  [TestClass()]
  public class DefaultHttpServiceTests
  {
    [TestMethod()]
    public void HttpExecuteTest()
    {
      var service = new DefaultHttpService();
      var result = service.Execute("POST", "http://services.odata.org/v4/(S(34wtn2c0hkuk5ekg0pjr513b))/TripPinServiceRW/People"
        , null, null, true, req =>
      {
        req.Timeout = 3000;
        req.SetHeader("OData-Version", "4.0");
        req.SetHeader("OData-MaxVersion", "4.0");
        req.SetContent(w =>
        {
          w.Write(@"{  
    ""UserName"":""lewisblack"",
    ""FirstName"":""Lewis"",
    ""LastName"":""Black"",
    ""Emails"":[  
        ""lewisblack@example.com""
    ],
    ""AddressInfo"":[  
        {  
            ""Address"":""187 Suffolk Ln."",
            ""City"":{  
                ""CountryRegion"":""United States"",
                ""Name"":""Boise"",
                ""Region"":""ID""
            }
        }
    ],
    ""Gender"":""Male"",
    ""Concurrency"":635519729375200400
}");
          w.Close();
        }, "application/json");
      }).Wait().AsString();
      Assert.IsTrue(result.Length > 650);
    }
  }
}
