using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Attributes stored in AML elements for compact memory usage
  /// </summary>
  [Flags]
  public enum ElementAttribute
  {
    /// <summary>
    /// Indicates that the element represents a null element
    /// </summary>
    Null = 0x01,
    /// <summary>
    /// Indicates that the element came from a data store (e.g. from the server)
    /// </summary>
    FromDataStore = 0x02,
    /// <summary>
    /// Indicates that the element is read-only and should not be modified
    /// </summary>
    ReadOnly = 0x04,
    /// <summary>
    /// Indicates that the Item element contains a default <c>generation</c> property (&lt;generation&gt;1&lt;/generation&gt;)
    /// </summary>
    ItemDefaultGeneration = 0x40000000,
    /// <summary>
    /// Indicates that the Item element contains a default <c>is_current</c> property (&lt;is_current&gt;1&lt;/is_current&gt;)
    /// </summary>
    ItemDefaultIsCurrent = 0x20000000,
    /// <summary>
    /// Indicates that the Item element contains a default <c>is_released</c> property (&lt;is_released&gt;0&lt;/is_released&gt;)
    /// </summary>
    ItemDefaultIsReleased = 0x10000000,
    /// <summary>
    /// Indicates that the Item element contains a default <c>major_rev</c> property (&lt;major_rev&gt;A&lt;/major_rev&gt;)
    /// </summary>
    ItemDefaultMajorRev = 0x8000000,
    /// <summary>
    /// Indicates that the Item element contains a default <c>new_version</c> property (&lt;new_version&gt;0&lt;/new_version&gt;)
    /// </summary>
    ItemDefaultNewVersion = 0x4000000,
    /// <summary>
    /// Indicates that the Item element contains a default <c>not_lockable</c> property (&lt;not_lockable&gt;0&lt;/not_lockable&gt;)
    /// </summary>
    ItemDefaultNotLockable = 0x2000000,
    /// <summary>
    /// Mask indicating that the Item element contains one or more default properties
    /// </summary>
    ItemDefaultAny = 0x7e000000
  }
}
