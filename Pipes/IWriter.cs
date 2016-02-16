using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public interface IWriter
  {
    object Parent { get; set; }
  }
  public interface IWriter<TCore> : IWriter
  {
    T Initialize<T>(T coreWriter) where T : TCore;
  }
}
