using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlElementReader : IPipeOutput<IXmlNode>, IPipeInput<IXmlElement>
  {
    protected IEnumerable<IXmlElement> _source;

    public XmlElementReader() { }

    public void Initialize(IEnumerable<IXmlElement> source)
    {
      _source = source;
    }

    public IEnumerator<IXmlNode> GetEnumerator()
    {
      IEnumerator<IXmlNode> currEnum;
      Stack<IEnumerator<IXmlNode>> enums = new Stack<IEnumerator<IXmlNode>>();
      Stack<IXmlNode> closeElems = new Stack<IXmlNode>();
      IXmlElement elem;

      enums.Push(_source.Cast<IXmlNode>().GetEnumerator());
      while (enums.Count > 0)
      {
        while (enums.Peek().MoveNext())
        {
          yield return enums.Peek().Current;
          elem = enums.Peek().Current as IXmlElement;
          if (elem != null && elem.Type != XmlNodeType.EmptyElement)
          {
            enums.Push(elem.Nodes().GetEnumerator());
            closeElems.Push(new XmlNode() { Name = elem.Name, Type = XmlNodeType.EndElement });
          }
        }
        currEnum = enums.Pop();
        currEnum.Dispose();
        if (closeElems.Count > 0) yield return closeElems.Pop();
      }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
