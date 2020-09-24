using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin.Documentation
{
  public class Paragraph : IElement
  {
    public List<IElement> Children { get; }

    public Paragraph()
    {
      Children = new List<IElement>();
    }
    public Paragraph(params IElement[] elements)
    {
      Children = elements.ToList();
    }

    public Paragraph(IEnumerable<IElement> elements)
    {
      Children = elements.ToList();
    }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
