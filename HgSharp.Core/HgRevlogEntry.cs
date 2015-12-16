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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace HgSharp.Core
{
    [DebuggerDisplay("rev: {Revision}, offset: {Offset}, length: {CompressedLength}, base: {BaseRevision}, linkrev: {LinkRevision}, nodeid: {NodeID.Short,nq}")]
    public class HgRevlogEntry
    {
        public static readonly HgRevlogEntry Null = null;

        public UInt32 Revision { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>>
        /// Only 6 bytes of this are ever used.
        /// </remarks>
        public UInt64 Offset { get; private set; }
        
        public UInt16 Flags { get; private set; }

        public UInt32 CompressedLength { get; private set; }

        public UInt32 UncompressedLength { get; private set; }

        public UInt32 BaseRevision { get; private set; }

        public UInt32 LinkRevision { get; private set; }

        public UInt32 FirstParentRevision { get; private set; }

        public HgNodeID FirstParentRevisionNodeID { get; private set; }

        public UInt32 SecondParentRevision { get; private set; }

        public HgNodeID SecondParentRevisionNodeID { get; private set; }

        public HgNodeID NodeID { get; private set; }

        internal HgRevlogEntry(UInt32 revision)
        {
            Revision = revision;
        }

        internal HgRevlogEntry(HgNodeID nodeID)
        {
            NodeID = nodeID;
        }

        public HgRevlogEntry(UInt32 revision, HgNodeID nodeID, ulong offset, ushort flags, uint compressedLength, uint uncompressedLength, 
            uint baseRevision, uint linkRevision, uint firstParentRevision, HgNodeID firstParentRevisionNodeID, uint secondParentRevision, HgNodeID secondParentRevisionNodeID)
        {
            Revision = revision;
            Offset = offset;
            Flags = flags;
            CompressedLength = compressedLength;
            UncompressedLength = uncompressedLength;
            BaseRevision = baseRevision;
            LinkRevision = linkRevision;
            FirstParentRevision = firstParentRevision;
            SecondParentRevision = secondParentRevision;
            NodeID = nodeID;
            FirstParentRevisionNodeID = firstParentRevisionNodeID;
            SecondParentRevisionNodeID = secondParentRevisionNodeID;
        }
    }
}