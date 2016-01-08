using System;
using System.Collections.Generic;

namespace InnovatorAdmin
{
  public interface IEditorScript
  {
    string Action { get; }
    string Name { get; }
    string Script { get; }
    bool AutoRun { get; }
    IEnumerable<IEditorScript> Children { get; }
    OutputType PreferredOutput { get; }
  }
}
