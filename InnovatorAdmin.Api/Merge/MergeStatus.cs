using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public enum MergeStatus
  {
    NoChange,
    TakeLocal,
    TakeRemote,
    UnresolvedConflict,
    ResolvedConflict
  }
}
