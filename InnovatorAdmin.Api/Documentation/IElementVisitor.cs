using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public interface IElementVisitor<T>
  {
    T Visit(CodeBlock codeBlock);
    T Visit(DocLink docLink);
    T Visit(Hyperlink hyperlink);
    T Visit(List list);
    T Visit(Paragraph paragraph);
    T Visit(Section section);
    T Visit(TextRun run);
  }
}
