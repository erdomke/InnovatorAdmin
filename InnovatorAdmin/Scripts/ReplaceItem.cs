using Innovator.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Scripts
{
  [DisplayName("Replace Item")]
  public class ReplaceItem : IAsyncScript
  {
    [DisplayName("Original ID"), Description("The ID of the original item which will be replaced")]
    public string OrigId { get; set; }
    [DisplayName("Replace ID"), Description("The ID of the replacement item with which the original will be replaced")]
    public string ReplaceId { get; set; }
    [DisplayName("Item Type"), Description("The type of the item to replace")]
    public string Type { get; set; }
    [DisplayName("Generate SQL"), Description("Whether to generate the SQL for the replace instead of actually doing the replace")]
    public bool GenerateSql { get; set; }

    public IPromise<string> Execute(IAsyncConnection conn)
    {
      return ExecuteAsync(conn).ToPromise();
    }

    private async Task<string> ExecuteAsync(IAsyncConnection conn)
    {
      var whereUsed = await GetWhereUsed(conn, this.Type, this.OrigId);
      var finalSql = new StringBuilder();
      string sql;
      string props;
      IReadOnlyResult res;
      foreach (var item in whereUsed)
      {
        props = item.Properties.Select(p => p + " = '" + this.ReplaceId + "'").GroupConcat(", ");
        sql = "update innovator.[" + item.MainType.Replace(' ', '_') + "] set " + props + " where id ='" + item.MainId + "';";
        if (this.GenerateSql)
        {
          finalSql.AppendLine(sql);
        }
        else
        {
          res = await conn.ApplySql(sql, true).ToTask();
          res.AssertNoError();
        }
      }

      if (this.GenerateSql)
        return finalSql.ToString();
      return "Complete.";
    }

    private async Task<IEnumerable<WhereUsedItem>> GetWhereUsed(IAsyncConnection conn, string type, string id)
    {
      var whereUsed = await conn.ApplyAsync(@"<AML>
                                                <Item type='SQL' action='SQL Process'>
                                                  <name>WhereUsed_General</name>
                                                  <PROCESS>CALL</PROCESS>
                                                  <ARG1>@0</ARG1>
                                                  <ARG2>@1</ARG2>
                                                </Item>
                                              </AML>", true, false, type, id).ToTask();
      return (
        from i in whereUsed.Items()
        where string.IsNullOrEmpty(type) || i.Property("parent_type").Value == type
        select new WhereUsedItem()
        {
          Type = i.Property("parent_type").Value,
          Id = i.Property("parent_id").Value,
          Icon = i.Property("icon").Value,
          MainType = i.Property("main_type").Value,
          MainId = i.Property("main_id").Value,
          Properties = i.Elements().OfType<IReadOnlyProperty>()
            .Where(p => p.Name.Length == 2 && p.Name[0] == 'p' && char.IsNumber(p.Name[1]))
            .Select(p => p.Value),
          Generation = i.Generation().AsInt(1),
          KeyedName = i.KeyedName().Value,
          MajorRev = i.MajorRev().Value
        }).ToList();
    }

    private class WhereUsedItem
    {
      public string Type { get; set; }
      public string Id { get; set; }
      public string Icon { get; set; }
      public string MainType { get; set; }
      public string MainId { get; set; }
      public IEnumerable<string> Properties { get; set; }
      public int Generation { get; set; }
      public string KeyedName { get; set; }
      public string MajorRev { get; set; }
    }
  }
}
