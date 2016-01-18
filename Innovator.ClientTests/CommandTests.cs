using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Innovator.Client.Tests
{
  [TestClass()]
  public class CommandTests
  {
    [TestMethod()]
    public void ToNormalizedAmlTest()
    {
      var cmd = new Command(@"<Item type='Measurements' action='add'>
                                    <cavity>@0</cavity>
                                    <extradate>@1</extradate>
                                    <measurement>@2</measurement>
                                    <notes>@3</notes>
                                    <opperator>@4</opperator>
                                    <session_id>@5</session_id>
                                    <source>@6</source>
                                    <tag_name>@7</tag_name>
                                  </Item>"
          , "1"
          , new System.DateTime(635802584310000000)
          , 27.878
          , "Nominal = 27.90; USL = 28.00; LSL = 27.80; Average = 27.88133; StdDev = 0.00684"
          , "Matt"
          , "SEC-0000051120"
          , "7DFEF415AB7444BFB9D529665C44D444"
          , "Point 1");
      var factory = ElementFactory.Local;
      var query = cmd.ToNormalizedAml(factory.LocalizationContext);
      Assert.IsTrue(true);
    }
  }
}
