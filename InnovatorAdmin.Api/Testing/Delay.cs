using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Introduces a delay into a unit test to ensure that async actions are complete
  /// </summary>
  public class Delay : ICommand
  {
    /// <summary>
    /// How many seconds to delay
    /// </summary>
    public string BySeconds { get; set; }
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    public string Comment { get; set; }
    /// <summary>
    /// The date/time to delay from.  If not specified, it will be the current date/time
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// Code for executing the command
    /// </summary>
    public async Task Run(TestContext context)
    {
      var start = DateTime.UtcNow;
      string fromDate;
      if (!string.IsNullOrEmpty(From)
        && (DateTime.TryParse(From, out start)
          || (context.Parameters.TryGetValue(From, out fromDate)
            && DateTime.TryParse(fromDate, out start))))
      {
        start = start.ToUniversalTime();
      }

      int offset;
      if (!int.TryParse(BySeconds, out offset))
        return;

      var delay = (int)(start.AddSeconds(offset) - DateTime.UtcNow).TotalMilliseconds;
      if (delay > 0)
        await Task.Delay(delay);
    }

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
