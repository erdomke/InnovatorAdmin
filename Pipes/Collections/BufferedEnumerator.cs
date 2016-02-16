using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Collections
{
  public class BufferedEnumerator<T> : IEnumerator<T>
  {
    private IEnumerator<T> _core;
    private InspectableQueue<T> _buffer = new InspectableQueue<T>();
    private int _peekIndex = 0;
    
    public BufferedEnumerator(IEnumerator<T> core)
    {
      _core = core;
    }
    public BufferedEnumerator(IEnumerable<T> core)
    {
      _core = core.GetEnumerator();
    }

    public T Current
    {
      get { return _buffer.Count > 0 ? _buffer.Peek() : _core.Current; }
    }

    public bool PeekNext()
    {
      if (++_peekIndex < _buffer.Count) return true;
      if (_buffer.Count < 1) _buffer.Enqueue(_core.Current);
      if (_core.MoveNext())
      {
        _buffer.Enqueue(_core.Current);
        return true;
      }
      return false;
    }

    public T CurrentPeek
    {
      get { return _buffer.GetElement(_peekIndex); }
    }

    public void Dispose()
    {
      _core.Dispose();
    }

    object System.Collections.IEnumerator.Current
    {
      get { return this.Current; }
    }

    public bool MoveNext()
    {
      if (_buffer.Count > 0)
      {
        _buffer.Dequeue();
        _peekIndex = 0;
      }
      return _buffer.Count > 0 || _core.MoveNext();
    }

    public void Reset()
    {
      
    }
  }
}
