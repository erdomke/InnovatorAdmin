using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public interface IAmlNode
  {
    string ToAml();
    void ToAml(XmlWriter writer);
  }
}
