﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using System.Xml;
using System.IO;
using System.Data;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using InnovatorAdmin.Testing;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using InnovatorAdmin.Plugin;
using System.Linq.Expressions;

namespace InnovatorAdmin.Editor
{
  public class ArasEditorProxy : IEditorProxy
  {
    public static string UnitTestAction;
    private static Dictionary<string, Func<IPluginMethod>> _pluginFactory;

    #region "Default Actions"
    private static readonly string[] _baseActions = new string[] {
            "ActivateActivity",
            "AddItem",
            "ApplyAML",
            "ApplyItem",
            "ApplyMethod",
            "ApplySQL",
            "ApplyUpdate",
            "BuildProcessReport",
            "CancelWorkflow",
            "ChangeUserPassword",
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
            "DownloadFile",
            "EditItem",
            "EvaluateActivity",
            "ExecuteEscalations",
            "ExecuteReminders",
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
            "ValidateWorkflowMap"
            };
    #endregion

    private string[] _actions;
    private Editor.AmlEditorHelper _helper;
    private Version _version;

    public string Action
    {
      get { return _helper.SoapAction; }
      set { _helper.SoapAction = value; }
    }

    public Connections.ConnectionData ConnData { get; }

    public IAsyncConnection Connection { get; }

    public string Name { get; }


    public ArasEditorProxy(IAsyncConnection conn, string name)
    {
      Connection = conn;
      Initialize();
      _helper.InitializeConnection(Connection, null);
      Name = name;
    }

    public ArasEditorProxy(IAsyncConnection conn, Connections.ConnectionData connData)
    {
      Connection = conn;
      Initialize();
      ConnData = connData;
      _helper.InitializeConnection(Connection, ConnData);
      Name = connData.ConnectionName;
    }

    private void Initialize()
    {
      if (_pluginFactory == null)
      {
        UnitTestAction = "Plugin/" + typeof(RunUnitTests).Name;
        _pluginFactory = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(a => a.GetTypes())
          .Where(t => typeof(IPluginMethod).IsAssignableFrom(t)
            && t.GetConstructors().Any(c => c.GetParameters().Length == 0))
          .ToDictionary(p => "Plugin/" + p.Name
            , p => Expression.Lambda<Func<IPluginMethod>>(
                Expression.New(p.GetConstructor(Type.EmptyTypes))
              ).Compile());
      }

      _helper = new Editor.AmlEditorHelper();
      _version = Connection.FetchVersion(false).Wait() ?? new Version(12, 0);
      _actions = GetActions(_version.Major).OrderBy(a => a).ToArray();
    }

    private IEnumerable<string> GetActions(int version)
    {
      foreach (var action in _baseActions)
      {
        yield return action;
      }
      if (version < 10)
      {
        yield return "CacheDiag";
        yield return "CheckImportedItemType";
      }
      if (version < 11)
      {
        yield return "LoadCache";
        yield return "ResetServerCache";
        yield return "SaveCache";
        yield return "ExportItemType";
      }
      if (version <= 0 || version >= 10)
      {
        yield return "CreateFileExchangeTxn";
        yield return "ProcessFileTransferResult";
        yield return "StartFileExchangeTxn";
      }
      if (version <= 0 || version >= 11)
      {
        yield return "GetCheckUpdateInfo";
        yield return "VaultApplyAml";
        yield return "VaultApplyItem";
      }

      foreach (var plugin in _pluginFactory.Keys)
        yield return plugin;
    }

    public virtual IEnumerable<string> GetActions()
    {
      return _actions;
    }

    private async Task<IResultObject> ProcessPlugin(IPluginMethod method, string commands, Action<int, string> progressCallback)
    {
      var context = new PluginContext(new PluginConnection(Connection, commands)
        , progressCallback
        , ConnectionManager.Current.Library.Connections
            .Select(c => c.ArasCredentials())
            .Where(c => c != null));

      try
      {
        var result = await method.Execute(context).ConfigureAwait(false);
        var xmlReader = (result as PluginResult)?.CreateReader();
        if (xmlReader != null)
          return new ResultObject(xmlReader, Connection, "");
        else
          return new ResultObject(result.Write, result.Count, Connection);
      }
      catch (ServerException ex)
      {
        return new ResultObject(ex.CreateReader(), Connection, "");
      }
      catch (Exception ex)
      {
        return new ResultObject(Connection.AmlContext.ServerException(ex, true).CreateReader(), Connection, "");
      }
    }

    public virtual IPromise<IResultObject> Process(ICommand request, bool async, Action<int, string> progressCallback)
    {
      if (!(request is InnovatorCommand innCmd))
        throw new NotSupportedException("Cannot run commands created by a different proxy");

      var cmd = innCmd.Internal;

      // Process Unit Tests separately
      if (_pluginFactory.TryGetValue(cmd.ActionString, out var pluginInit))
        return ProcessPlugin(pluginInit(), cmd.Aml, progressCallback).ToPromise();

      var elem = XElement.Parse(cmd.Aml);
      var firstItem = elem.DescendantsAndSelf("Item").FirstOrDefault();
      string select = null;
      if (firstItem != null && (firstItem.Parent == null || firstItem.Parent.Elements("Item").Count() == 1))
      {
        select = firstItem.AttributeValue("select");
      }

      XElement[] queries;
      if (async && innCmd.ConcurrentCount > 0 && innCmd.StatementCount > 0)
      {
        var query = elem;
        while (!IsQueryTag(query) && query.Elements().Any())
          query = query.Elements().First();
        if (IsQueryTag(query))
        {
          if (query.Parent == null)
          {
            queries = new XElement[] { query };
          }
          else
          {
            var allQueries = query.Parent.Elements().Where(IsQueryTag).ToArray();
            // Switch to ApplyAML where necessary
            if (cmd.Action == CommandAction.ApplyItem && innCmd.StatementCount > 1)
              cmd.Action = CommandAction.ApplyAML;
            if (allQueries.Length <= innCmd.StatementCount)
              queries = new XElement[] { elem };
            else
              queries = allQueries.Batch(innCmd.StatementCount).Select(ConcatQueries).ToArray();
          }
        }
        else
        {
          queries = new XElement[] { elem };
        }
      }
      else
      {
        queries = new XElement[] { elem };
      }

      Func<Stream, IResultObject> getResult = s =>
      {
        var result = new ResultObject(s, Connection, select);
        if (cmd.Action == CommandAction.ApplySQL)
          result.PreferredMode = OutputType.Table;
        return (IResultObject)result;
      };

      if (cmd.Action == CommandAction.DownloadFile)
      {
        return DownloadFile(queries, cmd, innCmd, async).ToPromise();
      }
      else if (queries.Length == 1)
      {
        return ProcessCommand(GetCommand(queries[0], cmd, innCmd.Parameters, true), async)
          .Convert(getResult);
      }
      else
      {
        var remote = Connection as IRemoteConnection;
        if (remote == null)
          return Promises.Rejected<IResultObject>(new Exception("Connection must be an IRemoteConnection to use batch processing"));
        return new DataPromise(queries, innCmd.ConcurrentCount, remote
          , (q) => GetCommand(q, cmd, innCmd.Parameters, false)
          , getResult)
          .Progress(progressCallback);
      }
    }

