using System;
using System.IO;

namespace InnovatorAdmin
{
  public interface IPackageFile
  {
    string Path { get; }
    Stream Open();
  }
}
