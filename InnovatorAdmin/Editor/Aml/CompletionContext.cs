using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin.Editor
{
  public class CompletionContext
  {
    public IEnumerable<ICompletionData> Items { get; set; }
    public bool IsXmlAttribute { get; set; }
    public int Overlap { get; set; }

    public CompletionContext()
    {
      Items = Enumerable.Empty<ICompletionData>();
      Overlap = 0;
    }
  }
}