    private class DataPromise : Promise<IResultObject>
    {
      private Stream _resultStream = new MemoryStream();
      private XmlWriter _writer;
      private int _totalCount;
      private int _completeCount = 0;
      private int _errorCount = 0;
      private int _started = 0;
      private object _lock = new object();
      private CancellationTokenSource _cts = new CancellationTokenSource();
      private Func<Stream, IResultObject> _getResult;

      public DataPromise(IList<XElement> queries, int concurrentCount, IRemoteConnection conn
        , Func<XElement, Command> getCommand
        , Func<Stream, IResultObject> getResult)
      {
        var settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true,
          IndentChars = "  "
        };
        _writer = XmlWriter.Create(_resultStream, settings);
        _writer.WriteStartElement("AsyncResult");
        _totalCount = queries.Count;
        _getResult = getResult;

        base.CancelTarget(this);

        ProcessPooled(queries, concurrentCount, getCommand, conn)
          .ContinueWith(t =>
          {
            if (t.IsFaulted)
            {
              Exception ex = t.Exception;
              if (ex != null && ex.InnerException != null)
                ex = ex.InnerException;
              base.Reject(ex);
            }
            else if (!t.IsCanceled && !_cts.IsCancellationRequested)
            {
              lock (_lock)
              {
                _writer.WriteEndElement();
                _writer.Flush();
                _resultStream.Position = 0;
              }
              base.Resolve(getResult(_resultStream));
            }
          });
      }

      public override void Cancel()
      {
        if (!_cts.IsCancellationRequested)
        {
          _cts.Cancel();
          lock (_lock)
          {
            _writer.WriteStartElement("Canceled");
            _writer.WriteAttributeString("at", DateTime.Now.ToString("s"));
            _writer.WriteAttributeString("batchesStarted", _started.ToString() + " of " + _totalCount.ToString());
            _writer.WriteAttributeString("batchesFinished", _completeCount.ToString());
            _writer.WriteEndElement();
            _writer.WriteEndElement();
            _writer.Flush();
            _resultStream.Position = 0;
            base.Resolve(_getResult(_resultStream));
          }
        }
      }

      private void RecordResult(IReadOnlyResult result, XElement query)
      {
        if (!_cts.IsCancellationRequested)
        {
          lock (_lock)
          {
            _completeCount++;
            if (result.Exception == null)
            {
              if (result.Items().Any())
              {
                foreach (var item in result.Items())
                {
                  item.ToAml(_writer);
                }
              }
              else
              {
                _writer.WriteElementString("Result", result.Value);
              }
            }
            else
            {
              _writer.WriteStartElement("Error");
              result.Exception.ToAml(_writer);
              _writer.WriteStartElement("Query");
              query.WriteTo(_writer);
              _writer.WriteEndElement();
              _writer.WriteEndElement();
              _errorCount++;
            }
            Notify(_completeCount * 100 / _totalCount, _errorCount + " error(s), " + (_completeCount - _errorCount) + " success(es)");
          }
        }
      }

      private async Task ProcessPooled(IList<XElement> queries, int concurrentCount, Func<XElement, Command> getCommand, IRemoteConnection conn)
      {
        // now let's send HTTP requests to each of these URLs in parallel
        var allTasks = new List<Task>();
        var throttler = new SemaphoreSlim(initialCount: concurrentCount);
        var factory = conn.AmlContext;
        var pool = await ConnectionPool.Create(conn, concurrentCount).ToTask(_cts.Token);

        foreach (var query in queries)
        {
          // do an async wait until we can schedule again
          await throttler.WaitAsync();

          // using Task.Run(...) to run the lambda in its own parallel
          // flow on the threadpool
          allTasks.Add(Task.Run(async () =>
          {
            try
            {
              Interlocked.Increment(ref _started);
              var cmd = getCommand(query);
              var stream = await pool.Process(cmd, true).ToTask(_cts.Token);
              RecordResult(factory.FromXml(stream, null, conn), query);
            }
            catch (Exception ex)
            {
              _writer.WriteStartElement("Error");
              _writer.WriteStartElement("Details");
              _writer.WriteCData(ex.ToString());
              _writer.WriteEndElement();
              _writer.WriteStartElement("Query");
              query.WriteTo(_writer);
              _writer.WriteEndElement();
              _writer.WriteEndElement();
              _errorCount++;
            }
            finally
            {
              throttler.Release();
            }
          }, _cts.Token));

          if (_cts.IsCancellationRequested)
            return;
        }

        // won't get here until all urls have been put into tasks
        await Task.WhenAll(allTasks);

        // won't get here until all tasks have completed in some way
        // (either success or exception)
      }
    }

