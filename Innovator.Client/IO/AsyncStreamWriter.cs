using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Innovator.Client
{
  internal class AsyncStreamWriter : IStreamWriter
  {
    private const int BufferSize = 4096;

    private bool _isClosed = false;
    private Action _onComplete;
    private Action<Exception> _onError;
    private WorkQueue _readQueue = new WorkQueue();
    private Stream _stream;
    private WorkQueue _writeQueue = new WorkQueue();

    public AsyncStreamWriter(Stream stream, Action onComplete, Action<Exception> onError)
    {
      _stream = stream;
      _onComplete = onComplete;
      _onError = onError;
    }

    public void Close()
    {
      if (!_isClosed)
      {
        _readQueue.QueueAndStart(() =>
        {
          _writeQueue.QueueAndStart(() =>
          {
            try
            {
              if (_stream.ShouldClose())
                _stream.Dispose();
              else
                _stream.Flush();
              _onComplete.Invoke();
            }
            catch (Exception ex)
            {
              if (_onError != null) _onError.Invoke(ex);
            }
          });
          _readQueue.StartNext();
        });
        _isClosed = true;
      }
    }

    public IStreamWriter Write(params byte[] value)
    {
      return Write(value, 0, value.Length);
    }

    public IStreamWriter Write(byte[] buffer, int offset, int count)
    {
      _readQueue.QueueAndStart(() =>
      {
        _writeQueue.QueueAndStart(() => WriteAsync(buffer, offset, count));
        _readQueue.StartNext();
      });
      return this;
    }

    public IStreamWriter Write(Stream value)
    {
      if (!value.CanRead)
        throw new ArgumentException("Stream must be readable.");

      _readQueue.QueueAndStart(() => ReadAsync(value, BufferSize));
      return this;
    }

    public IStreamWriter Write(string value)
    {
      var bytes = Encoding.UTF8.GetBytes(value);
      return Write(bytes, 0, bytes.Length);
    }

    private void ReadAsync(Stream value, int count)
    {
      try
      {
        var buffer = new byte[count];
        value.BeginRead(buffer, 0, count, new AsyncCallback(ar =>
        {
          try
          {
            int read = value.EndRead(ar);
            if (read > 0)
            {
              _writeQueue.QueueAndStart(() => WriteAsync(buffer, 0, read));
            }
            if (read >= count)
            {
              // recurse as there might be more in the buffer
              ReadAsync(value, count);
            }
            else
            {
              _readQueue.StartNext();
            }
          }
          catch (Exception ex)
          {
            if (_onError != null) _onError.Invoke(ex);
          }
        }), buffer);
      }
      catch (Exception ex)
      {
        if (_onError != null) _onError.Invoke(ex);
      }
    }

    private void WriteAsync(byte[] buffer, int offset, int count)
    {
      _stream.BeginWrite(buffer, offset, count, new AsyncCallback(ar =>
      {
        try
        {
          _stream.EndWrite(ar);
          _writeQueue.StartNext();
        }
        catch (Exception ex)
        {
          if (_onError != null) _onError.Invoke(ex);
        }
      }), true);
    }

    private class WorkQueue
    {
      private Queue<Action> _items = new Queue<Action>();
      private object _mutex = new object();

      public void QueueAndStart(Action workItem)
      {
        var needsStart = _items.Count < 1;
        _items.Enqueue(workItem);
        if (needsStart) workItem.Invoke();
      }

      public void StartNext()
      {
        _items.Dequeue();
        if (_items.Count > 0) 
          _items.Peek().Invoke();
      }
    }
  }
}
