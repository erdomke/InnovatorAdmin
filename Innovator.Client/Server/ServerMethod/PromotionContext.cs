using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class PromotionContext : IPromotionContext
  {
    private IServerConnection _conn;
    private IReadOnlyItem _item;

    public LifeCycleTransition Transition
    {
      get { return new LifeCycleTransition(_item.Property("transition").AsItem()); }
    }
        
    public IServerConnection Conn
    {
      get { return _conn; }
    }

    public IReadOnlyItem Item
    {
      get { return _item; }
    }

    public PromotionContext(IServerConnection conn, IReadOnlyItem item)
    {
      _conn = conn;
      _item = item;
    }
  }
}
