using System;
using System.Collections.Generic;

namespace InnovatorAdmin
{
  public interface IPackage : IDisposable
  {
    IEnumerable<IPackageFile> Files();
    IPackageFile Manifest(bool create);
    bool TryAccessFile(string path, bool create, out IPackageFile file);
  }
}
