using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  public interface IRelationship<TRelated> : IReadOnlyItem { }
  public interface INullRelationship<TSource> : IReadOnlyItem { }
}
