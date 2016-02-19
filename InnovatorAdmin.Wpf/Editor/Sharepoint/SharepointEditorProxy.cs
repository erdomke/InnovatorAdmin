using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using Innovator.Client.Connection;
using System.Net;
using System.Xml.Linq;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Schema;

namespace InnovatorAdmin.Editor
{
  public class SharepointEditorProxy : IEditorProxy
  {
    private XNamespace ns = (XNamespace)"http://schemas.microsoft.com/sharepoint/soap/";
    private IEditorHelper _helper = new XmlEditorHelper();
    private DefaultHttpService _http = new DefaultHttpService();
    private Connections.ConnectionData _connData;
    private System.Net.ICredentials _cred;
    private string _siteUrl;
    private XmlSchemaSet _schemas;

    private ServiceDescription _wsdlLists;
    private ServiceDescription _wsdlSiteData;

    public string Action { get; set; }

    public Connections.ConnectionData ConnData { get { return _connData; } }

    public SharepointEditorProxy(Connections.ConnectionData connData)
    {
      _connData = connData;

      switch (_connData.Authentication)
      {
        case Connections.Authentication.Windows:
          _cred = CredentialCache.DefaultNetworkCredentials;
          break;
        case Connections.Authentication.Explicit:
          _cred = new NetworkCredential(_connData.UserName, _connData.Password);
          break;
      }
    }

    public async Task<IEditorProxy> Initialize()
    {
      // Get the full Url
      var url = new Uri(_connData.Url).GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/_vti_bin/sitedata.asmx";
      var data = await CallWebServiceAsync(url, "http://schemas.microsoft.com/sharepoint/soap/GetSiteAndWeb",
        @"<?xml version='1.0' encoding='utf-8'?><soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'><soap:Body><GetSiteAndWeb xmlns='http://schemas.microsoft.com/sharepoint/soap/'><strUrl>" + _connData.Url + "</strUrl></GetSiteAndWeb></soap:Body></soap:Envelope>");
      var resp = XElement.Load(data);
      _siteUrl = resp.Descendants(ns + "strWeb").First().Value;

      var listDataWsdl = _http.Execute("GET", _siteUrl + "/_vti_bin/lists.asmx?wsdl", null, _cred, true, null).ToTask();
      var siteDataWsdl = _http.Execute("GET", _siteUrl + "/_vti_bin/sitedata.asmx?wsdl", null, _cred, true, null).ToTask();

      _wsdlLists = LoadWsdl((await listDataWsdl).AsStream);
      _wsdlSiteData = LoadWsdl((await siteDataWsdl).AsStream);

      _schemas = InnovatorAdmin.Editor.XmlSchemas.SchemasFromDescrip(_wsdlLists);
      return this;
    }

    private ServiceDescription LoadWsdl(Stream data)
    {
      using (var reader = new StreamReader(data))
      using (var xml = XmlReader.Create(reader))
      {
        return ServiceDescription.Read(xml);
      }
    }

    public string Name
    {
      get { return this.ConnData.ConnectionName; }
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
      throw new NotImplementedException();
    }

    public Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async, Action<int, string> progressCallback)
    {
      throw new NotImplementedException();
    }

    public Innovator.Client.IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      return GetSiteNodes().ToPromise()
        .Convert(n => (IEnumerable<IEditorTreeNode>)new IEditorTreeNode[] {
          new EditorTreeNode()
          {
            Name = "Site Content",
            Image = Icons.Folder16,
            HasChildren = true,
            Children = n
          },
          new EditorTreeNode()
          {
            Name = "------",
          },
          SoapEditorProxy.ServiceTreeNode(_wsdlLists, _schemas),
          SoapEditorProxy.ServiceTreeNode(_wsdlSiteData, _schemas)
        });
    }

    private async Task<IEnumerable<IEditorTreeNode>> GetSiteNodes()
    {
      var data = await CallWebServiceAsync(_siteUrl + "/_vti_bin/sitedata.asmx", "http://schemas.microsoft.com/sharepoint/soap/GetListCollection",
        @"<?xml version='1.0' encoding='utf-8'?><soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'><soap:Body><GetListCollection xmlns='http://schemas.microsoft.com/sharepoint/soap/' /></soap:Body></soap:Envelope>");
      var resp = XElement.Load(data);
      var results = new List<IEditorTreeNode>();
      foreach (var elem in resp.Descendants(ns + "_sList"))
      {
        results.Add(new EditorTreeNode()
        {
          Name = elem.Element(ns + "Title").Value,
          Description = elem.Element(ns + "BaseType").Value,
          Image = Icons.Class16,
          HasChildren = true,
          ChildGetter = () => GetListProperties(elem.Element(ns + "InternalName").Value)
            .OrderBy(n => n.Name)
        });
      }
      return results.OrderBy(n => n.Name);
    }

    private IEnumerable<IEditorTreeNode> GetListProperties(string name)
    {
      var data = CallWebService(_siteUrl + "/_vti_bin/lists.asmx", "http://schemas.microsoft.com/sharepoint/soap/GetList",
        @"<?xml version='1.0' encoding='utf-8'?><soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'><soap:Body><GetList xmlns='http://schemas.microsoft.com/sharepoint/soap/'><listName>" + name + "</listName></GetList></soap:Body></soap:Envelope>");
      var resp = XElement.Load(data);
      foreach (var field in resp.Descendants(ns + "Fields").First().Elements(ns + "Field"))
      {
        var typeText = field.AttributeValue("Type");
        if (field.Attribute("MaxLength") != null)
          typeText += "[" + field.AttributeValue("MaxLength") + "]";

        yield return new EditorTreeNode()
        {
          Name = field.AttributeValue("DisplayName") ?? field.AttributeValue("StaticName"),
          Description = "Field " + field.AttributeValue("StaticName") + ": " + typeText,
          Image = Icons.Property16
        };
      }
    }

    private Stream CallWebService(string url, string soapAction, string payload)
    {
      return _http.Execute("POST", url, null, _cred, false, r =>
      {
        r.SetHeader("SOAPAction", "\"" + soapAction + "\"");
        r.SetContent(w => w.Write(payload), "text/xml;charset=UTF-8");
      }).Wait().AsStream;
    }
    private async Task<Stream> CallWebServiceAsync(string url, string soapAction, string payload)
    {
      var resp = await _http.Execute("POST", url, null, _cred, true, r =>
      {
        r.SetHeader("SOAPAction", "\"" + soapAction + "\"");
        r.SetContent(w => w.Write(payload), "text/xml;charset=UTF-8");
      }).ToTask();
      return resp.AsStream;
    }

    public void Dispose()
    {
      // Do nothing
    }
  }
}
