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

namespace HgSharp.Core
{
    [DebuggerDisplay("{Metadata.Revision}:{Metadata.NodeID.Short,nq}")]
    public class HgChangeset
    {
        public static readonly HgChangeset Null = new HgChangeset(
            new HgRevlogEntryMetadata(HgNodeID.Null, uint.MaxValue, uint.MaxValue, HgNodeID.Null, HgNodeID.Null, uint.MaxValue, uint.MaxValue),
            HgNodeID.Null, new HgAuthor("", ""), DateTimeOffset.MinValue, HgBranch.Default, HgNodeID.Null, new string[]{}, "");

        public HgRevlogEntryMetadata Metadata { get; private set; }

        public HgNodeID ManifestNodeID { get; private set; }

        public HgAuthor CommittedBy { get; private set; }

        public DateTimeOffset CommittedAt { get; private set; }

        public HgBranch Branch { get; private set; }

        /// <summary>
        /// <c>hg graft</c> records source changeset in <c>source</c> extra parameter.
        /// </summary>
        public HgNodeID SourceNodeID { get; private set; }

        public ReadOnlyCollection<string> Files { get; private set; }

        public string Comment { get; private set; }

        public HgChangeset(HgRevlogEntryMetadata metadata, HgNodeID manifestNodeID, HgAuthor committedBy, DateTimeOffset committedAt, HgBranch branch, 
            HgNodeID sourceNodeID, IEnumerable<string> files, string comment)
        {
            Metadata = metadata;
            ManifestNodeID = manifestNodeID;
            CommittedBy = committedBy;
            CommittedAt = committedAt;
            Branch = branch;
            SourceNodeID = sourceNodeID;
            Files = new ReadOnlyCollection<string>(new List<string>(files));
            Comment = comment;
        }
    }
}