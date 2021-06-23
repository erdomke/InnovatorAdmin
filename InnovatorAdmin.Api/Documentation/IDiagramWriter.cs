using System.IO;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public interface IDiagramWriter<T>
  {
    Task WriteAsync(EntityDiagram diagram, T target);
    Task WriteAsync(StateDiagram diagram, T target);
  }
}
