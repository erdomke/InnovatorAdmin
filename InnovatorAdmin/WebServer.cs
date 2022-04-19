using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  internal class WebServer : IDisposable
  {
    private readonly HttpListener _listener = new HttpListener();

    public WebServer(string[] prefixes)
    {
      if (!HttpListener.IsSupported)
        throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

      // URI prefixes are required, for example 
      // "http://localhost:8080/index/".
      if (prefixes == null || prefixes.Length == 0)
        throw new ArgumentNullException(nameof(prefixes));

      foreach (string s in prefixes)
        _listener.Prefixes.Add(s);

      _listener.Start();
    }

    public void Dispose()
    {
      Stop();
    }

    public void Run()
    {
      ThreadPool.QueueUserWorkItem((o) =>
      {
        try
        {
          while (_listener.IsListening)
          {
            ThreadPool.QueueUserWorkItem(async (c) =>
            {
              var ctx = c as HttpListenerContext;
              try
              {
                await GetResponse(ctx.Request, ctx.Response).ConfigureAwait(false);
              }
              catch { } // suppress any exceptions
              finally
              {
                if (_listener.IsListening)
                  ctx.Response.OutputStream.Close();
              }
            }, _listener.GetContext());
          }
        }
        catch { } // suppress any exceptions
      });
    }

    private async Task GetResponse(HttpListenerRequest request, HttpListenerResponse response)
    {
      var parts = request.Url.LocalPath.TrimStart('/').Split('/');
      var window = Application.OpenForms.OfType<EditorWindow>().FirstOrDefault(e => e.Uid == parts[0]);
      if (window == null)
        return;
      await window.GetResponse(request, response).ConfigureAwait(false);
    }

    public void Stop()
    {
      _listener.Stop();
      _listener.Close();
    }
  }
}
