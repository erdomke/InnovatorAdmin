using Pipes.Sgml.Selector;
using Pipes.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Query
{
  public class MatchVisitor : ISelectorVisitor
  {
    private IEnumerable<IQueryableNode> _matches;
    private IQueryEngine _engine;

    public StringComparison Comparison { get; set; }
    public bool ExceptionOnUnsupported { get; set; }

    public int MatchSpecificity { get; set; }

    public MatchVisitor(IQueryEngine engine)
    {
      _engine = engine;
    }

    public bool IsMatch()
    {
      return _matches.Any();
    }

    public void Initialize(IQueryableNode node)
    {
      _matches = Enumerable.Repeat(node, 1);
      this.MatchSpecificity = 0;
    }

    public void Visit(AllSelector selector)
    {
      // Do nothing
    }

    public void Visit(AggregateSelectorList selector)
    {
      if (selector.Delimiter == ",")
      {
        var matches = _matches.ToList();
        foreach (var sel in selector.Select(s => new { Selector = s, Specificity = s.GetSpecificity()}).OrderByDescending(s => s.Specificity))
        {
          _matches = matches;
          sel.Selector.Visit(this);
          if (_matches.Any()) 
          {
            this.MatchSpecificity = sel.Specificity;
            return;
          }
        }
      }
      else
      {
        foreach (var sel in selector)
        {
          sel.Visit(this);
          if (!_matches.Any()) return;
        }
      }
    }

    public void Visit(AttributeRestriction selector)
    {
      _matches = _matches.Where(n => IsMatch(n, selector));
    }

    private bool IsMatch(IQueryableNode node, AttributeRestriction elem)
    {
      string value;
      if (node.TryGetAttributeValue(elem.Name, this.Comparison, out value))
      {
        switch (elem.Comparison)
        {
          case Selector.AttributeComparison.Contains:
            return !string.IsNullOrEmpty(elem.Value) && value.IndexOf(elem.Value, this.Comparison) >= 0;
          case Selector.AttributeComparison.EndsWith:
            return !string.IsNullOrEmpty(elem.Value) && value.EndsWith(elem.Value, this.Comparison);
          case Selector.AttributeComparison.Equals:
            return string.Compare(value, elem.Value ?? "", this.Comparison) == 0;
          case Selector.AttributeComparison.Exists:
            return true;
          case Selector.AttributeComparison.HyphenatedListStartsWith:
            return string.IsNullOrEmpty(elem.Value) || string.Compare(value, elem.Value, this.Comparison) == 0 || value.StartsWith(elem.Value + '-', this.Comparison);
          case Selector.AttributeComparison.NotEquals:
            return string.Compare(value, elem.Value ?? "", this.Comparison) != 0;
          case Selector.AttributeComparison.StartsWith:
            return !string.IsNullOrEmpty(elem.Value) && value.StartsWith(elem.Value, this.Comparison);
          case Selector.AttributeComparison.WhitespaceListContains:
            return !string.IsNullOrEmpty(elem.Value) && value.Split(' ').Contains(elem.Value, GetComparer(this.Comparison));
        }
      }
      return false;
    }

    public void Visit(ComplexSelector selector)
    {
      bool first = true;
      foreach (var sel in selector.Reverse())
      {
        if (!first)
        {
          switch (sel.Delimiter)
          {
            case Combinator.AdjacentSibling:
              _matches = _matches.SelectMany(n => _engine.PrecedingSiblings(n).Take(1));
              break;
            case Combinator.Child:
              _matches = _matches.SelectMany(n => _engine.Parents(n).Take(1));
              break;
            case Combinator.Descendent:
              _matches = _matches.SelectMany(n => _engine.Parents(n));
              break;
            case Combinator.Sibling:
              _matches = _matches.SelectMany(n => _engine.PrecedingSiblings(n));
              break;
            default:
              NotSupported();
              break;
          }
        }
        sel.Selector.Visit(this);
        if (!_matches.Any()) return;
        first = false;
      }
    }

    private bool IsMatch(IXmlName x, IXmlName y)
    {
      return string.Compare(x.LocalName, y.LocalName, this.Comparison) == 0 &&
             ((EmptyNs(x.Namespace) && EmptyNs(y.Namespace)) ||
             string.Compare(x.Namespace, y.Namespace, this.Comparison) == 0);
    }

    private bool EmptyNs(string ns)
    {
      return ns == null || ns == string.Empty || ns == "*";
    }

    public void Visit(ElementRestriction selector)
    {
      _matches = _matches.Where(n => IsMatch(n.Name, selector.Name));
    }

    public void Visit(NthChildSelector selector)
    {
      switch (selector.Type)
      {
        case PseudoTypes.FunctionNthchild:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && CountMatches(selector, _engine.PrecedingSiblings(n).Count() + 1));
          break;
        case PseudoTypes.FunctionNthlastchild:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && CountMatches(selector, _engine.FollowingSiblings(n).Count() + 1));
          break;
        case PseudoTypes.FunctionNthLastOfType:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && CountMatches(selector, _engine.FollowingSiblings(n).Count(m => IsMatch(n.Name, m.Name)) + 1));
          break;
        case PseudoTypes.FunctionNthOfType:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && CountMatches(selector, _engine.PrecedingSiblings(n).Count(m => IsMatch(n.Name, m.Name)) + 1));
          break;
        default:
          NotSupported();
          break;
      }
    }

    private bool CountMatches(NthChildSelector selector, int count)
    {
      var withOffset = count - selector.Offset;
      return (selector.Step == 0 ? withOffset == 0 : withOffset % selector.Step == 0);
    }

    public void Visit(PseudoElementSelector selector)
    {
      NotSupported();
    }

    public void Visit(PseudoFunction selector)
    {
      switch (selector.Type)
      {
        case PseudoTypes.FunctionNot:
          var visitor = new MatchVisitor(new HtmlQueryEngine());
          var sel = selector.Body as ISelector;
          _matches = _matches.Where(n =>
          {
            visitor.Initialize(n);
            sel.Visit(visitor);
            return !visitor.IsMatch();
          });
          break;
        case PseudoTypes.FunctionContains:
        case PseudoTypes.FunctionDir:
        default:
          NotSupported();
          break;
      }
    }

    public void Visit(PseudoSelector selector)
    {
      switch (selector.Type)
      {
        case PseudoTypes.Active:
        case PseudoTypes.Checked:
        case PseudoTypes.Default:
        case PseudoTypes.Disabled:
        case PseudoTypes.Enabled:
        case PseudoTypes.Focus:
        case PseudoTypes.Hover:
        case PseudoTypes.Indeterminate:
        case PseudoTypes.Inrange:
        case PseudoTypes.Invalid:
        case PseudoTypes.Link:
        case PseudoTypes.Optional:
        case PseudoTypes.Outofrange:
        case PseudoTypes.Readonly:
        case PseudoTypes.Readwrite:
        case PseudoTypes.Required:
        case PseudoTypes.Target:
        case PseudoTypes.Unchecked:
        case PseudoTypes.Valid:
        case PseudoTypes.Visited:
          NotSupported();
          break;
        case PseudoTypes.Empty:
          _matches = _matches.Where(n => n.IsEmpty());
          break;
        case PseudoTypes.Firstchild:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.PrecedingSiblings(n).Any());
          break;
        case PseudoTypes.FirstOfType:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.PrecedingSiblings(n).Any(m => IsMatch(n.Name, m.Name)));
          break;
        case PseudoTypes.Lastchild:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.FollowingSiblings(n).Any());
          break;
        case PseudoTypes.Lastoftype:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.FollowingSiblings(n).Any(m => IsMatch(n.Name, m.Name)));
          break;
        case PseudoTypes.Onlychild:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.PrecedingSiblings(n).Any()
                                      && !_engine.FollowingSiblings(n).Any());
          break;
        case PseudoTypes.OnlyOfType:
          _matches = _matches.Where(n => _engine.Parents(n).Any()
                                      && !_engine.PrecedingSiblings(n).Any(m => IsMatch(n.Name, m.Name))
                                      && !_engine.FollowingSiblings(n).Any(m => IsMatch(n.Name, m.Name)));
          break;
        case PseudoTypes.Root:
          _matches = _matches.Where(n => !_engine.Parents(n).Any());
          break;
        default:
          NotSupported();
          break;
      }
    }

    public void Visit(UnknownSelector selector)
    {
      NotSupported();
    }

    
    private IEqualityComparer<string> GetComparer(StringComparison comparison)
    {
      switch (comparison)
      {
        case StringComparison.CurrentCultureIgnoreCase:
          return StringComparer.CurrentCultureIgnoreCase;
        case StringComparison.InvariantCulture:
          return StringComparer.InvariantCulture;
        case StringComparison.InvariantCultureIgnoreCase:
          return StringComparer.InvariantCultureIgnoreCase;
        case StringComparison.Ordinal:
          return StringComparer.Ordinal;
        case StringComparison.OrdinalIgnoreCase:
          return StringComparer.OrdinalIgnoreCase;
        default:
          return StringComparer.CurrentCulture;
      }
    }


    public void Visit(InlineSelector selector)
    {
      // Do nothing
    }

    private void NotSupported()
    {
      if (ExceptionOnUnsupported) throw new NotSupportedException();
      _matches = Enumerable.Empty<IQueryableNode>();
    }
  }
}
