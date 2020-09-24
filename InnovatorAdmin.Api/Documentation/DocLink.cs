using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class DocLink : IElement
  {
    public string Type { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
