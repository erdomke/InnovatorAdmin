using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public interface ICommand : ITestCommand
  {
    string Comment { get; }
  }
}
