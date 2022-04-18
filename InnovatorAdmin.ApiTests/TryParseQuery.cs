using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Tests
{
  [TestClass]
  public class TryParseQuery
  {
    [TestMethod]
    public void Query_SimpleUrl()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("http://server/InnovatorServer/?StartItem=Method:162592F32CAA4BDE830A56C500F53E91", out var query));
      Assert.AreEqual("Method", query.Type);
      Assert.AreEqual("162592F32CAA4BDE830A56C500F53E91", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }

    [TestMethod]
    public void Query_CurrentUrl()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("http://server/InnovatorServer/?StartItem=Method:DF8461A4421443879C31CE2D426B5673:current", out var query));
      Assert.AreEqual("Method", query.Type);
      Assert.IsNull(query.Id);
      Assert.AreEqual("DF8461A4421443879C31CE2D426B5673", query.ConfigId);
      Assert.AreEqual(ItemQueryType.Current, query.QueryType);
    }

    [TestMethod]
    public void Query_ReleasedUrl()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("http://server/InnovatorServer/?StartItem=Method:DF8461A4421443879C31CE2D426B5673:released", out var query));
      Assert.AreEqual("Method", query.Type);
      Assert.IsNull(query.Id);
      Assert.AreEqual("DF8461A4421443879C31CE2D426B5673", query.ConfigId);
      Assert.AreEqual(ItemQueryType.Released, query.QueryType);
    }

    [TestMethod]
    public void Query_ItemProperty()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("<related_id type=\"CommandBarButton\">D5B9AF7B219A4F1A8EC56ED2168A6345</related_id>", out var query));
      Assert.AreEqual("CommandBarButton", query.Type);
      Assert.AreEqual("D5B9AF7B219A4F1A8EC56ED2168A6345", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }

    [TestMethod]
    public void Query_ItemTag()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("<Item type=\"CommandBarSection\" id=\"009B59BBA3F04901A70A6CB9C3FAF1D9\">", out var query));
      Assert.AreEqual("CommandBarSection", query.Type);
      Assert.AreEqual("009B59BBA3F04901A70A6CB9C3FAF1D9", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }

    [TestMethod]
    public void Query_PermissionErrorTag()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("<af:permission type=\"can_update\" itemtype=\"ItemType\" id=\"45E899CD2859442982EB22BB2DF683E5\" />", out var query));
      Assert.AreEqual("ItemType", query.Type);
      Assert.AreEqual("45E899CD2859442982EB22BB2DF683E5", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }

    [TestMethod]
    public void Query_ItemErrorTag()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("<af:item type=\"xxx_Repository\" id=\"F093C899A07D4BACAADE61B7CF6266D6\" />", out var query));
      Assert.AreEqual("xxx_Repository", query.Type);
      Assert.AreEqual("F093C899A07D4BACAADE61B7CF6266D6", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }

    [TestMethod]
    public void Query_JavascriptShowItem()
    {
      Assert.IsTrue(ItemQuery.TryParseQuery("javascript:top.aras.uiShowItem('xxx_Data', '14D5342627E14EFC93AC49D626D2ADDB')", out var query));
      Assert.AreEqual("xxx_Data", query.Type);
      Assert.AreEqual("14D5342627E14EFC93AC49D626D2ADDB", query.Id);
      Assert.IsNull(query.ConfigId);
      Assert.AreEqual(ItemQueryType.ById, query.QueryType);
    }
  }
}
