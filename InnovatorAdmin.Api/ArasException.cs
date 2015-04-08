using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class ArasException : Exception
  {
    private XmlNode _error;
    private XmlNode _query;

    public XmlNode ErrorNode { get { return _error; } }
    public XmlNode QueryNode { get { return _query; } }

    public ArasException(XmlNode errorNode, XmlNode query)
      : base(errorNode.Element("faultstring", "A database error occurred.")) 
    {
      _error = errorNode;
      _query = query;
    }
    //public ArasException() : base() { }
    //public ArasException(string message) : base(message) { }
    //public ArasException(string message, Exception inner) : base(message, inner) { }
    protected ArasException(SerializationInfo info, StreamingContext context): base(info, context) { }
  }
}
