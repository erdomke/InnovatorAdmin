using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface ICommand
  {
    ICommand WithStatementCount(int statementCount);
    ICommand WithConcurrentCount(int concurrentCount);
    ICommand WithQuery(string query);
    ICommand WithAction(string action);
    ICommand WithParam(string name, object value);
  }
}
