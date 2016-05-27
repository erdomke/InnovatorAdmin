using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Wrapper for items representing Activities
  /// </summary>
  public class Activity : ItemWrapper
  {
    public Activity(IReadOnlyItem item) : base(item) { }

    /// <summary>
    /// Retrieve the Workflow Process Path by name
    /// </summary>
    public IReadOnlyItem Path(IConnection conn, string name)
    {
      var path = this.Relationships("Workflow Process Path").FirstOrDefault(i => i.Property("name").Value == name);
      if (path != null) return path;
      return conn.ItemByQuery(new Command(@"<Item type='Workflow Process Path' action='get'>
                                              <source_id>@0</source_id>
                                              <name>@1</name>
                                            </Item>", this.Id(), name));
    }
    /// <summary>
    /// Perform a vote for a specified assignment and path
    /// </summary>
    public IReadOnlyResult PerformVote(IConnection conn, string assignmentId, string pathName, 
      string comment = null)
    {
      var path = Path(conn, pathName);
      return conn.Apply(new Command(@"<AML>
                                        <Item type='Activity' action='EvaluateActivityEx'>
                                          <Activity>@0</Activity>
                                          <ActivityAssignment>@1</ActivityAssignment>
                                          <Paths>
                                            <Path id='@2'>@3</Path>
                                          </Paths>
                                          <DelegateTo>0</DelegateTo>
                                          <Tasks/>
                                          <Variables/>
                                          <Authentication mode=''/>
                                          <Comments>@4</Comments>
                                          <Complete>1</Complete>
                                        </Item>
                                      </AML>", this.Id(), assignmentId, path.Id(), pathName,
                                             comment));
    }
    public void SetDurationByDate(IConnection conn, DateTime dueDate, int minDuration = 1, 
                                  int maxDuration = int.MaxValue)
    {
      var props = this.LazyMap(conn, i => new { 
        ActiveDate = i.Property("active_date").AsDateTime(DateTime.Now) 
      });
      var duration = Math.Min(Math.Max((dueDate.Date - props.ActiveDate.Date).Days, 
                                      minDuration), maxDuration);
      this.Edit(conn, conn.AmlContext.Property("expected_duration", duration)).AssertNoError();
    }
    public void SetIsAuto(IConnection conn, bool isAuto)
    {
      this.Edit(conn, conn.AmlContext.Property("is_auto", isAuto)).AssertNoError();
    }
  }
}
