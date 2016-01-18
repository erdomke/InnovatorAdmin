using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Client.Connection
{
  public class DbConnection : IDbConnection
  {
    private IConnection _conn;

    internal IConnection CoreConnection { get { return _conn; } }

    public DbConnection(IConnection conn)
    {
      _conn = conn;
    }

    public DbCommand CreateCommand()
    {
      return new DbCommand(this);
    }

    public string Database
    {
      get { return _conn.Database; }
    }

    public ConnectionState State
    {
      get { return ConnectionState.Open; }
    }

    #region "Implementation"
    IDbCommand IDbConnection.CreateCommand()
    {
      return new DbCommand(this);
    }
    
    void IDbConnection.Close()
    {
      // Do nothing
    }

    void IDisposable.Dispose()
    {
      // Do nothing
    }

    void IDbConnection.Open()
    {
      throw new NotSupportedException();
    }

    int IDbConnection.ConnectionTimeout
    {
      get { return DefaultHttpService.DefaultTimeout; }
    }
    
    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
    {
      throw new NotSupportedException();
    }
    IDbTransaction IDbConnection.BeginTransaction()
    {
      throw new NotSupportedException();
    }
    void IDbConnection.ChangeDatabase(string databaseName)
    {
      throw new NotSupportedException();
    }
    string IDbConnection.ConnectionString
    {
      get { return _conn.Database; }
      set { throw new NotSupportedException(); }
    }
    #endregion

    private Dictionary<string, Properties> _typeInfo = new Dictionary<string,Properties>();
    internal Properties GetTypeInfo(string name)
    {
      Properties result;
      if (_typeInfo.TryGetValue(name, out result)) return result;
      var res = _conn.Apply(@"<Item type='Property' action='get' select='name,data_type,foreign_property(data_type),data_source'>
                                  <source_id><Item type='ItemType' action='get'><name>@0</name></Item></source_id>
                                </Item>", name);
      result = new Properties(res.AssertItems());
      _typeInfo[name] = result;
      return result;
    }
  }

  internal class Properties
  {
    private Dictionary<string, PropDefn> _typeInfo;

    public Properties(IEnumerable<IReadOnlyItem> props)
    {
      _typeInfo = props.ToDictionary(p => p.Property("name").Value, p => new PropDefn(p));
    }

    public PropDefn GetDefn(string name)
    {
      PropDefn result;
      if (_typeInfo.TryGetValue(name, out result)) return result;
      return new PropDefn();
    }
  }

  public class PropDefn
  {
    public string TypeName { get; set; }
    public Type Type { get; set; }
    public string DataSource { get; set; }

    public PropDefn()
    {
      this.Type = typeof(string);
      this.TypeName = "string";
    }
    public PropDefn(IReadOnlyItem prop)
    {
      this.TypeName = prop.Property("data_type").Value;
      if (this.TypeName == "foreign")
      {
        this.TypeName = prop.Property("foreign_property").AsItem().Property("data_type").AsString(this.TypeName);
      }

      switch (this.TypeName)
      {
        case "boolean":
          this.Type = typeof(bool);
          break;
        case "date":
          this.Type = typeof(DateTime);
          break;
        case "decimal":
        case "float":
          this.Type = typeof(double);
          break;
        //case "formatted text": --> what to do?
        //case "image":
        case "integer":
          this.Type = typeof(int);
          break;
        case "item":
          this.Type = typeof(ItemReference);
          break;
        default:
          this.Type = typeof(string);
          break;
      }

      if (prop.Property("data_source").Type().Value == "ItemType")
      {
        this.DataSource = prop.Property("data_source").Attribute("name").Value;
      }

    }
  }

  public class ItemReference : IReadOnlyItem
  {
    private IReadOnlyItem _item;

    public static implicit operator string(ItemReference itemRef)
    {
      return itemRef._item.Id();
    }
    public static implicit operator Guid(ItemReference itemRef)
    {
      return new Guid(itemRef._item.Id());
    }

    public ItemReference(IReadOnlyItem item)
    {
      _item = item;
    }

    public IReadOnlyResult AsResult()
    {
      return _item.AsResult();
    }

    public IItem Clone()
    {
      return _item.Clone();
    }

    public string Id()
    {
      return _item.Id();
    }

    public IReadOnlyProperty Property(string name)
    {
      return _item.Property(name);
    }

    public IEnumerable<IReadOnlyItem> Relationships()
    {
      return _item.Relationships();
    }

    public IEnumerable<IReadOnlyItem> Relationships(string type)
    {
      return _item.Relationships(type);
    }

    public IReadOnlyAttribute Attribute(string name)
    {
      return _item.Attribute(name);
    }

    public IEnumerable<IReadOnlyAttribute> Attributes()
    {
      return _item.Attributes();
    }

    public IEnumerable<IReadOnlyElement> Elements()
    {
      return _item.Elements();
    }

    public IServerContext Context
    {
      get { return _item.Context; }
    }

    public bool Exists
    {
      get { return _item.Exists; }
    }

    public string Name
    {
      get { return _item.Name; }
    }

    public IReadOnlyElement Parent
    {
      get { return _item.Parent; }
    }

    public string Value
    {
      get { return _item.Value; }
    }

    public string ToAml()
    {
      return _item.ToAml();
    }

    object ICloneable.Clone()
    {
      return _item.Clone();
    }

    public IReadOnlyProperty Property(string name, string lang)
    {
      return _item.Property(name, lang);
    }
  }
  
}
