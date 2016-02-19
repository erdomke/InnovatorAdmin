using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public enum SearchMode
  {
    /// <summary>Standard search</summary>
    Normal,
    /// <summary>RegEx search</summary>
    RegEx,
    /// <summary>Wildcard search</summary>
    Wildcard,
    Extended,
    XPath
  }
}
