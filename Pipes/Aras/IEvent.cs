using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public interface IEvent : IItem
  {
    string Name { get; }
    string Value { get; }
  }
}
