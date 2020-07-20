using Innovator.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugin
{
  internal class MethodRunner : IPluginMethod
  {
    private readonly IMethod _method;

    public MethodRunner(IMethod method)
    {
      _method = method;
    }

    public Task<IPluginResult> Execute(IPluginContext arg)
    {
      var result = _method.Execute(arg);
      return Task.FromResult((IPluginResult)new PluginResult(result));
    }
  }
}
