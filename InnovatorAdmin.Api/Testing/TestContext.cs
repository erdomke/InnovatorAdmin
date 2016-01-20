using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Testing
{
  public class TestContext
  {
    private Dictionary<string, string> _parameters = new Dictionary<string, string>();
    private Stack<IAsyncConnection> _conns = new Stack<IAsyncConnection>();
    private List<ProgressState> _progressStack = new List<ProgressState>();

    public IAsyncConnection Connection { get { return _conns.Any() ? _conns.Peek() : null; } }
    public XElement LastResult { get; set; }
    public IDictionary<string, string> Parameters { get { return _parameters; } }
    public Action<int, string> ProgressCallback { get; set; }

    public TestContext(IAsyncConnection conn)
    {
      _conns.Push(conn);
    }

    public void PushConnection(IAsyncConnection conn)
    {
      _conns.Push(conn);
    }
    public IAsyncConnection PopConnection()
    {
      return _conns.Pop();
    }

    public IDisposable StartSubProgress()
    {
      var result = new ProgressState(0, 1, _progressStack);
      _progressStack.Add(result);
      return result;
    }
    public void ReportProgress(int index, int count, string message)
    {
      if (ProgressCallback == null)
        return;

      if (_progressStack.Count < 1)
        _progressStack.Add(new ProgressState(index, count, _progressStack));
      else
        _progressStack.Last().Set(index, count);

      double progress = 0;
      double factor = 1;
      foreach (var prog in _progressStack)
      {
        progress += factor * ((double)prog.Index) / prog.Count;
        factor *= 1.0 / prog.Count;
      }
      ProgressCallback((int)progress, message);
    }

    private class ProgressState : IDisposable
    {
      private List<ProgressState> _stack;

      public int Index { get; set; }
      public int Count { get; set; }

      public ProgressState(int index, int count, List<ProgressState> stack)
      {
        Set(index, count);
        _stack = stack;
      }
      public void Set(int index, int count)
      {
        if (count < 1)
          throw new ArgumentException("count must be greater than 0");
        this.Index = index;
        this.Count = count;
      }

      public void Dispose()
      {
        _stack.Remove(this);
      }
    }
  }
}
