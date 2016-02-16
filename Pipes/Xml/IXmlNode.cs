using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public interface IXmlNode : Data.IDataRecord, IEnumerable<IXmlFieldValue>
  {
    IXmlName Name { get; }
    XmlNodeType Type { get; }
    object Value { get; }
  }
}
