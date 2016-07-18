using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using Innovator.Client.Model;

namespace Innovator.Server
{
  public class PromotionContext : IPromotionContext
  {
    private IServerConnection _conn;
    private IReadOnlyItem _item;

    public LifeCycleTransition Transition
    {
      get { return _item.Property("transition").AsItem() as LifeCycleTransition; }
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
