using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace InnovatorAdmin.Tests
{
  [TestClass()]
  public class SqlParserTests
  {
    [TestMethod()]
    public void TriggerSqlTest()
    {
      var sql = @"CREATE TRIGGER [Project_Task_Step04_Create_Trigger] ON [PROJECT_TASK]
INSTEAD OF UPDATE
AS
BEGIN
  UPDATE Activity2
  SET locked_by_id = inserted.locked_by_id
  FROM inserted
  WHERE Activity2.id = inserted.id

  UPDATE Activity2_Assignment
  SET locked_by_id = inserted.locked_by_id
  FROM inserted
  WHERE Activity2_Assignment.id = inserted.id

END";
      var names = DependencyAnalyzer.GetInnovatorNames(sql)
        .Select(n => n.FullName).ToArray();
      var correct = new string[] { "Project_Task_Step04_Create_Trigger", "Activity2", "inserted", "Activity2_Assignment", "inserted" };
      CollectionAssert.AreEqual(correct, names);
    }

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
      var names = DependencyAnalyzer.GetInnovatorNames(@"SELECT
    c.[id]
  , case when charindex('/', c.[CLASSIFICATION]) = 0 then c.[CLASSIFICATION] else left(c.CLASSIFICATION, charindex('/', c.[CLASSIFICATION])-1) end classification_01
  , case when charindex('/', c.[CLASSIFICATION]) = 0 then c.[CLASSIFICATION]
     when charindex('/', c.[CLASSIFICATION], charindex('/', c.[CLASSIFICATION])+1) = 0 then c.[CLASSIFICATION]
     else left(c.CLASSIFICATION, charindex('/', c.[CLASSIFICATION],charindex('/', c.[CLASSIFICATION])+1)-1) end classification_02
  , c.[CLASSIFICATION]
  , d_cr.LocalDate CREATED_ON
  , (select KEYED_NAME from innovator.[USER] where id = c.CREATED_BY_ID) CREATED_BY
  , (select Employee_ID from innovator.PERSON where ARAS_USER = c.CREATED_BY_ID) CREATED_BY_EMP_ID
  , (select KEYED_NAME from innovator.[IDENTITY] where id = c.OWNED_BY_ID) OWNED_BY
  , (select employee_id from innovator.Person_Search where ident_id = c.OWNED_BY_ID) OWNED_BY_emp_id
  , (select KEYED_NAME from innovator.[IDENTITY] where id = c.MANAGED_BY_ID) ISSUE_OWNER_REVIEWER
  , (select employee_id from innovator.Person_Search where ident_id = c.MANAGED_BY_ID) ISSUE_OWNER_REVIEWER_emp_id
  , resp.KEYED_NAME problem_assigned_to
  , presp.employee_id problem_assigned_to_emp_id
  , presp.department_text problem_assigned_to_dept
  , c.[CONCERN_ID]
  , cpmfg.ITEM_NUMBER current_part_mfg_line
  , isnull((select MFG_GROUP from innovator.DEPARTMENT where id = cpmfg.DEPARTMENT), pa.primary_mfg_group) current_part_mfg_group
  , (select CODE from innovator.INVENTORY_ORG where id = c.CURRENT_PART_ORGANIZATION) current_part_mfg_org
  , (select keyed_name from innovator.COMPANY where id = c.[CUSTOMER]) customer_direct
  , (select keyed_name from innovator.COMPANY where id = isnull(c.[CUSTOMER_OEM], c.[CUSTOMER])) customer_oem
  , d_cl.LocalDate date_closed
  , d_vb.LocalDate DATE_VEHICLE_BUILD
  , c.[DEALER_NUMBER]
  , c.[DEALER_STATE]
  , (select KEYED_NAME from innovator.FEATURE where id = fd.source_id) defective_feature
  , (select KEYED_NAME from innovator.DEFECT where id = fd.RELATED_ID) defect_type
  , innovator.htmltotext(c.[DESCRIPTION]) DESCRIPTION
  , c.[ISSUE]
  , pa.ITEM_NUMBER current_part_number
  , pa.MAJOR_REV current_part_rev
  , pb.ITEM_NUMBER problem_part_number
  , pb.MAJOR_REV problem_part_rev
  , c.TEST_PHASE
  , c.[TEST_PROBLEM_TYPE]
  , c.LAB
  , c.LAB_TEST_ROUND lab_round_ct_id
  , rnd.LAB_ROUND_ID LAB_ROUND_ID
  , (select keyed_name from innovator.COMPANY where id = c.[MANUFACTURER]) manufacturer
  , ppmfg.ITEM_NUMBER problem_part_mfg_line
  , isnull(ppdep.MFG_GROUP, pb.primary_mfg_group) problem_part_mfg_group
  , (select CODE from innovator.INVENTORY_ORG where id = isnull(c.[MANUFACTURING_ORG], ppdep.inventory_org)) problem_part_mfg_org
  , c.[NAME]
  , d_oc.LocalDate OCCURRENCE_DATE
  , c.[PROBLEM_STATE]
  , c.[PRODUCT_LINE]
  , prgm.TEAMLINK_ID program
  , prgm.NAME program_name
  , c.[QTY_FOUND]
  , c.[QTY_TO_REPORT]
  , c.[QUANTITY_INSPECTED]
  , c.[RESPONSIBILITY]
  , (select item_number from innovator.PART where id = c.[RESPONSIBLE_PART_NUMBER]) RESPONSIBLE_PART_NUMBER
  , (select PRIMARY_MFG_GROUP from innovator.PART where id = c.[RESPONSIBLE_PART_NUMBER]) RESPONSIBLE_PART_MFG_GROUP
  , (select keyed_name from innovator.FUNCTIONAL_GROUP where id = c.[RESPONSIBLE_PARTY]) RESPONSIBLE_PARTY
  , c.[SEVERITY]
  , c.[VEHICLE_MAKE]
  , c.[VEHICLE_MODEL]
  , c.[VEHICLE_MODEL_YEAR]
  , c.[MANUFACTURING_LOCATION_TYPE]
  , d_ev.LocalDate comment_date
  , com.comments
  , c.CURRENT_PART_SHIFT
  , c.OCCURRENCE_SHIFT
  , case when a.DATE_CLOSED is null then null else cast(round(DATEDIFF(ss, c.CREATED_ON, a.DATE_CLOSED) / 86400.0,0) as int) end CUSTOMER_ANALYSIS_DAYS_TO_COMPLETE
FROM innovator.[CONCERN] c
left join innovator.ISSUE i
  on i.id = c.ISSUE
left join innovator.[IDENTITY] resp
  on resp.id = case when c.issue is not null then 
                case when i.STATE = 'Verification' then i.MANAGED_BY_ID
                else i.OWNED_BY_ID end
               when c.state in ('Canceled', 'Closed', 'Closed : Conversion') then isnull(c.MANAGED_BY_ID, c.OWNED_BY_ID)
               when c.state = 'In Work' then c.OWNED_BY_ID
               else c.MANAGED_BY_ID end
left join innovator.ALIAS aresp
  on aresp.RELATED_ID = resp.id
left join innovator.PERSON presp
  on presp.ARAS_USER = aresp.SOURCE_ID
left join innovator.FEATURE_DEFECT fd
  on fd.id = c.DEFECT
left join innovator.MANUFACTURING_ENTITY cpmfg
  on cpmfg.id = c.CURRENT_PART_MFG_LINE
left join innovator.PART pa
  on pa.id = c.ITEM_NBR_ASSY
left join innovator.PART pb
  on pb.id = c.ITEM_NBR_BASE
left join innovator.MANUFACTURING_ENTITY ppmfg
on ppmfg.id = c.MANUFACTURING_LINE
left join innovator.DEPARTMENT ppdep
on ppdep.id = ppmfg.DEPARTMENT
left join innovator.Last_Comments com
on com.item_id = c.id
and com.itemtype_id = 'F1D2DE3EAFAB4B2B with innovator.[another] 935D7061C49DF35B'
left join (
  select a.concern, min(a.DATE_CLOSED) DATE_CLOSED
  from innovator.ANALYSIS a
  where a.CLASSIFICATION in ('Customer Return/Initial IEC Functional (Stage 1)', 'Customer Return/Initial Visual (Stage 1)') 
  and a.DATE_CLOSED is not null
  group by a.CONCERN
) a
on a.CONCERN = c.id
left join innovator.PROGRAM prgm
on prgm.id = c.PROGRAM
left join innovator.LAB_TEST_ROUND rnd
on rnd.id = c.LAB_TEST_ROUND
/* a test for innovator.[thingy] */
CROSS APPLY innovator.ConvertToLocalTbl(c.[CREATED_ON], 'Eastern Standard Time') d_cr
CROSS APPLY innovator.ConvertToLocalTbl(c.[DATE_CLOSED], 'Eastern Standard Time') d_cl
CROSS APPLY innovator.ConvertToLocalTbl(c.[DATE_VEHICLE_BUILD], 'Eastern Standard Time') d_vb
CROSS APPLY innovator.ConvertToLocalTbl(c.[OCCURRENCE_DATE], 'Eastern Standard Time') d_oc
CROSS APPLY innovator.ConvertToLocalTbl(com.event_date, 'Eastern Standard Time') d_ev")
        .Select(n => n.FullName).ToArray();
      CollectionAssert.AreEqual(correct, names);

      correct = new string[] {
        "PE_RollupAllPartsInDB", "PART", "PART", "PART", "PART_GOAL", "PART", "PART", "PART", "PART_GOAL", "PART", "PART_BOM",
        "PART_GOAL", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART", "PART_GOAL", "PART", "PART_BOM",
        "ind_tmp_BOMS_TO_USE_source_id", "ind_tmp_BOMS_TO_USE_related_id", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL",
        "PART_BOM", "PART", "PART", "PART", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_GOAL", "PART_BOM", "PART", "PART", "PART", "PART_GOAL"};

      names = DependencyAnalyzer.GetInnovatorNames(@"/*
name: PE_RollupAllPartsInDB
solution: PLM
created: 06-OCT-2006
purpose: Rollup all Parts cost and weight in DB
notes:
*/

CREATE PROCEDURE PE_RollupAllPartsInDB
AS
BEGIN
  -- 1. Set cost to NULL on Parts with no Part Goal for cost
  UPDATE PART
  SET cost=NULL, cost_basis=NULL
  FROM PART all_parts
    INNER JOIN
      (SELECT p.id id
       FROM PART p LEFT OUTER JOIN PART_GOAL pg ON p.id=pg.source_id
       WHERE (pg.goal IS NULL) OR (pg.goal != 'Cost')) no_cost
    ON all_parts.id=no_cost.id;
  
  -- 2. Set weight to NULL on Parts with no Part Goal for weight
  UPDATE PART
  SET weight=NULL, weight_basis=NULL
  FROM PART all_parts
    INNER JOIN
      (SELECT p.id id
       FROM PART p LEFT OUTER JOIN PART_GOAL pg ON p.id=pg.source_id
       WHERE (pg.goal IS NULL) OR (pg.goal != 'Weight')) no_weight
    ON all_parts.id=no_weight.id;
  
  DECLARE @TMP_PARTS TABLE(id CHAR(32) COLLATE database_default PRIMARY KEY);
  
  -- 3. Find leaf Parts
  INSERT INTO @TMP_PARTS(id)
  SELECT p.id
  FROM PART p LEFT OUTER JOIN PART_BOM pb ON p.id=pb.source_id
  WHERE pb.id IS NULL;
  
  -- 4. In all leaf Part Goals calculated_value must be NULL
  UPDATE PART_GOAL
  SET calculated_value=NULL
  FROM PART_GOAL pg INNER JOIN @TMP_PARTS p ON p.id=pg.source_id;
  
  -- 5. Set correct costs on leaf Parts
  UPDATE PART
  SET cost=new_cost.new_value, cost_basis=new_cost.new_basis
  FROM PART p
    INNER JOIN
      (SELECT p.id id, ISNULL(actual_value,ISNULL(estimated_value,ISNULL(guess_value,target_value))) new_value,
         CASE
           WHEN actual_value IS NOT NULL THEN 'Actual'
           WHEN estimated_value IS NOT NULL THEN 'Estimated'
           WHEN guess_value IS NOT NULL THEN 'Guess'
           WHEN target_value IS NOT NULL THEN 'Target'
           ELSE NULL
         END new_basis
       FROM @TMP_PARTS p INNER JOIN PART_GOAL pg ON p.id=pg.source_id
       WHERE pg.goal='Cost') new_cost
    ON p.id=new_cost.id;
  
  -- 6. Set correct weights on leaf Parts
  UPDATE PART
  SET weight=new_weight.new_value, cost_basis=new_weight.new_basis
  FROM PART p
    INNER JOIN
      (SELECT p.id id, ISNULL(actual_value,ISNULL(estimated_value,ISNULL(guess_value,target_value))) new_value,
         CASE
           WHEN actual_value IS NOT NULL THEN 'Actual'
           WHEN estimated_value IS NOT NULL THEN 'Estimated'
           WHEN guess_value IS NOT NULL THEN 'Guess'
           WHEN target_value IS NOT NULL THEN 'Target'
           ELSE NULL
         END new_basis
       FROM @TMP_PARTS p INNER JOIN PART_GOAL pg ON p.id=pg.source_id
       WHERE pg.goal='Weight') new_weight
    ON p.id=new_weight.id;
  
  DELETE FROM @TMP_PARTS;
  
  -- 3. Update all parts and check for infinite loop
  CREATE TABLE #tmp_PARTS_TO_USE(id CHAR(32) COLLATE database_default PRIMARY KEY);
  INSERT INTO #tmp_PARTS_TO_USE(id)
  SELECT id
  FROM PART;
  
  IF (@@ROWCOUNT = 0) /* there are no Parts in DB */
  BEGIN
    GOTO finish;
  END
  
  CREATE TABLE #tmp_BOMS_TO_USE(id CHAR(32) COLLATE database_default PRIMARY KEY,
                               source_id CHAR(32) COLLATE database_default,
                               related_id CHAR(32) COLLATE database_default);
  
  INSERT INTO #tmp_BOMS_TO_USE(id, source_id, related_id)
  SELECT id, source_id, related_id
  FROM PART_BOM
  WHERE related_id IS NOT NULL;
  
  IF (@@ROWCOUNT > 0) /* there are Part BOMs in DB */
  BEGIN
    CREATE INDEX ind_tmp_BOMS_TO_USE_source_id ON #tmp_BOMS_TO_USE(source_id);
    CREATE INDEX ind_tmp_BOMS_TO_USE_related_id ON #tmp_BOMS_TO_USE(related_id);
  END
  
  /* +++ iterate over leaf Parts. And calculate their costs. Check for infinite loop also. */
  DECLARE @PARTS_TO_ROLLUP INT;
  DECLARE @CURRENT_PARTS TABLE(id CHAR(32) COLLATE database_default PRIMARY KEY);
  
  WHILE (1=1)
  BEGIN
    DECLARE @LEAF_PARTS_COUNT INT;
    
    /* find leaf nodes */
    INSERT INTO @TMP_PARTS(id)
    SELECT p.id
    FROM #tmp_PARTS_TO_USE p LEFT OUTER JOIN #tmp_BOMS_TO_USE pb ON p.id=pb.source_id
    WHERE pb.id IS NULL;
    
    SET @LEAF_PARTS_COUNT = @@ROWCOUNT;
    
    IF (@LEAF_PARTS_COUNT = 0) /* there are no leaf nodes any more */
    BEGIN
      DECLARE @infinitive_loop_parent_id CHAR(32); /* id of some ""parent"" Part from infinite loop */
      DECLARE @infinitive_loop_child_id CHAR(32);  /* id of ""child"" Part from infinite loop */
      
      SELECT TOP 1 @infinitive_loop_parent_id = pb.source_id, @infinitive_loop_child_id=pb.related_id
      FROM #tmp_BOMS_TO_USE pb INNER JOIN #tmp_PARTS_TO_USE p ON pb.source_id=p.id;
      
      IF (@infinitive_loop_parent_id IS NULL) /* there is no infinite loop. There no Parts any more. */
      BEGIN
        BREAK;
      END
      
      DECLARE @parent_part_keyed_name VARCHAR(128);
      DECLARE @child_part_keyed_name VARCHAR(128);
      
      SELECT @parent_part_keyed_name=ISNULL(keyed_name,id)
      FROM PART
      WHERE id=@infinitive_loop_parent_id;
      
      SELECT @child_part_keyed_name=ISNULL(keyed_name,id)
      FROM PART
      WHERE id=@infinitive_loop_child_id;
      
      DECLARE @infiniteLoopMsg VARCHAR(2000); /* 2000>>128+32+128+32 */
      DECLARE @infiniteLoopSeverity INT;     /* the user-defined severity level associated with this message */
      DECLARE @infiniteLoopState INT;        /* Is an arbitrary integer from 1 through 127 that represents information about the invocation state of the error. */
      
      SET @infiniteLoopMsg = '<B>The BOM structure contains circular references</B>. Please check dependency between Part ""' + 
        @parent_part_keyed_name + '"" (' + @infinitive_loop_parent_id + ') and ""' +
        @child_part_keyed_name + '"" (' + @infinitive_loop_child_id + ').';
      SET @infiniteLoopSeverity = 16; /* Severity Levels 11 through 16: These messages indicate errors that can be corrected by the user. */
      SET @infiniteLoopState = 1;
      
      RAISERROR(@infiniteLoopMsg, @infiniteLoopSeverity, @infiniteLoopState)
      RETURN @@ERROR;
    END
    
    DELETE FROM @CURRENT_PARTS;
    
    INSERT INTO @CURRENT_PARTS(id)
    SELECT DISTINCT pb.source_id
    FROM #tmp_BOMS_TO_USE pb INNER JOIN @TMP_PARTS p ON p.id=pb.related_id;
    
    -- . Update calculated cost value on Part Goals
    UPDATE PART_GOAL
    SET calculated_value=new_cost.new_val
    FROM PART_GOAL pg
      INNER JOIN
        (SELECT pg.id id,
           CASE
             WHEN SUM(
                    CASE child_p.cost_basis
                      WHEN 'Actual' THEN 0
                      WHEN 'Calculated' THEN 0
                      WHEN 'Estimated' THEN 0
                      ELSE ABS(child_pb.QUANTITY)
                    END
                  ) > 0 THEN NULL
             ELSE SUM( ISNULL(child_p.cost,0)*ISNULL(child_pb.quantity, 0) )
           END new_val
         FROM @CURRENT_PARTS current_p
           INNER JOIN PART_GOAL pg ON pg.source_id=current_p.id
           INNER JOIN PART_BOM child_pb ON child_pb.source_id=current_p.id
           INNER JOIN PART child_p ON child_pb.related_id=child_p.id
         WHERE pg.goal='Cost'
         GROUP BY pg.id) new_cost
      ON pg.id=new_cost.id;
      
    -- . Set correct costs on current Parts
    UPDATE PART
    SET cost=new_cost.new_value, cost_basis=new_cost.new_basis
    FROM PART p
      INNER JOIN
        (SELECT p.id id, ISNULL(actual_value,ISNULL(calculated_value,ISNULL(estimated_value,ISNULL(guess_value,target_value)))) new_value,
           CASE
             WHEN actual_value IS NOT NULL THEN 'Actual'
             WHEN calculated_value IS NOT NULL THEN 'Calculated'
             WHEN estimated_value IS NOT NULL THEN 'Estimated'
             WHEN guess_value IS NOT NULL THEN 'Guess'
             WHEN target_value IS NOT NULL THEN 'Target'
             ELSE NULL
           END new_basis
         FROM @CURRENT_PARTS p INNER JOIN PART_GOAL pg ON p.id=pg.source_id
         WHERE pg.goal='Cost') new_cost
      ON p.id=new_cost.id;
    
    -- . Update calculated weight value on Part Goals
    UPDATE PART_GOAL
    SET calculated_value=new_weight.new_val
    FROM PART_GOAL pg
      INNER JOIN
        (SELECT pg.id id,
           CASE
             WHEN SUM(
                    CASE child_p.weight_basis
                      WHEN 'Actual' THEN 0
                      WHEN 'Calculated' THEN 0
                      WHEN 'Estimated' THEN 0
                      ELSE 1
                    END
                  ) > 0 THEN NULL
             ELSE SUM( ISNULL(child_p.weight,0)*ISNULL(child_pb.quantity, 0) )
           END new_val
         FROM @CURRENT_PARTS current_p
           INNER JOIN PART_GOAL pg ON pg.source_id=current_p.id
           INNER JOIN PART_BOM child_pb ON child_pb.source_id=current_p.id
           INNER JOIN PART child_p ON child_pb.related_id=child_p.id
         WHERE pg.goal='Weight'
         GROUP BY pg.id) new_weight
      ON pg.id=new_weight.id;
    
    -- . Set correct weights on current Parts
    UPDATE PART
    SET weight=new_weight.new_value, weight_basis=new_weight.new_basis
    FROM PART p
      INNER JOIN
        (SELECT p.id id, ISNULL(actual_value,ISNULL(calculated_value,ISNULL(estimated_value,ISNULL(guess_value,target_value)))) new_value,
           CASE
             WHEN actual_value IS NOT NULL THEN 'Actual'
             WHEN calculated_value IS NOT NULL THEN 'Calculated'
             WHEN estimated_value IS NOT NULL THEN 'Estimated'
             WHEN guess_value IS NOT NULL THEN 'Guess'
             WHEN target_value IS NOT NULL THEN 'Target'
             ELSE NULL
           END new_basis
         FROM @CURRENT_PARTS p INNER JOIN PART_GOAL pg ON p.id=pg.source_id
         WHERE pg.goal='Weight') new_weight
      ON p.id=new_weight.id;
    
    /* delete BOMs pointing to leaf Parts */
    DELETE FROM #tmp_BOMS_TO_USE
    WHERE related_id IN (SELECT id FROM @TMP_PARTS);
    
    /* delete leaf Parts */
    DELETE FROM #tmp_PARTS_TO_USE
    WHERE id IN (SELECT id FROM @TMP_PARTS);

    DELETE FROM @TMP_PARTS;
  END
  DROP TABLE #tmp_BOMS_TO_USE;
  DROP TABLE #tmp_PARTS_TO_USE;
  /* +++ iterate over leaf Parts */

finish:
  SELECT 1 success;
END")
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
                  , isnull(id, config_id) named /* named */
                  , isnull(id, config_id) named2 -- named asdfsdf
                  , isnull(id, config_id)
                  , 3
                  , created_by_id
                  , owned_by_id stuff
                  , tr.major_rev
                  , tr.minor_rev
                from innovator.[transaction_request] tr
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var correct = new string[] { "count", "not_null_id", "named", "named2", "created_by_id", "stuff", "major_rev", "minor_rev" };
      var names = parsed.GetColumnNames().ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void SelectColumnNamesAsteriskTest()
    {
      var sql = @"select 1 + 3 count
                  , *
                  , tr.*
                  , tr2.* /* not-named */
                  , 3
                  , created_by_id
                from innovator.[transaction_request] tr
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var correct = new string[] { "count", "*", "tr.*", "tr2.*", "created_by_id" };
      var names = parsed.GetColumnNames().ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void SelectColumnNamesEqualsTest()
    {
      var sql = @"select count = 1 + 3
                  , isnull(id, config_id) not_null_id
                  , isnull(id, config_id) /* not-named */
                  , isnull(id, config_id)
                  , 3
                  , created_by_id = thing
                  , owned_by_id stuff
                  , major_rev = tr.major_rev2
                  , tr.minor_rev
                from innovator.[transaction_request] tr
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var correct = new string[] { "count", "not_null_id", "created_by_id", "stuff", "major_rev", "minor_rev" };
      var names = parsed.GetColumnNames().ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void SelectColumnNamesSubSelectTest()
    {
      var sql = @"select *
                from (
                  select count, non_null_id, b.*
                  from a
                  inner join (
                    select first, second, third
                    from another
                  ) b
                  on a.thing = b.stuff
                ) d
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var correct = new string[] { "count", "non_null_id", "first", "second", "third" };
      var ctx = new SqlContext(parsed);
      var names = ctx.Tables.Single().Columns.ToArray();
      CollectionAssert.AreEqual(correct, names);
    }

    [TestMethod()]
    public void SelectColumnNamesSubSelectNoNameTest()
    {
      var sql = @"select *, 1 + 2 count
                from (
                  select *
                  from (
                    select *, 3 + 4 thing
                    from another
                  ) b
                ) d
                group by id, config_id, major_rev";
      var parsed = new SqlTokenizer(sql).Parse();
      var ctx = new SqlContext(parsed);
      var names = ctx.Tables.Single().Columns.ToArray();
      //CollectionAssert.AreEqual(correct, names);
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
