using System;
namespace Innovator.Client
{
  /// <summary>
  /// A readonly property of an item
  /// </summary>
  public interface IReadOnlyProperty : IReadOnlyElement
  {
    /// <summary>Value converted to a nullable boolean.
    /// If the value cannot be converted, an exception is thrown</summary>
    bool? AsBoolean();
    /// <summary>Value converted to a boolean using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    bool AsBoolean(bool defaultValue);
    /// <summary>Value converted to a nullable DateTime in the local timezone.
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime? AsDateTime();
    /// <summary>Value converted to a DateTime in the local timezone using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    /// <example>
    /// <code lang="C#">
    /// // If the part was created after 2016-01-01, put the name of the creator in the description
    /// if (comp.CreatedOn().AsDateTime(DateTime.MaxValue) > new DateTime(2016, 1, 1))
    /// {
    ///     edits.Property("description").Set("Created by: " + comp.CreatedById().KeyedName().Value);
    /// }
    /// </code>
    /// </example>
    DateTime AsDateTime(DateTime defaultValue);
    /// <summary>Value converted to a nullable DateTime in the UTC timezone.
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime? AsDateTimeUtc();
    /// <summary>Value converted to a DateTime in the UTC timezone using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    DateTime AsDateTimeUtc(DateTime defaultValue);
    /// <summary>Value converted to a nullable double.
    /// If the value cannot be converted, an exception is thrown</summary>
    double? AsDouble();
    /// <summary>Value converted to a double using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    double AsDouble(double defaultValue);
    /// <summary>Value converted to a nullable Guid.
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid? AsGuid();
    /// <summary>Value converted to a Guid using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid AsGuid(Guid defaultValue);
    /// <summary>Value converted to a nullable int.
    /// If the value cannot be converted, an exception is thrown</summary>
    int? AsInt();
    /// <summary>Value converted to a int using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    int AsInt(int defaultValue);
    /// <summary>Value converted to a nullable long.
    /// If the value cannot be converted, an exception is thrown</summary>
    long? AsLong();
    /// <summary>Value converted to a long using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    long AsLong(long defaultValue);
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' item (where the
    /// <c>Exists</c> property returns <c>false</c>) is returned</summary>
    IReadOnlyItem AsItem();
    /// <summary>Value converted to a string using the default value if null.</summary>
    string AsString(string defaultValue);
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
