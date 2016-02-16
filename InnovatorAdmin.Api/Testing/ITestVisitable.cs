using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public interface ITestVisitable
  {
    void Visit(ITestVisitor visitor);
  }
}
