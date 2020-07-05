using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnovatorAdmin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class TestSerializerTests
  {
    [TestMethod()]
    public void ReadTestSuiteTest()
    {
      var data = @"<!-- Test Suite Info -->
<TestSuite>
  <Init>
    <!-- Parameter initialization -->
    <Param name='thing'>One &amp; Another</Param>
  </Init>
  <Tests>
    <!-- What this test does -->
    <Test name='Create MCO Part'>
      <!-- Hey, I'm a query -->
      <Item type='MCO Part' action='add' id=''>
      </Item>
      <!-- I verify correctness -->
      <AssertMatch match='//Item[1]' removeSysProps='0'>
        <Remove match='created_on' />
        <Remove match='modified_on' />
        <Expected>
          <Item type='MCO Part' id=''>
          </Item>
        </Expected>
      </AssertMatch>
      <AssertMatch match='@name'>
        <Expected>My &lt; Name</Expected>
      </AssertMatch>
      <!-- Store data for the next run -->
      <Param name='mco_id' select='//Item[1]/@id' />
    </Test>
    <Test name='Add MCO Org'>
      <Item type='MCO Part Inventory Org' action='add'>
        <source_id>@mco_id</source_id>
      </Item>
      <AssertMatch match='//Item[1]'>
        <Expected>
          <Item type='MCO Part' id=''>
          </Item>
        </Expected>
      </AssertMatch>
    </Test>
  </Tests>
  <Cleanup>
    <Item type='MCO Part' action='delete' id=''>
    </Item>
  </Cleanup>
</TestSuite>";
      TestSuite suite;
      using (var reader = new StringReader(data))
      using (var writer = new StringWriter())
      {
        suite = TestSerializer.ReadTestSuite(reader);
        suite.Write(writer);
        writer.Flush();
        Assert.AreEqual(data.Substring(39), writer.ToString().Substring(84));
      }
    }

    [TestMethod()]
    public void IsXmlTest()
    {
      Assert.AreEqual(false, TestSerializer.IsXml(null));
      Assert.AreEqual(false, TestSerializer.IsXml(""));
      Assert.AreEqual(false, TestSerializer.IsXml("   "));
      Assert.AreEqual(false, TestSerializer.IsXml("  <!--  "));
      Assert.AreEqual(false, TestSerializer.IsXml("  <!--  asdfasdf -->  "));
      Assert.AreEqual(false, TestSerializer.IsXml("  <!--  asdfasdf --> <a"));
      Assert.AreEqual(true, TestSerializer.IsXml("  <!--  asdfasdf --> <and some=''"));
      Assert.AreEqual(true, TestSerializer.IsXml("  <Item/>  "));
      Assert.AreEqual(false, TestSerializer.IsXml("  asdfasdf "));
    }
  }
}