    private Command GetCommand(XElement elem, Command orig, Dictionary<string, object> parameters, bool allowReuse)
    {
      // Check for file uploads and process if need be
      Command result;
      var files = elem.DescendantsAndSelf("Item")
        .Where(e => e.Attributes("type").Any(a => a.Value == "File")
                  && e.Elements("actual_filename").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("id").Any(p => !string.IsNullOrEmpty(p.Value))
                  && e.Attributes("action").Any(p => !string.IsNullOrEmpty(p.Value)));
      if (files.Any())
      {
        var upload = Connection.CreateUploadCommand();
        upload.AddFileQuery(orig.Aml);
        upload.WithAction(orig.ActionString);
        foreach (var param in parameters)
        {
          upload.WithParam(param.Key, param.Value);
        }
        result = upload;
      }
      else if (allowReuse)
      {
        result = orig;
      }
      else
      {
        result = new Command(elem.ToString()).WithAction(orig.ActionString);
        foreach (var param in parameters)
        {
          result.WithParam(param.Key, param.Value);
        }
      }
      var firstItem = elem.DescendantsAndSelf("Item").FirstOrDefault();
      string select = null;
      if (firstItem != null && (firstItem.Parent == null || firstItem.Parent.Elements("Item").Count() == 1))
      {
        select = firstItem.AttributeValue("select");
      }

      if (result.Action == CommandAction.ApplyAML && result.Aml.IndexOf("<AML>") < 0)
      {
        result.Aml = "<AML>" + result.Aml + "</AML>";
      }
      return result;
    }

    private bool IsQueryTag(XElement elem)
    {
      return elem.Name.LocalName == "Item" || elem.Name.LocalName == "sql" || elem.Name.LocalName == "SQL";
    }

    private XElement ConcatQueries(IEnumerable<XElement> elements)
    {
      if (elements == null || !elements.Any())
        throw new NotSupportedException();
      if (elements.First().Name.LocalName == "Item")
      {
        var result = new XElement("AML");
        foreach (var elem in elements)
        {
          result.Add(elem);
        }
        return result;
      }
      else
      {
        var builder = new StringBuilder();
        foreach (var elem in elements)
        {
          builder.AppendLine(elem.Value);
        }
        return new XElement("sql", builder.ToString());
      }
    }

    protected virtual IPromise<Stream> ProcessCommand(Command cmd, bool async)
    {
      return Connection.Process(cmd, async);
    }

    public virtual IEditorProxy Clone()
    {
      return new ArasEditorProxy(Connection, ConnData);
    }

    public ICommand NewCommand()
    {
      return new InnovatorCommand();
    }
    public Editor.IEditorHelper GetHelper()
    {
      return _helper;
    }


