using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class DelegateContext : WorkflowContext, IDelegateContext
  {
    private IReadOnlyItem _assignment;
    private IReadOnlyItem _delegate;

    public DelegateContext(IServerConnection conn, IReadOnlyItem item)
      : base(conn, item)
    {
      var aml = conn.AmlContext;
      _assignment = aml.Item(aml.Type("Activity Assignment"), aml.Id(item.Property("AssignmentId").Value),
        aml.SourceId(aml.KeyedName(item.KeyedName()), aml.Type(item.Type().Value), item.Id()),
        aml.Property("id", item.Property("AssignmentId").Value)
      );
      _delegate = aml.Item(aml.Type("Activity Assignment"), aml.Id(item.Property("ToAssignmentId").Value),
        aml.SourceId(aml.KeyedName(item.KeyedName()), aml.Type(item.Type().Value), item.Id()),
        aml.Property("id", item.Property("ToAssignmentId").Value)
      );
    }

    public IReadOnlyItem Assignment
    {
      get { return _assignment; }
    }
    
    public IReadOnlyItem DelegateTo
    {
      get { return _delegate; }
    }
  }
}
