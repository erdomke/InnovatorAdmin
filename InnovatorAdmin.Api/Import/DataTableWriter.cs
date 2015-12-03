using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class DataTableWriter : IDataWriter
  {
    private DataTable _table;
    private IList<string> _headers;
    private IList<object> _values;
    private DataRow _row;
    private bool _loadingData;

    public DataTable Table
    {
      get 
      { 
        if (_loadingData)
        {
          _table.EndLoadData();
          _loadingData = false;
        }
        return _table; 
      }
    }
    
    public void Reset()
    {
      _table = null;
    }

    public void Row()
    {
      if (_table == null)
      {
        _headers = new List<string>();
        _values = new List<object>();
      }
      else
      {
        if (!_loadingData)
        {
          _table.BeginLoadData();
          _loadingData = true;
        }
        _row = _table.NewRow();
        _row.BeginEdit();
      }
    }

    public void RowEnd()
    {
      if (_table == null)
      {
        _table = new DataTable();
        for (var i = 0; i < _headers.Count; i++)
        {
          _table.Columns.Add(_headers[i], _values[i] == null ? typeof(string) : _values[i].GetType());
        }
        _table.BeginLoadData();
        _loadingData = true;
        _table.LoadDataRow(_values.ToArray(), true);
        _headers = null;
        _values = null;
      }
      else
      {
        _row.EndEdit();
        _table.Rows.Add(_row);
        _row = null;
      }
    }

    public void Cell(string columnName, object value)
    {
      if (_table == null)
      {
        _headers.Add(columnName);
        _values.Add(value);
      }
      else
      {
        _row[columnName] = value;
      }
    }
  }
}
