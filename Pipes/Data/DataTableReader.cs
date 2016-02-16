using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Pipes.Data
{
  public class DataTableReader : Table.ITable
  {
    private DataTable _table;

    public DataTableReader(DataTable table)
    {
      _table = table;
    }

    public IEnumerable<Table.IColumnMetadata> Columns
    {
      get 
      {
        return _table.Columns.OfType<DataColumn>().Select((c) => (Table.IColumnMetadata)new DataColumnMetadata(c));
      }
    }

    public IEnumerator<IDataRecord> GetEnumerator()
    {
      foreach (DataRow row in _table.Rows)
      {
        yield return new DataRowRecord(row);
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private class DataRowRecord : IDataRecord
    {
      private DataRow _row;

      public DataRowRecord(DataRow row)
      {
        _row = row;
      }

      public int FieldCount
      {
        get { return _row.Table.Columns.Count; }
      }

      public object Item(string name)
      {
        return _row[name];
      }

      public FieldStatus Status(string name)
      {
        if (_row.IsNull(name))
        {
          return FieldStatus.Null;
        }
        else if (_row[name] is string && (string)_row[name] == "")
        {
          return FieldStatus.Empty;
        }
        else
        {
          return FieldStatus.FilledIn;
        }
      }

      public IEnumerator<IFieldValue> GetEnumerator()
      {
        for (var i = 0; i < _row.Table.Columns.Count; i++)
        {
          yield return new FieldValue() { Name = _row.Table.Columns[i].ColumnName, Value = _row[i] };
        }
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }
    private class DataColumnMetadata : Table.IColumnMetadata
    {
      private DataColumn _column;

      public DataColumnMetadata(DataColumn column)
      {
        _column = column;
      }

      public Type DataType
      {
        get { return _column.DataType; }
      }

      public string Label
      {
        get { return _column.Caption; }
      }

      public string Name
      {
        get { return _column.ColumnName; }
      }
    }
  }
}
