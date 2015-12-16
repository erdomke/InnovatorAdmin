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
    [DebuggerDisplay("{Path,nq}")]
    public class HgAclEntry
    {
        private readonly IList<HgAclPrincipal> principals;

        public string Path { get; private set; }

        public ReadOnlyCollection<HgAclPrincipal> Principals
        {
            get { return new ReadOnlyCollection<HgAclPrincipal>(principals);}
        }

        public HgAclEntry(string path, IList<HgAclPrincipal> principals)
        {
            Path = path;
            this.principals = principals ?? new List<HgAclPrincipal>();
        }
    }
}