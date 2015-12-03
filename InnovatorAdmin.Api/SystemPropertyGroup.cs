using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  [Flags]
  public enum SystemPropertyGroup
  {
      None = 0
    , State = 1         // state, current_state, is_released, behavior, not_lockable, release_date, effective_date, superseded_date
    , Permission = 2    // permission_id
    , History = 4       // created_by_id, created_on, modified_by_id, modified_on
    , Versioning = 8    // major_rev, minor_rev
  }
}
