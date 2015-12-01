using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class CompletionData
  {
    public IEnumerable<string> Items { get; set; }
    public bool MultiValueAttribute { get; set; }
    public int Overlap { get; set; }
    public CompletionType State { get; set; }

    public CompletionData()
    {
      Items = Enumerable.Empty<string>();
      Overlap = 0;
    }
  }
}
