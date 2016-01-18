using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  public interface IReadOnlyElement : ICloneable
  {
    IReadOnlyAttribute Attribute(string name);
    IEnumerable<IReadOnlyAttribute> Attributes();
    IEnumerable<IReadOnlyElement> Elements();
    IServerContext Context { get; }
    /// <summary>Returns <c>true</c> if this element actually exists in the underlying AML, 
    /// otherwise, returns <c>false</c> to indicate that the element is just a null placeholder
    /// put in place to reduce unnecessary null reference checks</summary>
    bool Exists { get; }
    /// <summary>Local XML name of the element</summary>
    string Name { get; }
    /// <summary>Parent element</summary>
    IReadOnlyElement Parent { get; }
    /// <summary>String value of the element</summary>
    string Value { get; }

    string ToAml();
  }
  public interface IElement : IReadOnlyElement
  {
    new IAttribute Attribute(string name);
    new IEnumerable<IAttribute> Attributes();
    new IEnumerable<IElement> Elements();
    new IElement Parent { get; }
    IElement Add(params object[] content);
    IElement Add(object content);
    void Remove();
    void RemoveAttributes();
    void RemoveNodes();
  }

  public interface ILogical : IElement { }
  public interface IReadOnlyLogical : IReadOnlyElement { }
  public interface IRelationships : IElement { }
  public interface IReadOnlyRelationships : IReadOnlyElement { }
}
