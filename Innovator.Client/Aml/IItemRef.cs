using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Reference to an Aras item specifying the type name and ID
  /// </summary>
  public interface IItemRef
  {
    /// <summary>The ID of the item as retrieved from either the attribute or the property</summary>
    string Id();
    /// <summary>The type of the item as retrieved from either the attribute or the property</summary>
    string TypeName();
  }
}
