using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  public class Test
  {
    private List<ITestCommand> _commands = new List<ITestCommand>();

    public string Comment { get; set; }
    public IList<ITestCommand> Commands { get { return _commands; } }
    public string Name { get; set; }

    public async Task<TestRun> Run(TestContext context)
    {
      var start = DateTime.Now;
      var st = Stopwatch.StartNew();
      int i = 0;
      try
      {
        TestRun result = null;

        for (i = 0; i < _commands.Count; i++)
        {
          try
          {
            await _commands[i].Run(context);
          }
          catch (AssertionFailedException ex)
          {
            if (result == null)
              result = new TestRun()
              {
                Name = this.Name,
                Result = TestResult.Fail,
                Start = start,
                ErrorLine = i + 1,
                Message = ex.Message
              };
          }
        }

        if (result == null)
          return new TestRun()
          {
            Name = this.Name,
            Result = TestResult.Pass,
            ElapsedMilliseconds = st.ElapsedMilliseconds,
            Start = start
          };

        result.ElapsedMilliseconds = st.ElapsedMilliseconds;
        return result;
      }
      catch (Exception ex)
      {
        return new TestRun()
        {
          Name = this.Name,
          Result = TestResult.Fail,
          ElapsedMilliseconds = st.ElapsedMilliseconds,
          Start = start,
          ErrorLine = i + 1,
          Message = ex.Message
        };
      }
      finally
      {
        context.LastResult = null;
      }
    }
  }
}
