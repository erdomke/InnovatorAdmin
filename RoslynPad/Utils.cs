using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynPad
{
  internal static class Utils
  {
    internal static string GetConnectionFilePath()
    {
      string path = @"{0}\{1}\connections.xml";
      return string.Format(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin");
    }
  }
}
