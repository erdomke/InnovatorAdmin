using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class XPathTests
  {
    [TestMethod]
    public void ParseRelationships()
    {
      var tokens = XPathToken.Parse("Relationships/Item[@type = 'File'][@id]").ToList();
      Assert.AreEqual(13, tokens.Count);
      Assert.AreEqual("Relationships", tokens[0].Value);
      Assert.AreEqual("Item", tokens[2].Value);
      Assert.IsTrue(tokens[4].TryGetAxis(out var axis1));
      Assert.AreEqual(XPathAxis.Attribute, axis1);
      Assert.IsTrue(tokens[7].TryGetString(out var str));
      Assert.AreEqual("File", str);
      Assert.IsTrue(tokens[10].TryGetAxis(out var axis2));
      Assert.AreEqual(XPathAxis.Attribute, axis2);
    }

    [TestMethod]
    public void ParseOrOperator()
    {
      var tokens = XPathToken.Parse("Relationships/Item[@type = 'File' or @type = 'Folder']").ToList();
      Assert.AreEqual("Relationships", tokens[0].Value);
      Assert.AreEqual("Item", tokens[2].Value);
      Assert.IsTrue(tokens[4].TryGetAxis(out var axis1));
      Assert.AreEqual(XPathAxis.Attribute, axis1);
      Assert.IsTrue(tokens[7].TryGetString(out var str));
      Assert.AreEqual("File", str);
      Assert.AreEqual(XPathTokenType.Operator, tokens[8].Type);
    }
  }
}
