using InnovatorAdmin;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  public partial class AmlEditorHelper : ISqlMetadataProvider
  {
    protected IAsyncConnection _conn;
    private ArasMetadataProvider _metadata;
    protected SqlCompletionHelper _sql;
    
    private string GetType(XmlReader reader)
    {
      var type = reader.GetAttribute("type");
      if (!string.IsNullOrEmpty(type))
        return type;
      var typeId = reader.GetAttribute("typeId");
      ItemType itemType;
      if (!string.IsNullOrEmpty(typeId) && _metadata != null && _metadata.TypeById(typeId, out itemType))
        return itemType.Name;

      return null;
    }

    public IPromise<CompletionContext> GetCompletions(string xml, int caret, string soapAction)
    {
      //var overlap = 0;
      if (string.IsNullOrEmpty(xml)) return Promises.Resolved<CompletionContext>(new CompletionContext());

      var path = new List<AmlNode>();
      var existingAttributes = new HashSet<string>();
      Func<string, bool> notExisting = s => !existingAttributes.Contains(s);
      string attrName = null;
      string value = null;
      bool cdata = false;

      var state = XmlUtils.ProcessFragment(xml.Substring(0, caret), (r, o, st) =>
      {
        switch (r.NodeType)
        {
          case XmlNodeType.Element:
            if (!r.IsEmptyElement)
              path.Add(new AmlNode()
              {
                LocalName = r.LocalName,
                Type = GetType(r),
                Action = r.GetAttribute("action"),
                Condition = r.GetAttribute("condition")
              });

            existingAttributes.Clear();
            break;
          case XmlNodeType.EndElement:
            path.RemoveAt(path.Count - 1);
            break;
          case XmlNodeType.Attribute:
            existingAttributes.Add(r.LocalName);
            if (st == XmlState.Attribute || st == XmlState.AttributeValue)
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

      if (caret > 0 && state == XmlState.Tag && (xml[caret - 1] == '"' || xml[caret - 1] == '\''))
        return Promises.Resolved<CompletionContext>(new CompletionContext() { IsXmlTag = true });

      IPromise<IEnumerable<ICompletionData>> items = null;
      IEnumerable<ICompletionData> appendItems = Enumerable.Empty<ICompletionData>();
      var filter = string.Empty;

      if (path.Count < 1)
      {
        switch (soapAction)
        {
          case "ApplySQL":
            items = Elements("sql");
            break;
          case "ApplyAML":
            items = Elements("AML");
            break;
          case "GetAssignedTasks":
            items = Elements("params");
            break;
          default:
            items = Elements("Item");
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
                items = Attributes(notExisting, "id");
                break;
              case "Task":
                items = Attributes(notExisting, "id", "completed");
                break;
              case "Variable":
                items = Attributes(notExisting, "id");
                break;
              case "Authentication":
                items = Attributes(notExisting, "mode");
                break;
              case "Item":
                switch (soapAction)
                {
                  case "GenerateNewGUIDEx":
                    items = Attributes(notExisting, "quantity");
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
                      , "typeId"
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

                    items = Attributes(notExisting, attributes.ToArray());
                    break;
                }
                break;
              default:
                items = Attributes(notExisting, "condition", "is_null");
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
                  items = AttributeValues("can_add", "can_delete", "can_get", "can_update");
                  break;
                case "doGetItem":
                case "version":
                case "isCriteria":
                case "related_expand":
                case "serverEvents":
                  items = AttributeValues("0", "1");
                  break;
                case "id":
                  var newGuid = new AttributeValueCompletionData()
                  {
                    Text = "(New Guid)",
                    Content = FormatText.ColorText("(New Guid)", Brushes.Purple),
                    Action = () => Guid.NewGuid().ToString("N").ToUpperInvariant()
                  };
                  items = Promises.Resolved(Enumerable.Repeat<ICompletionData>(newGuid, 1));
                  break;
                case "queryType":
                  items = AttributeValues("Effective", "Latest", "Released");
                  break;
                case "orderBy":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    var lastComma = value.LastIndexOf(",");
                    if (lastComma >= 0) value = value.Substring(lastComma + 1).Trim();

                    items = new OrderByPropertyFactory(_metadata, itemType).GetPromise();
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
                      .Continue(it => new SelectPropertyFactory(_metadata, it).GetPromise());
                  }
                  break;
                case "type":
                  if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(path.Last().Type))
                  {
                    items = Promises.Resolved(Enumerable.Repeat<ICompletionData>(new AttributeValueCompletionData()
                    {
                      Text = path.Last().Type
                    }, 1));
                  }
                  else if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _metadata.ItemTypeByName(path[path.Count - 3].Type, out itemType))
                    {
                      items = Promises.Resolved(ItemTypeCompletion<AttributeValueCompletionData>(itemType.Relationships));
                    }
                  }
                  else
                  {
                    items = Promises.Resolved(ItemTypeCompletion<AttributeValueCompletionData>(_metadata.ItemTypes));
                  }
                  break;
                case "typeId":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    items = Promises.Resolved(Enumerable.Repeat<ICompletionData>(new AttributeValueCompletionData()
                    {
                      Text = itemType.Id
                    }, 1));
                  }
                  else if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _metadata.ItemTypeByName(path[path.Count - 3].Type, out itemType))
                    {
                      items = Promises.Resolved(ItemTypeCompletion<AttributeValueCompletionData>(itemType.Relationships, true));
                    }
                  }
                  else
                  {
                    items = Promises.Resolved(ItemTypeCompletion<AttributeValueCompletionData>(_metadata.ItemTypes, true));
                  }
                  break;
                case "where":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    return _sql.Completions("select * from innovator.[" + itemType.Name.Replace(' ', '_') 
                      + "] where " + value, xml, caret, xml[caret - value.Length - 1].ToString(), true);
                  }
                  break;
              }
            }
            else
            {
              switch (attrName)
              {
                case "condition":
                  items = AttributeValues("between"
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
                  items = AttributeValues("0", "1");
                  break;
              }
            }

            filter = value;
            break;
          default:
            if (path.Any() && state == XmlState.Tag)
              appendItems = new ICompletionData[] { new BasicCompletionData() { Text = "/" + path.Last().LocalName + ">" } };


            if (path.Count == 1 && path.First().LocalName == "AML" && state == XmlState.Tag)
            {
              items = CompletionExtensions.GetPromise<BasicCompletionData>("Item");
            }
            else if (path.Count == 1 && path.First().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase) && soapAction == "ApplySQL")
            {
              return _sql.Completions(value, xml, caret, cdata ? "]]>" : "<");
            }
            else
            {
              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];
              if (state == XmlState.Tag && last.LocalName == "Item")
              {
                switch (last.Action)
                {
                  case "AddHistory":
                    items = Elements("action", "filename", "form_name");
                    break;
                  case "GetItemsForStructureBrowser":
                    items = Elements("Item");
                    break;
                  case "EvaluateActivity":
                    items = Elements("Activity", "ActivityAssignment", "Paths", "DelegateTo"
                      , "Tasks", "Variables", "Authentication", "Comments", "Complete");
                    break;
                  case "instantiateWorkflow":
                    items = Elements("WorkflowMap");
                    break;
                  case "promoteItem":
                    items = Elements("state", "comments");
                    break;
                  case "Run Report":
                    items = Elements("report_name", "AML");
                    break;
                  case "SQL Process":
                    items = Elements("name", "PROCESS", "ARG1", "ARG2", "ARG3", "ARG4"
                      , "ARG5", "ARG6", "ARG7", "ARG8", "ARG9");
                    break;
                  default:
                    // Completions for item properties
                    var buffer = new List<ICompletionData>();

                    buffer.Add(new BasicCompletionData("Relationships") { Image = WpfImages.XmlTag16 } );
                    if (last.Action == "get")
                    {
                      buffer.Add(new BasicCompletionData("and") { Image = WpfImages.Operator16 });
                      buffer.Add(new BasicCompletionData("not") { Image = WpfImages.Operator16 });
                      buffer.Add(new BasicCompletionData("or") { Image = WpfImages.Operator16 });
                    }
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(last.Type)
                      && _metadata.ItemTypeByName(last.Type, out itemType))
                    {
                      items = new PropertyCompletionFactory(_metadata, itemType).GetPromise(buffer);
                    }
                    else
                    {
                      items = Promises.Resolved<IEnumerable<ICompletionData>>(buffer);
                    }
                    break;
                }
              }
              else if (state == XmlState.Tag && last.LocalName == "params" && soapAction == "GetAssignedTasks")
              {
                items = Elements("inBasketViewMode", "workflowTasks", "projectTasks", "actionTasks", "userID");
              }
              else if (state == XmlState.Tag && last.LocalName == "Paths" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = Elements("Path");
              }
              else if (state == XmlState.Tag && last.LocalName == "Tasks" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = Elements("Task");
              }
              else if (state == XmlState.Tag && last.LocalName == "Variables" && path.Count > 1 && path[path.Count - 2].Action == "EvaluateActivity")
              {
                items = Elements("Variable");
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
                    if (lastItem.Action == "Run Report" && path.Last().LocalName.Equals("AML"))
                    {
                      items = Elements("Item");
                    }
                    else if (!string.IsNullOrEmpty(lastItem.Type)
                      && _metadata.ItemTypeByName(lastItem.Type, out itemType))
                    {
                      items = PropertyValueCompletion(itemType, state, path).ToPromise();
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
        Items = FilterAndSort(i.Concat(appendItems), filter),
        Overlap = (filter ?? "").Length
      });
    }

    private static IPromise<IEnumerable<ICompletionData>> Elements(params string[] values)
    {
      return Promises.Resolved(values.Select(v => (ICompletionData)new BasicCompletionData()
      {
        Text = v,
        Image = WpfImages.XmlTag16
      }));
    }

    private static IPromise<IEnumerable<ICompletionData>> Attributes(Func<string, bool> filter, params string[] values)
    {
      return Promises.Resolved(values.Where(filter).Select(v => (ICompletionData)new AttributeCompletionData()
      {
        Text = v,
        Image = WpfImages.Attribute16
      }));
    }
    private static IPromise<IEnumerable<ICompletionData>> AttributeValues(params string[] values)
    {
      return Promises.Resolved(values.Select(v => (ICompletionData)new AttributeValueCompletionData()
      {
        Text = v,
        Image = WpfImages.EnumValue16
      }));
    }

    private IEnumerable<ICompletionData> ItemTypeCompletion<T>(IEnumerable<ItemType> itemTypes, bool insertId = false) where T : BasicCompletionData, new()
    {
      return itemTypes.Select(i =>
      {
        var result = new T();
        result.Text = i.Name;
        result.Image = WpfImages.Class16;
        if (insertId) result.Action = () => i.Id;
        if (!string.IsNullOrWhiteSpace(i.Label)) result.Description = i.Label;
        return result;
      }).Concat(itemTypes.Where(i => !string.IsNullOrWhiteSpace(i.Label) &&
                                     !string.Equals(i.Name, i.Label, StringComparison.OrdinalIgnoreCase))
          .Select(i =>
          {
            var result = new T();
            result.Image = WpfImages.Class16;
            result.Text = i.Label;
            result.Description = i.Name;
            result.Content = FormatText.MutedText(i.Label);
            if (insertId)
              result.Action = () => i.Id;
            else
              result.Action = () => i.Name;
            return result;
          }));
    }

    private async Task<IEnumerable<ICompletionData>> PropertyValueCompletion(ItemType itemType, XmlState state, IList<AmlNode> path)
    {
      var p = await _metadata.GetProperty(itemType, path.Last().LocalName).ToTask();

      if (p.Type == PropertyType.item && p.Restrictions.Any())
      {
        var completions = p.Restrictions
          .Select(type => (state != XmlState.Tag ? "<" : "") + "Item type='" + type + "'")
          .GetCompletions<BasicCompletionData>();

        if (p.Restrictions.Any(r => string.Equals(r, "File", StringComparison.OrdinalIgnoreCase)))
        {
          var uploadComplete = new BasicCompletionData()
          {
            Text = "Select file to upload...",
            Content = FormatText.ColorText("Select file to upload...", Brushes.Purple),
            Action = () =>
            {
              using (var dialog = new System.Windows.Forms.OpenFileDialog())
              {
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                  var upload = _conn.CreateUploadCommand();
                  var query = upload.AddFile(Guid.NewGuid().ToString("N").ToUpperInvariant(),
                    dialog.FileName);
                  if (state == XmlState.Tag)
                    query = query.TrimStart('<');
                  return query;
                }
              }
              return string.Empty;
            }
          };
          completions = completions.Concat(Enumerable.Repeat(uploadComplete, 1));
        }

        return completions;
      }
      else if (p.Type == PropertyType.boolean)
      {
        return new string[] { "0", "1" }.GetCompletions<BasicCompletionData>();
      }
      else if (p.Type == PropertyType.list && !string.IsNullOrEmpty(p.DataSource))
      {
        var values = await _metadata.ListValues(p.DataSource).ToTask();

        var hash = new HashSet<string>(values.Select(v => v.Value), StringComparer.CurrentCultureIgnoreCase);
        return values
          .Select(v => v.Value)
          .GetCompletions<BasicCompletionData>()
          .Concat(values.Where(v => !string.IsNullOrWhiteSpace(v.Label) && !hash.Contains(v.Label))
                  .Select(v => new BasicCompletionData() {
                    Text = v.Label + " (" + v.Value + ")",
                    Description = v.Value,
                    Content = FormatText.MutedText(v.Label + " (" + v.Value + ")"),
                    Action = () => v.Value
                  }));
      }
      else if (p.Name == "classification")
      {
        var paths = await _metadata.GetClassPaths(itemType).ToTask();
        return paths.GetCompletions<BasicCompletionData>();
      }
      else if (p.Name == "name" && itemType.Name == "Method" && path[path.Count - 2].Action == "get")
      {
        return _metadata.MethodNames.GetCompletions<BasicCompletionData>();
      }
      else if (p.Name == "name" && itemType.Name == "ItemType" && path[path.Count - 2].Action == "get")
      {
        return ItemTypeCompletion<BasicCompletionData>(_metadata.ItemTypes);
      }
      else
      {
        return Enumerable.Empty<ICompletionData>();
      }
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

      XmlUtils.ProcessFragment(xml, (r, o, st) =>
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
              if (depth == 0 && offset >= start && offset < end)
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
      _isInitialized = true;
    }

    public string LastOpenTag(string xml)
    {
      bool isOpenTag = false;
      var path = new List<AmlNode>();

      var state = XmlUtils.ProcessFragment(xml, (r, o, st) =>
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
          case XmlNodeType.Attribute:
            // Do nothing
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
      return values
        .Where(i => string.IsNullOrEmpty(substring)
          || i.Text.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0)
        .OrderBy(i => (string.IsNullOrEmpty(substring)
          || i.Text.StartsWith(substring, StringComparison.CurrentCultureIgnoreCase)) ? 0 : 1)
        .ThenBy(i => i.Text.StartsWith("/") ? 1 : 0)
        .ThenBy(i => i.Text)
        .ToArray();
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
        .Select(i => "innovator.[" + i.Name.Replace(' ', '_') + "]")
        .Concat(_metadata.Sqls()
          .Where(s => string.Equals(s.Type, "type", StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Type, "view", StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Type, "function", StringComparison.OrdinalIgnoreCase))
          .Select(s => "innovator.[" + s.KeyedName + "]"));
    }

    public IPromise<IEnumerable<IListValue>> GetColumnNames(string tableName)
    {
      ItemType itemType;
      if (tableName.StartsWith("innovator.", StringComparison.OrdinalIgnoreCase))
        tableName = tableName.Substring(10);

      if (_metadata.ItemTypeByName(tableName.Replace('_', ' '), out itemType))
        return _metadata.GetProperties(itemType).Convert(p => p.OfType<IListValue>());

      return Promises.Resolved(Enumerable.Empty<IListValue>());
    }


    private class SelectPropertyFactory : PropertyCompletionFactory
    {
      public SelectPropertyFactory(ArasMetadataProvider metadata, ItemType itemType) :
        base(metadata, itemType) { }

      protected override BasicCompletionData CreateCompletion()
      {
        return new AttributeValueCompletionData() { MultiValue = true };
      }
    }

    private class OrderByPropertyFactory : PropertyCompletionFactory
    {
      public OrderByPropertyFactory(ArasMetadataProvider metadata, ItemType itemType) :
        base(metadata, itemType) { }

      protected override BasicCompletionData CreateCompletion()
      {
        return new AttributeValueCompletionData() { MultiValue = true };
      }

      protected override IEnumerable<ICompletionData> GetCompletions(IEnumerable<IListValue> normal, IEnumerable<IListValue> byLabel)
      {
        return base.GetCompletions(normal, byLabel)
          .Concat(normal.Select(i =>
          {
            var data = ConfigureNormalProperty(CreateCompletion(), i);
            data.Text += " DESC";
            data.Description += " DESC";
            return data;
          }))
          .Concat(byLabel.Select(i =>
          {
            var data = ConfigureLabeledProperty(CreateCompletion(), i);
            data.Text += " DESC";
            data.Description += " DESC";
            data.Content = FormatText.MutedText(data.Text);
            data.Action = () => i.Value + " DESC";
            return data;
          }));
      }
    }

    private class WherePropertyFactory : PropertyCompletionFactory
    {
      public WherePropertyFactory(ArasMetadataProvider metadata, ItemType itemType) :
        base(metadata, itemType) { }

      protected override BasicCompletionData CreateCompletion()
      {
        return new AttributeValueCompletionData() { MultiValue = true };
      }
      protected override BasicCompletionData ConfigureNormalProperty(BasicCompletionData data, IListValue prop)
      {
        var res = base.ConfigureNormalProperty(data, prop);
        res.Action = () => "[" + _itemType.Name.Replace(' ', '_') + "].[" + prop.Value + "]";
        return res;
      }
    }

    public IEnumerable<string> GetFunctionNames(bool tableValued)
    {
      return Enumerable.Empty<string>();
    }
  }
}
