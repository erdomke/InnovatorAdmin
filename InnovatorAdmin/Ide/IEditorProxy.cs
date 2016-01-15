using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;
using InnovatorAdmin.Editor;
using InnovatorAdmin.Connections;

namespace InnovatorAdmin
{
  public interface IEditorProxy : IDisposable
  {
    string Action { get; set; }
    ConnectionData ConnData { get; }
    string Name { get; }

    IEditorProxy Clone();
    IEnumerable<string> GetActions();
    IEditorHelper GetHelper();
    IEditorHelper GetOutputHelper();
    ICommand NewCommand();
    IPromise<IResultObject> Process(ICommand request, bool async, Action<int, string> progressCallback);
    IPromise<IEnumerable<IEditorTreeNode>> GetNodes();
  }
}
