using System;
namespace InnovatorAdmin
{
  public interface IEditorScript
  {
    string Action { get; }
    string Name { get; }
    string Script { get; }
    bool AutoRun { get; }
  }
}
