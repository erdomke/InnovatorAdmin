using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface ISqlGroupNode
  {
    SqlNode NodeByOffset(int offset);
    IEnumerable<SqlNode> Items { get; }
    int IndexOf(SqlNode node);
  }

  public class SqlGroupBase<T> : SqlNode, ICollection<T>, ISqlGroupNode where T : SqlNode
  {
    public override int StartOffset
    {
      get
      {
        if (base.StartOffset < 0 && _nodes.Any())
          base.StartOffset = _nodes.Min(n => n.StartOffset);
        return base.StartOffset;
      }
      set
      {
        if (value >= 0)
          throw new InvalidOperationException();
        base.StartOffset = value;
      }
    }

    private List<T> _nodes = new List<T>();

    public SqlGroupBase()
    {
      this.Type = SqlType.Query;
    }

    public T this[int i]
    {
      get { return _nodes[i]; }
      set { _nodes[i] = value; }
    }

    public void Add(T item)
    {
      _nodes.Add(item);
      item.Parent = this;
    }

    public SqlNode NodeByOffset(int offset)
    {
      if (offset < this.StartOffset) return null;
      var i = _nodes.Count - 1;
      while (i >= 0 && offset < _nodes[i].StartOffset) i--;
      var searchable = _nodes[i] as ISqlGroupNode;
      if (searchable == null) return _nodes[i];
      return searchable.NodeByOffset(offset);
    }

    public void Clear()
    {
      _nodes.Clear();
      base.StartOffset = -1;
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

    public IEnumerable<SqlNode> Items
    {
      get { return this.OfType<SqlNode>(); }
    }


    int ISqlGroupNode.IndexOf(SqlNode node)
    {
      var ofType = node as T;
      if (ofType == null)
        return -1;
      return _nodes.IndexOf(ofType);
    }
  }
}
