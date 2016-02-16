using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.IO
{
  public class FileTextSource : IPipeOutput<System.IO.TextReader>
  {
    private IEnumerable<string> _fileNames;

    public FileTextSource(string value)
    {
      _fileNames = new Collections.SingleItemEnumerable<string>(value);
    }
    public FileTextSource(IEnumerable<string> values)
    {
      _fileNames = values;
    }

    public IEnumerator<System.IO.TextReader> GetEnumerator()
    {
      return new FileTextEnumerator(_fileNames.GetEnumerator());
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private class FileTextEnumerator : IEnumerator<System.IO.TextReader>
    {
      private IEnumerator<string> _fileNames;
      private System.IO.TextReader _curr;

      public TextReader Current
      {
        get { return _curr; }
      }
      object System.Collections.IEnumerator.Current
      {
        get { return _curr; }
      }

      public FileTextEnumerator(IEnumerator<string> fileNames)
      {
        _fileNames = fileNames;
      }

      public void Dispose()
      {
        if (_curr != null) _curr.Dispose();
      }
      public bool MoveNext()
      {
        Dispose();
        if (_fileNames.MoveNext())
        {
          _curr = new System.IO.StreamReader(_fileNames.Current);
          return true;
        }
        else
        {
          return false;
        }
      }
      public void Reset()
      {
        Dispose();
        _fileNames.Reset();
      }
    }
  }
}
