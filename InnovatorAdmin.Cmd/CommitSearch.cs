using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  class CommitSearch
  {
    public string Sha { get; set; }

    public IList<string> BranchNames { get; } = new List<string>();


  }
}
