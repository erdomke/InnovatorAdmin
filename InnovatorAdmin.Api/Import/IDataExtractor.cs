using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public interface IDataExtractor
  {
    bool AtEnd { get; }
    int GetTotalCount();
    int NumProcessed { get; }
    void Reset();
    void Write(int count, params IDataWriter[] writers);
  }
}
