using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class CodeBlock : IElement
  {
    public string Language { get; set; }
    public string Code { get; set; }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
