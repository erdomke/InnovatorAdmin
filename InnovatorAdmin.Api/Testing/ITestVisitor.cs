using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public interface ITestVisitor
  {
    void Visit(AssertMatch match);
    void Visit(Delay delay);
    void Visit(DownloadFile download);
    void Visit(Login login);
    void Visit(Logout logout);
    void Visit(ParamAssign param);
    void Visit(Query query);
    void Visit(Test test);
    void Visit(TestRun run);
    void Visit(TestSuite suite);
  }
}
