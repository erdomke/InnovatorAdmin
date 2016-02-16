using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Json
{
  public interface IJsonWriter
  {
    IJsonWriter Array();
    IJsonWriter ArrayEnd();
    void Flush();
    IJsonWriter ListSeparator();
    IJsonWriter NullProp(string name);
    IJsonWriter Object();
    IJsonWriter ObjectEnd();
    IJsonWriter Prop(string name);
    IJsonWriter Prop(string name, object value);
    IJsonWriter Raw(string value);
    IJsonWriter Value(object obj);
  }
}
