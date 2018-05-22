using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugin
{
  public interface IPluginMethod
  {
    Task<IPluginResult> Execute(IPluginContext arg);
  }
}
