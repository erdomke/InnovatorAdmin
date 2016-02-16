using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class DataRecord : IDataRecord
  {
    private string[] _columnNames;
    private object[] _data;

    public DataRecord(object[] data)
    {
      _data = data;
    }
    public DataRecord(object[] data, string[] columnNames)
    {
      if (columnNames != null && data.Length != columnNames.Length)
        throw new ArgumentException(string.Format("Column count {0} does not match data count {1}. Line data = {2}", columnNames.Length, data.Length, data.GroupConcat(", ", d => d.ToString())));
      _data = data;
      _columnNames = columnNames;
    }

    public int FieldCount
    {
      get { return _data.Length; }
    }

    public object Item(string name)
    {
      return _data[ColumnIndex(name)];
    }

    private int ColumnIndex(string name)
    {
      int idx;

      if (_columnNames == null)
      {
        if (int.TryParse(name, out idx) && idx < _data.Length)
        {
          return idx;
        }
        else
        {
          throw new ArgumentException("A column with that name is not in the table.");
        }
      }
      else
      {
        idx = Array.IndexOf(_columnNames, name);
        if (idx < 0) throw new ArgumentException("A column with that name is not in the table.");
        return idx;
      }
    }

    public FieldStatus Status(string name)
    {
      var idx = ColumnIndex(name);
      if (_data[idx] == null)
      {
        return FieldStatus.Null;
      }
      else if (_data[idx] is string && (string)_data[idx] == "")
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
      for(int i = 0; i<_data.Length; i++)
      {
        if (_columnNames == null)
        {
          yield return new FieldValue() { Name = i.ToString(), Value = _data[i] };
        }
        else
        {
          yield return new FieldValue() { Name = _columnNames[i], Value = _data[i] };
        }
      }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
