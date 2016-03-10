using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IEditorScript
  {
    string Action { get; }
    string Name { get; }
    bool AutoRun { get; }
    IEnumerable<IEditorScript> Children { get; }
    OutputType PreferredOutput { get; }
    Task<string> GetScript();
  }
}
