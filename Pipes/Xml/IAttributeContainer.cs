using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IAttributeContainer
  {
    IEnumerable<Data.IFieldValue> Attributes { get; }
    object Attribute(string name);
    Data.FieldStatus AttributeStatus(string name);
  }
}
