using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IResultObject
  {
    OutputType PreferredMode { get; }
    int ItemCount { get; }
    ITextSource GetTextSource();
    DataSet GetDataSet();
    string Title { get; }
    string Html { get; }
  }
}
