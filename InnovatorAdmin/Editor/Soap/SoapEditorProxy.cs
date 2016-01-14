using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;
using Innovator.Client;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.Document;

namespace InnovatorAdmin.Editor
{
  public class SoapEditorProxy : IEditorProxy
  {
    private IEditorHelper _helper;
    private Connections.ConnectionData _connData;
    private ServiceDescription _descrip;
    private XmlSchemaSet _schemas;
    private string _baseUrl;
    private System.Net.ICredentials _cred;

    private Dictionary<string, string> _actionUrls;

    public SoapEditorProxy(Connections.ConnectionData connData, ServiceDescription description, XmlSchemaSet schemas)
    {
      _connData = connData;
      _descrip = description;
      _schemas = schemas;
      _helper = new XmlCompletionDataProvider(schemas, "http://schemas.xmlsoap.org/soap/envelope/"
        , "soapenv", () => "xmlns:soap=\"" + description.TargetNamespace + "\""
        , e => (e.Name == "Envelope" && e.QualifiedName.Namespace == "http://schemas.xmlsoap.org/soap/envelope/")
          || TypeMatchesAction(e.QualifiedName, this.Action));

      _baseUrl = new Uri(_connData.Url).GetLeftPart(UriPartial.Path);
      switch (_connData.Authentication)
      {
        case Connections.Authentication.Windows:
          _cred = CredentialCache.DefaultNetworkCredentials;
          break;
        case Connections.Authentication.Explicit:
          _cred = new NetworkCredential(_connData.UserName, _connData.Password);
          break;
      }

      _actionUrls = _descrip.Services.OfType<Service>()
        .SelectMany(s => s.Ports.OfType<Port>().Where(SoapPort))
        .SelectMany(p => _descrip.Bindings[p.Binding.Name].Operations.OfType<OperationBinding>())
        .Select(o => new { Name = o.Name, Address = o.Extensions.OfType<SoapOperationBinding>().First().SoapAction })
        .Distinct()
        .ToDictionary(a => a.Name, a => a.Address);
    }

    //public static void ParseWsdl(string url)
    //{
    //  //http://www.border.gov.au/_vti_bin/Lists.asmx?wsdl
    //  var reader = XmlReader.Create(url);
    //  var wsdl = ServiceDescription.Read(reader);
    //  var schemaSet = new XmlSchemaSet();

    //  var soap = new StringReader(string.Format(Properties.Resources.SoapSchema, wsdl.TargetNamespace));
    //  schemaSet.Add(XmlSchema.Read(soap, new ValidationEventHandler(Validation)));
    //  foreach (var schema in wsdl.Types.Schemas.OfType<XmlSchema>())
    //  {
    //    schemaSet.Add(schema);
    //  }
    //  schemaSet.Compile();
    //  var thing = 2 + 2;
    //}


    public string Action { get; set; }

    public Connections.ConnectionData ConnData
    {
      get { return _connData; }
    }

    public string Name
    {
      get { return _connData.ConnectionName; }
    }

    public IEditorProxy Clone()
    {
      return this;
    }

    public IEnumerable<string> GetActions()
    {
      return _actionUrls.Keys;
    }

    public IEditorHelper GetHelper()
    {
      return _helper;
    }

    public IEditorHelper GetOutputHelper()
    {
      return _helper;
    }

    public ICommand NewCommand()
    {
      return new SoapCommand();
    }

    public Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async)
    {
      var soapCmd = request as SoapCommand;
      if (soapCmd == null)
        throw new InvalidOperationException("The request must be of type 'SoapCommand'");

      var http = new Innovator.Client.Connection.DefaultHttpService();
      return http.Execute("POST", _baseUrl, null, _cred, true, r =>
      {
        r.SetHeader("SOAPAction", "\"" + _actionUrls[soapCmd.Action] + "\"");
        r.SetContent(w => w.Write(soapCmd.Query), "text/xml;charset=UTF-8");
      }).Convert(r => (IResultObject)new SoapResult(r.AsStream));
    }

    private class SoapResult : IResultObject
    {
      private System.Data.DataSet _dataSet = new System.Data.DataSet();
      private ITextSource _text;

      public SoapResult(Stream data)
      {
        var rope = new Rope<char>();
        using (var reader = new StreamReader(data))
        using (var writer = new Editor.RopeWriter(rope))
        {
          /*var buffer = new char[4096];
          var count = reader.Read(buffer, 0, buffer.Length);
          while (count > 0)
          {
            writer.Write(buffer, 0, count);
            count = reader.Read(buffer, 0, buffer.Length);
          }*/
          IndentXml(reader, writer);
        }
        _text = new RopeTextSource(rope);
      }

