using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  public class QueryString : UriBuilder, IDictionary<string, QueryValue>
  {
    private string _root;
    private Dictionary<string, QueryValue> _dict;

    public ICollection<string> Keys { get { return _dict.Keys; } }
    public ICollection<QueryValue> Values { get { return _dict.Values; } }
    public int Count { get { return _dict.Count; } }
    public bool IsReadOnly { get { return false; } }

    public QueryValue this[string key]
    {
      get
      {
        QueryValue result;
        if (_dict.TryGetValue(key, out result))
          return result;
        return QueryValue.Empty;
      }
      set { _dict[key] = value; }
    }

    public QueryString(string uri) : base(uri)
    {
      Init(uri);
    }

    public QueryString(Uri uri) : base(uri)
    {
      Init(uri.ToString());
    }

    private void Init(string uri)
    {
      var idx = uri.IndexOf('?');
      if (idx >= 0)
        _root = uri.Substring(0, idx + 1);
      else
        _root = string.Empty;
      _dict = (idx < 0 ? uri : uri.Substring(idx + 1))
        .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(k => k.Split('='))
        .Where(k => k.Length == 2)
        .ToLookup(a => Uri.UnescapeDataString(a[0]), a => Uri.UnescapeDataString(a[1]), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(k => k.Key, k => new QueryValue(k), StringComparer.OrdinalIgnoreCase);
    }

    public QueryString Add(string key, QueryValue value)
    {
      QueryValue existing;
      if (_dict.TryGetValue(key, out existing))
        existing.Add(value);
      else
        _dict.Add(key, value);
      return this;
    }

    public QueryString Set(string key, QueryValue value)
    {
      _dict[key] = value;
      return this;
    }

    public QueryString Clear()
    {
      _dict.Clear();
      return this;
    }

    public QueryString Remove(string key)
    {
      _dict.Remove(key);
      return this;
    }

    public bool ContainsKey(string key)
    {
      return _dict.ContainsKey(key);
    }

    void IDictionary<string, QueryValue>.Add(string key, QueryValue value)
    {
      _dict.Add(key, value);
    }

    void ICollection<KeyValuePair<string, QueryValue>>.Clear()
    {
      _dict.Clear();
    }

    bool IDictionary<string, QueryValue>.Remove(string key)
    {
      return _dict.Remove(key);
    }

    public override string ToString()
    {
      var builder = new StringBuilder(_root);
      var first = true;
      foreach (var kvp in _dict)
      {
        foreach (var value in kvp.Value)
        {
          if (!first)
            builder.Append('&');
          builder.Append(Uri.EscapeDataString(kvp.Key));
          builder.Append('=');
          builder.Append(Uri.EscapeDataString(value));
          first = false;
        }
      }
      return builder.ToString();
    }

    public bool TryGetValue(string key, out QueryValue value)
    {
      return _dict.TryGetValue(key, out value);
    }

    public bool TryGetValue(string key, out string value)
    {
      QueryValue result;
      if (_dict.TryGetValue(key, out result))
      {
        value = result.ToString();
        return true;
      }
      value = null;
      return false;
    }

    void ICollection<KeyValuePair<string, QueryValue>>.Add(KeyValuePair<string, QueryValue> item)
    {
      _dict.Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<string, QueryValue>>.Contains(KeyValuePair<string, QueryValue> item)
    {
      return _dict.Contains(item);
    }

    void ICollection<KeyValuePair<string, QueryValue>>.CopyTo(KeyValuePair<string, QueryValue>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, QueryValue>>)_dict).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, QueryValue>>.Remove(KeyValuePair<string, QueryValue> item)
    {
      return _dict.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, QueryValue>> GetEnumerator()
    {
      return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  public class QueryValue : IEnumerable<string>, IEquatable<QueryValue>, ICollection
  {
    private string[] _values;

    public bool HasValue { get { return _values != null && _values.Length > 0; } }

    int ICollection.Count { get { return _values.Length; } }
    object ICollection.SyncRoot { get { return this; } }
    bool ICollection.IsSynchronized { get { return false; } }

    public QueryValue(string value) : this(value, true) { }

    public QueryValue(string value, bool splitOnComma)
    {
      if (splitOnComma)
        _values = value.Split(',');
      else
        _values = new string[] { value };
    }

    public QueryValue(IEnumerable<string> values)
    {
      _values = values.ToArray();
    }

    public void Add(QueryValue value)
    {
      var arr = new string[_values.Length + value._values.Length];
      _values.CopyTo(arr, 0);
      value._values.CopyTo(arr, _values.Length);
      _values = arr;
    }

    public override string ToString()
    {
      if (_values.Length < 1)
        return null;
      return string.Join(",", _values);
    }

    public IEnumerator<string> GetEnumerator()
    {
      return ((IEnumerable<string>)_values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(QueryValue other)
    {
      if (object.ReferenceEquals(_values, other._values))
        return true;

      if (_values == null || other._values == null)
        return false;

      if (_values.Length != other._values.Length)
        return false;

      for (var i = 0; i < _values.Length; i++)
      {
        if (_values[i] != other._values[i])
          return false;
      }

      return true;
    }
    public override bool Equals(object obj)
    {
      var qry = obj as QueryValue;
      if (qry != null)
        return Equals(qry);
      if (obj is string)
        return string.Equals(this.ToString(), (string)obj);
      return false;
    }

    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      _values.CopyTo(array, index);
    }

    public static implicit operator QueryValue(string[] values)
    {
      return new QueryValue(values);
    }

    public static implicit operator QueryValue(string value)
    {
      return new QueryValue(value);
    }

    public static implicit operator string(QueryValue value)
    {
      return value.ToString();
    }

    public static implicit operator string[] (QueryValue value)
    {
      return value._values;
    }

    public static bool operator ==(QueryValue x, QueryValue y)
    {
      if (object.ReferenceEquals(x, y))
        return true;

      if (((object)x == null) || ((object)y == null))
        return false;

      return x.Equals(y);
    }
    public static bool operator !=(QueryValue x, QueryValue y)
    {
      return !(x == y);
    }

    public static string operator +(QueryValue x, QueryValue y)
    {
      return x.ToString() + y.ToString();
    }

    public static string operator +(QueryValue x, string y)
    {
      return x.ToString() + y.ToString();
    }

    public static explicit operator DateTime(QueryValue x)
    {
      return ((DateTime?)x).Value;
    }

    public static explicit operator DateTime? (QueryValue x)
    {
      if (string.IsNullOrEmpty(x.ToString()))
        return null;
      if (x._values.Length > 1)
        throw new FormatException();
      return DateTime.Parse(x._values[0]);
    }

    public static explicit operator double(QueryValue x)
    {
      return ((double?)x).Value;
    }

    public static explicit operator double? (QueryValue x)
    {
      if (string.IsNullOrEmpty(x.ToString()))
        return null;
      if (x._values.Length > 1)
        throw new FormatException();
      return double.Parse(x._values[0]);
    }

    public static explicit operator Guid(QueryValue x)
    {
      return ((Guid?)x).Value;
    }

    public static explicit operator Guid? (QueryValue x)
    {
      if (string.IsNullOrEmpty(x.ToString()))
        return null;
      if (x._values.Length > 1)
        throw new FormatException();
      return new Guid(x._values[0]);
    }

    public static explicit operator int(QueryValue x)
    {
      return ((int?)x).Value;
    }

    public static explicit operator int? (QueryValue x)
    {
      if (string.IsNullOrEmpty(x.ToString()))
        return null;
      if (x._values.Length > 1)
        throw new FormatException();
      return int.Parse(x._values[0]);
    }

    public static explicit operator long(QueryValue x)
    {
      return ((long?)x).Value;
    }

    public static explicit operator long? (QueryValue x)
    {
      if (string.IsNullOrEmpty(x.ToString()))
        return null;
      if (x._values.Length > 1)
        throw new FormatException();
      return long.Parse(x._values[0]);
    }
    private static QueryValue _empty = new QueryValue(new string[] { });
    public static QueryValue Empty { get { return _empty; } }
  }
}
