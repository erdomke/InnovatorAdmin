using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  internal class DependencyAnalyzer
  {
    private const string PreAnalyzed = "_dependencies_analyzed";

    private Dictionary<ItemReference, ItemReference> _allDefinitions = new Dictionary<ItemReference,ItemReference>();
    private Dictionary<ItemReference, References> _allDependencies = new Dictionary<ItemReference, References>();
    private Dictionary<ItemReference, HashSet<ItemReference>> _allItemDependencies = new Dictionary<ItemReference, HashSet<ItemReference>>();

    private Dictionary<ItemProperty, ItemReference> _customProps;
    private HashSet<ItemReference> _definitions = new HashSet<ItemReference>();
    private HashSet<ItemReference> _dependencies = new HashSet<ItemReference>();
    private XmlElement _elem;
    
    private HashSet<ItemReference> _coreMethods;
    private Dictionary<string, ItemReference> _systemIdentities;
    private Dictionary<string, ItemType> _itemTypes;
    private Dictionary<string, ItemReference> _sql;

    private SqlParser _parser = new SqlParser();

    public ItemReference GetSystemIdentity(string id)
    {
      ItemReference result;
      if (!_systemIdentities.TryGetValue(id, out result)) result = null;
      return result;
    }

    public DependencyAnalyzer(Connection _conn, Dictionary<string, ItemType> itemTypes)
    {
      _parser.SchemaToKeep = "innovator";

      _coreMethods = new HashSet<ItemReference>();
      ItemReference itemRef;
      var methodItems = _conn.GetItems("ApplyItem", Properties.Resources.GetCoreMethods);
      foreach (var methodItem in methodItems)
      {
        itemRef = new ItemReference("Method", "[Method].[config_id] = '" + methodItem.Element("config_id", "") + "'");
        itemRef.KeyedName = methodItem.Element("id").Attribute("keyed_name", "");
        _coreMethods.Add(itemRef);
      }

      _systemIdentities = new Dictionary<string, ItemReference>();
      var sysIdents = _conn.GetItems("ApplyItem", Properties.Resources.SystemIdentities);
      foreach (var sysIdent in sysIdents)
      {
        itemRef = ItemReference.FromFullItem(sysIdent, false);
        itemRef.KeyedName = sysIdent.Element("name", "");
        _systemIdentities.Add(itemRef.Unique, itemRef);
      }

      _sql = new Dictionary<string, ItemReference>();
      var sqlItems = _conn.GetItems("ApplyItem", Properties.Resources.SqlItems);
      foreach (var sql in sqlItems)
      {
        itemRef = ItemReference.FromFullItem(sql, false);
        itemRef.KeyedName = sql.Element("name", "");
        _sql.Add(itemRef.KeyedName, itemRef);
      }

      _customProps = new Dictionary<ItemProperty, ItemReference>();
      var customPropItems = _conn.GetItems("ApplyItem", Properties.Resources.CustomUserProperties);
      XmlElement itemType;
      foreach (var customProp in customPropItems)
      {
        itemType = customProp.Elements("source_id").Element("Item");
        _customProps.Add(new ItemProperty()
        {
          ItemType = itemType.Element("name").InnerText,
          ItemTypeId = itemType.Element("id").InnerText,
          Property = customProp.Element("name").InnerText,
          PropertyId = customProp.Element("id").InnerText
        }, new ItemReference("Property", customProp.Element("id").InnerText)
        {
          KeyedName = customProp.Element("name").InnerText
        });
      }

      _itemTypes = itemTypes;
    }

    public void Reset()
    {
      _allDefinitions.Clear();
      _allDependencies.Clear();
      _allItemDependencies.Clear();
    }

    /// <summary>
    /// Removes depencencies
    /// </summary>
    /// <returns>Nodes which are top-level nodes</returns>
    public IEnumerable<XmlNode> RemoveDependencyContexts(ItemReference dependency)
    {
      References refs;
      if (_allDependencies.TryGetValue(dependency, out refs))
      {
        foreach (var node in refs.GetContexts())
        {
          if (node.ParentNode == null || node.Attribute(PreAnalyzed) == "1")
          {
            yield return node;
          }
          else
          {
            node.Detatch();
          }
        }
      }
    }

    public IEnumerable<XmlNode> GetReferences(ItemReference masterRef, ItemReference dependency)
    {
      References refs;
      if (_allDependencies.TryGetValue(dependency, out refs))
      {
        return refs.GetReferencesByMaster(masterRef);
      }
      else
      {
        return Enumerable.Empty<XmlNode>();
      }
    }

    /// <summary>
    /// Removes depencencies
    /// </summary>
    /// <returns>Nodes which are top-level nodes</returns>
    public IEnumerable<XmlNode> RemoveDependencyReferences(ItemReference dependency)
    {
      References refs;
      if (_allDependencies.TryGetValue(dependency, out refs))
      {
        foreach (var node in refs.GetReferences())
        {
          if (node.ParentNode == null || node.Attribute(PreAnalyzed) == "1")
          {
            yield return node;
          }
          else
          {
            node.Detatch();
          }
        }
      }
    }

    public IEnumerable<ItemReference> GetDependencies(ItemReference itemRef)
    {
      HashSet<ItemReference> dependencies;
      if (_allItemDependencies.TryGetValue(itemRef, out dependencies))
      {
        return dependencies;
      }
      return Enumerable.Empty<ItemReference>();
    }

    public void GatherDependencies(XmlElement elem, ItemReference itemRef, IEnumerable<ItemReference> existingDependencies)
    {
      _dependencies.Clear();
      _definitions.Clear();

      if (existingDependencies != null) _dependencies.UnionWith(existingDependencies);

      _elem = elem;
      VisitNode(elem, itemRef);

      // Clean up dependencies
      foreach (var defn in _definitions)
      {
        _dependencies.Remove(defn);
        _allDefinitions.Add(defn, itemRef);
      }

      foreach (var depend in _dependencies)
      {
        depend.Origin = itemRef;
      }

      _dependencies.ExceptWith(_systemIdentities.Values);
      _dependencies.ExceptWith(_coreMethods);
      _dependencies.ExceptWith(_itemTypes.Values.Where(i => i.IsCore).Select(i => i.Reference));
      _dependencies.ExceptWith(_dependencies.Where(d => 
      {
        if (d.Type == "ItemType" && !string.IsNullOrEmpty(d.KeyedName))
        {
          ItemType it;
          if (_itemTypes.TryGetValue(d.KeyedName, out it) && it.IsCore) return true;
        }
        return false;
      }).ToList());

      if (_dependencies.Any()) 
      { 
        HashSet<ItemReference> dependSet;
        if (_allItemDependencies.TryGetValue(itemRef, out dependSet))
        {
          dependSet.UnionWith(_dependencies);
        }
        else 
        {
          _allItemDependencies[itemRef] = new HashSet<ItemReference>(_dependencies);
        }
      }

      elem.SetAttribute(PreAnalyzed, "1");

      _dependencies.Clear();
      _definitions.Clear();
    }
    public void CleanDependencies()
    {
      ItemReference topDefn;
      foreach (var kvp in _allItemDependencies)
      {
        foreach (var value in kvp.Value.ToList())
        {
          if (_allDefinitions.TryGetValue(value, out topDefn))
          {
            kvp.Value.Remove(value);
            kvp.Value.Add(topDefn);
          }
        }
      }
    }

    private void VisitNode(XmlElement elem, ItemReference masterRef)
    {
      var textChildren = elem.ChildNodes.OfType<XmlText>().ToList();

      // Add a dependency to the relevant itemTypes
      if (elem.LocalName == "Item" && elem.HasAttribute("type"))
      {
        ItemType itemType;
        if ( _itemTypes.TryGetValue(elem.Attribute("type"), out itemType))
        {
          AddDependency(itemType.Reference, elem, elem, masterRef);
        }
        else
        {
          AddDependency(new ItemReference("ItemType", "[ItemType].[name] = '" + elem.Attributes["type"].Value + "'")
          {
            KeyedName = elem.Attributes["type"].Value
          }, elem, elem, masterRef);
        }
      }

      // Item property node
      if (elem.HasAttribute("type") && textChildren.Count == 1 && !string.IsNullOrEmpty(textChildren[0].Value))
      {
        AddDependency(ItemReference.FromItemProp(elem), elem.Parent(), elem, masterRef);
      }
      else if (elem.LocalName == "sqlserver_body" && elem.Parent().LocalName == "Item" && elem.Parent().Attribute("type") == "SQL")
      {
        var names = _parser.FindSqlServerObjectNames(elem.InnerText)
          .Select(n => n.StartsWith(_parser.SchemaToKeep + ".", StringComparison.OrdinalIgnoreCase) ? 
                       n.Substring(_parser.SchemaToKeep.Length+1).ToLowerInvariant() : 
                       n.ToLowerInvariant())
          .Distinct();

        ItemType itemType;
        ItemReference sql;
        foreach (var name in names)
        {
          if (_itemTypes.TryGetValue(name.Replace('_', ' '), out itemType))
          {
            AddDependency(itemType.Reference, elem.Parent(), elem, masterRef);
          }
          else if (_sql.TryGetValue(name, out sql))
          {
            AddDependency(sql, elem.Parent(), elem, masterRef);
          }
        }
      }
      else if (elem.LocalName == "data_source" && textChildren.Count == 1 && !string.IsNullOrEmpty(textChildren[0].Value))
      {
        // Property data source dependencies
        var parent = elem.ParentNode as XmlElement;
        if (parent != null && parent.LocalName == "Item" && parent.Attribute("type") == "Property")
        {
          var keyedName = elem.Attribute("keyed_name");
          var itemtype = _itemTypes.Values.FirstOrDefault(i => i.Reference.Unique == elem.Parent().Parent().Parent().Attribute("id") && i.IsPolymorphic);
          if (itemtype == null)
          {
            var dataType = parent.Element("data_type");
            if (dataType != null)
            {
              switch (dataType.InnerText.ToLowerInvariant())
              {
                case "list":
                case "mv_list":
                case "filter list":
                  AddDependency(new ItemReference("List", textChildren[0].Value) { KeyedName = keyedName }, parent, elem, masterRef);
                  break;
                case "item":
                  AddDependency(new ItemReference("ItemType", textChildren[0].Value) { KeyedName = keyedName }, parent, elem, masterRef);
                  break;
                case "sequence":
                  AddDependency(new ItemReference("Sequence", textChildren[0].Value) { KeyedName = keyedName }, parent, elem, masterRef);
                  break;
              }
            }
          }
        }
      }
      else if (elem != _elem && elem.LocalName == "Item" && elem.HasAttribute("type")
        && elem.Attribute("action", "") == "get"
        && (elem.HasAttribute("id") || elem.HasAttribute("where")))
      {
        // Item queries
        AddDependency(ItemReference.FromFullItem(elem, true), elem.Parent().Parent(), elem.Parent(), masterRef);
      }
      else
      {
        if (elem != _elem && elem.LocalName == "Item" && elem.HasAttribute("type") && elem.HasAttribute("id"))
        {
          _definitions.Add(ItemReference.FromFullItem(elem, false));
        }
        var isItem = (elem.LocalName == "Item" && elem.HasAttribute("type"));
        ItemProperty newProp;
        ItemReference propRef;

        foreach (var child in elem.Elements())
        {
          if (isItem)
          {
            newProp = new ItemProperty()
            {
              ItemType = elem.Attributes["type"].Value,
              ItemTypeId = (elem.HasAttribute("typeid") ? elem.Attributes["typeid"].Value : null),
              Property = child.LocalName
            };
            if (_customProps.TryGetValue(newProp, out propRef))
            {
              propRef = propRef.Clone();
              AddDependency(propRef, elem, child, masterRef);
            }
          }
          VisitNode(child, masterRef);
        }
      }
    }

    private void AddDependency(ItemReference itemRef, XmlNode context, XmlNode reference, ItemReference masterRef)
    {
      _dependencies.Add(itemRef);
      if (context != null)
      {
        References refs;
        if (!_allDependencies.TryGetValue(itemRef, out refs))
        {
          refs = new References();
          _allDependencies.Add(itemRef, refs);
        }
        refs.AddReferences(reference, context, masterRef);
      }
    }

    private class References
    {
      private List<ReferenceContext> _contexts = new List<ReferenceContext>();
      
      public void AddReferences(XmlNode reference, XmlNode context, ItemReference masterRef)
      {
        _contexts.Add(new ReferenceContext()
        {
          Context = context,
          Reference = reference,
          MasterRef = masterRef
        });
      }

      public IEnumerable<XmlNode> GetContexts()
      {
        return _contexts.Select(c => c.Context);
      }
      public IEnumerable<XmlNode> GetReferences()
      {
        return _contexts.Select(c => c.Reference);
      }
      public IEnumerable<XmlNode> GetReferencesByMaster(ItemReference masterRef)
      {
        return (from c in _contexts where c.MasterRef.Equals(masterRef) select c.Reference);
      }
    }

    private class ReferenceContext
    {
      public XmlNode Context { get; set; }
      public ItemReference MasterRef { get; set; }
      public XmlNode Reference { get; set; }
    }
  }
}
