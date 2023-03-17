using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin
{
  public class ItemReference : IComparable<ItemReference>, IComparable, IEquatable<ItemReference>, ICloneable
  {
    private string _unique;
    private string _baseUnique;
    private string _type;

    public string KeyedName { get; set; }
    // Used when understanding dependencies
    public ItemReference Origin { get; set; }
    public string Type
    {
      get { return _type; }
      set { _type = value; }
    }
    public string Unique
    {
      get { return _unique; }
      set
      {
        _unique = value;
        if (_unique.IndexOf("[config_id] = '") > 0)
          _baseUnique = _unique.Substring(_unique.Length - 33, 32);
      }
    }

    // Used with exporting
    public int Levels { get; set; }
    public SystemPropertyGroup SystemProps { get; set; }

    public ItemReference()
    {
      this.Levels = -1;
      this.SystemProps = SystemPropertyGroup.None;
    }
    public ItemReference(string type, string unique) : this()
    {
      this.Type = type;
      this.Unique = unique;
    }

    public static ItemReference FromElement(XmlElement elem)
    {
      return elem.LocalName == "Item" ?
        ItemReference.FromFullItem(elem, false) :
        ItemReference.FromItemProp(elem);
    }

    public static ItemReference FromItemProp(XmlElement elem)
    {
      if (elem.Elements("Item").Any())
        return FromFullItem(elem.Element("Item"), false);

      return new ItemReference(elem.Attributes["type"].Value, elem.InnerText)
      {
        KeyedName = (elem.HasAttribute("keyed_name") ? elem.Attributes["keyed_name"].Value : "")
      };
    }

    public static ItemReference FromElement(System.Xml.Linq.XElement elem)
    {
      return elem.Name.LocalName == "Item" ?
        ItemReference.FromFullItem(elem, false) :
        ItemReference.FromItemProp(elem);
    }

    public static ItemReference FromItemProp(System.Xml.Linq.XElement elem)
    {
      if (elem.Elements("Item").Any())
        return FromFullItem(elem.Element("Item"), false);

      return new ItemReference((string)elem.Attribute("type"), (string)elem)
      {
        KeyedName = (string)elem.Attribute("keyed_name") ?? ""
      };
    }

    public static ItemReference FromFullItem(IReadOnlyItem elem, bool getKeyedName)
    {
      var result = new ItemReference();
      FillItemRef(result, elem, getKeyedName);
      return result;
    }

    internal static void FillItemRef(ItemReference result, IReadOnlyItem elem, bool getKeyedName)
    {
      FillItemRef(result, elem, getKeyedName
        , (p, a) => p.Attribute(a).AsString(null)
        , (p, e) => p.Property(e).AsString(null)
        , (p, e, a) => p.Property(e).Attribute(a).AsString(null));
    }

    private static void FillItemRef<T>(ItemReference result, T elem, bool getKeyedName
      , Func<T, string, string> getAttribute
      , Func<T, string, string> getElement
      , Func<T, string, string, string> getElementAttribute)
    {
      result.Type = getAttribute(elem, "type");
      result.Unique = getAttribute(elem, "id")
        ?? getAttribute(elem, "where")
        ?? "";

      if (getKeyedName)
      {
        if (result.Type == "FileType"
          && !string.IsNullOrEmpty(getElement(elem, "mimetype")))
        {
          result.KeyedName = (getElement(elem, "extension") ?? "") + ", "
            + (getElement(elem, "mimetype") ?? "");
        }
        else if (result.Type == "Identity"
          && !string.IsNullOrEmpty(getElement(elem, "name")))
        {
          result.KeyedName = (getElement(elem, "name") ?? "");
        }
        else
        {
          result.KeyedName = getElementAttribute(elem, "id", "keyed_name")
            ?? getAttribute(elem, "_keyed_name")
            ?? getElement(elem, "keyed_name")
            ?? getElement(elem, "name");

          var sourceKeyedName = getElementAttribute(elem, "source_id", "keyed_name");
          if (!string.IsNullOrEmpty(sourceKeyedName))
          {
            if (result.KeyedName.IsGuid())
            {
              var relatedKeyedName = getElementAttribute(elem, "related_id", "keyed_name");
              if (!string.IsNullOrEmpty(relatedKeyedName))
                result.KeyedName = sourceKeyedName + " > " + relatedKeyedName;
              else
                result.KeyedName = sourceKeyedName + ": " + result.KeyedName;
            }
            else 
            {
              result.KeyedName += " {" + sourceKeyedName + "}";
            }
          }
        }
      }
    }

    public static ItemReference FromFullItem(XmlElement elem, bool getKeyedName)
    {
      var result = new ItemReference();
      FillItemRef(result, elem, getKeyedName
        , (p, a) => p.Attribute(a, null)
        , (p, e) => p.Element(e, null)
        , (p, e, a) => p.Element(e)?.Attribute(a, null));
      return result;
    }

    public static ItemReference FromFullItem(System.Xml.Linq.XElement elem, bool getKeyedName)
    {
      var result = new ItemReference();
      FillItemRef(result, elem, getKeyedName
        , (p, a) => (string)p.Attribute(a)
        , (p, e) => (string)p.Element(e)
        , (p, e, a) => (string)p.Element(e)?.Attribute(a));
      return result;
    }

    public static IEnumerable<ItemReference> FromFullItems(XmlElement elem, bool getKeyedName)
    {
      var node = elem;
      while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
      if (node == null) return Enumerable.Empty<ItemReference>();
      return (from e in node.ParentNode.NextSiblingsAndSelf().SelectMany(s => s.ChildNodes.OfType<XmlElement>())
              where e.LocalName == "Item"
              select FromFullItem(e, getKeyedName));
    }

    public override bool Equals(object obj)
    {
      var itemRef = obj as ItemReference;
      if (itemRef == null) return false;
      return ((IEquatable<ItemReference>)this).Equals(itemRef);
    }

    bool IEquatable<ItemReference>.Equals(ItemReference itemRef)
    {
      return string.Equals(_baseUnique ?? _unique, itemRef._baseUnique ?? itemRef._unique, StringComparison.OrdinalIgnoreCase)
        && string.Equals(this.Type, itemRef.Type, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
      return (_baseUnique ?? _unique ?? "").ToUpperInvariant().GetHashCode()
        ^ (this.Type ?? "").ToUpperInvariant().GetHashCode();
    }

    public override string ToString()
    {
      return (this.Type ?? "") + ": " +
        (string.IsNullOrEmpty(this.KeyedName) ?
          (this.Unique ?? "") :
          this.KeyedName
        );
    }

    public int CompareTo(ItemReference other)
    {
      var compare = this.Type.ToUpperInvariant().CompareTo(other.Type.ToUpperInvariant());
      if (compare == 0) compare = (this.KeyedName.ToUpperInvariant() ?? "").CompareTo(other.KeyedName.ToUpperInvariant() ?? "");
      if (compare == 0) compare = this.Unique.CompareTo(other.Unique);
      return compare;
    }

    public ItemReference Clone()
    {
      var result = new ItemReference();
      result.KeyedName = this.KeyedName;
      result.Origin = this.Origin;
      result._type = this._type;
      result._unique = this._unique;
      result._baseUnique = this._baseUnique;
      return result;
    }

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    public int CompareTo(object obj)
    {
      var itemRef = obj as ItemReference;
      if (itemRef == null) throw new ArgumentException();
      return CompareTo(itemRef);
    }
  }
}
