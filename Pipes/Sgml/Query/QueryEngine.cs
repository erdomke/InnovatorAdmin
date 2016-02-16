using Pipes.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Query
{
  public class QueryEngine : Pipes.Sgml.Query.IQueryEngine
  {
    public IEnumerable<IQueryableNode> Parents(IQueryableNode node)
    {
      var result = node.Parent();
      while (result != null)
      {
        yield return result;
        result = result.Parent();
      }
    }

    public IEnumerable<IQueryableNode> PrecedingSiblings(IQueryableNode node)
    {
      var result = node.PrecedingSibling();
      while (result != null)
      {
        yield return result;
        result = result.PrecedingSibling();
      }
    }

    public IEnumerable<IQueryableNode> FollowingSiblings(IQueryableNode node)
    {
      var result = node.FollowingSibling();
      while (result != null)
      {
        yield return result;
        result = result.FollowingSibling();
      }
    }
  }
}
