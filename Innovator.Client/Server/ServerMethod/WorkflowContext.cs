using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using Innovator.Client.Model;

namespace Innovator.Server
{
  public class WorkflowContext : IWorkflowContext
  {
    private WorkflowEvent _event;
    private IServerConnection _conn;
    private Activity _activity;
    private IReadOnlyItem _context;
    private bool _contextLoaded = false;
    private IResult _result;

    public Activity Activity
    {
      get { return _activity; }
    }
    public IServerConnection Conn
    {
      get { return _conn; }
    }
    public IReadOnlyItem Context
    {
      get
      {
        EnsureContext();
        return _context;
      }
    }
    public IErrorBuilder ErrorBuilder
    {
      get { return _result; }
    }
    public Exception Exception
    {
      get { return _result.Exception; }
    }
    public WorkflowEvent WorkflowEvent
    {
      get { return _event; }
    }

    public Action<IItem> QueryDefaults { get; set; }

    public WorkflowContext(IServerConnection conn, IReadOnlyItem item)
    {
      _conn = conn;
      _activity = item as Activity;
      _result = conn.AmlContext.Result();
      switch (item.Property("WorkflowEvent").Value)
      {
        case "on_activate":
          _event = WorkflowEvent.OnActivate;
          break;
        case "on_assign":
          _event = WorkflowEvent.OnAssign;
          break;
        case "on_close":
          _event = WorkflowEvent.OnClose;
          break;
        case "on_delegate":
          _event = WorkflowEvent.OnDelegate;
          break;
        case "on_due":
          _event = WorkflowEvent.OnDue;
          break;
        case "on_escalate":
          _event = WorkflowEvent.OnEscalate;
          break;
        case "on_refuse":
          _event = WorkflowEvent.OnRefuse;
          break;
        case "on_remind":
          _event = WorkflowEvent.OnRemind;
          break;
        case "on_vote":
          _event = WorkflowEvent.OnVote;
          break;
        default:
          _event = WorkflowEvent.Other;
          break;
      }
    }

    private void EnsureContext()
    {
      if (!_contextLoaded)
      {
        _contextLoaded = true;
        var aml = Conn.AmlContext;
        var query = aml.Item(aml.Type("Workflow"), aml.Action("get"),
          aml.Select("source_type", "source_id"), aml.RelatedExpand(false),
          aml.RelatedId(
            aml.Item(aml.Type("Workflow Process"), aml.Action("get"),
              aml.Relationships(
                aml.Item(aml.Type("Workflow Process Activity"), aml.Action("get"),
                  aml.RelatedId(_activity.Id())
                )
              )
            )
          )
        );
        if (QueryDefaults != null) QueryDefaults(query);
        var workflow = Conn.ItemByQuery(query.ToAml());

        query = aml.Item(aml.TypeId(workflow.Property("source_type").Value),
          aml.Id(workflow.SourceId().Value), aml.Action("get"));
        if (QueryDefaults != null) QueryDefaults(query);
        _context = Conn.ItemByQuery(query.ToAml());
        _result.ErrorContext(_context);
      }
    }
  }
}
