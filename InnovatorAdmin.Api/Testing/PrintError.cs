using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Prints the error from any queries that aren't followed by an AssertMatch
  /// </summary>
  public class PrintError : ITestCommand
  {
    private XElement _errorNode;

    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    public string Comment { get; set; }
    /// <summary>
    /// Error node that was found after the last execution
    /// </summary>
    public XElement ErrorNode { get { return _errorNode; } }

    /// <summary>
    /// Code for executing the command
    /// </summary>
    public async Task Run(TestContext context)
    {
      _errorNode = GetPrimaryPath(context.LastResult)
        .FirstOrDefault(e => e.Name.LocalName == "Fault" && e.Name.NamespaceName == "http://schemas.xmlsoap.org/soap/envelope/");
    }

    private IEnumerable<XElement> GetPrimaryPath(XElement root)
    {
      yield return root;
      var curr = root.Elements().FirstOrDefault();
      while (curr != null)
      {
        yield return curr;
        curr = curr.Elements().FirstOrDefault();
      }
    }

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
