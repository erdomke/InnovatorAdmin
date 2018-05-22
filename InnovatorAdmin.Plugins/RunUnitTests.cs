using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using InnovatorAdmin.Plugin;
using InnovatorAdmin.Testing;

namespace InnovatorAdmin
{
  public class RunUnitTests : IPluginMethod
  {
    public async Task<IPluginResult> Execute(IPluginContext arg)
    {
      TestSuite suite;
      using (var reader = new StringReader(arg.Conn.OriginalRequest))
      {
        suite = TestSerializer.ReadTestSuite(reader);
      }
      var context = new TestContext((IAsyncConnection)arg.Conn);
      foreach (var cred in arg.Credentials)
      {
        context.CredentialStore.Add(cred);
      }
      context.ProgressCallback = arg.RecordProgress;
      await suite.Run(context).ConfigureAwait(false);

      return new PluginResult(suite.Write, suite.Tests.Count);
    }
  }
}
