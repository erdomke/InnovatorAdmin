using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class GroupedList<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
  {
    private Dictionary<TKey, Grouping<TKey, TElement>> _elements = new Dictionary<TKey, Grouping<TKey, TElement>>();

    public int KeyCount { get { return _elements.Count; } }

    /// <summary>
    /// Adds a key-value-pair to the list
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="element">Value</param>
    /// <returns>Count of values for the key after the add operation completed</returns>
    public int Add(TKey key, TElement element)
    {
      Grouping<TKey, TElement> grouping;
      if (!_elements.TryGetValue(key, out grouping))
      {
        grouping = new Grouping<TKey, TElement>(key);
        _elements[key] = grouping;
      }
      grouping.Add(element);
      return grouping.Count;
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

    private class Grouping<TK, TE> : IGrouping<TK, TE>
    {
      private TK _key;
      private IList<TE> _elements = new List<TE>();

      public int Count
      {
        get { return _elements.Count; }
      }
      public TK Key
      {
        get { return _key; }
      }

      public Grouping(TK key)
      {
        _key = key;
      }

      public void Add(TE element)
      {
        _elements.Add(element);
      }

      public IEnumerator<TE> GetEnumerator()
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
