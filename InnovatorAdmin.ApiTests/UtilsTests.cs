using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using InnovatorAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prop = global::InnovatorAdmin.ApiTests.Properties;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class UtilsTests
  {
    [TestMethod()]
    public void XPathTest()
    {
      var doc = new XmlDocument();
      doc.LoadXml(prop.Resources.MsdnXml);
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
