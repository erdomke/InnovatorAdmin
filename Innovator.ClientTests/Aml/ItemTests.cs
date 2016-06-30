using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class ItemTests
  {
    [TestMethod()]
    public void PropertySetWithNullableData()
    {
      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Type("Stuff"), aml.Action("edit"));
      DateTime? someDate = null;
      DateTime? someDate2 = new DateTime(2016, 01, 01);
      item.Property("some_date").Set(someDate);
      item.Property("some_date_2").Set(someDate2);
      Assert.AreEqual("<Item type=\"Stuff\" action=\"edit\"><some_date is_null=\"1\" /><some_date_2>2016-01-01T00:00:00</some_date_2></Item>", item.ToAml());
    }

    [TestMethod()]
    public void UtcDateConversion()
    {
      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Type("stuff"), aml.Property("created_on", "2016-05-24T13:22:42"));
      var localDate = item.CreatedOn().AsDateTime().Value;
      var utcDate = item.CreatedOn().AsDateTimeUtc().Value;
      Assert.AreEqual(DateTime.Parse("2016-05-24T13:22:42"), localDate);
      Assert.AreEqual(DateTime.Parse("2016-05-24T17:22:42"), utcDate);
    }
    [TestMethod()]
    public void PropertyItemExtraction()
    {
      var aml = ElementFactory.Local;
      var result = aml.FromXml("<Item type='thing' id='1234'><item_prop type='another' keyed_name='stuff'>12345ABCDE12345612345ABCDE123456</item_prop></Item>");
      var propItem = result.AssertItem().Property("item_prop").AsItem().ToAml();
      Assert.AreEqual("<Item type=\"another\" id=\"12345ABCDE12345612345ABCDE123456\"><keyed_name>stuff</keyed_name><id keyed_name=\"stuff\" type=\"another\">12345ABCDE12345612345ABCDE123456</id></Item>", propItem);
    }
    [TestMethod()]
    public void AmlFromArray()
    {
      var aml = ElementFactory.Local;
      IEnumerable<object> parts = new object[] { aml.Type("stuff"), aml.Attribute("keyed_name", "thingy"), "12345ABCDE12345612345ABCDE123456" };
      var item = aml.Item(aml.Type("Random Thing"),
        aml.Property("item_ref", parts)
      );
      Assert.AreEqual("<Item type=\"Random Thing\"><item_ref type=\"stuff\" keyed_name=\"thingy\">12345ABCDE12345612345ABCDE123456</item_ref></Item>", item.ToAml());
    }

    [TestMethod()]
    public void CloneTest()
    {
      var itemAml = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
  <SOAP-ENV:Body>
    <Result>
      <Item type=""ItemType"" typeId=""450906E86E304F55A34B3C0D65C097EA"" id=""F0834BBA6FB64394B78DF5BB725532DD"">
        <created_by_id keyed_name=""_Super User"" type=""User"">
          <Item type=""User"" typeId=""45E899CD2859442982EB22BB2DF683E5"" id=""AD30A6D8D3B642F5A2AFED1A4B02BEFA"">
            <id keyed_name=""_Super User"" type=""User"">AD30A6D8D3B642F5A2AFED1A4B02BEFA</id>
            <first_name>_Super</first_name>
            <itemtype>45E899CD2859442982EB22BB2DF683E5</itemtype>
          </Item>
        </created_by_id>
        <id keyed_name=""Report"" type=""ItemType"">F0834BBA6FB64394B78DF5BB725532DD</id>
        <label xml:lang=""en"">Report</label>
      </Item>
    </Result>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
      var item = ElementFactory.Local.FromXml(itemAml).AssertItem();
      var clone = item.Clone();
      var cloneAml = clone.ToAml();
      var expected = @"<Item type=""ItemType"" typeId=""450906E86E304F55A34B3C0D65C097EA"" id=""F0834BBA6FB64394B78DF5BB725532DD""><created_by_id keyed_name=""_Super User"" type=""User""><Item type=""User"" typeId=""45E899CD2859442982EB22BB2DF683E5"" id=""AD30A6D8D3B642F5A2AFED1A4B02BEFA""><id keyed_name=""_Super User"" type=""User"">AD30A6D8D3B642F5A2AFED1A4B02BEFA</id><first_name>_Super</first_name><itemtype>45E899CD2859442982EB22BB2DF683E5</itemtype></Item></created_by_id><id keyed_name=""Report"" type=""ItemType"">F0834BBA6FB64394B78DF5BB725532DD</id><label xml:lang=""en"">Report</label></Item>";
      Assert.AreEqual(expected, cloneAml);
    }
  }
}
