using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InnovatorAdmin
{
  public class FileSysDiffDirectory : IDiffDirectory
  {
    private string _base;

    public FileSysDiffDirectory(string directory)
    {
      _base = directory;
    }

    public IEnumerable<IDiffFile> GetFiles()
    {
      return Directory.GetFiles(_base, "*", SearchOption.AllDirectories)
        .Select(f => new FileSysDiffFile(_base, f.Substring(_base.Length).TrimStart('\\'))).ToArray();
    }

    private class FileSysDiffFile : IDiffFile
    {
      private string _base;
      private string _path;

      public FileSysDiffFile(string baseDir, string path)
      {
        _base = baseDir;
        _path = path;
      }

      public string Path { get { return _path; } }

      public Stream OpenRead()
      {
        return new FileStream(System.IO.Path.Combine(_base, _path), FileMode.Open, FileAccess.Read);
      }
    }
  }
}
