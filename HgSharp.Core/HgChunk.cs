// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{NodeID.Short,nq}")]
    public class HgChunk
    {
        public HgNodeID NodeID { get; private set; }

        public HgNodeID FirstParentNodeID { get; private set; }

        public HgNodeID SecondParentNodeID { get; private set; }

        public HgNodeID ChangesetNodeID { get; private set; }

        public byte[] Data { get; private set; }

        public HgChunk(HgNodeID nodeID, HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, HgNodeID changesetNodeID, byte[] data)
        {
            NodeID = nodeID;
            FirstParentNodeID = firstParentNodeID;
            SecondParentNodeID = secondParentNodeID;
            ChangesetNodeID = changesetNodeID;
            Data = data;
        }
    }
}