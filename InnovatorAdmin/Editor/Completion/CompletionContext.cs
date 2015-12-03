using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin.Editor
{
  public class CompletionContext
  {
    public IEnumerable<string> Items { get; set; }
    public bool MultiValueAttribute { get; set; }
    public int Overlap { get; set; }
    public CompletionType State { get; set; }

    public CompletionContext()
    {
      Items = Enumerable.Empty<string>();
      Overlap = 0;
    }
  }
}
