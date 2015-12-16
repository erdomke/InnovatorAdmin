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

    /// <summary>
    /// Enumerable of methods where core = 1
    /// </summary>
    public IEnumerable<ItemReference> CoreMethods
    {
      get { return _methods.Where(m => m.IsCore); }
    }
    /// <summary>
    /// Enumerable of all item types
    /// </summary>
    public IEnumerable<ItemType> ItemTypes
    {
      get { return _itemTypesByName.Values; }
    }
    /// <summary>
    /// Enumerable of all method names
    /// </summary>
    public IEnumerable<string> MethodNames
    {
      get { return _methods.Select(i => i.KeyedName); }
    }
    /// <summary>
    /// Enumerable of all lists that are auto-generated for a polyitem item type
    /// </summary>
    public IEnumerable<ItemReference> PolyItemLists
    {
      get { return _polyItemLists; }
    }
    /// <summary>
    /// Enumerable of all system identities
    /// </summary>
    public IEnumerable<ItemReference> SystemIdentities
    {
      get { return _systemIdentities.Values; }
    }

    /// <summary>
    /// Gets a reference to a system identity given the ID (if the ID matches a system identity;
    /// otherwise <c>null</c>)
    /// </summary>
    /// <param name="id">ID of the identity to check</param>
    /// <returns>An <see cref="ItemReference"/> if the ID matches a system identity; otherwise
    /// <c>null</c></returns>
    public ItemReference GetSystemIdentity(string id)
    {
      ItemReference result;
      if (!_systemIdentities.TryGetValue(id, out result)) result = null;
      return result;
    }
    /// <summary>
    /// Get an Item Type by ID
    /// </summary>
    public ItemType TypeById(string id)
    {
      return _itemTypesById[id];
    }
    /// <summary>
    /// Try to get an Item Type by ID
    /// </summary>
    public bool TypeById(string id, out ItemType type)
    {
      return _itemTypesById.TryGetValue(id, out type);
    }
    /// <summary>
    /// Try to get an Item Type by name
    /// </summary>
    public bool ItemTypeByName(string name, out ItemType type)
    {
      return _itemTypesByName.TryGetValue(name, out type);
    }
    /// <summary>
    /// Try to get a custom property by the Item Type and name information
    /// </summary>
    public bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef)
    {
      return _customProps.TryGetValue(path, out propRef);
    }
    /// <summary>
    /// Try to get SQL information from a name
    /// </summary>
    public bool SqlRefByName(string name, out ItemReference sql)
    {
      return _sql.TryGetValue(name, out sql);
    }

    /// <summary>
    /// Constructor for the ArasMetadataProvider class
    /// </summary>
    private ArasMetadataProvider(IAsyncConnection conn)
    {
      _conn = conn;
      Reset();
    }

    /// <summary>
    /// Wait synchronously for the asynchronous data loads to complete
    /// </summary>
    public void Wait()
    {
      Promises.All(_metadataComplete, _secondaryMetadata).Wait();
    }

    public IPromise CompletePromise()
    {
      return Promises.All(_metadataComplete, _secondaryMetadata);
    }

    /// <summary>
    /// Clear all the metadata and stard asynchronously reloading it.
    /// </summary>
    public void Reset()
    {
      if (_metadataComplete == null || _metadataComplete.IsRejected || _metadataComplete.IsResolved)
      {
        _customProps.Clear();
        _itemTypesByName.Clear();
        _itemTypesById = null;
        _metadataComplete = _conn.ApplyAsync(@"<Item type='ItemType' action='get' select='is_versionable,is_dependent,implementation_type,core,name,label'></Item>", true, true)
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
              result.Label = itemTypeData.Property("label").Value;
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
          .Continue(r =>
          {
            _methods = r.Items().Select(i =>
            {
              var method = Method.FromFullItem(i, false);
              method.KeyedName = i.Property("name").AsString("");
              method.IsCore = i.Property("core").AsBoolean(false);
              return method;
            });

            return _conn.ApplyAsync(@"<Item type='Identity' action='get' select='id,name'>
                                      <name condition='in'>'World', 'Creator', 'Owner', 'Manager', 'Innovator Admin', 'Super User'</name>
                                    </Item>", true, true);
          }).Continue(r =>
          {
            var sysIdents =
              r.Items()
              .Select(i =>
              {
                var itemRef = ItemReference.FromFullItem(i, false);
                itemRef.KeyedName = i.Property("name").AsString("");
                return itemRef;
              });
            _systemIdentities = sysIdents.ToDictionary(i => i.Unique);

            return _conn.ApplyAsync(@"<Item type='SQL' action='get' select='id,name'></Item>", true, false);
          }).Continue(r =>
          {
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
          }).Continue(r =>
          {
            IReadOnlyItem itemType;
            foreach (var customProp in r.Items())
            {
              itemType = customProp.SourceItem();
              _customProps[new ItemProperty()
              {
                ItemType = itemType.Property("name").Value,
                ItemTypeId = itemType.Id(),
                Property = customProp.Property("name").Value,
                PropertyId = customProp.Id()
              }] = new ItemReference("Property", customProp.Id())
              {
                KeyedName = customProp.Property("name").Value
              };
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
          }).Done(r =>
          {
            _polyItemLists = r.Items().Select(i => ItemReference.FromFullItem(i, true));
          });
      }
    }

    public IPromise<IEnumerable<Property>> GetPropertiesByTypeId(string id)
    {
      ItemType itemType;
      if (!_itemTypesById.TryGetValue(id, out itemType))
        return Promises.Rejected<IEnumerable<Property>>(new KeyNotFoundException());
      return GetProperties(itemType);
    }

    /// <summary>
    /// Gets a promise to return information about all properties of a given Item Type
    /// </summary>
    public IPromise<IEnumerable<Property>> GetProperties(ItemType itemType)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return Promises.Resolved<IEnumerable<Property>>(itemType.Properties.Values);

      return _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>@0</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source,stored_length,prec,scale\" /></Relationships></Item></AML>"
        , true, true, itemType.Name)
        .Convert(r =>
        {
          LoadProperties(itemType, r.AssertItem());
          return (IEnumerable<Property>)itemType.Properties.Values;
        }).Fail(ex => System.Diagnostics.Debug.Print("PROPLOAD: " + ex.ToString()));
    }
    /// <summary>
    /// Gets a promise to return information about property of a given Item Type and name
    /// </summary>
    public IPromise<Property> GetProperty(ItemType itemType, string name)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return LoadedProperty(itemType, name);

      return GetProperties(itemType)
        .Continue(r =>
        {
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
        newProp.Precision = prop.Property("prec").AsInt(-1);
        newProp.Scale = prop.Property("scale").AsInt(-1);
        newProp.StoredLength = prop.Property("stored_length").AsInt(-1);
        if (newProp.Type == PropertyType.item && prop.Property("data_source").Attribute("name").HasValue())
        {
          newProp.Restrictions.Add(prop.Property("data_source").Attribute("name").Value);
        }
        type.Properties.Add(newProp.Name, newProp);
      }
    }

    private static Dictionary<IAsyncConnection, ArasMetadataProvider> _cache
      = new Dictionary<IAsyncConnection, ArasMetadataProvider>();

    /// <summary>
    /// Return a cached metadata object for a given connection
    /// </summary>
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
