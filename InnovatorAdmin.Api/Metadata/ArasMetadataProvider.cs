using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin
{
  public class ArasMetadataProvider
  {
    private IAsyncConnection _conn;
    private Dictionary<ItemProperty, ItemReference> _customProps
      = new Dictionary<ItemProperty,ItemReference>();
    private Dictionary<string, ItemType> _itemTypesByName
      = new Dictionary<string, ItemType>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ItemType> _itemTypesById;
    private IPromise _metadataComplete;
    private IEnumerable<Method> _methods = Enumerable.Empty<Method>();
    private IEnumerable<ItemReference> _polyItemLists = Enumerable.Empty<ItemReference>();
    private IPromise _secondaryMetadata;
    private Dictionary<string, ItemReference> _sql;
    private Dictionary<string, ItemReference> _systemIdentities;

    public IEnumerable<ItemReference> CoreMethods
    {
      get { return _methods.Where(m => m.IsCore); }
    }
    public IEnumerable<ItemType> ItemTypes
    {
      get { return _itemTypesByName.Values; }
    }
    public IEnumerable<string> MethodNames
    {
      get { return _methods.Select(i => i.KeyedName); }
    }
    public IEnumerable<ItemReference> PolyItemLists
    {
      get { return _polyItemLists; }
    }
    public IEnumerable<ItemReference> SystemIdentities
    {
      get { return _systemIdentities.Values; }
    }

    public ItemReference GetSystemIdentity(string id)
    {
      ItemReference result;
      if (!_systemIdentities.TryGetValue(id, out result)) result = null;
      return result;
    }
    public ItemType TypeById(string id)
    {
      return _itemTypesById[id];
    }
    public bool TypeById(string id, out ItemType type)
    {
      return _itemTypesById.TryGetValue(id, out type);
    }
    public bool ItemTypeByName(string name, out ItemType type)
    {
      return _itemTypesByName.TryGetValue(name, out type);
    }
    public bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef)
    {
      return _customProps.TryGetValue(path, out propRef);
    }
    public bool SqlRefByName(string name, out ItemReference sql)
    {
      return _sql.TryGetValue(name, out sql);
    }

    public ArasMetadataProvider(IAsyncConnection conn)
    {
      _conn = conn;
      Reset();
    }

    public void Wait()
    {
      Promises.All(_metadataComplete, _secondaryMetadata).Wait();
    }

    public void Reset()
    {
      _itemTypesByName.Clear();
      _itemTypesById = null;
      _metadataComplete = _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='is_versionable,is_dependent,implementation_type,core,name'></Item>", true, true)
        .Continue(r =>
        {
          ItemType result;

          foreach (var itemTypeData in r.Items())
          {
            result = new ItemType();
            result.Id = itemTypeData.Id();
            result.IsCore = itemTypeData.Property("core").AsBoolean(false);
            result.IsDependent = itemTypeData.Property("is_dependent").AsBoolean(false);
            result.IsFederated = itemTypeData.Property("implementation_type").Value == "federated";
            result.IsPolymorphic = itemTypeData.Property("implementation_type").Value == "polymorphic";
            result.IsVersionable = itemTypeData.Property("is_versionable").AsBoolean(false);
            result.Name = itemTypeData.Property("name").Value;
            result.Reference = ItemReference.FromFullItem(itemTypeData, true);
            _itemTypesByName[result.Name] = result;
          }

          _itemTypesById = _itemTypesByName.Values.ToDictionary(i => i.Id);
          return _conn.ApplyAsync(@"<Item action='get' type='RelationshipType' related_expand='0' select='related_id,source_id,relationship_id,name' />", true, true);
        })
        .Continue(r =>
        {
          ItemType relType;
          ItemType source;
          ItemType related;

          foreach (var rel in r.Items())
          {
            if (rel.SourceId().Attribute("name").HasValue()
              && _itemTypesByName.TryGetValue(rel.SourceId().Attribute("name").Value, out source)
              && rel.Property("relationship_id").Attribute("name").HasValue()
              && _itemTypesByName.TryGetValue(rel.Property("relationship_id").Attribute("name").Value, out relType))
            {
              source.Relationships.Add(relType);
              relType.Source = source;
              if (rel.RelatedId().Attribute("name").HasValue()
                && _itemTypesByName.TryGetValue(rel.RelatedId().Attribute("name").Value, out related))
              {
                relType.Related = related;
              }
            }
          }

          return _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='id,name'>
                                      <id condition='in'>
                                        (select it.ID
                                        from innovator.[ITEMTYPE] it
                                        where it.ID in
                                          (select source_id
                                           from innovator.[PROPERTY] p
                                           where p.ORDER_BY is not null))
                                      </id>
                                    </Item>", true, false);
        }).Continue(r =>
        {
          ItemType result;
          foreach (var itemType in r.Items())
          {
            if (_itemTypesByName.TryGetValue(itemType.Property("name").Value, out result))
            {
              result.IsSorted = true;
            }
          }

          return _conn.ApplyAsync(@"<Item type='Property' action='get' select='source_id,item_behavior,name' related_expand='0'>
                                      <data_type>item</data_type>
                                      <data_source>
                                        <Item type='ItemType' action='get'>
                                          <is_versionable>1</is_versionable>
                                        </Item>
                                      </data_source>
                                      <item_behavior>float</item_behavior>
                                      <name condition='not in'>'config_id','id'</name>
                                    </Item>", true, false);
        })
        .Done(r =>
        {
          ItemType result;
          foreach (var floatProp in r.Items())
          {
            if (_itemTypesByName.TryGetValue(floatProp.SourceId().Attribute("name").Value.ToLowerInvariant(), out result))
            {
              result.FloatProperties.Add(floatProp.Property("name").AsString(""));
            }
          }
        });
      _secondaryMetadata = _conn.ApplyAsync(@"<Item type='Method' action='get' select='config_id,core,name'></Item>", true, false)
        .Continue(r => {
          _methods = r.Items().Select(i => {
            var method = Method.FromFullItem(i, false);
            method.KeyedName = i.Property("name").AsString("");
            method.IsCore = i.Property("core").AsBoolean(false);
            return method;
          });

          return _conn.ApplyAsync(@"<Item type='Identity' action='get' select='id,name'>
                                      <name condition='in'>'World', 'Creator', 'Owner', 'Manager', 'Innovator Admin', 'Super User'</name>
                                    </Item>", true, true);
        }).Continue(r => {
          var sysIdents =
            r.Items()
            .Select(i => {
              var itemRef = ItemReference.FromFullItem(i, false);
              itemRef.KeyedName = i.Property("name").AsString("");
              return itemRef;
            });
          _systemIdentities = sysIdents.ToDictionary(i => i.Unique);

          return _conn.ApplyAsync(@"<Item type='SQL' action='get' select='id,name'></Item>", true, false);
        }).Continue(r => {
          var sqlItems = r.Items()
            .Select(i =>
            {
              var itemRef = ItemReference.FromFullItem(i, false);
              itemRef.KeyedName = i.Property("name").AsString("");
              return itemRef;
            });
          _sql = sqlItems.ToDictionary(i => i.KeyedName.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase);

          return _conn.ApplyAsync(@"<Item type='Property' action='get' select='name,source_id(id,name)'>
                                          <id condition='in'>(SELECT p.id
                                        from innovator.PROPERTY p
                                        inner join innovator.ITEMTYPE it
                                        on p.SOURCE_ID = it.id
                                        where p.CREATED_BY_ID &lt;&gt; 'AD30A6D8D3B642F5A2AFED1A4B02BEFA'
                                        and it.CORE = 1
                                        and it.CREATED_BY_ID = 'AD30A6D8D3B642F5A2AFED1A4B02BEFA')</id>
                                        </Item>", true, false);
        }).Continue(r => {
          IReadOnlyItem itemType;
          foreach (var customProp in r.Items())
          {
            itemType = customProp.SourceItem();
            _customProps.Add(new ItemProperty()
            {
              ItemType = itemType.Property("name").Value,
              ItemTypeId = itemType.Id(),
              Property = customProp.Property("name").Value,
              PropertyId = customProp.Id()
            }, new ItemReference("Property", customProp.Id())
            {
              KeyedName = customProp.Property("name").Value
            });
          }

          return _conn.ApplyAsync(@"<Item type='List' action='get' select='id'>
                                      <id condition='in'>(select l.id
                                        from innovator.LIST l
                                        inner join innovator.PROPERTY p
                                        on l.id = p.DATA_SOURCE
                                        and p.name = 'itemtype'
                                        inner join innovator.ITEMTYPE it
                                        on it.id = p.SOURCE_ID
                                        and it.IMPLEMENTATION_TYPE = 'polymorphic')
                                      </id>
                                    </Item>", true, false);
        }).Done(r => {
          _polyItemLists = r.Items().Select(i => ItemReference.FromFullItem(i, true));
        });
    }

    public IPromise<IEnumerable<Property>> GetProperties(ItemType itemType)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return Promises.Resolved<IEnumerable<Property>>(itemType.Properties.Values);

      return _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>@0</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source\" /></Relationships></Item></AML>"
        , true, true, itemType.Name)
        .Convert(r =>
        {
          LoadProperties(itemType, r.AssertItem());
          return (IEnumerable<Property>)itemType.Properties.Values;
        }).Fail(ex => System.Diagnostics.Debug.Print("PROPLOAD: " + ex.ToString()));
    }
    public IPromise<Property> GetProperty(ItemType itemType, string name)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return LoadedProperty(itemType, name);

      return _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>@0</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source\" /></Relationships></Item></AML>"
        , true, true, itemType.Name)
        .Continue(r =>
        {
          LoadProperties(itemType, r.AssertItem());
          return LoadedProperty(itemType, name);
        });
    }

    private IPromise<Property> LoadedProperty(ItemType itemType, string name)
    {
      Property prop;
      if (itemType.Properties.TryGetValue(name, out prop))
      {
        return Promises.Resolved(prop);
      }
      else
      {
        return Promises.Rejected<Property>(new KeyNotFoundException());
      }
    }

    /// <summary>
    /// Loads the property metadata for the current type into the schema.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="itemTypeMeta">The properties.</param>
    private void LoadProperties(ItemType type, IReadOnlyItem itemTypeMeta)
    {
      var props = itemTypeMeta.Relationships("Property");
      Property newProp = null;
      foreach (var prop in props)
      {
        newProp = new Property(prop.Property("name").Value);
        newProp.Label = prop.Property("label").Value;
        newProp.SetType(prop.Property("data_type").Value);
        if (newProp.Type == PropertyType.item && prop.Property("data_source").Attribute("name").HasValue())
        {
          newProp.Restrictions.Add(prop.Property("data_source").Attribute("name").Value);
        }
        type.Properties.Add(newProp.Name, newProp);
      }
    }

    private static Dictionary<IAsyncConnection, ArasMetadataProvider> _cache
      = new Dictionary<IAsyncConnection, ArasMetadataProvider>();

    public static ArasMetadataProvider Cached(IAsyncConnection conn)
    {
      ArasMetadataProvider result;
      if (!_cache.TryGetValue(conn, out result))
      {
        result = new ArasMetadataProvider(conn);
        _cache[conn] = result;
      }
      return result;
    }
  }
}
