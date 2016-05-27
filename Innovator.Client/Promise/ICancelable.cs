using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Indicates that an operation can be canceled
  /// </summary>
  public interface ICancelable
  {
    /// <summary>
    /// Cancel the operation
    /// </summary>
    void Cancel();
  }
}
