using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  [Flags]
  public enum ElementAttribute
  {
    FromDataStore = 0x01,
    ReadOnly = 0x02,
    PartialLoad = 0x04,
    ItemDefaultGeneration = 0x40000000,
    ItemDefaultIsCurrent = 0x20000000,
    ItemDefaultIsReleased = 0x10000000,
    ItemDefaultMajorRev = 0x8000000,
    ItemDefaultNewVersion = 0x4000000,
    ItemDefaultNotLockable = 0x2000000,
    ItemDefaultAny = 0x7e000000
  }
}
