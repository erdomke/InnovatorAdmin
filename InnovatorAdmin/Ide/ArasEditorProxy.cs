using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using System.Xml;
using System.IO;
using System.Data;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace InnovatorAdmin
{
  public class ArasEditorProxy : IEditorProxy
  {
    #region "Default Actions"
    private static readonly string[] _defaultActions = new string[] {
            "ActivateActivity",
            "AddItem",
            "ApplyAML",
            "ApplyItem",
            "ApplyMethod",
            "ApplySQL",
            "ApplyUpdate",
            "BuildProcessReport",
            "CacheDiag",
            "CancelWorkflow",
            "ChangeUserPassword",
            "CheckImportedItemType",
            "ClearCache",
            "ClearHistory",
            "CloneForm",
            "CloseWorkflow",
            "CompileMethod",
            "CopyItem",
            "CopyItem2",
            "CreateItem",
            "DeleteItem",
            "DeleteUsers",
            "DeleteVersionFile",
            "EditItem",
            "EvaluateActivity",
            "ExecuteEscalations",
            "ExecuteReminders",
            "ExportItemType",
            "GenerateNewGUID",
            "GenerateNewGUIDEx",
            "GenerateParametersGrid",
            "GenerateRelationshipsTabbar",
            "GenerateRelationshipsTable",
            "GetAffectedItems",
            "GetAssignedActivities",
            "GetAssignedTasks",
            "GetConfigurableGridMetadata",
            "GetCurrentUserID",
            "GetFormForDisplay",
            "GetHistoryItems",
            "GetIdentityList",
            "GetItem",
            "GetItemAllVersions",
            "GetItemLastVersion",
            "GetItemNextStates",
            "GetItemRelationships",
            "GetItemTypeByFormID",
            "GetItemTypeForClient",
            "GetItemWhereUsed",
            "GetMainTreeItems",
            "GetNextSequence",
            "GetPermissionsForClient",
            "GetUsersList",
            "GetUserWorkingDirectory",
            "InstantiateWorkflow",
            "LoadCache",
            "LoadProcessInstance",
            "LoadVersionFile",
            "LockItem",
            "LogMessage",
            "LogOff",
            "MergeItem",
            "NewItem",
            "NewRelationship",
            "PopulateRelationshipsGrid",
            "PopulateRelationshipsTables",
            "ProcessReplicationQueue",
            "PromoteItem",
            "PurgeItem",
            "ReassignActivity",
            "RebuildKeyedName",
            "RebuildView",
            "ReplicationExecutionResult",
            "ResetAllItemsAccess",
            "ResetItemAccess",
            "ResetLifeCycle",
            "ResetServerCache",
            "SaveCache",
            "ServerErrorTest",
            "SetDefaultLifeCycle",
            "SetNullBooleanTo0",
            "SetUserWorkingDirectory",
            "SkipItem",
            "StartDefaultWorkflow",
            "StartNamedWorkflow",
            "StartWorkflow",
            "StoreVersionFile",
            "TransformVaultServerURL",
            "UnlockAll",
            "UnlockItem",
            "UpdateItem",
            "ValidateUser",
            "ValidateVote",
            "ValidateWorkflowMap"};
    #endregion

    private IAsyncConnection _conn;
    private Connections.ConnectionData _connData;
    private string _name;
    private Editor.AmlEditorHelper _helper;

    public string Action
    {
      get { return _helper.SoapAction; }
      set { _helper.SoapAction = value; }
    }
    public Connections.ConnectionData ConnData
    {
      get { return _connData; }
      set { _connData = value; }
    }
    public IAsyncConnection Connection
    {
      get { return _conn; }
    }
    public string Name
    {
      get { return _name; }
    }

    public ArasEditorProxy(IAsyncConnection conn, string name)
    {
      _conn = conn;
      _helper = new Editor.AmlEditorHelper();
      _helper.InitializeConnection(_conn);
      _name = name;
    }

    public virtual IEnumerable<string> GetActions()
    {
      return _defaultActions;
    }

    public virtual Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async)
    {
      var innCmd = request as InnovatorCommand;
      if (innCmd == null)
        throw new NotSupportedException("Cannot run commands created by a different proxy");

      var cmd = innCmd.Internal;

      // Check for file uploads and process if need be
      var elem = System.Xml.Linq.XElement.Parse(cmd.Aml);
      var files = elem.DescendantsAndSelf("Item")
        .Where(e => e.Attributes("type").Any(a => a.Value == "File")
                  && e.Elements("actual_filename").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("id").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("action").Any(p => !string.IsNullOrEmpty(p.Value)));
      if (files.Any())
      {
        var upload = _conn.CreateUploadCommand();
        upload.AddFileQuery(cmd.Aml);
        upload.WithAction(cmd.ActionString);
        foreach (var param in innCmd.Parameters)
        {
          upload.WithParam(param.Key, param.Value);
        }
        cmd = upload;
      }

      if (cmd.Action == CommandAction.ApplyAML && cmd.Aml.IndexOf("<AML>") < 0)
      {
        cmd.Aml = "<AML>" + cmd.Aml + "</AML>";
      }
      return ProcessCommand(cmd, async)
        .Convert(s => {
          var result = new ResultObject(s, _conn);
          if (cmd.Action == CommandAction.ApplySQL)
            result.PreferredMode = OutputType.Table;
          return (IResultObject)result;
        });
    }

    protected virtual IPromise<Stream> ProcessCommand(Command cmd, bool async)
    {
      return _conn.Process(cmd, async);
    }

    public virtual IEditorProxy Clone()
    {
      return new ArasEditorProxy(_conn, _name) { ConnData = _connData };
    }

    public ICommand NewCommand()
    {
      return new InnovatorCommand();
    }
    public Editor.IEditorHelper GetHelper()
    {
      return _helper;
    }

    private class ResultObject : IResultObject
    {
      private ITextSource _text;
      private int _count;
      private DataSet _dataSet;
      private int _amlLength;
      private string _title;
      private IAsyncConnection _conn;
      private OutputType _preferredMode = OutputType.Text;
      private string _html;

      public int ItemCount
      {
        get { return _count; }
      }

      public ResultObject(Stream aml, IAsyncConnection conn)
      {
        System.Diagnostics.Debug.Print("{0:hh:MM:ss} Document loaded", DateTime.Now);
        var rope = new Rope<char>();
        using (var reader = new StreamReader(aml))
        using (var writer = new Editor.RopeWriter(rope))
        {
          IndentXml(reader, writer, out _count);
        }
        _amlLength = rope.Length;
        _conn = conn;
        _text = new RopeTextSource(rope);
        System.Diagnostics.Debug.Print("{0:hh:MM:ss} Document rendered", DateTime.Now);
      }

      public ITextSource GetDocument()
      {
        return _text;
      }

      public System.Data.DataSet GetDataSet()
      {
        if (_dataSet == null && _amlLength > 0)
        {
          var doc = new XmlDocument();
          doc.Load(_text.CreateReader());
          _dataSet = Extensions.GetItemTable(_conn.AmlContext.FromXml(doc.DocumentElement)
            , ArasMetadataProvider.Cached(_conn));
        }
        return _dataSet;
      }

      private void IndentXml(TextReader xmlContent, TextWriter writer, out int itemCount)
      {
        itemCount = 0;
        char[] writeNodeBuffer = null;
        var levels = new int[64];
        int level = 0;
        string lastElement = null;
        string value;

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CheckCharacters = true;

        using (var reader = XmlReader.Create(xmlContent))
        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
          bool canReadValueChunk = reader.CanReadValueChunk;
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                if (reader.LocalName == "Item") levels[level]++;
                xmlWriter.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                xmlWriter.WriteAttributes(reader, false);
                if (reader.LocalName == "id" && _title == null)
                {
                  _title = reader.GetAttribute("type") + " " + reader.GetAttribute("keyed_name");
                }
                if (reader.IsEmptyElement)
                {
                  xmlWriter.WriteEndElement();
                }
                else
                {
                  lastElement = reader.LocalName;
                  level++;
                }
                break;
              case XmlNodeType.Text:
                if (canReadValueChunk && lastElement != "Result")
                {
                  if (writeNodeBuffer == null)
                  {
                    writeNodeBuffer = new char[1024];
                  }
                  int count;
                  while ((count = reader.ReadValueChunk(writeNodeBuffer, 0, 1024)) > 0)
                  {
                    xmlWriter.WriteChars(writeNodeBuffer, 0, count);
                  }
                }
                else
                {
                  value = reader.Value;
                  if (lastElement == "Result" && value.Trim().StartsWith("<"))
                  {
                    _html = value;
                    _preferredMode = OutputType.Html;
                  }
                  xmlWriter.WriteString(value);
                }
                break;
              case XmlNodeType.CDATA:
                xmlWriter.WriteCData(reader.Value);
                break;
              case XmlNodeType.EntityReference:
                xmlWriter.WriteEntityRef(reader.Name);
                break;
              case XmlNodeType.ProcessingInstruction:
              case XmlNodeType.XmlDeclaration:
                xmlWriter.WriteProcessingInstruction(reader.Name, reader.Value);
                break;
              case XmlNodeType.Comment:
                xmlWriter.WriteComment(reader.Value);
                break;
              case XmlNodeType.DocumentType:
                xmlWriter.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                break;
              case XmlNodeType.Whitespace:
              case XmlNodeType.SignificantWhitespace:
                xmlWriter.WriteWhitespace(reader.Value);
                break;
              case XmlNodeType.EndElement:
                xmlWriter.WriteFullEndElement();
                level--;
                break;
            }
          }

          xmlWriter.Flush();
          itemCount = levels.FirstOrDefault(i => i > 0);
          writer.Flush();
        }

        if (itemCount > 1) _title = "";
      }


      public string Title
      {
        get { return _title; }
      }

      public OutputType PreferredMode
      {
        get { return _preferredMode; }
        set { _preferredMode = value; }
      }

      public string Html
      {
        get { return _html; }
      }
    }

    private class InnovatorCommand : ICommand
    {
      private Command _internal = new Command();
      private Dictionary<string, object> _params = new Dictionary<string,object>();

      public Command Internal
      {
        get { return _internal; }
      }
      public Dictionary<string, object> Parameters
      {
        get { return _params; }
      }

      public ICommand WithAction(string action)
      {
        _internal.WithAction(action);
        return this;
      }
      public ICommand WithParam(string name, object value)
      {
        _internal.WithParam(name, value);
        _params[name] = value;
        return this;
      }
      public ICommand WithQuery(string query)
      {
        _internal.WithAml(query);
        return this;
      }

    }

    public void Dispose()
    {
      var remote = _conn as IRemoteConnection;
      if (remote != null)
        remote.Logout(true, true);
    }

    public IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      return Promises.All(_conn.ApplyAsync(new Command("<Item/>").WithAction(CommandAction.GetMainTreeItems)
          , true, false), ArasMetadataProvider.Cached(_conn).CompletePromise())
        .Convert(r => ((IReadOnlyResult)r[0]).AssertItem()
            .Property("root")
            .Elements().OfType<IReadOnlyItem>()
            .Select(ProcessTreeNode)
            .Concat(Scripts()));
    }

    private IEnumerable<IEditorTreeNode> Scripts()
    {
      yield return new EditorTreeNode()
      {
        Name = "--------------------------"
      };
      yield return new EditorTreeNode()
      {
        Name = "AML Cookbook",
        ImageKey = "folder-special-16",
        Description = "Useful AML Scripts",
        HasChildren = true,
        Children = Enumerable.Repeat(new EditorTreeNode()
        {
          Name = "Exports",
          ImageKey = "folder-special-16",
          Description = "Metadata Export Scripts",
          HasChildren = true,
          Children = _exports.Select(s => new EditorTreeNode()
          {
            Name = s.Name,
            ImageKey = "xml-tag-16",
            HasChildren = false,
            Scripts = Enumerable.Repeat(s, 1)
          })
        }, 1).Concat(_scripts.Select(s => new EditorTreeNode()
        {
          Name = s.Name,
          ImageKey = "xml-tag-16",
          HasChildren = false,
          Scripts = Enumerable.Repeat(s, 1)
        }))
      };
      yield return new EditorTreeNode()
      {
        Name = "Item Types",
        ImageKey = "folder-special-16",
        HasChildren = true,
        Children = ArasMetadataProvider.Cached(_conn).ItemTypes
          .Where(i => !i.IsRelationship)
          .Select(ItemTypeNode)
          .OrderBy(n => n.Name)
      };
      yield return new EditorTreeNode()
      {
        Name = "Relationship Types",
        ImageKey = "folder-special-16",
        HasChildren = true,
        Children = ArasMetadataProvider.Cached(_conn).ItemTypes
          .Where(i => i.IsRelationship)
          .Select(ItemTypeNode)
          .OrderBy(n => n.Name)
      };
    }

    private IEditorTreeNode ProcessTreeNode(IReadOnlyItem item)
    {
      switch (item.Classification().Value.ToLowerInvariant())
      {
        case "tree node/savedsearchintoc":
          return new EditorTreeNode()
          {
            Name = item.Property("label").Value,
            Description = "Saved Search",
            ImageKey = "xml-tag-16",
            HasChildren = item.Relationships("Tree Node Child").Any(),
            Children = item.Relationships().Select(r => ProcessTreeNode(r.RelatedItem())),
            ScriptGetter = () => Enumerable.Repeat(
              new EditorScript() {
                Name = item.Property("label").Value,
                Action = "ApplyItem",
                Script = _conn.Apply("<Item type='SavedSearch' action='get' select='criteria' id='@0' />", item.Property("saved_search_id").Value)
                  .AssertItem().Property("criteria").Value
              }, 1)
          };
        case "tree node/itemtypeintoc":
          return new EditorTreeNode()
          {
            Name = item.Property("label").Value,
            ImageKey = "class-16",
            Description = "ItemType: " + item.Property("name").Value,
            HasChildren = true,
            Children = ItemTypeChildren(item.Property("itemtype_id").Value)
            .Concat(item.Relationships().Select(r => ProcessTreeNode(r.RelatedItem())))
          };
        default:
          return new EditorTreeNode()
          {
            Name = item.Property("label").Value,
            ImageKey = "folder-16",
            HasChildren = item.Relationships("Tree Node Child").Any(),
            Children = item.Relationships().Select(r => ProcessTreeNode(r.RelatedItem()))
          };
      }
    }

    private IEnumerable<IEditorTreeNode> ItemTypeChildren(string typeId)
    {
      var result = new List<IEditorTreeNode>() {
        new EditorTreeNode()
        {
          Name = "Properties",
          ImageKey = "folder-16",
          HasChildren = true,
          ChildGetter = () => ArasMetadataProvider.Cached(_conn)
            .GetPropertiesByTypeId(typeId).Wait()
            .Select(p => new EditorTreeNode()
            {
              Name = p.Label ?? p.Name,
              ImageKey = "property-16",
              Description = GetPropertyDescription(p)
            })
            .OrderBy(n => n.Name)
        }
      };
      var rels = ArasMetadataProvider.Cached(_conn).TypeById(typeId).Relationships;
      if (rels.Any())
      {
        result.Add(new EditorTreeNode()
        {
          Name = "Relationships",
          ImageKey = "folder-16",
          HasChildren = true,
          ChildGetter = () => ArasMetadataProvider.Cached(_conn)
            .TypeById(typeId).Relationships
            .Select(ItemTypeNode)
            .OrderBy(n => n.Name)
        });
      }
      return result;
    }

    private EditorTreeNode ItemTypeNode(ItemType itemType)
    {
      return new EditorTreeNode()
      {
        Name = itemType.Label ?? itemType.Name,
        ImageKey = "class-16",
        Description = "ItemType: " + itemType.Name,
        HasChildren = true,
        Children = ItemTypeChildren(itemType.Id),
        Scripts = new EditorScript[]
        {
          new EditorScript()
          {
            Name = "SQL Select",
            Action = "ApplySQL",
            ScriptGetter = () => SqlSelect(itemType)
          }
        }
      };
    }

    private string SqlSelect(ItemType itemType)
    {
      var metadata = ArasMetadataProvider.Cached(_conn);
      var props = metadata
        .GetProperties(itemType)
        .Wait();

      var script = new StringBuilder("<sql>").AppendLine().AppendLine("select");
      var relations = new StringBuilder();
      var aliases = new Dictionary<string, string>();
      var primary = GetAlias(itemType, aliases);
      string alias;
      var first = true;

      foreach (var prop in props.OrderBy(p => p.Label ?? p.Name))
      {
        if (string.Equals(prop.TypeName, "federated", StringComparison.OrdinalIgnoreCase))
        {
          script.Append("  /* ").Append(prop.Name);
          if (!string.IsNullOrEmpty(prop.Label))
          {
            script.Append(" (").Append(prop.Label).Append(")");
          }
          script.Append(" is federated */");
        }
        else
        {
          if (first)
          {
            script.Append("  ");
            first = false;
          }
          else
          {
            script.Append(", ");
          }

          if (!string.IsNullOrEmpty(prop.ForeignTypeName))
          {
            alias = GetAlias(metadata.ItemTypeByName(prop.ForeignTypeName), aliases, (n, a) =>
            {
              relations.Append("inner join innovator.[").Append(n.Replace(' ', '_')).Append("] ")
                .AppendLine(a);
              relations.Append("on ").Append(primary).Append(".").Append(prop.ForeignLinkPropName)
                .Append(" = ").Append(a).AppendLine(".id");
            });
            script.Append(alias).Append(".").Append(prop.ForeignPropName);
          }
          else
          {
            script.Append(primary).Append(".").Append(prop.Name);
          }

          if (!string.IsNullOrEmpty(prop.Label))
          {
            script.Append(" \"").Append(prop.Label).Append("\"");
          }
        }
        script.AppendLine();
      }
      script.Append("from innovator.[").Append(itemType.Name.Replace(' ', '_')).Append("] ")
        .Append(primary).AppendLine();
      script.Append(relations.ToString());
      script.Append("where ").Append(primary).AppendLine(".is_current = 1");
      script.AppendLine("</sql>");
      return script.ToString();
    }

    private string GetAlias(ItemType itemType, Dictionary<string, string> existing
      , Action<string, string> itemAdded = null)
    {
      var mapping = existing.FirstOrDefault(k => k.Value == itemType.Name);
      if (!string.IsNullOrEmpty(mapping.Key))
        return mapping.Key;

      var words = (itemType.Label ?? itemType.Name).Split(' ');
      var aliasBase = (words.FirstOrDefault(w => w.Length == 3)
        ?? string.Join("", words.Select(w => w[0].ToString()))).ToLowerInvariant();
      var alias = aliasBase;
      var i = 1;

      while (existing.ContainsKey(alias))
      {
        alias = aliasBase + i.ToString();
        i++;
      }
      existing[alias] = itemType.Name;
      if (itemAdded != null)
        itemAdded(itemType.Name, alias);
      return alias;
    }


    private string GetPropertyDescription(Property prop)
    {
      var builder = new StringBuilder("Property ")
        .Append(prop.Name)
        .Append(": ")
        .Append(prop.TypeName);
      if (prop.Restrictions.Any())
      {
        builder.Append("[").Append(prop.Restrictions.First()).Append("]");
      }
      else if (prop.StoredLength > 0)
      {
        builder.Append("[").Append(prop.StoredLength).Append("]");
      }
      else if (prop.Precision > 0 || prop.Scale > 0)
      {
        builder.Append("[").Append(prop.Precision).Append(",").Append(prop.Scale).Append("]");
      }
      return builder.ToString();
    }

    #region "Scripts"
    private static EditorScript[] _scripts = new EditorScript[] {
      new EditorScript() {
        Name = "Add a File History Item",
        Action = "ApplyAML",
        Script = @"<!--
@id = item ID
@action = 'FileView', 'FileCheckin', or 'FileCheckout'
@filename = Name of the file, or other data
-->
<AML>
  <Item type='File' action='AddHistory' id='@id'>
    <action>@action</action>
    <filename>@filename</filename>
  </Item>
</AML>"
      },
      new EditorScript() {
        Name = "Add a Form History Item",
        Action = "ApplyAML",
        Script = @"<!--
@type = Item Type
@id = item ID
@action = 'FormView' or 'FormPrint'
@form_name = Name of the form, or other data
-->
<AML>
  <Item type='@type' action='AddHistory' id='@id'>
    <action>@action</action>
    <form_name>@form_name</form_name>
  </Item>
</AML>"
      },
      new EditorScript() {
        Name = "Generate GUIDs",
        Action = "GenerateNewGUIDEx",
        Script = @"<!--
@quantity = number of GUIDs to generate
-->
<Item quantity='@quantity' />"
      },
      new EditorScript() {
        Name = "Get InBasket Items",
        Action = "GetAssignedTasks",
        Script = @"<!--
@inBasketViewMode = Enumeration: Active|Both|Pending
@workflowTasks = Boolean: 0 or 1 indicating whether to include
@projectTasks = Boolean: 0 or 1 indicating whether to include
@actionTasks = Boolean: 0 or 1 indicating whether to include
@userID = [Optional] GUID ID for the user whose InBasket is to be retrieved.
-->
<params>
  <inBasketViewMode>@inBasketViewMode</inBasketViewMode>
  <workflowTasks>@workflowTasks</workflowTasks>
  <projectTasks>@projectTasks</projectTasks>
  <actionTasks>@actionTasks</actionTasks>
  <userID>@userID</userID>
</params>"
      },
      new EditorScript() {
        Name = "Get Multiple Items by ID",
        Action = "ApplyItem",
        Script = @"<!--
@type = Item Type
@ids = comma-separated list of ids (e.g. 'F13AF7BC3D7A4084AF67AB7BF938C409,A73B655731924CD0B027E4F4D5FCC0A9')
-->
<Item type='@type' idlist='@ids' action='get'></Item>"
      },
      new EditorScript() {
        Name = "Get Recursive Identity List for Current User",
        Action = "GetIdentityList",
        Script = @"<Item/>"
      },
      new EditorScript() {
        Name = "Get Recursive Tree (e.g. BOM)",
        Action = "ApplyItem",
        Script = @"<!--
@partId = ID of the parent part
-->
<Item type='Part' select='item_number' action='GetItemRepeatConfig' id='@partId'>
  <Relationships>
    <Item type='Part BOM' select='related_id,quantity' repeatProp='related_id' repeatTimes='10'/>
  </Relationships>
</Item>"
      },
      new EditorScript() {
        Name = "Get Structure Browser Data",
        Action = "ApplyItem",
        Script = @"<!--
@type = Item Type
@id = ID
@levels = # of levels to retrieve
-->
<Item type='Method' action='GetItemsForStructureBrowser'>
  <Item type='@type' id='@id' action='GetItemsForStructureBrowser' levels='@levels' />
</Item>"
      },
      new EditorScript() {
        Name = "Get TOC Items",
        Action = "GetMainTreeItems",
        Script = @"<Item/>"
      },
      new EditorScript() {
        Name = "Get User Item Permissions",
        Action = "ApplyAML",
        Script = @"<!--
@type = Item Type
@id = ID
@access_type = Permission to check for (e.g. 'can_add', 'can_update', 'can_get', 'can_delete', etc.)
-->
<AML>
  <Item access_type='@access_type' action='getPermissions' id='@id' type='@type' />
</AML>"
      },
      new EditorScript() {
        Name = "Get Valid Promotion States",
        Action = "ApplyAML",
        Script = @"<!--
@type = Item Type
@id = ID
@levels = # of levels to retrieve
-->
<AML>
  <Item type='@type' action='getItemNextStates' id='@id'></Item>
</AML>"
      },
      new EditorScript() {
        Name = "Get Where Used Data",
        Action = "ApplyAML",
        Script = @"<!--
@levels = # of levels to retrieve
-->
<Item type='@type' id='@id' action='getItemWhereUsed' />"
      },
      new EditorScript() {
        Name = "Promote an Item",
        Action = "ApplyAML",
        Script = @"<!--
@type = Item Type
@id = ID
@state = State
@comments = Comments
-->
<AML>
  <Item type='@type' action='promoteItem' id='@id'>
    <state>@state</state>
    <comments>@comments</comments>
  </Item>
</AML>"
      },
      new EditorScript() {
        Name = "Run a Report",
        Action = "ApplyItem",
        Script = @"<!--
@reportName = Report Name
@AML = AML to Retreive Item.  Note that a typeId attribute is often required or errors can occur.
-->
<Item type='Method' action='Run Report'>
  <report_name>@reportName</report_name>
  <AML>@AML!</AML>
</Item>"
      },
      new EditorScript() {
        Name = "Instantiate a Workflow",
        Action = "ApplyAML",
        Script = @"<!--
@type = Item Type
@id = ID
@WorkflowMap = Workflow Map ID
-->
<AML>
  <Item type='@type' id='@id' action='instantiateWorkflow'>
    <WorkflowMap>@WorkflowMap</WorkflowMap>
  </Item>
</AML>"
      },
      new EditorScript() {
        Name = "Update an Activity / Perform a Vote",
        Action = "ApplyAML",
        Script = @"<!--
@activity = Activity ID
@assignment = Assignment ID
@pathId = ID of the chosen Path
@pathName = Name of the chosen Path
@delegateTo = Either '0' or the ID of the user to delegate to.
@taskId = ID of the task
@taskCompleted = '0' or '1' indicating if the task was completed
@varId = ID of the variable
@varValue = Value of the variable
@authMode = Type of authentication (e.g. e-signature)
@authPassword = Password for the type of authentication
@comments = Comments
@complete = '0' or '1' indicating if the activity is complete
-->
<AML>
  <Item type='Activity' action='EvaluateActivity'>
    <Activity>@activity</Activity>
    <ActivityAssignment>@assignment</ActivityAssignment>
    <Paths>
      <Path id='@pathId'>@pathName</Path>
    </Paths>
    <DelegateTo>@delegateTo</DelegateTo>
    <Tasks>
      <Task id='@taskId' completed='@taskCompleted'></Task>
    </Tasks>
    <Variables>
      <Variable id='@varId'>@varValue</Variable>
    </Variables>
    <Authentication mode='@authMode'>@authPassword</Authentication>
    <Comments>@comments</Comments>
    <Complete>@complete</Complete>
  </Item>
</AML>"
      },
      new EditorScript() {
        Name = "Use the Current Time in a Query",
        Action = "ApplyAML",
        Script = @"<!--
The current time can be specified in an AML add query using the constant __now().
This constant is not available in other query types (e.g. get)

@type = Item Type
@id = ID
-->
<AML>
  <Item type='@type' id='@id' action='add'>
    <date_field>__now()</date_field>
  </Item>
</AML>"
      }
    };
    private static EditorScript[] _exports = new EditorScript[] {
      new EditorScript() {
        Name = "9.3 Metadata Export",
        Action = "ApplyAML",
        Script = @"<!-- Derived using the SQL below:
select '<Item type=''' + name + ''' action=''get'' select=''config_id''>'
  + case when name = 'Identity' then '<or><is_alias>0</is_alias><id condition=''in''>''DBA5D86402BF43D5976854B8B48FCDD1'',''E73F43AD85CD4A95951776D57A4D517B''</id></or>' else '' end
  + '</Item>' AML
from innovator.[ITEMTYPE]
where INSTANCE_DATA in (
  SELECT ta.name TableName
  FROM sys.tables ta
  INNER JOIN sys.partitions pa
  ON pa.OBJECT_ID = ta.OBJECT_ID
  INNER JOIN sys.schemas sc
  ON ta.schema_id = sc.schema_id
  WHERE ta.is_ms_shipped = 0
    AND pa.index_id IN (1,0)
    and ta.name in (select it.instance_data from innovator.ItemType it)
    and not ta.name in (select it.INSTANCE_DATA
      from innovator.[RELATIONSHIPTYPE] rt
      inner join innovator.[ITEMTYPE] it
      on it.id = rt.RELATIONSHIP_ID)
    and not ta.name in (
        'ACTIVITY2'
      , 'ACTIVITY_TEMPLATE'
      , 'APPLIED_UPDATES'
      , 'APQP_CAUSE_CATALOG'
      , 'APQP_DETECTION_CATALOG'
      , 'APQP_EFFECT_CATALOG'
      , 'APQP_FAILURE_MODE_CATALOG'
      , 'APQP_MEASUREMENT_TECHNIQUE'
      , 'APQP_OCCURRENCE_CATALOG'
      , 'APQP_SEVERITY_CATALOG'
      , 'BUSINESS_CALENDAR_YEAR'
      , 'CAD'
      , 'DATABASEUPGRADE'
      , 'HISTORY_CONTAINER'
      , 'MPROCESS_PLANNER'
      , 'PACKAGEDEFINITION'
      , 'PART'
      , 'PREFERENCE'
      , 'PROJECT'
      , 'PROJECT_TEMPLATE'
      , 'SAVEDSEARCH'
      , 'USER'
      , 'WBS_ELEMENT')
  GROUP BY sc.name,ta.name
  having SUM(pa.rows) <> 0)
order by 1
;
-->

<AML>
  <Item type='Action' action='get' select='config_id'></Item>
  <Item type='Chart' action='get' select='config_id'></Item>
  <Item type='Dashboard' action='get' select='config_id'></Item>
  <Item type='EMail Message' action='get' select='config_id'></Item>
  <Item type='FileType' action='get' select='config_id'></Item>
  <Item type='Form' action='get' select='config_id'></Item>
  <Item type='Grid' action='get' select='config_id'></Item>
  <Item type='History Action' action='get' select='config_id'></Item>
  <Item type='History Template' action='get' select='config_id'></Item>
  <Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>
  <Item type='ItemType' action='get' select='config_id'></Item>
  <Item type='Language' action='get' select='config_id'></Item>
  <Item type='Life Cycle Map' action='get' select='config_id'></Item>
  <Item type='List' action='get' select='config_id'></Item>
  <Item type='Locale' action='get' select='config_id'></Item>
  <Item type='Method' action='get' select='config_id'></Item>
  <Item type='Metric' action='get' select='config_id'></Item>
  <Item type='Permission' action='get' select='config_id'></Item>
  <Item type='PreferenceTypes' action='get' select='config_id'></Item>
  <Item type='Report' action='get' select='config_id'></Item>
  <Item type='Revision' action='get' select='config_id'></Item>
  <Item type='SearchMode' action='get' select='config_id'></Item>
  <Item type='Sequence' action='get' select='config_id'></Item>
  <Item type='SQL' action='get' select='config_id'></Item>
  <Item type='UserMessage' action='get' select='config_id'></Item>
  <Item type='Variable' action='get' select='config_id'></Item>
  <Item type='Vault' action='get' select='config_id'></Item>
  <Item type='Viewer' action='get' select='config_id'></Item>
  <Item type='Workflow Map' action='get' select='config_id'></Item>
</AML>"
      },new EditorScript() {
        Name = "11.0 Metadata Export",
        Action = "ApplyAML",
        Script = @"<!-- Derived using the SQL below:
select '<Item type=''' + name + ''' action=''get'' select=''config_id''>'
  + case when name = 'Identity' then '<or><is_alias>0</is_alias><id condition=''in''>''DBA5D86402BF43D5976854B8B48FCDD1'',''E73F43AD85CD4A95951776D57A4D517B''</id></or>' else '' end
  + '</Item>' AML
from innovator.[ITEMTYPE]
where INSTANCE_DATA in (
  SELECT ta.name TableName
  FROM sys.tables ta
  INNER JOIN sys.partitions pa
  ON pa.OBJECT_ID = ta.OBJECT_ID
  INNER JOIN sys.schemas sc
  ON ta.schema_id = sc.schema_id
  WHERE ta.is_ms_shipped = 0
    AND pa.index_id IN (1,0)
    and ta.name in (select it.instance_data from innovator.ItemType it)
    and not ta.name in (select it.INSTANCE_DATA
      from innovator.[RELATIONSHIPTYPE] rt
      inner join innovator.[ITEMTYPE] it
      on it.id = rt.RELATIONSHIP_ID)
    and not ta.name in (
        'ACTIVITY2'
      , 'ACTIVITY_TEMPLATE'
      , 'APPLIED_UPDATES'
      , 'APQP_CAUSE_CATALOG'
      , 'APQP_DETECTION_CATALOG'
      , 'APQP_EFFECT_CATALOG'
      , 'APQP_FAILURE_MODE_CATALOG'
      , 'APQP_MEASUREMENT_TECHNIQUE'
      , 'APQP_OCCURRENCE_CATALOG'
      , 'APQP_SEVERITY_CATALOG'
      , 'BUSINESS_CALENDAR_YEAR'
      , 'CAD'
      , 'DATABASEUPGRADE'
      , 'HISTORY_CONTAINER'
      , 'MPROCESS_PLANNER'
      , 'PACKAGEDEFINITION'
      , 'PART'
      , 'PREFERENCE'
      , 'PROJECT'
      , 'PROJECT_TEMPLATE'
      , 'SAVEDSEARCH'
      , 'USER'
      , 'WBS_ELEMENT')
  GROUP BY sc.name,ta.name
  having SUM(pa.rows) <> 0)
order by 1
;
-->

<AML>
  <Item type='Action' action='get' select='config_id'></Item>
  <Item type='Chart' action='get' select='config_id'></Item>
  <Item type='ConversionServer' action='get' select='config_id'></Item>
  <Item type='Dashboard' action='get' select='config_id'></Item>
  <Item type='EMail Message' action='get' select='config_id'></Item>
  <Item type='FileType' action='get' select='config_id'></Item>
  <Item type='Form' action='get' select='config_id'></Item>
  <Item type='Grid' action='get' select='config_id'></Item>
  <Item type='History Action' action='get' select='config_id'></Item>
  <Item type='History Template' action='get' select='config_id'></Item>
  <Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>
  <Item type='ItemType' action='get' select='config_id'></Item>
  <Item type='Language' action='get' select='config_id'></Item>
  <Item type='Life Cycle Map' action='get' select='config_id'></Item>
  <Item type='List' action='get' select='config_id'></Item>
  <Item type='Locale' action='get' select='config_id'></Item>
  <Item type='Method' action='get' select='config_id'></Item>
  <Item type='Metric' action='get' select='config_id'></Item>
  <Item type='Permission' action='get' select='config_id'></Item>
  <Item type='PreferenceTypes' action='get' select='config_id'></Item>
  <Item type='PresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='Report' action='get' select='config_id'></Item>
  <Item type='Revision' action='get' select='config_id'></Item>
  <Item type='SearchMode' action='get' select='config_id'></Item>
  <Item type='SecureMessageViewTemplate' action='get' select='config_id'></Item>
  <Item type='Sequence' action='get' select='config_id'></Item>
  <Item type='SQL' action='get' select='config_id'></Item>
  <Item type='SSVCPresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='SystemFileContainer' action='get' select='config_id'></Item>
  <Item type='Team' action='get' select='config_id'></Item>
  <Item type='UserMessage' action='get' select='config_id'></Item>
  <Item type='Variable' action='get' select='config_id'></Item>
  <Item type='Vault' action='get' select='config_id'></Item>
  <Item type='Viewer' action='get' select='config_id'></Item>
  <Item type='Workflow Map' action='get' select='config_id'></Item>
</AML>"
      }
    };
    #endregion
  }
}
