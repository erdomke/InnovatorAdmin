using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Merge
{
  class FileSysDiffDirectory : IDiffDirectory
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
      private string _compare;

      public FileSysDiffFile(string baseDir, string path)
      {
        _base = baseDir;
        _path = path;
      }

      public IComparable CompareKey
      {
        get
        {
          if (_compare == null)
          {
            using (var md5 = new MD5CryptoServiceProvider())
            using (var stream = new FileStream(System.IO.Path.Combine(_base, _path), FileMode.Open, FileAccess.Read))
            {
              var result = new StringBuilder(32);
              var hash = md5.ComputeHash(stream);
              for (var i = 0; i < hash.Length; i++)
              {
                result.AppendFormat("{0:x2}", hash[i]);
              }
              return result.ToString();
            }
          }
          return _compare;
        }
      }

      public string Path { get { return _path; } }

      public Stream OpenRead()
      {
        return new FileStream(System.IO.Path.Combine(_base, _path), FileMode.Open, FileAccess.Read);
      }
    }
  }
}
