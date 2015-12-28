using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Property
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
  }
}
