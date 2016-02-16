using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Sgml
{
  public class SgmlTextReader : Xml.XmlTextReader
  {
    private const string simulatedNode = "___ROOT___";

    public override IEnumerable<System.Xml.XmlReader> GetReaders()
    {
      foreach (var textReader in _source)
      {
        var sgmlReader = new SgmlReader();
        sgmlReader.DocType = "HTML";
        sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
        sgmlReader.CaseFolding = CaseFolding.ToLower;
        sgmlReader.InputStream = textReader;
        sgmlReader.SimulatedNode = this.ConformanceLevel == System.Xml.ConformanceLevel.Fragment ? simulatedNode : null;
        sgmlReader.StripDocType = false;
        yield return sgmlReader;
      }
    }

    public override IEnumerator<Xml.IXmlNode> GetEnumerator()
    {
      foreach (var reader in GetReaders())
      {
        while (reader.Read())
        {
          if (reader.LocalName != simulatedNode)
          {
            yield return Pipes.Xml.Util.GetNode(reader);
          }
        }
      }
    }
  }
}
