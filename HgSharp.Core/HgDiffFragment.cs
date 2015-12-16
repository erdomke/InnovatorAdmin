// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

namespace HgSharp.Core
{
    public abstract class HgDiffFragment
    {
        public HgDiffFragmentType Type { get; private set; }

        public string Content { get; private set; }

        public int IndexA { get; private set; }

        public int IndexB { get; private set; }

        public bool Added
        {
            get { return Type == HgDiffFragmentType.Added; }
        }

        public bool Removed
        {
            get { return Type == HgDiffFragmentType.Removed; }
        }

        public bool Unchanged
        {
            get { return Type == HgDiffFragmentType.Unchanged; }
        }

        protected HgDiffFragment(HgDiffFragmentType type, string content, int indexA, int indexB)
        {
            Type = type;
            Content = content;
            IndexA = indexA;
            IndexB = indexB;
        }
    }
}