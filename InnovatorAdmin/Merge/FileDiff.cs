using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class FileDiff
  {
    public string Path { get; set; }
    public FileStatus InBase { get; set; }
    public FileStatus InCompare { get; set; }
  }
}
