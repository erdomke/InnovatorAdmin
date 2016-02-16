using Pipes.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pipes.Xml
{
  public class XmlReaderHelper : IPipeInput<IXmlNode>
  {
    private List<Handler> _handlers = new List<Handler>();
    private List<IXmlNode> _parents = new List<IXmlNode>();
    private BufferedEnumerator<IXmlNode> _source;
    private bool _trackParents;

    public void Initialize(IEnumerable<IXmlNode> source)
    {
      _source = new BufferedEnumerator<IXmlNode>(source);
    }

    public void ElementHandler(Func<IXmlNode, bool> predicate, Action<IXmlElement> action)
    {
      _handlers.Add(new Handler() { Predicate = predicate, ElementAction = action });
    }
    public void ElementHandler(Func<IXmlNode, IEnumerable<IXmlNode>, bool> predicate, Action<IXmlElement> action)
    {
      _handlers.Add(new Handler() { ParentPredicate = predicate, ElementAction = action });
      _trackParents = true;
    }
    public void NodeHandler(Func<IXmlNode, bool> predicate, Action<IXmlNode> action)
    {
      _handlers.Add(new Handler() { Predicate = predicate, NodeAction = action });
    }
    public void NodeHandler(Func<IXmlNode, IEnumerable<IXmlNode>, bool> predicate, Action<IXmlNode> action)
    {
      _handlers.Add(new Handler() { ParentPredicate = predicate, NodeAction = action });
      _trackParents = true;
    }

    public void Process()
    {
      while (_source.MoveNext())
      {
        foreach (var handler in _handlers)
        {
          if (    (handler.ElementAction != null && (_source.Current.Type == XmlNodeType.Element 
                                                  || _source.Current.Type == XmlNodeType.EmptyElement) 
                || handler.NodeAction != null) 
             &&
                (  (handler.Predicate != null && handler.Predicate(_source.Current)) 
                || (handler.ParentPredicate != null && handler.ParentPredicate(_source.Current, _parents))))
          {
            if (handler.NodeAction != null) handler.NodeAction.Invoke(_source.Current);
            if (handler.ElementAction != null) handler.ElementAction.Invoke(GetElement());
          }
        }

        if (_trackParents)
        {
          if (_source.Current.Type == XmlNodeType.Element)
          {
            _parents.Add(_source.Current);
          }
          else if (_source.Current.Type == XmlNodeType.EndElement)
          {
            _parents.RemoveAt(_parents.Count - 1);
          }
        }
      }
    }

    private IXmlElement GetElement()
    {
      var root = new XElement("ROOT");
      var writer = new XElementWriter(root);
      var level = 0;
      writer.Node(_source.Current);
      level += (_source.Current.Type == XmlNodeType.Element ? 1 : 0);
      while (level > 0 && _source.PeekNext())
      {
        writer.Node(_source.CurrentPeek);
        level += (_source.CurrentPeek.Type == XmlNodeType.Element ? 1 : (_source.CurrentPeek.Type == XmlNodeType.EndElement ? -1 : 0));
      }
      return new XElementWrapper(root.Elements().First());
    }

    private class Handler
    {
      public Action<IXmlElement> ElementAction { get; set; }
      public Action<IXmlNode> NodeAction { get; set; }
      public Func<IXmlNode, bool> Predicate { get; set; }
      public Func<IXmlNode, IEnumerable<IXmlNode>, bool> ParentPredicate { get; set; }
    }

  }
}
