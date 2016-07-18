using System;
namespace Innovator.Client
{
  /// <summary>
  /// A read-only AML attribute
  /// </summary>
  public interface IReadOnlyAttribute
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
    /// <summary>Value converted to a string using the default value if null.</summary>
    string AsString(string defaultValue);
    /// <summary>Returns <c>true</c> if this element actually exists in the underlying AML,
    /// otherwise, returns <c>false</c> to indicate that the element is just a null placeholder
    /// put in place to reduce unnecessary null reference checks</summary>
    bool Exists { get; }
    /// <summary>Local XML name of the attribute</summary>
    string Name { get; }
    /// <summary>String value of the attribute</summary>
    string Value { get; }
  }
  /// <summary>
  /// A modifiable AML attribute
  /// </summary>
  public interface IAttribute : IReadOnlyAttribute
  {
    /// <summary>
    /// Set the value of this attribute
    /// </summary>
    void Set(object value);
    /// <summary>
    /// Remove the attribute from it's parent
    /// </summary>
    void Remove();
    /// <summary>String value of the attribute</summary>
    new string Value { get; set; }
  }
}
