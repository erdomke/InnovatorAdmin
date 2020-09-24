using System.Collections.Generic;

namespace InnovatorAdmin.Documentation
{
  public class Entity
  {
    public List<EntityAttribute> Attributes { get; } = new List<EntityAttribute>();
    public string Id { get; set; }
    public string Label { get; set; }
    public string Note { get; set; }
    public string Package { get; set; }
    public string Stereotype { get; set; }
    public EntityType Type { get; set; }
  }
}
