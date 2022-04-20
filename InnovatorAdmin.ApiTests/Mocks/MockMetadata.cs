using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin.Tests
{
  internal class MockMetadata : IArasMetadataProvider
  {
    public IEnumerable<Method> Methods => Enumerable.Empty<Method>();

    public IEnumerable<ItemType> ItemTypes => Enumerable.Empty<ItemType>();

    public IEnumerable<ItemReference> SystemIdentities => Enumerable.Empty<ItemReference>();

    public bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef)
    {
      propRef = null;
      return false;
    }

    public bool ItemTypeByName(string name, out ItemType type)
    {
      type = null;
      return false;
    }

    public bool SqlRefByName(string name, out ItemReference sql)
    {
      sql = null;
      return false;
    }
  }
}
