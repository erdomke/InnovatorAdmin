using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public class AssertionFailedException : Exception
  {
    public AssertionFailedException() : base() { }
    public AssertionFailedException(string message) : base(message) { }
    public AssertionFailedException(string message, Exception inner) : base(message, inner) { }
  }
}
