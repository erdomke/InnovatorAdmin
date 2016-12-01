using System;
using System.Collections.Generic;
using System.Linq;
#if SERIALIZATION
using System.Runtime.Serialization;
#endif
using System.Text;
using System.Xml;

namespace Innovator.Client
{
#if SERIALIZATION
  [Serializable]
#endif
  public class NoItemsFoundException : ServerException
  {
    internal NoItemsFoundException(string type, string query)
      : base("No items of type " + type + " found.", 0)
    {
      var detail = CreateDetailElement();
      detail.Add(new AmlElement(_fault.AmlContext, "af:legacy_faultstring", "No items of type " + type + " found using the criteria: " + query));
      this._query = query;
    }
    internal NoItemsFoundException(string message)
      : base(message, 0)
    {
      CreateDetailElement();
    }
    internal NoItemsFoundException(string message, Exception innerException)
      : base(message, 0, innerException)
    {
      CreateDetailElement();
    }
    internal NoItemsFoundException(Element fault) : base(fault) { }
#if SERIALIZATION
    public NoItemsFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
#endif

    private IElement CreateDetailElement()
    {
      var detail = _fault.ElementByName("detail") as Element;
      if (!detail.Exists || string.IsNullOrEmpty(detail.ElementByName("af:legacy_detail").Value))
        detail.Add(new AmlElement(_fault.AmlContext, "af:legacy_detail", this.Message));
      return detail;
    }
  }
}
