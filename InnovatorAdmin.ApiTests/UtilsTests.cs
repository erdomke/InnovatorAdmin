using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class UtilsTests
  {
    [TestMethod()]
    public void XPathTest()
    {
      var doc = new XmlDocument();
      doc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<bookstore>
    <book genre=""autobiography"" publicationdate=""1981-03-22"" ISBN=""1-861003-11-0"">
        <title>The Autobiography of Benjamin Franklin</title>
        <author>
            <first-name>Benjamin</first-name>
            <last-name>Franklin</last-name>
        </author>
        <price>8.99</price>
    </book>
    <book genre=""novel"" publicationdate=""1967-11-17"" ISBN=""0-201-63361-2"">
        <title>The Confidence Man</title>
        <author>
            <first-name>Herman</first-name>
            <last-name>Melville</last-name>
        </author>
        <price>11.99</price>
    </book>
    <book genre=""philosophy"" publicationdate=""1991-02-15"" ISBN=""1-861001-57-6"">
        <title>The Gorgias</title>
        <author>
            <name>Plato</name>
        </author>
        <price>9.99</price>
    </book>
</bookstore>");
      var nodes = doc.XPath("/bookstore/book/price").ToList();
      Assert.AreEqual(3, nodes.Count);
      Assert.AreEqual("8.99", nodes[0].InnerText);
      Assert.AreEqual("11.99", nodes[1].InnerText);
      Assert.AreEqual("9.99", nodes[2].InnerText);

      nodes = doc.XPath("//author[last-name = $p0]", "Franklin").ToList();
      Assert.AreEqual(1, nodes.Count);
      Assert.AreEqual("Benjamin", nodes[0].Element("first-name", ""));
    }

    [TestMethod()]
    public void DependencySortTest()
    {
      var a = new DependencyItem() { Name = "a" };
      var b = new DependencyItem() { Name = "b" };
      var c = new DependencyItem() { Name = "c" };
      var d = new DependencyItem() { Name = "d" };
      var e = new DependencyItem() { Name = "e" };

      a.Dependencies = new DependencyItem[] { b, d };
      d.Dependencies = new DependencyItem[] { b, c };
      b.Dependencies = new DependencyItem[] { e };

      var items = new DependencyItem[] { a, b, c, d, e };
      IList<DependencyItem> cycle = new List<DependencyItem>();
      var sorted = InnovatorAdmin.Utils.DependencySort(items, dep => dep.Dependencies, ref cycle, false).ToList();

      Assert.AreEqual(e, sorted[0]);
      Assert.AreEqual(b, sorted[1]);
      Assert.AreEqual(c, sorted[2]);
      Assert.AreEqual(d, sorted[3]);
      Assert.AreEqual(a, sorted[4]);
      Assert.AreEqual(0, cycle.Count);
    }

    [TestMethod()]
    public void DependencySortCycleTest()
    {
      var a = new DependencyItem() { Name = "a" };
      var b = new DependencyItem() { Name = "b" };
      var c = new DependencyItem() { Name = "c" };
      var d = new DependencyItem() { Name = "d" };
      var e = new DependencyItem() { Name = "e" };

      a.Dependencies = new DependencyItem[] { d };
      d.Dependencies = new DependencyItem[] { b, c };
      b.Dependencies = new DependencyItem[] { a };

      var items = new DependencyItem[] { a, b, c, d, e };
      IList<DependencyItem> cycle = new List<DependencyItem>();
      var sorted = InnovatorAdmin.Utils.DependencySort(items, dep => dep.Dependencies, ref cycle, false).ToList();

      Assert.AreEqual(a, cycle[0]);
      Assert.AreEqual(d, cycle[1]);
      Assert.AreEqual(b, cycle[2]);
      Assert.AreEqual(a, cycle[3]);
    }

    private class DependencyItem
    {
      public string Name { get; set; }
      public DependencyItem[] Dependencies { get; set; }
    }
  }
}
