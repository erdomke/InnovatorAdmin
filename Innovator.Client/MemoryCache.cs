using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Innovator.Client.Connection
{
  public class MemoryCache<TKey, TValue>
  {
    private object _mutex = new object();
    private long _size;
    private Dictionary<TKey, CacheEntry> _cache = new Dictionary<TKey, CacheEntry>();

    public long MaxSize { get; set; }

    public MemoryCache()
    {
      this.MaxSize = 100 * 1024 * 1024; // 100 Mb by default
    }

    public bool TryAdd(TKey key, TValue value)
    {
      lock (_mutex)
      {
        if (_cache.ContainsKey(key)) return false;
        var entry = new CacheEntry() { Key = key, Value = value, LastAccess = DateTime.UtcNow };
        if (entry.Size > this.MaxSize) this.MaxSize = (long)(entry.Size * 1.1);
        _size += entry.Size;
        _cache.Add(key, entry);
      }
      Cleanup();
      return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      lock (_mutex)
      {
        value = default(TValue);
        CacheEntry entry;
        if (!_cache.TryGetValue(key, out entry)) return false;
        
        entry.LastAccess = DateTime.UtcNow;
        value = entry.Value;
        return true;
      }
    }

    private void Cleanup()
    {
      while (_size > this.MaxSize && _cache.Count > 0)
      {
        lock (_mutex)
        {
          var lastUsed = _cache.Values.OrderBy(e => e.LastAccess).First();
          _size -= lastUsed.Size;
          _cache.Remove(lastUsed.Key);
        }
      }
    }

    private class CacheEntry
    {
      public TKey Key { get; set; }
      public TValue Value { get; set; }
      public DateTime LastAccess { get; set; }
      public int Size
      {
        get 
        { 
          if (typeof(TValue).IsArray)
          {
            if (this.Value == null) return 0;
            var list = (IList)this.Value;
            var count = list.Count;
            if (count == 0) return 0;
            return count * Marshal.SizeOf(list[0]);
          }
          return Marshal.SizeOf(this.Value); 
        }
      }
    }
  }
}
