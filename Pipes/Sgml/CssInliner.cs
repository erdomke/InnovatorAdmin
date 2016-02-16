using Pipes.Css;
using Pipes.Css.Model;
using Pipes.Sgml.Query;
using Pipes.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace Pipes.Sgml
{
  public static class CssInliner
  {
    public static string MoveCssInline(string htmlInput, bool removeStyleElements)
    {
      return MoveCssInline(htmlInput, removeStyleElements, null);
    }
    public static string MoveCssInline(string htmlInput, bool removeStyleElements, Uri basePath)
    {
      using (var reader = new System.IO.StringReader(htmlInput))
      {
        var web = new System.Net.WebClient();
        var sgmlReader = new Pipes.Sgml.SgmlReader();
        sgmlReader.DocType = "HTML";
        sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
        sgmlReader.CaseFolding = CaseFolding.ToLower;
        sgmlReader.InputStream = reader;
        sgmlReader.SimulatedNode = "html";
        sgmlReader.StripDocType = false;
        
        var doc = new XmlDocument();
        doc.Load(sgmlReader);
        var root = doc.DocumentElement;
        if (root.ChildNodes.OfType<XmlElement>().Count() == 1) root = root.ChildNodes.OfType<XmlElement>().Single();

        var parser = new Parser();
        var engine = new HtmlQueryEngine();
        var visitor = new MatchVisitor(engine) { Comparison = StringComparison.OrdinalIgnoreCase };

        var css = new StringBuilder();
        var nodes = doc.SelectNodes("//style").OfType<System.Xml.XmlNode>().ToList();
        foreach (var node in nodes)
        {
          css.AppendLine(node.InnerText);
          if (removeStyleElements) node.ParentNode.RemoveChild(node);
        }

        nodes = doc.SelectNodes("//link[@rel='stylesheet' and @href]").OfType<System.Xml.XmlNode>().ToList();
        foreach (var node in nodes)
        {
          css.Append("@import url('").Append(node.Attributes["href"].Value).Append("')");
          if (node.Attributes["media"] != null && !string.IsNullOrEmpty(node.Attributes["media"].Value))
          {
            css.Append(" ").Append(node.Attributes["media"].Value);
          }
          css.AppendLine(";");
          if (removeStyleElements) node.ParentNode.RemoveChild(node);
        }

        var stylesheet = parser.Parse(css.ToString());
        var elements = root.SelectNodes("//*").OfType<XmlElement>().Select(e => new Pipes.Xml.XmlElementWrapper(e)).ToList();
        var settings = new GlobalStyleContext();
        settings.ResourceLoader = p => {
          var href = (basePath == null ? new Uri(p) : new Uri(basePath, p));
          return DownloadStream(href);
        };

        var rules = stylesheet.Rules.GetStyleRules(settings).ToList();
        IEnumerable<StyleRule> elemRules;
        string inlineStyle;
        IEnumerable<Property> props;
        foreach (var elem in elements)
        {
          inlineStyle = elem.Attribute<string>("style", null);
          elemRules = rules;
          if (!string.IsNullOrEmpty(inlineStyle))
          {
            elemRules = elemRules.Concat(Enumerable.Repeat(Utils.ParseInline(inlineStyle), 1));
          }
          props = elemRules.ApplicableProperties(elem, engine, StringComparison.OrdinalIgnoreCase, settings);
          if (props.Any())
          {
            inlineStyle = props.Select(p => p.ToString()).Aggregate((p, c) => p + ";" + c);
            elem.Attribute("style", inlineStyle);
          }
        }

        return doc.OuterXml;
      }
    }

    private static string DownloadStream(Uri href)
    {
      System.IO.Stream stream;

      if (href.IsFile)
      {
        stream = new System.IO.FileStream(href.LocalPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
      }
      else
      {
        var fileReq = (HttpWebRequest)HttpWebRequest.Create(href);
        var fileResp = (HttpWebResponse)fileReq.GetResponse();
        stream = fileResp.GetResponseStream();
      }

      using (var reader = new System.IO.StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

  }
}
