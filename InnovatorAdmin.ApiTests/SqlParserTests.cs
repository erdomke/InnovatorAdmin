using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aras.Tools.InnovatorAdmin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prop = global::InnovatorAdmin.ApiTests.Properties;

namespace Aras.Tools.InnovatorAdmin.Tests
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
      var parser = new SqlParser();
      parser.SchemaToKeep = "innovator";
      var names = parser.FindSqlServerObjectNames(prop.Resources.SampleSql).ToArray();
      CollectionAssert.AreEqual(correct, names);

      correct = new string[] { 
        "PE_RollupAllPartsInDB", "PART", "PART", "PART", "PART_GOAL", "PART", "PART", "PART", "PART_GOAL", "PART", "PART_BOM", 
        "PART_GOAL", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART_BOM", 
        "ind_tmp_BOMS_TO_USE_source_id", "ind_tmp_BOMS_TO_USE_related_id", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL", 
        "PART_BOM", "PART", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_BOM", "PART", "PART", "PART", "PART_GOAL"};
      names = parser.FindSqlServerObjectNames(prop.Resources.SampleSql_PartRollup).ToArray();
      CollectionAssert.AreEqual(correct, names);
    }
  }
}
