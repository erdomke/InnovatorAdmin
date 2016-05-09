using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public static class ConnectionExtensions
  {
    /// <summary>
    /// Get the result of executing the specified query
    /// </summary>
    /// <param name="conn">Database action</param>
    /// <param name="action">SOAP action</param>
    /// <param name="query">Query to be performed.  If parameters are specified, then the query is fomatted using the <see cref="ElementFactory.FormatQuery"/> method</param>
    /// <param name="parameters">Parameters to be injected into the query</param>
    /// <returns>A read-only result</returns>
    public static IReadOnlyResult Apply(this IConnection conn, Command query, params object[] parameters)
    {
      if (parameters != null)
      {
        for (var i = 0; i < parameters.Length; i++)
        {
          query.WithParam(i.ToString(), parameters[i]);
        }
      }
      return conn.AmlContext.FromXml(conn.Process(query), query.Aml, conn);
    }
    public static IPromise<IReadOnlyResult> ApplyAsync(this IAsyncConnection conn, Command query, bool async, bool noItemsIsError, params object[] parameters)
    {
      var result = new Promise<IReadOnlyResult>();
      if (parameters != null)
      {
        for (var i = 0; i < parameters.Length; i++)
        {
          query.WithParam(i.ToString(), parameters[i]);
        }
      }

      result.CancelTarget(
        conn.Process(query, async)
        .Progress((p, m) => result.Notify(p, m))
        .Done(r =>
          {
            try
            {
              var res = conn.AmlContext.FromXml(r, query.Aml, conn);
              var ex = res.Exception;
              if (ex != null && (noItemsIsError || !(ex is NoItemsFoundException)))
              {
                result.Reject(ex);
              }
              else
              {
                result.Resolve(res);
              }
            }
            catch (Exception ex)
            {
              result.Reject(ex);
            }
          }).Fail(ex =>
          {
            result.Reject(ex);
          }));
      return result;
    }

    public static IReadOnlyResult ApplySql(this IConnection conn, Command sql, params object[] parameters)
    {
      if (parameters != null)
      {
        for (var i = 0; i < parameters.Length; i++)
        {
          sql.WithParam(i.ToString(), parameters[i]);
        }
      }
      if (!sql.Aml.TrimStart().StartsWith("<"))
      {
        sql.Aml = "<sql>" + sql.Aml + "</sql>";
      }
      return conn.Apply(sql.WithAction(CommandAction.ApplySQL));
    }

    public static IPromise<IReadOnlyResult> ApplySql(this IAsyncConnection conn, Command sql, bool async)
    {
      if (!sql.Aml.TrimStart().StartsWith("<"))
      {
        sql.Aml = "<sql>" + sql.Aml + "</sql>";
      }
      return conn.ApplyAsync(sql.WithAction(CommandAction.ApplySQL), async, true);
    }
    public static Connection.DbConnection AsDb(this IConnection conn)
    {
      return new Connection.DbConnection(conn);
    }

    /// <summary>
    /// Retrieve an item based on its type and ID
    /// </summary>
    /// <param name="conn">Connection to query the item on</param>
    /// <param name="itemTypeName">Name of the item type</param>
    /// <param name="id">ID of the item</param>
    public static IReadOnlyItem ItemById(this IConnection conn, string itemTypeName, string id)
    {
      if (itemTypeName.IsNullOrWhiteSpace())
        throw new ArgumentException("Item type must be specified", "itemTypeName");
      if (id.IsNullOrWhiteSpace())
        throw new ArgumentException("ID must be specified", "id");

      var aml = conn.AmlContext;
      var query = new Command("<Item type='@0' id='@1' action=\"get\" />"
                              , itemTypeName, id)
                              .WithAction(CommandAction.ApplyItem);
      return aml.FromXml(conn.Process(query), query.Aml, conn).AssertItem();
    }

    /// <summary>
    /// Retrieve an item based on its type and ID and map it to an object
    /// </summary>
    /// <param name="conn">Connection to query the item on</param>
    /// <param name="itemTypeName">Name of the item type</param>
    /// <param name="id">ID of the item</param>
    /// <param name="mapper">Mapping function used to get an object from the item data</param>
    public static T ItemById<T>(this IConnection conn, string itemTypeName, string id, Func<IReadOnlyItem, T> mapper)
    {
      if (itemTypeName.IsNullOrWhiteSpace())
        throw new ArgumentException("Item type must be specified", "itemTypeName");
      if (id.IsNullOrWhiteSpace())
        throw new ArgumentException("ID must be specified", "id");

      var aml = conn.AmlContext;
      var itemQuery = aml.Item(aml.Type(itemTypeName), aml.Id(id));
      return itemQuery.LazyMap(conn, mapper);
    }

    /// <summary>
    /// Retrieve an item based on its type and keyed name
    /// </summary>
    /// <param name="conn">Connection to query the item on</param>
    /// <param name="itemTypeName">Name of the item type</param>
    /// <param name="keyedName">Keyed name of the item</param>
    public static IReadOnlyItem ItemByKeyedName(this IConnection conn, string itemTypeName, string keyedName)
    {
      if (itemTypeName.IsNullOrWhiteSpace())
        throw new ArgumentException("Item type must be specified", "itemTypeName");
      if (keyedName.IsNullOrWhiteSpace())
        throw new ArgumentException("Keyed name must be specified", "keyedName");

      var aml = conn.AmlContext;
      var query = new Command("<Item type='@0 action=\"get\"><keyed_name>@1</keyed_name></Item>"
                              , itemTypeName, keyedName)
                              .WithAction(CommandAction.ApplyItem);
      return aml.FromXml(conn.Process(query), query.Aml, conn).AssertItem();
    }

    /// <summary>
    /// Get a single item from the database using the specified query.  If the result is not a single item, an exception will be thrown
    /// </summary>
    /// <param name="conn">Server connection</param>
    /// <param name="action">SOAP action</param>
    /// <param name="request">Query/command which should return a single item</param>
    /// <returns>A single readonly item</returns>
    public static IReadOnlyItem ItemByQuery(this IConnection conn, Command request)
    {
      return ItemByQuery(conn, request, false).Value;
    }
    /// <summary>
    /// Get a single item from the database using the specified query asynchronously.  If the result is not a single item, an exception will be thrown
    /// </summary>
    /// <param name="conn">Server connection</param>
    /// <param name="action">SOAP action</param>
    /// <param name="request">Query/command which should return a single item</param>
    /// <param name="async">Whether to perform this request asynchronously</param>
    /// <returns>A promise to return a single readonly item</returns>
    public static IPromise<IReadOnlyItem> ItemByQuery(this IConnection conn, Command request, bool async)
    {
      var result = new Promise<IReadOnlyItem>();
      result.CancelTarget(conn.ProcessAsync(request, async)
        .Progress((p, m) => result.Notify(p, m))
        .Done(r =>
        {
          if (string.IsNullOrEmpty(conn.UserId))
          {
            result.Reject(new LoggedOutException());
          }
          else
          {
            var res = conn.AmlContext.FromXml(r, request.Aml, conn);
            var ex = res.Exception;
            if (ex == null)
            {

              try
              {
                result.Resolve(res.AssertItem());
              }
              catch (Exception exc)
              {
                result.Reject(exc);
              }
            }
            else
            {
              result.Reject(ex);
            }
          }
        }).Fail(ex => result.Reject(ex)));
      return result;
    }
    public static IReadOnlyItem Lock(this IConnection conn, string itemTypeName, string id)
    {
      var aml = conn.AmlContext;
      return aml.Item(aml.Action("lock"),
        aml.Type(itemTypeName),
        aml.Id(id)
      ).Apply(conn).AssertItem();
    }
    public static string NextSequence(this IConnection conn, string sequenceName)
    {
      if (sequenceName.IsNullOrWhiteSpace())
        throw new ArgumentException("Sequence name must be specified", "sequenceName");

      var aml = conn.AmlContext;
      var query = new Command("<Item><name>@0</name></Item>", sequenceName)
                              .WithAction(CommandAction.GetNextSequence);
      return aml.FromXml(conn.Process(query), query.Aml, conn).Value;
    }
    internal static IPromise<System.IO.Stream> ProcessAsync(this IConnection conn, Command cmd, bool async)
    {
      var remote = conn as IAsyncConnection;
      if (remote != null)
      {
        return remote.Process(cmd, async);
      }

      var result = new Promise<System.IO.Stream>();
      try
      {
        result.Resolve(conn.Process(cmd));
      }
      catch (Exception ex)
      {
        result.Reject(ex);
      }
      return result;
    }
    public static IReadOnlyResult Promote(this IConnection conn, string itemTypeName, string id, string newState, string comments = null)
    {
      if (newState.IsNullOrWhiteSpace()) throw new ArgumentException("State must be a non-empty string to run a promotion.", "state");
      var aml = conn.AmlContext;
      var promoteItem = aml.Item(aml.Action("promoteItem"),
        aml.Type(itemTypeName),
        aml.Id(id),
        aml.State(newState)
      );
      if (!string.IsNullOrEmpty(comments)) promoteItem.Add(aml.Property("comments", comments));
      return promoteItem.Apply(conn).AssertNoError();
    }
    public static IReadOnlyItem Unlock(this IConnection conn, string itemTypeName, string id)
    {
      var aml = conn.AmlContext;
      return aml.Item(aml.Action("unlock"),
        aml.Type(itemTypeName),
        aml.Id(id)
      ).Apply(conn).AssertItem();
    }
  }
}
