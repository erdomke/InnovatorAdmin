using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InnovatorAdmin.Tests
{
  internal class MockDiffDirectory : IPackage
  {
    private List<MockDiffFile> _files = new List<MockDiffFile>();

    public void Add(string path, string xml)
    {
      _files.Add(new MockDiffFile(path, xml));
    }

    public void Dispose()
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<IPackageFile> Files()
    {
      return _files;
    }

    public IPackageFile Manifest(bool create)
    {
      if (create)
        throw new System.NotImplementedException();
      return new MockDiffFile("something.innpkg", "<Package/>");
    }

    public bool TryAccessFile(string path, bool create, out IPackageFile file)
    {
      throw new System.NotImplementedException();
    }

    private class MockDiffFile : IPackageFile
    {
      private string _xml;

      public string Path { get; }

      public MockDiffFile(string path, string xml)
      {
        Path = path;
        _xml = xml;
      }

      public Stream Open()
      {
        return new MemoryStream(Encoding.UTF8.GetBytes(_xml));
      }
    }
  }
}
