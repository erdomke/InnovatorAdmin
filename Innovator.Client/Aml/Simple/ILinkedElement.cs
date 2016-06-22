using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Aml.Simple
{
  interface ILinkedElement : IElement, ILink<ILinkedElement>
  {
  }
}
