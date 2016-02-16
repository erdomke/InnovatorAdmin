using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Data;
using System.IO;

namespace Pipes.Xml
{
  public class XmlTextReader : IXmlReader, IPipeInput<System.IO.TextReader>
  {
    protected IEnumerable<System.IO.TextReader> _source;

    public System.Xml.ConformanceLevel ConformanceLevel { get; set; }
    public System.Xml.WhitespaceHandling WhitespaceHandling { get; set; }

    public XmlTextReader() 
    {
      this.ConformanceLevel = System.Xml.ConformanceLevel.Document;
      this.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;
    }

    public void Initialize(IEnumerable<System.IO.TextReader> source)
    {
      _source = source;
    }

    public virtual IEnumerable<System.Xml.XmlReader> GetReaders()
    {
      foreach (var textReader in _source)
      {
        // Can't do this the "correct way" with the .Create method as that method does not handle the 
        // carriage return in the XML as expected (it is converted to a line feed).
        var reader = new System.Xml.XmlTextReader(textReader) { WhitespaceHandling = this.WhitespaceHandling };
        yield return reader;
      }
    }
    public virtual IEnumerator<IXmlNode> GetEnumerator()
    {
      foreach (var reader in GetReaders())
      {
        while (reader.Read())
        {
          yield return Util.GetNode(reader);
        }
      }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
