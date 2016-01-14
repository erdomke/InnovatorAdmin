using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public class TestRun
  {
    public string Name { get; set; }
    public DateTime Start { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public TestResult Result { get; set; }
    public int ErrorLine { get; set; }
    public string Message { get; set; }
    public string ErrorXml { get; set; }
  }
}
