using Innovator.Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class StateDiagram : IDiagram
  {
    public string Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }

    public Dictionary<string, StateNode> Nodes { get; }
    public List<StateLink> Links { get; }

    public StateDiagram(IReadOnlyItem item)
    {
      Id = item.Id();
      Type = item.TypeName();
      Name = item.Property("name").Value
        ?? item.KeyedName().Value;
      Label = item.Property("label").Value;
      Nodes = item.Relationships("Life Cycle State")
        .Concat(item.Relationships("Workflow Map Activity")
          .Select(i => i.RelatedItem()))
        .Select(i => new StateNode(i))
        .ToDictionary(n => n.Id);
      if (item.Property("start_state").HasValue()
        && Nodes.TryGetValue(item.Property("start_state").Value, out var startState))
      {
        startState.Type = NodeType.Start;
      }
      Links = item.Relationships("Life Cycle Transition")
        .Concat(item.Relationships("Workflow Map Activity")
          .SelectMany(i => i.RelatedItem().Relationships("Workflow Map Path")))
        .Select(i => new StateLink(i, Nodes))
        .ToList();
    }

    public Task WriteAsync<T>(IDiagramWriter<T> writer, T target)
    {
      return writer.WriteAsync(this, target);
    }
  }
}
