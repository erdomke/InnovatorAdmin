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

namespace HgSharp.Core.Util
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns an index of <paramref name="nth"/> occurence of <paramref name="value"/> in the <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static int IndexOfNth<T>(this T[] array, T value, int nth)
        {
            var n = 0;
            for(var i = 0; i < array.Length; ++i)
            {
                if(array[i].Equals(value))
                    if(++n == nth) return i;
            } // for

            return -1;
        }

        public static int IndexOf<T>(this T[] array, params T[] values)
            where T: IEquatable<T>
        {
            for(var i = 0; i <= array.Length - values.Length; ++i)
            {
                for(var j = 0; j < values.Length; ++j)
                    if(!array[i + j].Equals(values[j])) goto @continue;

                return i;

                @continue:;
            } // for

            return -1;
        }

        public static T[] Segment<T>(this T[] array, int start, int end)
        {
            return array.SegmentWithLength(start, end - start);
        }

        public static T[] SegmentWithLength<T>(this T[] array, int start, int length)
        {
            length = Math.Min(array.Length - start, length);
            var segment = new T[length];

            for(var i = 0; i < length; ++i)
            {
                segment[i] = array[start + i];
            }

            return segment;
        }

        private static unsafe bool f(ulong v, byte n, byte value)
        {
            unchecked
            {
                var i = (8 * n);
                var @ulong = (v >> i);
                var ulong1 = (@ulong & 0xff);
                return ulong1 == value;
            }

        }

        public static Segment[] Split(this byte[] _src, byte value)
        {
            var previousIndex = -1;
            var segments = new List<Segment>();
            var length = _src.Length;

            if(length > 0)
            {
                int index;

                for(index = 0; index < length; index++)
                {
                    var _value = _src[index];
                    if(_value == value)
                    {
                        segments.Add(new Segment(_src, previousIndex + 1, index - previousIndex));
                        previousIndex = index;
                    }
                }

                if(--index != previousIndex)
                {
                    segments.Add(new Segment(_src, previousIndex + 1, index - previousIndex));
                }
            }

            return segments.ToArray();        
        }
    }
}
