using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  [Serializable]
  public class ValidationException : ServerException
  {
    internal ValidationException(ElementFactory factory, string message
      , IReadOnlyItem item, params string[] properties)
      : base(factory, message, properties.Any() ? 1001 : 1)
    {
      CreateDetailElement(item, properties);
    }
    internal ValidationException(ElementFactory factory, string message, Exception innerException
      , IReadOnlyItem item, params string[] properties)
      : base(factory, message, properties.Any() ? 1001 : 1, innerException)
    {
      CreateDetailElement(item, properties);
    }
    public ValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
    internal ValidationException(ElementFactory factory, XmlElement node) : base(factory, node) { }

    private XmlElement CreateDetailElement(IReadOnlyItem item, params string[] properties)
    {
      if (item != null)
      {
        var detail = _faultNode.Elem("detail");
        detail.Elem("item").Attr("type", item.Type().Value).Attr("id", item.Id());
        if (properties.Any())
        {
          var props = detail.Elem("properties");
          foreach (var prop in properties)
          {
            props.Elem("property", prop);
          }
        }
        return detail;
      }
      return null;
    }
  }
}
