using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public class XmlDataWriter : IDataWriter
  {
    private bool _first;
    private StringBuilder _builder;
    private XmlWriter _xml;

    public XmlDataWriter()
    {
      _builder = new StringBuilder();
      Reset();
    }

    public void Reset()
    {
      _builder.Length = 0;
      _first = true;

      var settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.OmitXmlDeclaration = true;

      _xml = XmlTextWriter.Create(_builder, settings);
    }

    public void Row()
    {
      if (_first)
      {
        _xml.WriteStartElement("table");
        _first = false;
      }
      _xml.WriteStartElement("row");
    }

    public void RowEnd()
    {
      _xml.WriteEndElement();
    }

    public void Cell(string columnName, object value)
    {
      _xml.WriteElementString(columnName.ToLowerInvariant().Replace(' ', '_'), (value ?? string.Empty).ToString());
    }

    public override string ToString()
    {
      _xml.Flush();
      return _builder.ToString() + Environment.NewLine + "</table>";
    }
  }
}
