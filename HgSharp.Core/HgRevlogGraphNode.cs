// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{NodeID.Short,nq}")]
    public class HgRevlogGraphNode
    {
        public HgNodeID NodeID { get; private set; }

        public uint Revision { get; private set; }

        public HgRevlogGraphNode FirstParent { get; set; }

        public HgRevlogGraphNode SecondParenet { get; set; }

        public IList<HgRevlogGraphNode> Children { get; private set; }

        public HgRevlogGraphNode(HgNodeID nodeID, uint revision)
        {
            NodeID = nodeID;
            Revision = revision;
            Children = new List<HgRevlogGraphNode>();
        }
    }
}