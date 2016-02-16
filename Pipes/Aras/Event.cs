using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class Event : IEvent
  {
    public string Name { get; set; }
    public string Value { get; set; }

    public Event() { }
    public Event(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }
  }
}
