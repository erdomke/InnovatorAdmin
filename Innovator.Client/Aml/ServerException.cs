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
  public class ServerException : Exception, IAmlNode
  {
    private const string FaultNodeEntry = "``FaultNode";
    private const string DatabaseEntry = "``Database";
    private const string QueryEntry = "``Query";

    protected string _database;
    protected Element _fault;
    protected string _query;

    public string Database { get { return _database; } }
    public IElement Fault
    {
      get { return _fault; }
    }
    public string FaultCode
    {
      get { return _fault.ElementByName("faultcode").Value; }
      set { _fault.ElementByName("faultcode").Add(value); }
    }
    public string Query { get { return _query; } }

    internal ServerException(string message)
      : this(message, 1) { }
    internal ServerException(string message, Exception innerException)
      : this(message, 1, innerException) { }

#if SERIALIZATION
    public ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      var result = ElementFactory.Local.FromXml(info.GetString(FaultNodeEntry));
      _fault = result.Exception._fault;
      _database = info.GetString(DatabaseEntry);
      _query = info.GetString(QueryEntry);
    }
#endif
    public ServerException(Element fault) : base(fault.ElementByName("faultstring").Value ?? "An unexpected error occurred")
    {
      _fault = fault;
    }

    protected ServerException(string message, int code)
      : base(message)
    {
      CreateXml(message, code);
    }
    protected ServerException(string message, int code, Exception innerException)
      : base(message, innerException)
    {
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
      var aml = ElementFactory.Local;
      _fault = aml.Element("SOAP-ENV:Fault", aml.Element("faultcode", code), aml.Element("faultstring", message)) as Element;
    }

#if SERIALIZATION
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      if (_fault != null)
      {
        if (_fault.Parent.Parent.Exists)
        {
          info.AddValue(FaultNodeEntry, _fault.ToAml());
        }
        else
        {
          info.AddValue(FaultNodeEntry, _fault.ToAml());
        }
      }
      info.AddValue(DatabaseEntry, this.Database);
      info.AddValue(QueryEntry, this.Query);
    }
#endif

    public string ToAml()
    {
      using (var writer = new System.IO.StringWriter())
      using (var xml = XmlWriter.Create(writer, new XmlWriterSettings() { OmitXmlDeclaration = true }))
      {
        ToAml(xml, new AmlWriterSettings());
        xml.Flush();
        return writer.ToString();
      }
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      writer.WriteStartElement("SOAP-ENV", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
      writer.WriteStartElement("SOAP-ENV", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
      writer.WriteStartElement("SOAP-ENV", "Fault", "http://schemas.xmlsoap.org/soap/envelope/");
      writer.WriteAttributeString("xmlns", "af", null, "http://www.aras.com/InnovatorFault");
      foreach (var elem in _fault.Elements())
      {
        elem.ToAml(writer, settings);
      }
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    public IReadOnlyResult AsResult()
    {
      return new Result(ElementFactory.Local) { Exception = this };
    }

    public override string ToString()
    {
      return _fault.ToAml();
    }
  }
}
