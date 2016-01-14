using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace InnovatorAdmin.Testing
{
  public class AssertMatch : ITestCommand
  {
    private List<string> _removes = new List<string>();

    public string Comment { get; set; }
    public string Expected { get; set; }
    public string Match { get; set; }
    public bool RemoveSystemProperties { get; set; }
    public IList<string> Removes { get { return _removes; } }

    public AssertMatch()
    {
      this.RemoveSystemProperties = true;
    }

    public async Task Run(TestContext context)
    {
      if (context.LastResult == null)
        throw new InvalidOperationException("Cannot assert a match when no query has been run");
      if (string.IsNullOrWhiteSpace(this.Match))
        throw new ArgumentException("No match pattern is specified");

      var res = XPathResult.Evaluate(context.LastResult, this.Match);
      var elemRes = res as ElementsXpathResult;
      if (elemRes != null)
      {
        var elem = elemRes.Elements;

        // Remove properties from the actual as needed
        if (RemoveSystemProperties)
        {
          foreach (var sysProp in elem.Descendants("created_on").Concat(elem.Descendants("modified_on")).ToArray())
          {
            sysProp.Remove();
          }
        }
        foreach (var remove in _removes)
        {
          foreach (var e in elem)
          {
            foreach (var toRemove in e.XPathSelectElements(remove).ToArray())
            {
              toRemove.Remove();
            }
          }
        }

        if (!elemRes.EqualsString(this.Expected))
        {
          this.Expected = elemRes.ToString();
          throw new AssertionFailedException(GetFaultString(context.LastResult, res));
        }
      }
      else if (!res.EqualsString(this.Expected))
      {
        this.Expected = res.ToString();
        throw new AssertionFailedException(GetFaultString(context.LastResult, res));
      }
    }

    private string GetFaultString(XElement elem, IXPathResult result)
    {
      var fault = elem.Descendants("faultstring").FirstOrDefault();
      if (fault == null)
        return result.GetType().Name + " did not match the expected value";
      return "Aras Fault: " + fault.Value;
    }

  }
}
