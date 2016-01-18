using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class SingleItemContext : ISingleItemContext
  {
    private IServerConnection _conn;
    private IReadOnlyItem _item;
    
    public IServerConnection Conn
    {
      get { return _conn; }
    }

    public IReadOnlyItem Item
    {
      get { return _item; }
    }

    public SingleItemContext(IServerConnection conn, IReadOnlyItem item)
    {
      _conn = conn;
      _item = item;
    }
  }
}
