using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace Pipes.Sgml.Selector
{
  public class ComplexSelector : BaseSelector, IEnumerable<CombinatorSelector>
  {
    private readonly List<CombinatorSelector> _selectors;

    public ComplexSelector()
    {
      _selectors = new List<CombinatorSelector>();
    }

    public ComplexSelector AppendSelector(BaseSelector selector, Combinator combinator)
    {
      _selectors.Add(new CombinatorSelector(selector, combinator));
      return this;
    }

    public IEnumerator<CombinatorSelector> GetEnumerator()
    {
      return _selectors.GetEnumerator();
    }

    internal void ConcludeSelector(BaseSelector selector)
    {
      _selectors.Add(new CombinatorSelector { Selector = selector });
    }

    public int Length
    {
      get { return _selectors.Count; }
    }

    public CombinatorSelector this[int index]
    {
      get
      {
        return _selectors[index];
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)_selectors).GetEnumerator();
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
