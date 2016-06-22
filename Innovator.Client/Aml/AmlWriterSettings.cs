using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class AmlWriterSettings
  {
    public bool ExpandPropertyItems { get; set; }

    public AmlWriterSettings()
    {
      this.ExpandPropertyItems = true;
    }
  }
}
