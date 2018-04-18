using Innovator.Client;
using Innovator.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Property : IListValue
  {
    private List<string> _restrictions = new List<string>();

    public string Id { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
    public string DataSource { get; set; }
    public List<string> Restrictions
    {
      get
      {
        return _restrictions;
      }
    }
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
    public object DefaultValue { get; set; }
    public int ColumnWidth { get; set; }
    public bool IsRequired { get; set; }
    public bool ReadOnly { get; set; }

    public Property()
    {
      // Do Nothing
    }
    public Property(string name)
    {
      this.Name = name;
    }
    public Property(string name, PropertyType type)
    {
      this.Name = name;
      this.Type = type;
    }

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

    internal static Property FromItem(IReadOnlyItem prop, ItemType type)
    {
      var newProp = new Property(prop.Property("name").Value);
      newProp.Id = prop.Id();
      newProp.Label = prop.Property("label").Value;
      newProp.SetType(prop.Property("data_type").Value);
      newProp.Precision = prop.Property("prec").AsInt(-1);
      newProp.Scale = prop.Property("scale").AsInt(-1);
      newProp.StoredLength = prop.Property("stored_length").AsInt(-1);
      var foreign = prop.Property("foreign_property").AsItem();
      if (foreign.Exists)
      {
        newProp.ForeignLinkPropName = prop.Property("data_source").KeyedName().Value;
        newProp.ForeignPropName = foreign.Property("name").Value;
        newProp.ForeignTypeName = foreign.SourceId().KeyedName().Value;
      }
      newProp.DataSource = prop.Property("data_source").Value;
      if (newProp.Type == PropertyType.item && newProp.Name == "data_source" && type.Name == "Property")
      {
        newProp.Restrictions.AddRange(new string[] { "ItemType", "List", "Property" });
      }
      else if (newProp.Type == PropertyType.item && prop.Property("data_source").Attribute("name").HasValue())
      {
        newProp.Restrictions.Add(prop.Property("data_source").Attribute("name").Value);
      }
      newProp.Visibility =
        (prop.Property("is_hidden").AsBoolean(false) ? PropertyVisibility.None : PropertyVisibility.MainGrid)
        | (prop.Property("is_hidden2").AsBoolean(false) ? PropertyVisibility.None : PropertyVisibility.RelationshipGrid);
      newProp.SortOrder = prop.Property("sort_order").AsInt(int.MaxValue);
      newProp.ColumnWidth = prop.Property("column_width").AsInt(100);
      newProp.IsRequired = prop.Property("is_required").AsBoolean(false);
      newProp.ReadOnly = prop.Property("readonly").AsBoolean(false);

      return newProp;
    }
  }
}
