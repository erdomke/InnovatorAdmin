using InnovatorAdmin;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class CompletionHelper : ISqlMetadataProvider
  {
    protected IAsyncConnection _conn;
    private ArasMetadataProvider _metadata;
    private SqlCompletionHelper _sql;

    public CompletionHelper()
    {
      _sql = new SqlCompletionHelper(this);
    }

    public IPromise<CompletionContext> GetCompletions(string xml, int caret, string soapAction)
    {
      //var overlap = 0;
      if (string.IsNullOrEmpty(xml)) return Promises.Resolved<CompletionContext>(new CompletionContext());

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
        return Promises.Resolved<CompletionContext>(new CompletionContext());

      IPromise<IEnumerable<ICompletionData>> items = null;
      var filter = string.Empty;

      if (path.Count < 1)
      {
        switch (soapAction)
        {
          case "ApplySQL":
            items = CompletionExtensions.GetPromise<BasicCompletionData>("sql");
            break;
          case "ApplyAML":
            items = CompletionExtensions.GetPromise<BasicCompletionData>("AML");
            break;
          case "GetAssignedTasks":
            items = CompletionExtensions.GetPromise<BasicCompletionData>("params");
            break;
          default:
            items = CompletionExtensions.GetPromise<BasicCompletionData>("Item");
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
              case "Path":
                items = new string[] { "id" }.GetPromise<AttributeCompletionData>();
                break;
              case "Task":
                items = new string[] {"id", "completed"}.GetPromise<AttributeCompletionData>();
                break;
              case "Variable":
                items = new string[] { "id" }.GetPromise<AttributeCompletionData>();
                break;
              case "Authentication":
                items = new string[] { "mode" }.GetPromise<AttributeCompletionData>();
                break;
              case "Item":
                switch (soapAction)
                {
                  case "GenerateNewGUIDEx":
                    items = CompletionExtensions.GetPromise<AttributeCompletionData>("quantity");
                    break;
                  case "":
                    break;
                  default:
                    var attributes = new List<string>
                    { "action"
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
                      , "where"};
                    if (path.Last().Action == "getPermissions")
                      attributes.Add("access_type");

                    if (path.Count >= 3
                      && path[path.Count - 2].LocalName == "Relationships"
                      && path[path.Count - 3].LocalName == "Item"
                      && path[path.Count - 3].Action == "GetItemRepeatConfig")
                    {
                      attributes.Add("repeatProp");
                      attributes.Add("repeatTimes");
                    }

                    items = attributes.GetPromise<AttributeCompletionData>();
                    break;
                }
                break;
              default:
                items = new string[] { "condition", "is_null" }.GetPromise<AttributeCompletionData>();
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
                    , "promoteItem"
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

                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>(_metadata.MethodNames.Concat(baseMethods));
                  break;
                case "access_type":
                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>("can_add", "can_delete", "can_get", "can_update");
                  break;
                case "doGetItem":
                case "version":
                case "isCriteria":
                case "related_expand":
                case "serverEvents":
                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>("0", "1");
                  break;
                case "queryType":
                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>("Effective", "Latest", "Released");
                  break;
                case "orderBy":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    var lastComma = value.LastIndexOf(",");
                    if (lastComma >= 0) value = value.Substring(lastComma + 1).Trim();

                    items = _metadata.GetProperties(itemType)
                      .Convert(p => p.SelectMany(i => new ICompletionData[] {
                        new AttributeValueCompletionData() { Text = i.Name, Description = i.Label, MultiValue = true },
                        new AttributeValueCompletionData() { Text = i.Name + " DESC", Description = i.Label, MultiValue = true }
                      }));
                  }
                  break;
                case "select":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    string partial;
                    var selectPath = SelectPath(value, out partial);
                    value = partial;

                    var itPromise = new Promise<ItemType>();
                    RecurseProperties(itemType, selectPath, it => itPromise.Resolve(it));

                    items = itPromise
                      .Continue(it => _metadata.GetProperties(it))
                      .Convert(p => p.Select(i => (ICompletionData)new AttributeValueCompletionData() {
                        Text = i.Name,
                        Description = i.Label,
                        MultiValue = true
                      }));
                  }
                  break;
                case "type":
                  if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _metadata.ItemTypeByName(path[path.Count - 3].Type, out itemType))
                    {
                      items = CompletionExtensions.GetPromise<AttributeValueCompletionData>(itemType.Relationships.Select(r => r.Name));
                    }
                  }
                  else
                  {
                    items = CompletionExtensions.GetPromise<AttributeValueCompletionData>(_metadata.ItemTypes.Select(i => i.Name));
                  }
                  break;
                case "where":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    items = _metadata.GetProperties(itemType)
                      .Convert(i => i.Select(p => (ICompletionData)new AttributeValueCompletionData() {
                        Text = "[" + itemType.Name + "].[" + p.Name + "]",
                        Description = p.Label,
                        MultiValue = true
                      }));
                  }
                  break;
              }
            }
            else
            {
              switch (attrName)
              {
                case "condition":
                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>("between"
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
                  items = CompletionExtensions.GetPromise<AttributeValueCompletionData>("0", "1");
                  break;
              }
            }

            filter = value;
            break;
          default:
            if (path.Count == 1 && path.First().LocalName == "AML")
            {
              items = CompletionExtensions.GetPromise<BasicCompletionData>("Item");
            }
            if (path.Count == 1 && path.First().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase) && soapAction == "ApplySQL")
            {
              return _sql.Completions(value, xml, caret, cdata ? "]]>" : "<");
            }
            else
            {
              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];
              if (last.LocalName == "Item")
              {
                switch (last.Action)
                {
                  case "AddHistory":
                    items = new string[] {"action", "filename", "form_name"}.GetPromise<BasicCompletionData>();
                    break;
                  case "GetItemsForStructureBrowser":
                    items = new string[] { "Item" }.GetPromise<BasicCompletionData>();
                    break;
                  case "EvaluateActivity":
                    items = new string[] { "Activity", "ActivityAssignment", "Paths", "DelegateTo"
                      , "Tasks", "Variables", "Authentication", "Comments", "Complete" }.GetPromise<BasicCompletionData>();
                    break;
                  case "instantiateWorkflow":
                    items = new string[] { "WorkflowMap" }.GetPromise<BasicCompletionData>();
                    break;
                  case "promoteItem":
                    items = new string[] { "state", "comments" }.GetPromise<BasicCompletionData>();
                    break;
                  case "Run Report":
                    items = new string[] { "report_name", "AML" }.GetPromise<BasicCompletionData>();
                    break;
                  case "SQL Process":
                    items = new string[] { "name", "PROCESS", "ARG1", "ARG2", "ARG3", "ARG4"
                      , "ARG5", "ARG6", "ARG7", "ARG8", "ARG9" }.GetPromise<BasicCompletionData>();
                    break;
                  default:
                    // Completions for item properties
                    var buffer = new List<ICompletionData>();

                    buffer.Add(new BasicCompletionData("Relationships"));
                    if (last.Action == "get")
                    {
                      buffer.Add(new BasicCompletionData("and"));
                      buffer.Add(new BasicCompletionData("not"));
                      buffer.Add(new BasicCompletionData("or"));
                    }
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(last.Type)
                      && _metadata.ItemTypeByName(last.Type, out itemType))
                    {
                      items = _metadata.GetProperties(itemType)
                          .Convert(p => p.Select(i => new BasicCompletionData() { Text = i.Name, Description = i.Label }).Concat(buffer));
                    }
                    else
                    {
                      items = Promises.Resolved<IEnumerable<ICompletionData>>(buffer);
                    }
                    break;
                }
              }
              else if (last.LocalName == "params" && soapAction == "GetAssignedTasks")
              {
                items = new string[] { "inBasketViewMode", "workflowTasks", "projectTasks", "actionTasks", "userID" }.GetPromise<BasicCompletionData>();
              }
              else if (last.LocalName == "Paths" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = new string[] { "Path" }.GetPromise<BasicCompletionData>();
              }
              else if (last.LocalName == "Tasks" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = new string[] { "Task" }.GetPromise<BasicCompletionData>();
              }
              else if (last.LocalName == "Variables" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = new string[] { "Variable" }.GetPromise<BasicCompletionData>();
              }
              else if (path.Count > 1)
              {
                var lastItem = path.LastOrDefault(n => n.LocalName == "Item");
                if (lastItem != null)
                {
                  if (path.Last().LocalName == "Relationships")
                  {
                    items = CompletionExtensions.GetPromise<BasicCompletionData>("Item");
                  }
                  else if (path.Last().Condition == "in"
                    || path.Last().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase))
                  {
                    return _sql.Completions(value, xml, caret, cdata ? "]]>" : "<");
                  }
                  else
                  {
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(lastItem.Type)
                      && _metadata.ItemTypeByName(lastItem.Type, out itemType))
                    {
                      items = _metadata.GetProperty(itemType, path.Last().LocalName)
                        .Convert(p =>
                        {
                          if (p.Type == PropertyType.item && p.Restrictions.Any())
                          {
                            return p.Restrictions
                              .Select(type => "Item type='" + type + "'")
                              .GetCompletions<BasicCompletionData>();
                          }
                          else
                          {
                            return Enumerable.Empty<ICompletionData>();
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
        return Promises.Resolved(new CompletionContext());

      return items.Convert(i => new CompletionContext() {
        Items = string.IsNullOrEmpty(filter) ? i.OrderBy(j => j.Text) : FilterAndSort(i, filter),
        IsXmlAttribute = (state == XmlState.Attribute || state == XmlState.AttributeStart),
        Overlap = (filter ?? "").Length
      });
    }

    private void RecurseProperties(ItemType itemType, IEnumerable<string> remainingPath, Action<ItemType> callback)
    {
      if (remainingPath.Any())
      {
        _metadata.GetProperty(itemType, remainingPath.First())
        .Done(currProp =>
        {
          ItemType it;
          if (currProp.Type == PropertyType.item
            && currProp.Restrictions.Any()
            && _metadata.ItemTypeByName(currProp.Restrictions.First(), out it))
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
      _conn = conn;
      _metadata = ArasMetadataProvider.Cached(conn);
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

    private IEnumerable<ICompletionData> FilterAndSort(IEnumerable<ICompletionData> values, string substring)
    {
      var result = values
        .Where(i => i.Text.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0)
        .ToList();
      result.Sort((x, y) =>
      {
        var xStart = x.Text.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        var yStart = y.Text.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
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
          return x.Text.CompareTo(y.Text);
        }
      });
      return result;
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

    private class AmlNode
    {
      public string LocalName { get; set; }
      public string Type { get; set; }
      public string Action { get; set; }
      public string Condition { get; set; }
    }

    public IEnumerable<string> GetSchemaNames()
    {
      return Enumerable.Repeat("innovator", 1);
    }

    public IEnumerable<string> GetTableNames()
    {
      return _metadata.ItemTypes
        .Select(i => "innovator.[" + i.Name.Replace(' ', '_') + "]");
    }

    public IPromise<IEnumerable<string>> GetColumnNames(string tableName)
    {
      ItemType itemType;
      if (_metadata.ItemTypeByName(tableName.Replace('_', ' '), out itemType))
        return _metadata.GetProperties(itemType).Convert(p => p.Select(i => i.Name));

      return Promises.Resolved(Enumerable.Empty<string>());
    }
  }
}
