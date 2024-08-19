using Innovator.Client;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace InnovatorAdmin
{
  /// <summary>
  /// Detect the dependencies between AML scripts
  /// </summary>
  internal class DependencyAnalyzer
  {
    private static Regex _treeGridViewUrl = new Regex(@"tgvdId=(?<id>[0-9a-fA-F]{32})");

    //// Persistent variables between scans
    // Pointer from a child reference to the master reference that defines it
    private readonly Dictionary<ItemReference, ItemReference> _allDefinitions = new Dictionary<ItemReference, ItemReference>();
    // Pointer from a child reference to the XML data and master reference where the child is referenced
    private readonly Dictionary<ItemReference, References> _allDependencies = new Dictionary<ItemReference, References>();
    // All the dependencies for a given item based on the reference
    private readonly Dictionary<ItemReference, HashSet<ItemReference>> _allItemDependencies = new Dictionary<ItemReference, HashSet<ItemReference>>();
    // Instances where a delete depends on another script
    private readonly Dictionary<ItemReference, HashSet<ItemReference>> _allDeleteDependencies = new Dictionary<ItemReference, HashSet<ItemReference>>();

    private const string ItemTypeByNameWhere = "[ItemType].[name] = '";

    //// Temporary variables only used within a set of scans
    private readonly HashSet<ItemReference> _definitions = new HashSet<ItemReference>();
    private readonly HashSet<ItemReference> _dependencies = new HashSet<ItemReference>();
    private readonly IArasMetadataProvider _metadata;
    private XmlElement _elem;
    private Action<IDependencyContext> _customDependencies;

    public DependencyAnalyzer(IArasMetadataProvider metadata, Action<IDependencyContext> customDependencies = null)
    {
      _metadata = metadata;
      _customDependencies = customDependencies;
    }

    public void Reset()
    {
      _allDefinitions.Clear();
      _allDependencies.Clear();
      _allItemDependencies.Clear();
    }

    public void Reset(IEnumerable<ItemReference> refsToKeep)
    {
      if (refsToKeep.Any())
      {
        var refsToKeepHash = new HashSet<ItemReference>(refsToKeep);

        // Clean up the definitions as necessary
        var defnsToRemove = (from p in _allDefinitions where !refsToKeepHash.Contains(p.Value) select p.Key).ToList();
        foreach (var defnToRemove in defnsToRemove)
        {
          _allDefinitions.Remove(defnToRemove);
        }

        // Clean up the dependencies as necessary
        foreach (var depend in _allDependencies)
        {
          depend.Value.RemoveAllButMasterList(refsToKeepHash);
        }

        // Remove dependency information
        var itemsToRemove = (from p in _allItemDependencies where !refsToKeepHash.Contains(p.Key) select p.Key).ToList();
        foreach (var itemToRemove in itemsToRemove)
        {
          _allItemDependencies.Remove(itemToRemove);
        }
      }
      else
      {
        this.Reset();
      }
    }

    /// <summary>
    /// Removes a dependency from the data
    /// </summary>
    /// <param name="masterRef">Parent of the dependency</param>
    /// <param name="dependency">Child of the dependency</param>
    public void RemoveDependency(ItemReference masterRef, ItemReference dependency)
    {
      HashSet<ItemReference> childDependencies;
      if (_allItemDependencies.TryGetValue(masterRef, out childDependencies))
      {
        childDependencies.Remove(dependency);
      }
      References refs;
      if (_allDependencies.TryGetValue(dependency, out refs))
      {
        refs.RemoveByMaster(masterRef);
      }
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
          if (node.ParentNode == null || node.Attribute(XmlFlags.Attr_DependenciesAnalyzed) == "1")
          {
            yield return node;
          }
          else
          {
            node.Detach();
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
          if (node.ParentNode == null || node.Attribute(XmlFlags.Attr_DependenciesAnalyzed) == "1")
          {
            yield return node;
          }
          else
          {
            node.Detach();
          }
        }
      }
    }

    /// <summary>
    /// Get the items that the <paramref name="itemRef"/> depends on
    /// </summary>
    /// <param name="itemRef">The reference to check</param>
    /// <returns>The list of dependencies</returns>
    public IEnumerable<ItemReference> GetDependencies(ItemReference itemRef)
    {
      if (_allItemDependencies.TryGetValue(itemRef, out var dependencies))
        return dependencies;
      return Enumerable.Empty<ItemReference>();
    }

    public IEnumerable<ItemReference> GetDeleteDependencies(ItemReference itemRef)
    {
      return _allDeleteDependencies.TryGetValue(itemRef, out var dependencies) ? dependencies : Enumerable.Empty<ItemReference>();
    }

    /// <summary>
    /// Get the items that depend on the <paramref name="itemRef"/>
    /// </summary>
    /// <param name="itemRef">The reference to check</param>
    /// <returns>The list of dependencies</returns>
    public IEnumerable<ItemReference> GetReverseDependencies(ItemReference itemRef)
    {
      return _allItemDependencies.Where(k => k.Value.Contains(itemRef)).Select(k => k.Key);
    }

    /// <summary>
    /// Determine the dependencies of an install item script
    /// </summary>
    public void AddReferenceAndDependencies(InstallItem installItem)
    {
      AddReferenceAndDependencies(installItem.Script, installItem.Reference, installItem.CoreDependencies);
    }

    private void AddReferenceAndDependencies(XmlElement elem, ItemReference itemRef, IEnumerable<ItemReference> existingDependencies)
    {
      _dependencies.Clear();
      _definitions.Clear();

      var originalXml = elem.GetAttribute(DiffAnnotation.OriginalXmlAttribute);
      if (!string.IsNullOrEmpty(originalXml))
      {
        Debug.Print("Found original XML for " + itemRef.KeyedName);
        var doc = new XmlDocument();
        doc.LoadXml(originalXml);
        elem = doc.DocumentElement;
      }

      if (existingDependencies != null) _dependencies.UnionWith(existingDependencies);

      _elem = elem;
      VisitNode(elem, itemRef);
      if (_customDependencies != null)
      {
        var context = new Context(elem, itemRef, this);
        _customDependencies(context);
      }

      // Clean up dependencies
      foreach (var defn in _definitions)
      {
        _dependencies.Remove(defn);
        try
        {
          _allDefinitions.Add(defn, itemRef);
        }
        catch (ArgumentException)
        {
          //throw new ArgumentException(string.Format("Error: {0} is defined twice, once in {1} and another time in {2}.\r\nCurrently processing {3}",
          //  defn, _allDefinitions[defn], itemRef, elem.OuterXml));
        }
      }
      _dependencies.ExceptWith(_metadata.SystemIdentities);
      _dependencies.ExceptWith(_metadata.Methods.Where(m => m.IsCore));
      _dependencies.ExceptWith(_metadata.ItemTypes.Where(i => i.IsCore).Select(i => i.Reference));
      _dependencies.ExceptWith(_dependencies.Where(d =>
      {
        if (d.Type == "ItemType" && !string.IsNullOrEmpty(d.KeyedName))
        {
          ItemType it;
          if (_metadata.ItemTypeByName(d.KeyedName, out it) && it.IsCore) return true;
        }
        return false;
      }).ToList());
      _dependencies.Remove(itemRef);

      foreach (var depend in _dependencies)
      {
        depend.Origin = itemRef;
      }

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

      elem.SetAttribute(XmlFlags.Attr_DependenciesAnalyzed, "1");

      _dependencies.Clear();
      _definitions.Clear();
    }

    /// <summary>
    /// After adding all of the dependencies, perform cleanup operations
    /// </summary>
    public void FinishAdding()
    {
      ItemReference topDefn;
      ItemReference currValue;

      var refToDeleteRef = _allItemDependencies.Keys
        .Where(i => i.Type == InstallItem.ScriptType
          && i.KeyedName.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0
          && i.Origin != null)
        .ToDictionary(i => i.Origin);

      foreach (var kvp in _allItemDependencies)
      {
        var isDelete = kvp.Key.Type == InstallItem.ScriptType
          && kvp.Key.KeyedName.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0;
        foreach (var value in kvp.Value.ToList())
        {
          currValue = value;
          if (isDelete && refToDeleteRef.TryGetValue(value, out var otherDelete))
          {
            // Make sure that dependencies between delete scripts are handled and sorted properly
            kvp.Value.Remove(value);
            if (otherDelete != kvp.Key)
              kvp.Value.Add(otherDelete);
          }
          else if (currValue.Unique.StartsWith(ItemTypeByNameWhere))
          {
            // If there is a dependency on an item type solely by name, add dependences on the
            // item type creation script and all subsequent edit scripts
            topDefn = _allDefinitions.Values
              .FirstOrDefault(v => v.KeyedName == currValue.KeyedName
                && v.Type == "ItemType") ??
              _allDefinitions.Keys
              .FirstOrDefault(v => v.KeyedName == currValue.KeyedName
                && v.Type == "ItemType")
              ;
            if (topDefn == null)
            {
              Debug.Print("Unable to find definition for " + currValue.Unique);
            }
            else
            {
              kvp.Value.Remove(currValue);
              kvp.Value.Add(topDefn);
              kvp.Value.UnionWith(_allItemDependencies.Keys
                .Where(k => k.Type == InstallItem.ScriptType && k.Unique.Contains(topDefn.Unique)));
              currValue = topDefn;
            }
          }

          // Make sure the dependency points to the root dependency and not a child that can't be found
          // and sorted properly
          if (_allDefinitions.TryGetValue(currValue, out topDefn))
          {
            kvp.Value.Remove(currValue);
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
        if (_metadata.ItemTypeByName(elem.Attribute("type").ToLowerInvariant(), out itemType))
        {
          AddDependency(itemType.Reference, elem, elem, masterRef);
        }
        else
        {
          AddDependency(new ItemReference("ItemType", ItemTypeByNameWhere + elem.Attributes["type"].Value + "'")
          {
            KeyedName = elem.Attributes["type"].Value
          }, elem, elem, masterRef);
        }

        if (!string.IsNullOrEmpty(elem.GetAttribute("action")))
        {
          // AML with a custom action
          var methodRef = _metadata.Methods
            .FirstOrDefault(m => m.KeyedName == elem.GetAttribute("action"));
          if (methodRef != null)
          {
            AddDependency(methodRef.Clone(), elem, elem, masterRef);
          }
        }
      }

      // Item property node
      if (elem.HasAttribute("type") && textChildren.Count == 1 && !string.IsNullOrEmpty(textChildren[0].Value))
      {
        AddDependency(ItemReference.FromItemProp(elem), elem.Parent(), elem, masterRef);
      }
      else if (elem.LocalName == "sqlserver_body" && elem.Parent().LocalName == "Item" && elem.Parent().Attribute("type") == "SQL")
      {
        var names = GetInnovatorNames(elem.InnerText)
          .Select(n => n.FullName.StartsWith("innovator.", StringComparison.OrdinalIgnoreCase) ?
                       n.FullName.Substring(10).ToLowerInvariant() :
                       n.FullName.ToLowerInvariant())
          .Distinct();

        ItemType itemType;
        ItemReference sql;
        foreach (var name in names)
        {
          if (_metadata.ItemTypeByName(name.Replace('_', ' '), out itemType)
            || _metadata.ItemTypeByName(name, out itemType))
          {
            AddDependency(itemType.Reference, elem.Parent(), elem, masterRef);
          }
          else if (_metadata.SqlRefByName(name, out sql))
          {
            AddDependency(sql, elem.Parent(), elem, masterRef);
          }
        }
      }
      else if (elem.LocalName == "data_source" && textChildren.Count == 1 && !string.IsNullOrEmpty(textChildren[0].Value))
      {
        // Property data source dependencies
        var parent = elem.ParentNode as XmlElement;
        if (parent?.LocalName == "Item" && parent.Attribute("type") == "Property")
        {
          var keyedName = elem.Attribute("keyed_name");
          var itemtype = _metadata.ItemTypes.FirstOrDefault(i => i.Reference.Unique == elem.Parent().Parent().Parent().Attribute("id") && i.IsPolymorphic);
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
      else if (elem.LocalName == "reference" && elem.Parent().LocalName == "Item"
        && (elem.Parent().Attribute("type") == "FeedTemplate" || elem.Parent().Attribute("type") == "FileSelectorTemplate")
        && TryGetSsvcReference(elem.InnerText, out var ssvcElement))
      {
        VisitNode(ssvcElement, masterRef);
      }
      else if (elem.LocalName == "document"
        && elem.ChildNodes.OfType<XmlText>().FirstOrDefault()?.Value.Trim().StartsWith("{") == true)
      {
        // A JSON document
        try
        {
          using (var doc = JsonDocument.Parse(elem.InnerText))
            VisitJson(doc.RootElement, elem, masterRef);
        }
        catch (JsonException) { }
      }
      else if (elem.LocalName == "method_code"
        && elem.Parent().LocalName == "Item"
        && elem.Parent().Attribute("type") == "Method"
        && elem.Parent().Element("method_type", "") == "C#")
      {
        // A C# server method
        var names = new HashSet<string>(GetCSharpMethodCalls(elem.InnerText));
        foreach (var methodRef in _metadata.Methods.Where(m => names.Contains(m.KeyedName)))
          AddDependency(methodRef.Clone(), elem.Parent(), elem, masterRef);
      }
      else if (elem != _elem && elem.LocalName == "Item" && elem.HasAttribute("type")
        && elem.Attribute("action", "") == "get"
        && (elem.HasAttribute("id") || elem.HasAttribute("where")))
      {
        // Item queries
        AddDependency(ItemReference.FromFullItem(elem, true), elem.Parent().Parent(), elem.Parent(), masterRef);
      }
      else if (textChildren.Count == 1 && textChildren[0].Value.StartsWith("vault:///?fileId=", StringComparison.OrdinalIgnoreCase))
      {
        // Vault Id references for image properties
        AddDependency(new ItemReference("File", textChildren[0].Value.Substring(17)), elem.Parent(), elem, masterRef);
      }
      else if (elem.LocalName == "parameters"
        && elem.Parent().LocalName == "Item"
        && elem.Parent().Attribute("type") == "Relationship View"
        && textChildren.Count == 1
        && _treeGridViewUrl.TryMatch(textChildren[0].Value, out var treeGridView))
      {
        var itemRef = new ItemReference("rb_TreeGridViewDefinition", treeGridView.Groups["id"].Value);
        if (!_allDeleteDependencies.TryGetValue(itemRef, out var hashSet))
        {
          hashSet = new HashSet<ItemReference>();
          _allDeleteDependencies.Add(itemRef, hashSet);
        }
        hashSet.Add(masterRef);
      }
      else
      {
        if (elem != _elem && elem.LocalName == "Item"
          && elem.HasAttribute("type") && elem.HasAttribute("id")
          && elem.Attribute("action", "edit") != "edit")
        {
          _definitions.Add(ItemReference.FromFullItem(elem, elem.Attribute("type") == "ItemType"));
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
            if (_metadata.CustomPropertyByPath(newProp, out propRef))
            {
              propRef = propRef.Clone();
              AddDependency(propRef, elem, child, masterRef);
            }
          }
          VisitNode(child, masterRef);
        }
      }
    }

    private void VisitJson(JsonElement element, XmlElement propertyTag, ItemReference masterRef)
    {
      foreach (var property in element.EnumerateObject())
      {
        if (property.Name == "on_execute"
          && property.Value.ValueKind == JsonValueKind.String
          && property.Value.GetString().IsGuid())
        {
          AddDependency(new ItemReference("Method", "[Method].[config_id] = '" + property.Value.GetString() + "'"), propertyTag.Parent(), propertyTag, masterRef);
        }
        else if (property.Value.ValueKind == JsonValueKind.Array)
        {
          foreach (var item in property.Value.EnumerateArray())
          {
            if (item.ValueKind == JsonValueKind.Object)
              VisitJson(item, propertyTag, masterRef);
          }
        }
        else if (property.Value.ValueKind == JsonValueKind.Object)
        {
          VisitJson(property.Value, propertyTag, masterRef);
        }
      }
    }

    private bool TryGetSsvcReference(string reference, out XmlElement element)
    {
      if (string.IsNullOrEmpty(reference))
      {
        element = null;
        return false;
      }

      var doc = new XmlDocument();
      if (reference == "this")
      {
        element = doc.CreateElement("Item");
        return true;
      }
      else if (reference.StartsWith("<"))
      {
        doc.LoadXml(reference);
        element = doc.DocumentElement;
        return true;
      }
      else if (reference.IndexOf('(') > 0)
      {
        var path = reference.Substring(0, reference.IndexOf('(')).Split('/');
        element = doc.CreateElement("Item");
        var current = element;
        foreach (var relationship in path)
        {
          if (!string.IsNullOrEmpty(current.GetAttribute("type")))
          {
            var rel = doc.CreateElement("Relationships");
            var related = doc.CreateElement("Item");
            current.AppendChild(rel);
            rel.AppendChild(related);
            current = related;
          }
          current.SetAttribute("type", relationship);
        }
        var property = reference.Substring(reference.IndexOf('(') + 1).TrimEnd(')');
        current.SetAttribute("select", property);
        return true;
      }
      else
      {
        element = doc.CreateElement("Item");
        element.SetAttribute("select", reference);
        return true;
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

      public int Count
      {
        get { return _contexts.Count; }
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
        return from c in _contexts where c.MasterRef.Equals(masterRef) select c.Reference;
      }

      public void RemoveByMaster(ItemReference masterRef)
      {
        var i = 0;
        while (i < _contexts.Count)
        {
          if (_contexts[i].MasterRef == masterRef)
          {
            _contexts.RemoveAt(i);
          }
          else
          {
            i++;
          }
        }
      }

      public void RemoveAllButMasterList(HashSet<ItemReference> masterRefsToKeep)
      {
        var i = 0;
        while (i < _contexts.Count)
        {
          if (masterRefsToKeep.Contains(_contexts[i].MasterRef))
          {
            i++;
          }
          else
          {
            _contexts.RemoveAt(i);
          }
        }
      }
    }

    private class ReferenceContext
    {
      public XmlNode Context { get; set; }
      public ItemReference MasterRef { get; set; }
      public XmlNode Reference { get; set; }
    }

    public static IEnumerable<SqlName> GetInnovatorNames(string sql)
    {
      var parsed = new SqlTokenizer(sql).ToArray();
      return parsed.OfType<SqlName>()
        .Where(n => string.Equals(n[0].Text, "innovator", StringComparison.OrdinalIgnoreCase)
          || (!n.Any(l => l.Text == ".") && !n[0].Text.StartsWith("@")));
    }

    public static IEnumerable<string> GetCSharpMethodCalls(string code)
    {
      var methods = new HashSet<string>();
      try
      {
        code = $@"public class MethodCode
{{
  public object Item()
  {{
{code}
  }}
}}";
        var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
        foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
          var identifier = invocation.Expression.DescendantNodesAndSelf()
            .OfType<IdentifierNameSyntax>()
            .LastOrDefault();
          var name = identifier?.Identifier.Text;
          if (name == "newItem"
            && invocation.ArgumentList.Arguments.Count == 2)
          {
            var actionLiterals = GetStringLiterals(invocation.ArgumentList.Arguments[1].Expression).ToList();
            if (actionLiterals.Count == 1)
              methods.Add(actionLiterals[0]);
          }
          else if ((name == "apply" || name == "setAction")
            && invocation.ArgumentList.Arguments.Count == 1)
          {
            var actionLiterals = GetStringLiterals(invocation.ArgumentList.Arguments[0].Expression).ToList();
            if (actionLiterals.Count == 1)
              methods.Add(actionLiterals[0]);
          }
          else if ((name == "applyAML" || name == "loadAML")
            && invocation.ArgumentList.Arguments.Count == 1)
          {
            methods.UnionWith(GetStringLiterals(invocation.ArgumentList.Arguments[0].Expression)
              .Where(s => s.StartsWith("<"))
              .SelectMany(GetAmlActions));
          }
          else if ((name == "apply" || name == "setAction")
            && invocation.ArgumentList.Arguments.Count == 1)
          {
            var actionLiterals = GetStringLiterals(invocation.ArgumentList.Arguments[0].Expression).ToList();
            if (actionLiterals.Count == 1)
              methods.Add(actionLiterals[0]);
          }
          // applySQL
        }
      }
      catch (Exception) { }
      methods.ExceptWith(CoreActions.GetActions(int.MaxValue));
      return methods;
    }

    private static IEnumerable<string> GetAmlActions(string xmlString)
    {
      var results = new List<string>();
      try
      {
        using (var reader = new StringReader(xmlString))
        using (var xml = XmlReader.Create(reader))
        {
          while (xml.Read())
          {
            if (xml.NodeType == XmlNodeType.Element
              && xml.LocalName == "Item"
              && !string.IsNullOrEmpty(xml.GetAttribute("action")))
            {
              results.Add(xml.GetAttribute("action"));
            }
          }
        }
      }
      catch (XmlException) { }
      return results;
    }

    private static IEnumerable<string> GetStringLiterals(ExpressionSyntax expressionSyntax)
    {
      return expressionSyntax.DescendantNodesAndSelf()
        .OfType<LiteralExpressionSyntax>()
        .Select(l => l.Token.Value)
        .OfType<string>();
    }

    private class Context : IDependencyContext
    {
      private readonly DependencyAnalyzer _parent;

      public XmlElement Element { get; }

      public ItemReference ItemReference { get; }

      public Context(XmlElement element, ItemReference itemReference, DependencyAnalyzer parent)
      {
        Element = element;
        ItemReference = itemReference;
        _parent = parent;
      }

      public void AddDependency(ItemReference itemRef, XmlNode context, XmlNode reference)
      {
        _parent.AddDependency(itemRef, context, reference, ItemReference);
      }
    }
  }
}
