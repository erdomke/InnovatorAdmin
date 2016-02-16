using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IXmlAttribute : IXmlFieldValue
  {
    new string Value { get; set; }
  }
}
