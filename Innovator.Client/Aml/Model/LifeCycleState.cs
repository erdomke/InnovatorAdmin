using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class LifeCycleState : ItemWrapper
  {
    public LifeCycleState(IReadOnlyItem item) : base(item) { }

    public new string Name() { return base.Property("name").Value; }
    public string Label() { return base.Property("label").Value; }
  }
}
