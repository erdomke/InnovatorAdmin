using Innovator.Client;
using Microsoft.Extensions.Logging;
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

    private ILogger _logger;

    public InstallProcessor(IAsyncConnection conn, ILogger logger)
    {
      _conn = conn;
      _exportTools = new ExportProcessor(conn, logger);
      _currLine = -1;
      _logger = logger;
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
      _currLine = 0;

      var exception = default(Exception);
      using (var activity = SharedUtils.StartActivity("InstallProcessor.Install", null, new Dictionary<string, object>()
      {
        ["script.title"] = _script.Title,
        ["script.created"] = _script.Created,
        ["script.creator"] = _script.Creator
      }))
      {
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
          activity.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
          if (!(ex is ServerException))
            _logger?.LogError(ex, null);
          activity.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
          _conn.Apply("<Item type=\"DatabaseUpgrade\" action=\"merge\" id=\"@0\"><upgrade_status>2</upgrade_status></Item>", upgradeId); //.AssertNoError();
        }
      }
      OnActionComplete(new ActionCompleteEventArgs() { Exception = exception });
    }

    private void InstallLines()
    {
      ExportProcessor.EnsureSystemData(_conn, ref _itemTypes);

      bool repeat;
      RecoverableErrorEventArgs args;
      XmlNode query;
      XmlElement newQuery;
      ItemType itemType;
      string configId;

      ReportProgress(0, "Starting install.");
      IEnumerable<IReadOnlyItem> items;

      var linesToInstall = _lines.Skip(_currLine).ToList();
      var lockedItems = new List<IReadOnlyItem>();
      foreach (var group in linesToInstall
        .Where(l => l.Reference.Unique.IsGuid())
        .GroupBy(l => l.Reference.Type))
      {
        lockedItems.AddRange(_conn.Apply(new Command((IFormattable)$@"<Item type='{group.Key}' action='get' select='locked_by_id'>
  <id condition='in'>{group.Select(l => l.Reference.Unique)}</id>
  <locked_by_id condition='is not null' />
</Item>")).Items());
      }

      if (lockedItems.Count > 0)
      {
        using (SharedUtils.StartActivity("InstallProcessor.Install.HandleServerError"))
        {
          var rootMessage = "The import is attempting to update items locked by the following individuals: "
            + string.Join(", ", lockedItems
              .GroupBy(i => i.Property("locked_by_id").Attribute("keyed_name").AsString(""))
              .Select(g => $"{g.Key} ({g.Count()})"))
            + ".";
          args = new RecoverableErrorEventArgs()
          {
            Message = rootMessage + " Select `Retry` to unlock these items and `Ignore` to proceed."
          };
          HandleErrorDefault(args, null);
          if (args.RecoveryOption == RecoveryOption.Abort)
            throw new InvalidOperationException(rootMessage);

          if (args.RecoveryOption == RecoveryOption.Retry)
          {
            foreach (var group in lockedItems.GroupBy(i => i.Type().AsString(null)))
            {
              _conn.Apply(new Command((IFormattable)$@"<Item type='{group.Key}' action='unlock' idlist='{string.Join(",", group.Select(i => i.Id()))}' />")).AssertNoError();
            }
          }
        }
      }

      foreach (var line in linesToInstall)
      {
        repeat = true;
        _logger.LogInformation("Performing {Line} ({Index} of {Count})", line.ToString(), _currLine + 1, _lines.Count);
        ReportProgress((int)(_currLine * 80.0 / _lines.Count), string.Format("Performing {0} ({1} of {2})", line.ToString(), _currLine + 1, _lines.Count));
        query = line.Script;

        while (repeat)
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

              var relationshipsElement = newQuery.Elem("Relationships");
              foreach (var relType in query.ElementsByXPath("Relationships/Item")
                .Where(i => (string.IsNullOrEmpty(i.GetAttribute("where")))
                  || string.IsNullOrEmpty(i.GetAttribute("action")))
                .Select(e => e.Attribute("type"))
                .Distinct(StringComparer.OrdinalIgnoreCase))
              {
                relationshipsElement.Elem("Item")
                  .Attr("type", relType)
                  .Attr("action", "get");
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

              foreach (var group in query.ElementsByXPath("Relationships/Item")
                .Where(i => (string.IsNullOrEmpty(i.GetAttribute("where")))
                  || string.IsNullOrEmpty(i.GetAttribute("action")))
                .GroupBy(e => e.Attribute("type", ""), StringComparer.OrdinalIgnoreCase))
              {
                MergeRelationshipsOnVersionableParent(sourceId, group.Key
                  , items.FirstOrDefault()?.Relationships()
                    .Where(r => string.Equals(r.TypeName(), group.Key, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                  , group.ToList());
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

            repeat = false;
          }
          catch (ServerException ex)
          {
            using (SharedUtils.StartActivity("InstallProcessor.Install.HandleServerError"))
            {
              args = new RecoverableErrorEventArgs() { Exception = ex };
              if (line.Type == InstallType.DependencyCheck && ex.FaultCode == "0")
                args.Message = "Unable to find required dependency " + line.Reference.Type + ": " + line.Reference.KeyedName;

              HandleErrorDefault(args, query);
              switch (args.RecoveryOption)
              {
                case RecoveryOption.Abort:
                  throw;
                case RecoveryOption.Retry:
                  query = args.NewQuery ?? query;
                  _logger?.LogInformation("{NewQuery}", SharedUtils.TidyXml(query.WriteTo, false));
                  break;
                default: // case RecoveryOption.Skip:
                  repeat = false;
                  break;
              }
            }
          }
        }

        _currLine++;
      }
    }

    private string GetPropertyValue(XmlElement rel, string property)
    {
      if (rel.Element(property) == null)
      {
        return null;
      }
      else if (rel.Element(property).Element("Item") == null)
      {
        return rel.Element(property).InnerText;
      }
      else if (rel.Element(property).Element("Item").Attribute("action") == "get")
      {
        var relatedQuery = rel.Element(property).Element("Item");
        return _conn.Apply(relatedQuery).AssertItem().Id();
      }
      else
      {
        return rel.Element(property).Element("Item").Attribute("id");
      }
    }

    private readonly string[] _preferredRelationshipMatch = new[] { "related_id", "item_number", "name", "label", "path" };

    private void MergeRelationshipsOnVersionableParent(string sourceId, string relType, IReadOnlyList<IReadOnlyItem> existing, IReadOnlyList<XmlElement> incoming)
    {
      var propToMatch = default(string);
      if (existing?.Count > 0)
      {
        propToMatch = _preferredRelationshipMatch
          .FirstOrDefault(p =>
            existing
              .Where(i => i.Property(p).HasValue())
              .Select(i => i.Property(p).Value)
              .Distinct(StringComparer.OrdinalIgnoreCase)
              .Count() == existing.Count
            && incoming
              .Where(e => e.Element(p) != null)
              .Select(e => GetPropertyValue(e, p))
              .Distinct(StringComparer.OrdinalIgnoreCase)
              .Count() == incoming.Count);
      }

      if (propToMatch == null)
      {
        foreach (var rel in incoming)
        {
          rel.RemoveAttribute("id");
          rel.SetAttribute("action", "add");
        }
      }
      else
      {
        var xref = existing.ToDictionary(i => i.Property(propToMatch).Value, StringComparer.OrdinalIgnoreCase);
        var relTableName = relType.Replace(' ', '_');
        foreach (var rel in incoming)
        {
          rel.RemoveAttribute("id");
          var propValue = GetPropertyValue(rel, propToMatch);
          if (xref.TryGetValue(propValue, out var existingRel))
          {
            rel.SetAttribute("where", $"[{relTableName}].[source_id]='{sourceId}' and [{relTableName}].[{propToMatch}]='{propValue}'");
            rel.SetAttribute("action", "edit");
          }
          else
          {
            rel.SetAttribute("action", "add");
          }
        }
      }
    }

    private void HandleErrorDefault(RecoverableErrorEventArgs args, XmlNode query)
    {
      _logger?.LogError(args.Exception, args.Message);

      var isAuto = false;
      if (args.Exception?.FaultCode == "0"
        && (query?.Attribute("action") == "delete" || query?.Attribute("action") == "purge"))
      {
        args.RecoveryOption = RecoveryOption.Skip;
        isAuto = true;
      }
      else if (args.Exception?.Message.Trim() == "Identity already exists."
        && query?.Attribute("type") == "Identity"
        && query?.Attribute("action") == "add")
      {
        ((XmlElement)query).SetAttribute("action", "edit");
        args.NewQuery = query;
        args.RecoveryOption = RecoveryOption.Retry;
        isAuto = true;
      }

      if (isAuto)
      {
        _logger?.LogInformation("Automatically decided to {action}", args.RecoveryOption.ToString());
      }
      else
      {
        OnErrorRaised(args);
        _logger?.LogInformation("User decided to {action}", args.RecoveryOption.ToString());
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
      OnProgressChanged(new ProgressChangedEventArgs(message, progress));
    }

    protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
    {
      ProgressChanged(this, e);
    }
  }
}
