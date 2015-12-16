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
using System.IO;
using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public static class MPatch
    {
        [DebuggerDisplay("{start} {end} {len}")]
        struct Fragment
        {
            public int Start { get; set; }
            public int End { get; set; }
            public int Length { get; set; }
            public int Offset { get; set; }
            public byte[] Data { get; set; }

            public Fragment(int start, int end, int length, int offset, byte[] data) : this()
            {
                Start = start;
                End = end;
                Length = length;
                Offset = offset;
                Data = data;
            }
        }

        class FragmentList
        {
            private readonly LinkedList<Fragment> fragments;

            public LinkedListNode<Fragment> Head { get; set; }

            public LinkedListNode<Fragment> Tail { get; private set; }

            public FragmentList()
            {
                fragments = new LinkedList<Fragment>();
            }

            public void Add(Fragment fragment)
            {
                var f = new Fragment {
                    Start = fragment.Start,
                    End = fragment.End,
                    Length = fragment.Length,
                    Data = fragment.Data,
                    Offset = fragment.Offset
                };

                if(fragments.Count == 0)
                {
                    fragments.AddFirst(f);
                    Head = fragments.First;

                    fragments.AddAfter(Head, default(Fragment));
                    Tail = fragments.Last;
                }
                else
                    fragments.AddBefore(Tail, f);
            }
        }

        static int Gather(FragmentList dest, FragmentList src, int cut, int offset)
        {
            unchecked
            {
                int postend, c, l;
                var s = src.Head;

                while(s != src.Tail)
                {
                    if(s.Value.Start + offset >= cut)
                        break; /* We've gone far enough */

                    postend = offset + s.Value.Start + s.Value.Length;
                    if(postend <= cut)
                    {
                        /* Save this hunk */
                        offset += s.Value.Start + s.Value.Length - s.Value.End;
                        dest.Add(s.Value);
                        s = s.Next;
                    } // if
                    else
                    {
                        /* Break up this hunk */
                        c = cut - offset;
                        if(s.Value.End < c)
                        {
                            c = s.Value.End;
                        }
                        l = cut - offset - s.Value.Start;
                        if(s.Value.Length < l)
                            l = s.Value.Length;

                        offset += s.Value.Start + l - c;

                        Fragment d = new Fragment();

                        d.Start = s.Value.Start;
                        d.End = c;
                        d.Length = l;
                        d.Data = s.Value.Data;
                        d.Offset = s.Value.Offset;

                        dest.Add(d);

                        s.Value = new Fragment(c, s.Value.End, s.Value.Length - l, s.Value.Offset + l, s.Value.Data);
                        /*s.Value.Start = c;
                        s.Value.Length = s.Value.Length - l;
                        s.Value.Offset = s.Value.Offset + l;*/

                        break;
                    } // else
                }

                src.Head = s;

                return offset;
            }
        }

        static int Discard(FragmentList src, int cut, int offset)
        {
            unchecked
            {
                int postend, c, l;
                var s = src.Head;

                while(s != src.Tail)
                {
                    if(s.Value.Start + offset >= cut)
                        break; /* We've gone far enough */

                    postend = offset + s.Value.Start + s.Value.Length;
                    if(postend <= cut)
                    {
                        /* Save this hunk */
                        offset += s.Value.Start + s.Value.Length - s.Value.End;
                        s = s.Next;
                    } // if
                    else
                    {
                        /* Break up this hunk */
                        c = cut - offset;
                        if(s.Value.End < c)
                        {
                            c = s.Value.End;
                        }
                        l = cut - offset - s.Value.Start;
                        l = cut - offset - s.Value.Start;
                        if(s.Value.Length < l)
                            l = s.Value.Length;

                        offset += s.Value.Start + l - c;

                        s.Value = new Fragment(c, s.Value.End, s.Value.Length - l, s.Value.Offset + l, s.Value.Data);
                        /*s.Value.Start = c;
                        s.Value.Length = s.Value.Length - l;
                        s.Value.Offset = s.Value.Offset + l;*/

                        break;
                    } // else
                }

                src.Head = s;

                return offset;
            }
        }

        private static FragmentList Combine(FragmentList a, FragmentList b)
        {
            unchecked
            {
                int offset = 0;
                int post;
                FragmentList c = new FragmentList();


                for(var bh = b.Head; bh != b.Tail; bh = bh.Next)
                {
                    offset = Gather(c, a, bh.Value.Start, offset);
                    post = Discard(a, bh.Value.End, offset);

                    var ct = new Fragment {
                        Start = bh.Value.Start - offset,
                        End = bh.Value.End - post,
                        Length = bh.Value.Length,
                        Data = bh.Value.Data,
                        Offset = bh.Value.Offset
                    };

                    c.Add(ct);

                    offset = post;
                }

                var t = a.Head;
                for(; t != a.Tail; t = t.Next)
                    c.Add(t.Value);

                return c;
            }
        }

        private static FragmentList Decode(IBinaryReader patchReader)
        {
            var l = new FragmentList();
            
            while(patchReader.BaseStream.Position != patchReader.BaseStream.Length)
            {
                var start = patchReader.ReadUInt32();
                var end = patchReader.ReadUInt32();
                var len = patchReader.ReadUInt32();
                var data = patchReader.ReadBytes((int)len);
                
                var f = new Fragment {
                    Start = (int)start,
                    End = (int)end,
                    Length = (int)len,
                    Data = data
                };

                //Console.WriteLine("chunk:" + start + " " + end + " " + len);

                l.Add(f);
            }

            return l;
        }

        private static int GetPatchedSize(int len, FragmentList l)
        {
            unchecked
            {
                int outlen = 0;
                int last = 0;

                var f = l.Head;

                while(f != l.Tail)
                {
                    if(f.Value.Start < last || f.Value.End > len)
                        throw new ApplicationException("invalid patch");

                    outlen += f.Value.Start - last;
                    last = f.Value.End;
                    outlen += f.Value.Length;

                    f = f.Next;
                }

                outlen += len - last;
                return outlen;
            }
        }

        private static int Apply(byte[] buf, byte[] orig, int len, FragmentList l)
        {
            unchecked
            {
                var last = 0;
                var p = 0;

                for(var f = l.Head; f != l.Tail; f = f.Next)
                {
                    if(f.Value.Start < last || f.Value.End > len)
                        throw new ArgumentException("invalid patch");

                    Buffer.BlockCopy(orig, last, buf, p, f.Value.Start - last);
                    p += f.Value.Start - last;
                    Buffer.BlockCopy(/* src */ f.Value.Data, f.Value.Offset, /* dst */ buf, p, f.Value.Length);
                    last = f.Value.End;
                    p += f.Value.Length;
                }

                Buffer.BlockCopy(orig, last, buf, p, len - last);
                return 1;
            }
        }

        private static FragmentList Fold(IList<byte[]> bins, int start, int end)
        {
            unchecked
            {
                if(start + 1 == end)
                {
                   /* var hash = bins.Sum(b => (uint)b);
                    Console.WriteLine(bins.Length + " hash " + hash);*/
                    using(var br = new BigEndianBinaryReader(new MemoryStream(bins[start])))
                        return Decode(br);
                }

                var len = (end - start) / 2;
                var l = Fold(bins, start, start + len);
                var r = Fold(bins, start + len, end);

                return Combine(l, r);
            }
        }

        public static byte[] Patch(byte[] text, IList<byte[]> patches)
        {
            var p = Fold(patches, 0, patches.Count);
            var outlen = GetPatchedSize(text.Length, p);
            var @out = new byte[outlen];
            Apply(@out, text, text.Length, p);

            return @out;
        }
    }
}
