using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.AutoComplete
{
  public class CompletionData
  {
    public IEnumerable<string> Items { get; set; }
    public bool MultiValueAttribute { get; set; }
    public int Overlap { get; set; }
    public XmlState State { get; set; }

    public CompletionData()
    {
      Items = Enumerable.Empty<string>();
      Overlap = 0;
    }
  }
}
