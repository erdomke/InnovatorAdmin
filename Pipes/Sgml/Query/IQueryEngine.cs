using System;
using System.Collections.Generic;
using Pipes.Xml;

namespace Pipes.Sgml.Query
{
  public interface IQueryEngine
  {
    IEnumerable<IQueryableNode> FollowingSiblings(IQueryableNode node);
    IEnumerable<IQueryableNode> Parents(IQueryableNode node);
    IEnumerable<IQueryableNode> PrecedingSiblings(IQueryableNode node);
  }
}
