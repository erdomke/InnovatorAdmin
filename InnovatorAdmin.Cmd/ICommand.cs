using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  internal interface ICommand
  {
    Task<int> Execute(ILogger logger);
  }
}
