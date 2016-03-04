using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        WriteAllText(path, value.ToString());
      }
    }

    public async Task WriteAllText(string path, string value)
    {
      System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
      using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
      using (var writer = new StreamWriter(stream))
      using (var reader = new StringReader(value))
      {
        await reader.CopyToAsync(writer);
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

    public static string CleanFileName(string name)
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

    internal static string GetFolder()
    {
      string path = @"{0}\{1}\Snippets\";
      return string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin");
    }

    private static SnippetManager _instance = new SnippetManager();
    public static SnippetManager Instance { get { return _instance; } }
  }
}
