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
    internal ValidationReportException(ElementFactory factory, string message
      , IReadOnlyItem item, string report)
      : base(factory, message, 1001)
    {
      CreateDetailElement(item, report);
    }
    internal ValidationReportException(ElementFactory factory, string message, Exception innerException
      , IReadOnlyItem item, string report)
      : base(factory, message, 1001, innerException)
    {
      CreateDetailElement(item, report);
    }
    public ValidationReportException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
    internal ValidationReportException(ElementFactory factory, XmlElement node) : base(factory, node) { }

    private XmlElement CreateDetailElement(IReadOnlyItem item, string report)
    {
      var detail = _faultNode.Elem("detail");
      if (item != null)
      {
        detail.Elem("item").Attr("type", item.Type().Value).Attr("id", item.Id());
      }
      detail.Elem("error_resolution_report", report);
      return detail;
    }
  }
}
