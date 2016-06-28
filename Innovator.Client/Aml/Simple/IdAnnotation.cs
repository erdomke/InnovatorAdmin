using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  class IdAnnotation : IReadOnlyProperty, IReadOnlyAttribute, ILinkedAnnotation
  {
    private ILinkedAnnotation _next;
    private IReadOnlyElement _parent;
    private Guid _value;

    public ElementFactory AmlContext
    {
      get { return _parent == null ? ElementFactory.Local : _parent.AmlContext; }
    }
    public bool Exists { get { return _parent != null; } }
    public string Name { get { return "id"; } }
    public ILinkedAnnotation Next
    {
      get { return _next; }
      set { _next = value; }
    }
    public IReadOnlyElement Parent { get { return _parent ?? Item.NullItem; } }
    public string Value { get { return _value.ToArasId(); } }

    public IdAnnotation(IReadOnlyElement parent, Guid value)
    {
      _parent = parent;
      _value = value;
    }

    public Guid? AsGuid()
    {
      return _value;
    }
    public Guid AsGuid(Guid defaultValue)
    {
      return _value;
    }
    public string AsString(string defaultValue)
    {
      return this.Value;
    }

    #region IReadOnlyValue

    bool? IReadOnlyValue.AsBoolean()
    {
      throw new NotSupportedException();
    }
    bool IReadOnlyValue.AsBoolean(bool defaultValue)
    {
      throw new NotSupportedException();
    }
    DateTime? IReadOnlyValue.AsDateTime()
    {
      throw new NotSupportedException();
    }
    DateTime IReadOnlyValue.AsDateTime(DateTime defaultValue)
    {
      throw new NotSupportedException();
    }
    DateTime? IReadOnlyValue.AsDateTimeUtc()
    {
      throw new NotSupportedException();
    }
    DateTime IReadOnlyValue.AsDateTimeUtc(DateTime defaultValue)
    {
      throw new NotSupportedException();
    }
    double? IReadOnlyValue.AsDouble()
    {
      throw new NotSupportedException();
    }
    double IReadOnlyValue.AsDouble(double defaultValue)
    {
      throw new NotSupportedException();
    }
    int? IReadOnlyValue.AsInt()
    {
      throw new NotSupportedException();
    }
    int IReadOnlyValue.AsInt(int defaultValue)
    {
      throw new NotSupportedException();
    }
    IReadOnlyItem IReadOnlyProperty.AsItem()
    {
      return _parent as IReadOnlyItem;
    }
    long? IReadOnlyValue.AsLong()
    {
      throw new NotSupportedException();
    }
    long IReadOnlyValue.AsLong(long defaultValue)
    {
      throw new NotSupportedException();
    }
    #endregion

    public IReadOnlyAttribute Attribute(string name)
    {
      return Attributes().FirstOrDefault(a => a.Name == name)
        ?? Client.Attribute.NullAttr;
    }

    public IEnumerable<IReadOnlyAttribute> Attributes()
    {
      var item = _parent as IReadOnlyItem;
      if (item != null)
      {
        var type = item.Type();
        if (type.Exists)
          yield return type;
        var keyedName = item.KeyedName();
        if (keyedName.Exists)
        {
          var result = new Attribute("keyed_name", keyedName.Value);
          result.Next = result;
          yield return result;
        }
      }
    }

    public IEnumerable<IReadOnlyElement> Elements()
    {
      return Enumerable.Empty<IReadOnlyElement>();
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      writer.WriteStartElement("id");
      foreach (var attr in Attributes())
      {
        writer.WriteAttributeString(attr.Name, attr.Value);
      }
      writer.WriteString(this.Value);
      writer.WriteEndElement();
    }
  }
}
