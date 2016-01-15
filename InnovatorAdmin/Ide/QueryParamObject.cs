using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  class QueryParamGroup : ICustomTypeDescriptor
  {
    private List<QueryParameter> _items;

    public IList<QueryParameter> Items { get { return _items; } }

    public QueryParamGroup()
    {
      _items = new List<QueryParameter>();
    }

    public AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    public string GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    public string GetComponentName()
    {
      return TypeDescriptor.GetComponentName(this, true);
    }

    public TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      return null;
    }

    public object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    public EventDescriptorCollection GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      var pds = new PropertyDescriptor[_items.Count];

      for (var i = 0; i < _items.Count; i++)
      {
        pds[i] = new QueryParamDescriptor(_items[i]);
      }


      return new PropertyDescriptorCollection(pds);
    }

    public PropertyDescriptorCollection GetProperties()
    {
      return this.GetProperties(new Attribute[0]);
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      return _items;
    }
  }
}
