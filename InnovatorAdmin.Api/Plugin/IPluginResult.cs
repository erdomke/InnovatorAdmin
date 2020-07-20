using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugin
{
  public interface IPluginResult
  {
    int Count { get; }
    void Write(TextWriter writer);
  }
}
