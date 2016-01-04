using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace InnovatorAdmin
{
  public class SnippetManager
  {
    private static string _lastQueryKey = "___LastQuery";

    public static string LastQueryKey 
    {
      get { return _lastQueryKey; }
      set { _lastQueryKey = value; }
    }
    private Dictionary<string, Snippet> _cache = new Dictionary<string, Snippet>();
    private BlockingCollection<Action> _queue = new BlockingCollection<Action>(100);
    private Thread _thread;

    [System.Diagnostics.DebuggerStepThrough()]
    public SnippetManager()
    {
      _thread = new Thread(() =>
      {
        Action action;
        while (!_queue.IsCompleted)
        {
          action = null;
          try
          {
            action = _queue.Take();
          }
          catch (InvalidOperationException) { }

          if (action != null)
            action.Invoke();
        }
      });
      _thread.Start();
    }

    public Snippet GetLastQueryByConnection(string connection)
    {
      var result = this[_lastQueryKey + "_" + (connection ?? "")];
      if (result.IsEmpty())
        result = this[_lastQueryKey];
      return result;
    }
    public void SetLastQueryByConnection(string connection, Snippet value)
    {
      this[_lastQueryKey + "_" + connection] = value;
    }

    public Snippet this[string name]
    {
      get
      {
        Snippet result;
        var key = CleanFileName(name);
        if (!_cache.TryGetValue(key, out result))
        {
          var path = GetFolder() + key + ".txt";
          if (System.IO.File.Exists(path))
            result = new Snippet(System.IO.File.ReadAllText(path));
          else
            result = new Snippet();
          _cache[key] = result;
        }
        return result;
      }
      set
      {
        var key = CleanFileName(name);
        _cache[key] = value;
        var path = GetFolder() + key + ".txt";
        _queue.Add(() =>
        {
          System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
          System.IO.File.WriteAllText(path, value.ToString());
        });
      }
    }

    public bool ContainsName(string name)
    {
      var key = CleanFileName(name);
      return _cache.ContainsKey(key);
    }

    public IEnumerable<string> Names()
    {
      return System.IO.Directory.GetFiles(GetFolder(), "*.txt")
        .Select(f => System.IO.Path.GetFileNameWithoutExtension(f))
        .Where(n => n != LastQueryKey);
    }

    private string CleanFileName(string name)
    {
      var invalidChars = System.IO.Path.GetInvalidFileNameChars();
      var nameArray = name.ToCharArray(0, Math.Min(name.Length, 96));

      for (var i = 0; i < nameArray.Length; i++)
      {
        if (invalidChars.Contains(nameArray[i]))
        {
          nameArray[i] = '_';
        }
      }

      return new string(nameArray);
    }

    public void Close()
    {
      _queue.CompleteAdding();
      _thread.Join(3000);
    }

    internal static string GetFolder()
    {
      string path = @"{0}\{1}\Snippets\";
      return string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin");
    }

    private static SnippetManager _instance = new SnippetManager();
    public static SnippetManager Instance { get { return _instance; } }
  }
}
