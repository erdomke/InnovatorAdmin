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

using System.Linq;

namespace HgSharp.Core
{
    public class HgRequirements
    {
        public static readonly string RevlogV1 = "revlogv1";
        public static readonly string FnCache = "fncache";
        public static readonly string Store = "store";
        public static readonly string DotEncode = "dotencode";
        public static readonly string Largefiles = "largefiles";

        public ReadOnlyCollection<string> Requirements { get; private set; }

        public HgRequirements(IEnumerable<string> requirements)
        { 
            Requirements = new ReadOnlyCollection<string>(new List<string>(requirements));
        }

        public bool Requires(string requirement)
        {
            return Requirements.Any(r => r == requirement);
        }
    }
}