using Innovator.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Plugin
{
  public class PluginResult : IPluginResult
  {
    private readonly Func<XmlReader> _createReader;
    private readonly Action<TextWriter> _writer;

    public int Count { get; }

    public PluginResult(Action<TextWriter> writer, int count)
    {
      _writer = writer;
      Count = count;
    }

    public PluginResult(IReadOnlyResult result)
    {
      Count = result.Items().Count();
      _createReader = result.CreateReader;
    }

    public XmlReader CreateReader()
    {
      return _createReader?.Invoke();
    }

    public void Write(TextWriter writer)
    {
      if (_writer != null)
      {
        _writer.Invoke(writer);
      }
      else if (_createReader != null)
      {
        using (var reader = _createReader.Invoke())
        using (var xml = XmlWriter.Create(writer, new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true,
          IndentChars = "  ",
          CheckCharacters = true,
          ConformanceLevel = ConformanceLevel.Fragment
        }))
        {
          xml.WriteNode(reader, false);
        }
      }
    }
  }
}
