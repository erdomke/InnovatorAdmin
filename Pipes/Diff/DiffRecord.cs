using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Diff
{
  public class DiffRecord<T> : IDiffRecord<T>
  {
    private List<IDiffRecord> _changes = new List<IDiffRecord>();

    public T Base { get; set; }
    public T Compare { get; set; }
    public DiffAction Action { get; set; }

    object IDiffRecord.Base 
    { 
      get 
      { 
        return this.Base; 
      }
    }
    object IDiffRecord.Compare
    {
      get
      {
        return this.Compare;
      }
    }

    public IEnumerable<IDiffRecord> Changes 
    {
      get
      {
        return _changes;
      }
    }

    public void AddChange(IDiffRecord change)
    {
      _changes.Add(change);
    }
    public void AddChanges(IEnumerable<IDiffRecord> change)
    {
      if (change != null) _changes.AddRange(change);
    }
  }
}
