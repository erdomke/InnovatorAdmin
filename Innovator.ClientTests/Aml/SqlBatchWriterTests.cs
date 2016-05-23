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
  public class SqlBatchWriterTests
  {
    [TestMethod()]
    public void SqlBatchWriter_ParamSubs()
    {
      var conn = new TestConnection();
      var sql = new SqlBatchWriter(conn);
      sql.Command("insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (@0, @1, 'Asset', @2);", "Zero", "One", "Two");
      sql.Command("insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (@0, @1, 'Asset', @2);", "2Zero", "2One", "2Two");
      sql.Command("insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (@0, @1, 'Asset', @2);", null, "3One", "3Two");
      Assert.AreEqual(@"<sql>insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (N&apos;Zero&apos;, N&apos;One&apos;, &apos;Asset&apos;, N&apos;Two&apos;);
insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (N&apos;2Zero&apos;, N&apos;2One&apos;, &apos;Asset&apos;, N&apos;2Two&apos;);
insert into innovator._sync_mes_entity_tester (id, item_number, classification, line) values (null, N&apos;3One&apos;, &apos;Asset&apos;, N&apos;3Two&apos;);
</sql>", sql.ToString());
    }
  }
}
