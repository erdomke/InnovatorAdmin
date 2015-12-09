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

      using (var reader = await cmd.ExecuteReaderAsync(token))
      {
        var table = new DataTable();
        table.Load(reader);
        return new SqlResultObject(table,
          string.Format("({0} row(s) affected)", Math.Max(table.Rows.Count, reader.RecordsAffected)));
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
  }
}
