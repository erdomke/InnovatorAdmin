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
  }
}
