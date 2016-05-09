using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  public interface IReadOnlyItem : IReadOnlyElement, IItemRef
  {
    IReadOnlyResult AsResult();
    /// <summary>Creates a duplicate of the item object.  All properties (including the ID) are preserved</summary>
    new IItem Clone();
    /// <summary>Returns a reference to the property with the specified name</summary>
    /// <remarks>If the property does not exist, the .Exists member of the property will return false</remarks>
    IReadOnlyProperty Property(string name);
    /// <summary>Returns a reference to the property with the specified name and language</summary>
    /// <remarks>If the property does not exist, the .Exists member of the property will return false</remarks>
    IReadOnlyProperty Property(string name, string lang);
    /// <summary>Returns the set of relationships associated with this item</summary>
    IEnumerable<IReadOnlyItem> Relationships();
    /// <summary>Returns the set of relationships associated with this item of the specified type</summary>
    IEnumerable<IReadOnlyItem> Relationships(string type);
  }
  public interface IItem : IElement, IReadOnlyItem
  {
    new IProperty Property(string name);
    new IProperty Property(string name, string lang);
    new IEnumerable<IItem> Relationships();
    new IEnumerable<IItem> Relationships(string type);
  }
}
