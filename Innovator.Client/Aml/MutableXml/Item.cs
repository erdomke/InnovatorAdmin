using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Item : Element, IItem
  {
    public override string Value
    {
      get
      {
        return base.Value;
      }
    }

    internal Item(ElementFactory factory, params object[] content)
      : base(factory, "Item", content) { }
    internal Item(ElementFactory factory, XmlElement node)
      : base(factory, node) { }

    public override IElement Add(object content)
    {
      var item = content as Item;
      if (item != null)
      {
        var relationships = _node.ChildNodes.OfType<XmlElement>()
                                  .SingleOrDefault(e => e.LocalName == "Relationships");
        if (relationships == null)
          relationships = (XmlElement)_node.AppendChild(_node.OwnerDocument.CreateElement("Relationships"));
        relationships.AppendChild(_node.GetLocalNode(item._node));
        return this;
      }
      return base.Add(content);
    }
    public IReadOnlyResult AsResult()
    {
      return new Result(_factory, _node);
    }

    public IItem Clone()
    {
      return (IItem)((ICloneable)this).Clone();
    }
    public string Id()
    {
      if (!this.Exists) return null;
      IAttribute attr;
      IElement elem;
      if (base.TryGetAttribute("id", out attr))
      {
        return attr.Value;
      }
      else if (base.TryGetElement("id", out elem))
      {
        return elem.Value;
      }
      else
      {
        return null;
      }
    }
    public IProperty Property(string name)
    {
      return Property(name, null);
    }
    public IProperty Property(string name, string lang)
    {
      if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
      if (!this.Exists) return Innovator.Client.Property.NullProperty;

      var propNode = _node.ChildNodes.OfType<XmlElement>()
                          .SingleOrDefault(e => e.LocalName == name
                          && e.NamespaceURI == "http://www.aras.com/I18N"
                          && e.GetAttribute("xml:lang") == lang);
      var langSearch = lang ?? _factory.LocalizationContext.LanguageCode;
      if (propNode == null)
      {
        propNode = _node.ChildNodes.OfType<XmlElement>()
                        .FirstOrDefault(e => e.LocalName == name
                          && (!e.HasAttribute("xml:lang")
                            || (e.GetAttribute("xml:lang") == langSearch
                              && langSearch == _factory.LocalizationContext.LanguageCode)));
      }
      if (propNode == null) return (this.Exists ?
         _factory.PropertyTemplate(name, lang, _node) :
         Innovator.Client.Property.NullProperty);
      return _factory.ElementFromXml(propNode) as Property;
    }
    public IEnumerable<IItem> Relationships()
    {
      var relTag = RelationshipTag();
      if (relTag == null) return Enumerable.Empty<IItem>();
      return relTag.ChildNodes.OfType<XmlElement>()
                   .Select(e => _factory.ElementFromXml(e)).Cast<IItem>();
    }
    public IEnumerable<IItem> Relationships(string type)
    {
      var relTag = RelationshipTag();
      if (relTag == null) return Enumerable.Empty<IItem>();
      return relTag.ChildNodes.OfType<XmlElement>().Where(e => e.GetAttribute("type") == type)
                   .Select(e => _factory.ElementFromXml(e)).Cast<IItem>();
    }

    public override bool Equals(object obj)
    {
      var item = obj as Item;
      if (item == null) return false;
      return Equals(item);
    }
    public bool Equals(Item obj)
    {
      if (!this.Exists && !obj.Exists)
      {
        return true;
      }
      else if (this.Exists && obj.Exists)
      {
        if (!string.IsNullOrEmpty(this._node.GetAttribute("type"))
          && !string.IsNullOrEmpty(this._node.GetAttribute("id"))
          && !string.IsNullOrEmpty(obj._node.GetAttribute("type"))
          && !string.IsNullOrEmpty(obj._node.GetAttribute("id")))
        {
          return this._node.GetAttribute("type").Equals(obj._node.GetAttribute("type"))
            && this._node.GetAttribute("id").Equals(obj._node.GetAttribute("id"));
        }
        else
        {
          return this._node.OuterXml.Equals(obj._node.OuterXml);
        }
      }
      else
      {
        return false;
      }
    }
    public override int GetHashCode()
    {
      if (!this.Exists) return 0;
      return (!string.IsNullOrEmpty(this._node.GetAttribute("type"))
              && !string.IsNullOrEmpty(this._node.GetAttribute("id")) ?
        this._node.GetAttribute("type").GetHashCode() ^ this._node.GetAttribute("id").GetHashCode() :
        this._node.OuterXml.GetHashCode());
    }

    private XmlElement RelationshipTag()
    {
      if (!this.Exists) return null;
      return _node.ChildNodes.OfType<XmlElement>()
                  .SingleOrDefault(e => e.LocalName == "Relationships"
                    && e.ChildNodes.OfType<XmlElement>().Any());
    }

    public static implicit operator string(Item value)
    {
      return value._node.OuterXml;
    }

    internal static readonly Item NullItem = new Item(null, (XmlElement)null);

    IReadOnlyProperty IReadOnlyItem.Property(string name)
    {
      return this.Property(name);
    }
    IReadOnlyProperty IReadOnlyItem.Property(string name, string lang)
    {
      return this.Property(name, lang);
    }

    IEnumerable<IReadOnlyItem> IReadOnlyItem.Relationships()
    {
      return this.Relationships().Cast<IReadOnlyItem>();
    }

    IEnumerable<IReadOnlyItem> IReadOnlyItem.Relationships(string type)
    {
      return this.Relationships(type).Cast<IReadOnlyItem>();
    }

#if NET4
    public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
    {
      this.Property(binder.Name).Set(value);
      return true;
    }
    public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
    {
      result = null;
      var prop = this.Property(binder.Name);
      if (!prop.Exists) return false;
      result = prop;
      return true;
    }
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return _node.ChildNodes.OfType<XmlElement>()
        .Where(e => e.LocalName != "Relationships")
        .Select(e => e.LocalName);
    }
#endif

    public string TypeName()
    {
      return this.Type().Value;
    }
  }
}
