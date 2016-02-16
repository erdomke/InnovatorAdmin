using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pipes.Css;

namespace Pipes.Sgml.Selector
{
  public class TextRendererSelectorVisitor : ISelectorVisitor
  {
    private TextWriter _writer;

    public bool FriendlyFormat { get; set; }
    public int Indentation { get; set; }

    public TextRendererSelectorVisitor(TextWriter writer)
    {
      _writer = writer;
      this.Indentation = 0;
    }

    public void Visit(AggregateSelectorList selector)
    {
      bool first = true;
      foreach (var sel in selector)
      {
        if (!first) _writer.Write(selector.Delimiter);
        Indentation++;
        sel.Visit(this);
        Indentation--;
        first = false;
      }
    }

    public void Visit(AllSelector selector)
    {
      _writer.Write('*');
    }

    public void Visit(AttributeRestriction selector)
    {
      if (string.IsNullOrEmpty(selector.Name.Namespace) && 
        string.Compare(selector.Name.LocalName, "id", StringComparison.OrdinalIgnoreCase) == 0 &&
        selector.Comparison == AttributeComparison.Equals)
      {
        _writer.Write('#');
        _writer.Write(selector.Value);
      }
      else if (string.IsNullOrEmpty(selector.Name.Namespace) &&
        string.Compare(selector.Name.LocalName, "class", StringComparison.OrdinalIgnoreCase) == 0 &&
        selector.Comparison == AttributeComparison.WhitespaceListContains)
      {
        _writer.Write('.');
        _writer.Write(selector.Value);
      }
      else
      {
        _writer.Write('[');
        if (!string.IsNullOrEmpty(selector.Name.Namespace))
        {
          _writer.Write(selector.Name.Prefix ?? selector.Name.Namespace);
          _writer.Write('|');
        }
        _writer.Write(selector.Name.LocalName);

        switch (selector.Comparison)
        {
          case AttributeComparison.Contains:
            _writer.Write("*=");
            break;
          case AttributeComparison.EndsWith:
            _writer.Write("$=");
            break;
          case AttributeComparison.Equals:
            _writer.Write('=');
            break;
          case AttributeComparison.HyphenatedListStartsWith:
            _writer.Write("|=");
            break;
          case AttributeComparison.StartsWith:
            _writer.Write("^=");
            break;
          case AttributeComparison.WhitespaceListContains:
            _writer.Write("~=");
            break;
          case AttributeComparison.NotEquals:
            _writer.Write("!=");
            break;
          default: // AttributeComparison.Exists:
            _writer.Write(']');
            return;
        }
        _writer.Write('\"');
        _writer.Write(selector.Value);
        _writer.Write("\"]");
      }
    }

    public void Visit(ComplexSelector selector)
    {
      var n = selector.Length - 1;

      for (var i = 0; i < n; i++)
      {
        selector[i].Selector.Visit(this);
        _writer.Write(selector[i].Character);
      }
      selector[n].Selector.Visit(this);
    }

    public void Visit(ElementRestriction selector)
    {
      if (selector.Name.Namespace != "*")
      {
        _writer.Write(selector.Name.Prefix ?? selector.Name.Namespace ?? "");
        _writer.Write('|');
      }
      _writer.Write(selector.Name.LocalName);
    }

    public void Visit(NthChildSelector selector)
    {
      this.Visit((PseudoSelector)selector);

      _writer.Write('(');
      if (selector.Step == 2 && selector.Offset == 1)
      {
        _writer.Write(PseudoSelectorPrefix.NthChildOdd);
      }
      else if (selector.Step == 2 && selector.Offset == 0)
      {
        _writer.Write(PseudoSelectorPrefix.NthChildEven);
      }
      else
      {
        _writer.Write(selector.Step);
        _writer.Write('n');
        if (selector.Offset >= 0) _writer.Write('+');
        _writer.Write(selector.Offset);
      }
      _writer.Write(')');
    }

    public void Visit(PseudoElementSelector selector)
    {
      _writer.Write(':');
      switch (selector.Element)
      {
        case PseudoElements.After:
          _writer.Write(PseudoSelectorPrefix.PseudoElementAfter);
          break;
        case PseudoElements.Before:
          _writer.Write(PseudoSelectorPrefix.PseudoElementBefore);
          break;
        case PseudoElements.Firstletter:
          _writer.Write(PseudoSelectorPrefix.PseudoElementFirstletter);
          break;
        case PseudoElements.Firstline:
          _writer.Write(PseudoSelectorPrefix.PseudoElementFirstline);
          break;
        case PseudoElements.Selection:
          _writer.Write(PseudoSelectorPrefix.PseudoElementSelection);
          break;
        default:
          //_writer.Write(':');
          break;
      }
    }

    public void Visit(PseudoFunction selector)
    {
      this.Visit((PseudoSelector)selector);
      _writer.Write('(');
      var bodySelect = selector.Body as ISelector;
      if (bodySelect == null)
      {
        _writer.Write(selector.Body);
      }
      else
      {
        bodySelect.Visit(this);
      }
      _writer.Write(')');
    }

