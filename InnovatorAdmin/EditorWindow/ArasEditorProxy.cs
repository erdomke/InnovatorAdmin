using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using System.Xml;
using System.IO;
using System.Data;

namespace InnovatorAdmin
{
  class ArasEditorProxy : IEditorProxy
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

    public IEnumerable<string> GetActions()
    {
      return _defaultActions;
    }

    public Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async)
    {
      var innCmd = request as InnovatorCommand;
      if (innCmd == null)
        throw new NotSupportedException("Cannot run commands created by a different proxy");

      var cmd = innCmd.Internal;
      if (cmd.Action == CommandAction.ApplyAML && cmd.Aml.IndexOf("<AML>") < 0)
      {
        cmd.Aml = "<AML>" + cmd.Aml + "</AML>";
      }
      return _conn.Process(cmd, async)
        .Convert(s => (IResultObject)new ResultObject(s.AsString())
        {
          PreferTable = cmd.Action == CommandAction.ApplySQL
        });
    }

    public IEditorProxy Clone()
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
      private string _aml;
      private int _count;
      private DataTable _table;

      public bool PreferTable { get; set; }

      public int ItemCount
      {
        get { return _count; }
      }

      public ResultObject(string aml)
      {
        _aml = IndentXml(aml, out _count);
      }

      public string GetText()
      {
        return _aml;
      }

      public System.Data.DataTable GetTable()
      {
        if (_table == null && !string.IsNullOrEmpty(_aml))
        {
          var doc = new XmlDocument();
          doc.LoadXml(_aml);
          _table = Extensions.GetItemTable(doc);
        }
        return _table;
      }

      private string IndentXml(string xmlContent, out int itemCount)
      {
        itemCount = 0;
        char[] writeNodeBuffer = null;
        var levels = new int[64];
        int level = 0;

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.CheckCharacters = true;

        using (var strReader = new StringReader(xmlContent))
        using (var reader = XmlReader.Create(strReader))
        using (var writer = new StringWriter())
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
                if (reader.IsEmptyElement)
                {
                  xmlWriter.WriteEndElement();
                }
                else
                {
                  level++;
                }
                break;
              case XmlNodeType.Text:
                if (canReadValueChunk)
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
                  xmlWriter.WriteString(reader.Value);
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
          return writer.ToString();
        }
      }
    }

    private class InnovatorCommand : ICommand
    {
      private Command _internal = new Command();

      public Command Internal
      {
        get { return _internal; }
      }

      public ICommand WithAction(string action)
      {
        _internal.WithAction(action);
        return this;
      }
      public ICommand WithParam(string name, object value)
      {
        _internal.WithParam(name, value);
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
        remote.Dispose();
    }
  }
}
