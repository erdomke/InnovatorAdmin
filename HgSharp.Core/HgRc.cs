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
    public class HgRc
    {
        private readonly IList<HgRcSection> sections = new List<HgRcSection>();
 
        public ReadOnlyCollection<HgRcSection> Sections
        {
            get { return new ReadOnlyCollection<HgRcSection>(sections); }
        }

        public HgRcSection this[string sectionName]
        {
            get { return sections.SingleOrDefault(s => s.Name == sectionName); } 
        }

        public HgRc(IList<HgRcSection> sections)
        {
            this.sections = sections ?? new List<HgRcSection>();
        }

        public HgRc(params HgRcSection[] sections)
        {
            this.sections = new List<HgRcSection>(sections ?? new HgRcSection[]{});
        }

        public void Add(HgRcSection section)
        {
            sections.Add(section);
        }
    }
}