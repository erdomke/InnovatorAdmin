using Innovator.Client;

namespace InnovatorAdmin.Documentation
{
  public class StateNode
  {
    public string Id { get; }
    public string Name { get; }
    public string Label { get; }
    public Point Location { get; }
    public NodeType Type { get; set; }
    public bool IsAutomatic { get; }
    public string Image { get; }

    public StateNode(IReadOnlyItem item)
    {
      Id = item.Id();
      Name = item.Property("name").Value;
      Label = item.Property("label").Value;
      Location = new Point(item.Property("x").AsInt(0), item.Property("y").AsInt(0));
      if (item.Property("is_start").AsBoolean(false))
        Type = NodeType.Start;
      else if (item.Property("is_end").AsBoolean(false))
        Type = NodeType.End;
      else
        Type = NodeType.General;
      IsAutomatic = item.Property("is_auto").AsBoolean(false);
      Image = item.Property("image").Value;
    }
  }
}
