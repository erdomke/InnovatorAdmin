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
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> MissingIn<T>(this IEnumerable<T> @this, IEnumerable<T> other, Func<T, T, bool> equality)
        {
            if(@this == null) throw new ArgumentNullException("this");
            if(other == null) throw new ArgumentNullException("other");
            if(equality == null) throw new ArgumentNullException("equality");

            return MissingInImpl(@this, other, equality);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            if(@this == null) throw new ArgumentNullException("this");
            if(other == null) throw new ArgumentNullException("other");

            return AppendImpl(@this, other);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            if(@this == null) throw new ArgumentNullException("this");
            if(other == null) throw new ArgumentNullException("other");

            return PrependImpl(@this, other);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T other)
        {
            if(@this == null) throw new ArgumentNullException("this");

            return @this.Append(new [] { other });
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T other)
        {
            if(@this == null) throw new ArgumentNullException("this");

            return @this.Prepend(new [] { other });
        }

        private static IEnumerable<T> AppendImpl<T>(IEnumerable<T> @this, IEnumerable<T> other)
        {
            foreach(var t in @this) yield return t;
            foreach(var o in other) yield return o;
        }

        private static IEnumerable<T> PrependImpl<T>(IEnumerable<T> @this, IEnumerable<T> other)
        {
            foreach(var o in other) yield return o;
            foreach(var t in @this) yield return t;
        }

        private static IEnumerable<T> MissingInImpl<T>(IEnumerable<T> @this, IEnumerable<T> other, Func<T, T, bool> equality)
        {
            return @this.Where(t => other.All(o => !equality(t, o)));
            //var t = new HashSet<T>(@this, new MissingInEqualityComparer<T>(equality));
            //var o = new HashSet<T>(other);

            return new HashSet<T>(@this, new MissingInEqualityComparer<T>(equality)).Except(other, new MissingInEqualityComparer<T>(equality));

            //return @this.Where(t => other.All(o => !equality(t, o)));
        }
       
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }
        
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if(source == null) throw new ArgumentNullException("source");
            if(selector == null) throw new ArgumentNullException("selector");
            if(comparer == null) throw new ArgumentNullException("comparer");
            
            using(var sourceIterator = source.GetEnumerator())
            {
                if(!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");
                
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                
                while(sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    
                    if(comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    } // if
                } // while

                return max;
            } // using
        }


        internal class MissingInEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> equality;

            public MissingInEqualityComparer(Func<T, T, bool> equality)
            {
                this.equality = equality;
            }

            public bool Equals(T x, T y)
            {
                return equality(x, y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
