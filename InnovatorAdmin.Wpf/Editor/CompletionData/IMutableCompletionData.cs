using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public interface IMutableCompletionData : ICompletionData
  {
    new string Text { get; set; }
    new object Description { get; set; }
  }
}
