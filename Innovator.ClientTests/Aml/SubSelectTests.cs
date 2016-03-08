using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class SubSelectTests
  {
    [TestMethod()]
    public void ParseItemSelect_Complex()
    {
      var cols = SubSelect.FromString("first, second (thing, another2(id, config_id)), no_paren, third (stuff), another (id)");
      var expected = new string[] { "first", "second", "no_paren", "third", "another" };
      CollectionAssert.AreEqual(expected, cols.Select(c => c.Name).ToArray());
    }

    [TestMethod()]
    public void ParseItemSelect_Simple()
    {
      var cols = SubSelect.FromString("config_id,name,is_relationship");
      var expected = new string[] { "config_id", "name", "is_relationship" };
      CollectionAssert.AreEqual(expected, cols.Select(c => c.Name).ToArray());
    }
  }
}
