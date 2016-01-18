using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class VoteContext : WorkflowContext, IVoteContext
  {
    private IReadOnlyItem _assignment;

    public VoteContext(IServerConnection conn, IReadOnlyItem item) : base(conn, item) 
    {
      var aml = conn.AmlContext;
      _assignment = aml.Item(aml.Type("Activity Assignment"), aml.Id(item.Property("AssignmentId").Value),
        aml.SourceId(aml.KeyedName(item.KeyedName()), aml.Type(item.Type().Value), item.Id()),
        aml.Property("id", item.Property("AssignmentId").Value)
      );
    }

    public IReadOnlyItem Assignment
    {
      get { return _assignment; }
    }

    public string Path
    {
      get { return Activity.Property("Path").Value; }
    }
  }
}