    public void Visit(PseudoSelector selector)
    {
      _writer.Write(':');
      switch (selector.Type)
      {
        case PseudoTypes.FunctionNthchild:       _writer.Write(PseudoSelectorPrefix.PseudoFunctionNthchild);       break;
        case PseudoTypes.FunctionNthlastchild:   _writer.Write(PseudoSelectorPrefix.PseudoFunctionNthlastchild);   break;
        case PseudoTypes.FunctionNthOfType:      _writer.Write(PseudoSelectorPrefix.PseudoFunctionNthOfType);      break;
        case PseudoTypes.FunctionNthLastOfType:  _writer.Write(PseudoSelectorPrefix.PseudoFunctionNthLastOfType);  break;
        case PseudoTypes.Root:                   _writer.Write(PseudoSelectorPrefix.PseudoRoot);                   break;
        case PseudoTypes.FirstOfType:            _writer.Write(PseudoSelectorPrefix.PseudoFirstOfType);            break;
        case PseudoTypes.Lastoftype:             _writer.Write(PseudoSelectorPrefix.PseudoLastoftype);             break;
        case PseudoTypes.Onlychild:              _writer.Write(PseudoSelectorPrefix.PseudoOnlychild);              break;
        case PseudoTypes.OnlyOfType:             _writer.Write(PseudoSelectorPrefix.PseudoOnlyOfType);             break;
        case PseudoTypes.Firstchild:             _writer.Write(PseudoSelectorPrefix.PseudoFirstchild);             break;
        case PseudoTypes.Lastchild:              _writer.Write(PseudoSelectorPrefix.PseudoLastchild);              break;
        case PseudoTypes.Empty:                  _writer.Write(PseudoSelectorPrefix.PseudoEmpty);                  break;
        case PseudoTypes.Link:                   _writer.Write(PseudoSelectorPrefix.PseudoLink);                   break;
        case PseudoTypes.Visited:                _writer.Write(PseudoSelectorPrefix.PseudoVisited);                break;
        case PseudoTypes.Active:                 _writer.Write(PseudoSelectorPrefix.PseudoActive);                 break;
        case PseudoTypes.Hover:                  _writer.Write(PseudoSelectorPrefix.PseudoHover);                  break;
        case PseudoTypes.Focus:                  _writer.Write(PseudoSelectorPrefix.PseudoFocus);                  break;
        case PseudoTypes.Target:                 _writer.Write(PseudoSelectorPrefix.PseudoTarget);                 break;
        case PseudoTypes.Enabled:                _writer.Write(PseudoSelectorPrefix.PseudoEnabled);                break;
        case PseudoTypes.Disabled:               _writer.Write(PseudoSelectorPrefix.PseudoDisabled);               break;
        case PseudoTypes.Checked:                _writer.Write(PseudoSelectorPrefix.PseudoChecked);                break;
        case PseudoTypes.Unchecked:              _writer.Write(PseudoSelectorPrefix.PseudoUnchecked);              break;
        case PseudoTypes.Indeterminate:          _writer.Write(PseudoSelectorPrefix.PseudoIndeterminate);          break;
        case PseudoTypes.Default:                _writer.Write(PseudoSelectorPrefix.PseudoDefault);                break;
        case PseudoTypes.Valid:                  _writer.Write(PseudoSelectorPrefix.PseudoValid);                  break;
        case PseudoTypes.Invalid:                _writer.Write(PseudoSelectorPrefix.PseudoInvalid);                break;
        case PseudoTypes.Required:               _writer.Write(PseudoSelectorPrefix.PseudoRequired);               break;
        case PseudoTypes.Inrange:                _writer.Write(PseudoSelectorPrefix.PseudoInrange);                break;
        case PseudoTypes.Outofrange:             _writer.Write(PseudoSelectorPrefix.PseudoOutofrange);             break;
        case PseudoTypes.Optional:               _writer.Write(PseudoSelectorPrefix.PseudoOptional);               break;
        case PseudoTypes.Readonly:               _writer.Write(PseudoSelectorPrefix.PseudoReadonly);               break;
        case PseudoTypes.Readwrite:              _writer.Write(PseudoSelectorPrefix.PseudoReadwrite);              break;
        case PseudoTypes.FunctionDir:            _writer.Write(PseudoSelectorPrefix.PseudoFunctionDir);            break;
        case PseudoTypes.FunctionNot:            _writer.Write(PseudoSelectorPrefix.PseudoFunctionNot);            break;
        case PseudoTypes.FunctionLang:           _writer.Write(PseudoSelectorPrefix.PseudoFunctionLang);           break;
        case PseudoTypes.FunctionContains:       _writer.Write(PseudoSelectorPrefix.PseudoFunctionContains);       break;
      }
    }

    public void Visit(UnknownSelector selector)
    {
      _writer.Write(selector.Token);
    }


    public void Visit(InlineSelector selector)
    {
      // Do Nothing
    }
  }
}
