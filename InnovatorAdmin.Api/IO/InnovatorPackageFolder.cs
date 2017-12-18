using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InnovatorAdmin
{
  public class InnovatorPackageFolder : InnovatorPackage
  {
    private readonly string _manifestPath;
    private readonly string _baseFolder;

    public InnovatorPackageFolder(string path)
    {
      _manifestPath = path;
      _baseFolder = Path.GetDirectoryName(_manifestPath);
    }

    protected override System.IO.Stream GetExistingStream(string path)
    {
      var relPath = GetRelativePath(path);
      if (!File.Exists(relPath))
        return null;

      return new FileStream(relPath, FileMode.Open, FileAccess.Read);
    }

    protected override System.IO.Stream GetNewStream(string path)
    {
      return new FileStream(EnsureRelativePath(path), FileMode.Create, FileAccess.Write);
    }

    private string GetRelativePath(string path)
    {
      if (string.IsNullOrEmpty(path)) return _manifestPath;
      return Path.Combine(_baseFolder, path);
    }
    private string EnsureRelativePath(string path)
    {
      var fullPath = GetRelativePath(path);
      var dir = Path.GetDirectoryName(fullPath);
      if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
      return fullPath;
    }

    protected override bool PathExists(string path)
    {
      return File.Exists(GetRelativePath(path));
    }

    protected override IEnumerable<string> GetPaths()
    {
      return Directory.GetFiles(_baseFolder, "*.*", SearchOption.AllDirectories)
        .Where(p => !string.Equals(p, _manifestPath, StringComparison.OrdinalIgnoreCase))
        .Select(p => p.Substring(_baseFolder.Length).TrimStart('\\'));
    }
  }
}
