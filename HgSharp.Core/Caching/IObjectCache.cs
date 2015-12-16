using System;

namespace HgSharp.Core.Caching
{
    public interface IObjectCache
    {
        void Add(string key, object value);

        T GetOrAdd<T>(string key, Func<T> valueProvider);
    }
}
