using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class ServerEvent
  {
    public string Event { get; }
    public ItemReference Method { get; }
    public int SortOrder { get; }

    public ServerEvent(IReadOnlyItem serverEvent)
    {
      Event = serverEvent.Property("server_event").Value;
      var related = serverEvent.RelatedId();
      Method = related.AsItem().Exists
        ? ItemReference.FromFullItem(related.AsItem(), true)
        : new ItemReference(related.Type().Value, related.Value)
        {
          KeyedName = related.KeyedName().Value
        };
      SortOrder = serverEvent.Property("sort_order").AsInt(0);
    }
  }
}
