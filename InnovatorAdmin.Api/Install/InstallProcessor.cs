using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class InstallProcessor : IProgressReporter
  {
    private readonly IAsyncConnection _conn;
    private int _currLine;
    private InstallScript _script;
    private List<InstallItem> _lines;
    private readonly StringBuilder _log = new StringBuilder();
    private Dictionary<string, ItemType> _itemTypes;
    private readonly ExportProcessor _exportTools;

    public event EventHandler<ActionCompleteEventArgs> ActionComplete;
    public event EventHandler<RecoverableErrorEventArgs> ErrorRaised;
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public string InstallLog { get { return _log.ToString(); } }

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
      _log.Length = 0;
      _script = script;
      _lines = (_script.DependencySorted
        ? (_script.Lines ?? Enumerable.Empty<InstallItem>())
        : (await ExportProcessor.SortByDependencies(script.Lines, _conn).ConfigureAwait(false))
          .Where(l => l.Type != InstallType.DependencyCheck)
      ).Where(l => l.Script != null && l.Type != InstallType.Warning).ToList();
      _currLine = -1;
    }

    public void Install()
    {
      _currLine = 0;
      InstallLines();
    }

    public void Continue()
    {
      InstallLines();
    }

    public void SkipAndContinue()
    {
      _currLine++;
      InstallLines();
    }

    private void InstallLines()
    {
      string upgradeId = Guid.NewGuid().ToString("N").ToUpperInvariant();

      ExportProcessor.EnsureSystemData(_conn, ref _itemTypes);

      try
      {
        bool cont;
        RecoverableErrorEventArgs args;
        XmlNode query;
        XmlElement newQuery;
        ItemType itemType;
        string configId;

        ReportProgress(0, "Starting install.");
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
              var cmd = new Command(query.OuterXml);
              cmd.Settings = x => x.Timeout = TimeSpan.FromMinutes(5);
              items = _conn.Apply(cmd).AssertItems();
              if (line.Type == InstallType.Create) line.InstalledId = items.First().Attribute("id").Value;

              // Execute any sql scripts
              var sqlScripts = line.Script
                            .DescendantsAndSelf(e => e.Attribute(XmlFlags.Attr_SqlScript, "") != "")
                            .Select(e => e.Attribute(XmlFlags.Attr_SqlScript, ""));
              if (sqlScripts.Any())
              {
                _conn.ApplySql(sqlScripts.Aggregate((p, c) => p + Environment.NewLine + c)).AssertNoError();
              }

              cont = false;
            }
            catch (ServerException ex)
            {
              _log.Append(DateTime.Now.ToString("s")).AppendLine(": ERROR");
              _log.AppendLine(ex.ToAml());
              _log.AppendLine(" for query ");
              _log.AppendLine(ex.Query);

              if (ex.Message.Trim() == "No items of type ? found."
                && (query.Attribute("action") == "delete" || query.Attribute("action") == "purge"))
              {
                // Ignore errors with trying to delete something that isn't there.
                _log.Append(DateTime.Now.ToString("s")).AppendLine(": Skipping install step");
                cont = false;
                break;
              }
              else
              {
                args = new RecoverableErrorEventArgs() { Exception = ex };
                if (line.Type == InstallType.DependencyCheck && ex.FaultCode == "0")
                {
                  args.Message = "Unable to find required dependency " + line.Reference.Type + ": " + line.Reference.KeyedName;
                }
                OnErrorRaised(args);
                switch (args.RecoveryOption)
                {
                  case RecoveryOption.Abort:
                    _log.Append(DateTime.Now.ToString("s")).AppendLine(": Install aborted.");
                    throw;
                  case RecoveryOption.Retry:
                    query = args.NewQuery ?? query;
                    _log.Append(DateTime.Now.ToString("s")).AppendLine(": Retrying install step with query:");
                    _log.AppendLine(query.OuterXml);
                    break;
                  case RecoveryOption.Skip:
                    _log.Append(DateTime.Now.ToString("s")).AppendLine(": Skipping install step.");
                    cont = false;
                    break;
                }
              }
            }
          }

          _currLine++;
        }

        if (_script.AddPackage)
        {
          var pkg = new DatabasePackage(_conn);
          pkg.Write(_script, e =>
          {
            args = new RecoverableErrorEventArgs() { Message = e };
            OnErrorRaised(args);
            switch (args.RecoveryOption)
            {
              case RecoveryOption.Skip:
                return DatabasePackageAction.RemoveElementsFromPackages;
              case RecoveryOption.Retry:
                return DatabasePackageAction.TryAgain;
              default:
                return DatabasePackageAction.Abort;
            }
          }, (i, m) =>
          {
            ReportProgress((int)(i * 0.2 + 80), m);
          });
        }

        _conn.Apply("<Item type=\"DatabaseUpgrade\" action=\"merge\" id=\"@0\"><upgrade_status>1</upgrade_status></Item>", upgradeId).AssertNoError();
        OnActionComplete(new ActionCompleteEventArgs());
      }
      catch (Exception ex)
      {
        _conn.Apply("<Item type=\"DatabaseUpgrade\" action=\"merge\" id=\"@0\"><upgrade_status>2</upgrade_status></Item>", upgradeId); //.AssertNoError();
        OnActionComplete(new ActionCompleteEventArgs() { Exception = ex });
      }
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
      _log.Append(DateTime.Now.ToString("s")).AppendLine(message);
      OnProgressChanged(new ProgressChangedEventArgs(message, progress));
    }
    protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
    {
      ProgressChanged(this, e);
    }
  }
}
