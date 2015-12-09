using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class ItemType
  {
    private List<string> _floatProps = new List<string>();
    private List<ItemType> _relationships = new List<ItemType>();
    private Dictionary<string, Property> _properties
      = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);

    public IList<string> FloatProperties
    {
      get { return _floatProps; }
    }
    public string Id { get; set; }
    public bool IsCore { get; set; }
    public bool IsDependent { get; set; }
    public bool IsFederated { get; set; }
    public bool IsPolymorphic { get; set; }
    public bool IsRelationship
    {
      get
      {
        return Source != null;
      }
    }
    /// <summary>
    /// Indicates whether the ItemType search results are automatically sorted as evideneced by
    /// having one or more properties with a defined order_by
    /// </summary>
    public bool IsSorted { get; set; }
    public bool IsVersionable { get; set; }
    public string Name { get; set; }
    public IDictionary<string, Property> Properties { get { return _properties; } }
    public ItemReference Reference { get; set; }
    public IList<ItemType> Relationships
    {
      get { return _relationships; }
    }

    public ItemType Source { get; set; }
    public ItemType Related { get; set; }

    public override bool Equals(object obj)
    {
      var it = obj as ItemType;
      if (it == null) return false;
      return Equals(it);
    }
    public bool Equals(ItemType obj)
    {
      return (this.Id ?? "").Equals(obj.Id);
    }
    public override int GetHashCode()
    {
      return (this.Id ?? "").GetHashCode();
    }

  }
}
