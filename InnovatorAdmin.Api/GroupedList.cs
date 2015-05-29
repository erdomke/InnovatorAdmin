using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class GroupedList<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
  {
    private Dictionary<TKey, Grouping<TKey, TElement>> _elements = new Dictionary<TKey, Grouping<TKey, TElement>>();

    public int KeyCount { get { return _elements.Count; } }
    public void Add(TKey key, TElement element)
    {
      Grouping<TKey, TElement> grouping;
      if (!_elements.TryGetValue(key, out grouping))
      {
        grouping = new Grouping<TKey, TElement>(key);
        _elements[key] = grouping;
      }
      grouping.Add(element);
    }
    public bool TryGetElements(TKey key, out IEnumerable<TElement> element)
    {
      element = null;
      Grouping<TKey, TElement> grouping;
      if (_elements.TryGetValue(key, out grouping))
      {
        element = grouping;
        return true;
      }
      return false;
    }

    public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
    {
      return _elements.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _elements.Values.GetEnumerator();
    }

    private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
      private TKey _key;
      private IList<TElement> _elements = new List<TElement>();

      public Grouping(TKey key)
      {
        _key = key;
      }

      public TKey Key
      {
        get { return _key; }
      }

      public void Add(TElement element)
      {
        _elements.Add(element);
      }

      public IEnumerator<TElement> GetEnumerator()
      {
        return _elements.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return _elements.GetEnumerator();
      }
    }
  }
}
