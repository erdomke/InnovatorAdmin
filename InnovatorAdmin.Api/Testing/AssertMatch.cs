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
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
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

    /// <summary>
    /// Code for executing the command
    /// </summary>
    public async Task Run(TestContext context)
    {
      if (context.LastResult == null)
        throw new InvalidOperationException("Cannot assert a match when no query has been run");
      if (string.IsNullOrWhiteSpace(this.Match))
        throw new ArgumentException("No match pattern is specified");

      this.Actual = context.LastResult.ToString();
      var res = XPathResult.Evaluate(context.LastResult, this.Match, context.Connection);
      var elemRes = res as ElementsXpathResult;
      if (elemRes != null)
      {
        this.IsXml = true;
        elemRes.Elements = elemRes.Elements.Select(e => new XElement(e)).ToArray();
        var elems = elemRes.Elements;

        // Remove properties from the actual as needed
        if (RemoveSystemProperties)
        {
          // Remove system properties defined above
          foreach (var sysProp in elems.Descendants().Where(e => _systemProps.Contains(e.Name.LocalName)).ToArray())
          {
            sysProp.Remove();
          }
          // Remote item ID attributes
          foreach (var item in elems.DescendantsAndSelf().Where(e => e.Name.LocalName == "Item"))
          {
            var idAttr = item.Attribute("id");
            if (idAttr != null) idAttr.Remove();
          }
          // Remove source_id properties that aren't necessary
          foreach (var sourceId in elems.Descendants().Where(e => e.Name.LocalName == "source_id" && e.Parent != null
            && e.Parent.Name.LocalName == "Item" && e.Parent.Parent != null && e.Parent.Parent.Name.LocalName == "Relationships").ToArray())
          {
            sourceId.Remove();
          }
          // Remove redundant itemtype properties to save space
          foreach (var itemtype in elems.Descendants().Where(e => e.Name.LocalName == "itemtype" && e.Parent != null
            && e.Parent.Name.LocalName == "Item" && e.Parent.Attribute("typeId") != null
            && string.Equals(e.Value, e.Parent.Attribute("typeId").Value)).ToArray())
          {
            itemtype.Remove();
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

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
