using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public interface IAmlDeserializer
  {
    IResult FromXml(XmlReader xml);
    IReadOnlyResult FromXml(XmlReader xml, string query, string database);
    void SetDefault(IAmlDeserializer defaultImpl);
  }
}
