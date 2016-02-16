using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Command which can only occur during a test
  /// </summary>
  public interface ITestCommand : ITestVisitable
  {
    /// <summary>
    /// Code for executing the command
    /// </summary>
    Task Run(TestContext context);
  }
}
