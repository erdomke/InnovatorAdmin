using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class DelimitedTextDataReader : IPipeInput<IEnumerable<string>>, IPipeOutput<IDataRecord>
  {
    private bool _headers;
    private IEnumerable<IEnumerable<string>> _source;

    public bool HasHeaders
    {
      get { return _headers; }
      set { _headers = value; }
    }

    public void Initialize(IEnumerable<IEnumerable<string>> source)
    {
      _source = source;
    }
    public IEnumerator<IDataRecord> GetEnumerator()
    {
      string[] columnNames = null;
      foreach (var line in _source)
      {
        if (_headers && columnNames == null)
        {
          columnNames = line.ToArray();
        }
        else
        {
          if (_headers)
          {
            yield return new DataRecord(line.OfType<object>().ToArray(), columnNames);
          }
          else
          {
            yield return new DataRecord(line.OfType<object>().ToArray());
          }
        }
      }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
