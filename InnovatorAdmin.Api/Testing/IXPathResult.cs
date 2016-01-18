using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace InnovatorAdmin.Testing
{
  public interface IXPathResult
  {
    bool EqualsString(string compare);
  }

  public static class XPathResult
  {
    public static IXPathResult Evaluate(XElement elem, string xPath)
    {
      if (xPath == "x:NewId()")
        return new StringXpathResult(Guid.NewGuid().ToString("N").ToUpperInvariant());

      if (elem == null)
        throw new InvalidOperationException("Cannot match an XPath when data is available");

      var output = elem.XPathEvaluate(xPath);
      if (output is bool)
      {
        return new BoolXpathResult((bool)output);
      }
      else if (output is double)
      {
        return new NumericXpathResult((double)output);
      }
      else if (output is string)
      {
        return new StringXpathResult((string)output);
      }
      else if (output is IEnumerable)
      {
        var enumerable = (IEnumerable)output;
        if (!enumerable.OfType<XObject>().Any())
          return new EmptyXpathResult();

        if (enumerable.OfType<XText>().Any())
          return new StringXpathResult(enumerable.OfType<XText>().GroupConcat("", t => t.Value));

        if (enumerable.OfType<XAttribute>().Any())
          return new StringXpathResult(enumerable.OfType<XAttribute>().GroupConcat("", t => t.Value));

        if (enumerable.OfType<XElement>().Any())
        {
          var elems = enumerable.OfType<XElement>().ToArray();
          if (elems.Length == 1 && elems[0].Nodes().Count() == 1 && elems[0].Nodes().First() is XText)
          {
            return new StringXpathResult(((XText)elems[0].Nodes().First()).Value);
          }
          else
          {
            return new ElementsXpathResult() { Elements = enumerable.OfType<XElement>().ToArray() };
          }
        }

        throw new NotSupportedException();
      }
      else
      {
        throw new NotSupportedException();
      }
    }
  }

  public class BoolXpathResult : IXPathResult
  {
    private bool _value;

    public BoolXpathResult(bool value) { _value = value; }

    public bool EqualsString(string compare)
    {
      return (_value && (string.Equals(compare, "1") || string.Equals(compare, "true", StringComparison.OrdinalIgnoreCase)))
        || (!_value && (string.Equals(compare, "0") || string.Equals(compare, "false", StringComparison.OrdinalIgnoreCase)));
    }

    public override string ToString() { return _value ? "1" : "0"; }
  }
  public class NumericXpathResult : IXPathResult
  {
    private double _value;

    public NumericXpathResult(double value) { _value = value; }

    public bool EqualsString(string compare)
    {
      double dblCompare;
      return double.TryParse(compare, out dblCompare) && _value == dblCompare;
    }

    public override string ToString() { return _value.ToString(); }
  }
  public class StringXpathResult : IXPathResult
  {
    private string _value;

    public StringXpathResult(string value) { _value = value; }

    public bool EqualsString(string compare)
    {
      return string.Equals(_value, compare);
    }

    public override string ToString() { return _value; }
  }
  public class EmptyXpathResult : IXPathResult
  {
    public bool EqualsString(string compare)
    {
      return false;
    }

    public override string ToString() { return string.Empty; }
  }


}
