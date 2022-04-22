using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class InstallProcessor : IProgressReporter
  {
    private DependencySorter _sorter = new DependencySorter();
    private readonly IAsyncConnection _conn;
    private int _currLine;
    private InstallScript _script;
    private List<InstallItem> _lines;
    private Dictionary<string, ItemType> _itemTypes;
    private readonly ExportProcessor _exportTools;

    public event EventHandler<ActionCompleteEventArgs> ActionComplete;
    public event EventHandler<RecoverableErrorEventArgs> ErrorRaised;
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public XmlWriter LogWriter { get; set; }

    public InstallProcessor(IAsyncConnection conn)
    {
      _conn = conn;
      _exportTools = new ExportProcessor(conn);
      _currLine = -1;
    }

    public async Task<InstallScript> ConvertManifestXml(XmlDocument doc, string name)
    {
      ExportProcessor.EnsureSystemData(_conn, ref _itemTypes);

      foreach (var elem in doc.ElementsByXPath("//Item[@action='add']").ToList())
      {
        elem.SetAttribute("action", "merge");
      }
      ItemType itemType;
      foreach (var elem in doc.ElementsByXPath("//Item[@type and @id]").ToList())
      {
        if (_itemTypes.TryGetValue(elem.Attribute("type", "").ToLowerInvariant(), out itemType) && itemType.IsVersionable)
        {
          elem.SetAttribute(XmlFlags.Attr_ConfigId, elem.Attribute("id"));
          elem.SetAttribute("where", string.Format("[{0}].[config_id] = '{1}'", itemType.Name.Replace(' ', '_'), elem.Attribute("id")));
          elem.RemoveAttribute("id");
        }
      }

      var result = new InstallScript()
      {
        Title = name
      };
      await _exportTools.Export(result, doc).ConfigureAwait(false);
      return result;
    }

    public async Task Initialize(InstallScript script)
    {
      _script = script;
      _lines = (_script.DependencySorted
        ? (_script.Lines ?? Enumerable.Empty<InstallItem>())
        : (await _sorter.SortByDependencies(script.Lines, _conn).ConfigureAwait(false))
          .Where(l => l.Type != InstallType.DependencyCheck)
      ).Where(l => l.Script != null && l.Type != InstallType.Warning).ToList();
      _currLine = -1;
    }

    public void Install()
    {
      if (LogWriter != null)
      {
        LogWriter.WriteStartElement("event");
        LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
        LogWriter.WriteAttributeString("type", "Start");
        LogWriter.WriteStartElement("server");
        if (_conn is Innovator.Client.Connection.ArasHttpConnection httpConn)
          LogWriter.WriteAttributeString("url", httpConn.Url.ToString());
        LogWriter.WriteAttributeString("db", _conn.Database);
        LogWriter.WriteAttributeString("user", _conn.UserId);
        LogWriter.WriteEndElement();
        LogWriter.WriteStartElement("client");
        LogWriter.WriteAttributeString("os_version", Environment.OSVersion.ToString());
        LogWriter.WriteAttributeString("machine_name", Environment.MachineName);
        LogWriter.WriteAttributeString("user", $"{Environment.UserDomainName}\\{Environment.UserName}");
        LogWriter.WriteEndElement();
        LogWriter.WriteEndElement();
        LogWriter.Flush();
      }
      _currLine = 0;

      var exception = default(Exception);
      var upgradeId = Guid.NewGuid().ToArasId();
      try
      {
        _conn.Apply(@"<Item type='DatabaseUpgrade' action='merge' id='@0'>
                        <upgrade_status>0</upgrade_status>
                        <is_latest>0</is_latest>
                        <type>1</type>
                        <os_user>@1</os_user>
                        <name>@2</name>
                        <description>@3</description>
                        <applied_on>__now()</applied_on>
                      </Item>"
          , upgradeId
          , Environment.UserDomainName + "\\" + Environment.UserName
          , _script.Title.Left(64)
          , _script.Description.Left(512)).AssertNoError();
        InstallLines();
        _conn.Apply("<Item type=\"DatabaseUpgrade\" action=\"merge\" id=\"@0\"><upgrade_status>1</upgrade_status></Item>", upgradeId).AssertNoError();
      }
      catch (Exception ex)
      {
        if (LogWriter != null && !(ex is ServerException))
        {
          LogWriter.WriteStartElement("event");
          LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
          LogWriter.WriteAttributeString("type", "Error");
          LogWriter.WriteAttributeString("message", ex.Message);

          LogWriter.WriteStartElement("details");
          LogWriter.WriteValue(ex.ToString());
          LogWriter.WriteEndElement();

          LogWriter.WriteEndElement();
          LogWriter.Flush();
        }
        _conn.Apply("<Item type=\"DatabaseUpgrade\" action=\"merge\" id=\"@0\"><upgrade_status>2</upgrade_status></Item>", upgradeId); //.AssertNoError();
      }

      if (LogWriter != null)
      {
        LogWriter.WriteStartElement("event");
        LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
        LogWriter.WriteAttributeString("type", "Finish");
        LogWriter.WriteEndElement();
        LogWriter.Flush();
      }
      OnActionComplete(new ActionCompleteEventArgs() { Exception = exception });
    }

    private void InstallLines()
    {
      ExportProcessor.EnsureSystemData(_conn, ref _itemTypes);

      bool cont;
      RecoverableErrorEventArgs args;
      XmlNode query;
      XmlElement newQuery;
      ItemType itemType;
      string configId;

      ReportProgress(0, "Starting install.");
      IEnumerable<IReadOnlyItem> items;
      foreach (var line in _lines.Skip(_currLine).ToList())
      {
        cont = true;
        ReportProgress((int)(_currLine * 80.0 / _lines.Count), string.Format("Performing {0} ({1} of {2})", line.ToString(), _currLine + 1, _lines.Count));
        query = line.Script;

        while (cont)
        {
          try
          {
            // If the original item uses a where clause or is versionable or the target item is versionable

            if ((query.Attribute("action") == "merge" || query.Attribute("action") == "edit") && TryGetConfigId(query, out configId)
               && (query.Attribute("where") != null || (_itemTypes.TryGetValue(query.Attribute("type").ToLowerInvariant(), out itemType))
               && itemType.IsVersionable))
            {
              newQuery = query.Clone() as XmlElement;
              newQuery.InnerXml = "";
              newQuery.SetAttribute("action", "get");
              newQuery.SetAttribute("select", "id");

              // The item type became versionable in the target database
              if (newQuery.Attribute("where") == null)
              {
                newQuery.RemoveAttribute("id");
                newQuery.SetAttribute("where", string.Format("[{0}].[config_id] = '{1}'", query.Attribute("type", "").Replace(' ', '_'), configId));
              }

              // If the item exists, get the id for use in the relationships
              // If the item doesn't exist, make sure the id = config_id for the add
              items = _conn.Apply(newQuery.OuterXml).Items();
              string sourceId = items.Any() ? items.First().Attribute("id").Value : configId;
              newQuery = query.Clone() as XmlElement;
              newQuery.SetAttribute("id", sourceId);
              newQuery.RemoveAttribute("where");
              newQuery.RemoveAttribute(XmlFlags.Attr_ConfigId);
              query = newQuery;

              string relatedId;
              string whereClause;
              // Check relationships and match based on source_id and related_id where necessary
              var rels = query.ElementsByXPath("Relationships/Item[related_id]").ToArray();
              foreach (var rel in rels)
              {
                if (rel.Element("related_id").Element("Item") == null)
                {
                  relatedId = rel.InnerText;
                }
                else if (rel.Element("related_id").Element("Item").Attribute("action") == "get")
                {
                  var relatedQuery = rel.Element("related_id").Element("Item");
                  relatedId = _conn.Apply(relatedQuery).AssertItem().Id();
                }
                else
                {
                  relatedId = rel.Element("related_id").Element("Item").Attribute("id");
                }
                whereClause = string.Format("[{0}].[source_id]='{1}' and [{0}].[related_id]='{2}'"
                  , rel.Attribute("type", "").Replace(' ', '_'), sourceId, relatedId);

                if (!string.IsNullOrEmpty(relatedId))
                {
                  newQuery = rel.OwnerDocument.CreateElement("Item");
                  newQuery.SetAttribute("type", rel.Attribute("type"));
                  newQuery.SetAttribute("where", whereClause);
                  newQuery.SetAttribute("action", "get");

                  items = _conn.Apply(newQuery.OuterXml).Items();
                  rel.RemoveAttribute("id");
                  if (items.Any())
                  {
                    rel.SetAttribute("where", whereClause);
                    rel.SetAttribute("action", "edit");
                  }
                  else
                  {
                    rel.SetAttribute("action", "add");
                  }
                }
              }
            }

            //Fix Relationships on cmf generated ItemTypes
            if ((query.Attribute("action") == "merge" || query.Attribute("action") == "edit")
              && query.Attribute("_cmf_generated") != null && query.Attribute("where") != null)
            {
              var sourceItem = _conn.Apply($"<Item action='get' type='ItemType'><name>{query.Element("name").InnerText}</name></Item>").AssertItem();
              var sourceId = sourceItem.Id();
              var queryClone = query.Clone() as XmlElement;
              queryClone.SetAttribute("id", sourceId);
              queryClone.RemoveAttribute("where");
              query = queryClone;

              string relatedId = "";
              string whereClause;
              foreach (var rel in query.ElementsByXPath("Relationships/Item[related_id]").ToList())
              {
                if (rel.Element("related_id").Element("Item") == null)
                {
                  relatedId = rel.Element("related_id").InnerText;
                }
                else
                {
                  var relatedQuery = rel.Element("related_id").Element("Item");
                  relatedQuery.Attr("action", "get");
                  relatedId = _conn.Apply(relatedQuery).AssertItem().Id();
                  rel.Element("related_id").InnerText = relatedId;
                }
                whereClause = string.Format("[{0}].[source_id]='{1}' and [{0}].[related_id]='{2}'"
                  , rel.Attribute("type", "").Replace(' ', '_'), sourceId, relatedId);

                if (!string.IsNullOrEmpty(relatedId))
                {
                  rel.RemoveAttribute("id");
                  rel.SetAttribute("where", whereClause);
                  rel.SetAttribute("action", "merge");
                }
              }
            }
            var cmd = new Command(query.OuterXml)
            {
              Settings = x => x.Timeout = TimeSpan.FromMinutes(5)
            };
            items = _conn.Apply(cmd).AssertItems();
            if (line.Type == InstallType.Create) line.InstalledId = items.First().Attribute("id").Value;

            // Execute any sql scripts
            var sqlScripts = line.Script
              .DescendantsAndSelf(e => e.Attribute(XmlFlags.Attr_SqlScript, "") != "")
              .Select(e => e.Attribute(XmlFlags.Attr_SqlScript, ""));
            if (sqlScripts.Any())
              _conn.ApplySql(sqlScripts.Aggregate((p, c) => p + Environment.NewLine + c)).AssertNoError();

            cont = false;
          }
          catch (ServerException ex)
          {
            args = new RecoverableErrorEventArgs() { Exception = ex };
            if (line.Type == InstallType.DependencyCheck && ex.FaultCode == "0")
            {
              args.Message = "Unable to find required dependency " + line.Reference.Type + ": " + line.Reference.KeyedName;
            }
            var isAuto = TryHandleErrorDefault(args, query);
            if (!isAuto)
              OnErrorRaised(args);

            switch (args.RecoveryOption)
            {
              case RecoveryOption.Abort:
                if (LogWriter != null)
                {
                  LogWriter.WriteStartElement("action");
                  LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
                  LogWriter.WriteAttributeString("type", "Abort");
                  LogWriter.WriteEndElement();

                  LogWriter.WriteEndElement();
                  LogWriter.Flush();
                }
                throw;
              case RecoveryOption.Retry:
                query = args.NewQuery ?? query;
                if (LogWriter != null)
                {
                  LogWriter.WriteStartElement("action");
                  LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
                  LogWriter.WriteAttributeString("type", "Retry");
                  if (isAuto)
                    LogWriter.WriteAttributeString("is_auto", "1");
                  query.WriteTo(LogWriter);
                  LogWriter.WriteEndElement();

                  LogWriter.WriteEndElement();
                  LogWriter.Flush();
                }
                break;
              default: // case RecoveryOption.Skip:
                if (LogWriter != null)
                {
                  LogWriter.WriteStartElement("action");
                  LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
                  LogWriter.WriteAttributeString("type", "Skip");
                  if (isAuto)
                    LogWriter.WriteAttributeString("is_auto", "1");
                  LogWriter.WriteEndElement();

                  LogWriter.WriteEndElement();
                  LogWriter.Flush();
                }
                cont = false;
                break;
            }
          }
        }

        _currLine++;
      }
    }

    private bool TryHandleErrorDefault(RecoverableErrorEventArgs args, XmlNode query)
    {
      var isAuto = false;
      if (args.Exception.FaultCode == "0"
        && (query.Attribute("action") == "delete" || query.Attribute("action") == "purge"))
      {
        args.RecoveryOption = RecoveryOption.Skip;
        isAuto = true;
      }
      else if (args.Exception.Message.Trim() == "Identity already exists."
        && query.Attribute("type") == "Identity"
        && query.Attribute("action") == "add")
      {
        ((XmlElement)query).SetAttribute("action", "edit");
        args.NewQuery = query;
        args.RecoveryOption = RecoveryOption.Retry;
        isAuto = true;
      }
      //else if (args.Exception.FaultCode == "SOAP-ENV:Server.PropertiesAreNotUniqueException")
      //{
      //  args.Exception.Fault.Element("detail").Element("af:item");
      //}

      if (LogWriter != null)
      {
        LogWriter.WriteStartElement("event");
        LogWriter.WriteAttributeString("time", DateTime.UtcNow.ToString("s"));
        LogWriter.WriteAttributeString("type", "Error" + (isAuto ? "." + args.RecoveryOption.ToString() : ".Prompt"));
        LogWriter.WriteAttributeString("message", args.Exception.Message);

        if (!isAuto)
          args.Exception.ToAml(LogWriter);
        LogWriter.WriteStartElement("AML");
        System.Xml.Linq.XElement.Parse(args.Exception.Query).WriteTo(LogWriter);
        LogWriter.WriteEndElement();
        LogWriter.Flush();
      }

      return isAuto;
    }

    private bool TryGetConfigId(XmlNode query, out string configId)
    {
      configId = query.Attribute(XmlFlags.Attr_ConfigId);
      if (configId != null) return true;
      var where = query.Attribute("where");
      if (string.IsNullOrWhiteSpace(where)) return false;
      var type = query.Attribute("type");
      if (string.IsNullOrWhiteSpace(type)) return false;
      var prefix = string.Format("[{0}].[config_id] = '", type.Replace(' ', '_'));
      if (where.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && where.Length >= (prefix.Length + 33))
      {
        configId = prefix.Substring(prefix.Length, 32);
        return true;
      }
      return false;
    }

    protected virtual void OnActionComplete(ActionCompleteEventArgs e)
    {
      if (ActionComplete != null) ActionComplete(this, e);
    }

    protected virtual void OnErrorRaised(RecoverableErrorEventArgs e)
    {
      if (ErrorRaised != null) ErrorRaised(this, e);
    }

    protected virtual void ReportProgress(int progress, string message)
    {
      OnProgressChanged(new ProgressChangedEventArgs(message, progress));
    }

    protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
    {
      ProgressChanged(this, e);
    }
  }
}
