// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HgSharp.Core.Util
{
    public class DefaultableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dictionary;
        private Func<TKey, TValue> defaultValueProvider; 

        public DefaultableDictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> defaultValueProvider)
        {
            this.dictionary = dictionary;
            this.defaultValueProvider = defaultValueProvider;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Remove(item);
        }

        public int Count
        {
            get { return dictionary.Count; } 
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; } 
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] 
        { 
            get
            {
                if(!dictionary.ContainsKey(key))
                    dictionary[key] = defaultValueProvider(key);

                return dictionary[key];
            }
            set { dictionary[key] = value; } 
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; } 
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
