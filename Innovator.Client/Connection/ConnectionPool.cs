using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Innovator.Client
{
  public class ConnectionPool : IAsyncConnection, IDisposable
  {
    PooledConnection[] _pool;
    private IRemoteConnection _ref;
    private Promise<bool> _available;

    private ConnectionPool(IRemoteConnection conn, int size)
    {
      _available = new Promise<bool>();
      _ref = conn;
      _pool = new PooledConnection[size];
      for (var i = 0; i < size; i++)
      {
        var idx = i;
        conn.Clone(true)
          .Done(c => {
            _pool[idx] = new PooledConnection(c);
            _available.Resolve(true);
          });
      }
    }

    public static IPromise<ConnectionPool> Create(IRemoteConnection conn, int size)
    {
      var result = new ConnectionPool(conn, size);
      return result._available
        .Convert(a => result);
    }

    public ElementFactory AmlContext { get { return _ref.AmlContext; } }
    public string Database { get { return _ref.Database; } }
    public string UserId { get { return _ref.UserId; } }
    public UploadCommand CreateUploadCommand()
    {
      return _ref.CreateUploadCommand();
    }

    public void Dispose()
    {
      for (var i = 0; i < _pool.Length; i++)
      {
        var conn = _pool[i];
        _pool[i] = null;
        conn.Dispose();
      }
    }

    public string MapClientUrl(string relativeUrl)
    {
      return _ref.MapClientUrl(relativeUrl);
    }

    public Stream Process(Command request)
    {
      var conn = GetConnection();
      return conn.Process(request, false).Value;
    }

    public IPromise<Stream> Process(Command request, bool async)
    {
      var conn = GetConnection();
      return conn.Process(request, async);
    }

    private PooledConnection GetConnection()
    {
      PooledConnection result = null;
      for (var i = 0; i < _pool.Length; i++)
      {
        var curr = _pool[i];
        if (curr != null)
        {
          if (result == null)
          {
            result = curr;
          }
          else if (result.ConcurrentQueries > curr.ConcurrentQueries)
          {
            result = curr;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Individual connection which tracks the number of pending queries
    /// </summary>
    private class PooledConnection : IDisposable
    {
      private int _concurrentQueries;
      private IRemoteConnection _conn;
      private ConnectionState _state = ConnectionState.Normal;

      public int ConcurrentQueries { get { return _concurrentQueries; } }

      public PooledConnection(IRemoteConnection conn)
      {
        _conn = conn;
      }

      public IPromise<Stream> Process(Command cmd, bool async)
      {
        if (_state != ConnectionState.Normal)
          return Promises.Rejected<Stream>(new ObjectDisposedException("Cannot execute a query because the connection is being disposed (i.e. logged out)."));
        Interlocked.Increment(ref _concurrentQueries);
        return _conn.Process(cmd, async)
          .Always(() =>
          {
            var newCount = Interlocked.Decrement(ref _concurrentQueries);
            if (newCount < 1 && _state == ConnectionState.Disposing)
              ExecuteDispose();
          });
      }

      public void Dispose()
      {
        if (_state == ConnectionState.Normal)
        {
          _state = ConnectionState.Disposing;
          if (_concurrentQueries < 1)
            ExecuteDispose();
        }
      }

      private void ExecuteDispose()
      {
        var conn = _conn;
        _conn = null;
        conn.Dispose();
        _state = ConnectionState.Disposed;
      }

      private enum ConnectionState
      {
        Normal,
        Disposing,
        Disposed
      }
    }
  }
}
