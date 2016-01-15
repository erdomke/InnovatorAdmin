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
    private static HashSet<string> _systemProps = new HashSet<string>(new string[]
    {
      "created_on", "created_by_id", "modified_on", "modified_by_id", "id", "config_id", "keyed_name"
    });

    public string Actual { get; set; }
    public string Comment { get; set; }
    public string Expected { get; set; }
    public bool IsXml { get; set; }
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

      this.Actual = context.LastResult.ToString();
      var res = XPathResult.Evaluate(context.LastResult, this.Match);
      var elemRes = res as ElementsXpathResult;
      if (elemRes != null)
      {
        elemRes.Elements = elemRes.Elements.Select(e => new XElement(e)).ToArray();
        var elems = elemRes.Elements;

        // Remove properties from the actual as needed
        if (RemoveSystemProperties)
        {
          foreach (var sysProp in elems.Descendants().Where(e => _systemProps.Contains(e.Name.LocalName)).ToArray())
          {
            sysProp.Remove();
          }
          foreach (var item in elems.DescendantsAndSelf().Where(e => e.Name.LocalName == "Item"))
          {
            var idAttr = item.Attribute("id");
            if (idAttr != null) idAttr.Remove();
          }
        }
        foreach (var remove in _removes)
        {
          foreach (var e in elems)
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
