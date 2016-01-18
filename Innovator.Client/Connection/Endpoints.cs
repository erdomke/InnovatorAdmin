using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client.Connection
{
  internal class Endpoints
  {
    private List<String> _databases = new List<string>();
    private List<String> _download = new List<string>();
    private List<String> _policyService = new List<string>();
    private List<String> _query = new List<string>();
    private List<String> _renewSession = new List<string>();

    public Uri Base { get; set; }

    public IList<string> Databases { get { return _databases; } }
    public IList<string> Download { get { return _download; } }
    public IList<string> PolicyService { get { return _policyService; } }
    public IList<string> Query { get { return _query; } }
    public IList<string> RenewSession { get { return _renewSession; } }

    public Endpoints(XElement elem)
    {
      foreach (var endpoint in elem.Elements("endpoint"))
      {
        switch (endpoint.Attribute("action").Value)
        {
          case "Databases":
            _databases = endpoint.Elements("uri").Select(e => e.Value).ToList();
            break;
          case "Download":
            _download = endpoint.Elements("uri").Select(e => e.Value).ToList();
            break;
          case "Query":
            _query = endpoint.Elements("uri").Select(e => e.Value).ToList();
            break;
          case "RenewSession":
            _renewSession = endpoint.Elements("uri").Select(e => e.Value).ToList();
            break;
          case "VerificationService":
            _policyService = endpoint.Elements("uri").Select(e => e.Value).ToList();
            break;
        }
      }
    }

  }
}
