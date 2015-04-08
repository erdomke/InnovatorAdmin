using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class ItemProperty : IEquatable<ItemProperty>
  {
    public string ItemType { get; set; }
    public string ItemTypeId { get; set; }
    public string Property { get; set; }
    public string PropertyId { get; set; }

    public override bool Equals(object obj)
    {
      var prop = obj as ItemProperty;
      if (prop == null) return false;
      return ((IEquatable<ItemProperty>)this).Equals(prop);
    }
    bool IEquatable<ItemProperty>.Equals(ItemProperty other)
    {
      return Utils.StringEquals(this.ItemType, other.ItemType)
        && Utils.StringEquals(this.Property, other.Property);
    }
    public override int GetHashCode()
    {
      return (this.ItemType ?? "").GetHashCode() ^ (this.Property ?? "").GetHashCode();
    }
    public override string ToString()
    {
      return (this.ItemType ?? (this.ItemTypeId ?? "")) + ": " + this.Property;
    }
  }
}
