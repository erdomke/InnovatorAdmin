using System.IO;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public interface IDiagram
  {
    Task WriteAsync<T>(IDiagramWriter<T> writer, T target);
  }
}
