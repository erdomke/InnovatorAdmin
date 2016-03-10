using Microsoft.VisualStudio.TestTools.UnitTesting;
using InnovatorAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class XmlNodeWriterTests
  {
    public void PerformXmlRoundTrip(string xml)
    {
      var doc = new XmlDocument();

      using (var str = new StringReader(xml))
      using (var reader = XmlReader.Create(str))
      using (var writer = new XmlNodeWriter(doc))
      {
        writer.WriteNode(reader, false);
      }

      var settings = new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "  ",
        OmitXmlDeclaration = true
      };

      using (var reader = new XmlNodeReader(doc))
      using (var str = new StringWriter())
      using (var writer = XmlWriter.Create(str, settings))
      {
        writer.WriteNode(reader, false);
        writer.Flush();
        str.Flush();
        Assert.AreEqual(xml, str.ToString());
      }
    }

    [TestMethod()]
    public void VerifyXmlNodeWriter()
    {
      var xml = @"<Item type=""Permission"" action=""get"" id=""1F3CE59DC3034F08BC92405174976404"" levels=""1"">
  <classification condition=""like"">Something</classification>
  <created_by_id />
  <css><![CDATA[Data that spans lines
with more <> ""' data here]]></css>
  <generation>something &amp; another</generation>
  <Relationships>
    <!-- A comment here-->
    <Item type=""Access"" action=""get"">
      <behavior>fixed</behavior>
    </Item>
    <!-- Another comment here-->
    <Item type=""Access"" action=""get"">
      <can_change_access>0</can_change_access>
    </Item>
  </Relationships>
</Item>";
      PerformXmlRoundTrip(xml);
    }
  }
}
