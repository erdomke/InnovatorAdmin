using System;
namespace Innovator.Client
{
  /// <summary>
  /// Very base property interface used to flag elements which are properties
  /// </summary>
  public interface IReadOnlyProperty_Base : IReadOnlyElement { }

  /// <summary>
  /// Property of type boolean
  /// </summary>
  public interface IReadOnlyProperty_Boolean : IReadOnlyProperty_Base
  {
    /// <summary>Value converted to a nullable boolean.
    /// If the value cannot be converted, an exception is thrown</summary>
    bool? AsBoolean();
    /// <summary>Value converted to a boolean using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    bool AsBoolean(bool defaultValue);
  }

  /// <summary>
  /// Property of type date
  /// </summary>
  public interface IReadOnlyProperty_Date : IReadOnlyProperty_Base
  {
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
  }

  /// <summary>
  /// Property of type integer, decimal, float, etc.
  /// </summary>
  public interface IReadOnlyProperty_Number : IReadOnlyProperty_Base
  {
    /// <summary>Value converted to a nullable double.
    /// If the value cannot be converted, an exception is thrown</summary>
    double? AsDouble();
    /// <summary>Value converted to a double using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    double AsDouble(double defaultValue);
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
  }

  /// <summary>
  /// Property of type item
  /// </summary>
  public interface IReadOnlyProperty_Item<in T> : IReadOnlyProperty_Base
  {
    /// <summary>Value converted to a nullable Guid.
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid? AsGuid();
    /// <summary>Value converted to a Guid using the default value if null.
    /// If the value cannot be converted, an exception is thrown</summary>
    Guid AsGuid(Guid defaultValue);
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' item (where the
    /// <c>Exists</c> property returns <c>false</c>) is returned</summary>
    IReadOnlyItem AsItem();
  }

  /// <summary>
  /// Property that is of type text/string or something similar
  /// </summary>
  public interface IReadOnlyProperty_Text : IReadOnlyProperty_Base
  {
    /// <summary>Value converted to a string using the default value if null.</summary>
    string AsString(string defaultValue);
  }

  /// <summary>
  /// A readonly property of an item
  /// </summary>
  public interface IReadOnlyProperty
    : IReadOnlyProperty_Boolean
    , IReadOnlyProperty_Date
    , IReadOnlyProperty_Item<IReadOnlyItem>
    , IReadOnlyProperty_Number
    , IReadOnlyProperty_Text
  {

  }

  /// <summary>
  /// Very base property interface used to flag elements which are properties
  /// </summary>
  public interface IProperty_Base : IElement
  {
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
  /// <summary>
  /// Property of type boolean
  /// </summary>
  public interface IProperty_Boolean : IReadOnlyProperty_Boolean, IProperty_Base { }
  /// <summary>
  /// Property of type date
  /// </summary>
  public interface IProperty_Date    : IReadOnlyProperty_Date, IProperty_Base { }
  /// <summary>
  /// Property of type item
  /// </summary>
  public interface IProperty_Item<in T>    : IReadOnlyProperty_Item<T>, IProperty_Base
  {
    /// <summary>Value converted to a read-only item.
    /// If the value cannot be converted, a 'null' item (where the
    /// <c>Exists</c> property returns <c>false</c>) is returned</summary>
    new IItem AsItem();
  }

  /// <summary>
  /// Property of type integer, decimal, float, etc.
  /// </summary>
  public interface IProperty_Number  : IReadOnlyProperty_Number, IProperty_Base { }
  /// <summary>
  /// Property that is of type text/string or something similar
  /// </summary>
  public interface IProperty_Text    : IReadOnlyProperty_Text, IProperty_Base { }

  /// <summary>
  /// A modifiable property of an item
  /// </summary>
  public interface IProperty
    : IProperty_Boolean
    , IProperty_Date
    , IProperty_Item<IReadOnlyItem>
    , IProperty_Number
    , IProperty_Text
    , IReadOnlyProperty
  {

  }
}
