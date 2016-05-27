using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class ValidationContext : IValidationContext
  {
    private IItem _changes;
    private IServerConnection _conn;
    private IReadOnlyItem _existing;
    private bool _existingLoaded;
    public IResult _result;


    public IItem Item
    {
      get { return _changes; }
    }
    public IServerConnection Conn
    {
      get { return _conn; }
    }
    public IErrorBuilder ErrorBuilder
    {
      get { return _result; }
    }
    public Exception Exception
    {
      get { return _result.Exception; }
    }
    public IReadOnlyItem Existing
    {
      get
      {
        EnsureExisting();
        return _existing;
      }
    }
    public bool IsNew
    {
      get
      {
        EnsureExisting();
        return _existing == null;
      }
    }
    public Action<IItem> QueryDefaults { get; set; }

    public IReadOnlyItem Merged
    {
      get
      {
        if (this.IsNew) return _changes;
        var merges = _existing.Clone();
        var names = new HashSet<string>(_changes.Elements().Select(e => e.Name));
        var toRemove = merges.Elements().Where(e => names.Contains(e.Name)).ToList();
        foreach (var elem in toRemove)
        {
          elem.Remove();
        }

        foreach (var elem in _changes.Elements())
        {
          merges.Add(elem);
        }
        return merges;
      }
    }

    public bool IsBeingSetNull(string name)
    {
      var isNew = this.IsNew;
      var prop = _changes.Property("name");
      return (isNew && !prop.HasValue())
        || (!IsNew && prop.IsNull().AsBoolean(false));
    }

    public bool IsChanging(params string[] names)
    {
      // Are any of the properties in the changing item?
      var propExists = names.Any(n => _changes.Property(n).Exists);
      if (!propExists) return false;

      // Is this new?
      if (this.IsNew) return true;

      return names.Any(n => _changes.Property(n).Value != _existing.Property(n).Value);
    }

    public IReadOnlyProperty NewOrExisting(string name)
    {
      var result = _changes.Property(name);
      if (result.Exists) return result;

      if (this.IsNew)
      {
        var item = _changes as Item;
        if (item == null) return Property.NullProperty;
        return Conn.AmlContext.PropertyTemplate(name, null, item.Node);
      }

      return _existing.Property(name);
    }

    public ValidationContext(IServerConnection conn, IItem changes)
    {
      _changes = changes;
      _conn = conn;
      _result = _conn.AmlContext.Result();
      _result.ErrorContext(_changes);
    }

    private void EnsureExisting()
    {
      if (!_existingLoaded)
      {
        _existingLoaded = true;
        if (!string.IsNullOrEmpty(_changes.Id()))
        {
          var aml = Conn.AmlContext;
          var query = aml.Item(Item.Type(), aml.Id(Item.Id()), aml.Action("get"));
          if (QueryDefaults != null) QueryDefaults.Invoke(query);
          var items = query.Apply(Conn).Items();
          if (items.Any()) _existing = items.Single();
        }
      }
    }
  }
}
