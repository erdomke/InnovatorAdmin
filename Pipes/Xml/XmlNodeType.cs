using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public enum XmlNodeType
  {
    Element,
    Attribute,
    Text,
    CDATA,
    Comment,
    Whitespace,
    Entity,
    DocumentType,
    Notation,
    SignificantWhiteSpace,
    EndElement,
    XmlDeclaration,
    EmptyElement,
    Other
  }
}
