using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.IO
{
  public class FileStreamSource : IPipeOutput<Stream>
  {
    private IEnumerable<string> _fileNames;

    public FileStreamSource(string value)
    {
      _fileNames = new Collections.SingleItemEnumerable<string>(value);
    }
    public FileStreamSource(IEnumerable<string> values)
    {
      _fileNames = values;
    }

    public IEnumerator<Stream> GetEnumerator()
    {
      foreach (var str in _fileNames)
      {
        var reader = new FileStream(str, FileMode.Open, FileAccess.Read);
        yield return reader;
        reader.Dispose();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
