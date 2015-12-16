using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  interface IUpdateListener
  {
    void UpdateCheckProgress(int progress);
    void UpdateCheckComplete(Version latestVersion);
  }
}
