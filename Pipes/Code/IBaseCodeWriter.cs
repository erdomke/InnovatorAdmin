using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Code
{
  public interface IBaseCodeWriter
  {
    void Flush();
    IBaseCodeWriter Comment(string value);
    IBaseCodeWriter CommentLine(string value);
    IBaseCodeWriter DateValue(object value);
    IBaseCodeWriter Line();
    IBaseCodeWriter Line(string value);
    IBaseCodeWriter Null();
    IBaseCodeWriter NumberValue(object value);
    IBaseCodeWriter Raw(string value);
    IBaseCodeWriter StringValue(object value);
    IBaseCodeWriter Identifier(object value);
  }
}
