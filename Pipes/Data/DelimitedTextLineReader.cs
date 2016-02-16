using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class DelimitedTextLineReader : IPipeInput<System.IO.TextReader>, IPipeOutput<IEnumerable<String>>
  {
    private DelimitedTextReader _base;
    private bool _skipEmptyLines;

    public char FieldEnclosingChar
    {
      get { return _base.FieldEnclosingChar; }
      set { _base.FieldEnclosingChar = value; }
    }
    public char EnclosingCharEscape
    {
      get { return _base.EnclosingCharEscape; }
      set { _base.EnclosingCharEscape = value; }
    }
    public bool SkipEmptyLines
    {
      get { return _skipEmptyLines; }
      set { _skipEmptyLines = value; }
    }


    public DelimitedTextLineReader()
    {
      _base = new DelimitedTextReader();
      _base.AddDelim('\r');
    }

    public void AddDelim(char delim)
    {
      _base.AddDelim(delim);
    }
    public void AddDelim(string delim)
    {
      _base.AddDelim(delim);
    }
    public void Initialize(IEnumerable<System.IO.TextReader> source)
    {
      _base.Initialize(source);
    }

    public IEnumerator<IEnumerable<string>> GetEnumerator()
    {
      var reader = _base.GetEnumerator() as DelimitedTextReader.DelimitedEnumerator;
      var values = new List<string>();
      while (reader.MoveNext())
      {
        values.Add(reader.Current);
        if (reader.LastMatch == '\r' || reader.LastMatch == '\n' || reader.LastMatch == '\0')
        {
          if (values.Count == 1 && string.IsNullOrEmpty(values[0])) values.Clear();
          if (!_skipEmptyLines || values.Count > 0) yield return values;
          values = new List<string>();
        }
      }
      if (values.Count > 0) yield return values;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
