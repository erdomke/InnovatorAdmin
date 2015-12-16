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
using System.IO;
using System.Linq;
using HgSharp.Core.DiffLib;
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public static class BDiff
    {
        public static BDiffBlock[] GetBlocks(byte[] source, byte[] destination)
        {
            var a = source.Split((byte)'\n');
            var b = destination.Split((byte)'\n');

            var matches = new SequenceMatcher<Segment>(a, b, (l, r) => l.Equals(r)).GetMatchingBlocks();
        
            return matches.Select(m => new BDiffBlock(m.SourceIndex, m.SourceIndex + m.Length, m.DestinationIndex, m.DestinationIndex + m.Length)).ToArray();
        }

        public static byte[] Diff(byte[] source, byte[] destination)
        {
            var ms = new MemoryStream();
            var bw = new BigEndianBinaryWriter(ms);

            if(source == null || source.Length == 0)
            {
                bw.Write((uint)0);
                bw.Write((uint)0);
                bw.Write((uint)destination.Length);
                bw.Write(destination);

                bw.Flush();

                return ms.ToArray();
            } // if

            var a = source.Split((byte)'\n');
            var b = destination.Split((byte)'\n');

            var p = new List<int> { 0 };
            Array.ForEach(a, s => p.Add(p[p.Count - 1] + s.Length));

            var d = new SequenceMatcher<Segment>(a, b, (l, r) => l.Equals(r)).GetMatchingBlocks();

            int la = 0, lb = 0;

            foreach(var x in d)
            {
                int am = x.SourceIndex, bm = x.DestinationIndex, size = x.Length;
                var sz = 
                    (lb == bm && lb == 0) ?
                    0 :
                    Enumerable.Range(lb, bm - lb).Select(i => b[i]).Sum(w => w.Length);

                if(am > la || sz > 0)
                {
                    bw.Write((uint)p[la]);
                    bw.Write((uint)p[am]);
                    bw.Write((uint)sz);

                    if(sz > 0)
                    {
                        for(var z = lb; z < bm; ++z)
                            bw.Write(destination, b[z].Offset, b[z].Length);
                    } // if
                } // if

                la = am + size;
                lb = bm + size;
            } // foreach

            bw.Flush();

            return ms.ToArray();
        }
    }

    public class BDiffBlock
    {
        public int StartSource { get; private set; }

        public int EndSource { get; private set; }

        public int StartDestination { get; private set; }

        public int EndDestination { get; private set; }

        public BDiffBlock(int startSource, int endSource, int startDestination, int endDestination)
        {
            StartSource = startSource;
            EndSource = endSource;
            StartDestination = startDestination;
            EndDestination = endDestination;
        }
    }
}