    private async Task<IResultObject> DownloadFile(XElement[] queries, Command cmd, InnovatorCommand innCmd, bool async)
    {
      if (queries.Length != 1)
        throw new NotSupportedException("Can only download a single file");
      if (queries[0].Attribute("type").Value != "File")
        throw new NotSupportedException("Can only download Items of type 'File'");

      queries[0].SetAttributeValue("action", "get");
      queries[0].SetAttributeValue("select", "filename,mimetype");
      var fileCmd = GetCommand(queries[0], cmd, innCmd.Parameters, true);
      var fileData = await ProcessCommand(fileCmd, async).ToTask();
      fileCmd.Action = CommandAction.ApplyItem;
      var metadata = Connection.AmlContext.FromXml(await ProcessCommand(fileCmd, async).ToTask(), fileCmd.Aml, Connection).AssertItem();
      var fileName = metadata.Property("filename").Value;
      var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "\\" + fileName);
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      using (var dest = new FileStream(path, FileMode.Create, FileAccess.Write))
      {
        await fileData.CopyToAsync(dest);
      }
      var result = new DownloadResult();
      result.Html = string.Format(@"<html><body><a href='file:///{0}' style='font-size:200%'>{1}</a></body></html>", path.Replace('\\', '/'), fileName);
      return result;
    }

    private class DownloadResult : IResultObject
    {
      public string Html { get; set; }

      public int ItemCount { get { return 1; } }

      public OutputType PreferredMode
      {
        get { return OutputType.Html; }
      }

      public string Title { get; set; }

      public DataSet GetDataSet()
      {
        throw new NotSupportedException();
      }

      public ITextSource GetTextSource()
      {
        return new StringTextSource(this.Html);
      }
    }

    private class ResultObject : IResultObject
    {
      private readonly ITextSource _text;
      private readonly int _count;
      private DataSet _dataSet;
      private readonly int _amlLength;
      private string _title;
      private readonly IAsyncConnection _conn;
      private OutputType _preferredMode = OutputType.Text;
      private string _html;
      private readonly string _select;

      public int ItemCount
      {
        get { return _count; }
      }

      public ResultObject(Action<TextWriter> writer, int count, IAsyncConnection conn)
      {
        var rope = new Rope<char>();
        using (var textWriter = new Editor.RopeWriter(rope))
        {
          writer(textWriter);
        }
        _amlLength = rope.Length;
        _count = count;
        _conn = conn;
        _text = new RopeTextSource(rope);
        _dataSet = new DataSet();
      }

      public ResultObject(Stream aml, IAsyncConnection conn, string select)
        : this(XmlReader.Create(aml), conn, select) { }

      public ResultObject(XmlReader reader, IAsyncConnection conn, string select)
      {
        _conn = conn;
        var rope = new Rope<char>();
        using (var writer = new Editor.RopeWriter(rope))
        {
          IndentXml(reader, writer, out _count);
        }
        _amlLength = rope.Length;
        _text = new RopeTextSource(rope);
        _select = select;
      }

      public ITextSource GetTextSource()
      {
        return _text;
      }

      public System.Data.DataSet GetDataSet()
      {
        if (_dataSet == null && _amlLength > 0)
        {
          var doc = new XmlDocument();
          doc.Load(_text.CreateReader());
          _dataSet = Extensions.GetItemTable(_conn.AmlContext.FromXml(new XmlNodeReader(doc.DocumentElement))
            , ArasMetadataProvider.Cached(_conn), _select);
        }
        return _dataSet;
      }



      private void IndentXml(XmlReader reader, TextWriter writer, out int itemCount)
      {
        itemCount = 0;
        char[] writeNodeBuffer = null;
        var levels = new int[64];
        var elemIds = new List<string>();
        string value;

        var types = new HashSet<string>();

        using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true,
          IndentChars = "  ",
          CheckCharacters = true,
          ConformanceLevel = ConformanceLevel.Fragment
        }))
        {
          bool canReadValueChunk = reader.CanReadValueChunk;
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                xmlWriter.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                xmlWriter.WriteAttributes(reader, false);

                if (reader.LocalName == "Item")
                {
                  levels[elemIds.Count]++;
                  var currType = reader.GetAttribute("type");
                  if (!string.IsNullOrWhiteSpace(currType)) types.Add(currType);
                }
                else if (reader.LocalName == "id" && _title == null)
                {
                  _title = reader.GetAttribute("type") + " " + reader.GetAttribute("keyed_name");
                }

                if (reader.IsEmptyElement)
                {
                  xmlWriter.WriteEndElement();
                }
                else
                {
                  if (reader.LocalName == "Item")
                  {
                    var currType = reader.GetAttribute("type");
                    elemIds.Add(string.IsNullOrWhiteSpace(currType) ? reader.LocalName : currType);
                  }
                  else
                  {
                    elemIds.Add(reader.LocalName);
                  }
                }
                break;
              case XmlNodeType.Text:
                var textReader = canReadValueChunk
                    ? (TextReader)new XmlValueReader(reader)
                    : new StringReader(reader.Value);

                var formatXml = elemIds.EndsWith("Action", "item_query")
                  || elemIds.EndsWith("EMail Message", "query_string")
                  || elemIds.EndsWith("Grid", "query")
                  || elemIds.EndsWith("Report", "report_query")
                  || elemIds.EndsWith("Report", "xsl_stylesheet")
                  || elemIds.EndsWith("SavedSearch", "criteria")
                  || elemIds.EndsWith("ItemType", "class_structure")
                  || elemIds.EndsWith("qry_QueryReference", "filter_xml")
                  || elemIds.EndsWith("qry_QueryItem", "filter_xml")
                  || elemIds.EndsWith("mp_MacCondition", "condition_xml")
                  || elemIds.EndsWith("tp_Block", "content")
                  || elemIds.EndsWith("Fault", "faultstring");
                var useCData = elemIds.EndsWith("EMail Message", "body_html")
                  || elemIds.EndsWith("EMail Message", "body_plain")
                  || elemIds.EndsWith("Method", "method_code")
                  || elemIds.EndsWith("SQL", "sqlserver_body")
                  || (elemIds.Last() != "Result" && textReader.Peek() == '<' && (textReader as XmlValueReader)?.ReadLength > 1028);

                if (formatXml)
                {
                  xmlWriter.WriteCData(FormatXml(reader, textReader));
                }
                else if (useCData)
                {
                  xmlWriter.WriteCData(textReader.ReadToEnd());
                }
                else if (elemIds.Last() == "Result" && textReader.Peek() == '<')
                {
                  _html = textReader.ReadToEnd();
                  _preferredMode = OutputType.Html;
                  xmlWriter.WriteString(_html);
                }
                else
                {
                  if (writeNodeBuffer == null)
                  {
                    writeNodeBuffer = new char[4096];
                  }
                  int count;
                  while ((count = textReader.Read(writeNodeBuffer, 0, writeNodeBuffer.Length)) > 0)
                  {
                    xmlWriter.WriteChars(writeNodeBuffer, 0, count);
                  }
                }
                break;
              case XmlNodeType.CDATA:
                if (elemIds.EndsWith("Fault", "faultstring"))
                {
                  var txtReader = canReadValueChunk
                    ? (TextReader)new XmlValueReader(reader)
                    : new StringReader(reader.Value);
                  xmlWriter.WriteCData(FormatXml(reader, txtReader));
                }
                else
                {
                  xmlWriter.WriteCData(reader.Value);
                }
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
              case XmlNodeType.SignificantWhitespace:
                xmlWriter.WriteWhitespace(reader.Value);
                break;
              case XmlNodeType.EndElement:
                xmlWriter.WriteFullEndElement();
                elemIds.RemoveAt(elemIds.Count - 1);
                break;
            }
          }

          xmlWriter.Flush();
          itemCount = Array.Find(levels, i => i > 0);
          writer.Flush();
        }

        if (itemCount > 1)
        {
          ItemType itemType;
          if (types.Count == 1 && ArasMetadataProvider.Cached(_conn).ItemTypeByName(types.First(), out itemType))
          {
            _title = _count + " " + itemType.Label ?? itemType.Name + " Item(s)";
          }
          else
          {
            _title = _count + " Item(s)";
          }
        }
      }

      private string FormatXml(XmlReader reader, TextReader textReader)
      {
        try
        {
          var w = new StringWriter();
          using (var r = XmlReader.Create(textReader, new XmlReaderSettings()
          {
            ConformanceLevel = ConformanceLevel.Fragment
          }))
          using (var xml = XmlWriter.Create(w, new XmlWriterSettings()
          {
            OmitXmlDeclaration = true,
            Indent = true,
            IndentChars = "  ",
            CheckCharacters = true,
            ConformanceLevel = ConformanceLevel.Fragment
          }))
          {
            xml.WriteNode(r, false);
            xml.Flush();
            w.Flush();
          }
          return w.ToString();
        }
        catch (XmlException)
        {
          var textValue = reader.Value;
          if (string.IsNullOrEmpty(textValue))
            textValue = (textReader as XmlValueReader)?.ReadBufferToEnd();
          return textValue;
        }
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
      private Dictionary<string, object> _params = new Dictionary<string, object>();

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

      private int _concurrentCount;
      public int ConcurrentCount { get { return _concurrentCount; } }
      public ICommand WithConcurrentCount(int concurrentCount)
      {
        _concurrentCount = concurrentCount;
        return this;
      }

      private int _statementCount;
      public int StatementCount { get { return _statementCount; } }
      public ICommand WithStatementCount(int statementCount)
      {
        _statementCount = statementCount;
        return this;
      }
    }

    public void Dispose()
    {
      var remote = Connection as IRemoteConnection;
      if (remote != null)
        remote.Logout(true, true);
    }

    private string[] _itemTypeReportNames;

    public IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      var tocPromise = _version.Major >= 12
        ? Connection.ApplyAsync(@"<Item type='Method' action='GetCommandBarItems'>
  <location_name>TOC</location_name>
</Item>", true, false)
        : Connection.ApplyAsync(new Command("<Item/>").WithAction(CommandAction.GetMainTreeItems)
          , true, false);

      return Promises.All(tocPromise,
          Connection.ApplyAsync(@"<Item type='ItemType' action='get'>
                              <name>ItemType</name>
                              <Relationships>
                                <Item type='Item Report' action='get'>

                                </Item>
                              </Relationships>
                            </Item>", true, false),
          ArasMetadataProvider.Cached(Connection).ReloadPromise())
        .Convert(r =>
        {
          _itemTypeReportNames = ((IReadOnlyResult)r[1]).AssertItem().Relationships()
            .Select(rep => rep.RelatedItem().Property("name").Value).ToArray();
          var node = TocNode.FromXml(((IReadOnlyResult)r[0]).CreateReader());
          return node
            .Children
            .Select(ProcessTreeNode)
            .Concat(Scripts());
        });
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
        Image = Icons.FolderSpecial16,
        Description = "Useful AML Scripts",
        HasChildren = true,
        Children = Enumerable.Repeat(new EditorTreeNode()
        {
          Name = "Exports",
          Image = Icons.FolderSpecial16,
          Description = "Metadata Export Scripts",
          HasChildren = true,
          Children = _exports.Select(s => new EditorTreeNode()
          {
            Name = s.Name,
            Image = Icons.XmlTag16,
            HasChildren = false,
            Scripts = Enumerable.Repeat(s, 1)
          })
        }, 1).Concat(_scripts.Select(s => new EditorTreeNode()
        {
          Name = s.Name,
          Image = Icons.XmlTag16,
          HasChildren = false,
          Scripts = Enumerable.Repeat(s, 1)
        }))
      };
      yield return new EditorTreeNode()
      {
        Name = "Item Types",
        Image = Icons.FolderSpecial16,
        HasChildren = true,
        Children = ArasMetadataProvider.Cached(Connection).ItemTypes
          .Where(i => !i.IsRelationship)
          .Select(ItemTypeNode)
          .OrderBy(n => n.Name)
      };
      yield return new EditorTreeNode()
      {
        Name = "Relationship Types",
        Image = Icons.FolderSpecial16,
        HasChildren = true,
        Children = ArasMetadataProvider.Cached(Connection).ItemTypes
          .Where(i => i.IsRelationship)
          .Select(ItemTypeNode)
          .OrderBy(n => n.Name)
      };
    }

    private IEditorTreeNode ProcessTreeNode(TocNode node)
    {
      var savedSearch = node.References
        .FirstOrDefault(r => string.Equals(r.TypeName(), "Saved Search", StringComparison.OrdinalIgnoreCase));
      var itemType = node.References
        .FirstOrDefault(r => string.Equals(r.TypeName(), "ItemType", StringComparison.OrdinalIgnoreCase));
      if (savedSearch != null)
      {
        return new EditorTreeNode()
        {
          Name = node.Label,
          Description = "Saved Search",
          Image = Icons.XmlTag16,
          HasChildren = node.Children.Any(),
          Children = node.Children.Select(ProcessTreeNode),
          ScriptGetter = () => Enumerable.Repeat(
            new EditorScript()
            {
              Name = node.Label,
              Action = "ApplyItem",
              Script = Connection.Apply("<Item type='SavedSearch' action='get' select='criteria' id='@0' />", savedSearch.Id())
                .AssertItem().Property("criteria").Value
            }, 1)
        };
      }
      else if (itemType != null)
      {
        return new EditorTreeNode()
        {
          Name = node.Label,
          Image = Icons.Class16,
          Description = "ItemType: " + itemType.TypeName(),
          HasChildren = true,
          ScriptGetter = () => ItemTypeScripts(ArasMetadataProvider.Cached(Connection).ItemTypeById(itemType.Id())),
          Children = ItemTypeChildren(itemType.Id())
            .Concat(node.Children.Select(ProcessTreeNode))
        };
      }
      else
      {
        return new EditorTreeNode()
        {
          Name = node.Label,
          Image = Icons.Folder16,
          HasChildren = node.Children.Any(),
          Children = node.Children.Select(ProcessTreeNode)
        };
      }
    }

    private IEnumerable<IEditorTreeNode> ItemTypeChildren(string typeId)
    {
      var result = new List<IEditorTreeNode>() {
        new EditorTreeNode()
        {
          Name = "Properties",
          Image = Icons.Folder16,
          HasChildren = true,
          ChildGetter = () => ArasMetadataProvider.Cached(Connection)
            .GetPropertiesByTypeId(typeId).Wait()
            .Select(p => new EditorTreeNode()
            {
              Name = p.Label ?? p.Name,
              Image = Icons.Property16,
              HasChildren = p.Type == PropertyType.item && p.Restrictions.Any()
                && p.Name != "id" && p.Name != "config_id",
              ChildGetter = () => ItemTypeChildren(ArasMetadataProvider.Cached(Connection).ItemTypeByName(p.Restrictions.First()).Id),
              Description = $"Property {p.Name}: {p.TypeDisplay()}"
            })
            .OrderBy(n => n.Name)
        }
      };
      var rels = ArasMetadataProvider.Cached(Connection).ItemTypeById(typeId).Relationships;
      if (rels.Any())
      {
        result.Add(new EditorTreeNode()
        {
          Name = "Relationships",
          Image = Icons.Folder16,
          HasChildren = true,
          ChildGetter = () => ArasMetadataProvider.Cached(Connection)
            .ItemTypeById(typeId).Relationships
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
        Image = Icons.ForItemType(itemType),
        Description = "ItemType: " + itemType.Name,
        HasChildren = true,
        Children = ItemTypeChildren(itemType.Id),
        Scripts = ItemTypeScripts(itemType)
      };
    }
    public static EditorScript ItemTypeAddScript(IAsyncConnection conn, ItemType itemType)
    {
      return new EditorScript()
      {
        Name = "New " + (itemType.Label ?? itemType.Name),
        Action = "ApplyItem",
        ScriptGetter = async () =>
        {
          var builder = new StringBuilder();
          builder.AppendFormat("<Item type='{0}' action='add'>", itemType.Name).AppendLine();
          foreach (var prop in (await ArasMetadataProvider.Cached(conn).GetProperties(itemType).ToTask())
                                .Where(p => p.DefaultValue != null))
          {
            builder.Append("  <").Append(prop.Name).Append(">");
            builder.Append(conn.AmlContext.LocalizationContext.Format(prop.DefaultValue));
            builder.Append("</").Append(prop.Name).Append(">").AppendLine();
          }
          builder.Append("</Item>");
          return builder.ToString();
        }
      };
    }
    private IEnumerable<IEditorScript> ItemTypeScripts(ItemType itemType)
    {
      yield return new EditorScript()
      {
        Name = "List " + (itemType.Label ?? itemType.Name),
        Action = "ApplyItem",
        ScriptGetter = async () =>
        {
          var it = (await Connection.ApplyAsync(@"<Item type='ItemType' action='get' select='default_page_size'>
                                      <name>@0</name>
                                      <Relationships>
                                        <Item type='Property' action='get' select='default_search'>
                                        </Item>
                                      </Relationships>
                                    </Item>", true, false, itemType.Name).ToTask()).AssertItem();
          var builder = new StringBuilder();
          builder.AppendFormat("<Item type='{0}' action='get'", itemType.Name);
          if (it.Property("default_page_size").HasValue())
            builder.AppendFormat(" page='1' pagesize='{0}'", it.Property("default_page_size").Value);
          else
            builder.Append(" maxRecords='250'");
          builder.Append('>').AppendLine();
          foreach (var prop in it.Relationships().Where(p => p.Property("default_search").HasValue()))
          {
            builder.Append("  <").Append(prop.Name).Append(">");
            builder.Append(prop.Property("default_search").Value);
            builder.Append("</").Append(prop.Name).Append(">").AppendLine();
          }
          builder.Append("</Item>");
          return builder.ToString();
        },
        AutoRun = true,
        PreferredOutput = OutputType.Table
      };
      yield return ItemTypeAddScript(Connection, itemType);
      yield return new EditorScript()
      {
        Name = "--------------------------"
      };
      foreach (var report in _itemTypeReportNames)
      {
        yield return new EditorScript()
        {
          Name = report,
          Action = "ApplyItem",
          Script = @"<Item type='Method' action='Run Report'>
  <report_name>" + report + @"</report_name>
  <AML>
    <Item type='ItemType' typeId='450906E86E304F55A34B3C0D65C097EA' id='" + itemType.Id + @"' />
  </AML>
</Item>",
          AutoRun = true
        };
      }
      yield return new EditorScript()
      {
        Name = "ItemType: SQL Select",
        Action = "ApplySQL",
        ScriptGetter = () => SqlSelect(itemType)
      };
    }

    private async Task<string> SqlSelect(ItemType itemType)
    {
      var metadata = ArasMetadataProvider.Cached(Connection);
      var props = await metadata
        .GetProperties(itemType).ToTask();

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
        Name = "New ItemType",
        Action = "ApplyItem",
        Script = @"<Item type='ItemType' action='add' typeId='450906E86E304F55A34B3C0D65C097EA'>
  <name>@0</name>
  <allow_private_permission>1</allow_private_permission>
  <enforce_discovery>1</enforce_discovery>
  <implementation_type>table</implementation_type>
  <is_relationship>0</is_relationship>
  <is_versionable>0</is_versionable>
  <manual_versioning>0</manual_versioning>
  <revisions keyed_name='Default'>7FE395DD8B9F4E1090756A34B733D75E</revisions>
  <show_parameters_tab>1</show_parameters_tab>
  <Relationships>
    <Item type='TOC Access' action='add' typeId='38C9CE2A4E06401DABF942E1D0224E87'>
      <related_id>
        <Item type='Identity' action='get'>
          <name>World</name>
        </Item>
      </related_id>
      <category>Administration</category>
    </Item>
    <Item type='Can Add' action='add' typeId='3A65F41FF1FC42518A702FDA164AF420'>
      <can_add>1</can_add>
      <related_id>
        <Item type='Identity' action='get'>
          <name>World</name>
        </Item>
      </related_id>
    </Item>
    <Item type='Allowed Permission' action='add' typeId='DB54505FA3E9419DA3C1E1AFB7A48C1C'>
      <is_default>1</is_default>
      <related_id>
        <Item type='Permission' action='get'>
          <name>Default Access</name>
        </Item>
      </related_id>
    </Item>
  </Relationships>
</Item>"
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
      },new EditorScript() {
        Name = "11.0sp5 Metadata Export",
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
      , 'DESIGN_QUALITY_DOCUMENT'
      , 'FILE'
      , 'FILECONTAINERLOCATOR'
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
  <Item type='cmf_ContentType' action='get' select='config_id'></Item>
  <Item type='cmf_Style' action='get' select='config_id'></Item>
  <Item type='cmf_TabularView' action='get' select='config_id'></Item>
  <Item type='cmf_TabularViewHeaderRow' action='get' select='config_id'></Item>
  <Item type='CommandBarButton' action='get' select='config_id'></Item>
  <Item type='CommandBarDropDown' action='get' select='config_id'></Item>
  <Item type='CommandBarSection' action='get' select='config_id'></Item>
  <Item type='CommandBarSeparator' action='get' select='config_id'></Item>
  <Item type='ConversionRule' action='get' select='config_id'></Item>
  <Item type='ConversionServer' action='get' select='config_id'></Item>
  <Item type='ConverterType' action='get' select='config_id'></Item>
  <Item type='Dashboard' action='get' select='config_id'></Item>
  <Item type='EMail Message' action='get' select='config_id'></Item>
  <Item type='FileType' action='get' select='config_id'></Item>
  <Item type='Form' action='get' select='config_id'></Item>
  <Item type='GlobalPresentationConfig' action='get' select='config_id'></Item>
  <Item type='Grid' action='get' select='config_id'></Item>
  <Item type='History Action' action='get' select='config_id'></Item>
  <Item type='History Template' action='get' select='config_id'></Item>
  <Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>
  <Item type='ItemType' action='get' select='config_id'></Item>
  <Item type='Language' action='get' select='config_id'></Item>
  <Item type='Life Cycle Map' action='get' select='config_id'></Item>
  <Item type='List' action='get' select='config_id'></Item>
  <Item type='Locale' action='get' select='config_id'></Item>
  <Item type='Measurement Unit' action='get' select='config_id'></Item>
  <Item type='Method' action='get' select='config_id'></Item>
  <Item type='Metric' action='get' select='config_id'></Item>
  <Item type='Permission' action='get' select='config_id'></Item>
  <Item type='PreferenceTypes' action='get' select='config_id'></Item>
  <Item type='PresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='Report' action='get' select='config_id'></Item>
  <Item type='Revision' action='get' select='config_id'></Item>
  <Item type='SearchMode' action='get' select='config_id'></Item>
  <Item type='SecureMessageViewTemplate' action='get' select='config_id'></Item>
  <Item type='SelfServiceReportHelp' action='get' select='config_id'></Item>
  <Item type='Sequence' action='get' select='config_id'></Item>
  <Item type='SQL' action='get' select='config_id'></Item>
  <Item type='SSVCPresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='SystemFileContainer' action='get' select='config_id'></Item>
  <Item type='Team' action='get' select='config_id'></Item>
  <Item type='tp_XmlSchema' action='get' select='config_id'></Item>
  <Item type='UserMessage' action='get' select='config_id'></Item>
  <Item type='Variable' action='get' select='config_id'></Item>
  <Item type='Vault' action='get' select='config_id'></Item>
  <Item type='Viewer' action='get' select='config_id'></Item>
  <Item type='Workflow Map' action='get' select='config_id'></Item>
</AML>"
      },new EditorScript() {
        Name = "11.0sp9 Metadata Export",
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
      , 'DESIGN_QUALITY_DOCUMENT'
      , 'FILE'
      , 'FILECONTAINERLOCATOR'
      , 'HISTORY_CONTAINER'
      , 'MPROCESS_PLANNER'
      , 'PACKAGEDEFINITION'
      , 'PART'
      , 'PREFERENCE'
      , 'PROJECT'
      , 'PROJECT_TEMPLATE'
      , 'SAVEDSEARCH'
      , 'USER'
      , 'WBS_ELEMENT'
      , 'TEAM')
  GROUP BY sc.name,ta.name
  having SUM(pa.rows) <> 0)
order by 1
;
-->

<AML>
  <Item type='Action' action='get' select='config_id'></Item>
  <Item type='Chart' action='get' select='config_id'></Item>
  <Item type='cmf_ContentType' action='get' select='config_id'></Item>
  <Item type='cmf_Style' action='get' select='config_id'></Item>
  <Item type='cmf_TabularView' action='get' select='config_id'></Item>
  <Item type='cmf_TabularViewHeaderRow' action='get' select='config_id'></Item>
  <Item type='CommandBarButton' action='get' select='config_id'></Item>
  <Item type='CommandBarDropDown' action='get' select='config_id'></Item>
  <Item type='CommandBarEdit' action='get' select='config_id'></Item>
  <Item type='CommandBarMenu' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuButton' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuCheckbox' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuSeparator' action='get' select='config_id'></Item>
  <Item type='CommandBarSection' action='get' select='config_id'></Item>
  <Item type='CommandBarSeparator' action='get' select='config_id'></Item>
  <Item type='CommandBarShortcut' action='get' select='config_id'></Item>
  <Item type='ConversionRule' action='get' select='config_id'></Item>
  <Item type='ConversionServer' action='get' select='config_id'></Item>
  <Item type='ConverterType' action='get' select='config_id'></Item>
  <Item type='Dashboard' action='get' select='config_id'></Item>
  <Item type='EMail Message' action='get' select='config_id'></Item>
  <Item type='FileType' action='get' select='config_id'></Item>
  <Item type='Form' action='get' select='config_id'></Item>
  <Item type='GlobalPresentationConfig' action='get' select='config_id'></Item>
  <Item type='Grid' action='get' select='config_id'></Item>
  <Item type='History Action' action='get' select='config_id'></Item>
  <Item type='History Template' action='get' select='config_id'></Item>
  <Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>
  <Item type='ItemType' action='get' select='config_id'></Item>
  <Item type='Language' action='get' select='config_id'></Item>
  <Item type='Life Cycle Map' action='get' select='config_id'></Item>
  <Item type='List' action='get' select='config_id'></Item>
  <Item type='Locale' action='get' select='config_id'></Item>
  <Item type='Measurement Unit' action='get' select='config_id'></Item>
  <Item type='Method' action='get' select='config_id'></Item>
  <Item type='Metric' action='get' select='config_id'></Item>
  <Item type='Permission' action='get' select='config_id'></Item>
  <Item type='PreferenceTypes' action='get' select='config_id'></Item>
  <Item type='PresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='Report' action='get' select='config_id'></Item>
  <Item type='Revision' action='get' select='config_id'></Item>
  <Item type='SearchMode' action='get' select='config_id'></Item>
  <Item type='SecureMessageViewTemplate' action='get' select='config_id'></Item>
  <Item type='SelfServiceReportHelp' action='get' select='config_id'></Item>
  <Item type='Sequence' action='get' select='config_id'></Item>
  <Item type='SQL' action='get' select='config_id'></Item>
  <Item type='SSVCPresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='SystemFileContainer' action='get' select='config_id'></Item>
  <Item type='tp_XmlSchema' action='get' select='config_id'></Item>
  <Item type='UserMessage' action='get' select='config_id'></Item>
  <Item type='Variable' action='get' select='config_id'></Item>
  <Item type='Vault' action='get' select='config_id'></Item>
  <Item type='Viewer' action='get' select='config_id'></Item>
  <Item type='Workflow Map' action='get' select='config_id'></Item>
</AML>"
      },new EditorScript() {
        Name = "11.0sp12 Metadata Export",
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
      , 'DESIGN_QUALITY_DOCUMENT'
      , 'FILE'
      , 'FILECONTAINERLOCATOR'
      , 'HISTORY_CONTAINER'
      , 'MPROCESS_PLANNER'
      , 'PACKAGEDEFINITION'
      , 'PART'
      , 'PREFERENCE'
      , 'PROJECT'
      , 'PROJECT_TEMPLATE'
      , 'SAVEDSEARCH'
      , 'USER'
      , 'WBS_ELEMENT'
      , 'TEAM')
  GROUP BY sc.name,ta.name
  having SUM(pa.rows) <> 0)
order by 1
;
-->

<AML>
  <Item type='Action' action='get' select='config_id'></Item>
  <Item type='Chart' action='get' select='config_id'></Item>
  <Item type='cmf_ContentType' action='get' select='config_id'></Item>
  <Item type='cmf_Style' action='get' select='config_id'></Item>
  <Item type='cmf_TabularView' action='get' select='config_id'></Item>
  <Item type='cmf_TabularViewHeaderRow' action='get' select='config_id'></Item>
  <Item type='CommandBarButton' action='get' select='config_id'></Item>
  <Item type='CommandBarDropDown' action='get' select='config_id'></Item>
  <Item type='CommandBarEdit' action='get' select='config_id'></Item>
  <Item type='CommandBarMenu' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuButton' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuCheckbox' action='get' select='config_id'></Item>
  <Item type='CommandBarMenuSeparator' action='get' select='config_id'></Item>
  <Item type='CommandBarSection' action='get' select='config_id'></Item>
  <Item type='CommandBarSeparator' action='get' select='config_id'></Item>
  <Item type='CommandBarShortcut' action='get' select='config_id'></Item>
  <Item type='ConversionRule' action='get' select='config_id'></Item>
  <Item type='ConversionServer' action='get' select='config_id'></Item>
  <Item type='ConverterType' action='get' select='config_id'></Item>
  <Item type='Dashboard' action='get' select='config_id'></Item>
  <Item type='EMail Message' action='get' select='config_id'></Item>
  <Item type='FileType' action='get' select='config_id'></Item>
  <Item type='Form' action='get' select='config_id'></Item>
  <Item type='GlobalPresentationConfig' action='get' select='config_id'></Item>
  <Item type='Grid' action='get' select='config_id'></Item>
  <Item type='History Action' action='get' select='config_id'></Item>
  <Item type='History Template' action='get' select='config_id'></Item>
  <Item type='Identity' action='get' select='config_id'><or><is_alias>0</is_alias><id condition='in'>'DBA5D86402BF43D5976854B8B48FCDD1','E73F43AD85CD4A95951776D57A4D517B'</id></or></Item>
  <Item type='ItemType' action='get' select='config_id'></Item>
  <Item type='Language' action='get' select='config_id'></Item>
  <Item type='Life Cycle Map' action='get' select='config_id'></Item>
  <Item type='List' action='get' select='config_id'></Item>
  <Item type='Locale' action='get' select='config_id'></Item>
  <Item type='Measurement Unit' action='get' select='config_id'></Item>
  <Item type='Method' action='get' select='config_id'></Item>
  <Item type='Metric' action='get' select='config_id'></Item>
  <Item type='mp_MacPolicy' action='get' select='config_id'></Item>
  <Item type='mp_PolicyAccessEnvAttribute' action='get' select='config_id'></Item>
  <Item type='Permission' action='get' select='config_id'></Item>
  <Item type='Permission_ExplicitDefine' action='get' select='config_id'></Item>
  <Item type='Permission_ItemClassification' action='get' select='config_id'></Item>
  <Item type='Permission_PropertyValue' action='get' select='config_id'></Item>
  <Item type='PreferenceTypes' action='get' select='config_id'></Item>
  <Item type='PresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='qry_QueryDefinition' action='get' select='config_id'></Item>
  <Item type='rb_TreeGridViewDefinition' action='get' select='config_id'></Item>
  <Item type='Report' action='get' select='config_id'></Item>
  <Item type='Revision' action='get' select='config_id'></Item>
  <Item type='SearchMode' action='get' select='config_id'></Item>
  <Item type='SecureMessageViewTemplate' action='get' select='config_id'></Item>
  <Item type='SelfServiceReportHelp' action='get' select='config_id'></Item>
  <Item type='Sequence' action='get' select='config_id'></Item>
  <Item type='SQL' action='get' select='config_id'></Item>
  <Item type='SSVCPresentationConfiguration' action='get' select='config_id'></Item>
  <Item type='SystemFileContainer' action='get' select='config_id'></Item>
  <Item type='tp_XmlSchema' action='get' select='config_id'></Item>
  <Item type='UserMessage' action='get' select='config_id'></Item>
  <Item type='Variable' action='get' select='config_id'></Item>
  <Item type='Vault' action='get' select='config_id'></Item>
  <Item type='Viewer' action='get' select='config_id'></Item>
  <Item type='Workflow Map' action='get' select='config_id'></Item>
  <Item type='xClassificationTree' action='get' select='config_id'></Item>
  <Item type='xPropertyDefinition' action='get' select='config_id'></Item>
</AML>"
      }
    };
    #endregion


    public IEditorHelper GetOutputHelper()
    {
      return _helper;
    }
  }
}
