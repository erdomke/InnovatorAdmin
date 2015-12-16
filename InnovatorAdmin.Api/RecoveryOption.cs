using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  /// <summary>
  /// Enum describing how to recover from an installation error
  /// </summary>
  public enum RecoveryOption
  {
    /// <summary>
    /// Abort installation
    /// </summary>
    Abort,
    /// <summary>
    /// Retry the problematic installation step
    /// </summary>
    Retry,
    /// <summary>
    /// Skip the problematic installation step
    /// </summary>
    Skip
  }
}
