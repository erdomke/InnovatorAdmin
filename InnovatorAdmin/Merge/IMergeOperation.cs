using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IMergeOperation
  {
    IEnumerable<FileCompare> GetChanges();
    Stream GetLocal(string relPath);
    Stream GetRemote(string relPath);
    string MergePath(string relPath);
    MergeFilePaths GetPaths(string relPath);
  }
}
