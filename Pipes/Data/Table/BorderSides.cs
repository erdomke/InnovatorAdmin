using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  /// <summary>
  /// Specifies which sides have borders.
  /// </summary>
  [Flags]
  public enum BorderSides
  {
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    All = 15,
    NotSet = 16
  }
}
