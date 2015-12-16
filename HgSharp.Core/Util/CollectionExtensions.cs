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
using System.Text;

namespace HgSharp.Core.Util
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach(var item in items)
                collection.Add(item);
        }

        public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var elements = collection.Where(predicate).ToList();
            foreach(var element in elements) collection.Remove(element);
        }
    }
}
