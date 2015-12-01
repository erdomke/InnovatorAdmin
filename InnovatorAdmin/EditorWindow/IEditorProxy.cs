using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IEditorProxy
  {
    IAsyncConnection Connection { get; set; }
    IEnumerable<string> GetActions();
    IPromise<System.IO.Stream> Process(Command request, bool async);
    IEditorProxy Clone();
  }
}
