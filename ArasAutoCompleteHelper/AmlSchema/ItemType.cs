using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.AutoComplete.AmlSchema
{
  class ItemType
  {
    private PropertyCollection _properties = new PropertyCollection();
    private List<ItemType> _relationships = new List<ItemType>();

    public bool IsRelationship
    {
      get
      {
        return Source != null;
      }
    }
    public string Name { get; set; }
    public ItemType Source { get; set; }
    public ItemType Related { get; set; }
    public PropertyCollection Properties
    {
      get
      {
        return _properties;
      }
    }
    public List<ItemType> Relationships
    {
      get
      {
        return _relationships;
      }
    }

    public ItemType()
    {
      // Do Nothing
    }
    public ItemType(string name)
    {
      this.Name = name;
    }
    public ItemType(string name, ItemType source, ItemType related)
    {
      this.Name = name;
      this.Source = source;
      this.Related = related;
    }
  }
}
