using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  internal class Connection
  {
    private Func<string, XmlNode, XmlNode> _applyAction;

    public Connection(Func<string, XmlNode, XmlNode> applyAction)
    {
      _applyAction = applyAction;
    }

    public XmlNode CallAction(string soapAction, XmlNode input)
    {
      return _applyAction.Invoke(soapAction, input);
    }
    public IEnumerable<XmlElement> GetItems(string soapAction, string input, bool noItemsIsError = false)
    {
      var inputDoc = new XmlDocument();
      inputDoc.LoadXml(input);
      return this.GetItems(soapAction, inputDoc);
    }
    public IEnumerable<XmlElement> GetItems(string soapAction, XmlNode input, bool noItemsIsError = false)
    {
      var result = _applyAction.Invoke(soapAction, input);
      var err = GetError(result);
      if (err == null || (!noItemsIsError && err.Element("faultcode", "") == "0"))
      {
        var node = result;
        while (node != null && node.LocalName != "Item") node = node.ChildNodes.OfType<XmlElement>().FirstOrDefault();
        if (node == null) return Enumerable.Empty<XmlElement>();
        return node.ParentNode.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "Item");
      }
      else
      {
        throw new ArasException(err, input);
      }
    }

    public XmlNode GetError(XmlNode item)
    {
      if (item.LocalName == "Envelope" && (item.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(item.NamespaceURI)))
      {
        item = item.Element("Body");
        if (item != null && (item.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(item.NamespaceURI)))
        {
          item = item.Element("Fault");
          if (item != null && (item.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(item.NamespaceURI)))
          {
            return item;
          }
        }
      }
      return null;
    }
  }
}
