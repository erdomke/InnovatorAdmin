using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DbParams : IDataParameterCollection, IEnumerable<IDbDataParameter>
  {
    private List<IDbDataParameter> _params = new List<IDbDataParameter>();

    public int Count
    {
      get { return _params.Count; }
    }

    public void Add(string name, object value)
    {
      if (!_params.Any(p => p.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase)))
      {
        Add(new DbParameter() { ParameterName = name, Value = value });
      }
    }
    public void Add(IDbDataParameter param)
    {
      if (!_params.Contains(param)) _params.Add(param);
    }

    public IDbDataParameter ByName(string name)
    {
      return _params.FirstOrDefault(p => p.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void Clear()
    {
      _params.Clear();
    }

    public bool Contains(string parameterName)
    {
      return _params.Any(p => p.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
    }

    public int IndexOf(string parameterName)
    {
      for (var i = 0; i < _params.Count; i++)
      {
        if (_params[i].ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase)) return i;
      }
      return -1;
    }

    public void RemoveAt(string parameterName)
    {
      _params.RemoveAt(IndexOf(parameterName));
    }

    public void RemoveAt(int index)
    {
      _params.RemoveAt(index);
    }

    public IEnumerator<IDbDataParameter> GetEnumerator()
    {
      return _params.GetEnumerator();
    }

    #region Implementation
    int IList.Add(object value)
    {
      this.Add((IDbDataParameter)value);
      return _params.Count - 1;
    }

    object IDataParameterCollection.this[string parameterName]
    {
      get
      {
        return _params.First(p => p.ParameterName == parameterName);
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public object this[int index]
    {
      get { return _params[index]; }
      set { _params[index] = (IDbDataParameter)value; }
    }

    bool IList.Contains(object value)
    {
      return _params.Contains((IDbDataParameter)value);
    }

    int IList.IndexOf(object value)
    {
      return _params.IndexOf((IDbDataParameter)value);
    }

    void IList.Insert(int index, object value)
    {
      _params.Insert(index, (IDbDataParameter)value);
    }

    bool IList.IsFixedSize
    {
      get { return false; }
    }

    bool IList.IsReadOnly
    {
      get { return false; }
    }

    void IList.Remove(object value)
    {
      _params.Remove((IDbDataParameter)value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      ((IList)_params).CopyTo(array, index);
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get { return null; }
    }
    #endregion

    System.Collections.IEnumerator IEnumerable.GetEnumerator()
    {
      return _params.GetEnumerator();
    }
  }
}
