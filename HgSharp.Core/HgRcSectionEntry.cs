// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Diagnostics;

namespace HgSharp.Core
{
    internal class HgRcSectionEntry
    {
        public Tuple<string, string> Property { get; set; }

        public string Verbatim { get; set; }

        public override string ToString()
        {
            return Property == null ? Verbatim : string.Format("{0} = {1}", Property.Item1, Property.Item2);
        }
    }
}