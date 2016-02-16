using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IXmlReader : IPipeOutput<IXmlNode>
  {
    IEnumerable<System.Xml.XmlReader> GetReaders();
  }
}
