using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  public class TestContext
  {
    private Dictionary<string, string> _parameters = new Dictionary<string, string>();
    private IAsyncConnection _conn;

    public IAsyncConnection Connection { get { return _conn; } }
    public IDictionary<string, string> Parameters { get { return _parameters; } }
    public XElement LastResult { get; set; }

    public TestContext(IAsyncConnection conn)
    {
      _conn = conn;
    }
  }
}
