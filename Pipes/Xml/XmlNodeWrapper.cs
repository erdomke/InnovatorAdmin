using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public class XmlNodeWrapper : IXmlNode
  {
    private System.Xml.XmlNode _node;

    public XmlNodeWrapper(System.Xml.XmlNode node)
    {
      _node = node;
    }

    public IXmlName Name
    {
      get { return new XmlNodeNameWrapper(_node); }
    }

    public XmlNodeType Type
    {
      get { return Util.GetType(_node.NodeType, (_node is XmlElement && ((XmlElement)_node).IsEmpty)); }
    }

    public object Value
    {
      get 
      {
        var textNode = _node as XmlText;
        if (textNode == null)
        {
          return null;
        }
        else
        {
          return textNode.Value;
        }
      }
    }

    public int FieldCount
    {
      get { return 0; }
    }

    public object Item(string name)
    {
      return null;
    }

    public Data.FieldStatus Status(string name)
    {
      return Data.FieldStatus.Undefined;
    }

    public IEnumerator<Data.IFieldValue> GetEnumerator()
    {
      return null;
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return null;
    }
    IEnumerator<IXmlFieldValue> IEnumerable<IXmlFieldValue>.GetEnumerator()
    {
      return null;
    }
  }
}
