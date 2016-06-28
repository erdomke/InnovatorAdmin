using System;
namespace Innovator.Client
{
  /// <summary>
  /// A readonly property of an item
  /// </summary>
  public interface IReadOnlyProperty : IReadOnlyElement, IReadOnlyValue
  {
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' item (where the
    /// <c>Exists</c> property returns <c>false</c>) is returned</summary>
    IReadOnlyItem AsItem();
  }
  /// <summary>
  /// A modifiable property of an item
  /// </summary>
  public interface IProperty : IReadOnlyProperty, IElement
  {
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' item (where the
    /// <c>Exists</c> property returns <c>false</c>) is returned</summary>
    new IItem AsItem();
    /// <summary>
    /// Set the value of the property
    /// </summary>
    /// <param name="value">Value to set the property to</param>
    /// <example>
    /// <code lang="C#">
    /// // If the part was created after 2016-01-01, put the name of the creator in the description
    /// if (comp.CreatedOn().AsDateTime(DateTime.MaxValue) > new DateTime(2016, 1, 1))
    /// {
    ///     edits.Property("description").Set("Created by: " + comp.CreatedById().KeyedName().Value);
    /// }
    /// </code>
    /// </example>
    void Set(object value);
  }
}
