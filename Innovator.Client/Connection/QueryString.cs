using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Innovator.Client.Connection
{
  public class QueryString : IEnumerable<KeyValuePair<string, string>>
  {
    private List<KeyValuePair<string, string>> _values = new List<KeyValuePair<string, string>>();

    public QueryString() { }
    public QueryString(string query)
    {
      if (string.IsNullOrEmpty(query)) return;

      var parts = HttpUtility.ParseQueryString(query);
      foreach (var key in parts.AllKeys)
      {
        Add(key, parts[key]);
      }
    }
    public QueryString(Uri url) : this(url.Query) { }

    public void Add(string key, string value)
    {
      _values.Add(new KeyValuePair<string, string>(key, value));
    }

    public override string ToString()
    {
      if (!_values.Any()) return string.Empty;

      var builder = new StringBuilder("?");
      var first = true;
      foreach (var kvp in _values)
      {
        if (!first) builder.Append("&");
        builder.Append(Uri.EscapeUriString(kvp.Key)).Append("=").Append(Uri.EscapeUriString(kvp.Value));
        first = false;
      }
      return builder.ToString();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return _values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _values.GetEnumerator();
    }
  }
}
