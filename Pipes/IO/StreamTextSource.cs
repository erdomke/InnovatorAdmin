using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.IO
{
  public class StreamTextSource : IPipeOutput<System.IO.TextReader>
  {
    private IEnumerable<Stream> _streams;

    public StreamTextSource(Stream value)
    {
      _streams = new Collections.SingleItemEnumerable<Stream>(value);
    }
    public StreamTextSource(IEnumerable<Stream> values)
    {
      _streams = values;
    }

    public IEnumerator<System.IO.TextReader> GetEnumerator()
    {
      foreach (var str in _streams)
      {
        if (str.CanSeek) str.Seek(0, SeekOrigin.Begin);
        var reader = new System.IO.StreamReader(str);
        yield return reader;
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
