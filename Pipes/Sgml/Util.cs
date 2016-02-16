using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Xml;
using System.Web.UI;

namespace Pipes.Sgml
{
  public static class Util
  {
    public static ISgmlWriter Attribute(this ISgmlWriter writer, HtmlTextWriterAttribute name, object value)
    {
      writer.Attribute(name.ToString(), value);
      return writer;
    }
    public static ISgmlWriter Element(this ISgmlWriter writer, HtmlTextWriterTag name)
    {
      writer.Element(name.ToString());
      return writer;
    }
    public static ISgmlWriter Element(this ISgmlWriter writer, HtmlTextWriterTag name, object value)
    {
      writer.Element(name.ToString(), value);
      return writer;
    }

    public static string ReplaceHtmlElement(this string html, Func<Xml.IXmlNode, Sgml.ISgmlWriter, bool> replacer)
    {
      if (string.IsNullOrEmpty(html)) return html;
      html = html.Trim();
      if (!html.StartsWith("<!DOCTYPE") && !(html.StartsWith("<html", StringComparison.InvariantCultureIgnoreCase) && 
                                             html.EndsWith("</html>", StringComparison.InvariantCultureIgnoreCase))) {
        html = "<html__>" + html + "</html__>";
      }

      var reader = new Pipes.IO.StringTextSource(html).Pipe(new Pipes.Sgml.SgmlTextReader());
      var stackCount = 0;
      using (var writer = new System.IO.StringWriter())
      {
        using (var xml = Pipes.Sgml.HtmlTextWriter.Create(writer))
        {
          foreach (var item in reader)
          {
            if (item.Name.LocalName == "html__")
            {
              // Do nothing
            }
            else if (stackCount > 0)
            {
              // Make sure to skip the entire element including all children
              switch (item.Type)
              {
                case XmlNodeType.Element:
                  stackCount += 1;
                  break;
                case XmlNodeType.EndElement:
                  stackCount -= 1;
                  break;
              }
            }
            else if (replacer != null && replacer.Invoke(item, xml))
            {
              stackCount = (item.Type == XmlNodeType.Element ? 1 : 0);
            }
            else
            {
              xml.Node(item);
            }
          }
          xml.Flush();
        }
        return writer.ToString();
      }
    }
  }
}
