// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Text.RegularExpressions;

namespace HgSharp.Core
{
    public class HgGlobPathPatternMatcher : HgPathPatternMatcher
    {
        private readonly Regex regex;

        public HgGlobPathPatternMatcher(string pattern)
        {
            regex = ParseGlob(pattern);
        }

        public override bool Matches(HgPath hgPath)
        {
            return regex.IsMatch(hgPath.FullPath.TrimStart('/'));
        }

        private static Regex ParseGlob(string pattern)
        {
            var i = 0;
            var n = pattern.Length;
            var res = "";
            var group = 0;

            Func<string> peek = () => i < n ? pattern[i].ToString() : null;

            while(i < n)
            {
                string c = pattern[i].ToString();
                i += 1;
                if(!@"*?[{},\\".Contains(c))
                    res += Regex.Escape(c);
                else if(c == "*")
                {
                    if(peek() == "*")
                    {
                        i += 1;
                        res += ".*";
                    }
                    else
                    {
                        res += "[^/]*";
                    }
                }
                else if(c == "?")
                {
                    res += ".";
                }
                else if(c == "[")
                {
                    var j = i;
                    if(j < n && "!]".Contains(pattern[j].ToString()))
                    {
                        j += 1;
                    }
                    while(j < n && pattern[j] != ']')
                    {
                        j += 1;
                    }
                    if(j >= n)
                    {
                        res += "\\[";
                    }
                    else
                    {
                        var stuff = pattern.Substring(i, j - i).Replace("\\", "\\\\");
                        i = j + 1;

                        if(stuff[0] == '!')
                            stuff = "^" + stuff.Substring(1);
                        else if(stuff[0] == '^')
                            stuff = "\\" + stuff;

                        res = res + "[" + stuff + "]";
                    }
                }
                else if(c == "{")
                {
                    group += 1;
                    res += "(?:";
                }
                else if(c == "}" && group > 0)
                {
                    res += ")";
                    group -= 1;
                }
                else if(c == "," && group > 0)
                {
                    res += "|";
                }
                else if(c == "\\")
                {
                    var p = peek();
                    if(p != null)
                    {
                        i += 1;
                        res += Regex.Escape(p);
                    }
                    else
                    {
                        res += Regex.Escape(c);
                    }
                }
                else
                {
                    res += Regex.Escape(c);
                }
            } // while

            return new Regex(res);
        }
    }
}