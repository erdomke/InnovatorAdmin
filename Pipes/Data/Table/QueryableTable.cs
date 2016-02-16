using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  internal class QueryableTable : IQueryableTable
  {
    private ITable _source;
    private Dictionary<string, IColumnMetadata> _columns;
    private List<Func<IDataRecord, bool>> _criteria;

    public QueryableTable(ITable source)
    {
      _source = source;
      _columns = new Dictionary<string, IColumnMetadata>();
      foreach (var col in _source.Columns)
      {
        _columns[col.Name] = col;
      }
      _criteria = new List<Func<IDataRecord, bool>>();
    }

    public void AddColumnFilter(Func<IColumnMetadata, bool> criteria)
    {
      var keys = _columns.Keys.ToList();
      foreach (var key in keys)
      {
        if (!criteria(_columns[key])) _columns.Remove(key);
      }
    }
    public void AddRowFilter(Func<IDataRecord, bool> criteria)
    {
      _criteria.Add(criteria);
    }

    public IEnumerable<IColumnMetadata> Columns
    {
      get { return _columns.Values; }
    }

    public IEnumerator<IDataRecord> GetEnumerator()
    {
      bool skip;
      foreach (var row in _source)
      {
        skip = false;
        foreach (var criteria in _criteria)
        {
          if (!criteria(row))
          {
            skip = true;
            break;
          }
        }

        if (!skip)
        {
          yield return new QueryableRow(row, this);
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool ContainsColumnName(string name)
    {
      return _columns.ContainsKey(name);
    }

    private class QueryableRow : IDataRecord
    {
      private IDataRecord _row;
      private IQueryableTable _parent;

      public QueryableRow(IDataRecord row, IQueryableTable parent)
      {
        _row = row;
        _parent = parent;
      }

      public int FieldCount
      {
        get { return _parent.Columns.Count(); }
      }

      public object Item(string name)
      {
        if (_parent.ContainsColumnName(name))
        {
          return _row.Item(name);
        }
        else
        {
          return null;
        }
      }

      public FieldStatus Status(string name)
      {
        if (_parent.ContainsColumnName(name))
        {
          return _row.Status(name);
        }
        else
        {
          return FieldStatus.Undefined;
        }
      }

      public IEnumerator<IFieldValue> GetEnumerator()
      {
        foreach (var val in _row)
        {
          if (_parent.ContainsColumnName(val.Name))
          {
            yield return val;
          }
        }
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }
  }
}
