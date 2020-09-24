using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin.Documentation
{
  public class Section : IElement
  {
    public string Header { get; }
    public List<IElement> Children { get; }

    public Section(string header)
    {
      Header = header;
      Children = new List<IElement>();
    }

    public Section(string header, IEnumerable<IElement> children) : this(header)
    {
      Children = children.ToList();
    }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
