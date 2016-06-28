using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  [Serializable]
  public class ValidationReportException : ServerException
  {
    internal ValidationReportException(string message
      , IReadOnlyItem item, string report)
      : base(message, 1001)
    {
      CreateDetailElement(item, report);
    }
    internal ValidationReportException(string message, Exception innerException
      , IReadOnlyItem item, string report)
      : base(message, 1001, innerException)
    {
      CreateDetailElement(item, report);
    }
    public ValidationReportException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
    internal ValidationReportException(Element fault) : base(fault) { }

    private IElement CreateDetailElement(IReadOnlyItem item, string report)
    {
      var detail = _fault.ElementByName("detail");
      if (item != null)
      {
        detail.Add(new AmlElement(_fault.AmlContext, "item"
        , new Attribute("type", item.Type().Value)
        , new Attribute("id", item.Id())));
      }
      detail.Add(new AmlElement(_fault.AmlContext, "error_resolution_report", report));
      return detail;
    }
  }
}
