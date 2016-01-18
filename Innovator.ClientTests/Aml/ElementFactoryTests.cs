using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.Aml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Innovator.Client.Aml.Tests
{
  [TestClass()]
  public class ElementFactoryTests
  {
    [TestMethod()]
    public void FormatAmlTest()
    {
      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name>@0</name><is_current>@1</is_current><date>@2</date></Item>",
          "first & second > third", true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_NamedParam()
    {
      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name>@string</name><is_current>@bool</is_current><date>@dateTime</date></Item>",
          new Connection.DbParams() {
            {"string", "first & second > third"},
            {"bool", true},
            {"dateTime", new DateTime(2015, 1, 1)}
          }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_EmptyXmlElement()
    {
      Assert.AreEqual(@"<Item type=""PCO Task"" action=""add""><source_id>1234</source_id><related_id><Item type=""Task"" action=""add""><date_due_target>2020-01-14T16:38:12</date_due_target><indent>0</indent><is_complete is_null=""1"" /><name>Update the documentation</name><date_start_target>2020-01-13T16:38:12</date_start_target><owned_by_id>F13AF7BC3D7A4084AF67AB7BF938C409</owned_by_id></Item></related_id></Item>", 
        new Command(@"<Item type='PCO Task' action='add'><source_id>@0</source_id><related_id><Item type='Task' action='add'><date_due_target>2020-01-14T16:38:12</date_due_target><indent>0</indent><is_complete is_null='1' /><name>Update the documentation</name><date_start_target>2020-01-13T16:38:12</date_start_target><owned_by_id>F13AF7BC3D7A4084AF67AB7BF938C409</owned_by_id></Item></related_id></Item>"
          , "1234").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_EnumTest()
    {
      Assert.AreEqual("<Item><name condition=\"in\">N'1',N'2',N'3'</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name condition='in'>@0</name><is_current>@1</is_current><date>@2</date></Item>",
          new string[] { "1", "2", "3" }, true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_IdListTest()
    {
      Assert.AreEqual("<Item idlist=\"1,2,3\"><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item idlist='@0'><is_current>@1</is_current><date>@2</date></Item>",
          new string[] { "1", "2", "3" }, true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlParam()
    {
      Assert.AreEqual("<Item><is_current condition=\"in\">(select '@0' \"@0\" from thing where stuff = N'test ''&gt;'' thing')</is_current></Item>",
        new Command("<Item><is_current condition='in'>(select '@0' \"@0\" from thing where stuff = @0)</is_current></Item>",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlOnly()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff = N'test ''>'' thing'",
        new Command("select '@0' \"@0\" from thing where stuff = @0",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlEnumString()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff = N'1,2,3'",
        new Command("select '@0' \"@0\" from thing where stuff = @0",
          new List<string>() { "1", "2", "3" }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlEnumInClause()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff in (N'1',N'2',N'3')",
        new Command("select '@0' \"@0\" from thing where stuff in @0",
          new List<string>() { "1", "2", "3" }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_ApplySql()
    {
      Assert.AreEqual("<sql>select '@0' \"@0\" from thing where stuff = N'test ''&gt;'' thing'</sql>",
        new Command("<sql>select '@0' \"@0\" from thing where stuff = @0</sql>",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_RawSupport()
    {
      Assert.AreEqual("<Item><name>&lt;Item /&gt;</name><is_current><Item /></is_current></Item>",
        new Command("<Item><name>@0</name><is_current>@0!</is_current></Item>",
          "<Item />", true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }
  }
}
