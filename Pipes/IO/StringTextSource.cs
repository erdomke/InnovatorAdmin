using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.IO
{
  public class StringTextSource : IPipeOutput<System.IO.TextReader>
  {
    private IEnumerable<string> _strings;

    public StringTextSource(string value)
    {
      _strings = new Collections.SingleItemEnumerable<string>(value);
    }
    public StringTextSource(IEnumerable<string> values)
    {
      _strings = values;
    }

    public IEnumerator<System.IO.TextReader> GetEnumerator()
    {
      foreach (var str in _strings)
      {
        var reader = new System.IO.StringReader(str);
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
