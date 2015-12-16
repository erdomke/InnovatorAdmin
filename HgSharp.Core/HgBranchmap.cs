// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Branch.Name,nq}")]
    public class HgBranchmap
    {
        public HgBranch Branch { get; private set; }

        public ReadOnlyCollection<HgNodeID> NodeIDs { get; private set; }

        public HgBranchmap(HgBranch branch, IEnumerable<HgNodeID> nodeIDs)
        {
            Branch = branch;
            NodeIDs = new ReadOnlyCollection<HgNodeID>(new List<HgNodeID>(nodeIDs));
        }
    }
}
