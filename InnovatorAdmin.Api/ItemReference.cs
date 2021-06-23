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
    public static ItemReference FromFullItem(IReadOnlyItem elem, bool getKeyedName)
    {
      var result = new ItemReference();
      FillItemRef(result, elem, getKeyedName);
      return result;
    }

    internal static void FillItemRef(ItemReference result, IReadOnlyItem elem, bool getKeyedName)
    {
      result.Type = elem.Type().Value;
      if (elem.Attribute("id").Exists)
      {
        result.Unique = elem.Attribute("id").Value;
      }
      else if (elem.Attribute("where").Exists)
      {
        result.Unique = elem.Attribute("where").Value;
      }
      if (getKeyedName)
      {
        if (result.Type == "FileType" && elem.Property("mimetype").HasValue())
        {
          result.KeyedName = elem.Property("extension").AsString("") + ", "
            + elem.Property("mimetype").AsString("");
        }
        else
        {
          IReadOnlyProperty_Base node = elem.Property("id");
          if (node.Exists && node.KeyedName().Exists)
          {
            result.KeyedName = node.KeyedName().Value;
          }
          else
          {
            result.KeyedName = node.Attribute("_keyed_name").Value
                            ?? elem.KeyedName().AsString(null)
                            ?? elem.Property("name").AsString(null);
          }

          node = elem.SourceId();
          if (node.Exists && node.KeyedName().Exists)
          {
            if (result.KeyedName.IsGuid())
            {
              var related = elem.RelatedId();
              if (related.Exists && related.KeyedName().Exists)
              {
                result.KeyedName = node.Attribute("keyed_name").Value + " > " + related.Attribute("keyed_name").Value;
              }
              else
              {
                result.KeyedName = node.Attribute("keyed_name").Value + ": " + result.KeyedName;
              }
            }
            else if (!string.IsNullOrEmpty(node.Attribute("keyed_name").Value))
            {
              result.KeyedName += " {" + node.Attribute("keyed_name").Value + "}";
            }
          }
        }
      }
    }
    public static ItemReference FromFullItem(XmlElement elem, bool getKeyedName)
    {
      var result = new ItemReference();
      result.Type = elem.Attributes["type"].Value;
      if (elem.HasAttribute("id"))
      {
        result.Unique = elem.Attributes["id"].Value;
      }
      else if (elem.HasAttribute("where"))
      {
        result.Unique = elem.Attributes["where"].Value;
      }
      if (getKeyedName)
      {
        if (result.Type == "FileType" && !string.IsNullOrEmpty(elem.Element("mimetype", null)))
        {
          result.KeyedName = elem.Element("extension", "") + ", " + elem.Element("mimetype", "");
        }
        else if (result.Type == "Identity" && !string.IsNullOrEmpty(elem.Element("name", null)))
        {
          result.KeyedName = elem.Element("name", "");
        }
        else
        {
          var node = elem.Elements(e => e.LocalName == "id" && e.HasAttribute("keyed_name")).SingleOrDefault();
          if (node != null)
          {
            result.KeyedName = node.Attribute("keyed_name");
          }
          else
          {
            result.KeyedName = elem.Attribute("_keyed_name", null)
                            ?? elem.Element("keyed_name", null)
                            ?? elem.Element("name", null);
          }

          node = elem.Elements(e => e.LocalName == "source_id" && e.HasAttribute("keyed_name")).SingleOrDefault();
          if (node != null)
          {
            if (result.KeyedName.IsGuid())
            {
              var related = elem.Elements(e => e.LocalName == "related_id" && e.HasAttribute("keyed_name")).SingleOrDefault();
              if (related == null)
              {
                result.KeyedName = node.Attribute("keyed_name") + ": " + result.KeyedName;
              }
              else
              {
                result.KeyedName = node.Attribute("keyed_name") + " > " + related.Attribute("keyed_name");
              }
            }
            else if (!string.IsNullOrEmpty(node.Attribute("keyed_name")))
            {
              result.KeyedName += " {" + node.Attribute("keyed_name") + "}";
            }
          }
        }
      }

      return result;
    }
    public static IEnumerable<ItemReference> FromFullItems(XmlElement elem, bool getKeyedName)
    {
      var node = elem;
      while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
      if (node == null) return Enumerable.Empty<ItemReference>();
      return (from e in node.ParentNode.ChildNodes.OfType<XmlElement>()
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
