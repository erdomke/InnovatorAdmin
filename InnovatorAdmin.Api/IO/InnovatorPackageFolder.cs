using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InnovatorAdmin
{
  public class InnovatorPackageFolder : InnovatorPackage
  {
    private string _basePath;

    public InnovatorPackageFolder(string path)
    {
      _basePath = path;
    }

    protected override System.IO.Stream GetExistingStream(string path)
    {
      return new FileStream(GetRelativePath(path), FileMode.Open, FileAccess.Read);
    }

    protected override System.IO.Stream GetNewStream(string path)
    {
      return new FileStream(EnsureRelativePath(path), FileMode.OpenOrCreate, FileAccess.Write);
    }

    private string GetRelativePath(string path)
    {
      if (string.IsNullOrEmpty(path)) return _basePath;
      return Path.Combine(Path.GetDirectoryName(_basePath), path);
    }
    private string EnsureRelativePath(string path)
    {
      var fullPath = GetRelativePath(path);
      var dir = Path.GetDirectoryName(fullPath);
      if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
      return fullPath;
    }
  }
}
