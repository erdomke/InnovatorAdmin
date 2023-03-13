using Innovator.Client;
using Innovator.Client.Model;
using InnovatorAdmin.Documentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  [DebuggerDisplay("{name}")]
  public class Property : IListValue
  {
    public bool Applicable { get; set; }
    public bool Core { get; set; }
    public string Id { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
    public string DataSource { get; set; }
    public List<string> Restrictions { get; } = new List<string>();
    public string TypeName { get; set; }
    public PropertyType Type { get; set; }
    public int StoredLength { get; set; }
    public int Precision { get; set; }
    public int Scale { get; set; }
    public string ForeignLinkPropName { get; set; }
    public string ForeignPropName { get; set; }
    public string ForeignTypeName { get; set; }
    public PropertyVisibility Visibility { get; set; }
    public int SortOrder { get; set; }
    public string DefaultSearch { get; set; }
    public object DefaultValue { get; set; }
    public int ColumnWidth { get; set; }
    public bool IsRequired { get; set; }
    public bool ReadOnly { get; set; }
    public string Description { get; set; }
    public int? KeyedNameOrder { get; set; }
    public string ClassPath { get; set; }
    public string Pattern { get; private set; }
    public int? OrderBy { get; private set; }
    public bool IsKeyed { get; private set; }

    private Property() { }
       
    public void SetType(string typeName)
    {
      this.TypeName = typeName;
      switch (typeName)
      {
        case "string":
        case "text":
        case "sequence":
          this.Type = PropertyType.text;
          break;
        case "integer":
        case "float":
        case "decimal":
          this.Type = PropertyType.number;
          break;
        case "date":
          this.Type = PropertyType.date;
          break;
        case "item":
          this.Type = PropertyType.item;
          break;
        case "list":
        case "filter list":
        case "color list":
          this.Type = PropertyType.list;
          break;
        case "boolean":
          this.Type = PropertyType.boolean;
          break;
        default:
          this.Type = PropertyType.unknown;
          break;
      }
    }

    public string TypeDisplay()
    {
      var builder = new StringBuilder(TypeName)
          .Append('[');
      if (Restrictions.Any())
      {
        builder.Append(Restrictions.First());
        foreach (var restriction in Restrictions.Skip(1))
          builder.Append(",").Append(restriction);
      }
      else if (StoredLength > 0)
      {
        builder.Append(StoredLength);
      }
      else if (Precision > 0 || Scale > 0)
      {
        builder.Append(Precision).Append(',').Append(Scale);
      }
      else if (!string.IsNullOrEmpty(ForeignLinkPropName))
      {
        builder.Append(ForeignLinkPropName).Append('.').Append(ForeignPropName);
      }

      if (IsRequired)
      {
        if (builder[builder.Length - 1] != '[')
          builder.Append(", ");
        builder.Append("not null");
      }
      if (builder[builder.Length - 1] == '[')
        builder.Remove(builder.Length - 1, 1);
      else
        builder.Append(']');
      return builder.ToString();
    }

    string IListValue.Value
    {
      get { return this.Name; }
    }

    public IPropertyDefinition ToItem(ElementFactory aml)
    {
      var item = aml.Item(aml.Type(Name.StartsWith("xp-") ? "xPropertyDefinition" : "Property"), aml.Id(Id)
        , aml.Property("name", Name)
        , aml.Property("label", Label)
        , aml.Property("data_type", TypeName)
        , aml.Property("data_source", DataSource)
        , aml.Property("is_required", IsRequired)
        , aml.Property("readonly", ReadOnly)
        , aml.Property("column_width", ColumnWidth)
      );
      if (Precision >= 0)
        item.Property("prec").Set(Precision);
      if (Scale >= 0)
        item.Property("scale").Set(Scale);
      if (StoredLength >= 0)
        item.Property("stored_length").Set(StoredLength);
      if (SortOrder < int.MaxValue)
        item.Property("sort_order").Set(SortOrder);
      return (IPropertyDefinition)item;
    }

    internal static Property FromItem(IReadOnlyItem prop, ItemType type, Func<string, string> getName = null)
    {
      var newProp = new Property()
      {
        Applicable = true,
        Name = prop.Property("name").Value,
        Id = prop.Id(),
        Label = prop.Property("label").Value,
        Precision = prop.Property("prec").AsInt(-1),
        Scale = prop.Property("scale").AsInt(-1),
        StoredLength = prop.Property("stored_length").AsInt(-1),
        Description = prop.Property("help_text").AsString(null) ?? prop.Property("help_tooltip").AsString(null),
        ClassPath = prop.Property("class_path").Value,
        DefaultSearch = prop.Property("default_search").Value,
        Pattern = prop.Property("pattern").Value,
        KeyedNameOrder = prop.Property("keyed_name_order").AsInt(),
        OrderBy = prop.Property("order_by").AsInt(),
        IsKeyed = prop.Property("is_keyed").AsBoolean(false)
      };
      if (_propertyHelp.TryGetValue(newProp.Name, out var stdDescription))
      {
        newProp.Core = true;
        newProp.Description = newProp.Description ?? stdDescription;
      }
      if (prop.Property("default_value").HasValue())
        newProp.DefaultValue = prop.Property("default_value").Value;

      newProp.SetType(prop.Property("data_type").Value);
      var foreign = prop.Property("foreign_property").AsItem();
      if (foreign.Exists)
      {
        newProp.ForeignLinkPropName = prop.Property("data_source").KeyedName().Value
          ?? prop.Property("data_source").AsItem().Property("name").Value;
        newProp.ForeignPropName = foreign.Property("name").Value
          ?? foreign.Property("keyed_name").Value;
        newProp.ForeignTypeName = foreign.SourceId().KeyedName().Value
          ?? foreign.SourceId().AsItem().Property("name").Value;
      }
      newProp.DataSource = prop.Property("data_source").Value;
      if (newProp.Type == PropertyType.item && newProp.Name == "data_source" && type.Name == "Property")
      {
        newProp.Restrictions.AddRange(new string[] { "ItemType", "List", "Property" });
      }
      else if (prop.Property("data_source").Attribute("name").HasValue()
        || prop.Property("data_source").KeyedName().HasValue())
      {
        newProp.Restrictions.Add(prop.Property("data_source").Attribute("name").Value
          ?? prop.Property("data_source").KeyedName().Value);
      }
      else if (prop.Property("data_source").HasValue() && !string.IsNullOrEmpty(prop.Property("data_source").Value))
      {
        var name = getName?.Invoke(prop.Property("data_source").Value);
        if (!string.IsNullOrEmpty(name))
          newProp.Restrictions.Add(name);
      }
      newProp.Visibility =
        (prop.Property("is_hidden").AsBoolean(false) ? PropertyVisibility.None : PropertyVisibility.MainGrid)
        | (prop.Property("is_hidden2").AsBoolean(false) ? PropertyVisibility.None : PropertyVisibility.RelationshipGrid);
      newProp.SortOrder = prop.Property("sort_order").AsInt(int.MaxValue);
      newProp.ColumnWidth = prop.Property("column_width").AsInt(100);
      newProp.IsRequired = prop.Property("is_required").AsBoolean(false);
      newProp.ReadOnly = prop.Property("readonly").AsBoolean(false);

      switch (newProp.Name)
      {
        case "classification":
          newProp.Applicable = type.ClassPaths.Any() || type.IsPolymorphic || type.IsFederated;
          break;
        case "config_id":
        case "generation":
        case "is_current":
        case "major_rev":
        case "minor_rev":
        case "new_version":
        case "release_date":
          newProp.Applicable = type.IsVersionable || type.IsPolymorphic || type.IsFederated;
          break;
        case "current_state":
        case "is_released":
        case "state":
          newProp.Applicable = type.HasLifeCycle || type.IsPolymorphic || type.IsFederated;
          break;
        case "related_id":
          newProp.Applicable = prop.Property("data_source").HasValue();
          break;
      }

      return newProp;
    }

    private static Dictionary<string, string> _propertyHelp = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
      { "classification", "Describes the type of the item with a tree-like structure (Inherited from Item)" },
      { "config_id", "Server assigned. Defines a common ID linking all the generations of an item (Inherited from Item)" },
      { "created_by_id", "Server assigned. ID of the user that created the item (Inherited from Item)" },
      { "created_on", "Server assigned. Date the item was created (Inherited from Item)" },
      { "css", "Defines styling within the web client (Inherited from Item)" },
      { "current_state", "Server assigned. ID of the current life cycle state (Inherited from Item)" },
      { "generation", "Server assigned. Sequential number identifying the version/snapshot of the item (Inherited from Item)" },
      { "id", "Server assigned. A 32 character globally unique identifier (GUID) for the item (Inherited from Item)" },
      { "is_current", "Server assigned. Boolean returning true if this is the most current generation (Inherited from Item)" },
      { "is_released", "Boolean returning true if this item is in a released life cycle state (Inherited from Item)" },
      { "keyed_name", "Server assigned. Human friendly identifier for the item (Inherited from Item)" },
      { "locked_by_id", "ID of the user that has locked the item (Inherited from Item)" },
      { "major_rev", "Revision label of the versioned item (Inherited from Item)" },
      { "managed_by_id", "ID of the identity considered the 'Manager' for permissions and workflow assignments (Inherited from Item)" },
      { "minor_rev", "Not used (Inherited from Item)" },
      { "modified_by_id", "Server assigned. ID of the user that last modified the item (Inherited from Item)" },
      { "modified_on", "Server assigned. Date the item was last modified (Inherited from Item)" },
      { "new_version", "Boolean returning true if a new version of this automatically versioned item has been created for the current lock-save-unlock cycle (Inherited from Item)" },
      { "not_lockable", "Boolean returning true if this item cannot be changed regardless of permissions (Inherited from Item)" },
      { "owned_by_id", "ID of the identity considered the 'Owner' for permissions and workflow assignments (Inherited from Item)" },
      { "permission_id", "ID of the current permission item (Inherited from Item)" },
      { "state", "Server assigned. Name of the current life cycle state (Inherited from Item)" },
      { "team_id", "ID of the current team (Inherited from Item)" },
      { "effective_date", "Date that this version is considered effective and can first be used (Inherited from Versioned Item)" },
      { "release_date", "Server assigned. Date that this version was first promoted to a released life cycle state (Inherited from Versioned Item)" },
      { "superseded_date", "Date that this version was superseded by the release of a more recent version (Inherited from Versioned Item)" },
      { "behavior", "Server assigned. Describes whether the related item will always point to the specified version or the most current version (Inherited from Relationship)" },
      { "source_id", "ID of the parent (source) item for the relationship (Inherited from Relationship)" },
      { "related_id", "ID of the child (related) item for the relationship (Inherited from Relationship)" },
      { "sort_order", "Number describing the order items should be sorted (Inherited from Relationship)" },
      { "indexed_on", "Server assigned. Date the item was last indexed by the Enterprise Search indexer (Inherited from Indexed Item)" },
      { "itemtype", "ID of the item type this item belongs to (Inherited from Poly Item)" }
    };

    public IEnumerable<AmlTypeDefinition> GetTypeDefinitions()
    {
      switch (Type)
      {
        case PropertyType.boolean:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.Boolean) };
        case PropertyType.date:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.Date) };
        case PropertyType.item:
          return Restrictions.Select(r => AmlTypeDefinition.FromDefinition(AmlDataType.Item, r));
        case PropertyType.list:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.List, DataSource) };
        case PropertyType.number:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.Float) };
        case PropertyType.text:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.String) };
        default:
          return new[] { AmlTypeDefinition.FromDefinition(AmlDataType.Unknown) };
      }
    }
  }
}
