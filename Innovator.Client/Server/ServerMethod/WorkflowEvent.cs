using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public enum WorkflowEvent
  {
    Other,
    OnActivate,
    OnAssign,
    OnClose,
    OnDelegate,
    OnDue,
    OnEscalate,
    OnRefuse,
    OnRemind,
    OnVote
  }
}
