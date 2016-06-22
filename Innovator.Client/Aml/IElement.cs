using System;
using System.Collections.Generic;
using System.Xml;

namespace Innovator.Client
{
  /// <summary>A read-only AML element.  This element could be an Item, property, result tag, or something else</summary>
  public interface IReadOnlyElement : IAmlNode
  {
    /// <summary>Retrieve the attribute with the specified name</summary>
    IReadOnlyAttribute Attribute(string name);
    /// <summary>Retrieve all attributes specified for the element</summary>
    IEnumerable<IReadOnlyAttribute> Attributes();
    /// <summary>Retrieve all child elements</summary>
    IEnumerable<IReadOnlyElement> Elements();
    /// <summary>Retrieve the context used for rendering primitive values</summary>
    ElementFactory AmlContext { get; }
    /// <summary>Returns <c>true</c> if this element actually exists in the underlying AML,
    /// otherwise, returns <c>false</c> to indicate that the element is just a null placeholder
    /// put in place to reduce unnecessary null reference checks</summary>
    bool Exists { get; }
    /// <summary>Local XML name of the element</summary>
    string Name { get; }
    /// <summary>Retrieve the parent element</summary>
    IReadOnlyElement Parent { get; }
    /// <summary>String value of the element</summary>
    string Value { get; }
  }
  /// <summary>A modifiable AML element.  This element could be an Item, property, result tag, or something else</summary>
  public interface IElement : IReadOnlyElement
  {
    /// <summary>Retrieve the attribute with the specified name</summary>
    new IAttribute Attribute(string name);
    /// <summary>Retrieve all attributes specified for the element</summary>
    new IEnumerable<IAttribute> Attributes();
    /// <summary>Retrieve all child elements</summary>
    new IEnumerable<IElement> Elements();
    /// <summary>Retrieve the parent element</summary>
    new IElement Parent { get; }
    /// <summary>Add new content to the element</summary>
    IElement Add(params object[] content);
    /// <summary>Remove the element from its parent</summary>
    void Remove();
    /// <summary>Remove attributes from the element</summary>
    void RemoveAttributes();
    /// <summary>Remove child nodes from the element</summary>
    void RemoveNodes();
  }

  public interface ILogical : IElement { }
  public interface IReadOnlyLogical : IReadOnlyElement { }
  public interface IRelationships : IElement { }
  public interface IReadOnlyRelationships : IReadOnlyElement { }
}
