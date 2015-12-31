using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public static class SqlExtension
  {
    public static async Task<SqlResultObject> GetResultAsync(this SqlCommand cmd
      , CancellationToken token = default(CancellationToken))
    {
      if (cmd.Connection.State == ConnectionState.Closed)
        await cmd.Connection.OpenAsync(token);

      using (var reader = await cmd.ExecuteReaderAsync())
      {
        var dataSet = new DataSet();
        var nextResult = true;
        while (nextResult)
        {
          var table = new DataTable();
          table.Load(reader);
          dataSet.Tables.Add(table);

          for (var i = 0; i < table.Columns.Count; i++)
          {
            table.Columns[i].SortOrder(i * 10);
          }

          nextResult = !reader.IsClosed && reader.HasRows;
        }

        return new SqlResultObject(dataSet, dataSet.Tables.Count > 0
          ? dataSet.Tables.OfType<DataTable>().GroupConcat(Environment.NewLine, t => string.Format("({0} row(s) affected)", t.Rows.Count))
          : string.Format("({0} row(s) affected)", reader.RecordsAffected));
      }
    }

    public static async Task<IList<T>> GetListAsync<T>(this SqlCommand cmd
      , Func<DbDataReader, Task<T>> builder
      , CancellationToken token = default(CancellationToken))
    {
      var result = new List<T>();

      if (cmd.Connection.State == ConnectionState.Closed)
        await cmd.Connection.OpenAsync(token);

      using (var reader = await cmd.ExecuteReaderAsync(token))
      {
        while (await reader.ReadAsync())
        {
          result.Add(await builder.Invoke(reader));
        }
      }

      return result;
    }

    public static async Task<string> GetFieldStringAsync(this DbDataReader reader, int fieldIndex)
    {
      if (await reader.IsDBNullAsync(fieldIndex))
        return null;
      return await reader.GetFieldValueAsync<string>(fieldIndex);
    }
  }
}
