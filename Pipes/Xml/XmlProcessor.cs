using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlProcessor : IProcessor<IEnumerable<IXmlNode>>, IWriter<Sgml.ISgmlWriter>
  {
    private IEnumerable<IXmlNode> _data;
    private Sgml.ISgmlWriter _writer;

    public object Parent { get; set; }

    public void InitializeData(IEnumerable<IXmlNode> data)
    {
      _data = data;
    }

    public void Execute()
    {
      foreach (var node in _data)
      {
        _writer.Node(node);
      }
    }

    public T Initialize<T>(T coreWriter) where T : Sgml.ISgmlWriter
    {
      _writer = coreWriter;
      return coreWriter;
    }
  }
}
