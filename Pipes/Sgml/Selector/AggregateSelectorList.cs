using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Pipes.Sgml.Selector
{
  public class AggregateSelectorList : SelectorList
  {
    public readonly string Delimiter;

    public AggregateSelectorList(string delimiter)
    {
      if (delimiter.Length > 1)
      {
        throw new ArgumentException("Expected single character delimiter or empty string", "delimiter");
      }

      Delimiter = delimiter;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
