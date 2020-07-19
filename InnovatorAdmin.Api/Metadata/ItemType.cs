using Innovator.Client;
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
    public string Id { get; }

    /// <summary>
    /// Whether the item type is marked as "core"
    /// </summary>
    public bool IsCore { get; }

    /// <summary>
    /// Whether the item type is marked as "dependent"
    /// </summary>
    public bool IsDependent { get; }

    /// <summary>
    /// Whether the item type is marked as "federated"
    /// </summary>
    public bool IsFederated { get; }

    /// <summary>
    /// Whether the item type is a polyitem type
    /// </summary>
    public bool IsPolymorphic { get; }

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
    public bool IsVersionable { get; }

    /// <summary>
    /// Label of the item type
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Name of the item type
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Properties of the item type
    /// </summary>
    public IDictionary<string, Property> Properties { get { return _properties; } }

    /// <summary>
    /// <see cref="ItemReference"/> of the item type
    /// </summary>
    public ItemReference Reference { get; }

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
    public IEnumerable<string> States { get; set; }
    public string Description { get; }

    public ItemType(IReadOnlyItem itemType, HashSet<string> coreIds = null)
    {
      Id = itemType.Id();
      IsCore = itemType.Property("core").AsBoolean(coreIds?.Contains(itemType.Id()) == true);
      IsDependent = itemType.Property("is_dependent").AsBoolean(false);
      IsFederated = itemType.Property("implementation_type").Value == "federated";
      IsPolymorphic = itemType.Property("implementation_type").Value == "polymorphic";
      IsVersionable = itemType.Property("is_versionable").AsBoolean(false);
      Label = itemType.Property("label").Value;
      Name = itemType.Property("name").Value
        ?? itemType.KeyedName().Value
        ?? itemType.IdProp().KeyedName().Value;
      Reference = ItemReference.FromFullItem(itemType, true);
      Description = itemType.Property("description").Value;
    }

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
