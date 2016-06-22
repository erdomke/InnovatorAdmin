using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  /// <summary>
  /// Represents an Aras Item that is read only.  By default, the connection object returns IReadOnlyItems to encouarge
  /// treating the results as immutable
  /// </summary>
  public interface IReadOnlyItem : IReadOnlyElement, IItemRef
  {
    /// <summary>Creates a duplicate of the item object.  All properties (including the ID) are preserved</summary>
    IItem Clone();
    /// <summary>Returns a reference to the property with the specified name</summary>
    /// <remarks>If the property does not exist, a non-null object will be returned that has an <c>Exists</c> member which will return <c>false</c></remarks>
    /// <param name="name">Name of the property</param>
    IReadOnlyProperty Property(string name);
    /// <summary>Returns a reference to the property with the specified name and language</summary>
    /// <remarks>If the property does not exist, a non-null object will be returned that has an <c>Exists</c> member which will return <c>false</c></remarks>
    /// <param name="name">Name of the property</param>
    /// <param name="lang">Language of the (multilingual) property</param>
    IReadOnlyProperty Property(string name, string lang);
    /// <summary>Returns the set of relationships associated with this item</summary>
    IEnumerable<IReadOnlyItem> Relationships();
    /// <summary>Returns the set of relationships associated with this item of the specified type</summary>
    /// <param name="type">Name of the ItemType for the relationships you wish to retrieve</param>
    IEnumerable<IReadOnlyItem> Relationships(string type);
  }
  /// <summary>
  /// Represents an Aras Item that is modifiable
  /// </summary>
  public interface IItem : IElement, IReadOnlyItem
  {
    /// <summary>Returns a reference to the property with the specified name</summary>
    /// <remarks>If the property does not exist, a non-null object will be returned that has an <c>Exists</c> member which will return <c>false</c></remarks>
    /// <param name="name">Name of the property</param>
    new IProperty Property(string name);
    /// <summary>Returns a reference to the property with the specified name and language</summary>
    /// <remarks>If the property does not exist, a non-null object will be returned that has an <c>Exists</c> member which will return <c>false</c></remarks>
    /// <param name="name">Name of the property</param>
    /// <param name="lang">Language of the (multilingual) property</param>
    new IProperty Property(string name, string lang);
    /// <summary>Returns the set of relationships associated with this item</summary>
    new IEnumerable<IItem> Relationships();
    /// <summary>Returns the set of relationships associated with this item of the specified type</summary>
    /// <param name="type">Name of the ItemType for the relationships you wish to retrieve</param>
    new IEnumerable<IItem> Relationships(string type);
  }
}
