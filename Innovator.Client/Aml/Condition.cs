using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Possible value for the <c>condition</c> attribute of a property for an AML get query
  /// </summary>
  public enum Condition
  {
    /// <summary>Not specified</summary>
    Undefined,
    /// <summary>SQL between</summary>
    Between,
    /// <summary>Is equal to</summary>
    Equal,
    /// <summary>Is greather than</summary>
    GreaterThan,
    /// <summary>Is greather than or equal to</summary>
    GreaterThanEqual,
    /// <summary>Is in a list of values or a subselect</summary>
    In,
    /// <summary>Is X where X is either <c>null</c> or <c>not null</c></summary>
    Is,
    /// <summary>Is not null</summary>
    IsNotNull,
    /// <summary>Is null</summary>
    IsNull,
    /// <summary>Is less than</summary>
    LessThan,
    /// <summary>Is less or equal to</summary>
    LessThanEqual,
    /// <summary>Matches the pattern</summary>
    Like,
    /// <summary>Is equal to</summary>
    NotEqual,
    /// <summary>Is between</summary>
    NotBetween,
    /// <summary>Is not in the list or subselect</summary>
    NotIn,
    /// <summary>Does not match the pattern</summary>
    NotLike
  }
}
