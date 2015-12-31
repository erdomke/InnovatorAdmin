using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  /// <summary>
  /// Class with metadata describing an ItemType
  /// </summary>
  public class ItemType
  {
    private List<string> _floatProps = new List<string>();
    private List<ItemType> _relationships = new List<ItemType>();
    private Dictionary<string, Property> _properties
      = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> ClassPaths { get; set; }
    /// <summary>
    /// List of the properties which reference versionable items and are set to float
    /// </summary>
    public IList<string> FloatProperties
    {
      get { return _floatProps; }
    }
    /// <summary>
    /// ID of the item type
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Whether the item type is marked as "core"
    /// </summary>
    public bool IsCore { get; set; }
    /// <summary>
    /// Whether the item type is marked as "dependent"
    /// </summary>
    public bool IsDependent { get; set; }
    /// <summary>
    /// Whether the item type is marked as "federated"
    /// </summary>
    public bool IsFederated { get; set; }
    /// <summary>
    /// Whether the item type is a polyitem type
    /// </summary>
    public bool IsPolymorphic { get; set; }
    /// <summary>
    /// Whether the item type is a relationship
    /// </summary>
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
    /// <summary>
    /// Whether the item type is versionable (automatic or manual)
    /// </summary>
    public bool IsVersionable { get; set; }
    /// <summary>
    /// Label of the item type
    /// </summary>
    public string Label { get; set; }
    /// <summary>
    /// Name of the item type
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Properties of the item type
    /// </summary>
    public IDictionary<string, Property> Properties { get { return _properties; } }
    /// <summary>
    /// <see cref="ItemReference"/> of the item type
    /// </summary>
    public ItemReference Reference { get; set; }
    /// <summary>
    /// List of child relationships of the item type
    /// </summary>
    public IList<ItemType> Relationships
    {
      get { return _relationships; }
    }
    /// <summary>
    /// Source item type (if this item type is a relationship)
    /// </summary>
    public ItemType Source { get; set; }
    /// <summary>
    /// Related item type (if this item type is a relationship)
    /// </summary>
    public ItemType Related { get; set; }
    public string TabLabel { get; set; }

    /// <summary>
    /// Whether the given object is equal to this one
    /// </summary>
    public override bool Equals(object obj)
    {
      var it = obj as ItemType;
      if (it == null) return false;
      return Equals(it);
    }
    /// <summary>
    /// Whether the given object is equal to this one
    /// </summary>
    public bool Equals(ItemType obj)
    {
      return (this.Id ?? "").Equals(obj.Id);
    }
    /// <summary>
    /// Gets the hash code for the item
    /// </summary>
    public override int GetHashCode()
    {
      return (this.Id ?? "").GetHashCode();
    }

  }
}
