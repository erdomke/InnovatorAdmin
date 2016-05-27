using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  /// <summary>
  /// Represents a modifiable result of an Aras query that can either be a string, an exception, or zero or more items
  /// </summary>
  public interface IResult : IReadOnlyResult, IErrorBuilder
  {
    /// <summary>Return a single item.  If that is not possible, throw an appropriate
    /// exception (e.g. the exception returned by the server where possible)</summary>
    /// <param name="type">If specified, throw an exception if the item doesn't have the specified type</param>
    new IItem AssertItem(string type = null);
    /// <summary>Add the specified item to the result</summary>
    IResult Add(IItem content);
    /// <summary>Return the string value of the result</summary>
    new string Value { get; set; }
  }
  /// <summary>
  /// Represents a read-only result of an Aras query that can either be a string, an exception, or zero or more items
  /// </summary>
  public interface IReadOnlyResult : IAmlNode
  {
    /// <summary>Return a single item.  If that is not possible, throw an appropriate
    /// exception (e.g. the exception returned by the server where possible)</summary>
    /// <param name="type">If specified, throw an exception if the item doesn't have the specified type</param>
    IReadOnlyItem AssertItem(string type = null);
    /// <summary>Return an enumerable of items.  Throw an exception for any error including 'No items found'</summary>
    IEnumerable<IReadOnlyItem> AssertItems();
    /// <summary>Do nothing other than throw an exception if there is an error other than 'No Items Found'</summary>
    IReadOnlyResult AssertNoError();
    /// <summary>Return an exception (if there is one), otherwise, return <c>null</c></summary>
    ServerException Exception { get; }
    /// <summary>Return an enumerable of items.  Throw an exception if there is an error other than 'No Items Found'</summary>
    IEnumerable<IReadOnlyItem> Items();
    /// <summary>Return the string value of the result</summary>
    string Value { get; }
  }
}
