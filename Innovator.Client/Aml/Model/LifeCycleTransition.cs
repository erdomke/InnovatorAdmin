using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Wrapper for items representing Life Cycle Transitions
  /// </summary>
  public class LifeCycleTransition : ItemWrapper
  {
    public LifeCycleTransition(IReadOnlyItem item) : base(item) { }

    public LifeCycleState FromState()
    {
      return new LifeCycleState(base.Property("from_state").AsItem());
    }
    public LifeCycleState ToState()
    {
      return new LifeCycleState(base.Property("to_state").AsItem());
    }
    public IReadOnlyItem Identity() { return base.Property("role").AsItem(); }
  }
}
