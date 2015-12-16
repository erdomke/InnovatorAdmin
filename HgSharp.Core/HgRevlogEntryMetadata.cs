// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Revision}:{NodeID.Short,nq}")]
    public class HgRevlogEntryMetadata
    {
        public HgNodeID NodeID { get; private set; }

        public UInt32 Revision { get; private set; }

        public UInt32 LinkRevision { get; private set; }

        public HgNodeID FirstParentRevisionNodeID { get; private set; }

        public HgNodeID SecondParentRevisionNodeID { get; private set; }

        public uint FirstParentRevision { get; private set; }

        public uint SecondParentRevision { get; private set; }

        public IEnumerable<HgRevision> Parents
        {
            get
            {
                if(FirstParentRevisionNodeID != HgNodeID.Null) yield return new HgRevision(FirstParentRevision, FirstParentRevisionNodeID);
                if(SecondParentRevisionNodeID != HgNodeID.Null) yield return new HgRevision(SecondParentRevision, SecondParentRevisionNodeID);
            }
        }

        public HgRevlogEntryMetadata(HgNodeID nodeID, uint revision, uint linkRevision, HgNodeID firstParentRevisionNodeID, HgNodeID secondParentRevisionNodeID, 
            uint firstParentRevision, uint secondParentRevision)
        {
            SecondParentRevision = secondParentRevision;
            FirstParentRevision = firstParentRevision;
            NodeID = nodeID;
            Revision = revision;
            LinkRevision = linkRevision;
            FirstParentRevisionNodeID = firstParentRevisionNodeID;
            SecondParentRevisionNodeID = secondParentRevisionNodeID;
        }
    }
}