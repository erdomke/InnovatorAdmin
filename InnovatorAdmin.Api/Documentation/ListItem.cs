using System.Collections.Generic;
using System.Xml.Linq;

namespace InnovatorAdmin.Documentation
{
  public class ListItem
  {
    public List<IElement> Term { get; } = new List<IElement>();
    public List<IElement> Description { get; } = new List<IElement>();

    public ListItem() { }

    public ListItem(string term, string description)
    {
      Term.Add(new TextRun(term));
      Description.Add(new TextRun(description));
    }

    public ListItem(string description)
    {
      Description.Add(new TextRun(description));
    }

    internal static ListItem Parse(XElement element)
    {
      var result = new ListItem();
      var term = element.Element("term");
      if (term != null)
        result.Term.AddRange(OperationElement.ParseDoc(term.Nodes()));

      var description = element.Element("description");
      if (description != null)
        result.Description.AddRange(OperationElement.ParseDoc(description.Nodes()));
      return result;
    }
  }
}
