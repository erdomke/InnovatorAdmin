using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using Innovator.Client;
using InnovatorAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public partial class AmlEditorHelper : ISqlMetadataProvider
  {
    private ArasMetadataProvider _metadata;
    private XmlCompletionDataProvider _xsltProvider;
    protected SqlCompletionHelper _sql;
    private static HashSet<string> _systemControlledProperties = new HashSet<string>(new string[]
    {
      "config_id",
      "created_by_id",
      "created_on",
      "current_state",
      "state",
      "generation",
      "is_current",
      "is_released",
      "keyed_name",
      "modified_by_id",
      "modified_on",
      "new_version",
      "not_lockable",
      "state"
    });

    private XmlCompletionDataProvider XsltProvider
    {
      get
      {
        if (_xsltProvider == null)
        {
          var schema = new System.Xml.Schema.XmlSchema();
          using (var stream = typeof(XmlCompletionDataProvider).Assembly.GetManifestResourceStream("InnovatorAdmin.Editor.xslt.xsd"))
          {
            using (var reader = XmlReader.Create(stream))
            {
              var handler = new System.Xml.Schema.ValidationEventHandler((s, e) => { });
              schema = System.Xml.Schema.XmlSchema.Read(reader, handler);
              schema.Compile(handler);
            }
          }

          _xsltProvider = new XmlCompletionDataProvider(schema);
        }
        return _xsltProvider;
      }
    }

    private string GetType(XmlReader reader)
    {
      var type = reader.GetAttribute("type");
      if (!string.IsNullOrEmpty(type))
        return type;
      var typeId = reader.GetAttribute("typeId");
      ItemType itemType;
      if (!string.IsNullOrEmpty(typeId) && _metadata != null && _metadata.ItemTypeById(typeId, out itemType))
        return itemType.Name;

      return null;
    }

    public async Task<CompletionContext> GetCompletions(ITextSource xml, int caret, string soapAction)
    {
      //var overlap = 0;
      if (xml.TextLength < 1) return new CompletionContext();

      var path = new List<AmlNode>();
      var existingAttributes = new HashSet<string>();
      Func<string, bool> notExisting = s => !existingAttributes.Contains(s);
      string attrName = null;
      string value = null;
      bool cdata = false;
      var paramNames = new HashSet<string>();

      var reader = new XmlFragmentReader(xml.CreateReader(0, caret));

      try
      {
        while (reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
              var elemName = reader.LocalName;
              if (!reader.IsEmptyElement)
                path.Add(new AmlNode()
                {
                  Offset = reader.Offset,
                  LocalName = reader.LocalName,
                  Type = GetType(reader),
                  Action = reader.GetAttribute("action"),
                  Condition = reader.GetAttribute("condition"),
                  Id = reader.GetAttribute("id")
                });

              attrName = null;
              value = null;
              existingAttributes.Clear();
              for (var i = 0; i < reader.AttributeCount; i++)
              {
                reader.MoveToAttribute(i);
                existingAttributes.Add(reader.LocalName);
                if (reader.EndState == XmlState.Attribute || reader.EndState == XmlState.AttributeValue)
                  attrName = reader.LocalName;
                value = reader.Value;
                if (elemName == "Param")
                {
                  if (reader.LocalName == "name")
                  {
                    paramNames.Add("@" + reader.Value.TrimStart('@'));
                  }
                }
                else if (reader.Value.StartsWith("@") && reader.LocalName != "match")
                {
                  paramNames.Add(reader.Value.TrimEnd('!'));
                }
              }
              break;
            case XmlNodeType.EndElement:
              path.RemoveAt(path.Count - 1);
              attrName = null;
              value = null;
              existingAttributes.Clear();
              break;
            case XmlNodeType.CDATA:
              cdata = true;
              value = reader.Value;
              break;
            case XmlNodeType.Text:
              cdata = false;
              value = reader.Value;
              if (!string.IsNullOrWhiteSpace(reader.Value) && reader.Value.Length <= 128 && path.Count > 1
                && path.Last().LocalName != "Item")
              {
                var lastItem = path.LastOrDefault(n => n.LocalName == "Item");
                if (lastItem != null)
                {
                  lastItem.Values[path.Last().LocalName] = reader.Value;
                }
              }
              if (reader.Value.StartsWith("@"))
              {
                paramNames.Add(reader.Value.TrimEnd('!'));
              }
              break;
          }
        }
      }
      catch (XmlException) { }

      // Bail within comments to avoid conflicting with typing
      var state = reader.EndState;
      if (state == XmlState.Comment)
        return new CompletionContext();
      if (caret > 0 && state == XmlState.Tag && (xml.GetCharAt(caret - 1) == '"' || xml.GetCharAt(caret - 1) == '\''))
        return new CompletionContext() { IsXmlTag = true };

      IEnumerable<ICompletionData> items = null;
      var appendItems = Enumerable.Empty<ICompletionData>();
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
          case ArasEditorProxy.UnitTestAction:
            items = Elements("TestSuite");
            break;
          case "RebuildKeyedName":
            items = Elements("ItemTypes");
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
              case "TestSuite":
              case "Tests":
              case "Init":
              case "Cleanup":
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
              case "Param":
                items = Attributes(notExisting, "name", "select");
                break;
              case "Remove":
                items = Attributes(notExisting, "match");
                break;
              case "AssertMatch":
                items = Attributes(notExisting, "match", "removeSysProps");
                break;
              case "Test":
                items = Attributes(notExisting, "name");
                break;
              case "Login":
                items = Attributes(notExisting, "database", "url", "password", "username", "type");
                break;
              case "Delay":
                items = Attributes(notExisting, "from", "by");
                break;
              case "actual_data":
                var lastItem = path.LastOrDefault(p => p.LocalName == "Item");
                if (lastItem != null && lastItem.Type == "File")
                {
                  items = Attributes(notExisting, "encoding");
                }
                else
                {
                  items = Attributes(notExisting, "condition", "is_null");
                }
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
                    var attributes = new HashSet<string>
                    {
                      "action"
                      , "doGetItem"
                      , "id"
                      , "idlist"
                      , "serverEvents"
                      , "type"
                      , "typeId"
                      , "where"
                    };

                    switch (path.Last().Action)
                    {
                      case "copy":
                        attributes.UnionWith(new string[]
                        {
                          "do_add",
                          "do_lock"
                        });
                        break;
                      case "copyAsIs":
                      case "copyAsNew":
                        attributes.UnionWith(new string[]
                        {
                          "do_lock",
                          "lock_related",
                          "useInputProperties",
                        });
                        break;
                      case "create":
                      case "get":
                      case "getItemAllVersions":
                      case "GetItemConfig":
                      case "getItemLastVersion":
                      case "getItemRelationships":
                      case "GetItemRepeatConfig":
                      case "recache":
                        attributes.UnionWith(new string[]
                        {
                          "config_path"
                          , "expand"
                          , "isCriteria"
                          , "language"
                          , "levels"
                          , "maxRecords"
                          , "page"
                          , "pagesize"
                          , "orderBy"
                          , "queryDate"
                          , "queryType"
                          , "relas_only"
                          , "related_expand"
                          , "returnMode"
                          , "select"
                          , "stdProps"
                        });
                        if (path.Last().Action == "create")
                          attributes.Add("do_skipOnAfterAdd");
                        break;
                      case "getPermissions":
                        attributes.Add("access_type");
                        break;
                      case "add":
                      case "AddItem":
                        attributes.Add("do_skipOnAfterAdd");
                        break;
                      case "edit":
                      case "update":
                        attributes.Add("version");
                        attributes.Add("unlock");
                        break;
                      case "merge":
                        attributes.Add("do_skipOnAfterAdd");
                        attributes.Add("version");
                        attributes.Add("unlock");
                        break;
                    }

                    if (path.Count >= 3
                      && path[path.Count - 2].LocalName == "Relationships"
                      && path[path.Count - 3].LocalName == "Item"
                      && path[path.Count - 3].Action == "GetItemRepeatConfig")
                    {
                      attributes.Add("repeatProp");
                      attributes.Add("repeatTimes");
                    }

                    items = Attributes(notExisting, attributes.ToArray()).ToArray();
                    foreach (var item in items.OfType<AttributeCompletionData>().Where(i => i.Text == "where"))
                    {
                      item.QuoteChar = '"';
                    }
                    break;
                }
                break;
              default:
                items = Attributes(notExisting, "condition", "is_null");
                items = items.Concat(new[] {
                  new BasicCompletionData() { Text = "between", Action = () => "condition='between'", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "eq", Action = () => "condition='eq'", Content = "eq (=, Equals)", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "ge", Action = () => "condition='ge'", Content = "ge (>=, Greather than or equal to)", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "gt", Action = () => "condition='gt'", Content = "gt (>, Greather than)", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "in", Action = () => "condition='in'", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "is", Action = () => "condition='is'", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "le", Action = () => "condition='le'", Content = "le (<=, Less than or equal to)", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "like", Action = () => "condition='like'", Description = "Both * and % are wildcards", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "lt", Action = () => "condition='lt'", Content = "lt (<, Less than)", Image = Icons.EnumValue16.Wpf },
                  new BasicCompletionData() { Text = "ne", Action = () => "condition='ne'", Content = "ne (<>, !=, Not Equals)", Image = Icons.EnumValue16.Wpf }
                });
                break;
            }

            filter = attrName;
            break;
          case XmlState.AttributeValue:
            if (path.Last().LocalName == "Item")
            {
              //private enum RelActionType
              //{
              //  add,
              //  delete,
              //  purge,
              //  update,
              //  create,
              //  edit,
              //  merge,
              //  get,
              //  skip,
              //  exclude
              //}

              ItemType itemType;
              switch (attrName)
              {
                case "action":
                  var baseMethods = new string[] {"ActivateActivity"
                    , "add"
                    , "AddHistory"
                    , "AddItem"
                    , "ApplyUpdate"
                    , "BuildProcessReport"
                    , "CancelWorkflow"
                    , "closeWorkflow"
                    , "copy"
                    , "copyAsIs"
                    , "copyAsNew"
                    , "create"
                    , "delete"
                    , "edit"
                    , "EmailItem"
                    , "EvaluateActivity"
                    , "get"
                    , "getAffectedItems"
                    , "GetInheritedServerEvents"
                    , "getItemAllVersions"
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

                  var aras = _conn as Innovator.Client.Connection.IArasConnection;
                  var version = aras == null ? -1 : aras.Version;

                  var methods = (IEnumerable<string>)baseMethods;
                  if (version < 10)
                    methods = methods.Concat(Enumerable.Repeat("checkImportedItemType", 1));
                  if (version < 11)
                    methods = methods.Concat(Enumerable.Repeat("exportItemType", 1));
                  if (version < 0 || version >= 10)
                    methods = methods.Concat(Enumerable.Repeat("VaultServerEvent", 1));
                  if (version < 0 || version >= 11)
                    methods = methods.Concat(new string[] { "GetInheritedServerEvents", "getHistoryItems" });

                  items = _metadata.MethodNames.Select(m => (ICompletionData)new AttributeValueCompletionData()
                  {
                    Text = m,
                    Image = Icons.Method16.Wpf
                  }).Concat(methods.Select(m => (ICompletionData)new AttributeValueCompletionData()
                  {
                    Text = m,
                    Image = Icons.MethodFriend16.Wpf
                  }));
                  break;
                case "access_type":
                  items = AttributeValues("can_add", "can_delete", "can_get", "can_update", "can_discover", "can_change_access");
                  break;
                case "expand":
                case "doGetItem":
                case "do_add":
                case "do_lock":
                case "do_skipOnAfterAdd":
                case "isCriteria":
                case "lock_related":
                case "relas_only":
                case "related_expand":
                case "serverEvents":
                case "stdProps":
                case "unlock":
                case "useInputProperties":
                case "version":
                  items = new ICompletionData[] {
                    new AttributeValueCompletionData() {
                      Text = "0",
                      Image = Icons.EnumValue16.Wpf,
                      Content = "0 (False)"
                    },
                    new AttributeValueCompletionData()
                    {
                      Text = "1",
                      Image = Icons.EnumValue16.Wpf,
                      Content = "1 (True)"
                    }
                  };
                  break;
                case "id":
                  var newGuid = new AttributeValueCompletionData()
                  {
                    Text = "(New Guid)",
                    Content = FormatText.ColorText("(New Guid)", Brushes.Purple),
                    Action = () => Guid.NewGuid().ToString("N").ToUpperInvariant()
                  };
                  items = Enumerable.Repeat<ICompletionData>(newGuid, 1);
                  var last = path.Last();
                  if (last.Action != "add")
                  {
                    switch (last.Type)
                    {
                      case "ItemType":
                        items = items.Concat(ItemTypeCompletion<AttributeValueCompletionData>(_metadata.ItemTypes, true));
                        break;
                      case "Method":
                        items = items.Concat(_metadata.AllMethods.Select(r => new AttributeValueCompletionData()
                        {
                          Text = r.KeyedName,
                          Action = () => r.Unique,
                          Image = Icons.EnumValue16.Wpf
                        }));
                        break;
                      case "SQL":
                        items = items.Concat(_metadata.Sqls().Select(r => new AttributeValueCompletionData()
                        {
                          Text = r.KeyedName,
                          Action = () => r.Unique,
                          Image = Icons.EnumValue16.Wpf
                        }));
                        break;
                      case "Identity":
                        items = items.Concat(_metadata.SystemIdentities.Select(r => new AttributeValueCompletionData()
                        {
                          Text = r.KeyedName,
                          Action = () => r.Unique,
                          Image = Icons.EnumValue16.Wpf
                        }));
                        break;
                      case "Sequence":
                        items = items.Concat(_metadata.Sequences.Select(r => new AttributeValueCompletionData()
                        {
                          Text = r.KeyedName,
                          Action = () => r.Unique,
                          Image = Icons.EnumValue16.Wpf
                        }));
                        break;
                      case "User":
                        items = items.Concat(new ICompletionData[] { new AttributeValueCompletionData() {
                          Action = () => _conn.UserId,
                          Text = "Me",
                          Image = Icons.EnumValue16.Wpf
                        } });
                        break;
                    }
                  }
                  break;
                case "queryDate":
                  items = GetDateCompletions();
                  break;
                case "queryType":
                  items = AttributeValues("Effective", "Latest", "Released");
                  break;
                case "returnMode":
                  items = AttributeValues("itemsOnly", "countOnly");
                  break;
                case "orderBy":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    var lastComma = value.LastIndexOf(",");
                    if (lastComma >= 0) value = value.Substring(lastComma + 1).Trim();

                    items = await new OrderByPropertyFactory(_metadata, itemType).GetPromise().ToTask();
                  }
                  break;
                case "select":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    string partial;
                    var selectPath = SelectPath(value, out partial);
                    value = partial;

                    var it = await RecurseProperties(itemType, selectPath);

                    items = await new SelectPropertyFactory(_metadata, it).GetPromise().ToTask();
                  }
                  break;
                case "type":
                  if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(path.Last().Type))
                  {
                    items = Enumerable.Repeat<ICompletionData>(new AttributeValueCompletionData()
                    {
                      Text = path.Last().Type
                    }, 1);
                  }
                  else if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _metadata.ItemTypeByName(path[path.Count - 3].Type, out itemType))
                    {
                      items = ItemTypeCompletion<AttributeValueCompletionData>(itemType.Relationships);
                    }
                  }
                  else
                  {
                    items = ItemTypeCompletion<AttributeValueCompletionData>(_metadata.ItemTypes);
                  }
                  break;
                case "typeId":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    items = Enumerable.Repeat<ICompletionData>(new AttributeValueCompletionData()
                    {
                      Text = itemType.Id
                    }, 1);
                  }
                  else if (path.Count > 2
                    && path[path.Count - 3].LocalName == "Item"
                    && path[path.Count - 2].LocalName == "Relationships")
                  {
                    if (!string.IsNullOrEmpty(path[path.Count - 3].Type)
                      && _metadata.ItemTypeByName(path[path.Count - 3].Type, out itemType))
                    {
                      items = ItemTypeCompletion<AttributeValueCompletionData>(itemType.Relationships, true);
                    }
                  }
                  else
                  {
                    items = ItemTypeCompletion<AttributeValueCompletionData>(_metadata.ItemTypes, true);
                  }
                  break;
                case "where":
                  if (!string.IsNullOrEmpty(path.Last().Type)
                    && _metadata.ItemTypeByName(path.Last().Type, out itemType))
                  {
                    return await _sql.Completions("select * from innovator.[" + itemType.Name.Replace(' ', '_')
                      + "] where " + value, xml, caret, xml.GetCharAt(caret - value.Length - 1).ToString(), true).ToTask();
                  }
                  break;
              }

              if (items != null && paramNames.Any())
                items = items.Concat(Parameters(paramNames.ToArray()));
            }
            else
            {
              switch (attrName)
              {
                case "type":
                  if (path.Last().LocalName == "Login")
                  {
                    items = AttributeValues("Explicit", "Anonymous", "Windows");
                  }
                  break;
                case "select":
                  if (path.Last().LocalName == "Param")
                  {
                    items = AttributeValues("(//Item)[1]", "x:Database()", "x:FixedNewId()", "x:NewId()", "x:Now()", "x:UserId()");
                  }
                  break;
                case "match":
                  if (path.Last().LocalName == "AssertMatch")
                  {
                    items = AttributeValues("(//Item)[1]");
                  }
                  break;
                case "encoding":
                  items = AttributeValues("none", "base64");
                  break;
                case "condition":
                  items = new ICompletionData[] {
                    new AttributeValueCompletionData() { Text = "between", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "eq", Content = "eq (=, Equals)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "ge", Content = "ge (>=, Greather than or equal to)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "gt", Content = "gt (>, Greather than)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "in", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "is not null", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "is null", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "is", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "le", Content = "le (<=, Less than or equal to)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "like", Description = "Both * and % are wildcards", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "lt", Content = "lt (<, Less than)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "ne", Content = "ne (<>, !=, Not Equals)", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "not between", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "not in", Image = Icons.EnumValue16.Wpf },
                    new AttributeValueCompletionData() { Text = "not like", Image = Icons.EnumValue16.Wpf }
                  };
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
              items = Elements("Item");
            }
            else if (path.Last().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase) && !path.Any(p => p.LocalName == "Item"))
            {
              return await _sql.Completions(value, xml, caret, cdata ? "]]>" : "<").ToTask();
            }
            else if (path.Last().LocalName == "TestSuite")
            {
              items = Elements("Tests", "Init", "Cleanup");
            }
            else if (soapAction == "RebuildKeyedName" && path.Last().LocalName == "ItemTypes")
            {
              items = ItemTypeCompletion<BasicCompletionData>(_metadata.ItemTypes);
            }
            else if (path.Last().LocalName == "Tests")
            {
              items = Elements("Test");
            }
            else if (path.Last().LocalName == "Test")
            {
              items = Elements("AssertMatch", "Param", "Item", "sql", "GetNextSequence", "Login", "Logout", "Delay", "DownloadFile");
            }
            else if (path.Last().LocalName == "Init" || path.Last().LocalName == "Cleanup")
            {
              items = Elements("Param", "Item", "sql", "GetNextSequence", "Login", "Logout", "Delay");
            }
            else if (path.Last().LocalName == "DownloadFile" || path.Last().LocalName == "GetNextSequence")
            {
              items = Elements("Item");
            }
            else if (path.Last().LocalName == "AssertMatch")
            {
              items = Elements("Expected", "Remove");
            }
            else if (path.Last().LocalName == "Expected" || path.Last().LocalName == "Param")
            {
              items = Elements("Item");
            }
            else if (state == XmlState.CData && value.TrimStart().StartsWith("<"))
            {
              var lastItem = path.LastOrDefault(p => p.LocalName == "Item");
              if (path.Last().LocalName == "xsl_stylesheet" && lastItem != null && lastItem.Type == "Report")
              {
                items = XsltProvider.HandleTextEntered(new StringTextSource(value), value.Length, value.Last().ToString());
              }
              else if (lastItem != null && ((path.Last().LocalName == "criteria" && lastItem.Type == "SavedSearch")
                || (path.Last().LocalName == "query_string" && lastItem.Type == "EMail Message")
                || (path.Last().LocalName == "report_query" && lastItem.Type == "Report")))
              {
                items = (await GetCompletions(new StringTextSource(value), value.Length, "ApplyItem")).Items;
              }
            }
            else
            {
              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];
              if (last.LocalName == "Item")
              {
                if (state == XmlState.Tag)
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
                      if (path.Any(n => n.LocalName == "GetNextSequence") || SoapAction == "GetNextSequence")
                      {
                        items = new[] {
                          new BasicCompletionData() {
                            Text = "name",
                            Image = Icons.Property16.Wpf
                          }
                        };
                      }
                      else
                      {
                        // Completions for item properties
                        var buffer = new List<ICompletionData>();

                        buffer.Add(new BasicCompletionData("Relationships") { Image = Icons.XmlTag16.Wpf });
                        if (last.Action == "get")
                        {
                          buffer.Add(new BasicCompletionData("and") { Image = Icons.Operator16.Wpf });
                          buffer.Add(new BasicCompletionData("not") { Image = Icons.Operator16.Wpf });
                          buffer.Add(new BasicCompletionData("or") { Image = Icons.Operator16.Wpf });
                        }
                        ItemType itemType;
                        if (!string.IsNullOrEmpty(last.Type)
                          && _metadata.ItemTypeByName(last.Type, out itemType))
                        {
                          switch (last.Action)
                          {
                            case "add":
                            case "create":
                            case "edit":
                            case "update":
                            case "merge":
                              items = await new PropertyCompletionFactory(_metadata, itemType)
                              {
                                Filter = p => !_systemControlledProperties.Contains(p.Name)
                              }.GetPromise(buffer).ToTask();
                              break;
                            default:
                              items = await new PropertyCompletionFactory(_metadata, itemType).GetPromise(buffer).ToTask();
                              break;
                          }
                          if (itemType.Name == "File")
                          {
                            items = items.Concat(new[] {
                              new BasicCompletionData() {
                                Text = "actual_filename",
                                Image = Icons.Property16.Wpf
                              },
                              new BasicCompletionData() {
                                Text = "actual_data",
                                Image = Icons.Property16.Wpf
                              }
                            });
                          }
                        }
                        else
                        {
                          items = buffer;
                        }
                      }
                      break;
                  }
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
                    items = Elements("Item");
                  }
                  else if (path.Last().Condition == "in"
                    || path.Last().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase))
                  {
                    return await _sql.Completions(value, xml, caret, cdata ? "]]>" : "<").ToTask();
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
                      items = await PropertyValueCompletion(itemType, state, lastItem, path).ToPromise().ToTask();
                      if (items != null && paramNames.Any())
                        items = items.Concat(Parameters(paramNames.ToArray()));
                    }
                    else if (path.Last().LocalName == "name" && (SoapAction == "GetNextSequence" || path.Any(n => n.LocalName == "GetNextSequence")))
                    {
                      items = _metadata.Sequences.Select(s => s.KeyedName).GetCompletions<BasicCompletionData>();
                    }
                  }
                }
              }
            }

            break;
        }
      }

      if (items == null)
        return new CompletionContext();

      if (items.Any(i => i.Text == "Item"))
      {
        items = items.Concat(new AttributeCompletionData[] {
          new AttributeCompletionData()
          {
            Text = "Item action='get'",
            Action = () => "Item action='get' type",
            Image = Icons.Attribute16.Wpf
          }
        });
      }

      return new CompletionContext()
      {
        Items = FilterAndSort(items.Concat(appendItems), filter),
        Overlap = (filter ?? "").Length
      };
    }

    private static IEnumerable<ICompletionData> Parameters(params string[] values)
    {
      return values.Select(v => (ICompletionData)new BasicCompletionData()
      {
        Text = v,
        Image = Icons.Attribute16.Wpf
      });
    }

    private static IEnumerable<ICompletionData> Elements(params string[] values)
    {
      return values.Select(v => (ICompletionData)new BasicCompletionData()
      {
        Text = v,
        Image = Icons.XmlTag16.Wpf
      });
    }

    private static IEnumerable<ICompletionData> Attributes(Func<string, bool> filter, params string[] values)
    {
      return values.Where(filter).Select(v => (ICompletionData)new AttributeCompletionData()
      {
        Text = v,
        Image = Icons.Field16.Wpf
      });
    }
    private static IEnumerable<ICompletionData> AttributeValues(params string[] values)
    {
      return values.Select(v => (ICompletionData)new AttributeValueCompletionData()
      {
        Text = v,
        Image = Icons.EnumValue16.Wpf
      });
    }

    private IEnumerable<ICompletionData> ItemTypeCompletion<T>(IEnumerable<ItemType> itemTypes, bool insertId = false) where T : BasicCompletionData, new()
    {
      return itemTypes.Select(i =>
      {
        var result = new T();
        result.Text = i.Name;
        result.Image = Icons.Class16.Wpf;
        if (insertId) result.Action = () => i.Id;
        if (!string.IsNullOrWhiteSpace(i.Label)) result.Description = i.Label;
        return result;
      }).Concat(itemTypes.Where(i => !string.IsNullOrWhiteSpace(i.Label) &&
                                     !string.Equals(i.Name, i.Label, StringComparison.OrdinalIgnoreCase))
          .Select(i =>
          {
            var result = new T();
            result.Image = Icons.Class16.Wpf;
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

    private static ICompletionData[] _boolCompletions = new ICompletionData[] {
      new BasicCompletionData() {
        Text = "0",
        Content = "0 (False)"
      },
      new BasicCompletionData()
      {
        Text = "1",
        Content = "1 (True)"
      }
    };

    private async Task<IEnumerable<ICompletionData>> PropertyValueCompletion(ItemType itemType, XmlState state, AmlNode lastItem, IList<AmlNode> path)
    {
      string itemValue;

      if (lastItem.Action == "EvaluateActivity")
      {
        var lastName = path.Last().LocalName;
        if (lastName == "Activity")
        {
          return new ICompletionData[]
          {
            new BasicCompletionData()
            {
              Text = "Select context item...",
              Content = FormatText.ColorText("Select context item...", Brushes.Purple),
              Action = () =>
              {
                var items = EditorWindow.GetItems(_conn, "<Item action='get'", 18).ToArray();
                if (items.Length != 1)
                  return string.Empty;

                var activities = _conn.Apply(@"<Item type='Activity' action='get' select='id'>
                                                  <id condition='in'>(select act.id
                                                from innovator.[Workflow] w
                                                inner join innovator.[Workflow_Process] wp
                                                on wp.id = w.related_id
                                                inner join innovator.[Workflow_Process_Activity] wpa
                                                on wpa.source_id = w.related_id
                                                inner join innovator.[Activity] act
                                                on act.id = wpa.related_id
                                                where w.source_id = @0
                                                and wp.state = 'Active'
                                                and wp.locked_by_id is null
                                                and act.state = 'Active'
                                                and act.is_auto = '0')</id>
                                                </Item>", items[0].Unique).Items().ToArray();
                if (activities.Length != 1)
                  return string.Empty;

                return activities[0].Id() + "<!-- " + activities[0].Property("id").KeyedName().Value + " for " + items[0].KeyedName + "-->";
              }
            }
          };
        }
        else if (lastName == "ActivityAssignment" && lastItem.Values.TryGetValue("Activity", out itemValue))
        {
          var assn = (await _conn.ApplyAsync(@"<Item type='Activity Assignment' action='get' select='id,related_id' related_expand='0'>
                                                <is_disabled>0</is_disabled>
                                                <closed_on condition='is null'></closed_on>
                                                <source_id>@0</source_id>
                                              </Item>", true, false, itemValue).ToTask()).Items().ToArray();
          if (assn.Length == 1)
            return Enumerable.Repeat(new BasicCompletionData()
            {
              Text = assn[0].Id() + "<!-- " + assn[0].RelatedId().KeyedName().Value + " -->",
              Image = Icons.EnumValue16.Wpf,
              Content = assn[0].RelatedId().KeyedName().Value
            }, 1);
          return Enumerable.Empty<ICompletionData>();
        }
        else if (lastName == "Paths" && lastItem.Values.TryGetValue("Activity", out itemValue))
        {
          var act = (await _conn.ApplyAsync(@"<Item type='Activity' action='get' select='can_delegate,can_refuse' id='@0'>
                                              <Relationships>
                                                <Item type='Workflow Process Path' action='get' select='id,name,label,is_default'>
                                                </Item>
                                              </Relationships>
                                            </Item>", true, false, itemValue).ToTask()).Items().ToArray();
          if (act.Length != 1)
            return Enumerable.Empty<ICompletionData>();

          var defaultPath = act[0].Relationships("Workflow Process Path")
            .FirstOrNullItem(i => i.Property("is_default").AsBoolean(false))
            ?? act[0].Relationships("Workflow Process Path").FirstOrNullItem();

          var paths = new List<ICompletionData>();
          if (act[0].Property("can_delegate").AsBoolean(true))
          {
            paths.Add(new BasicCompletionData()
            {
              Text = "<Path id='" + defaultPath.Id() + "'>Delegate</Path>",
              Image = Icons.XmlTag16.Wpf,
              Content = "Delegate"
            });
          }
          if (act[0].Property("can_refuse").AsBoolean(true))
          {
            paths.Add(new BasicCompletionData()
            {
              Text = "<Path id='" + defaultPath.Id() + "'>Refuse</Path>",
              Image = Icons.XmlTag16.Wpf,
              Content = "Refuse"
            });
          }
          paths.AddRange(act[0].Relationships("Workflow Process Path").Select(p => new BasicCompletionData()
          {
            Text = "<Path id='" + p.Id() + "'>" + p.Property("label").AsString(p.Property("name").Value) + "</Path>",
            Image = Icons.XmlTag16.Wpf,
            Content = p.Property("label").AsString(p.Property("name").Value)
          }));
          return paths;
        }
        else if (lastName == "Complete")
        {
          return _boolCompletions;
        }
        else
        {
          return Enumerable.Empty<ICompletionData>();
        }
      }
      else
      {
        var p = await _metadata.GetProperty(itemType, path.Last().LocalName).ToTask();

        if (p.Type == PropertyType.item && p.Restrictions.Any())
        {
          var completions = p.Restrictions
            .Select(type => (ICompletionData)new BasicCompletionData()
            {
              Text = (state != XmlState.Tag ? "<" : "") + "Item type='" + type + "'",
              Image = Icons.XmlTag16.Wpf
            });

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
          else
          {
            completions = completions.Concat(Enumerable.Repeat(new ItemPropertyCompletionData(_conn, path, p), 1));
          }

          if (p.Restrictions.Any(r => string.Equals(r, "ItemType", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(ItemTypeCompletion<BasicCompletionData>(_metadata.ItemTypes, true));
          }
          if (p.Restrictions.Any(r => string.Equals(r, "Method", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(_metadata.AllMethods.Select(m => new BasicCompletionData()
            {
              Text = m.KeyedName,
              Action = () => m.Unique,
              Image = Icons.Method16.Wpf
            }));
          }
          if (p.Restrictions.Any(r => string.Equals(r, "SQL", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(_metadata.Sqls().Select(m => new BasicCompletionData()
            {
              Text = m.KeyedName,
              Action = () => m.Unique,
              Image = Icons.EnumValue16.Wpf
            }));
          }
          if (p.Restrictions.Any(r => string.Equals(r, "Identity", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(_metadata.SystemIdentities.Select(m => new BasicCompletionData()
            {
              Text = m.KeyedName,
              Action = () => m.Unique,
              Image = Icons.EnumValue16.Wpf
            }));
          }
          if (p.Restrictions.Any(r => string.Equals(r, "User", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(new ICompletionData[] { new BasicCompletionData() {
              Action = () => _conn.UserId,
              Text = "Me",
              Image = Icons.EnumValue16.Wpf
            } });
          }
          if (p.Restrictions.Any(r => string.Equals(r, "Sequence", StringComparison.OrdinalIgnoreCase)))
          {
            completions = completions.Concat(_metadata.Sequences.Select(m => new BasicCompletionData()
            {
              Text = m.KeyedName,
              Action = () => m.Unique,
              Image = Icons.EnumValue16.Wpf
            }));
          }

          return completions;
        }
        else if (p.Type == PropertyType.date)
        {
          return GetDateCompletions();
        }
        else if (p.Type == PropertyType.boolean)
        {
          return _boolCompletions;
        }
        else if (p.Type == PropertyType.list && !string.IsNullOrEmpty(p.DataSource))
        {
          var values = await _metadata.ListValues(p.DataSource).ToTask();

          var hash = new HashSet<string>(values.Select(v => v.Value), StringComparer.CurrentCultureIgnoreCase);
          return values
            .Select(v => new BasicCompletionData()
            {
              Text = v.Value,
              Image = Icons.EnumValue16.Wpf
            })
            .Concat(values.Where(v => !string.IsNullOrWhiteSpace(v.Label) && !hash.Contains(v.Label))
                    .Select(v => new BasicCompletionData()
                    {
                      Text = v.Label + " (" + v.Value + ")",
                      Image = Icons.EnumValue16.Wpf,
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
        else if (p.Name == "name" && itemType.Name == "Property"
          && lastItem.Values.TryGetValue("label", out itemValue)
          && IsUpdateAction(lastItem.Action))
        {
          var output = new char[itemValue.Length];
          var o = 0;
          for (var i = 0; i < itemValue.Length; i++)
          {
            if (char.IsLetterOrDigit(itemValue[i]))
            {
              output[o] = char.ToLowerInvariant(itemValue[i]);
              o++;
            }
            else if (o == 0 || output[o - 1] != '_')
            {
              output[o] = '_';
              o++;
            }
          }
          if (output[o - 1] == '_') o--;
          return Enumerable.Repeat(new string(output, 0, Math.Min(o, 32)), 1).GetCompletions<BasicCompletionData>();
        }
        else if ((itemType.Name == "Value" || itemType.Name == "Filter Value")
          && ((p.Name == "value" && lastItem.Values.TryGetValue("label", out itemValue))
            || (p.Name == "label" && lastItem.Values.TryGetValue("value", out itemValue)))
          && IsUpdateAction(lastItem.Action))
        {
          return new string[] { itemValue }.GetCompletions<BasicCompletionData>();
        }
        else if ((p.Name == "name" || p.Name == "keyed_name") && itemType.Name == "Property" && lastItem.Action == "get"
          && lastItem.Values.TryGetValue("source_id", out itemValue))
        {
          var parentType = _metadata.ItemTypeById(itemValue);
          return await new PropertyCompletionFactory(_metadata, parentType).GetPromise().ToTask();
        }
        else if (p.Name == "class_path" && itemType.Name == "Property"
          && lastItem.Values.TryGetValue("source_id", out itemValue))
        {
          var parentType = _metadata.ItemTypeById(itemValue);
          var paths = await _metadata.GetClassPaths(parentType).ToTask();
          return paths.GetCompletions<BasicCompletionData>();
        }
        else if ((p.Name == "name" || p.Name == "keyed_name") && itemType.Name == "Method" && lastItem.Action == "get")
        {
          return _metadata.MethodNames.GetCompletions<BasicCompletionData>();
        }
        else if ((p.Name == "name" || p.Name == "keyed_name") && itemType.Name == "ItemType" && lastItem.Action == "get")
        {
          return ItemTypeCompletion<BasicCompletionData>(_metadata.ItemTypes);
        }
        else if ((p.Name == "name" || p.Name == "keyed_name") && itemType.Name == "SQL" && lastItem.Action == "get")
        {
          return _metadata.Sqls().Select(s => new BasicCompletionData()
          {
            Text = s.KeyedName,
            Image = Icons.EnumValue16.Wpf
          });
        }
        else if (p.Name == "state" && lastItem.Action == "promoteItem"
          && !string.IsNullOrEmpty(lastItem.Type) && !string.IsNullOrEmpty(lastItem.Id))
        {
          var stateResult = await _conn.ApplyAsync(@"<Item type='@0' action='getItemNextStates' id='@1'></Item>"
            , true, false, lastItem.Type, lastItem.Id).ToTask();
          var states = stateResult.Items().Select(i => i.Property("to_state").AsItem().Property("name").Value).ToArray();
          return states.GetCompletions<BasicCompletionData>();
        }
        else if (p.Name == "state" && !string.IsNullOrEmpty(lastItem.Type))
        {
          var states = await _metadata.ItemTypeStates(itemType);
          return states.GetCompletions<BasicCompletionData>();
        }
        else
        {
          return Enumerable.Empty<ICompletionData>();
        }
      }
    }

    private bool IsUpdateAction(string action)
    {
      return action == "add" || action == "create"
          || action == "edit" || action == "merge";
    }

    private IEnumerable<ICompletionData> GetDateCompletions()
    {
      return new ICompletionData[] {
        new BasicCompletionData() {
          Text = DateTime.Now.ToString("s"),
          Image = Icons.EnumValue16.Wpf,
          Content = DateTime.Now.ToString("s") + " (Now)"
        }
        // Add a completion dialog option to open a calendar control
      };
    }

    private class ItemPropertyCompletionData : ICompletionData
    {
      private IAsyncConnection _conn;
      private IList<AmlNode> _path;
      private Property _prop;

      public ItemPropertyCompletionData(IAsyncConnection conn, IList<AmlNode> path, Property prop)
      {
        _conn = conn;
        _path = path;
        _prop = prop;
      }

      public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs)
      {
        var item = _path.LastOrDefault(n => n.LocalName == "Item");
        if (item == null)
          return;

        var query = string.Format("<Item type='{0}' action='get'><keyed_name condition='like'>**</keyed_name></Item>", _prop.Restrictions.First());
        var items = EditorWindow.GetItems(_conn, query, query.Length - 21);
        if (items.Any(i => _prop.Restrictions.Contains(i.Type)))
        {
          var allItems = items.Where(i => _prop.Restrictions.Contains(i.Type)).ToArray();
          if (item.Action == "add" || item.Action == "create")
          {
            if (allItems.Length == 1)
            {
              PerformComplete(textArea, completionSegment, allItems);
            }
            else
            {
              var start = item.Offset;
              var doc = textArea.Document;
              while (start > 0 && (doc.GetCharAt(start - 1) == '\t' || doc.GetCharAt(start - 1) == ' '))
                start--;
              var begin = start;
              var end = completionSegment.Offset;
              while (doc.GetCharAt(end) != '>')
                end--;
              var prefix = doc.GetText(start, end - start);
              start = completionSegment.EndOffset;
              end = doc.IndexOf("</Item>", completionSegment.EndOffset, doc.TextLength - completionSegment.EndOffset, StringComparison.Ordinal);
              end = end < 0 ? doc.TextLength : end + 7;
              var suffix = doc.GetText(start, end - start);
              var text = allItems
                .Select(i => prefix + " type='" + i.Type + "' keyed_name='" + i.KeyedName + "'>" + i.Unique + suffix)
                .GroupConcat(Environment.NewLine);
              doc.Replace(begin, end - begin, text);
            }
          }
          else if (item.Action == "merge" || item.Action == "edit" || item.Action == "update")
          {
            if (allItems.Length > 1)
              throw new Exception("Can only set a property value to a single item");
            PerformComplete(textArea, completionSegment, allItems);
          }
          else
          {
            PerformComplete(textArea, completionSegment, allItems);
          }
        }
      }

      private void PerformComplete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, ItemReference[] allItems)
      {
        var start = completionSegment.Offset;
        var end = completionSegment.EndOffset;
        var doc = textArea.Document;
        while (doc.GetCharAt(start) != '>')
          start--;

        if (allItems.Length == 1)
        {
          doc.Replace(start, end - start, " type='" + allItems[0].Type + "' keyed_name='" + allItems[0].KeyedName + "'>" + allItems[0].Unique);
        }
        else
        {
          doc.Replace(start, end - start, " condition='in'>'" + allItems.GroupConcat("','", i => i.Unique) + "'");
        }
      }

      public object Content
      {
        get { return FormatText.ColorText(this.Text, Brushes.Purple); }
      }

      public object Description
      {
        get { return null; }
      }

      public ImageSource Image
      {
        get { return null; }
      }

      public double Priority
      {
        get { return 0; }
      }

      public string Text
      {
        get { return "Search for item(s).."; }
      }
    }

    private async Task<ItemType> RecurseProperties(ItemType itemType, IEnumerable<string> remainingPath)
    {
      if (remainingPath.Any())
      {
        try
        {
          var currProp = await _metadata.GetProperty(itemType, remainingPath.First()).ToTask();
          ItemType it;
          if (currProp.Type == PropertyType.item
            && currProp.Restrictions.Any()
            && _metadata.ItemTypeByName(currProp.Restrictions.First(), out it))
          {
            return await RecurseProperties(it, remainingPath.Skip(1));
          }
          else
          {
            return itemType;
          }
        }
        catch (Exception ex)
        {
          return itemType;
        }
      }
      else
      {
        return itemType;
      }
    }

    public virtual void InitializeConnection(IAsyncConnection conn, InnovatorAdmin.Connections.ConnectionData connData)
    {
      _conn = conn;
      _connData = connData;
      _metadata = ArasMetadataProvider.Cached(conn);
      _isInitialized = true;
    }

    public string LastOpenTag(ITextSource xml)
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
              path.Add(new AmlNode()
              {
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
      private Dictionary<string, string> _values = new Dictionary<string, string>();

      public int Offset { get; set; }
      public string LocalName { get; set; }
      public string Type { get; set; }
      public string Id { get; set; }
      public string Action { get; set; }
      public string Condition { get; set; }
      public Dictionary<string, string> Values { get { return _values; } }
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
        base(metadata, itemType)
      { }

      protected override BasicCompletionData CreateCompletion()
      {
        return new AttributeValueCompletionData() { MultiValue = true };
      }
    }

    private class OrderByPropertyFactory : PropertyCompletionFactory
    {
      public OrderByPropertyFactory(ArasMetadataProvider metadata, ItemType itemType) :
        base(metadata, itemType)
      { }

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

    public IEnumerable<string> GetFunctionNames(bool tableValued)
    {
      return Enumerable.Empty<string>();
    }
  }
}
