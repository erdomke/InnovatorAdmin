using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public interface IProcessor
  {
    void Execute();
  }
  public interface IProcessor<Tin> : IProcessor
  {
    void InitializeData(Tin data);
  }
}
