using System;
namespace Innovator.Client
{
  /// <summary>
  /// A read-only AML attribute
  /// </summary>
  public interface IReadOnlyAttribute : IReadOnlyValue
  {
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
