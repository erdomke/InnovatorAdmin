using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class Row : IRow
  {
    private IEnumerable<ICell> _cells;
    private int _count;

    public IEnumerable<ICell> Cells
    {
      get { return _cells; }
    }
    public int FieldCount
    {
      get { return _count; }
    }

    public Row(params ICell[] cells)
    {
      _cells = cells;
      _count = cells.Length;
    }
    public Row(IEnumerable<ICell> cells)
    { 
      var collection = cells as ICollection<ICell>;
      _cells = cells;
      _count = (collection == null ? _cells.Count() : collection.Count);
    }
    public Row(IEnumerable<ICell> cells, int count)
    {
      _cells = cells;
      _count = count;
    }

    public object Item(string name)
    {
      var cell = _cells.FirstOrDefault(c => c.Name == name);
      return cell == null ? null : cell.Value;
    }
    public IEnumerator<IFieldValue> GetEnumerator()
    {
      return _cells.Cast<IFieldValue>().GetEnumerator();
    }
    public FieldStatus Status(string name)
    {
      var cell = _cells.FirstOrDefault(c => c.Name == name);
      if (cell == null)
      {
        return FieldStatus.Undefined;
      }
      else if (cell.Value == null)
      {
        return FieldStatus.Null;
      }
      else if (cell.Value == string.Empty)
      {
        return FieldStatus.Empty;
      }
      else
      {
        return FieldStatus.FilledIn;
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
