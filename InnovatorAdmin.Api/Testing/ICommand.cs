using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Command which can occur during initialization, cleanup, or a test
  /// </summary>
  public interface ICommand : ITestCommand
  {
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    string Comment { get; }
  }
}
