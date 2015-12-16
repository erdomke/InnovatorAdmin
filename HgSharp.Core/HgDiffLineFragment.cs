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
    [DebuggerDisplay("{Index} {Line} ({FragmentType} {IndexA}, {IndexB})")]
    public class HgDiffLineFragment : HgDiffFragment
    {
        public int Index { get; set; }

        //public IList<HgDiffInlineFragment> Diff { get; set; } 

        public HgDiffLineFragment(HgDiffFragmentType type, string content, int index, int indexA, int indexB) : base(type, content, indexA, indexB)
        {
            Index = index;
        }
    }
}