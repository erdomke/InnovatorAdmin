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
    internal ValidationException(string message
      , IReadOnlyItem item, params string[] properties)
      : base(message, properties.Any() ? 1001 : 1)
    {
      CreateDetailElement(item, properties);
    }
    internal ValidationException(string message, Exception innerException
      , IReadOnlyItem item, params string[] properties)
      : base(message, properties.Any() ? 1001 : 1, innerException)
    {
      CreateDetailElement(item, properties);
    }
    public ValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
    internal ValidationException(Element fault) : base(fault) { }

    private IElement CreateDetailElement(IReadOnlyItem item, params string[] properties)
    {
      var detail = _fault.ElementByName("detail");
      detail.Add(new AmlElement(_fault.AmlContext, "item"
        , new Attribute("type", item.Type().Value)
        , new Attribute("id", item.Id())));
      if (properties.Any())
      {
        var props = new AmlElement(_fault.AmlContext, "properties");
        foreach (var prop in properties)
        {
          props.Add(new AmlElement(_fault.AmlContext, "property", prop));
        }
        detail.Add(props);
      }
      return detail;
    }
  }
}
