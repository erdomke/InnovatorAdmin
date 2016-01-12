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
    private IEditorHelper _helper;
    private Connections.ConnectionData _connData;
    private ServiceDescription _descrip;
    private XmlSchemaSet _schemas;


    public SoapEditorProxy(Connections.ConnectionData connData, ServiceDescription description, XmlSchemaSet schemas)
    {
      _connData = connData;
      _descrip = description;
      _schemas = schemas;
      _helper = new XmlCompletionDataProvider(schemas, "http://schemas.xmlsoap.org/soap/envelope/", "soapenv");
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
      return Promises.Resolved(
        _descrip.Services.OfType<Service>().Select(s => (IEditorTreeNode)new EditorTreeNode() {
          Name = s.Name,
          ImageKey = "folder-16",
          HasChildren = true,
          Children = s.Ports.OfType<Port>().Select(p => (IEditorTreeNode)new EditorTreeNode() {
            Name = p.Name,
            ImageKey = "folder-16",
            HasChildren = true,
            Children = _descrip.Bindings[p.Binding.Name].Operations.OfType<OperationBinding>()
              .OrderBy(o => o.Name, StringComparer.OrdinalIgnoreCase)
              .Select(o =>
              (IEditorTreeNode)new EditorTreeNode() {
                Name = o.Name,
                ImageKey = "xml-tag-16",
                HasChildren = false,
                Scripts = GetScripts(o)
              })
          })
        }));
    }

    private IEnumerable<IEditorScript> GetScripts(OperationBinding opBinding)
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
        var generator = new Microsoft.Xml.XMLGen.XmlSampleGenerator(_schemas, elem);

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
  }
}
