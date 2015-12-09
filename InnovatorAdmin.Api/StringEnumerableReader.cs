using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class StringEnumerableReader : TextReader
  {
    private int _pos;
    private IEnumerator<string> _enum;
    private bool _hasMore;

    public StringEnumerableReader(IEnumerable<string> value)
    {
      _enum = value.GetEnumerator();
      _hasMore = _enum.MoveNext();
      _pos = 0;
    }

    public override void Close()
    {
      base.Close();
    }
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        _enum.Dispose();
    }
    public override int Peek()
    {
      if (!_hasMore) return -1;
      return (int)_enum.Current[_pos];
    }
    public override int Read()
    {
      if (!_hasMore) return -1;
      var result = (int)_enum.Current[_pos];
      _pos++;
      if (_pos >= _enum.Current.Length)
      {
        _hasMore = _enum.MoveNext();
        _pos = 0;
      }
      return result;
    }
    public override int Read(char[] buffer, int index, int count)
    {
      if (!_hasMore) return 0;

      var totalCopied = 0;
      int copyCount;
      while (totalCopied < count && _hasMore)
      {
        copyCount = Math.Min(count - totalCopied, _enum.Current.Length - _pos);
        _enum.Current.CopyTo(_pos, buffer, index, copyCount);
        totalCopied += copyCount;
        index += copyCount;
        _pos += copyCount;

        // Wrap around
        if (_pos >= _enum.Current.Length)
        {
          _hasMore = _enum.MoveNext();
          _pos = 0;
        }
      }

      return totalCopied;
    }
    public override string ReadLine()
    {
      if (!_hasMore) return null;
      throw new NotSupportedException();
    }

    public override string ReadToEnd()
    {
      if (!_hasMore) return null;
      var builder = new StringBuilder(_enum.Current.Substring(_pos));
      while (_enum.MoveNext())
      {
        builder.Append(_enum.Current);
      }
      _hasMore = false;
      return builder.ToString();
    }
  }
}
