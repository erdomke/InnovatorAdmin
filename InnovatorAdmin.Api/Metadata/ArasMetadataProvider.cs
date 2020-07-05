using Innovator.Client;
using Innovator.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class ArasMetadataProvider : IArasMetadataProvider
  {
    private IAsyncConnection _conn;
    private Dictionary<ItemProperty, ItemReference> _customProps
      = new Dictionary<ItemProperty, ItemReference>();
    private Dictionary<string, ItemType> _itemTypesByName
      = new Dictionary<string, ItemType>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ItemType> _itemTypesById;
    private Task<bool[]> _metadataComplete;
    private IEnumerable<Method> _methods = Enumerable.Empty<Method>();
    private Dictionary<string, Sql> _sql;
    private Dictionary<string, ItemReference> _systemIdentities;
    private Dictionary<string, IEnumerable<ListValue>> _listValues
      = new Dictionary<string, IEnumerable<ListValue>>();
    private Dictionary<string, IEnumerable<IListValue>> _serverReports
      = new Dictionary<string, IEnumerable<IListValue>>();
    private readonly Dictionary<string, IEnumerable<IListValue>> _serverActions
      = new Dictionary<string, IEnumerable<IListValue>>();

    /// <summary>
    /// Enumerable of methods where core = 1
    /// </summary>
    public IEnumerable<ItemReference> CoreMethods
    {
      get { return _methods.Where(m => m.IsCore); }
    }
    /// <summary>
    /// Enumerable of methods where core = 1
    /// </summary>
    public IEnumerable<Method> AllMethods
    {
      get { return _methods; }
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
    public IEnumerable<ItemReference> PolyItemLists { get; private set; } = Enumerable.Empty<ItemReference>();
    /// <summary>
    /// Enumerable of all sequences
    /// </summary>
    public IEnumerable<ItemReference> Sequences
    {
      get { return Sequences1; }
    }
    /// <summary>
    /// Enumerable of all system identities
    /// </summary>
    public IEnumerable<ItemReference> SystemIdentities
    {
      get { return _systemIdentities.Values; }
    }

    /// <summary>
    /// Hashset of all CMF-generated ItemType IDs
    /// </summary>
    public HashSet<string> CmfGeneratedTypes { get; private set; } = new HashSet<string>();

    /// <summary>
    /// Gets the IDs of all the TOC presentation configs.
    /// </summary>
    public HashSet<string> TocPresentationConfigs { get; } = new HashSet<string>();

    /// <summary>
    /// Dictionary of all ItemTypes linked from contentTypes
    /// </summary>
    public Dictionary<string, ItemReference> CmfLinkedTypes { get; private set; } = new Dictionary<string, ItemReference>();

    public IEnumerable<ItemReference> Sequences1 { get; set; } = Enumerable.Empty<ItemReference>();

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
    public ItemType ItemTypeById(string id)
    {
      return _itemTypesById[id];
    }
    /// <summary>
    /// Try to get an Item Type by ID
    /// </summary>
    public bool ItemTypeById(string id, out ItemType type)
    {
      return _itemTypesById.TryGetValue(id, out type);
    }
    /// <summary>
    /// Get an Item Type by name
    /// </summary>
    public ItemType ItemTypeByName(string name)
    {
      return _itemTypesByName[name];
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
      Sql buffer;
      sql = null;
      if (_sql.TryGetValue(name, out buffer))
      {
        sql = buffer;
        return true;
      }
      return false;
    }
    public IEnumerable<Sql> Sqls()
    {
      return _sql.Values;
    }

    public IPromise<IEnumerable<ListValue>> ListValues(string id)
    {
      IEnumerable<ListValue> result;
      if (_listValues.TryGetValue(id, out result))
        return Promises.Resolved(result);

      return _conn.ApplyAsync("<Item type='List' action='get' id='@0' select='id'><Relationships><Item type='Value' action='get' select='label,value' /><Item type='Filter Value' action='get' select='label,value' /></Relationships></Item>"
        , true, false, id)
        .Convert(r =>
        {
          var values = (IEnumerable<ListValue>)r.AssertItem().Relationships()
            .Select(i => new ListValue()
            {
              Label = i.Property("label").Value,
              Value = i.Property("value").Value
            }).ToArray();
          _listValues[id] = values;
          return values;
        });
    }

    public async Task<IEnumerable<string>> ItemTypeStates(ItemType itemtype)
    {
      if (itemtype.States != null)
        return itemtype.States;

      var result = await _conn.ApplyAsync(@"<Item type='ItemType Life Cycle' action='get' select='related_id'>
  <related_id>
    <Item type='Life Cycle Map' action='get' select='id'>
      <Relationships>
        <Item action='get' type='Life Cycle State' select='name'>
        </Item>
      </Relationships>
    </Item>
  </related_id>
  <source_id>@0</source_id>
</Item>", true, false, itemtype.Id);
      var states = result.Items()
        .SelectMany(i => i.RelatedItem().Relationships().OfType<LifeCycleState>())
        .Select(i => i.NameProp().Value).ToArray();
      itemtype.States = states;
      return states;
    }

    public IEnumerable<IListValue> ServerReports(string typeName)
    {
      IEnumerable<IListValue> result;
      if (_serverReports.TryGetValue(typeName, out result))
        return result;

      var items = _conn.Apply(@"<Item type='Item Report' action='get' select='related_id(name,label)'>
  <related_id>
    <Item type='Report' action='get'>
      <location>server</location>
      <type>item</type>
    </Item>
  </related_id>
  <source_id>
    <Item type='ItemType' action='get'>
      <name>@0</name>
    </Item>
  </source_id>
</Item>", typeName).Items();
      result = items.Select(r => new ListValue()
      {
        Label = r.RelatedItem().Property("label").Value,
        Value = r.RelatedItem().Property("name").Value
      }).ToArray();
      _serverReports[typeName] = result;
      return result;
    }

    public IEnumerable<IListValue> ServerItemActions(string typeName)
    {
      IEnumerable<IListValue> result;
      if (_serverActions.TryGetValue(typeName, out result))
        return result;

      var items = _conn.Apply(@"<Item type='Item Action' action='get' select='related_id(label,method(name))'>
  <related_id>
    <Item type='Action' action='get'>
      <location>server</location>
      <type>item</type>
    </Item>
  </related_id>
  <source_id>
    <Item type='ItemType' action='get'>
      <name>@0</name>
    </Item>
  </source_id>
</Item>", typeName).Items();
      result = items.Select(r => new ListValue()
      {
        Label = r.RelatedItem().Property("label").Value,
        Value = r.RelatedItem().Property("method").AsItem().Property("name").Value
      }).ToArray();
      _serverActions[typeName] = result;
      return result;
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
    public Task ReloadTask()
    {
      return _metadataComplete;
    }
    public IPromise ReloadPromise()
    {
      return _metadataComplete.ToPromise();
    }

    /// <summary>
    /// Clear all the metadata and stard asynchronously reloading it.
    /// </summary>
    public void Reset()
    {
      if (_metadataComplete == null || _metadataComplete.IsCompleted)
        _metadataComplete = Task.WhenAll(ReloadItemTypeMetadata(), ReloadSecondaryMetadata());
    }

    private async Task<bool> ReloadItemTypeMetadata()
    {
      var itemTypes = _conn.ApplyAsync("<Item type='ItemType' action='get' select='is_versionable,is_dependent,implementation_type,core,name,label'></Item>", true, true).ToTask();
      var relTypes = _conn.ApplyAsync("<Item action='get' type='RelationshipType' related_expand='0' select='related_id,source_id,relationship_id,name,label' />", true, true).ToTask();
      var sortedProperties = _conn.ApplyAsync(@"<Item type='Property' action='get' select='source_id'>
  <order_by condition='is not null'></order_by>
</Item>", true, false).ToTask();
      var floatProps = _conn.ApplyAsync(@"<Item type='Property' action='get' select='source_id,item_behavior,name' related_expand='0'>
                                      <data_type>item</data_type>
                                      <data_source>
                                        <Item type='ItemType' action='get'>
                                          <is_versionable>1</is_versionable>
                                        </Item>
                                      </data_source>
                                      <item_behavior>float</item_behavior>
                                      <name condition='not in'>'config_id','id'</name>
                                    </Item>", true, false).ToTask();

      // Load in the item types
      var r = await itemTypes;
      ItemType result;

      foreach (var itemTypeData in r.Items())
      {
        result = new ItemType()
        {
          Id = itemTypeData.Id(),
          IsCore = itemTypeData.Property("core").AsBoolean(false),
          IsDependent = itemTypeData.Property("is_dependent").AsBoolean(false),
          IsFederated = itemTypeData.Property("implementation_type").Value == "federated",
          IsPolymorphic = itemTypeData.Property("implementation_type").Value == "polymorphic",
          IsVersionable = itemTypeData.Property("is_versionable").AsBoolean(false),
          Label = itemTypeData.Property("label").Value,
          Name = itemTypeData.Property("name").Value,
          Reference = ItemReference.FromFullItem(itemTypeData, true)
        };
        _itemTypesByName[result.Name] = result;
      }

      _itemTypesById = _itemTypesByName.Values.ToDictionary(i => i.Id);

      // Load in the relationship types
      r = await relTypes;
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
          relType.TabLabel = rel.Property("label").AsString(null);
          if (rel.RelatedId().Attribute("name").HasValue()
            && _itemTypesByName.TryGetValue(rel.RelatedId().Attribute("name").Value, out related))
          {
            relType.Related = related;
          }
        }
      }

      // Sorted Types
      r = await sortedProperties;
      foreach (var prop in r.Items())
      {
        if (_itemTypesByName.TryGetValue(prop.SourceId().Attribute("name").Value, out result))
        {
          result.IsSorted = true;
        }
      }

      // Float props
      r = await floatProps;
      foreach (var floatProp in r.Items())
      {
        if (_itemTypesByName.TryGetValue(floatProp.SourceId().Attribute("name").Value.ToLowerInvariant(), out result))
        {
          result.FloatProperties.Add(floatProp.Property("name").AsString(""));
        }
      }

      return true;
    }

    private async Task<bool> ReloadSecondaryMetadata()
    {
      var methods = _conn.ApplyAsync("<Item type='Method' action='get' select='config_id,core,name,method_code,comments'></Item>", true, false).ToTask();
      var sysIdents = _conn.ApplyAsync(@"<Item type='Identity' action='get' select='id,name'>
                                      <name condition='in'>'World', 'Creator', 'Owner', 'Manager', 'Innovator Admin', 'Super User'</name>
                                    </Item>", true, true).ToTask();
      var sqls = _conn.ApplyAsync("<Item type='SQL' action='get' select='id,name,type'></Item>", true, false).ToTask();
      var customProps = _conn.ApplyAsync(@"<Item type='Property' action='get' select='name,source_id(id,name)'>
                                          <created_by_id condition='ne'>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
                                          <source_id>
                                            <Item type='ItemType' action='get'>
                                              <core>1</core>
                                              <created_by_id>AD30A6D8D3B642F5A2AFED1A4B02BEFA</created_by_id>
                                            </Item>
                                          </source_id>
                                        </Item>", true, false).ToTask();
      var polyLists = _conn.ApplyAsync(@"<Item type='Property' action='get' select='data_source(id)'>
                                          <name>itemtype</name>
                                          <data_type>list</data_type>
                                          <source_id>
                                            <Item type='ItemType' action='get'>
                                              <implementation_type>polymorphic</implementation_type>
                                            </Item>
                                          </source_id>
                                        </Item>", true, false).ToTask();
      var sequences = _conn.ApplyAsync(@"<Item type='Sequence' action='get' select='name'></Item>", true, false).ToTask();
      var elementTypes = _conn.ApplyAsync(@"<Item action='get' type='cmf_ElementType' select='generated_type'>
                                                <Relationships>
                                                  <Item action='get' type='cmf_PropertyType' select='generated_type'>
                                                  </Item>
                                                </Relationships>
                                              </Item>", true, false).ToTask();
      var contentTypes = _conn.ApplyAsync(@"<Item action='get' type='cmf_ContentType' select='linked_item_type'>
                                              <linked_item_type>
                                                <Item type='ItemType' action='get'>
                                                </Item>
                                              </linked_item_type>
                                            </Item>", true, false).ToTask();
      var presentationConfigs = _conn.ApplyAsync(@"<Item type='ITPresentationConfiguration' action='get' select='id,related_id'>
  <related_id>
    <Item type='PresentationConfiguration' action='get' select='id'>
      <name condition='like'>*_TOC_Configuration</name>
      <color condition='is null'></color>
      <Relationships>
        <Item action='get' type='cui_PresentConfigWinSection' select='id' />
        <Item action='get' type='PresentationCommandBarSection' select='id,related_id(id)' />
      </Relationships>
    </Item>
  </related_id>
</Item>", true, false).ToTask();

      _methods = (await methods).Items().Select(i => new Method(i)).ToList();

      _systemIdentities = (await sysIdents).Items()
        .Select(i =>
        {
          var itemRef = ItemReference.FromFullItem(i, false);
          itemRef.KeyedName = i.Property("name").AsString("");
          return itemRef;
        }).ToDictionary(i => i.Unique);


      _sql = (await sqls).Items()
        .Select(i =>
        {
          var itemRef = Sql.FromFullItem(i, false);
          itemRef.KeyedName = i.Property("name").AsString("");
          itemRef.Type = i.Property("type").AsString("");
          return itemRef;
        }).ToDictionary(i => i.KeyedName.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase);

      var r = (await customProps);
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

      PolyItemLists = (await polyLists).Items()
        .OfType<Innovator.Client.Model.Property>()
        .Select(i => new ItemReference("List", i.DataSource().Value)
        {
          KeyedName = i.DataSource().KeyedName().Value
        }).ToArray();

      Sequences1 = (await sequences).Items().Select(i => ItemReference.FromFullItem(i, true)).ToArray();

      try
      {
        CmfGeneratedTypes = new HashSet<string>((await elementTypes).Items().SelectMany(x =>
        {
          var relations = x.Relationships().Select(y => y.Property("generated_type").Value).ToList();
          relations.Add(x.Property("generated_type").Value);
          return relations;
        }));

        CmfLinkedTypes = (await contentTypes).Items().ToDictionary(x => x.Property("linked_item_type").Value, y => ItemReference.FromFullItem(y, true));
      }
      catch (ServerException)
      {
        //TODO: Do something when cmf types don't exist
      }

      try
      {
        TocPresentationConfigs.Clear();
        TocPresentationConfigs.UnionWith((await presentationConfigs)
          .Items()
          .Select(i => i.RelatedItem())
          .Where(i => i.Relationships().Count() == 1
            && i.Relationships().Single().RelatedId().KeyedName().AsString("").EndsWith("_TOC_Content"))
          .Select(i => i.Id()));
      }
      catch (ServerException)
      {
        //TODO: Do something
      }

      return true;
    }

    public IPromise<IEnumerable<Property>> GetPropertiesByTypeId(string id)
    {
      ItemType itemType;
      if (!_itemTypesById.TryGetValue(id, out itemType))
        return Promises.Rejected<IEnumerable<Property>>(new KeyNotFoundException());
      return GetProperties(itemType);
    }

    public IPromise<IEnumerable<string>> GetClassPaths(ItemType itemType)
    {
      if (_conn == null || itemType.ClassPaths != null)
        return Promises.Resolved(itemType.ClassPaths);

      return _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" id=\"@0\" select='class_structure'></Item></AML>"
        , true, true, itemType.Id)
        .Convert(r =>
        {
          var structure = r.AssertItem().Property("class_structure").Value;
          if (string.IsNullOrEmpty(structure))
          {
            itemType.ClassPaths = Enumerable.Empty<string>();
          }
          else
          {
            try
            {
              itemType.ClassPaths = ParseClassStructure(new System.IO.StringReader(structure)).ToArray();
            }
            catch (XmlException)
            {
              itemType.ClassPaths = Enumerable.Empty<string>();
            }
          }
          return itemType.ClassPaths;
        });
    }

    private IEnumerable<string> ParseClassStructure(System.IO.TextReader structure)
    {
      var path = new List<string>();
      var returned = 0;

      using (var reader = XmlReader.Create(structure))
      {
        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "class")
          {
            if (reader.IsEmptyElement)
            {
              returned = path.Count;
              yield return path.Concat(Enumerable.Repeat(reader.GetAttribute("name"), 1)).GroupConcat("/");
            }
            else
            {
              var name = reader.GetAttribute("name");
              if (!string.IsNullOrEmpty(name))
                path.Add(name);
            }
          }
          else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "class")
          {
            if (returned < path.Count)
              yield return path.GroupConcat("/");

            if (path.Count > 0)
              path.RemoveAt(path.Count - 1);
            returned = path.Count;
          }
        }
      }
    }

    /// <summary>
    /// Gets a promise to return information about all properties of a given Item Type
    /// </summary>
    public IPromise<IEnumerable<Property>> GetProperties(ItemType itemType)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return Promises.Resolved<IEnumerable<Property>>(itemType.Properties.Values);

      var xPropQuery = default(string);
      if (_itemTypesById.ContainsKey("A253DB1415194344AEFB5E6A2029AB3A"))
      {
        xPropQuery = @"<Item type='ItemType_xPropertyDefinition' action='get' select='related_id(name,label,data_type,data_source,stored_length,prec,scale,default_value,column_width,is_required,readonly)'></Item>";
      }

      var aml = $@"<AML>
  <Item action='get' type='ItemType' select='name'>
    <name>@0</name>
    <Relationships>
      <Item action='get' type='Property' select='name,label,data_type,data_source,stored_length,prec,scale,foreign_property(name,source_id),is_hidden,is_hidden2,sort_order,default_value,column_width,is_required,readonly' />
      {xPropQuery}
    </Relationships>
  </Item>
</AML>";

      var promise = _conn.ApplyAsync(aml, true, true, itemType.Name)
        .Convert(r =>
        {
          LoadProperties(itemType, r.AssertItem());
          return (IEnumerable<Property>)itemType.Properties.Values;
        }).Fail(ex => System.Diagnostics.Debug.Print("PROPLOAD: " + ex.ToString()));

      if (!string.IsNullOrEmpty(xPropQuery))
      {
        promise = promise.Continue(p =>
        {
          return _conn.ApplyAsync(@"<Item type='xClassificationTree' action='get' select='id'>
  <Relationships>
    <Item type='xClassificationTree_ItemType' action='get' select='id'>
      <related_id>@0</related_id>
    </Item>
    <Item action='get' type='xClass' select='id'>
      <Relationships>
        <Item action='get' type='xClass_xPropertyDefinition' select='related_id(name,label,data_type,data_source,stored_length,prec,scale,default_value,column_width,is_required,readonly)'>
          <is_current>1</is_current>
        </Item>
      </Relationships>
    </Item>
  </Relationships>
</Item>", true, false, itemType.Id);
        }).Convert(r =>
        {
          foreach (var prop in r.Items()
            .SelectMany(i => i.Relationships("xClass"))
            .SelectMany(i => i.Relationships("xClass_xPropertyDefinition"))
            .Select(i => i.RelatedItem()))
          {
            var newProp = Property.FromItem(prop, itemType);
            itemType.Properties[newProp.Name] = newProp;
          }
          return (IEnumerable<Property>)itemType.Properties.Values;
        }).Fail(ex => System.Diagnostics.Debug.Print("PROPLOAD: " + ex.ToString()));
      }

      return promise;
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
      var props = itemTypeMeta.Relationships("Property")
        .Concat(itemTypeMeta.Relationships("ItemType_xPropertyDefinition").Select(i => i.RelatedItem()));
      foreach (var prop in props)
      {
        var newProp = Property.FromItem(prop, type);
        type.Properties[newProp.Name] = newProp;
      }
    }

    private static Dictionary<string, ArasMetadataProvider> _cache
      = new Dictionary<string, ArasMetadataProvider>();

    /// <summary>
    /// Return a cached metadata object for a given connection
    /// </summary>
    public static ArasMetadataProvider Cached(IAsyncConnection conn)
    {
      ArasMetadataProvider result;
      var key = conn.Database + "|" + conn.UserId;
      var remote = conn as IRemoteConnection;
      if (remote != null)
        key += "|" + remote.Url;
      if (!_cache.TryGetValue(key, out result) || string.IsNullOrEmpty(result._conn.UserId))
      {
        result = new ArasMetadataProvider(conn);
        _cache[key] = result;
      }
      return result;
    }
  }
}
