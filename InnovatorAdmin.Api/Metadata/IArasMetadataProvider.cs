using System.Collections.Generic;

namespace InnovatorAdmin
{
  public interface IArasMetadataProvider
  {
    IEnumerable<ItemReference> CoreMethods { get; }
    IEnumerable<ItemType> ItemTypes { get; }
    IEnumerable<ItemReference> SystemIdentities { get; }

    bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef);
    bool ItemTypeByName(string name, out ItemType type);
    bool SqlRefByName(string name, out ItemReference sql);
  }
}