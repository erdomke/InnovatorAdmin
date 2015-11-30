using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  class Property
  {
    private List<string> _restrictions = new List<string>();

    public string Name { get; set; }
    public List<string> Restrictions
    {
      get
      {
        return _restrictions;
      }
    }
    public PropertyType Type { get; set; }

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
        default:
          this.Type = PropertyType.unknown;
          break;
      }
    }
  }
}
