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
using System.Linq;
using HgSharp.Core.Util;

namespace HgSharp.Core.DiffLib
{
    public class SequenceMatcher<T>
    {
        private readonly T[] source;
        private readonly T[] destination;
        private readonly Func<T, T, bool> equality;

        private readonly IDictionary<T, IList<int>> destinationIndices = new Dictionary<T, IList<int>>(); 

        public SequenceMatcher(T[] source, T[] destination, Func<T, T, bool> equality)
        {
            this.source = source;
            this.destination = destination;
            this.equality = equality;

            Init();
        }

        private void Init()
        {
            var n = destination.Length;
            var popular = new Dictionary<T, bool>();

            for(var i = 0; i < n; ++i)
            {
                var element = destination[i];
                if(destinationIndices.ContainsKey(element))
                {
                    var indices = destinationIndices[element];
                    if(n >= 2000 && (indices.Count * 100 > n))
                    {
                        popular[element] = true;
                        indices.Clear();
                    }
                    else
                        indices.Add(i);
                } // if
                else
                {
                    destinationIndices[element] = new List<int> { i };
                } // else
            } // for

            foreach(var k in popular.Keys)
                destinationIndices.Remove(k);
        }

        public Match GetLongestMatch(int alo, int ahi, int blo, int bhi)
        {
            var j2len = new Dictionary<int, int>();
            int besti = alo, bestj = blo, bestsize = 0;

            for(var i = alo; i < ahi; ++i)
            {
                var newj2len = new Dictionary<int, int>();
                if(destinationIndices.ContainsKey(source[i]))
                {
                    foreach(var j in destinationIndices[source[i]])
                    {
                        if(j < blo) continue;
                        if(j >= bhi) break;

                        var k = newj2len[j] = (j2len.ContainsKey(j - 1) ? j2len[j - 1] : 0) + 1;
                        
                        if(k > bestsize)
                        {
                            besti = i - k + 1;
                            bestj = j - k + 1;
                            bestsize = k;
                        } // if
                    }
                }

                j2len = newj2len;
            } // for

            while(besti > alo && bestj > blo && equality(source[besti - 1], destination[bestj - 1]))
            {
                besti = besti - 1;
                bestj = bestj - 1;
                bestsize = bestsize + 1;
            }

            while(besti + bestsize < ahi && bestj + bestsize < bhi && equality(source[besti + bestsize], destination[bestj + bestsize]))
            {
                bestsize += 1;
            }

            return new Match(besti, bestj, bestsize);
        }

        public IList<Match> GetMatchingBlocks()
        {
            int la = source.Length, lb = destination.Length;
            
            var queue = new Queue<Tuple<int, int, int, int>>();
            queue.Enqueue(Tuple.Create(0, la, 0, lb));
            
            var matching_blocks = new List<Match>();

            while (queue.Count > 0)
            {
                int alo = 0, ahi = 0, blo = 0, bhi = 0;
                queue.Dequeue().Assign(ref alo, ref ahi, ref blo, ref bhi);
                
                var x = GetLongestMatch(alo, ahi, blo, bhi);

                if(x.Length > 0)
                {
                    matching_blocks.Add(x);

                    if(alo < x.SourceIndex && blo < x.DestinationIndex) 
                        queue.Enqueue(Tuple.Create(alo, x.SourceIndex, blo, x.DestinationIndex));
                    if(x.SourceIndex + x.Length < ahi && x.DestinationIndex + x.Length < bhi) 
                        queue.Enqueue(Tuple.Create(x.SourceIndex + x.Length, ahi, x.DestinationIndex + x.Length, bhi));
                }
            }

            matching_blocks.Sort(MatchComparer);

            int i1 = 0, j1 = 0, k1 = 0;

            var non_adjacent = new LinkedList<Tuple<int, int, int>>();
            
            foreach(var x in matching_blocks)
            {
                if(i1 + k1 == x.SourceIndex && j1 + k1 == x.DestinationIndex)
                    k1 += x.Length;
                else
                {
                    if(k1 > 0)
                        non_adjacent.AddLast(Tuple.Create(i1, j1, k1));

                    i1 = x.SourceIndex;
                    j1 = x.DestinationIndex;
                    k1 = x.Length;
                } // else
            }

            if(k1 > 0) 
                non_adjacent.AddLast(Tuple.Create(i1, j1, k1));

            non_adjacent.AddLast(Tuple.Create(la, lb, 0));

            matching_blocks = non_adjacent.Select(t => new Match(t.Item1, t.Item2, t.Item3)).ToList();

            return matching_blocks;

        }

        private static int MatchComparer(Match l, Match r)
        {
            if(l.SourceIndex < r.SourceIndex) return -1;
            if(l.SourceIndex > r.SourceIndex) return 1;
            if(l.DestinationIndex < r.DestinationIndex) return -1;
            if(l.DestinationIndex > r.DestinationIndex) return 1;
            if(l.Length < r.Length) return -1;
            if(l.Length > r.Length) return 1;

            return 0;
        }
    }
}