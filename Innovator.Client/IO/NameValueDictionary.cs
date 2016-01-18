using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  internal class NameValueDictionary : IDictionary<string, string>
  {
    private NameValueCollection _inner;

    public int Count
    {
      get { return _inner.Count; }
    }

    public ICollection<string> Keys
    {
      get { return _inner.AllKeys; }
    }

    public string this[string key]
    {
      get { return _inner[key]; }
      set { _inner[key] = value; }
    }

    bool ICollection<KeyValuePair<string, string>>.IsReadOnly
    {
      get { return false; }
    }

    ICollection<string> IDictionary<string, string>.Values
    {
      get { return this.Select(k => k.Value).ToList(); }
    }

    public NameValueDictionary(NameValueCollection inner)
    {
      _inner = inner;
    }

    public void Add(string key, string value)
    {
      _inner.Add(key, value);
    }

    public void Clear()
    {
      _inner.Clear();
    }

    public bool ContainsKey(string key)
    {
      return _inner.AllKeys.Contains(key);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      for (var i = 0; i < _inner.Count; i++)
      {
        yield return new KeyValuePair<string, string>(_inner.GetKey(i), _inner[i]);
      }
    }

    public bool Remove(string key)
    {
      _inner.Remove(key);
      return true;
    }

    public bool TryGetValue(string key, out string value)
    {
      value = _inner[key];
      if (value == null) return this.ContainsKey(key);
      return true;
    }

    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
    {
      this.Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
    {
      return this.ContainsKey(item.Key);
    }

    void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
      this.ToArray().CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
    {
      return this.Remove(item.Key);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
