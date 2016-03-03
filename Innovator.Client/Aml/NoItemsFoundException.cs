using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  [Serializable]
  public class NoItemsFoundException : ServerException
  {
    internal NoItemsFoundException(ElementFactory factory, string type, string query)
      : base(factory, "No items of type " + type + " found.", 0)
    {
      var detail = CreateDetailElement();
      detail.AppendChild(_faultNode.OwnerDocument.CreateElement("af", "legacy_faultstring", "http://www.aras.com/InnovatorFault")).InnerText =
        "No items of type " + type + " found using the criteria: " + query;
      this._query = query;
    }
    internal NoItemsFoundException(ElementFactory factory, string message)
      : base(factory, message, 0)
    {
      CreateDetailElement();
    }
    internal NoItemsFoundException(ElementFactory factory, string message, Exception innerException)
      : base(factory, message, 0, innerException)
    {
      CreateDetailElement();
    }
    public NoItemsFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
    internal NoItemsFoundException(ElementFactory factory, XmlElement node) : base(factory, node) { }

    private XmlElement CreateDetailElement()
    {
      var detail = _faultNode.Elem("detail");
      detail.AppendChild(_faultNode.OwnerDocument.CreateElement("af", "legacy_detail", "http://www.aras.com/InnovatorFault")).InnerText = this.Message;
      return detail;
    }
  }
}
