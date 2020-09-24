using System.Collections.Generic;

namespace InnovatorAdmin
{
  public interface IArasMetadataProvider
  {
    IEnumerable<Method> Methods { get; }
    IEnumerable<ItemType> ItemTypes { get; }
    IEnumerable<ItemReference> SystemIdentities { get; }

    /// <summary>
    /// A custom property on a core item type
    /// </summary>
    bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef);
    bool ItemTypeByName(string name, out ItemType type);
    bool SqlRefByName(string name, out ItemReference sql);
  }
}
