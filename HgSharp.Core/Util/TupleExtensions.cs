// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;

namespace HgSharp.Core.Util
{
    public static class TupleExtensions
    {
        public static Tuple<T1, T2, T3> Assign<T1, T2, T3>(this Tuple<T1, T2, T3> t, ref T1 t1, ref T2 t2, ref T3 t3)
        {
            t1 = t.Item1;
            t2 = t.Item2;
            t3 = t.Item3;

            return t;
        }

        public static Tuple<T1, T2, T3, T4> Assign<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> t, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4)
        {
            t1 = t.Item1;
            t2 = t.Item2;
            t3 = t.Item3;
            t4 = t.Item4;

            return t;
        }
    }
}