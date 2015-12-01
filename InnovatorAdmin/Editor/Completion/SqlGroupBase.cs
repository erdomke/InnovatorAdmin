using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public interface ISearchableSqlNode
  {
    SqlNode NodeByOffset(int offset);
  }

  public class SqlGroupBase<T> : SqlNode, ICollection<T>, ISearchableSqlNode where T : SqlNode
  {
    private List<T> _nodes = new List<T>();

    public T this[int i]
    {
      get { return _nodes[i]; }
      set { _nodes[i] = value; }
    }

    public void Add(T item)
    {
      _nodes.Add(item);
      item.Parent = this;
      if (this.StartOffset < 0)
        this.StartOffset = item.StartOffset;
      else
        this.StartOffset = Math.Min(this.StartOffset, item.StartOffset);
    }

    public SqlNode NodeByOffset(int offset)
    {
      if (offset < this.StartOffset) return null;
      var i = _nodes.Count - 1;
      while (i >= 0 && offset < _nodes[i].StartOffset) i--;
      var searchable = _nodes[i] as ISearchableSqlNode;
      if (searchable == null) return _nodes[i];
      return searchable.NodeByOffset(offset);
    }

    public void Clear()
    {
      _nodes.Clear();
    }

    public bool Contains(T item)
    {
      return _nodes.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _nodes.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _nodes.Count; }
    }

    public int IndexOf(T value)
    {
      return _nodes.IndexOf(value);
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      return _nodes.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _nodes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override string ToString()
    {
      return _nodes.GroupConcat(" ", n => n.ToString());
    }
  }
}
