using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InnovatorAdmin
{
  public class DirectoryPackage : IPackage
  {
    private readonly string _manifestPath;
    private string _base;

    public DirectoryPackage(string directory)
    {
      if (directory.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase)
        || directory.EndsWith(".mf", StringComparison.OrdinalIgnoreCase))
      {
        _manifestPath = Path.GetFileName(directory);
        _base = Path.GetDirectoryName(directory);
      }
      else
      {
        _base = directory;
        _manifestPath = Directory.GetFiles(directory, "*.innpkg")
          .Concat(Directory.GetFiles(directory, "*.mf"))
          .FirstOrDefault();
        if (_manifestPath != null)
          _manifestPath = Path.GetFileName(_manifestPath);
      }
    }

    public bool TryAccessFile(string path, bool create, out IPackageFile file)
    {
      if (create || File.Exists(Path.Combine(_base, path)))
      {
        file = new FileSysFile(_base, path, create);
        return true;
      }
      else
      {
        file = null;
        return false;
      }
    }

    public IEnumerable<IPackageFile> Files()
    {
      return Directory.GetFiles(_base, "*", SearchOption.AllDirectories)
        .Select(f => new FileSysFile(_base, f.Substring(_base.Length).TrimStart('\\'), false))
        .Where(f => !string.Equals(f.Path, _manifestPath, StringComparison.OrdinalIgnoreCase)
          && !f.Path.EndsWith(".mf", StringComparison.OrdinalIgnoreCase));
    }

    public IPackageFile Manifest(bool create)
    {
      return TryAccessFile(_manifestPath, create, out var file) ? file : null;
    }

    public void Dispose()
    {
      // Do nothing
    }

    private class FileSysFile : IPackageFile
    {
      private string _base;
      private string _path;
      private bool _create;

      public FileSysFile(string baseDir, string path, bool create)
      {
        _base = baseDir.TrimEnd('/', '\\');
        _path = path.Replace('\\', '/').Trim('/');
        _create = create;
      }

      public string Path { get { return _path; } }

      public Stream Open()
      {
        var path = System.IO.Path.Combine(_base, _path);
        if (_create)
        {
          var dir = System.IO.Path.GetDirectoryName(path);
          if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
          return new FileStream(path, FileMode.Create, FileAccess.Write);
        }
        else
        {
          return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
      }
    }
  }
}
