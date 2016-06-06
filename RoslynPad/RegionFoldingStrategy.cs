using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynPad
{
  public class RegionFoldingStrategy
  {
    public void UpdateFoldings(FoldingManager manager, TextDocument document)
    {
      manager.UpdateFoldings(GetFoldings(document), -1);
    }

    private IEnumerable<NewFolding> GetFoldings(TextDocument doc)
    {
      NewFolding currFolding = null;
      foreach (var line in doc.Lines)
      {
        if (currFolding != null && doc.GetText(line).Trim() == "#endregion")
        {
          currFolding.EndOffset = line.EndOffset;
          yield return currFolding;
          currFolding = new NewFolding();
        }
        else if (doc.GetText(line).StartsWith("#region "))
        {
          currFolding = new NewFolding();
          currFolding.StartOffset = line.Offset;
        }
      }
    }

  }
}
