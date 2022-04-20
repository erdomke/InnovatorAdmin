using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InnovatorAdmin.Tests
{
  internal class MockDiffDirectory : IDiffDirectory
  {
    private List<MockDiffFile> _files = new List<MockDiffFile>();

    public void Add(string path, string xml)
    {
      _files.Add(new MockDiffFile(path, xml));
    }

    public IEnumerable<IDiffFile> GetFiles()
    {
      return _files;
    }

    private class MockDiffFile : IDiffFile
    {
      private string _xml;

      public string Path { get; }

      public MockDiffFile(string path, string xml)
      {
        Path = path;
        _xml = xml;
      }

      public Stream OpenRead()
      {
        return new MemoryStream(Encoding.UTF8.GetBytes(_xml));
      }
    }
  }
}
