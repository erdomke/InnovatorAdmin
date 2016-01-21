using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnovatorAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class ExtensionsTests
  {
    [TestMethod()]
    public void SelectColumnsTest()
    {
      var cols = Extensions.SelectColumns("first, second (thing, another2(id, config_id)), no_paren, third (stuff), another (id)");
      var expected = new string[] { "first", "second", "no_paren", "third", "another" };
      CollectionAssert.AreEqual(expected, cols.ToArray());
    }
  }
}
