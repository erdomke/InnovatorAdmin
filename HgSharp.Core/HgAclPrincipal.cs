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
    [DebuggerDisplay("{Type,nq} {Name,nq}, Excludes: {Excludes}")]
    public class HgAclPrincipal
    {
        public bool Excludes { get; private set; }

        public HgAclPrincipalType Type { get; private set; }

        public string Name { get; private set; }

        public HgAclPrincipal(bool excludes, HgAclPrincipalType type, string name)
        {
            Excludes = excludes;
            Type = type;
            Name = name;
        }
    }
}