using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class Hyperlink : IElement
  {
    public string Href { get; }
    public List<IElement> Children { get; }

    public Hyperlink(string href, IEnumerable<IElement> children)
    {
      Href = href;
      Children = children.ToList();
    }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
