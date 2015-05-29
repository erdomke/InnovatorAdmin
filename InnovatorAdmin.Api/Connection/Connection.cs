using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class Connection
  {
    private IArasConnection _extConn;
    private XmlElement _currUser;

    public IArasConnection ExternalConnection { get { return _extConn; } }

    public Connection(IArasConnection extConn)
    {
      _extConn = extConn;
    }

    public XmlNode CallAction(string soapAction, string input)
    {
      return XmlUtils.DocFromXml(_extConn.CallAction(soapAction, input)).DocumentElement;
    }
    public XmlNode CallAction(string soapAction, XmlNode input, Action<int, int> progressReporter = null)
    {
      return XmlUtils.DocFromXml(_extConn.CallAction(soapAction, input.OuterXml, new ProgressCallback(progressReporter))).DocumentElement;
    }
    public XmlElement GetCurrUserInfo()
    {
      if (_currUser == null)
      {
        _currUser = this.GetItems("ApplyItem", "<Item type=\"User\" action=\"get\" id=\"" + _extConn.GetUserId() + "\" />").FirstOrDefault();
      }
      return _currUser;
    }
    public IEnumerable<XmlElement> GetItems(string soapAction, string input, bool noItemsIsError = false)
    {
      var inputDoc = new XmlDocument();
      inputDoc.LoadXml(input);
      return this.GetItems(soapAction, inputDoc);
    }
    public IEnumerable<XmlElement> GetItems(string soapAction, XmlNode input, bool noItemsIsError = false)
    {
      var result = XmlUtils.DocFromXml(_extConn.CallAction(soapAction, input.OuterXml)).DocumentElement;
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
      if (item != null && item.LocalName == "Envelope" && (item.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(item.NamespaceURI)))
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

    private class ProgressCallback : MarshalByRefObject, IProgressCallback
    {
      private Action<int, int> _callback;

      public ProgressCallback(Action<int, int> callback)
      {
        _callback = callback;
      }

      public void ReportProgress(int currentStep, int totalSteps)
      {
        _callback.Invoke(currentStep, totalSteps);
      }
    }
  }
}
