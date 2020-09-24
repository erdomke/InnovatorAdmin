using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Documentation
{
  public class List : IElement
  {
    public ListType Type { get; set; }
    public ListItem Header { get; set; }
    public List<ListItem> Children { get; } = new List<ListItem>();

    internal static List Parse(XElement element)
    {
      var result = new List();
      if (Enum.TryParse<ListType>((string)element.Attribute("type") ?? "bullet", true, out var listType))
        result.Type = listType;
      var header = element.Element("listheader");
      if (header != null)
        result.Header = ListItem.Parse(header);

      result.Children.AddRange(element.Elements("item").Select(ListItem.Parse));
      return result;
    }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
