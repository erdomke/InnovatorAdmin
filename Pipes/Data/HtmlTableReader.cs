using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class HtmlTableReader : IPipeInput<Xml.IXmlNode>, IPipeOutput<IDataRecord>
  {
    private IEnumerable<Xml.IXmlNode> _source;

    public IEnumerable<IDataRecord> ProcessCells(Func<int, Xml.IXmlNode, string> processor)
    {
      var values = new List<object>();
      var cellContent = new StringBuilder();

      foreach (var node in _source.SkipWhile(n => n.Name.LocalName != "tr"))
      {
        if (node.Name.LocalName == "tr" && node.Type == Xml.XmlNodeType.Element)
        {
          values.Clear();
        }
        else if (node.Name.LocalName == "tr" && node.Type == Xml.XmlNodeType.EndElement)
        {
          yield return new DataRecord(values.ToArray());
        }
        else if (node.Name.LocalName == "td" && node.Type == Xml.XmlNodeType.Element)
        {
          cellContent.Length = 0;
        }
        else if (node.Name.LocalName == "td" && node.Type == Xml.XmlNodeType.EndElement)
        {
          values.Add(cellContent.ToString());
        }
        else if (node.Name.LocalName == "table" && node.Type == Xml.XmlNodeType.EndElement)
        {
          yield break;
        }
        else if (processor == null)
        {
          cellContent.Append(node.Value);
        }
        else
        {
          cellContent.Append(processor(values.Count, node));
        }
      }
    }

    public IEnumerator<IDataRecord> GetEnumerator()
    {
      return ProcessCells(null).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Initialize(IEnumerable<Xml.IXmlNode> source)
    {
      _source = source;
    }
  }
}
