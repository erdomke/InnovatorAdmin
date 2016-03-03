using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  [Serializable]
  public class ServerException : Exception
  {
    private const string FaultNodeEntry = "``FaultNode";
    private const string DatabaseEntry = "``Database";
    private const string QueryEntry = "``Query";

    protected string _database;
    protected XmlElement _faultNode;
    private IElement _fault;
    private ElementFactory _factory;
    protected string _query;

    public string Database { get { return _database; } }
    public IElement Fault
    {
      get { return _fault; }
    }
    public string FaultCode
    {
      get { return _fault.Elements().Single(e => e.Name == "faultcode").Value; }
      set { ((Element)_fault).SetElement("faultcode", value); }
    }
    public string Query { get { return _query; } }

    internal ServerException(ElementFactory factory, string message)
      : this(factory, message, 1) { }
    internal ServerException(ElementFactory factory, string message, Exception innerException)
      : this(factory, message, 1, innerException) { }

    public ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      _factory = ElementFactory.Local;
      var doc = new XmlDocument(Element.BufferDocument.NameTable);
      doc.LoadXml(info.GetString(FaultNodeEntry));
      ConfigureFaultNode(doc.DocumentElement);
      _database = info.GetString(DatabaseEntry);
      _query = info.GetString(QueryEntry);
    }
    internal ServerException(ElementFactory factory, XmlElement node) : base(GetFaultString(node))
    {
      _factory = factory;
      ConfigureFaultNode(node);
    }
    protected ServerException(ElementFactory factory, string message, int code)
      : base(message)
    {
      _factory = factory;
      CreateXml(message, code);
    }
    protected ServerException(ElementFactory factory, string message, int code, Exception innerException)
      : base(message, innerException)
    {
      _factory = factory;
      CreateXml(message, code);
    }

    internal ServerException SetDetails(string database, string query)
    {
      _database = database;
      _query = query;
      return this;
    }

    private void CreateXml(string message, int code)
    {
      var doc = new XmlDocument(Element.BufferDocument.NameTable);
      doc.LoadXml("<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body><SOAP-ENV:Fault xmlns:af=\"http://www.aras.com/InnovatorFault\"><faultcode>"
        + code.ToString()
        + "</faultcode><faultstring>"
        + message
        + "</faultstring></SOAP-ENV:Fault></SOAP-ENV:Body></SOAP-ENV:Envelope>");
      ConfigureFaultNode(doc.DocumentElement);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      if (_fault != null)
      {
        if (_fault.Parent.Parent.Exists)
        {
          info.AddValue(FaultNodeEntry, _fault.Parent.Parent.ToString());
        }
        else
        {
          info.AddValue(FaultNodeEntry, _fault.ToString());
        }
      }
      info.AddValue(DatabaseEntry, this.Database);
      info.AddValue(QueryEntry, this.Query);
    }

    private void ConfigureFaultNode(XmlElement node)
    {
      _faultNode = GetFaultNode(node);
      _fault = _factory.ElementFromXml(_faultNode);
    }

    internal static XmlElement GetFaultNode(XmlElement node)
    {
      if (node.LocalName == "Envelope" && (node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(node.NamespaceURI)) && node.HasChildNodes)
        node = node.ChildNodes.OfType<XmlElement>().First();
      if (node.LocalName == "Body" && (node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(node.NamespaceURI)) && node.HasChildNodes)
        node = node.ChildNodes.OfType<XmlElement>().First();
      if (node.LocalName == "Fault" && (node.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || string.IsNullOrEmpty(node.NamespaceURI)) && node.HasChildNodes)
      {
        return node;
      }
      throw new InvalidOperationException();
    }
    private static string GetFaultString(XmlNode node)
    {
      return node.ChildNodes.OfType<XmlElement>().Single(n => n.LocalName == "faultstring").InnerText;
    }

    public string AsAmlString()
    {
      return _faultNode.Parents().Last().OuterXml;
    }

    public void AsAmlString(XmlWriter writer)
    {
      _faultNode.Parents().Last().WriteTo(writer);
    }

    public IReadOnlyResult AsResult()
    {
      return new Result(_factory, _faultNode);
    }

    public override string ToString()
    {
      var builder = new StringBuilder(base.ToString()).AppendLine().AppendLine();
      var settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.OmitXmlDeclaration = true;
      using (var xml = XmlWriter.Create(builder, settings))
      {
        xml.WriteStartElement("Fault");
        // Render the non-redundant information
        foreach (var elem in _faultNode.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName != "faultstring"))
        {
          elem.WriteTo(xml);
        }
        xml.WriteEndElement();
      }
      return builder.ToString();
    }
  }
}
