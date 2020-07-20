using Microsoft.VisualStudio.TestTools.UnitTesting;
using InnovatorAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class XmlUtilsTests
  {
    [TestMethod()]
    public void RemoveCommentsTest()
    {
      var xml = "<Result><Item></Item></Result>";
      var result = XmlUtils.RemoveComments(xml);
      Assert.AreEqual(xml, result);

      result = XmlUtils.RemoveComments("<!--A--><Result><Item><!--B--></Item><!--C--></Result><!--D-->");
      Assert.AreEqual(xml, result);
    }
  }
}
