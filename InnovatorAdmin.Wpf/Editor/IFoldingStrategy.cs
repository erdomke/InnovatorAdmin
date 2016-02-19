using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public interface IFoldingStrategy
  {
    void UpdateFoldings(FoldingManager manager, TextDocument document);
  }
}
