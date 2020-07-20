using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class GitDirectorySearch
  {
    public string Sha { get; set; }

    public IList<string> BranchNames { get; } = new List<string>();

    public string Path { get; set; }
  }
}
