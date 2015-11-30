using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public enum XmlState
  {
    Attribute,
    AttributeStart,
    AttributeValue,
    CData,
    Comment,
    Tag,
    Other
  }
}
