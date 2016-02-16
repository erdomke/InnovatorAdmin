using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Result of an AML unit test
  /// </summary>
  public enum TestResult
  {
    /// <summary>
    /// The test was not run
    /// </summary>
    NotRun,
    /// <summary>
    /// The test failed
    /// </summary>
    Fail,
    /// <summary>
    /// The test passed
    /// </summary>
    Pass
  }
}
