using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  interface ISyncContent
  {
    void SerializeToStream(Stream stream);
  }
}
