using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class CompletionHelper
  {
    private IAsyncConnection _conn;
    private ItemTypeCollection _itemTypes = new ItemTypeCollection();
    private IPromise<IEnumerable<string>> _methods;

    public IPromise<CompletionData> GetCompletions(string xml, int caret, string soapAction)
    {
      //var overlap = 0;
      if (string.IsNullOrEmpty(xml)) return Promises.Resolved<CompletionData>(new CompletionData());

      var path = new List<AmlNode>();
      string attrName = null;
      string value = null;
      bool cdata = false;

      var state = XmlUtils.ProcessFragment(xml.Substring(0, caret), (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
            if (!r.IsEmptyElement)
              path.Add(new AmlNode()
              {
                LocalName = r.LocalName,
                Type = r.GetAttribute("type"),
                Action = r.GetAttribute("action"),
                Condition = r.GetAttribute("condition")
              });
            break;
          case XmlNodeType.EndElement:
            path.RemoveAt(path.Count - 1);
            break;
          case XmlNodeType.Attribute:
            attrName = r.LocalName;
            value = r.Value;
            break;
          case XmlNodeType.CDATA:
            cdata = true;
            value = r.Value;
            break;
          case XmlNodeType.Text:
            cdata = false;
            value = r.Value;
            break;
        }
        return true;
      });
      if (state == XmlState.Tag && (xml.Last() == '"' || xml.Last() == '\''))
        return Promises.Resolved<CompletionData>(new CompletionData());

      IPromise<IEnumerable<string>> items = null;
      var filter = string.Empty;
      var multiValueAttribute = false;

      if (path.Count < 1)
      {
        switch (soapAction)
        {
          case "ApplySQL":
            items = StringPromise("sql");
            break;
          case "ApplyAML":
            items = StringPromise("AML");
            break;
          default:
            items = StringPromise("Item");
            break;
        }
      }
      else
      {
        switch (state)
        {
          case XmlState.Attribute:
          case XmlState.AttributeStart:
            switch (path.Last().LocalName)
            {
              case "and":
              case "or":
              case "not":
              case "Relationships":
              case "AML":
              case "sql":
              case "SQL":
                break;
              case "Item":
                switch (soapAction)
                {
                  case "GenerateNewGUIDEx":
                    items = StringPromise("quantity");
                    break;
                  case "":
                    break;
                  default:
                    items = StringPromise(new string[] {"action"
                      , "access_type"
                      , "config_path"
                      , "doGetItem"
                      , "id"
                      , "idlist"
                      , "isCriteria"
                      , "language"
                      , "levels"
                      , "maxRecords"
                      , "page"
                      , "pagesize"
                      , "orderBy"
                      , "queryDate"
                      , "queryType"
                      , "related_expand"
                      , "select"
                      , "serverEvents"
                      , "type"
                      , "typeID"
                      , "version"
                      , "where"}
                      .Where(i => path.Last().Action == "getPermissions" || i != "access_type"));
                    break;
                }
                break;
              default:
                items = StringPromise("condition", "is_null");
                break;
            }

            filter = attrName;
            break;
          case XmlState.AttributeValue:
            if (path.Last().LocalName == "Item")
            {
              ItemType itemType;
              switch (attrName)
              {
                case "action":
                  if (_methods == null && _conn != null)
                  {
                    _methods = _conn.ApplyAsync("<AML><Item action=\"get\" type=\"Method\" select=\"name\"></Item></AML>", true, false)
                      .Convert(r => r.Items()
                                     .Select(m => m.Property("name").AsString("")));
                  }

                  var baseMethods = new string[] {"ActivateActivity"
                    , "add"
                    , "AddItem"
                    , "AddHistory"
                    , "ApplyUpdate"
                    , "BuildProcessReport"
                    , "CancelWorkflow"
                    , "checkImportedItemType"
                    , "closeWorkflow"
                    , "copy"
                    , "copyAsIs"
                    , "copyAsNew"
                    , "create"
                    , "delete"
                    , "edit"
                    , "EmailItem"
                    , "EvaluateActivity"
                    , "exportItemType"
                    , "get"
                    , "getItemAllVersions"
                    , "getAffectedItems"
                    , "GetItemConfig"
                    , "getItemLastVersion"
                    , "getItemNextStates"
                    , "getItemRelationships"
                    , "GetItemRepeatConfig"
                    , "getItemWhereUsed"
                    , "GetMappedPath"
                    , "getPermissions"
                    , "getRelatedItem"
                    , "GetUpdateInfo"
                    , "instantiateWorkflow"
                    , "lock"
                    , "merge"
                    , "New Workflow Map"
                    , "PromoteItem"
                    , "purge"
                    , "recache"
                    , "replicate"
                    , "resetAllItemsAccess"
                    , "resetItemAccess"
                    , "resetLifecycle"
                    , "setDefaultLifecycle"
                    , "skip"
                    , "startWorkflow"
                    , "unlock"
                    , "update"
                    , "ValidateWorkflowMap"
                    , "version"};

                  items = _methods == null
                    ? StringPromise(baseMethods)
                    : _methods.Convert(m => m.Concat(baseMethods));
                  break;
                case "access_type":
                  items = StringPromise("can_add", "can_delete", "can_get", "can_update");
                  break;
                case "doGetItem":
                case "version":
                case "isCriteria":
                case "related_expand":
                case "serverEvents":
                  items = StringPromise("0", "1");
                  break;
                case "queryType":
                  items = StringPromise("Effective", "Latest", "Released");
                  break;
                case "orderBy":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    var lastComma = value.LastIndexOf(",");
                    if (lastComma >= 0) value = value.Substring(lastComma + 1).Trim();

                    items = GetProperties(itemType)
                      .Convert(p => p.SelectMany(i => new string[] { i.Name, i.Name + " DESC" }));
                  }
                  multiValueAttribute = true;
                  break;
                case "select":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    string partial;
                    var selectPath = SelectPath(value, out partial);
                    value = partial;

                    var itPromise = new Promise<ItemType>();
                    RecurseProperties(itemType, selectPath, it => itPromise.Resolve(it));

                    items = itPromise
                      .Continue(it => GetProperties(it))
                      .Convert(p => p.Select(i => i.Name));
                  }
                  multiValueAttribute = true;
                  break;
                case "type":
                  if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _itemTypes.TryGetValue(path[path.Count - 3].Type, out itemType))
                    {
                      items = StringPromise(itemType.Relationships.Select(r => r.Name));
                    }
                  }
                  else
                  {
                    items = StringPromise(_itemTypes.Select(i => i.Value.Name));
                  }
                  break;
                case "where":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _itemTypes.TryGetValue(path.Last().Type, out itemType))
                  {
                    items = GetProperties(itemType)
                      .Convert(i => i.Select(p => "[" + itemType.Name + "].[" + p.Name + "]"));
                  }
                  multiValueAttribute = true;
                  break;
              }
            }
            else
            {
              switch (attrName)
              {
                case "condition":
                  items = StringPromise("between"
                    , "eq"
                    , "ge"
                    , "gt"
                    , "in"
                    , "is not null"
                    , "is null"
                    , "is"
                    , "le"
                    , "like"
                    , "lt"
                    , "ne"
                    , "not between"
                    , "not in"
                    , "not like");
                  break;
                case "is_null":
                  items = StringPromise("0", "1");
                  break;
              }
            }

            filter = value;
            break;
          default:
            if (path.Count == 1 && path.First().LocalName == "AML")
            {
              items = StringPromise("Item");
            }
            if (path.Count == 1 && path.First().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase) && soapAction == "ApplySQL")
            {
              return SqlCompletions(value, xml, caret, cdata);
            }
            else
            {
              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];
              if (last.LocalName == "Item")
              {
                var buffer = new List<string>();

                buffer.Add("Relationships");
                if (last.Action == "get")
                {
                  buffer.Add("and");
                  buffer.Add("not");
                  buffer.Add("or");
                }
                ItemType itemType;
                if (!string.IsNullOrEmpty(last.Type)
                  && _itemTypes.TryGetValue(last.Type, out itemType))
                {
                  items = GetProperties(itemType)
                      .Convert(p => p.Select(i => i.Name).Concat(buffer));
                }
                else
                {
                  items = StringPromise(buffer);
                }
              }
              else if (path.Count > 1)
              {
                var lastItem = path.LastOrDefault(n => n.LocalName == "Item");
                if (lastItem != null)
                {
                  if (path.Last().LocalName == "Relationships")
                  {
                    items = StringPromise("Item");
                  }
                  else if (path.Last().Condition == "in")
                  {
                    return SqlCompletions(value, xml, caret, cdata);
                  }
                  else
                  {
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(lastItem.Type)
                      && _itemTypes.TryGetValue(lastItem.Type, out itemType))
                    {
                      items = GetProperty(itemType, path.Last().LocalName)
                        .Convert(p =>
                        {
                          if (p.Type == PropertyType.item && p.Restrictions.Any())
                          {
                            return p.Restrictions.Select(type => "Item type=\"" + type + "\"");
                          }
                          else
                          {
                            return Enumerable.Empty<string>();
                          }
                        });
                    }
                  }
                }
              }
            }

            break;
        }

      }

      if (items == null)
        return Promises.Resolved(new CompletionData());

      return items.Convert(i => new CompletionData() {
        Items = string.IsNullOrEmpty(filter) ? i.OrderBy(j => j) : FilterAndSort(i, filter),
        MultiValueAttribute = multiValueAttribute,
        Overlap = (filter ?? "").Length,
        State = GetCompletionType(state)
      });
    }

    private CompletionType GetCompletionType(XmlState state)
    {
      switch (state)
      {
        case XmlState.Attribute:
        case XmlState.AttributeStart:
          return CompletionType.Attribute;
        case XmlState.AttributeValue:
          return CompletionType.AttributeValue;
      }
      return CompletionType.Other;
    }

    private IPromise<CompletionData> SqlCompletions(string prefix, string all, int caret, bool cdata)
    {
      var lastIndex = cdata ? all.IndexOf("]]>", caret) : all.IndexOf('<', caret);
      var sql = prefix + (lastIndex < 0 ? all.Substring(caret) : all.Substring(caret, lastIndex - caret));
      var parseTree = new SqlTokenizer(sql).Parse();
      if (!parseTree.Any())
        return Promises.Resolved(new CompletionData());

      var currNode = parseTree.NodeByOffset(prefix.Length);
      var literal = currNode as SqlLiteral;

      if (literal != null)
      {

        if (literal.Text.Equals("from", StringComparison.OrdinalIgnoreCase)
          || literal.Text.Equals("join", StringComparison.OrdinalIgnoreCase)
          || literal.Text.Equals("apply", StringComparison.OrdinalIgnoreCase))
        {
          return Promises.Resolved(new CompletionData()
          {
            Items = _itemTypes.Values.Select(i => i.Name.Replace(' ', '_'))
              .Concat(Enumerable.Repeat("Innovator", 1)
              .OrderBy(i => i)),
            State = CompletionType.SqlObjectName,
            Overlap = 0
          });
        }
        else if (literal.Text == ".")
        {
          var name = literal.Parent as SqlNameDefinition;
          if (name != null)
          {
            var idx = name.IndexOf(literal);
            if (name[idx - 1].Text.Equals("innovator", StringComparison.OrdinalIgnoreCase))
            {
              return Promises.Resolved(new CompletionData()
              {
                Items = _itemTypes.Values.Select(i => i.Name.Replace(' ', '_')).Concat(Enumerable.Repeat("innovator", 1)),
                State = CompletionType.SqlObjectName,
                Overlap = 0
              });
            }
          }
          else
          {
            var group = literal.Parent as SqlGroup;
            if (group != null)
            {
              var idx = group.IndexOf(literal);
              var context = GetAliasContext(literal);
              string fullName;
              ItemType type;
              if (idx > 0 && group[idx -1] is SqlLiteral
                && context.TryGetValue(((SqlLiteral)group[idx -1]).Text.ToLowerInvariant(), out fullName)
                && _itemTypes.TryGetValue(fullName, out type))
              {
                return GetProperties(type)
                  .Convert(p => new CompletionData() {
                    Items = p.Select (i => i.Name),
                    State = CompletionType.SqlGeneral
                  });
              }
            }
          }
        }
        else if (SqlTokenizer.IsKeyword(literal.Text)
          || literal.Type == SqlType.Operator)
        {
          var group = literal.Parent as SqlGroup;
          if (group != null)
          {
            var types = GetTypes(group.OfType<SqlNameDefinition>().Select(n => n.Name));
            return Promises.All(types
              .Select(t => (IPromise)GetProperties(t)).ToArray())
              .Convert(l => new CompletionData() {
                Items = l.OfType<IEnumerable<Property>>()
                  .SelectMany(i => i)
                  .Select(i => i.Name)
                  .Distinct()
                  .OrderBy(i => i),
                State = CompletionType.SqlGeneral
              });
          }
        }
      }

      //System.Diagnostics.Debug.Print(parseTree.NodeByOffset(prefix.Length).ToString());
      return Promises.Resolved(new CompletionData());
    }

    private IEnumerable<ItemType> GetTypes(IEnumerable<string> names)
    {
      return from n in names
             where _itemTypes.ContainsKey(n)
             select _itemTypes[n];
    }

    private IDictionary<string, string> GetAliasContext(SqlNode node)
    {
      var result = new Dictionary<string, string>();

      var group = node.Parent as SqlGroup;
      while (group != null)
      {
        foreach (var name in group.OfType<SqlNameDefinition>())
        {
          if (name.Alias != null && !result.ContainsKey(name.Alias.ToLowerInvariant()))
            result[name.Alias.ToLowerInvariant()] = name.Name;
          result[name.Name.ToLowerInvariant()] = name.Name;
        }
        group = group.Parent as SqlGroup;
      }

      return result;
    }

    private void RecurseProperties(ItemType itemType, IEnumerable<string> remainingPath, Action<ItemType> callback)
    {
      if (remainingPath.Any())
      {
        GetProperty(itemType, remainingPath.First())
        .Done(currProp =>
        {
          ItemType it;
          if (currProp.Type == PropertyType.item
            && currProp.Restrictions.Any()
            && _itemTypes.TryGetValue(currProp.Restrictions.First(), out it))
          {
            RecurseProperties(it, remainingPath.Skip(1), callback);
          }
          else
          {
            callback(itemType);
          }
        })
        .Fail(ex => callback(itemType));
      }
      else
      {
        callback(itemType);
      }
    }

    private IPromise<IEnumerable<string>> StringPromise(IEnumerable<string> values)
    {
      return Promises.Resolved(values);
    }
    private IPromise<IEnumerable<string>> StringPromise(params string[] values)
    {
      return Promises.Resolved<IEnumerable<string>>(values);
    }

    public string GetQuery(string xml, int offset)
    {
      var start = -1;
      var end = -1;
      var depth = 0;
      string result = null;

      XmlUtils.ProcessFragment(xml, (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:

            if (depth == 0)
            {
              start = o;
            }
            if (r.IsEmptyElement)
            {
              end = xml.IndexOf("/>", o) + 2;
              if (offset >= start && offset < end)
              {
                result = xml.Substring(start, end - start);
                return false;
              }
            }
            else
            {
              depth++;
            }
            break;
          case XmlNodeType.EndElement:
            depth--;
            if (depth == 0)
            {
              end = xml.IndexOf('>', o) + 1;
              if (offset >= start && offset < end)
              {
                result = xml.Substring(start, end - start);
                return false;
              }
            }
            break;
        }
        return true;
      });

      return result;
    }

    public virtual void InitializeConnection(IAsyncConnection conn)
    {
      _itemTypes.Clear();
      if (conn != null)
      {
        _conn = conn;
        _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\" /><Item action=\"get\" type=\"RelationshipType\" related_expand=\"0\" select=\"related_id,source_id,relationship_id,name\" /></AML>"
          , true, false)
          .Done(r => LoadItemTypes(r.Items()));
      }
    }

    public string LastOpenTag(string xml)
    {
      bool isOpenTag = false;
      var path = new List<AmlNode>();

      var state = XmlUtils.ProcessFragment(xml, (r, o) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
            if (!r.IsEmptyElement)
            {
              path.Add(new AmlNode() {
                LocalName = r.LocalName
              });
              isOpenTag = true;
            }
            break;
          case XmlNodeType.EndElement:
            path.RemoveAt(path.Count - 1);
            isOpenTag = false;
            break;
          default:
            isOpenTag = false;
            break;
        }
        return true;
      });

      if (isOpenTag && path.Any()) return path.Last().LocalName;
      return null;
    }

    private IEnumerable<string> FilterAndSort(IEnumerable<string> values, string substring)
    {
      var result = values.Where(i => i.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
      result.Sort((x, y) =>
      {
        var xStart = x.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        var yStart = y.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        if (xStart && !yStart)
        {
          return -1;
        }
        else if (yStart && !xStart)
        {
          return 1;
        }
        else
        {
          return x.CompareTo(y);
        }
      });
      return result;
    }

    private IPromise<IEnumerable<Property>> GetProperties(ItemType itemType)
    {
      if (_conn == null || itemType.Properties.Count > 0)
        return Promises.Resolved<IEnumerable<Property>>(itemType.Properties.Values);

      return _conn.ApplyAsync("<AML><Item action=\"get\" type=\"ItemType\" select=\"name\"><name>@0</name><Relationships><Item action=\"get\" type=\"Property\" select=\"name,label,data_type,data_source\" /></Relationships></Item></AML>"
        , true, true, itemType.Name)
        .Convert(r => {
          LoadProperties(itemType, r.AssertItem());
          return (IEnumerable<Property>)itemType.Properties.Values;
        });
    }
    /// <summary>
    /// Loads the metadata pertaining to the item types in Aras.
    /// </summary>
    /// <param name="items">The item type data.</param>
    private void LoadItemTypes(IEnumerable<IReadOnlyItem> items)
    {
      try
      {
        ItemType currType = null;
        ItemType sourceType = null;

        foreach (var item in items)
        {
          Debug.Print(item.Type().Value);
          switch (item.Type().Value)
          {
            case "RelationshipType":
              if (_itemTypes.TryGetValue(item.Property("relationship_id").Attribute("name").Value, out currType))
              {
                if (item.SourceId().Attribute("name").HasValue() &&
                    _itemTypes.TryGetValue(item.SourceId().Attribute("name").Value, out sourceType))
                {
                  currType.Source = sourceType;
                  sourceType.Relationships.Add(currType);
                }
              }
              break;
            default: //"ItemType"
              _itemTypes.Add(new ItemType(item.Property("name").Value));
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Print(ex.ToString());
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
        newProp.SetType(prop.Property("data_type").Value);
        if (newProp.Type == PropertyType.item && prop.Property("data_source").Attribute("name").HasValue())
        {
          newProp.Restrictions.Add(prop.Property("data_source").Attribute("name").Value);
        }
        type.Properties.Add(newProp);
      }
    }

    private List<string> SelectPath(string selectStr, out string partial)
    {
      partial = null;
      var lastOperator = -1;
      var path = new List<string>();

      for (var i = 0; i < selectStr.Length; i++)
      {
        if (selectStr[i] == '(' || selectStr[i] == ')' || selectStr[i] == ',')
        {
          switch (selectStr[i])
          {
            case '(':
              path.Add(selectStr.Substring(lastOperator + 1, i - lastOperator - 1).Trim());
              break;
            case ')':
              path.RemoveAt(path.Count - 1);
              break;
          }
          lastOperator = i;
        }
      }
      if (lastOperator < selectStr.Length - 1)
      {
        partial = selectStr.Substring(lastOperator + 1).Trim();
      }
      return path;
    }

    private IPromise<Property> GetProperty(ItemType itemType, string name)
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

    private class AmlNode
    {
      public string LocalName { get; set; }
      public string Type { get; set; }
      public string Action { get; set; }
      public string Condition { get; set; }
    }
  }
}
