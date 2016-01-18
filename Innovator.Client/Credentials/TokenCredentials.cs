using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  internal class TokenCredentials : ICredentials
  {
    private string _content;
    private DateTime _expiration;

    public string Content
    {
      get { return _content; }
    }
    public string Database { get; set; }
    public DateTime Expiration
    {
      get { return _expiration; }
    }

    internal TokenCredentials(string value)
    {
      _content = value;

      // Extract the middle segment's json
      var parts = value.Split('.');
      if (parts.Length != 3) throw new ArgumentException();
      var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
      
      // Identify the portion of the json which is the expiration in Unix time and calculate
      var expStart = json.IndexOf("\"exp\":") + 6;
      var i = expStart;
      while (i < json.Length && char.IsDigit(json[i])) i++;
      var unixTime = int.Parse(json.Substring(expStart, i - expStart));
      _expiration = UnixEpoch.AddSeconds(unixTime).ToUniversalTime();
    }

    private byte[] Base64UrlDecode(string value)
    {
      var padding = value.Length % 4 == 0 ? 0 : 4 - (value.Length % 4);
      value = value.Replace('-', '+').Replace('_', '/');
      for (var i = 0; i < padding; i++)
        value += "=";
      return Convert.FromBase64String(value);
    }

    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
  }
}
