using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client.Connection
{
  internal class Endpoints
  {
    private string[] _auth;
    private string[] _authWin;
    private string[] _databases;
    private string[] _download;
    private string[] _policyService;
    private string[] _query;
    private string[] _renewSession;

    public Uri Base { get; set; }

    public IEnumerable<string> Auth { get { return _auth ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> AuthWin { get { return _authWin ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> Databases { get { return _databases ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> Download { get { return _download ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> PolicyService { get { return _policyService ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> Query { get { return _query ?? Enumerable.Empty<string>(); } }
    public IEnumerable<string> RenewSession { get { return _renewSession ?? Enumerable.Empty<string>(); } }

    public Endpoints() { }
    public Endpoints(XElement elem)
    {
      foreach (var endpoint in elem.Elements("endpoint"))
      {
        switch (endpoint.Attribute("action").Value)
        {
          case "Auth":
            _auth = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "AuthWin":
            _authWin = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "Databases":
            _databases = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "Download":
            _download = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "Query":
            _query = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "RenewSession":
            _renewSession = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
          case "VerificationService":
            _policyService = endpoint.Elements("uri").Select(e => e.Value).ToArray();
            break;
        }
      }
    }

  }
}
