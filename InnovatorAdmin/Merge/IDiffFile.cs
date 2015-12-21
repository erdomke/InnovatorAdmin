using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IDiffFile
  {
    string Path { get; }
    IComparable CompareKey { get; }
    Stream OpenRead();
  }
}
