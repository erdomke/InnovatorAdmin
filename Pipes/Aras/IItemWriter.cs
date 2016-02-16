using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public interface IItemWriter
  {
    void Flush();
    IItemWriter Attribute(string name, object value);
    IItemWriter Element(string name, object value);
    IItemWriter Element(string name);
    IItemWriter ElementEnd();
    IItemWriter Result();
    IItemWriter Result(string result);

    IItemWriter BooleanProperty(string name, object value);
    IItemWriter DateProperty(string name, object value);
    IItemWriter NumberProperty(string name, object value);
    IItemWriter StringProperty(string name, object value);
  }
}
