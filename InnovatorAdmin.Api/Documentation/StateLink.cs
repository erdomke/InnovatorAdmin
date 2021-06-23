using Innovator.Client;
using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin.Documentation
{
  public class StateLink
  {
    public string Id { get; }
    public string Name { get; }
    public string Label { get; }
    public List<Point> Segments { get; } = new List<Point>();
    public StateNode Source { get; }
    public StateNode Related { get; }
    public int ExecutionCount { get; set; } = 1;
    public bool IsDefault { get; }
    public bool IsOverride { get; }

    public StateLink(StateNode source, StateNode related, string name)
    {
      Source = source;
      Related = related;
      Name = name;
    }

    public StateLink(IReadOnlyItem item, Dictionary<string, StateNode> nodes)
    {
      Id = item.Id();
      Label = item.Property("label").Value;
      Name = item.Property("name").Value
        ?? item.Property("role").KeyedName().Value;
      if (item.Property("segments").HasValue())
      {
        Segments.AddRange(item.Property("segments").Value
          .Split('|')
          .Select(p => p.Split(','))
          .Select(p => new Point(int.Parse(p[0]), int.Parse(p[1]))));
      }

      if (nodes.TryGetValue(item.Property("from_state").Value ?? item.SourceItem().Id() ?? "", out var sourceNode))
        Source = sourceNode;
      if (nodes.TryGetValue(item.Property("to_state").Value ?? item.RelatedId().Value ?? "", out var relatedNode))
        Related = relatedNode;

      IsDefault = item.Property("is_default").AsBoolean(false);
      IsOverride = item.Property("is_override").AsBoolean(false);
    }
  }
}
