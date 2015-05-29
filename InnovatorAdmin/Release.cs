using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace Aras.Tools.InnovatorAdmin
{
  public class Release
  {
    public string DownloadUrl { get; set; }
    public DateTime? PublishDate { get; set; }
    public string Version { get; set; }

    private Release() { }

    public static Release GetLatest()
    {
      var assemName = typeof(Release).Assembly.GetName();
      var client = new System.Net.WebClient();
      client.Headers.Add("user-agent", "/erdomke/InnovatorAdmin");
      var releaseBytes = client.DownloadData("https://api.github.com/repos/erdomke/InnovatorAdmin/releases");
      var jsonReader = JsonReaderWriterFactory.CreateJsonReader(releaseBytes, new System.Xml.XmlDictionaryReaderQuotas());
      var doc = XElement.Load(jsonReader);
      var latestRelease = doc.Elements().OrderByDescending(e => ReformatVersion(e.Element("tag_name").Value)).First();
      var result = new Release();
      result.DownloadUrl = latestRelease.Element("assets").Elements("item").First().Element("browser_download_url").Value;
      DateTime publishDate;
      if (DateTime.TryParse(latestRelease.Element("published_at").Value, out publishDate)) result.PublishDate = publishDate;
      result.Version = latestRelease.Element("tag_name").Value;
      return result;
    }

    public static string ReformatVersion(string version)
    {
      var parts = version.Split('.');
      return parts.Select(p => ("00000" + p).Substring(p.Length)).Aggregate((p, c) => p + "." + c);
    }
  }
}
