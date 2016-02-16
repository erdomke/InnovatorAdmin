using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes;
using System.Diagnostics;

namespace Pipes.Sgml
{
  public class HtmlToText
  {
    public string Convert(string html)
    {
      var reader = new Pipes.IO.StringTextSource(html).Pipe(new Pipes.Sgml.SgmlTextReader());
      var result = new System.Text.StringBuilder();

      foreach (var node in reader)
      {
        switch (node.Type)
        {
          case Xml.XmlNodeType.Element:
            break;
          case Xml.XmlNodeType.EndElement:
            if(node.Name.LocalName.ToLowerInvariant() == "p") result.AppendLine();
            break;
          case Xml.XmlNodeType.Text:
            result.Append(node.Value);
            break;
        }
      }

      return result.ToString().Trim();
    }
  }
}
