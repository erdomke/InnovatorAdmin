using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Query
{
  public class HtmlQueryEngine : IQueryEngine
  {
    public IEnumerable<Xml.IQueryableNode> FollowingSiblings(Xml.IQueryableNode node)
    {
      var result = node.FollowingSibling();
      while (result != null)
      {
        yield return result;
        result = result.FollowingSibling();
      }
    }

    public IEnumerable<Xml.IQueryableNode> Parents(Xml.IQueryableNode node)
    {
      var curr = node;
      var result = node.Parent();
      while (result != null)
      {
        if (string.Compare(curr.Name.LocalName, "tr", StringComparison.OrdinalIgnoreCase) == 0 &&
          string.Compare(result.Name.LocalName, "table", StringComparison.OrdinalIgnoreCase) == 0)
        {
          yield return new TbodyNode(curr);
        }

        yield return result;
        curr = result;
        result = result.Parent();
      }

      var html = new HtmlNode();
      switch (curr.Name.LocalName.ToLowerInvariant())
      {
        case "html":
          // do nothing
          break;
        case "body":
        case "head":
          yield return html;
          break;
        case "title":
        case "base":
        case "link":
        case "style":
        case "meta":
        case "script":
        case "noscript":
        case "command":
          yield return new HeadNode(curr, null, html);
          yield return html;
          break;
        default:
          yield return new BodyNode(curr, null, html);
          yield return html;
          break;
      }
    }

    public IEnumerable<Xml.IQueryableNode> PrecedingSiblings(Xml.IQueryableNode node)
    {
      var result = node.PrecedingSibling();
      while (result != null)
      {
        yield return result;
        result = result.PrecedingSibling();
      }
    }

    private class TbodyNode : Xml.IQueryableNode
    {
      private Xml.XmlName _name = new Xml.XmlName("tbody");
      private Xml.IQueryableNode _row;

      public TbodyNode(Xml.IQueryableNode row)
      {
        _row = row;
      }

      public Xml.IXmlName Name
      {
        get { return _name; }
      }

      public bool TryGetAttributeValue(Xml.IXmlName name, StringComparison comparison, out string value)
      {
        value = null;
        return false;
      }

      public bool IsEmpty()
      {
        return false;
      }

      public Xml.IQueryableNode Parent()
      {
        return _row.Parent();
      }

      public Xml.IQueryableNode PrecedingSibling()
      {
        var value = _row.PrecedingSibling();
        if (value != null && string.Compare(value.Name.LocalName, "tr", StringComparison.OrdinalIgnoreCase) == 0) return value;
        return null;
      }

      public Xml.IQueryableNode FollowingSibling()
      {
        var value = _row.FollowingSibling();
        if (value != null && string.Compare(value.Name.LocalName, "tr", StringComparison.OrdinalIgnoreCase) == 0) return value;
        return null;
      }
    }
    private class HtmlNode : Xml.IQueryableNode
    {
      private Xml.XmlName _name = new Xml.XmlName("html");

      public Xml.IXmlName Name
      {
        get { return _name; }
      }

      public bool TryGetAttributeValue(Xml.IXmlName name, StringComparison comparison, out string value)
      {
        value = null;
        return false;
      }

      public bool IsEmpty()
      {
        return false;
      }

      public Xml.IQueryableNode Parent()
      {
        return null;
      }

      public Xml.IQueryableNode PrecedingSibling()
      {
        return null;
      }

      public Xml.IQueryableNode FollowingSibling()
      {
        return null;
      }
    }
    private class HeadNode : Xml.IQueryableNode
    {
      private Xml.XmlName _name = new Xml.XmlName("html");
      private Xml.IQueryableNode _sibling;
      private Xml.IQueryableNode _child;
      private Xml.IQueryableNode _parent;

      public HeadNode(Xml.IQueryableNode child, Xml.IQueryableNode sibling, Xml.IQueryableNode parent)
      {
        _child = child;
        _sibling = sibling;
        _parent = parent;
      }

      public Xml.IXmlName Name
      {
        get { return _name; }
      }

      public bool TryGetAttributeValue(Xml.IXmlName name, StringComparison comparison, out string value)
      {
        value = null;
        return false;
      }

      public bool IsEmpty()
      {
        return false;
      }

      public Xml.IQueryableNode Parent()
      {
        return _parent;
      }

      public Xml.IQueryableNode PrecedingSibling()
      {
        return null;
      }

      public Xml.IQueryableNode FollowingSibling()
      {
        if (_sibling != null) return _sibling;

        var result = _child.FollowingSibling();
        while (result != null)
        {
          switch (result.Name.LocalName.ToLowerInvariant())
          {
            case "body":
              return result;
            case "title":
            case "base":
            case "link":
            case "style":
            case "meta":
            case "script":
            case "noscript":
            case "command":
              // Do nothing
              break;
            default:
              return new BodyNode(null, this, _parent);
          }
          result = result.FollowingSibling();
        }

        return null;
      }
    }
    private class BodyNode : Xml.IQueryableNode
    {
      private Xml.XmlName _name = new Xml.XmlName("html");
      private Xml.IQueryableNode _sibling;
      private Xml.IQueryableNode _child;
      private Xml.IQueryableNode _parent;

      public BodyNode(Xml.IQueryableNode child, Xml.IQueryableNode sibling, Xml.IQueryableNode parent)
      {
        _child = child;
        _sibling = sibling;
        _parent = parent;
      }

      public Xml.IXmlName Name
      {
        get { return _name; }
      }

      public bool TryGetAttributeValue(Xml.IXmlName name, StringComparison comparison, out string value)
      {
        value = null;
        return false;
      }

      public bool IsEmpty()
      {
        return false;
      }

      public Xml.IQueryableNode Parent()
      {
        return _parent;
      }

      public Xml.IQueryableNode PrecedingSibling()
      {
        if (_sibling != null) return _sibling;

        var result = _child.PrecedingSibling();
        while (result != null)
        {
          switch (result.Name.LocalName.ToLowerInvariant())
          {
            case "body":
              return result;
            case "title":
            case "base":
            case "link":
            case "style":
            case "meta":
            case "script":
            case "noscript":
            case "command":
              return new HeadNode(null, this, _parent);
          }
          result = result.PrecedingSibling();
        } 
        
        return null;
      }

      public Xml.IQueryableNode FollowingSibling()
      {
        return null;
      }
    }
  }
}
