using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IXmlFieldValue : Data.IFieldValue
  {
    IXmlName XmlName { get; }
  }
}
