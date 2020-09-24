using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace InnovatorAdmin
{
  public enum ItemQueryType
  {
    ById,
    Current,
    Released
  }

  /// <summary>
  /// Represents a query for an item parsed from a URL or XML segment
  /// </summary>
  public class ItemQuery
  {
    public string Type { get; private set; }
    public string Id { get; private set; }
    public string ConfigId { get; private set; }
    public ItemQueryType QueryType { get; private set; } = ItemQueryType.ById;

    private ItemQuery() { }

    public static bool TryParseQuery(string value, out ItemQuery query)
    {
      query = null;
      if (value.StartsWith("<"))
      {
        if (value.StartsWith("<af:"))
          value = "<" + value.Substring(4);
        try
        {
          using (var reader = new StringReader(value))
          using (var xml = XmlReader.Create(reader))
          {
            if (xml.Read() && xml.NodeType == XmlNodeType.Element)
            {
              query = new ItemQuery()
              {
                Type = xml.GetAttribute("itemtype") ?? xml.GetAttribute("type"),
                Id = xml.GetAttribute("id")
              };
              if (string.IsNullOrEmpty(query.Id))
              {
                if (xml.Read() && xml.NodeType == XmlNodeType.Text)
                  query.Id = xml.Value;
                else
                  return false;
              }
              return true;
            }
          }
        }
        catch (XmlException) { }
      }
      else if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
      {
        if (string.Equals(uri.Scheme, "javascript", StringComparison.OrdinalIgnoreCase))
        {
          var match = Regex.Match(uri.LocalPath, @"aras\.uiShowItem\(['""]([A-Za-z0-9_% ]+)['""], ['""]([A-F0-9]{32})['""]\)", RegexOptions.IgnoreCase);
          if (match.Success)
          {
            query = new ItemQuery()
            {
              Type = match.Groups[1].Value,
              Id = match.Groups[2].Value,
            };
            return true;
          }
        }
        else
        {
          var match = Regex.Match(uri.Query, @"^\?StartItem=([A-Za-z0-9_% ]+):([A-F0-9]{32})(:released|:current)?(\&.*)?");
          if (match.Success)
          {
            query = new ItemQuery()
            {
              Type = match.Groups[1].Value
            };
            switch ((match.Groups[3].Value ?? "").ToUpperInvariant())
            {
              case ":RELEASED":
                query.ConfigId = match.Groups[2].Value;
                query.QueryType = ItemQueryType.Released;
                break;
              case ":CURRENT":
                query.ConfigId = match.Groups[2].Value;
                query.QueryType = ItemQueryType.Current;
                break;
              case "":
                query.Id = match.Groups[2].Value;
                break;
              default:
                return false;
            }
            return true;
          }
        }
      }
      return false;
    }
  }
}
