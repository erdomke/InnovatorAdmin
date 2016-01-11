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

namespace InnovatorAdmin.Editor
{
  public class SoapEditorProxy : IEditorProxy
  {
    private IEditorHelper _helper = new XmlEditorHelper();
    private Connections.ConnectionData _connData;
    private IPromise<ServiceInfo> _service;

    public SoapEditorProxy(Connections.ConnectionData connData)
    {
      _connData = connData;
      _service = GetService();
    }

    private IPromise<ServiceInfo> GetService()
    {
      var service = new Innovator.Client.Connection.DefaultHttpService();
      return service.Execute("GET", _connData.Url, null, null, true, null)
        .Convert(r =>
        {
          ServiceDescription descrip;
          using (var reader = new StreamReader(r.AsStream))
          using (var xml = XmlReader.Create(reader))
          {
            descrip = ServiceDescription.Read(xml);
          }

          var schemaSet = new XmlSchemaSet();
          var soap = new StringReader(string.Format(Properties.Resources.SoapSchema, descrip.TargetNamespace));
          schemaSet.Add(XmlSchema.Read(soap, new ValidationEventHandler(Validation)));
          foreach (var schema in descrip.Types.Schemas.OfType<XmlSchema>())
          {
            schemaSet.Add(schema);
          }
          schemaSet.Compile();

          return new ServiceInfo()
          {
            Description = descrip,
            Schemas = schemaSet
          };
        });
    }

    public static void ParseWsdl(string url)
    {
      //http://www.border.gov.au/_vti_bin/Lists.asmx?wsdl
      var reader = XmlReader.Create(url);
      var wsdl = ServiceDescription.Read(reader);
      var schemaSet = new XmlSchemaSet();

      var soap = new StringReader(string.Format(Properties.Resources.SoapSchema, wsdl.TargetNamespace));
      schemaSet.Add(XmlSchema.Read(soap, new ValidationEventHandler(Validation)));
      foreach (var schema in wsdl.Types.Schemas.OfType<XmlSchema>())
      {
        schemaSet.Add(schema);
      }
      schemaSet.Compile();
      var thing = 2 + 2;
    }



    private static void Validation(object sender, ValidationEventArgs e)
    {
      // Do nothing
    }

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
      return Enumerable.Empty<string>();
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
      throw new NotImplementedException();
    }

    private class SoapResult : IResultObject
    {

      public OutputType PreferredMode
      {
        get { throw new NotImplementedException(); }
      }

      public int ItemCount
      {
        get { throw new NotImplementedException(); }
      }

      public ICSharpCode.AvalonEdit.Document.ITextSource GetTextSource()
      {
        throw new NotImplementedException();
      }

      public System.Data.DataSet GetDataSet()
      {
        throw new NotImplementedException();
      }

      public string Title
      {
        get { throw new NotImplementedException(); }
      }

      public string Html
      {
        get { throw new NotImplementedException(); }
      }
    }

    private class SoapCommand : ICommand
    {
      public string Query { get; set; }

      public ICommand WithQuery(string query)
      {
        this.Query = query;
        return this;
      }

      public ICommand WithAction(string action)
      {
        // Do nothing
        return this;
      }

      public ICommand WithParam(string name, object value)
      {
        // Do nothing
        return this;
      }
    }

    public Innovator.Client.IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      return _service.Convert(svc =>
        svc.Description.Services.OfType<Service>().Select(s => (IEditorTreeNode)new EditorTreeNode() {
          Name = s.Name,
          ImageKey = "folder-16",
          HasChildren = true,
          Children = s.Ports.OfType<Port>().Select(p => (IEditorTreeNode)new EditorTreeNode() {
            Name = p.Name,
            ImageKey = "folder-16",
            HasChildren = true,
            Children = svc.Description.Bindings[p.Binding.Name].Operations.OfType<OperationBinding>()
              .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
              .Select(o =>
              (IEditorTreeNode)new EditorTreeNode() {
                Name = o.Name,
                ImageKey = "xml-tag-16",
                HasChildren = false,
                Scripts = GetScripts(o, svc)
              })
          })
        }));
    }

    private IEnumerable<IEditorScript> GetScripts(OperationBinding opBinding, ServiceInfo info)
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
        var generator = new Microsoft.Xml.XMLGen.XmlSampleGenerator(info.Schemas, elem);

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
          Script = writer.ToString()
        }, 1);
      }
    }

    public void Dispose()
    {
      // do nothing
    }

    private class ServiceInfo
    {
      public ServiceDescription Description { get; set; }
      public XmlSchemaSet Schemas { get; set; }
    }
  }
}
