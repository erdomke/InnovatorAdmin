using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public enum HiddenColumnOptions
  {
    /// <summary>
    /// Include hidden columns and make the visibile
    /// </summary>
    IncludeVisible,
    /// <summary>
    /// Include hidden columns, but hide them by default
    /// </summary>
    IncludeHidden,
    /// <summary>
    /// Exclude hidden columns entirely.
    /// </summary>
    Exclude
  }
}
