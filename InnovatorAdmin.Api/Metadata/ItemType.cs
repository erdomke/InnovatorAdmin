using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  /// <summary>
  /// Class with metadata describing an ItemType
  /// </summary>
  [DebuggerDisplay("{Name}")]
  public class ItemType
  {
    private readonly List<string> _floatProps = new List<string>();
    private readonly List<ItemType> _relationships = new List<ItemType>();
    private readonly Dictionary<string, Property> _properties;

    public bool HasLifeCycle { get; }

    public ClassStructure ClassStructure { get; set; }

    public IEnumerable<string> ClassPaths {
      get
      {
        if (ClassStructure == null)
          return Enumerable.Empty<string>();
        return ClassStructure.Descendants()
          .Select(n => n.Path)
          .OrderBy(p => p);
      }
    }

    /// <summary>
    /// List of the properties which reference versionable items and are set to float
    /// </summary>
    public IList<string> FloatProperties
    {
      get { return _floatProps; }
    }

    /// <summary>
    /// ID of the item type
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Whether the item type is marked as "core"
    /// </summary>
    public bool IsCore { get; }

    /// <summary>
    /// Whether the item type is marked as "dependent"
    /// </summary>
    public bool IsDependent { get; }

    /// <summary>
    /// Whether the item type is marked as "federated"
    /// </summary>
    public bool IsFederated { get; }

    /// <summary>
    /// Whether the item type is a polyitem type
    /// </summary>
    public bool IsPolymorphic { get; }

    /// <summary>
    /// Whether the item type is a relationship
    /// </summary>
    public bool IsRelationship { get; }

    /// <summary>
    /// Indicates whether the ItemType search results are automatically sorted as evideneced by
    /// having one or more properties with a defined order_by
    /// </summary>
    public bool IsSorted { get; set; }

    /// <summary>
    /// Whether the item type is versionable (automatic or manual)
    /// </summary>
    public bool IsVersionable { get; }

    /// <summary>
    /// Label of the item type
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Name of the item type
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Properties of the item type
    /// </summary>
    public IDictionary<string, Property> Properties { get { return _properties; } }

    /// <summary>
    /// <see cref="ItemReference"/> of the item type
    /// </summary>
    public ItemReference Reference { get; }

    /// <summary>
    /// List of child relationships of the item type
    /// </summary>
    public IList<ItemType> Relationships
    {
      get { return _relationships; }
    }

    public string TabLabel { get; set; }

    public string RelationshipTypeId { get; set; }

    public IEnumerable<string> States { get; set; }

    public string Description { get; }

    public string RelatedTypeName { get; set; }

    public string SourceTypeName { get; set; }

    public HashSet<string> Morphae { get; } = new HashSet<string>();

    public List<ServerEvent> ServerEvents { get; } = new List<ServerEvent>();

    public int? DefaultPageSize { get; }

    public int? MaxRecords { get; }

    public string RelationshipView { get; }

    public bool IsUiOnly => RelationshipView?.Contains("aras.innovator.TreeGridView") == true;

    public ItemType(IReadOnlyItem itemType, HashSet<string> coreIds = null, bool defaultProperties = false, Func<string, string> getName = null)
    {
      var relType = itemType.Property("relationship_id").AsItem();
      var sourceProp = itemType.SourceId();
      SourceTypeName = sourceProp.Attribute("name").Value ?? sourceProp.KeyedName().Value;
      var relatedProp = itemType.RelatedId();
      RelatedTypeName = relatedProp.Attribute("name").Value ?? relatedProp.KeyedName().Value;
      if (relType.Exists)
      {
        RelationshipTypeId = itemType.Id();
        TabLabel = itemType.Property("label").Value;
        RelationshipView = itemType.Relationships("Relationship View")
          .FirstOrNullItem(i => i.Property("start_page").HasValue())
          .Property("start_page").Value;
        itemType = relType;
      }

      Id = itemType.Id();
      IsCore = itemType.Property("core").AsBoolean(coreIds?.Contains(itemType.Id()) == true);
      IsDependent = itemType.Property("is_dependent").AsBoolean(false);
      IsFederated = itemType.Property("implementation_type").Value == "federated";
      IsRelationship = itemType.Property("is_relationship").AsBoolean(false); 
      IsPolymorphic = itemType.Property("implementation_type").Value == "polymorphic";
      IsVersionable = itemType.Property("is_versionable").AsBoolean(false);
      Label = itemType.Property("label").Value;
      Name = itemType.Property("name").Value
        ?? itemType.KeyedName().Value
        ?? itemType.IdProp().KeyedName().Value;
      Reference = ItemReference.FromFullItem(itemType, true);
      Description = itemType.Property("description").Value;
      if (itemType.Property("class_structure").HasValue())
        ClassStructure = new ClassStructure(itemType.Property("class_structure").Value);
      DefaultPageSize = itemType.Property("default_page_size").AsInt();
      MaxRecords = itemType.Property("maxrecords").AsInt();

      HasLifeCycle = itemType.Relationships("ItemType Life Cycle").Any();

      _properties = itemType.Relationships("Property")
        .Select(p => Property.FromItem(p, this, getName))
        .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

      if (_properties.Count > 0 || defaultProperties)
      {
        if (!_properties.ContainsKey("id"))
        {
          foreach (var property in ElementFactory.Local.FromXml(string.Format(_coreProperties, Id))
            .Items()
            .Select(p => Property.FromItem(p, this))
            .Where(p => !_properties.ContainsKey(p.Name)))
          {
            _properties[property.Name] = property;
          }
        }

        var propAml = @"<Item type='Property'>
  <column_alignment>left</column_alignment>
  <data_source keyed_name='{0}' type='ItemType' name='{0}'>{1}</data_source>
  <data_type>item</data_type>
  <is_hidden>{3}</is_hidden>
  <is_hidden2>1</is_hidden2>
  <is_indexed>1</is_indexed>
  <is_keyed>0</is_keyed>
  <is_multi_valued>0</is_multi_valued>
  <is_required>0</is_required>
  <item_behavior>float</item_behavior>
  <readonly>0</readonly>
  <sort_order>2944</sort_order>
  <name>{2}</name>
</Item>";
        if (sourceProp.Exists && !_properties.ContainsKey("source_id"))
        {
          _properties["source_id"] = Property.FromItem(ElementFactory.Local.FromXml(string.Format(propAml
            , SourceTypeName
            , sourceProp.Value
            , "source_id"
            , "1")).AssertItem(), this);
        }

        if (relatedProp.Exists && !_properties.ContainsKey("related_id"))
        {
          _properties["related_id"] = Property.FromItem(ElementFactory.Local.FromXml(string.Format(propAml
            , RelatedTypeName
            , relatedProp.Value
            , "related_id"
            , "0")).AssertItem(), this);
        }
      }
      WithExtra(itemType);
    }

    public ItemType WithScripts(IReadOnlyItem itemType)
    {
      foreach (var prop in itemType.Relationships("Property")
        .Select(p => Property.FromItem(p, this)))
        _properties[prop.Name] = prop;
      WithExtra(itemType);
      return this;
    }

    private void WithExtra(IReadOnlyItem itemType)
    {
      Morphae.UnionWith(itemType.Relationships("Morphae")
        .Select(r => r.RelatedId().Attribute("name").Value ?? r.RelatedId().KeyedName().Value));
      ServerEvents.AddRange(itemType.Relationships("Server Event").Select(i => new ServerEvent(i)));
    }

    /// <summary>
    /// Whether the given object is equal to this one
    /// </summary>
    public override bool Equals(object obj)
    {
      var it = obj as ItemType;
      if (it == null) return false;
      return Equals(it);
    }

    /// <summary>
    /// Whether the given object is equal to this one
    /// </summary>
    public bool Equals(ItemType obj)
    {
      return (this.Id ?? "").Equals(obj.Id);
    }

    /// <summary>
    /// Gets the hash code for the item
    /// </summary>
    public override int GetHashCode()
    {
      return (this.Id ?? "").GetHashCode();
    }

    private static string _coreProperties = @"<AML>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>string</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>classification</keyed_name>
    <label xml:lang='en'>Classification</label>
    <readonly>0</readonly>
    <sort_order>128</sort_order>
    <stored_length>512</stored_length>
    <name>classification</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source type='ItemType'>{0}</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>1</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>config_id</keyed_name>
    <readonly>1</readonly>
    <sort_order>2688</sort_order>
    <name>config_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='User' type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>1</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>created_by_id</keyed_name>
    <readonly>1</readonly>
    <sort_order>640</sort_order>
    <name>created_by_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>date</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>1</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>created_on</keyed_name>
    <pattern>short_date_time</pattern>
    <readonly>1</readonly>
    <sort_order>512</sort_order>
    <name>created_on</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>text</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>css</keyed_name>
    <readonly>0</readonly>
    <sort_order>2304</sort_order>
    <name>css</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='Life Cycle State' type='ItemType' name='Life Cycle State'>5EFB53D35BAE468B851CD388BEA46B30</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>current_state</keyed_name>
    <readonly>1</readonly>
    <sort_order>1280</sort_order>
    <name>current_state</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>integer</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>generation</keyed_name>
    <readonly>1</readonly>
    <sort_order>2432</sort_order>
    <name>generation</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source type='ItemType'>{0}</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>1</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>id</keyed_name>
    <readonly>1</readonly>
    <sort_order>384</sort_order>
    <name>id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>boolean</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>is_current</keyed_name>
    <readonly>1</readonly>
    <sort_order>1664</sort_order>
    <name>is_current</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>boolean</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>is_released</keyed_name>
    <label xml:lang='en'>Released</label>
    <readonly>1</readonly>
    <sort_order>2048</sort_order>
    <name>is_released</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>string</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>1</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>keyed_name</keyed_name>
    <readonly>0</readonly>
    <sort_order>256</sort_order>
    <stored_length>128</stored_length>
    <name>keyed_name</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='User' type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>locked_by_id</keyed_name>
    <readonly>1</readonly>
    <sort_order>1536</sort_order>
    <name>locked_by_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>string</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>major_rev</keyed_name>
    <readonly>0</readonly>
    <sort_order>1792</sort_order>
    <stored_length>8</stored_length>
    <name>major_rev</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='Identity' type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>managed_by_id</keyed_name>
    <readonly>0</readonly>
    <sort_order>896</sort_order>
    <name>managed_by_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>string</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>minor_rev</keyed_name>
    <readonly>0</readonly>
    <sort_order>1920</sort_order>
    <stored_length>8</stored_length>
    <name>minor_rev</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='User' type='ItemType' name='User'>45E899CD2859442982EB22BB2DF683E5</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>modified_by_id</keyed_name>
    <readonly>1</readonly>
    <sort_order>1152</sort_order>
    <name>modified_by_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>date</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>modified_on</keyed_name>
    <pattern>short_date_time</pattern>
    <readonly>1</readonly>
    <sort_order>1024</sort_order>
    <name>modified_on</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>boolean</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>new_version</keyed_name>
    <readonly>1</readonly>
    <sort_order>2560</sort_order>
    <name>new_version</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>boolean</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>not_lockable</keyed_name>
    <label xml:lang='en'>Not Lockable</label>
    <readonly>1</readonly>
    <sort_order>2176</sort_order>
    <name>not_lockable</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='Identity' type='ItemType' name='Identity'>E582AB17663F4EF28460015B2BE9E094</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>owned_by_id</keyed_name>
    <readonly>0</readonly>
    <sort_order>768</sort_order>
    <name>owned_by_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='Permission' type='ItemType' name='Permission'>C6A89FDE1294451497801DF78341B473</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>1</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>permission_id</keyed_name>
    <readonly>1</readonly>
    <sort_order>2816</sort_order>
    <name>permission_id</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_type>string</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>state</keyed_name>
    <readonly>1</readonly>
    <sort_order>1408</sort_order>
    <stored_length>32</stored_length>
    <name>state</name>
  </Item>
  <Item type='Property'>
    <column_alignment>left</column_alignment>
    <data_source keyed_name='Team' type='ItemType' name='Team'>CC23F9130F574E7D99DF9659F27590A6</data_source>
    <data_type>item</data_type>
    <is_hidden>1</is_hidden>
    <is_hidden2>1</is_hidden2>
    <is_indexed>0</is_indexed>
    <is_keyed>0</is_keyed>
    <is_multi_valued>0</is_multi_valued>
    <is_required>0</is_required>
    <item_behavior>float</item_behavior>
    <keyed_name>team_id</keyed_name>
    <label xml:lang='en'>Team</label>
    <readonly>0</readonly>
    <sort_order>2944</sort_order>
    <name>team_id</name>
  </Item>
</AML>";
  }
}