      private void IndentXml(TextReader xmlContent, TextWriter writer)
      {
        char[] writeNodeBuffer = null;

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CheckCharacters = true;

        using (var reader = XmlReader.Create(xmlContent))
        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
          bool canReadValueChunk = reader.CanReadValueChunk;
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                xmlWriter.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                xmlWriter.WriteAttributes(reader, false);
                if (reader.IsEmptyElement)
                {
                  xmlWriter.WriteEndElement();
                }
                break;
              case XmlNodeType.Text:
                if (canReadValueChunk)
                {
                  if (writeNodeBuffer == null)
                  {
                    writeNodeBuffer = new char[1024];
                  }
                  int count;
                  while ((count = reader.ReadValueChunk(writeNodeBuffer, 0, 1024)) > 0)
                  {
                    xmlWriter.WriteChars(writeNodeBuffer, 0, count);
                  }
                }
                break;
              case XmlNodeType.CDATA:
                xmlWriter.WriteCData(reader.Value);
                break;
              case XmlNodeType.EntityReference:
                xmlWriter.WriteEntityRef(reader.Name);
                break;
              case XmlNodeType.ProcessingInstruction:
              case XmlNodeType.XmlDeclaration:
                xmlWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                break;
              case XmlNodeType.Comment:
                xmlWriter.WriteComment(reader.Value);
                break;
              case XmlNodeType.DocumentType:
                xmlWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                break;
              case XmlNodeType.Whitespace:
              case XmlNodeType.SignificantWhitespace:
                xmlWriter.WriteWhitespace(reader.Value);
                break;
              case XmlNodeType.EndElement:
                xmlWriter.WriteFullEndElement();
                break;
            }
          }

          xmlWriter.Flush();
          writer.Flush();
        }
      }

      public OutputType PreferredMode
      {
        get { return OutputType.Text; }
      }

      public int ItemCount
      {
        get { return 0; }
      }

      public ICSharpCode.AvalonEdit.Document.ITextSource GetTextSource()
      {
        return _text;
      }

      public System.Data.DataSet GetDataSet()
      {
        return _dataSet;
      }

      public string Title
      {
        get { return "Response"; }
      }

      public string Html
      {
        get { return string.Empty; }
      }
    }

    private class SoapCommand : ICommand
    {
      public string Action { get; set; }
      public string Query { get; set; }

      public ICommand WithQuery(string query)
      {
        this.Query = query;
        return this;
      }

      public ICommand WithAction(string action)
      {
        this.Action = action;
        return this;
      }

      public ICommand WithParam(string name, object value)
      {
        // Do nothing
        return this;
      }
    }

    private static bool SoapPort(Port p)
    {
      return p.Extensions.OfType<SoapAddressBinding>().Any()
        || p.Extensions.OfType<Soap12AddressBinding>().Any();
    }

    public Innovator.Client.IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      return Promises.Resolved(Enumerable.Repeat(ServiceTreeNode(_descrip, _schemas), 1));
    }

    public static IEditorTreeNode ServiceTreeNode(ServiceDescription descrip, XmlSchemaSet schemas)
    {
      var p = descrip.Services.OfType<Service>()
        .SelectMany(s => s.Ports.OfType<Port>())
        .FirstOrDefault(SoapPort);
      return (IEditorTreeNode)new EditorTreeNode()
      {
        Name = p.Name,
        ImageKey = "folder-16",
        HasChildren = true,
        Children = descrip.Bindings[p.Binding.Name].Operations.OfType<OperationBinding>()
          .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
          .Select(o =>
          (IEditorTreeNode)new EditorTreeNode()
          {
            Name = o.Name,
            ImageKey = "xml-tag-16",
            HasChildren = false,
            Scripts = GetScripts(o, schemas)
          })
      };
    }

    private bool TypeMatchesAction(XmlQualifiedName elem, string action)
    {
      var opBinding = _descrip.Services.OfType<Service>()
        .SelectMany(s => s.Ports.OfType<Port>())
        .Where(SoapPort)
        .SelectMany(p => _descrip.Bindings[p.Binding.Name].Operations.OfType<OperationBinding>())
        .FirstOrDefault(o => o.Name == action);
      if (opBinding == null)
        return false;
      var operation = _descrip.PortTypes[opBinding.Binding.Type.Name].Operations.OfType<Operation>()
        .FirstOrDefault(o => o.Name == opBinding.Name);
      var input = _descrip.Messages[operation.Messages.Input.Message.Name].Parts[0].Element;
      return input.Name == elem.Name && input.Namespace == elem.Namespace;
    }

    private static IEnumerable<IEditorScript> GetScripts(OperationBinding opBinding, XmlSchemaSet schemas)
    {
      var binding = opBinding.Binding;
      var service = binding.ServiceDescription;
      var operation = service.PortTypes[binding.Type.Name].Operations.OfType<Operation>()
        .FirstOrDefault(o => o.Name == opBinding.Name);
      var elem = service.Messages[operation.Messages.Input.Message.Name].Parts[0].Element;
      //var schemaElem = service.Types.Schemas
      //  .SelectMany(s => s.Elements.Values.OfType<XmlSchemaElement>())
      //  .FirstOrDefault(e => e.Name == elem.Name);

      var settings = new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = true,
        IndentChars = "  "
      };

      using (var writer = new StringWriter())
      using (var xml = XmlTextWriter.Create(writer, settings))
      {
        var generator = new Microsoft.Xml.XMLGen.XmlSampleGenerator(schemas, elem);

        xml.WriteStartElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        xml.WriteAttributeString("xmlns", "soap", null, elem.Namespace);
        xml.WriteStartElement("soapenv", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
        xml.WriteEndElement();
        xml.WriteStartElement("soapenv", "Body", "http://schemas.xmlsoap.org/soap/envelope/");

        generator.WriteXml(xml);

        xml.WriteEndElement();
        xml.WriteEndElement();

        xml.Flush();
        writer.Flush();
        return Enumerable.Repeat<IEditorScript>(new EditorScript()
        {
          Name = "Sample Request",
          Script = writer.ToString(),
          Action = opBinding.Name
        }, 1);
      }
    }

    public void Dispose()
    {
      // do nothing
    }
  }
}
