using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Innovator.Client;

namespace Aras.Innovator.Tests
{
  [TestClass()]
  public class ItemTests
  {
    [TestMethod()]
    public void TestItemCreation_Constructor()
    {
      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Action("get"), aml.Type("Part"),
        aml.Property("is_active_rev", "1"),
        aml.Property("item_number", aml.Attribute("condition", "like"), "905-1954-*")
      );
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><is_active_rev>1</is_active_rev><item_number condition=\"like\">905-1954-*</item_number></Item>", item.ToString());

      Assert.AreEqual("get", item.Action().Value);
      Assert.AreEqual(true, item.ServerEvents().AsBoolean(true));
      Assert.AreEqual(false, item.ServerEvents().Exists);
      Assert.AreEqual(false, item.IsCurrent().AsBoolean().HasValue);
      Assert.AreEqual("905-1954-*", item.Property("item_number").Value);
      Assert.AreEqual(true, item.Property("is_active_rev").AsBoolean().Value);


      item = aml.Item(aml.Attribute("action", "get"),
        aml.Attribute("type", "Part"),
        aml.Property("is_active_rev", "1"),
        aml.Property("item_number", aml.Attribute("condition", "like"), "905-1954-*")
      );
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><is_active_rev>1</is_active_rev><item_number condition=\"like\">905-1954-*</item_number></Item>", item.ToString());
      
      //item = (Item)new Item().Action("get").Type("Part")
      //  .SetProperty("is_active_rev", "1")
      //  .Add(new Property("item_number", new Attribute("condition", "like"), "905-1954-*"));
      //Assert.AreEqual("<Item action=\"get\" type=\"Part\"><is_active_rev>1</is_active_rev><item_number condition=\"like\">905-1954-*</item_number></Item>", (string)item);  
    }


    [TestMethod()]
    public void TestItemCreation_Relationships()
    {
      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Type("Part"), aml.Action("get"),
        aml.Property("item_number", aml.Condition(Condition.Like), "905-1954-*"),
        aml.Relationships(
          aml.Item(aml.Type("Part BOM"), aml.Action("get"))
        )
      );

      Assert.AreEqual("<Item type=\"Part\" action=\"get\"><item_number condition=\"like\">905-1954-*</item_number><Relationships><Item type=\"Part BOM\" action=\"get\" /></Relationships></Item>", item.ToString());
    }

    [TestMethod]
    public void LanguageHandling()
    {
      var aml = new ElementFactory(new ServerContext() { LanguageCode = "fr" });
      var item = aml.FromXml("<Item type='Supplier' action='get' select='name' language='en,fr'><thing>All</thing><name xml:lang='fr'>Dell France</name><i18n:name xml:lang='en' xmlns:i18n='http://www.aras.com/I18N'>Dell Computers</i18n:name></Item>").AssertItem();
      Assert.AreEqual("All", item.Property("thing").Value);
      Assert.AreEqual("All", item.Property("thing", "en").Value);
      Assert.AreEqual("All", item.Property("thing", "fr").Value);
      Assert.AreEqual("Dell France", item.Property("name").Value);
      Assert.AreEqual("Dell Computers", item.Property("name", "en").Value);
      Assert.AreEqual("Dell France", item.Property("name", "fr").Value);

      item.Property("test1").Set("1");
      item.Property("test2", "fr").Set("2");
      item.Property("test3", "fr").Set("3");
      item.Property("test3", "en").Set("3.en");
      Assert.AreEqual("<Item type=\"Supplier\" action=\"get\" select=\"name\" language=\"en,fr\"><thing>All</thing><name xml:lang=\"fr\">Dell France</name><i18n:name xml:lang=\"en\" xmlns:i18n=\"http://www.aras.com/I18N\">Dell Computers</i18n:name><test1>1</test1><test2 xml:lang=\"fr\">2</test2><test3 xml:lang=\"fr\">3</test3><i18n:test3 xml:lang=\"en\" xmlns:i18n=\"http://www.aras.com/I18N\">3.en</i18n:test3></Item>",
        item.ToAml());
    }

    public void Example()
    {
      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Type("Part"), aml.Action("get"),
        aml.Property("item_number", aml.Condition(Condition.Like), "905-1954-*"),
        aml.Relationships(
          aml.Item(aml.Type("Part BOM"), aml.Action("get"))
        )
      );

      var query = new Command(@"<Item type='Part' action='get'>
                                    <item_number condition='like'>@0</item_number>
                                    <Relationships>
                                      <Item type='Part BOM' action='get'> </Item>
                                    </Relationships>
                                  </Item>", "905-1954-*");
      
      //var conn = Factory.DirectConnection("{URL_HERE}");
      // Login



//      var result = conn.GetResult("ApplyItem", 
//        @"<Item type='Process Change Order' action='get' select='state'>
//            <item_number>{0}</item_number>
//          </Item>", "yourNumber");
//      var isValid = result.Items().Any();
//      var state = result.AssertItem().State().Value;

    }

    //[TestMethod()]
    //public void PerfTest()
    //{
    //  var builder = new StringBuilder();
    //  builder.Append("<Result>");
    //  for (var i = 0; i < 5000; i++ )
    //  {
    //    builder.Append("<Item type=\"Part\" id=\"" + i + "\"><item_number>123" + i + "</item_number><name>Test " + i + "</name><description></description><keyed_name>asdfasdfasdfsadf </keyed_name><is_current>1</is_current><permission_id>asdfsadf</permission_id></Item>");
    //  }
    //  builder.Append("</Result>");

    //  var masterDoc = new XmlDocument();
    //  var aml = ElementFactory.Local;
    //  var doc = new XmlDocument(masterDoc.NameTable);
    //  doc.LoadXml(builder.ToString());
    //  var st = Stopwatch.StartNew();
    //  for (var j = 0; j < 100000; j++)
    //  {
    //    var child = doc.DocumentElement.FirstChild.ChildNodes.OfType<XmlNode>().Where(n => n.LocalName == "permission_id");
    //  } 
    //  Debug.Print("Method 1 {0} ms", st.ElapsedMilliseconds);

    //  st.Reset();
    //  masterDoc = new XmlDocument();
    //  var settings = new XmlReaderSettings();
    //  settings.NameTable = masterDoc.NameTable;
    //  st.Start();
    //  for (var j = 0; j < 100000; j++)
    //  {
    //    var child = doc.DocumentElement.FirstChild.SelectNodes("./permission_id");
    //  } 
    //  Debug.Print("Method 2 {0} ms", st.ElapsedMilliseconds);
    //  Debug.Print("hit");
    //}

    //private IEnumerable<XmlElement> GetElements(XmlDocument doc, XmlReaderSettings settings, string xml)
    //{
    //  using (var strReader = new System.IO.StringReader(xml))
    //  {
    //    using (var reader = XmlReader.Create(strReader, settings))
    //    {
    //      reader.Read();
    //      while (!reader.EOF)
    //      {
    //        if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "Item")
    //        {
    //          yield return (XmlElement)doc.ReadNode(reader);
    //        }
    //        else
    //        {
    //          reader.Read();
    //        }
    //      }
    //    }
    //  }
    //}
  }
}
