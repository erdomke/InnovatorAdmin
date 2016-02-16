using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public interface IConfigurable<T>
  {
    void InitializeSettings(T settings);
  }
}
