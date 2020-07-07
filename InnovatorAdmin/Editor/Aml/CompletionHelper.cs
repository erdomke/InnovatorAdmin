using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using Innovator.Client;
using InnovatorAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                  Id = reader.GetAttribute("id"),
                  By = reader.GetAttribute("by")
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
          case "RebuildKeyedName":
            items = Elements("ItemTypes");
            break;
          default:
            if (soapAction == ArasEditorProxy.UnitTestAction)
              items = Elements("TestSuite");
            else
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
                    var attributes = new Dictionary<string, string>
                    {
                      { "action", null }
                      , { "id", null }
                      , { "idlist", null }
                      , { "type", null }
                      , { "typeId", null }
                    };

                    if (TryGetActionDoc(path, out var doc))
                    {
                      foreach (var attr in doc.Attributes)
                        attributes[attr.Name] = attr.Summary;
                    }

                    if (path.Count >= 3
                      && path[path.Count - 2].LocalName == "Relationships"
                      && path[path.Count - 3].LocalName == "Item"
                      && path[path.Count - 3].Action == "GetItemRepeatConfig")
                    {
                      attributes["repeatProp"] = null;
                      attributes["repeatTimes"] = null;
                    }

                    items = Attributes(notExisting, attributes).ToArray();
                    foreach (var item in items.OfType<AttributeCompletionData>().Where(i => i.Text == "where"))
                    {
                      item.QuoteChar = '"';
                    }
                    break;
                }
                break;
              default:
                if (TryGetActionDoc(path, out var actionDoc))
                {
                  items = Attributes(notExisting, actionDoc.Attributes.Select(a => new KeyValuePair<string, string>(a.Name, a.Summary))).ToArray();
                }
                else
                {
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
                  if (path.Last().LocalName.StartsWith("xp-"))
                    items = items.Concat(Attributes(notExisting, "set", "permission_id", "explicit"));
                  if (path.Last().Condition == "is defined" || path.Last().Condition == "is not defined")
                    items = items.Concat(Attributes(notExisting, "defined_as"));
                  if (path.Last().Condition == "in")
                    items = items.Concat(Attributes(notExisting, "by"));
                }
                break;
            }

            filter = attrName;
            break;
          case XmlState.AttributeValue:
            if (TryGetActionDoc(path, out var attributeDoc) && attributeDoc.Attributes.Any(a => a.Name == attrName))
            {
              var attribute = attributeDoc.Attributes.FirstOrDefault(a => a.Name == attrName);
              var whereType = attribute?.ValueTypes.FirstOrDefault(t => t.Type == AmlDataType.WhereClause);
              var orderByType = attribute?.ValueTypes.FirstOrDefault(t => t.Type == AmlDataType.OrderBy);
              var selectType = attribute?.ValueTypes.FirstOrDefault(t => t.Type == AmlDataType.SelectList);
              var itemType = default(ItemType);

              if (orderByType != null)
              {
                var typeName = orderByType.Source ?? path.Last().Type;
                if (!string.IsNullOrEmpty(typeName)
                  && _metadata.ItemTypeByName(typeName, out itemType))
                {
                  var lastComma = value.LastIndexOf(",");
                  if (lastComma >= 0) value = value.Substring(lastComma + 1).Trim();

                  items = await new OrderByPropertyFactory(_metadata, itemType).GetPromise().ToTask();
                }
              }
              else if (selectType != null)
              {
                var typeName = selectType.Source ?? path.Last().Type;
                if (!string.IsNullOrEmpty(typeName)
                  && _metadata.ItemTypeByName(typeName, out itemType))
                {
                  var orig = value;
                  string partial;
                  var selectPath = SelectPath(value, out partial);
                  value = partial;

                  if (orig.EndsWith("["))
                  {
                    items = new ICompletionData[]
                    {
                          new BasicCompletionData("is_not_null()]") { Image = Icons.Method16.Wpf },
                    };
                  }
                  else
                  {
                    var it = await RecurseProperties(itemType, selectPath);
                    if (it != null)
                    {
                      items = await new SelectPropertyFactory(_metadata, it).GetPromise().ToTask();
                      items = items.Concat(new ICompletionData[]
                      {
                            new BasicCompletionData("*") { Image = Icons.Property16.Wpf },
                      });
                      if (it.Properties.Values.Any(p => p.Name.StartsWith("xp-")))
                      {
                        items = items.Concat(new ICompletionData[]
                        {
                              new BasicCompletionData("xp-*") { Image = Icons.Property16.Wpf },
                        });
                      }
                    }

                    if (selectPath.LastOrDefault()?.StartsWith("xp-") == true)
                    {
                      items = (items ?? Enumerable.Empty<ICompletionData>()).Concat(new ICompletionData[]
                      {
                            new BasicCompletionData("$value") { Image = Icons.Property16.Wpf },
                            new BasicCompletionData("@defined_as") { Image = Icons.Property16.Wpf },
                            new BasicCompletionData("@explicit") { Image = Icons.Property16.Wpf },
                            new BasicCompletionData("@permission_id") { Image = Icons.Property16.Wpf },
                      });
                    }
                  }
                }
              }
              else if (whereType != null)
              {
                var typeName = whereType.Source ?? path.Last().Type;
                if (!string.IsNullOrEmpty(typeName)
                    && _metadata.ItemTypeByName(typeName, out itemType))
                {
                  return await _sql.Completions("select * from innovator.[" + itemType.Name.Replace(' ', '_')
                    + "] where " + value, xml, caret, xml.GetCharAt(caret - value.Length - 1).ToString(), true).ToTask();
                }
              }
              else if (attribute?.ValueTypes.Any() == true)
              {
                items = await Completions<AttributeValueCompletionData>(state, path, attribute, attribute.ValueTypes);
              }
            }
            else if (path.Last().LocalName == "Item")
            {
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
                  var version = aras?.Version?.Major ?? -1;

                  var methods = (IEnumerable<string>)baseMethods;
                  if (version < 10)
                    methods = methods.Concat(Enumerable.Repeat("checkImportedItemType", 1));
                  if (version < 11)
                    methods = methods.Concat(Enumerable.Repeat("exportItemType", 1));
                  if (version < 0 || version >= 10)
                    methods = methods.Concat(Enumerable.Repeat("VaultServerEvent", 1));
                  if (version < 0 || version >= 11)
                    methods = methods.Concat(new string[] { "GetInheritedServerEvents", "getHistoryItems" });

                  items = _metadata.AllMethods.Select(m => (ICompletionData)new AttributeValueCompletionData()
                  {
                    Text = m.KeyedName,
                    Image = Icons.Method16.Wpf,
                    Description = m.Documentation?.Summary
                  }).Concat(methods.Select(m => (ICompletionData)new AttributeValueCompletionData()
                  {
                    Text = m,
                    Image = Icons.MethodFriend16.Wpf
                  }));
                  break;
                case "id":
                  items = await Completions<AttributeValueCompletionData>(state, path, null, new[] { AmlTypeDefinition.FromDefinition(AmlDataType.Item, path.Last().Type) });
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
                  if (path.Last().LocalName.StartsWith("xp-"))
                  {
                    items = items.Concat(new ICompletionData[]
                    {
                      new AttributeValueCompletionData() { Text = "is defined", Image = Icons.EnumValue16.Wpf },
                      new AttributeValueCompletionData() { Text = "is not defined", Image = Icons.EnumValue16.Wpf },
                    });
                  }
                  break;
                case "explicit":
                case "is_null":
                  items = AttributeValues("0", "1");
                  break;
                case "defined_as":
                  items = AttributeValues("class", "explicit");
                  break;
                case "set":
                  items = new ICompletionData[] {
                    new BasicCompletionData("value") { Image = Icons.Property16.Wpf },
                    new BasicCompletionData("explicit") { Image = Icons.Property16.Wpf },
                    new BasicCompletionData("permission_id") { Image = Icons.Property16.Wpf },
                  };
                  var idx = value.LastIndexOf('|');
                  if (idx >= 0)
                    value = value.Substring(idx + 1);
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
              if (TryGetActionDoc(path, out var actionDoc))
              {
                if (actionDoc.Elements.Any())
                {
                  items = actionDoc.Elements.Select(e => {
                    return (ICompletionData)new BasicCompletionData()
                    {
                      Text = e.Name + string.Join("", e.Attributes
                        .Where(a => a.ValueTypes.All(t => t.Type == AmlDataType.Enum) && a.ValueTypes.Sum(t => t.Values.Count()) == 1)
                        .Select(a => $" {a.Name}='{a.ValueTypes.SelectMany(t => t.Values).Single()}'")),
                      Image = Icons.XmlTag16.Wpf,
                      Description = e.Summary
                    };
                  });
                }
                else if (actionDoc?.ValueTypes.Any() == true)
                {
                  items = await Completions<BasicCompletionData>(state, path, actionDoc, actionDoc.ValueTypes);
                }

                if (actionDoc is CompletionDocumentation completeDoc && completeDoc.Completions != null)
                {
                  items = (await completeDoc.Completions(_conn, path, () => new BasicCompletionData())) ?? items;
                }
              }

              var j = path.Count - 1;
              while (path[j].LocalName == "and" || path[j].LocalName == "not" || path[j].LocalName == "or") j--;
              var last = path[j];

              var includeProperties = items?.Any() != true || (!_actionDocs.ContainsKey(last.Action ?? "") && last.Type != "Method");
              if (includeProperties && last.LocalName == "Item" && state == XmlState.Tag)
              {
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
                  var buffer = new List<ICompletionData>(items ?? Enumerable.Empty<ICompletionData>());
                  if (!buffer.Any(d => d.Text == "Relationships"))
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
              }
              else if (state == XmlState.Tag && last.LocalName == "params" && soapAction == "GetAssignedTasks")
              {
                items = Elements("inBasketViewMode", "workflowTasks", "projectTasks", "actionTasks", "userID");
              }
              else if (path.Count > 1 && items?.Any() != true)
              {
                var lastItem = path.LastOrDefault(n => n.LocalName == "Item");

                if (lastItem != null)
                {
                  if (path.Last().LocalName == "Relationships")
                  {
                    items = Elements("Item");
                  }
                  else if (path.Last().Condition == "in" && !string.IsNullOrEmpty(path.Last().By))
                  {
                    items = Elements("Item").Concat(new AttributeCompletionData[] {
                      new AttributeCompletionData()
                      {
                        Text = "Item action='get' select='" + path.Last().By + "'",
                        Action = () => "Item action='get' select='" + path.Last().By + "' type",
                        Image = Icons.Attribute16.Wpf
                      }
                    });
                  }
                  else if (path.Last().Condition == "in"
                    || path.Last().LocalName.Equals("sql", StringComparison.OrdinalIgnoreCase))
                  {
                    return await _sql.Completions(value, xml, caret, cdata ? "]]>" : "<").ToTask();
                  }
                  else
                  {
                    ItemType itemType;
                    if (!string.IsNullOrEmpty(lastItem.Type)
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
        try
        {
          var clipboard = System.Windows.Clipboard.GetText();
          var match = Regex.Match(clipboard, @"^http(s)?://.*?StartItem=(?<type>\w+):(?<id>[0-9A-Fa-f]{32})$");
          if (match.Success)
          {
            var text = $"Item type='{match.Groups["type"].Value}' id='{match.Groups["id"].Value}'";
            items = items.Concat(new ICompletionData[] {
              new BasicCompletionData()
              {
                Text = text,
                Action = () => text,
                Image = Icons.Attribute16.Wpf
              }
            });
          }
        }
        catch (Exception) { }

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

    private static IEnumerable<ICompletionData> Attributes(Func<string, bool> filter, IEnumerable<KeyValuePair<string, string>> values)
    {
      return values.Where(k => filter(k.Key)).Select(k => (ICompletionData)new AttributeCompletionData()
      {
        Text = k.Key,
        Image = Icons.Field16.Wpf,
        Description = k.Value
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

    private async Task<IEnumerable<ICompletionData>> Completions<T>(XmlState state, IList<AmlNode> path, AmlDocumentation doc, IEnumerable<AmlTypeDefinition> typeDefinitions) where T : BasicCompletionData, new()
    {
      var results = new List<ICompletionData>();
      var action = path.LastOrDefault(n => !string.IsNullOrEmpty(n.Action))?.Action;
      foreach (var typeDefn in typeDefinitions)
      {
        switch (typeDefn.Type)
        {
          case AmlDataType.Boolean:
            results.Add(new T() {
              Text = "0",
              Image = Icons.EnumValue16.Wpf,
              Content = "0 (False)"
            });
            results.Add(new T()
            {
              Text = "1",
              Image = Icons.EnumValue16.Wpf,
              Content = "1 (True)"
            });
            break;
          case AmlDataType.Date:
            results.Add(new T() {
              Text = DateTime.Now.ToString("s"),
              Image = Icons.EnumValue16.Wpf,
              Content = DateTime.Now.ToString("s") + " (Now)"
            });
            break;
          case AmlDataType.Enum:
            results.AddRange(typeDefn.Values.Select(v => (ICompletionData)new T()
            {
              Text = v,
              Image = Icons.EnumValue16.Wpf
            }));
            break;
          case AmlDataType.Item:
          case AmlDataType.ItemName:
            var itemTypeName = typeDefn.Source;
            if (string.IsNullOrEmpty(itemTypeName) && state == XmlState.Other && !string.IsNullOrEmpty(path.Last().Type))
              itemTypeName = path.Last().Type;

            if (typeDefn.Type != AmlDataType.ItemName)
            {
              try
              {
                var clipboardText = System.Windows.Clipboard.GetText();
                if (clipboardText.IsGuid())
                {
                  results.Add(new T()
                  {
                    Text = " (Paste Guid)",
                    Content = FormatText.ColorText("(Paste Guid)", Brushes.Purple),
                    Action = () => clipboardText
                  });
                }
              }
              catch (Exception) { }

              results.Add(state == XmlState.AttributeValue ? new T()
              {
                Text = "(New Guid)",
                Content = FormatText.ColorText("(New Guid)", Brushes.Purple),
                Action = () => Guid.NewGuid().ToString("N").ToUpperInvariant()
              } : new T()
              {
                Text = (state != XmlState.Tag ? "<" : "") + "Item type='" + itemTypeName + "'",
                Image = Icons.XmlTag16.Wpf
              });
            }

            if (action != "add" || state != XmlState.AttributeValue)
            {
              switch (itemTypeName)
              {
                case "RelationshipType":
                  if (typeDefn.Type == AmlDataType.ItemName)
                  {
                    results.AddRange(ItemTypeCompletion<T>(_metadata.ItemTypes.Where(i => i.IsRelationship), false));
                  }
                  break;
                case "ItemType":
                  var useKeyedName = typeDefn.Type == AmlDataType.ItemName || (doc?.Name == "type" && state == XmlState.AttributeValue);
                  results.AddRange(ItemTypeCompletion<T>(_metadata.ItemTypes, !useKeyedName));
                  break;
                case "Method":
                  results.AddRange(_metadata.AllMethods.Select(r => new T()
                  {
                    Text = r.KeyedName,
                    Action = () => typeDefn.Type == AmlDataType.ItemName ? r.KeyedName : r.Unique,
                    Image = Icons.Method16.Wpf,
                    Description = r.Documentation?.Summary
                  }));
                  break;
                case "SQL":
                  results.AddRange(_metadata.Sqls().Select(r => new T()
                  {
                    Text = r.KeyedName,
                    Action = () => typeDefn.Type == AmlDataType.ItemName ? r.KeyedName : r.Unique,
                    Image = Icons.EnumValue16.Wpf
                  }));
                  break;
                case "Identity":
                  results.AddRange(_metadata.SystemIdentities.Select(r => new T()
                  {
                    Text = r.KeyedName,
                    Action = () => typeDefn.Type == AmlDataType.ItemName ? r.KeyedName : r.Unique,
                    Image = Icons.EnumValue16.Wpf
                  }));
                  break;
                case "Sequence":
                  results.AddRange(_metadata.Sequences.Select(r => new T()
                  {
                    Text = r.KeyedName,
                    Action = () => typeDefn.Type == AmlDataType.ItemName ? r.KeyedName : r.Unique,
                    Image = Icons.EnumValue16.Wpf
                  }));
                  break;
                case "User":
                  if (typeDefn.Type != AmlDataType.ItemName)
                  {
                    results.Add(new T() {
                      Action = () => _conn.UserId,
                      Text = "Me",
                      Image = Icons.EnumValue16.Wpf
                    });
                  }
                  break;
                case "File":
                  if (state != XmlState.AttributeValue && typeDefn.Type != AmlDataType.ItemName)
                  {
                    results.Add(new T()
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
                    });
                  }
                  break;
              }

              if (state != XmlState.AttributeValue && typeDefn.Type != AmlDataType.ItemName && itemTypeName != "File")
                results.Add(new ItemPropertyCompletionData(_conn, path, itemTypeName));
            }
            break;
          case AmlDataType.List:
          case AmlDataType.MultiValueList:
            var listId = typeDefn.Source;
            if (!listId.IsGuid())
              listId = (await _conn.ApplyAsync("<Item type='List' action='get' select='id'><keyed_name>@0</keyed_name></Item>", true, false, typeDefn.Source)).AssertItem().Id();
            var values = await _metadata.ListValues(listId).ToTask();

            var hash = new HashSet<string>(values.Select(v => v.Value), StringComparer.CurrentCultureIgnoreCase);
            results.AddRange(values
              .Select(v => new T()
              {
                Text = v.Value,
                Image = Icons.EnumValue16.Wpf
              })
              .Concat(values.Where(v => !string.IsNullOrWhiteSpace(v.Label) && !hash.Contains(v.Label))
                .Select(v => new T()
                {
                  Text = v.Label + " (" + v.Value + ")",
                  Image = Icons.EnumValue16.Wpf,
                  Description = v.Value,
                  Content = FormatText.MutedText(v.Label + " (" + v.Value + ")"),
                  Action = () => v.Value
                })));
            break;
          default:
            break;
        }
      }
      return results;
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

    private async Task<IEnumerable<ICompletionData>> PropertyValueCompletion(ItemType itemType, XmlState state, AmlNode lastItem, IList<AmlNode> path)
    {
      string itemValue;

      var p = await _metadata.GetProperty(itemType, path.Last().LocalName).ToTask();
      if (p.Name == "classification")
      {
        var paths = await _metadata.GetClassPaths(itemType).ToTask();
        return paths.GetCompletions<BasicCompletionData>();
      }
      else if (p.Name == "name" && itemType.Name == "Property"
        && lastItem.Values.TryGetValue("label", out itemValue)
        && IsUpdateAction(lastItem.Action))
      {
        return Enumerable.Repeat(PropNameFromLabel(itemValue), 1).GetCompletions<BasicCompletionData>();
      }
      else if (p.Name == "name" && itemType.Name == "xPropertyDefinition"
        && lastItem.Values.TryGetValue("label", out itemValue)
        && IsUpdateAction(lastItem.Action))
      {
        return Enumerable.Repeat("xp-" + PropNameFromLabel(itemValue), 1).GetCompletions<BasicCompletionData>();
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
        return await Completions<BasicCompletionData>(state, path, null, p.GetTypeDefinitions());
      }
    }

    private string PropNameFromLabel(string label)
    {
      var output = new char[label.Length];
      var o = 0;
      for (var i = 0; i < label.Length; i++)
      {
        if (char.IsLetterOrDigit(label[i]))
        {
          output[o] = char.ToLowerInvariant(label[i]);
          o++;
        }
        else if (o == 0 || output[o - 1] != '_')
        {
          output[o] = '_';
          o++;
        }
      }
      if (output[o - 1] == '_') o--;
      return new string(output, 0, Math.Min(o, 32));
    }

    private bool IsUpdateAction(string action)
    {
      return action == "add" || action == "create"
          || action == "edit" || action == "merge";
    }

    private class ItemPropertyCompletionData : ICompletionData
    {
      private IAsyncConnection _conn;
      private IList<AmlNode> _path;
      private string _itemTypeName;

      public ItemPropertyCompletionData(IAsyncConnection conn, IList<AmlNode> path, string itemTypeName)
      {
        _conn = conn;
        _path = path;
        _itemTypeName = itemTypeName;
      }

      public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs)
      {
        var item = _path.LastOrDefault(n => n.LocalName == "Item");
        if (item == null)
          return;

        var query = string.Format("<Item type='{0}' action='get'><keyed_name condition='like'>**</keyed_name></Item>", _itemTypeName);
        var items = EditorWindow.GetItems(_conn, query, query.Length - 21);
        if (items.Any(i => _itemTypeName == i.Type))
        {
          var allItems = items.Where(i => _itemTypeName == i.Type).ToArray();
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
          var needsType = string.IsNullOrEmpty(_path.LastOrDefault()?.Type)
            || !string.Equals(_path.LastOrDefault()?.Type, allItems[0].Type, StringComparison.OrdinalIgnoreCase);
          doc.Replace(start, end - start, (needsType ? " type='" + allItems[0].Type + "'" : "") + " keyed_name='" + allItems[0].KeyedName + "'>" + allItems[0].Unique);
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
            return null;
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

    private bool TryGetActionDoc(List<AmlNode> path, out AmlDocumentation actionDoc)
    {
      actionDoc = null;
      var lastActionIdx = path.FindLastIndex(n => !string.IsNullOrEmpty(n.Action));
      if (lastActionIdx < 0)
        return false;

      var action = path[lastActionIdx].Action;
      if (!_actionDocs.TryGetValue(action ?? "", out actionDoc))
        actionDoc = _metadata.AllMethods.OfType<Method>().FirstOrDefault(m => m.KeyedName == action)?.Documentation;

      if (actionDoc == null)
        return false;

      for (var i = lastActionIdx + 1; i < path.Count && actionDoc != null; i++)
      {
        actionDoc = actionDoc.Elements.FirstOrDefault(e => e.Name == path[i].LocalName);
      }

      return actionDoc != null;
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


    private static List<string> SelectPath(string selectStr, out string partial)
    {
      partial = null;
      var lastOperator = selectStr.LastIndexOfAny(new[] { ')', '(', ',', '|', '[', ']' });
      if (lastOperator < selectStr.Length - 1)
      {
        partial = selectStr.Substring(lastOperator + 1).Trim();
      }
      var node = SelectNode.FromString(selectStr);

      var path = new List<string>();
      var curr = node.LastOrDefault();
      while (curr != null)
      {
        if (!string.IsNullOrEmpty(curr.Name))
          path.Add(curr.Name);
        curr = curr.LastOrDefault();
      }

      var itemsToKeep = 0;
      for (var i = 0; i < selectStr.Length; i++)
      {
        if (selectStr[i] == '(')
          itemsToKeep++;
        else if (selectStr[i] == ')')
          itemsToKeep--;
      }

      while (path.Count > itemsToKeep)
        path.RemoveAt(path.Count - 1);

      return path;
    }

    private class AmlNode
    {
      public int Offset { get; set; }
      public string LocalName { get; set; }
      public string Type { get; set; }
      public string Id { get; set; }
      public string Action { get; set; }
      public string Condition { get; set; }
      public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();
      public string By { get; set; }
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

    private delegate Task<IEnumerable<ICompletionData>> CompletionOverride(IAsyncConnection conn, IList<AmlNode> path, Func<BasicCompletionData> factory);

    private class CompletionDocumentation : AmlDocumentation
    {
      public CompletionOverride Completions { get; }

      public CompletionDocumentation(string name) : base(name) { }
      public CompletionDocumentation(string name, CompletionOverride overrideFunc) : base(name)
      {
        Completions = overrideFunc;
      }

      public CompletionDocumentation(string name, CompletionOverride overrideFunc, AmlDataType dataType, params string[] values) : base(name, null, dataType, values)
      {
        Completions = overrideFunc;
      }
    }

    private static Dictionary<string, AmlDocumentation> _actionDocs = new Dictionary<string, AmlDocumentation>();

    static AmlEditorHelper()
    {
      _actionDocs["add"] = new AmlDocumentation("add")
        .WithAttribute("do_skipOnAfterAdd", "If 1 then don't run onAfterAdd server events. Default is 0", AmlDataType.Boolean)
        .WithAttribute("serverEvents", "If 0 then disable server events when running the doGetItem only. onBefore/AfterAdd events are not disabled. Default is 1.", AmlDataType.Boolean);
      _actionDocs["AddItem"] = _actionDocs["add"];
      _actionDocs["copy"] = new AmlDocumentation("copy")
        .WithAttribute("do_add", "Whether or not to add the item to the database", AmlDataType.Boolean)
        .WithAttribute("do_lock", "Whether or not to keep the item locked when the operation is complete", AmlDataType.Boolean);
      _actionDocs["copyAsIs"] = new AmlDocumentation("copyAsIs")
        .WithAttribute("lock_related", null, AmlDataType.Boolean)
        .WithAttribute("do_lock", "Whether or not to keep the item locked when the operation is complete", AmlDataType.Boolean)
        .WithAttribute("useInputProperties", null, AmlDataType.Boolean);
      _actionDocs["copyAsNew"] = _actionDocs["copyAsIs"];
      _actionDocs["get"] = new AmlDocumentation("get")
        .WithAttribute("select", "A comma-delimited list of property names (column names) to return", AmlDataType.SelectList)
        .WithAttribute("orderBy", "A comma-delimited list of property names (column names) specifying the order of the results", AmlDataType.OrderBy)
        .WithAttribute("page", "The page number for the results set.", AmlDataType.Integer)
        .WithAttribute("pagesize", "The number of results to include in a page.", AmlDataType.Integer)
        .WithAttribute("maxRecords", "the absolute maximum Items to be searched in the database.", AmlDataType.Integer)
        .WithAttribute("levels", "The Item configuration depth to be returned", AmlDataType.Integer)
        .WithAttribute("serverEvents", "If 0 then disable the server events improving performance. Default is 1.", AmlDataType.Boolean)
        .WithAttribute("isCriteria", "If 0 then include the nested structure for the Item configuration in the response but don't use it as search criteria. Default is 1, which uses the nested structure in the request as search criteria.", AmlDataType.Boolean)
        .WithAttribute("related_expand", "If 0 then do not expand item properties, instead, only return the ID.", AmlDataType.Boolean)
        .WithAttribute("language", "A comma-delimited list of language codes", AmlDataType.String)
        .WithAttribute("queryType", "Defines which version of a versionable item to return based on the queryDate", AmlDataType.Enum, "Effective", "Latest", "Released")
        .WithAttribute("queryDate", "Date to use when searching for versionable items", AmlDataType.Date)
        .WithAttribute("relas_only", "If 1, only return the contents of the <Relationships> tag and not the parent item. Default is 0.", AmlDataType.Boolean)
        .WithAttribute("stdProps", "Whether to include standard system properties. Default is 1. If 0, properties such as config_id, created_by_id, current_state, modified_by_id, and permission_id are not returned.", AmlDataType.Boolean)
        .WithAttribute("expand", null, AmlDataType.Boolean)
        .WithAttribute("config_path", null, AmlDataType.String)
        .WithAttribute("where", null, AmlDataType.WhereClause)
        .WithAttribute("returnMode", null, AmlDataType.Enum, "itemsOnly", "countOnly");
      _actionDocs["create"] = new AmlDocumentation("create");
      _actionDocs["create"].WithAttributes(_actionDocs["get"].Attributes);
      _actionDocs["create"].WithAttributes(_actionDocs["add"].Attributes);

      _actionDocs["getItemAllVersions"] = _actionDocs["get"];
      _actionDocs["GetItemConfig"] = _actionDocs["get"];
      _actionDocs["getItemLastVersion"] = _actionDocs["get"];
      _actionDocs["getItemRelationships"] = _actionDocs["get"];
      _actionDocs["GetItemRepeatConfig"] = _actionDocs["get"];
      _actionDocs["recache"] = _actionDocs["get"];

      _actionDocs["getPermissions"] = new AmlDocumentation("getPermissions")
        .WithAttribute("access_type", "Permission to check for", AmlDataType.Enum, "can_add", "can_delete", "can_get", "can_update", "can_discover", "can_change_access");
      _actionDocs["edit"] = new AmlDocumentation("edit")
        .WithAttribute("version", "If 0 then don't version an Item on update. Default is 1, which is version the Item (if it's a versionable Item) on update.", AmlDataType.Boolean)
        .WithAttribute("serverEvents", "If 0 then disable the server events improving performance. Default is 1. Only Update events are disabled, Lock events can be executed if using Edit.", AmlDataType.Boolean)
        .WithAttribute("unlock", "If 1, then unlock the item after the update.", AmlDataType.Boolean)
        .WithAttribute("where", null, AmlDataType.WhereClause);
      _actionDocs["delete"] = new AmlDocumentation("delete")
        .WithAttribute("where", null, AmlDataType.WhereClause);
      _actionDocs["purge"] = new AmlDocumentation("purge")
        .WithAttribute("where", null, AmlDataType.WhereClause);
      _actionDocs["merge"] = new AmlDocumentation("merge")
        .WithAttribute("do_skipOnAfterAdd", "If 1 then don't run onAfterAdd server events. Default is 0", AmlDataType.Boolean)
        .WithAttribute("version", "If 0 then don't version an Item on update. Default is 1, which is version the Item (if it's a versionable Item) on update.", AmlDataType.Boolean)
        .WithAttribute("serverEvents", "If 0 then disable the server events improving performance. Default is 1. Only Update events are disabled, Lock events can be executed if using Edit.", AmlDataType.Boolean)
        .WithAttribute("unlock", "If 1, then unlock the item after the update.", AmlDataType.Boolean);
      _actionDocs["update"] = new AmlDocumentation("update")
        .WithAttribute("version", "If 0 then don't version an Item on update. Default is 1, which is version the Item (if it's a versionable Item) on update.", AmlDataType.Boolean)
        .WithAttribute("serverEvents", "If 0 then disable the server events improving performance. Default is 1. Only Update events are disabled, Lock events can be executed if using Edit.", AmlDataType.Boolean)
        .WithAttribute("where", null, AmlDataType.WhereClause);
      _actionDocs["AddHistory"] = new AmlDocumentation("AddHistory")
        .WithElements(new AmlDocumentation("action"), new AmlDocumentation("filename"), new AmlDocumentation("form_name"));
      _actionDocs["GetItemsForStructureBrowser"] = new AmlDocumentation("GetItemsForStructureBrowser")
        .WithElements(new AmlDocumentation("Item"));
      _actionDocs["EvaluateActivity"] = new AmlDocumentation("EvaluateActivity")
        .WithElements(new CompletionDocumentation("Activity", async (conn, path, factory) =>
          {
            var result = factory();
            result.Text = "Select context item...";
            result.Content = FormatText.ColorText("Select context item...", Brushes.Purple);
            result.Action = () =>
            {
              var items = EditorWindow.GetItems(conn, "<Item action='get'", 18).ToArray();
              if (items.Length != 1)
                return string.Empty;

              var activities = conn.Apply(@"<Item type='Workflow' action='get' select='related_id'>
  <source_id>@0</source_id>
  <related_id>
    <Item type='Workflow Process' action='get' select='id'>
      <state>Active</state>
      <locked_by_id condition='is null' />
      <Relationships>
        <Item action='get' type='Workflow Process Activity' select='related_id'>
          <related_id>
            <Item type='Activity' action='get' select='id'>
              <state>Active</state>
              <is_auto>0</is_auto>
            </Item>
          </related_id>
        </Item>
      </Relationships>
    </Item>
  </related_id>
</Item>", items[0].Unique).Items().ToArray();
              if (activities.Length != 1)
                return string.Empty;
              var activity = activities[0].RelatedItem().Relationships().Single().RelatedItem();

              return activity.Id() + "<!-- " + activity.Property("id").KeyedName().Value + " for " + items[0].KeyedName + "-->";
            };
            return new List<BasicCompletionData>() { result };
          })
          , new CompletionDocumentation("ActivityAssignment", async (conn, path, factory) =>
            {
              var lastItem = path.LastOrDefault(n => !string.IsNullOrEmpty(n.Action));
              if (lastItem.Values.TryGetValue("Activity", out var itemValue))
              {
                var assn = (await conn.ApplyAsync(@"<Item type='Activity Assignment' action='get' select='id,related_id' related_expand='0'>
  <is_disabled>0</is_disabled>
  <closed_on condition='is null'></closed_on>
  <source_id>@0</source_id>
</Item>", true, false, itemValue).ToTask()).Items().ToList();
                return assn.Select(a =>
                {
                  var item = factory();
                  item.Text = a.Id() + "<!-- " + a.RelatedId().KeyedName().Value + " -->";
                  item.Image = Icons.EnumValue16.Wpf;
                  item.Content = a.RelatedId().KeyedName().Value;
                  return item;
                });
              }
              return Enumerable.Empty<ICompletionData>();
            }, AmlDataType.Item, "Activity Assignment")
          , new CompletionDocumentation("Paths", async (conn, path, factory) =>
            {
              var lastItem = path.LastOrDefault(n => !string.IsNullOrEmpty(n.Action));
              if (lastItem.Values.TryGetValue("Activity", out var itemValue))
              {
                var act = (await conn.ApplyAsync(@"<Item type='Activity' action='get' select='can_delegate,can_refuse' id='@0'>
                                        <Relationships>
                                          <Item type='Workflow Process Path' action='get' select='id,name,label,is_default'>
                                          </Item>
                                        </Relationships>
                                      </Item>", true, false, itemValue).ToTask()).Items().ToArray();
                if (act.Length == 1)
                {
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
              }
              return Enumerable.Empty<ICompletionData>();
            })
            .WithElements(new AmlDocumentation("Path", null, AmlDataType.String)
              .WithAttribute("id", null, AmlDataType.Item, "Workflow Process Path"))
          , new AmlDocumentation("DelegateTo", null, AmlDataType.Item, "Identity")
          , new AmlDocumentation("Tasks")
            .WithElements(new AmlDocumentation("Task")
              .WithAttribute("id", null, AmlDataType.Item, "Activity Task Value")
              .WithAttribute("complete", null, AmlDataType.Boolean))
          , new AmlDocumentation("Variables")
            .WithElements(new AmlDocumentation("Variable")
              .WithAttribute("id", null, AmlDataType.Item, "Activity Variable Value"))
          , new AmlDocumentation("Authentication")
            .WithAttribute("mode", null, AmlDataType.Enum, "esignature", "password")
          , new AmlDocumentation("Comments", null, AmlDataType.String)
          , new AmlDocumentation("Complete", null, AmlDataType.Boolean));
      _actionDocs["instantiateWorkflow"] = new AmlDocumentation("instantiateWorkflow")
        .WithElements(new AmlDocumentation("WorkflowMap", null, AmlDataType.Item, "Workflow Map"));
      _actionDocs["promoteItem"] = new AmlDocumentation("promoteItem")
        .WithElements(new AmlDocumentation("state"),
          new AmlDocumentation("comments"));
      _actionDocs["Run Report"] = new AmlDocumentation("Run Report")
        .WithElements(new AmlDocumentation("report_name"),
          new AmlDocumentation("AML")
            .WithElements(new AmlDocumentation("Item")));
      _actionDocs["SQL Process"] = new AmlDocumentation("SQL Process")
        .WithElements(new AmlDocumentation("name"),
          new AmlDocumentation("PROCESS"),
          new AmlDocumentation("ARG1"),
          new AmlDocumentation("ARG2"),
          new AmlDocumentation("ARG3"),
          new AmlDocumentation("ARG4"),
          new AmlDocumentation("ARG5"),
          new AmlDocumentation("ARG6"),
          new AmlDocumentation("ARG7"),
          new AmlDocumentation("ARG8"),
          new AmlDocumentation("ARG9"));
    }
  }
}
