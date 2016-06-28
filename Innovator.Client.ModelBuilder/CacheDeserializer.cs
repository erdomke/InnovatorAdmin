using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Innovator.Client.ModelBuilder
{
  class CacheDeserializer : IAmlDeserializer
  {
    private IAmlDeserializer _default;

    public IResult FromXml(XmlReader xml)
    {
      return _default.FromXml(xml);
    }

    public IReadOnlyResult FromXml(XmlReader xml, string query, string database)
    {
      string currName = null;
      //var items = new List<Item>();

      while (xml.Read())
      {
        switch (xml.NodeType)
        {
          case XmlNodeType.Element:
            currName = xml.LocalName;
            switch (xml.LocalName)
            {
              case "Fault":
                return _default.FromXml(xml.ReadSubtree(), query, database);
              case "Item":
                //items.Add(ReadItem(xml));
                break;
            }
            break;
          case XmlNodeType.Text:
            if (currName == "Result")
            {
              return new TypedResult() { Value = xml.Value };
            }
            break;
        }
      }
      return null;
    }

    //private Item ReadItem(XmlReader xml)
    //{
    //  var result = new Item();
    //  while (xml.Read())
    //  {

    //  }
    //  return result;
    //}

    public void SetDefault(IAmlDeserializer defaultImpl)
    {
      _default = defaultImpl;
    }
  }
}
