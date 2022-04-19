using System;
using System.IO;

namespace InnovatorAdmin
{
  public interface IDiffFile
  {
    string Path { get; }
    Stream OpenRead();
  }
}
