// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HgSharp.Core
{
    public class HgAcl
    {
        private readonly ReadOnlyCollection<string> sources; 
        private readonly ReadOnlyCollection<HgAclEntry> allow;
        private readonly ReadOnlyCollection<HgAclEntry> deny;
        private readonly ReadOnlyCollection<HgAclEntry> allowBranches;
        private readonly ReadOnlyCollection<HgAclEntry> denyBranches;

        public ReadOnlyCollection<string> Sources { get { return sources; } } 
        
        public ReadOnlyCollection<HgAclEntry> Allow { get { return allow; } }

        public ReadOnlyCollection<HgAclEntry> Deny { get { return deny; } }

        public ReadOnlyCollection<HgAclEntry> AllowBranches { get { return allowBranches; } }

        public ReadOnlyCollection<HgAclEntry> DenyBranches { get { return denyBranches; } }

        public HgAcl(ReadOnlyCollection<string> sources, ReadOnlyCollection<HgAclEntry> allow, ReadOnlyCollection<HgAclEntry> deny, ReadOnlyCollection<HgAclEntry> allowBranches, ReadOnlyCollection<HgAclEntry> denyBranches)
        {
            this.allow = allow;
            this.deny = deny;
            this.allowBranches = allowBranches;
            this.denyBranches = denyBranches;
            this.sources = sources;
        }
    }
}
