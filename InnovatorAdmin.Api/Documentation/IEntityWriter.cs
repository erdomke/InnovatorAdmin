using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public interface IEntityWriter
  {
    void Write(EntityDiagram diagram, TextWriter writer);
  }
}
