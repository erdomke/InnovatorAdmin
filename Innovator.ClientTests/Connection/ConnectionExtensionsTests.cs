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
  public class ConnectionExtensionsTests
  {
    [TestMethod()]
    public void ItemByIdTest()
    {
      var conn = new TestConnection();
      var result = conn.ItemById("Company", "0E086FFA6C4646F6939B74C43D094182", i => new
      {
        FirstName = i.CreatedById().AsItem().Property("first_name").Value,
        PermName = i.PermissionId().AsItem().Property("name").Value,
        KeyedName = i.Property("id").KeyedName().Value,
        Empty = i.OwnedById().Value
      });
      Assert.AreEqual("First", result.FirstName);
      Assert.AreEqual("Company", result.PermName);
      Assert.AreEqual("Another Company", result.KeyedName);
      Assert.AreEqual(null, result.Empty);
    }
  }
}
