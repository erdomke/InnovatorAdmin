using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml
{
  public interface ISgmlWriter
  {
    ISgmlWriter Attribute(string name, object value);
    ISgmlWriter Attribute(string name, string ns, object value);
    ISgmlWriter Attribute(string prefix, string name, string ns, object value);
    ISgmlWriter Comment(string value);
    ISgmlWriter Element(string name, object value);
    ISgmlWriter Element(string name);
    ISgmlWriter ElementEnd();
    void Flush();
    ISgmlWriter NsElement(string name, string ns);
    ISgmlWriter NsElement(string prefix, string name, string ns);
    ISgmlWriter Raw(string value);
    ISgmlWriter Value(object value);
  }
}
