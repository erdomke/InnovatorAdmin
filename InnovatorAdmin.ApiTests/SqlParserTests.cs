using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnovatorAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prop = global::InnovatorAdmin.ApiTests.Properties;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class SqlParserTests
  {
    [TestMethod()]
    public void FindSqlServerObjectNamesTest()
    {
      var correct = new string[] {
        "innovator.USER", "innovator.PERSON", "innovator.IDENTITY", "innovator.Person_Search", "innovator.IDENTITY",
        "innovator.Person_Search", "innovator.DEPARTMENT", "innovator.INVENTORY_ORG", "innovator.COMPANY", "innovator.COMPANY",
        "innovator.FEATURE", "innovator.DEFECT", "innovator.htmltotext", "innovator.COMPANY", "innovator.INVENTORY_ORG", "innovator.PART",
        "innovator.PART", "innovator.FUNCTIONAL_GROUP", "innovator.CONCERN", "innovator.ISSUE", "innovator.IDENTITY", "innovator.ALIAS",
        "innovator.PERSON", "innovator.FEATURE_DEFECT", "innovator.MANUFACTURING_ENTITY", "innovator.PART", "innovator.PART",
        "innovator.MANUFACTURING_ENTITY", "innovator.DEPARTMENT", "innovator.Last_Comments", "innovator.ANALYSIS", "innovator.PROGRAM",
        "innovator.LAB_TEST_ROUND", "innovator.ConvertToLocalTbl", "innovator.ConvertToLocalTbl", "innovator.ConvertToLocalTbl",
        "innovator.ConvertToLocalTbl", "innovator.ConvertToLocalTbl" };
      var names = DependencyAnalyzer.GetInnovatorNames(prop.Resources.SampleSql)
        .Select(n => n.FullName).ToArray();
      CollectionAssert.AreEqual(correct, names);

      correct = new string[] {
        "PE_RollupAllPartsInDB", "PART", "PART", "PART", "PART_GOAL", "PART", "PART", "PART", "PART_GOAL", "PART", "PART_BOM",
        "PART_GOAL", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART_BOM",
        "ind_tmp_BOMS_TO_USE_source_id", "ind_tmp_BOMS_TO_USE_related_id", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL",
        "PART_BOM", "PART", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_BOM", "PART", "PART", "PART", "PART_GOAL"};

      names = DependencyAnalyzer.GetInnovatorNames(prop.Resources.SampleSql_PartRollup)
        .Select(n => n.FullName).ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void AliasParseTest()
    {
      var sql = @"select count(*)
                from innovator.[transaction_request]
                group by id";
      var parsed = new SqlTokenizer(sql).Parse();
      Assert.IsTrue(string.IsNullOrEmpty(parsed.OfType<SqlName>().First().Alias));
    }

    [TestMethod()]
    public void SelectColumnNamesTest()
    {
      var sql = @"select 1 + 3 count
                  , isnull(id, config_id) not_null_id
                  , isnull(id, config_id) /* not-named */
                  , isnull(id, config_id)
                  , 3
                  , created_by_id
                  , owned_by_id stuff
                  , tr.major_rev
                  , tr.minor_rev
                from innovator.[transaction_request] tr
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var correct = new string[] { "count", "not_null_id", "created_by_id", "stuff", "major_rev", "minor_rev" };
      var names = parsed.GetColumnNames().ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void SelectContextTest()
    {
      var sql = @"select 1 + 3 count
                  , tr.minor_rev
                from innovator.[transaction_request] tr
                inner join innovator.[Part_bom] pb
                on pb.source_id = tr.part
                inner join (
                  select id, stuff, another
                  from innovar.part
                ) d
                on d.id = pb.relate_id
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var context = new SqlContext(parsed);
      Assert.AreEqual(3, context.Tables.Count());
    }

    [TestMethod]
    public void SqlPartialTest()
    {
      var sql = @"select *
                  from (
                    select
                    from innovator.[PART]";
      var parsed = new SqlTokenizer(sql).Parse();
      Assert.AreEqual(typeof(SqlName), ((SqlGroup)parsed.Last()).Last().GetType());
    }

    [TestMethod()]
    public void SqlTestUnfinishedExpr()
    {
      var sql = @"select max(a.CAN_GET) CAN_GET
                  from innovator.[ALIAS] al
                  and ISNULL(al.

                  where a.SOURCE_ID = @permission
                  ;

                  select parent, child
                  from innovator.[IDENTITY] i
                  ;";
      var parsed = new SqlTokenizer(sql).Parse();
      Assert.AreEqual(2, parsed.Count);
    }
  }
}
