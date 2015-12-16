// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public abstract class HgPathPatternMatcher
    {
        public static HgPathPatternMatcher Parse(string pattern)
        {
            if(pattern.StartsWith("glob:")) return new HgGlobPathPatternMatcher(pattern.SubstringAfter("glob:"));
            if(pattern.StartsWith("re:")) return new HgRePathPatternMatcher(pattern.SubstringAfter("re:"));
            if(pattern.StartsWith("listfile0:")) return new HgListfilePathPatternMatcher(pattern.SubstringAfter("listfile0:"), '0');
            if(pattern.StartsWith("listfile:")) return new HgListfilePathPatternMatcher(pattern.SubstringAfter("listfile:"), '\n');

            //
            // Return Glob by default
            return new HgGlobPathPatternMatcher(pattern);
        }

        public abstract bool Matches(HgPath hgPath);
    }
}