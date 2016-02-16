using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IXmlName
  {
    string LocalName { get; }
    string Namespace { get; }
    string Prefix { get; }
  }
}
