using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class ScriptMenuGenerator
  {
    public IAsyncConnection Conn { get; set; }
    public Connections.ConnectionData ConnData { get; set; }
    public string Column { get; set; }
    public IEnumerable<IItemData> Items { get; set; }

    private string GetCriteria(string unique)
    {
      if (unique.IsGuid())
      {
        return "id='" + unique + "'";
      }
      return "where=\"" + unique.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;") + "\"";
    }

    public void SetItems(IEnumerable<ItemReference> refs)
    {
      this.Items = refs.Select(r => new ItemRefData(r)).ToArray();
    }

    private class ItemRefData : IItemData
    {
      private ItemReference _itemRef;

      public string Id { get { return _itemRef.Unique; } }

      public string Type { get { return _itemRef.Type; } }

      public ItemReference Ref { get { return _itemRef; } }

      public ItemRefData(ItemReference itemRef)
      {
        _itemRef = itemRef;
      }

      public object Property(string name)
      {
        if (name == "keyed_name")
          return _itemRef.KeyedName;
        return null;
      }
    }

    public IEnumerable<IEditorScript> GetScripts()
    {
      var items = (Items ?? Enumerable.Empty<IItemData>())
        .Where(i => !string.IsNullOrEmpty(i.Id) && !string.IsNullOrEmpty(i.Type))
        .ToArray();
      if (!items.Any())
        yield break;

      if (items.Skip(1).Any()) // There is more than one
      {
        if (items.OfType<DataRowItemData>().Any())
        {
          yield return new EditorScriptExecute()
          {
            Name = "Delete",
            Execute = () =>
            {
              foreach (var row in items.OfType<DataRowItemData>())
              {
                row.Delete();
              }
              return Task.FromResult(true);
            }
          };
        }
        else
        {
          var builder = new StringBuilder("<AML>");
          foreach (var item in items)
          {
            builder.AppendLine().AppendFormat("  <Item type='{0}' {1} action='delete'></Item>", item.Type, GetCriteria(item.Id));
          }
          builder.AppendLine().Append("</AML>");
          yield return new EditorScript()
          {
            Name = "Delete",
            Action = "ApplyAML",
            Script = builder.ToString()
          };
        }

        var dataRows = items.OfType<DataRowItemData>()
          .OrderBy(r => r.Property("generation")).ThenBy(r => r.Id)
          .ToArray();
        if (dataRows.Length == 2) // There are exactly two items
        {
          yield return new EditorScript()
          {
            Name = "------"
          };
          yield return new EditorScriptExecute()
          {
            Name = "Compare",
            Execute = async () =>
            {
              try
              {
                await Settings.Current.PerformDiff(dataRows[0].Id, dataRows[0].ToAml
                  , dataRows[1].Id, dataRows[1].ToAml);
              }
              catch (Exception ex)
              {
                Utils.HandleError(ex);
              }
            }
          };
        }
        yield return new EditorScript()
        {
          Name = "------"
        };
        yield return new EditorScriptExecute()
        {
          Name = "Export",
          Execute = () =>
          {
            var refs = items.OfType<ItemRefData>().Select(i => i.Ref);
            if (!refs.Any())
              refs = items.Select(i => new ItemReference(i.Type, i.Id));
            StartExport(refs);
            return Task.FromResult(true);
          }
        };
      }
      else
      {
        var item = items.Single();
        var rowItem = item as DataRowItemData;

        ArasMetadataProvider metadata = null;
        ItemType itemType = null;
        if (Conn != null)
        {
          metadata = ArasMetadataProvider.Cached(Conn);
          if (!metadata.ItemTypeByName(item.Type, out itemType))
            metadata = null;
        }

        if (Conn != null)
        {
          yield return ArasEditorProxy.ItemTypeAddScript(Conn, itemType);
        }
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (rowItem == null)
        {
          var script = string.Format("<Item type='{0}' {1} action='edit'></Item>", item.Type, GetCriteria(item.Id));
          if (item.Property("config_id") != null && itemType != null && itemType.IsVersionable)
          {
            script = string.Format("<Item type='{0}' where=\"[{1}].[config_id] = '{2}'\" action='edit'></Item>"
              , item.Type, item.Type.Replace(' ', '_'), item.Property("config_id"));
          }

          yield return new EditorScript()
          {
            Name = "Edit",
            Action = "ApplyItem",
            Script = script
          };
        }
        else
        {
          if (!string.IsNullOrEmpty(Column))
          {
            var prop = metadata.GetProperty(itemType, Column.Split('/')[0]).Wait();
            switch (prop.Type)
            {
              case PropertyType.item:
                yield return new EditorScriptExecute()
                {
                  Name = "Edit Value",
                  Execute = () =>
                  {
                    var query = string.Format("<Item type='{0}' action='get'><keyed_name condition='like'>**</keyed_name></Item>", prop.Restrictions.First());
                    var values = EditorWindow.GetItems(Conn, query, query.Length - 21);
                    var results = values.Where(i => prop.Restrictions.Contains(i.Type)).ToArray();
                    if (results.Length == 1)
                    {
                      rowItem.SetProperty(prop.Name, results[0].Unique);
                      rowItem.SetProperty(prop.Name + "/keyed_name", results[0].KeyedName);
                      rowItem.SetProperty(prop.Name + "/type", results[0].Type);
                    }
                    return Task.FromResult(true);
                  }
                };
                break;
            }
          }
        }
        if (metadata != null)
        {
          yield return new EditorScript()
          {
            Name = "View \"" + (itemType.Label ?? itemType.Name) + "\"",
            Action = "ApplyItem",
            Script = string.Format("<Item type='{0}' {1} action='get' levels='1'></Item>", item.Type, GetCriteria(item.Id)),
            AutoRun = true,
            PreferredOutput = OutputType.Table
          };
          if (item.Property("related_id") != null && itemType.Related != null)
          {
            yield return new EditorScript()
            {
              Name = "View \"" + (itemType.Related.Label ?? itemType.Related.Name) + "\"",
              Action = "ApplyItem",
              Script = string.Format("<Item type='{0}' id='{1}' action='get' levels='1'></Item>", itemType.Related.Name, item.Property("related_id")),
              AutoRun = true,
              PreferredOutput = OutputType.Table
            };
          }
        }
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (rowItem == null)
        {
          yield return new EditorScript()
          {
            Name = "Delete",
            Action = "ApplyItem",
            Script = string.Format("<Item type='{0}' {1} action='delete'></Item>", item.Type, GetCriteria(item.Id))
          };
        }
        else
        {
          yield return new EditorScriptExecute()
          {
            Name = "Delete",
            Execute = () =>
            {
              rowItem.Delete();
              return Task.FromResult(true);
            }
          };
        }
        if (item.Id.IsGuid())
        {
          yield return new EditorScript()
          {
            Name = "Replace Item",
            Action = "ApplySql",
            ScriptGetter = async () =>
            {
              var aml = string.Format("<Item type='{0}' action='get'><keyed_name condition='like'>**</keyed_name></Item>", item.Type);
              var replace = EditorWindow.GetItems(Conn, aml, aml.Length - 21);
              if (replace.Count() == 1)
              {
                var sqlItem = Conn.AmlContext.FromXml(_whereUsedSqlAml).AssertItem();
                var export = new ExportProcessor(Conn);
                var script = new InstallScript();
                var itemRef = ItemReference.FromFullItem(sqlItem, true);
                await export.Export(script, new[] { itemRef });
                var existing = script.Lines.FirstOrDefault(i => i.Reference.Equals(itemRef));
                var needsSql = true;
                if (existing != null)
                {
                  var merge = AmlDiff.GetMergeScript(XmlReader.Create(new StringReader(_whereUsedSqlAml)), new XmlNodeReader(existing.Script));
                  needsSql = merge.Elements().Any();
                }

                if (needsSql)
                {
                  if (Dialog.MessageDialog.Show("To run this action, InnovatorAdmin needs to install the SQL WhereUsed_General into the database.  Do you want to install this?", "Install SQL", "Install", "Cancel") == System.Windows.Forms.DialogResult.OK)
                  {
                    await Conn.ApplyAsync(_whereUsedSqlAml, true, false).ToTask();
                  }
                  else
                  {
                    return null;
                  }
                }

                var result = await Conn.ApplyAsync(@"<AML>
                                   <Item type='SQL' action='SQL PROCESS'>
                                     <name>WhereUsed_General</name>
                                     <PROCESS>CALL</PROCESS>
                                     <ARG1>@0</ARG1>
                                     <ARG2>@1</ARG2>
                                   </Item>
                                 </AML>", true, false, item.Type, item.Id).ToTask();
                var sql = new StringBuilder("<sql>");
                var whereUsed = result.Items().Where(i => !i.Property("type").HasValue() || i.Property("type").Value == i.Property("parent_type").Value);
                var replaceId = replace.First().Unique;
                sql.AppendLine();
                foreach (var i in whereUsed)
                {
                  var props = (from p in i.Elements().OfType<IReadOnlyProperty>()
                               where p.Name.Length == 2 && p.Name[0] == 'p' && char.IsNumber(p.Name[1])
                               select p.Value).GroupConcat(" = '" + replaceId + "',");
                  sql.Append("update innovator.[").Append(i.Property("main_type").Value.Replace(' ', '_')).Append("] set ");
                  sql.Append(props).Append(" = '").Append(replaceId).Append("'");
                  sql.Append(" where id ='").Append(i.Property("main_id").Value).Append("';");
                  sql.AppendLine();
                }
                sql.Append("</sql>");

                return sql.ToString();
              }

              return null;
            }
          };
        }
        yield return new EditorScript()
        {
          Name = "------"
        };
        yield return new EditorScriptExecute()
        {
          Name = "Export",
          Execute = () =>
          {
            var refs = new[] { new ItemReference(item.Type, item.Id) };
            StartExport(refs);
            return Task.FromResult(true);
          }
        };
        yield return new EditorScript()
        {
          Name = "------"
        };
        yield return new EditorScript()
        {
          Name = "Lock",
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' {1} action='lock'></Item>", item.Type, GetCriteria(item.Id))
        };
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (itemType != null && itemType.IsVersionable)
        {
          var whereClause = "id='" + item.Id + "'";
          if (!item.Id.IsGuid())
            whereClause = item.Id;

          yield return new EditorScript()
          {
            Name = "Revisions",
            AutoRun = true,
            Action = "ApplyItem",
            PreferredOutput = OutputType.Table,
            Script = string.Format(@"<Item type='{0}' action='get' orderBy='generation'>
<config_id condition='in'>(select config_id from innovator.[{1}] where {2})</config_id>
<generation condition='gt'>0</generation>
</Item>", item.Type, item.Type.Replace(' ', '_'), whereClause)
          };
          yield return new EditorScript()
          {
            Name = "------"
          };
        }
        yield return new EditorScript()
        {
          Name = "Promote",
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' {1} action='promoteItem'></Item>", item.Type, GetCriteria(item.Id))
        };
        yield return new EditorScript()
        {
          Name = "------"
        };
        yield return new EditorScript()
        {
          Name = "Where Used",
          AutoRun = true,
          Action = "ApplyItem",
          Script = string.Format("<Item type='{0}' {1} action='getItemWhereUsed'></Item>", item.Type, GetCriteria(item.Id))
        };
        yield return new EditorScript()
        {
          Name = "Structure Browser",
          Action = "ApplyItem",
          AutoRun = true,
          Script = string.Format(@"<Item type='Method' action='GetItemsForStructureBrowser'>
  <Item type='{0}' {1} action='GetItemsForStructureBrowser' levels='2' />
</Item>", item.Type, GetCriteria(item.Id))
        };
        yield return new EditorScript()
        {
          Name = "------"
        };
        if (metadata != null)
        {
          var actions = new EditorScript()
          {
            Name = "Actions"
          };

          var serverActions = metadata.ServerItemActions(item.Type)
            .OrderBy(l => l.Label ?? l.Value, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
          foreach (var action in serverActions)
          {
            actions.Add(new EditorScript()
            {
              Name = (action.Label ?? action.Value),
              Action = "ApplyItem",
              Script = string.Format("<Item type='{0}' {1} action='{2}'></Item>", item.Type, GetCriteria(item.Id), action.Value),
              AutoRun = true
            });
          }

          if (serverActions.Any())
            yield return actions;

          var reports = new EditorScript()
          {
            Name = "Reports"
          };

          var serverReports = metadata.ServerReports(item.Type)
            .OrderBy(l => l.Label ?? l.Value, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
          foreach (var report in serverReports)
          {
            reports.Add(new EditorScript()
            {
              Name = (report.Label ?? report.Value),
              Action = "ApplyItem",
              Script = @"<Item type='Method' action='Run Report'>
  <report_name>" + report.Value + @"</report_name>
  <AML>
    <Item type='" + itemType.Name + "' typeId='" + itemType.Id + "' " + GetCriteria(item.Id) + @" />
  </AML>
</Item>",
              AutoRun = true
            });
          }

          if (serverReports.Any())
            yield return reports;
        }
        if (item.Id.IsGuid())
        {
          yield return new EditorScriptExecute()
          {
            Name = "Copy ID",
            Execute = () =>
            {
              System.Windows.Clipboard.SetText(item.Id);
              return Task.FromResult(true);
            }
          };
        }
      }
    }

    private void StartExport(IEnumerable<ItemReference> selectedRefs)
    {
      if (Conn == null)
        return;

      var main = new Main();
      var wizard = (IWizard)main;
      wizard.ConnectionInfo = new[] { ConnData };
      wizard.Connection = Conn;

      var prog = new InnovatorAdmin.Controls.ProgressStep<ExportProcessor>(wizard.ExportProcessor);
      prog.MethodInvoke = e =>
      {
        wizard.InstallScript = new InstallScript();
        wizard.InstallScript.ExportUri = new Uri(wizard.ConnectionInfo.First().Url);
        wizard.InstallScript.ExportDb = wizard.ConnectionInfo.First().Database;
        wizard.InstallScript.Lines = Enumerable.Empty<InstallItem>();
        e.Export(wizard.InstallScript, selectedRefs, true);
      };
      prog.GoNextAction = () => wizard.GoToStep(new Controls.ExportResolve());
      main.Show();
      wizard.GoToStep(prog);
    }


    private const string _whereUsedSqlAml = @"<Item type='SQL' id='B14305C457DB4474BFF2E852459DAEB0' _keyed_name='WhereUsed_General' action='merge' _dependencies_analyzed='1'>
  <execution_flag>immediate</execution_flag>
  <old_name>WhereUsed_General</old_name>
  <sqlserver_body><![CDATA[/*
name: WhereUsed_General
solution: Extension to core
created: 2014-02-13
purpose: Get detailed where used information for the purpose of generating a replace script
*/
CREATE PROCEDURE innovator.WhereUsed_General(@itemtype as nvarchar(32), @itemid as char(32))
AS
BEGIN
/*
arguments:
@itemtype - the item type (name) for the item
@itemid - id of the item
*/

SELECT
    p.NAME prop_name
  , it.NAME it_name
  , it.INSTANCE_DATA it_table_name
  , it.IMPLEMENTATION_TYPE
  , sit.NAME parent_name
  , sit.INSTANCE_DATA parent_table_name
  , isnull(sit.OPEN_ICON, it.OPEN_ICON) open_icon
  , ROW_NUMBER() over (partition by it.ID order by p.NAME) p_num
into #PropData
from innovator.PROPERTY p
inner join innovator.ITEMTYPE ds
on p.DATA_SOURCE = ds.id
LEFT join
  (innovator.MORPHAE m
   inner join innovator.ITEMTYPE ds2
   on ds2.id = m.RELATED_ID
  )
on m.SOURCE_ID = ds.ID
inner join innovator.ITEMTYPE it
on it.ID = p.SOURCE_ID
LEFT JOIN
  (innovator.RELATIONSHIPTYPE rt
   inner join innovator.ITEMTYPE sit
   on sit.ID = rt.SOURCE_ID
  )
on rt.RELATIONSHIP_ID = it.ID
WHERE (ds.NAME = @itemtype or ds2.NAME = @itemtype)
and not p.name in ('config_id', 'id') and (sit.id is null or ds.id <> sit.id) -- Ignore links to itself
and it.IMPLEMENTATION_TYPE = 'table'
;

create table #WhereUsed (
  [icon] [nvarchar](128) NULL,
  [main_id] [char](32) NOT NULL,
  [main_type] [nvarchar](32) NOT NULL,
  [parent_id] [char](32) NOT NULL,
  [parent_type] [nvarchar](32) NOT NULL,
  [KEYED_NAME] [nvarchar](128) NULL,
  [MAJOR_REV] [nvarchar](8) NULL,
  [GENERATION] [int] NULL,
  [p1] [nvarchar](32) NULL,
  [p2] [nvarchar](32) NULL,
  [p3] [nvarchar](32) NULL,
  [p4] [nvarchar](32) NULL,
  [p5] [nvarchar](32) NULL,
  [p6] [nvarchar](32) NULL,
  [p7] [nvarchar](32) NULL,
  [p8] [nvarchar](32) NULL,
  [p9] [nvarchar](32) NULL,
  [p10] [nvarchar](32) NULL,
  [p11] [nvarchar](32) NULL,
  [p12] [nvarchar](32) NULL
);

declare @DynamicSql nvarchar(MAX);
declare @Params nvarchar(MAX);

-- Create a cursor for looping through the sql statements
declare sqlCursor cursor fast_forward for
SELECT
  CASE WHEN r.parent_name is null then
    'INSERT INTO #WhereUsed (icon,main_id,main_type,parent_id,parent_type,keyed_name,major_rev,generation,' + left(value_clause, len(value_clause) - 1) + ')
    SELECT ' + isnull('''' + open_icon + '''', 'null') + ' icon, m.id main_id, ''' + it_name + ''' main_type
      , m.id parent_id, ''' + it_name + ''' parent_type, m.KEYED_NAME, m.MAJOR_REV, m.GENERATION' + select_clause +
    ' FROM innovator.[' + it_table_name + '] m
    where ' + LEFT(where_clause, LEN(where_clause) - 3) + ';'
  else
    'INSERT INTO #WhereUsed (icon,main_id,main_type,parent_id,parent_type,keyed_name,major_rev,generation,' + left(value_clause, len(value_clause) - 1) + ')
    SELECT ' + isnull('''' + open_icon + '''', 'null') + ' icon, m.id main_id, ''' + it_name + ''' main_type
      , p.ID parent_id, ''' + parent_name + ''' parent_type, p.KEYED_NAME, p.MAJOR_REV, p.GENERATION' + select_clause +
    ' FROM innovator.[' + it_table_name + '] m
    left join innovator.[' + parent_table_name + '] p
    on p.id = m.source_id
    where ' + LEFT(where_clause, LEN(where_clause) - 3) + ';'
  end sql
from #PropData as r
CROSS APPLY
(
  SELECT 'm.' + prop_name + ' = @id or ' as [text()]
  from #PropData w
  where w.it_name = r.it_name
  for XML path ('')
) pre_trim_where(where_clause)
CROSS APPLY
(
  SELECT ', case when m.' + prop_name + ' = @id then ''' + prop_name + ''' else null end p' + cast(p_num AS nvarchar(5)) as [text()]
  from #PropData s
  where s.it_name = r.it_name
  for XML path ('')
) pre_trim_select(select_clause)
CROSS APPLY
(
  SELECT 'p' + cast(p_num AS nvarchar(5)) +',' as [text()]
  from #PropData v
  where v.it_name = r.it_name
  for XML path ('')
) pre_trim_value(value_clause)
GROUP by it_name, where_clause, select_clause, value_clause
, it_name
, it_table_name
, IMPLEMENTATION_TYPE
, parent_name
, parent_table_name
, open_icon
;

set @Params = N'@id as char(32)';

-- For each sql statement, execute it
open sqlCursor
fetch next from sqlCursor
into @DynamicSql

while @@FETCH_STATUS = 0
begin
  Execute sp_executesql @DynamicSql, @Params, @id = @itemid;

  fetch next from sqlCursor
  into @DynamicSql
end

close sqlCursor
deallocate sqlCursor

SELECT *
from #WhereUsed

drop table #PropData
drop table #WhereUsed

end]]></sqlserver_body>
  <stale>0</stale>
  <transform_first>0</transform_first>
  <type>procedure</type>
  <name>WhereUsed_General</name>
</Item>";
  }

  public interface IItemData
  {
    string Type { get; }
    string Id { get; }
    object Property(string name);
  }

  internal class DataRowItemData : IItemData
  {
    private DataRow _row;

    public DataRowItemData(DataRow row)
    {
      _row = row;
    }

    public string Id { get { return (string)Property("id"); } }
    public string Type { get { return (string)Property(Extensions.AmlTable_TypeName); } }

    public object Property(string name)
    {
      if (_row.Table.Columns.Contains(name) && !_row.IsNull(name))
        return _row[name];
      return null;
    }
    public void SetProperty(string name, object value)
    {
      _row[name] = value;
      if (_row.RowState == DataRowState.Detached)
        _row.Table.Rows.Add(_row);
    }

    public void Delete()
    {
      if (_row.RowState != DataRowState.Detached)
        _row.Delete();
    }

    public async Task ToAml(Stream stream)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.CheckCharacters = true;
      settings.Async = true;

      using (var xmlWriter = XmlWriter.Create(stream, settings))
      {
        await ToAml(xmlWriter);
      }
    }
    public async Task ToAml(XmlWriter writer)
    {
      var factory = ElementFactory.Local;
      await writer.WriteStartElementAsync(null, "Item", null);
      var id = (string)Property("id");
      var type = (string)Property(Extensions.AmlTable_TypeName);
      var typeId = (string)Property(Extensions.AmlTable_TypeId);
      if (!string.IsNullOrEmpty(id))
        await writer.WriteAttributeStringAsync(null, "id", null, id);
      if (!string.IsNullOrEmpty(type))
        await writer.WriteAttributeStringAsync(null, "type", null, type);
      if (!string.IsNullOrEmpty(typeId))
        await writer.WriteAttributeStringAsync(null, "typeId", null, typeId);

      var cols = _row.Table.Columns.OfType<DataColumn>()
        .Where(c => !_row.IsNull(c))
        .OrderBy(c => c.ColumnName)
        .ToArray();
      for (var i = 0; i < cols.Length; i++)
      {
        await writer.WriteStartElementAsync(null, cols[i].ColumnName, null);
        var j = i + 1;
        var prefix = cols[i].ColumnName + "/";
        while (j < cols.Length && cols[j].ColumnName.StartsWith(prefix))
        {
          await writer.WriteAttributeStringAsync(null, cols[j].ColumnName.Substring(prefix.Length), null
            , factory.LocalizationContext.Format(_row[cols[j]]));
          j++;
        }
        await writer.WriteStringAsync(factory.LocalizationContext.Format(_row[cols[i]]));
        await writer.WriteEndElementAsync();
        i += (j - i) - 1;
      }
      await writer.WriteEndElementAsync();
    }
  }
}
