using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Pipes.Sgml.Selector
{
  public abstract class SelectorList : BaseSelector, IEnumerable<ISelector>
  {
    protected List<ISelector> Selectors;

    protected SelectorList()
    {
      Selectors = new List<ISelector>();
    }

    public int Length
    {
      get { return Selectors.Count; }
    }

    public ISelector this[int index]
    {
      get { return Selectors[index]; }
      set { Selectors[index] = value; }
    }

    public SelectorList AppendSelector(ISelector selector)
    {
      Selectors.Add(selector);
      return this;
    }

    public SelectorList RemoveSelector(ISelector selector)
    {
      Selectors.Remove(selector);
      return this;
    }

    public SelectorList ClearSelectors()
    {
      Selectors.Clear();
      return this;
    }

    public IEnumerator<ISelector> GetEnumerator()
    {
      return Selectors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public abstract override void Visit(ISelectorVisitor visitor);
  }
}
