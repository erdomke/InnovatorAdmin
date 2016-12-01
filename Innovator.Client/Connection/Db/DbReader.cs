#if DBDATA
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DbReader : IDataReader
  {
    private DbConnection _conn;
    private int _count = 0;
    private int _depth = 0;
    private int _i = -1;
    private IList<IReadOnlyItem> _items;
    private List<string> _fields;
    private string _type;
    private bool _nextResult;

    public DbReader(DbConnection conn, IEnumerable<IReadOnlyItem> items)
    {
      _conn = conn;
      _items = items.ToList();

      _type = items.Select(i => i.Type().Value).FirstOrDefault();
      _count = _items.Count;
      GetMetadata();

      foreach (var rel in _items.SelectMany(i => i.Relationships())
                                .OrderBy(i => i.Type().Value))
      {
        _items.Add(rel);
      };
    }

    public void Close()
    {
      // Do nothing
    }

    public int Depth
    {
      get { return _depth; }
    }

    public DataTable GetSchemaTable()
    {
      throw new NotSupportedException();
    }

    public bool IsClosed
    {
      get { return false; }
    }

    public bool NextResult()
    {
      return _nextResult;
    }

    public bool Read()
    {
      if (_nextResult)
      {
        _nextResult = false;
        return true;
      }
      else
      {
        _i++;
        var moreItems = _i < _items.Count;
        if (moreItems && _items[_i].Type().Value != _type)
        {
          _type = _items[_i].Type().Value;
          GetMetadata();
          moreItems = false;
          _nextResult = true;
        }
        return moreItems;
      }
    }

    private void GetMetadata()
    {
      var itemsOfType = _items.Skip(_i < 0 ? 0 : _i).TakeWhile(i => i.Type().Value == _type);
      var props = new HashSet<string>();
      IReadOnlyItem childItem;

      foreach (var item in itemsOfType)
      {
        foreach (var prop in item.Elements().OfType<IReadOnlyProperty>())
        {
          props.Add("/" + prop.Name);

          childItem = prop.Elements().OfType<IReadOnlyItem>().FirstOrDefault();
          if (childItem != null)
          {
            foreach (var childProp in childItem.Elements().OfType<IReadOnlyProperty>())
            {
              props.Add(prop.Name + "/" + childProp.Name);
            }
          }
        }
      }

      _fields = new List<string>(props);
      _fields.Sort((x, y) => {
        var xParts = x.Split('/');
        var yParts = y.Split('/');
        var compare = xParts[0].CompareTo(yParts[0]);
        if (compare != 0) return compare;

        if (xParts[1] == "id" && yParts[1] != "id")
        {
          return -1;
        }
        else if (xParts[1] != "id" && yParts[1] == "id")
        {
          return 1;
        }
        return xParts[1].CompareTo(yParts[1]);
      });
    }

    public int RecordsAffected
    {
      get { return _count; }
    }

    public void Dispose()
    {
      // Do nothing
    }

    public int FieldCount
    {
      get { return _fields == null ? 0 : _fields.Count; }
    }

    public bool GetBoolean(int i)
    {
      return Property(_fields[i]).AsBoolean(false);
    }

    public byte GetByte(int i)
    {
      return (byte)Property(_fields[i]).AsInt(0);
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      throw new NotSupportedException();
    }

    public char GetChar(int i)
    {
      return (char)Property(_fields[i]).AsInt(0);
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      throw new NotSupportedException();
    }

    public IDataReader GetData(int i)
    {
      var item = Property(_fields[i]).AsItem();
      if (item == null || !item.Exists) return null;
      var result = new DbReader(_conn, Enumerable.Repeat(item, 1));
      result._depth = _depth + 1;
      return result;
    }

    public string GetDataTypeName(int i)
    {
      return GetDefn(_fields[i]).TypeName;
    }

    public DateTime GetDateTime(int i)
    {
      return Property(_fields[i]).AsDateTime(DateTime.MinValue);
    }

    public decimal GetDecimal(int i)
    {
      var cxt = _conn.CoreConnection.AmlContext.LocalizationContext;
      return cxt.AsDecimal(Property(_fields[i]).Value) ?? 0;
    }

    public double GetDouble(int i)
    {
      return Property(_fields[i]).AsDouble(0);
    }

    public Type GetFieldType(int i)
    {
      return GetDefn(_fields[i]).Type;
    }

    public float GetFloat(int i)
    {
      return (float)Property(_fields[i]).AsDouble(0);
    }

    public Guid GetGuid(int i)
    {
      return new Guid(Property(_fields[i]).Value);
    }

    public short GetInt16(int i)
    {
      return (short)Property(_fields[i]).AsInt(0);
    }

    public int GetInt32(int i)
    {
      return Property(_fields[i]).AsInt(0);
    }

    public long GetInt64(int i)
    {
      var cxt = _conn.CoreConnection.AmlContext.LocalizationContext;
      return cxt.AsLong(Property(_fields[i]).Value) ?? 0;
    }

    public string GetName(int i)
    {
      return _fields[i].Split('/')[1];
    }

    public int GetOrdinal(string name)
    {
      var i = _fields.IndexOf(name);
      if (i >= 0) return i;
      name = "/" + name;
      for (i = 0; i < _fields.Count; i++)
      {
        if (_fields[i].EndsWith(name)) return i;
      }
      return -1;
    }

    public string GetString(int i)
    {
      return Property(_fields[i]).Value;
    }

    public object GetValue(int i)
    {
      return this[_fields[i]];
    }

    public int GetValues(object[] values)
    {
      var count = Math.Min(_fields.Count, values.Length);
      for (var i = 0; i < count; i++)
      {
        values[i] = this[i];
      }
      return count;
    }

    public bool IsDBNull(int i)
    {
      var prop = Property(_fields[i]);
      return !prop.Exists || prop.Attribute("is_null").AsBoolean(false);
    }

    public object this[string name]
    {
      get
      {
        var type = GetDefn(name).Type;
        if (type == typeof(bool))
        {
          return Property(name).AsBoolean(false);
        }
        else if (type == typeof(DateTime))
        {
          return Property(name).AsDateTime(DateTime.MinValue);
        }
        else if (type == typeof(double))
        {
          return Property(name).AsDouble(0);
        }
        else if (type == typeof(int))
        {
          return Property(name).AsInt(0);
        }
        else if (type == typeof(ItemReference))
        {
          return new ItemReference(Property(name).AsItem());
        }
        else
        {
          return Property(name).Value;
        }
      }
    }

    public object this[int i]
    {
      get { return this[_fields[i]]; }
    }

    private IReadOnlyProperty Property(string name)
    {
      if (name[0] == '/')
      {
        return _items[_i].Property(name.Substring(1));
      }
      else
      {
        var i = name.IndexOf('/');
        return _items[_i].Property(name.Substring(0, i)).AsItem().Property(name.Substring(i + 1));
      }
    }
    private PropDefn GetDefn(string name)
    {
      if (name[0] == '/')
      {
        return _conn.GetTypeInfo(_type).GetDefn(name.Substring(1));
      }
      else
      {
        var i = name.IndexOf('/');
        var parent = _conn.GetTypeInfo(_type).GetDefn(name.Substring(0, i));

        return _conn.GetTypeInfo(parent.DataSource).GetDefn(name.Substring(i + 1));
      }
    }
  }
}
#endif
