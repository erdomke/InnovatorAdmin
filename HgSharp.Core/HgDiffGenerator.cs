// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgDiffGenerator
    {
        class Hunk
        {
            public int start_a, end_a, start_b, end_b;
            public IList<string> delta;

            public Hunk()
            {
            }

            public Hunk(int startA, int endA, int startB, int endB)
            {
                start_a = startA;
                end_a = endA;
                start_b = startB;
                end_b = endB;
                delta = new List<string>();
            }
        }


        static int __context = 3;

        private static readonly Regex ws = new Regex(@"[ \t]+");
        private static readonly Regex xws = new Regex(@"[ \t]+\n");
        private static readonly Regex bl = new Regex(@"\n+");

        private static string CleanWhitespace(string text, bool ignoreWhitespace, bool ignoreExcessiveWhitespace, bool ignoreBlankLines)
        {
            var result = text;

            if(ignoreWhitespace)
                result = ws.Replace(result, "");
            else if(ignoreExcessiveWhitespace)
            {
                result = ws.Replace(text, " ");
                result = xws.Replace(result, "\n");
            }

            if(ignoreBlankLines)
                result = bl.Replace(result, "\n");

            return result;
        }

        public HgUnifiedDiff UnifiedDiff(string a, string b, int context, bool ignoreWhitespace = false, bool ignoreExcessiveWhitespace = false, bool ignoreBlankLines = false)
        {
            var diff = Diff(a, b);
            var hunks = new List<HgUnifiedDiffHunk>();

            if(diff.Lines.Count == 0)
                return new HgUnifiedDiff(new HgUnifiedDiffHunk[] {});
            
            //
            // Collapsing lines
            HgDiffFragmentType? prevType = null;
            var prevCount = 0;
            var prevStart = 0;

            var collapsed = new List<Tuple<HgDiffFragmentType, int, int, int>>();

            foreach(var l in diff.Lines)
            {
                if(prevType == null)
                {
                    prevType = l.Type;
                    prevCount++;
                }
                else if(prevType.Value != l.Type)
                {
                    collapsed.Add(Tuple.Create(prevType.Value, prevStart, prevCount, collapsed.Count));
                    prevStart += prevCount;
                    prevCount = 1;
                    prevType = l.Type;
                }
                else
                {
                    prevCount++;
                }
            }

            var dt = diff.Lines.Count - (prevStart + prevCount);
            if(dt == 0)
            {
                collapsed.Add(Tuple.Create(prevType.Value, prevStart, prevCount, collapsed.Count));
            }

            var total = collapsed.Sum(l => l.Item3);
            Debug.Assert(total == diff.Lines.Count);

            if(collapsed.Count == 1)
            {
                return new HgUnifiedDiff(new[] { new HgUnifiedDiffHunk(diff.Lines) });
            } // if

            var splitters = collapsed.Where(c => c.Item1 == HgDiffFragmentType.Unchanged && (c.Item3 >= context * 2 || c.Item4 == 0 || c.Item4 == collapsed.Count - 1)).ToList();
            
            //
            // The whole thing is a diff
            if(splitters.Count == 0)
            {
                var hunk = new HgUnifiedDiffHunk(diff.Lines); 
                return new HgUnifiedDiff(new[] { hunk });
            } // if

            var cstart = 
                collapsed[0].Item1 == HgDiffFragmentType.Unchanged ?
                    -1 : 
                    0;
            var cend = -1;

            foreach(var splitter in splitters)
            {
                if(cstart == -1)
                {
                    cstart = Math.Max(splitter.Item2 + splitter.Item3 - context, 0);

                    /*var lines = diff.Lines.Take(splitter.Item2 + context).ToList();
                    hunks.Add(new HgUnifiedDiffHunk(lines.Select(l => new HgUnifiedDiffLine(l.Type, l.Line, l.Index))));

                    cstart = splitter.Item2 + splitter.Item3 - context;*/
                }
                else
                {
                    cend = splitter.Item2 + context;

                    var lines = diff.Lines.Skip(cstart).Take(cend - cstart).ToList();
                    cstart = splitter.Item2 + splitter.Item3 - context;

                    var added = lines.Where(l => l.Added).Select(l => l.Content).ToList();
                    var removed = lines.Where(l => l.Removed).Select(l => l.Content).ToList();

                    /*if(added.Count > 0 && removed.Count > 0)
                    {
                        if(ignoreBlankLines || ignoreExcessiveWhitespace || ignoreWhitespace)
                            if(CleanWhitespace(string.Join("\n", added), ignoreWhitespace, ignoreExcessiveWhitespace, ignoreBlankLines) ==
                                CleanWhitespace(string.Join("\n", removed), ignoreWhitespace, ignoreExcessiveWhitespace, ignoreBlankLines))
                            continue;
                    }*/

                    hunks.Add(new HgUnifiedDiffHunk(lines));
                }
            } // foreach

            if(cstart != -1 && cstart < diff.Lines.Count - context)
            {
                var lines = diff.Lines.Skip(cstart).Take(diff.Lines.Count - cstart).ToList();
                hunks.Add(new HgUnifiedDiffHunk(lines));
            }


            return new HgUnifiedDiff(hunks);
        }

        public HgDiff Diff(string a, string b)
        {
            var al = a.Split('\n').Select(l => l.TrimEnd('\n', '\r') + "\n").ToArray();
            var bl = b.Split('\n').Select(l => l.TrimEnd('\n', '\r') + "\n").ToArray();

            var diff = BDiff.GetBlocks(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b)).
                Select(block => new Hunk(block.StartSource, block.EndSource, block.StartDestination, block.EndDestination)).
                ToList();

            var lines = new List<HgDiffLineFragment>();

            Hunk prev = new Hunk();
            int oa = 0, ob = 0, idx = 0;
            for(var i = 0; i < diff.Count; ++i)
            {
                prev = i == 0 ? new Hunk() : diff[i - 1];
                var d = diff[i];
                int na = d.start_a, nb = d.start_b;

                lines.AddRange(al.Segment(oa, na).Select((l, il) => new HgDiffLineFragment(HgDiffFragmentType.Removed, l.TrimEnd('\n'), idx++, oa++, -1)));
                lines.AddRange(bl.Segment(ob, nb).Select((l, il) => new HgDiffLineFragment(HgDiffFragmentType.Added, l.TrimEnd('\n'), idx++, -1, ob++)));
                lines.AddRange(al.Segment(d.start_a, d.end_a).Select((l, il) => new HgDiffLineFragment(HgDiffFragmentType.Unchanged, l.TrimEnd('\n'), idx++, oa++, ob++)));


                oa = d.end_a;
                ob = d.end_b;
            }

            return new HgDiff(lines);
        }

        public string[] GenerateDiff(string a, DateTime ad, string b, DateTime bd, string fn1, string fn2)
        {
            if(string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return new string[]{};
        
            if(/* binary */ false)
            {
                
            }
            else if(string.IsNullOrEmpty(a))
            {
                
            }
            else if(string.IsNullOrEmpty(b))
            {
                
            }
            else
            {
                var al = a.Split('\n').Select(l => l.TrimEnd('\n') + "\n").ToArray();
                var bl = b.Split('\n').Select(l => l.TrimEnd('\n') + "\n").ToArray();

                var udiff = UniDiff(a, b, al, bl, "a/" + fn1, "b/" + fn2);

                return udiff;
            }

            return null;
        }

        
        private string[] UniDiff(string a, string b, string[] l1, string[] l2, string fn1, string fn2)
        {
            return null;
            /*var header = new List<string> { "--- h1", "+++ h2" };
            var return_hunks = new List<string>();
            var saved_delta = new List<string>();
            var delta = new List<string>();

            var diff = BDiff.GetBlocks(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b)).
                Select(block => new Hunk(block.StartSource, block.EndSource, block.StartDestination, block.EndDestination)).
                ToList();

            Hunk hunk = null;

            for(var i = 0; i < diff.Count; ++i)
            {
                var s = (i > 0) ? diff[i - 1] : new Hunk();

                if(delta.Count > 0) saved_delta.AddRange(delta);
                
                delta.Clear();

                var s1 = diff[i];
                var a1 = s.end_a;
                var a2 = s1.start_a;
                var b1 = s.end_b;
                var b2 = s1.start_b;

                var old = (a2 == 0) ? new string[] { } : l1.Segment(a1, a2);
                var newb = (b2 == 0) ? new string[] { } : l2.Segment(b1, b2);

                if(old.Length == 0 && newb.Length == 0) continue;

                /* if opts[:ignore_ws] || opts[:ignore_blank_lines] || opts[:ignore_ws_amount]
                      next if whitespace_clean(old.join,opts) == whitespace_clean(newb.join,opts)
                   end *

                var astart = context_start(a1);
                var bstart = context_start(b1);

                Hunk prev = null;
                if(hunk != null)
                {
                    if(astart < hunk.end_a + __context + 1)
                    {
                        prev = hunk;
                        astart = hunk.end_a;
                        bstart = hunk.end_b;
                    }
                    else
                    {
                        foreach(var h in yield_hunk(hunk, header, l1.ToList(), delta))
                            return_hunks.Add(h);

                        header = null;
                    }
                }

                if(prev != null)
                {
                    hunk.end_a = a2;
                    hunk.end_b = b2;
                    delta = hunk.delta.ToList();
                }
                else
                {
                    hunk = new Hunk(astart, a2, bstart, b2) { delta = delta };
                }

                if(a1 > 0)
                    hunk.delta.AddRange(l1.Segment(astart, a1).Select(l => " " + l));
                
                hunk.delta.AddRange(old.Select(l => "-" + l));
                hunk.delta.AddRange(newb.Select(l => "+" + l));
            } // for

            saved_delta.AddRange(delta);

            if(hunk != null)
                return_hunks.AddRange(yield_hunk(hunk, header, l1.ToList(), saved_delta));

            return return_hunks.ToArray();*/
        }

        private IEnumerable<string> yield_hunk(Hunk hunk, List<string> header, List<string> l1, List<string> delta)
        {
            if(header != null && header.Count > 0)
                foreach(var h in header) yield return h;

            delta = hunk.delta.ToList();
            int astart = hunk.start_a, a2 = hunk.end_a, bstart = hunk.start_b, b2 = hunk.end_b;
            var aend = context_end(a2, l1.Count);
            var alen = aend - astart;
            var blen = b2 - bstart + aend - a2;

            yield return string.Format("@@ -{0},{1} +{2},{3} @@", astart + 1, alen, bstart + 1, blen);

            foreach(var d in delta) yield return d;

            for(var i = a2; i < aend - 1L; ++i)
                yield return l1[i];

        }

        private int context_end(int l, int len)
        {
            var ret = len + __context;
            if(ret > len) ret = len;

            return ret;
        }

        private int context_start(int l)
        {
            var ret = l - __context; 
            if(ret < 0) return 0;
            return ret;
        }
    }
}
