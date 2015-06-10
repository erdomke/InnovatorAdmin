using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ADiff
{
  public enum Change
  {
    Add,
    Change,
    Delete,
    NoChange
  }
  public class AmlChange
  {
    public Change ChangeType { get; set; }
    public XElement Element { get; set; }
  }
}
