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
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
    public class HgCommit : HgCommitEntry
    {
        public HgAuthor CommittedBy { get; private set; }

        public DateTimeOffset CommittedAt { get; private set; }

        public HgBranch Branch { get; private set; }

        public string Comment { get; private set; }

        public ReadOnlyCollection<HgCommitFileEntry> Files { get; private set; }

        public HgCommit(HgNodeID firstParentNodeID, HgNodeID secondParentNodeID, HgAuthor committedBy, DateTimeOffset committedAt, HgBranch branch, string comment, 
            params HgCommitFileEntry[] files) : 
            base(firstParentNodeID, secondParentNodeID)
        {
            CommittedBy = committedBy;
            CommittedAt = committedAt;
            Branch = branch;
            Comment = comment;
            Files = new ReadOnlyCollection<HgCommitFileEntry>(new List<HgCommitFileEntry>(files));
        }
    }

    public class HgWorkingDirectory
    {
        public HgDirstate Dirstate { get; private set; }

        public string Branch { get; private set; }
    }

    public class HgDirstate
    {
        
    }
}
