using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class ItemType
  {
    private List<string> _floatProps = new List<string>();
    private List<ItemType> _relationships = new List<ItemType>();

    public IList<string> FloatProperties
    {
      get { return _floatProps; }
    }
    public string Id { get; set; }
    public bool IsCore { get; set; }
    public bool IsDependent { get; set; }
    public bool IsFederated { get; set; }
    public bool IsPolymorphic { get; set; }
    public bool IsVersionable { get; set; }
    public string Name { get; set; }
    public ItemReference Reference { get; set; }
    public IList<ItemType> Relationships
    {
      get { return _relationships; }
    }

    public override bool Equals(object obj)
    {
      var it = obj as ItemType;
      if (it == null) return false;
      return Equals(it);
    }
    public bool Equals(ItemType obj)
    {
      return (this.Id ?? "").Equals(obj.Id);
    }
    public override int GetHashCode()
    {
      return (this.Id ?? "").GetHashCode();
    }

  }
}
