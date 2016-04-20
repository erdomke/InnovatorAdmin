using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  [DebuggerDisplay("Base: {Base}, Compare: {Compare}, Diff? {IsDifferent}")]
  public struct ListCompare
  {
    private int _base;
    private int _compare;
    private bool _isDifferent;

    public int Base
    {
      get { return _base; }
      set { _base = value; }
    }
    public int Compare
    {
      get { return _compare; }
      set { _compare = value; }
    }
    public bool IsDifferent
    {
      get { return _isDifferent; }
      set { _isDifferent = value; }
    }

    public override string ToString()
    {
      return string.Format("{0} <-> {1}, {2}", _base, _compare, _isDifferent);
    }
    public override bool Equals(object obj)
    {
      if (obj is ListCompare) return Equals((ListCompare)obj);
      return false;
    }
    public bool Equals(ListCompare c)
    {
      return c._base == this._base && c._compare == this._compare && c._isDifferent == this._isDifferent;
    }

    public ListCompare(int baseValue, int compareValue)
    {
      _base = baseValue;
      _compare = compareValue;
      _isDifferent = false;
    }

    public static IList<ListCompare> Create<T, TKey>(IEnumerable<T> baseList, IEnumerable<T> compareList, Func<T, TKey> keyGetter) where TKey : IComparable
    {
      var baseKeys = baseList.Select((v, i) => new IndexedKey<TKey>() { Key = keyGetter(v), OriginalIndex = i }).OrderBy(k => k.Key).ToList();
      var compareKeys = compareList.Select((v, i) => new IndexedKey<TKey>() { Key = keyGetter(v), OriginalIndex = i }).OrderBy(k => k.Key).ToList();
      var basePtr = 0;
      var comparePtr = 0;
      var results = new List<ListCompare>();

      while (basePtr < baseKeys.Count && comparePtr < compareKeys.Count)
      {
        switch (baseKeys[basePtr].Key.CompareTo(compareKeys[comparePtr].Key))
        {
          case -1:
            results.Add(new ListCompare(baseKeys[basePtr].OriginalIndex, -1));
            basePtr++;
            break;
          case 1:
            results.Add(new ListCompare(-1, compareKeys[comparePtr].OriginalIndex));
            comparePtr++;
            break;
          default:
            results.Add(new ListCompare(baseKeys[basePtr].OriginalIndex, compareKeys[comparePtr].OriginalIndex));
            basePtr++;
            comparePtr++;
            break;
        }
      }

      while (basePtr < baseKeys.Count)
      {
        results.Add(new ListCompare(baseKeys[basePtr].OriginalIndex, -1));
        basePtr++;
      }

      while (comparePtr < compareKeys.Count)
      {
        results.Add(new ListCompare(-1, compareKeys[comparePtr].OriginalIndex));
        comparePtr++;
      }

      // Re-order the results base on the original indices
      results.Sort((x, y) =>
      {
        var val = x.Base.CompareTo(y.Base);
        if (val == 0) val = x.Compare.CompareTo(y.Compare);
        return val;
      });

      if (!results.Any() || results.Last().Base == -1) return results;

      var resultPtr = 0;
      var insert = -1;
      while (results[resultPtr].Base == -1)
      {
        insert = results.FindIndex(c => c.Compare == results[resultPtr].Compare - 1);
        if (insert <= resultPtr)
        {
          resultPtr++;
        }
        else
        {
          results.Insert(insert + 1, results[resultPtr]);
          results.RemoveAt(resultPtr);
        }
      }

      return results;
    }

    [DebuggerDisplay("Index: {OriginalIndex}, Key: {Key}")]
    private class IndexedKey<T>
    {
      public T Key { get; set; }
      public int OriginalIndex { get; set; }
    }

    public static bool operator ==(ListCompare x, ListCompare y)
    {
      return x.Equals(y);
    }
    public static bool operator !=(ListCompare x, ListCompare y)
    {
      return !x.Equals(y);
    }
  }
}
