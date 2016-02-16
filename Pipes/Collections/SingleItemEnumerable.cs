using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Collections
{
  public class SingleItemEnumerable<T> : IEnumerable<T>
  {
    private T _item;

    public SingleItemEnumerable(T item)
    {
      _item = item;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
      yield return _item;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
