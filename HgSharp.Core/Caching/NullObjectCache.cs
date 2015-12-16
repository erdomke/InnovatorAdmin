using System;

namespace HgSharp.Core.Caching
{
    public class NullObjectCache : IObjectCache
    {
        public void Add(string key, object value)
        {
        }

        public T GetOrAdd<T>(string key, Func<T> valueProvider)
        {
            return valueProvider();
        }
    }
}