using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class MultiItemContext : IMultipleItemContext
  {
    private IServerConnection _conn;
    private IEnumerable<IReadOnlyItem> _items;
    
    public IServerConnection Conn
    {
      get { return _conn; }
    }

    public IEnumerable<IReadOnlyItem> Items
    {
      get { return _items; }
    }

    public MultiItemContext(IServerConnection conn, IEnumerable<IReadOnlyItem> items)
    {
      _conn = conn;
      _items = items;
    }
  }
}
