using System;
using System.Globalization;
using Pipes.Css.Model;
using Pipes.Css.Model.TextBlocks;
using Pipes.Sgml.Selector;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
  internal sealed class SelectorFactory
  {
    private SelectorOperation _selectorOperation;
    private BaseSelector _currentSelector;
    private AggregateSelectorList _aggregateSelectorList;
    private ComplexSelector _complexSelector;
    private bool _hasCombinator;
    private Combinator _combinator;
    private SelectorFactory _nestedSelectorFactory;
    private string _attributeName;
    private string _attributeValue;
    private string _attributeOperator;

    internal SelectorFactory()
    {
      ResetFactory();
    }

    internal ISelector GetSelector()
    {
      if (_complexSelector != null)
      {
        _complexSelector.ConcludeSelector(_currentSelector);
        _currentSelector = _complexSelector;
      }

      if (_aggregateSelectorList == null || _aggregateSelectorList.Length == 0)
      {
        return _currentSelector ?? AllSelector.All();
      }

      if (_currentSelector == null && _aggregateSelectorList.Length == 1)
      {
        return _aggregateSelectorList[0];
      }

      if (_currentSelector == null)
      {
        return _aggregateSelectorList;
      }

      _aggregateSelectorList.AppendSelector(_currentSelector);
      _currentSelector = null;

      return _aggregateSelectorList;
    }

    internal void Apply(Block token)
    {
      switch (_selectorOperation)
      {
        case SelectorOperation.Data:
          ParseSymbol(token);
          break;

        case SelectorOperation.Class:
          ParseClass(token);
          break;

        case SelectorOperation.Attribute:
          ParseAttribute(token);
          break;

        case SelectorOperation.AttributeOperator:
          ParseAttributeOperator(token);
          break;

        case SelectorOperation.AttributeValue:
          ParseAttributeValue(token);
          break;

        case SelectorOperation.AttributeEnd:
          ParseAttributeEnd(token);
          break;

        case SelectorOperation.PseudoClass:
          ParsePseudoClass(token);
          break;

        case SelectorOperation.PseudoClassFunction:
          ParsePseudoClassFunction(token);
          break;

        case SelectorOperation.PseudoClassFunctionEnd:
          PrasePseudoClassFunctionEnd(token);
          break;

        case SelectorOperation.PseudoElement:
          ParsePseudoElement(token);
          break;
      }
    }

    internal SelectorFactory ResetFactory()
    {
      _attributeName = null;
      _attributeValue = null;
      _attributeOperator = string.Empty;
      _selectorOperation = SelectorOperation.Data;
      _combinator = Combinator.Descendent;
      _hasCombinator = false;
      _currentSelector = null;
      _aggregateSelectorList = null;
      _complexSelector = null;

      return this;
    }

    private void ParseSymbol(Block token)
    {
      switch (token.GrammarSegment)
      {
        // Attribute [A]
        case GrammarSegment.SquareBraceOpen:
          _attributeName = null;
          _attributeValue = null;
          _attributeOperator = string.Empty;
          _selectorOperation = SelectorOperation.Attribute;
          return;

        // Pseudo :P
        case GrammarSegment.Colon:
          _selectorOperation = SelectorOperation.PseudoClass;
          return;

        // ID #I
        case GrammarSegment.Hash:
          Insert(AttributeRestriction.Id(((SymbolBlock)token).Value));
          return;

        // Type E
        case GrammarSegment.Ident:
          Insert(ElementRestriction.LocalName(((SymbolBlock)token).Value));
          return;

        // Whitespace
        case GrammarSegment.Whitespace:
          Insert(Combinator.Descendent);
          return;

        case GrammarSegment.Delimiter:
          ParseDelimiter(token);
          return;

        case GrammarSegment.Comma:
          InsertCommaDelimited();
          return;
      }
    }

    private void ParseAttribute(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return;
      }

      _selectorOperation = SelectorOperation.AttributeOperator;

      switch (token.GrammarSegment)
      {
        case GrammarSegment.Ident:
          _attributeName = ((SymbolBlock)token).Value;
          break;

        case GrammarSegment.String:
          _attributeName = ((StringBlock)token).Value;
          break;

        default:
          _selectorOperation = SelectorOperation.Data;
          break;
      }
    }

    private void ParseAttributeOperator(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return;
      }

      _selectorOperation = SelectorOperation.AttributeValue;

      if (token.GrammarSegment == GrammarSegment.SquareBracketClose)
      {
        ParseAttributeEnd(token);
      }
      else if (token is MatchBlock || token.GrammarSegment == GrammarSegment.Delimiter)
      {
        _attributeOperator = token.ToString();
      }
      else
      {
        _selectorOperation = SelectorOperation.AttributeEnd;
      }
    }

    private void ParseAttributeValue(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return;
      }

      _selectorOperation = SelectorOperation.AttributeEnd;

      switch (token.GrammarSegment)
      {
        case GrammarSegment.Ident:
          _attributeValue = ((SymbolBlock)token).Value;
          break;

        case GrammarSegment.String:
          _attributeValue = ((StringBlock)token).Value;
          break;

        case GrammarSegment.Number:
          _attributeValue = ((NumericBlock)token).Value.ToString(CultureInfo.InvariantCulture);
          break;

        default:
          _selectorOperation = SelectorOperation.Data;
          break;
      }
    }

    private void ParseAttributeEnd(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return;
      }

      _selectorOperation = SelectorOperation.Data;

      if (token.GrammarSegment != GrammarSegment.SquareBracketClose)
      {
        return;
      }

      switch (_attributeOperator)
      {
        case "=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.Equals, Value = _attributeValue});
          break;

        case "~=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.WhitespaceListContains, Value = _attributeValue });
          break;

        case "|=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.HyphenatedListStartsWith, Value = _attributeValue });
          break;

        case "^=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.StartsWith, Value = _attributeValue });
          break;

        case "$=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.EndsWith, Value = _attributeValue });
          break;

        case "*=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.Contains, Value = _attributeValue });
          break;

        case "!=":
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.NotEquals, Value = _attributeValue });
          break;

        default:
          Insert(new AttributeRestriction(_attributeName) { Comparison = AttributeComparison.Exists });
          break;
      }
    }

    private void ParsePseudoClass(Block token)
    {
      _selectorOperation = SelectorOperation.Data;

      switch (token.GrammarSegment)
      {
        case GrammarSegment.Colon:
          _selectorOperation = SelectorOperation.PseudoElement;
          break;

        case GrammarSegment.Function:
          _attributeName = ((SymbolBlock)token).Value;
          _attributeValue = string.Empty;
          _selectorOperation = SelectorOperation.PseudoClassFunction;

          if (_nestedSelectorFactory != null)
          {
            _nestedSelectorFactory.ResetFactory();
          }

          break;

        case GrammarSegment.Ident:
          var pseudoSelector = GetPseudoSelector(token);

          if (pseudoSelector != null)
          {
            Insert(pseudoSelector);
          }
          break;
      }
    }

    private void ParsePseudoElement(Block token)
    {
      if (token.GrammarSegment != GrammarSegment.Ident)
      {
        return;
      }
      var data = ((SymbolBlock)token).Value;

      switch (data)
      {
        case PseudoSelectorPrefix.PseudoElementBefore:
          Insert(new PseudoElementSelector(PseudoElements.Before));
          break;

        case PseudoSelectorPrefix.PseudoElementAfter:
          Insert(new PseudoElementSelector(PseudoElements.After));
          break;

        case PseudoSelectorPrefix.PseudoElementSelection:
          Insert(new PseudoElementSelector(PseudoElements.Selection));
          break;

        case PseudoSelectorPrefix.PseudoElementFirstline:
          Insert(new PseudoElementSelector(PseudoElements.Firstline));
          break;

        case PseudoSelectorPrefix.PseudoElementFirstletter:
          Insert(new PseudoElementSelector(PseudoElements.Firstletter));
          break;

        default:
          Insert(new UnknownSelector(":" + data));
          break;
      }
    }

    private void ParseClass(Block token)
    {
      _selectorOperation = SelectorOperation.Data;

      if (token.GrammarSegment == GrammarSegment.Ident)
      {
        Insert(AttributeRestriction.Class(((SymbolBlock)token).Value));
      }
    }

    private void ParsePseudoClassFunction(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return;
      }

      switch (_attributeName)
      {
        case PseudoSelectorPrefix.PseudoFunctionNthchild:
        case PseudoSelectorPrefix.PseudoFunctionNthlastchild:
        case PseudoSelectorPrefix.PseudoFunctionNthOfType:
        case PseudoSelectorPrefix.PseudoFunctionNthLastOfType:
          {
            switch (token.GrammarSegment)
            {
              case GrammarSegment.Ident:
              case GrammarSegment.Number:
              case GrammarSegment.Dimension:
                _attributeValue += token.ToString();
                return;

              case GrammarSegment.Delimiter:
                var chr = ((DelimiterBlock)token).Value;

                if (chr == Specification.PlusSign || chr == Specification.MinusSign)
                {
                  _attributeValue += chr;
                  return;
                }

                break;
            }

            break;
          }
        case PseudoSelectorPrefix.PseudoFunctionNot:
          {
            if (_nestedSelectorFactory == null)
            {
              _nestedSelectorFactory = new SelectorFactory();
            }

            if (token.GrammarSegment != GrammarSegment.ParenClose || _nestedSelectorFactory._selectorOperation != SelectorOperation.Data)
            {
              _nestedSelectorFactory.Apply(token);
              return;
            }

            break;
          }
        case PseudoSelectorPrefix.PseudoFunctionDir:
          {
            if (token.GrammarSegment == GrammarSegment.Ident)
            {
              _attributeValue = ((SymbolBlock)token).Value;
            }

            _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
            return;
          }
        case PseudoSelectorPrefix.PseudoFunctionLang:
          {
            if (token.GrammarSegment == GrammarSegment.Ident)
            {
              _attributeValue = ((SymbolBlock)token).Value;
            }

            _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
            return;
          }
        case PseudoSelectorPrefix.PseudoFunctionContains:
          {
            switch (token.GrammarSegment)
            {
              case GrammarSegment.String:
                _attributeValue = ((StringBlock)token).Value;
                break;

              case GrammarSegment.Ident:
                _attributeValue = ((SymbolBlock)token).Value;
                break;
            }

            _selectorOperation = SelectorOperation.PseudoClassFunctionEnd;
            return;
          }
      }

      PrasePseudoClassFunctionEnd(token);
    }

    private void PrasePseudoClassFunctionEnd(Block token)
    {
      _selectorOperation = SelectorOperation.Data;

      if (token.GrammarSegment != GrammarSegment.ParenClose)
      {
        return;
      }

      switch (_attributeName)
      {
        case PseudoSelectorPrefix.PseudoFunctionNthchild:
          Insert(GetChildSelector(PseudoTypes.FunctionNthchild));
          break;

        case PseudoSelectorPrefix.PseudoFunctionNthlastchild:
          Insert(GetChildSelector(PseudoTypes.FunctionNthlastchild));
          break;

        case PseudoSelectorPrefix.PseudoFunctionNthOfType:
          Insert(GetChildSelector(PseudoTypes.FunctionNthOfType));
          break;

        case PseudoSelectorPrefix.PseudoFunctionNthLastOfType:
          Insert(GetChildSelector(PseudoTypes.FunctionNthLastOfType));
          break;

        case PseudoSelectorPrefix.PseudoFunctionNot:
          {
            var selector = _nestedSelectorFactory.GetSelector();
            Insert(new PseudoFunction(PseudoTypes.FunctionNot) { Body = selector });
            break;
          }
        case PseudoSelectorPrefix.PseudoFunctionDir:
          {
            Insert(new PseudoFunction(PseudoTypes.FunctionDir) { Body = _attributeValue });
            break;
          }
        case PseudoSelectorPrefix.PseudoFunctionLang:
          {
            Insert(new PseudoFunction(PseudoTypes.FunctionLang) { Body = _attributeValue });
            break;
          }
        case PseudoSelectorPrefix.PseudoFunctionContains:
          {
            Insert(new PseudoFunction(PseudoTypes.FunctionContains) { Body = _attributeValue });
            break;
          }
      }
    }

    private void InsertCommaDelimited()
    {
      if (_currentSelector == null)
      {
        return;
      }

      if (_aggregateSelectorList == null)
      {
        _aggregateSelectorList = new AggregateSelectorList(",");
      }

      if (_complexSelector != null)
      {
        _complexSelector.ConcludeSelector(_currentSelector);
        _aggregateSelectorList.AppendSelector(_complexSelector);
        _complexSelector = null;
      }
      else
      {
        _aggregateSelectorList.AppendSelector(_currentSelector);
      }

      _currentSelector = null;
    }

    private void Insert(BaseSelector selector)
    {
      if (_currentSelector != null)
      {
        if (!_hasCombinator)
        {
          var compound = _currentSelector as AggregateSelectorList;

          if (compound == null)
          {
            compound = new AggregateSelectorList("");
            compound.AppendSelector(_currentSelector);
          }

          compound.AppendSelector(selector);
          _currentSelector = compound;
        }
        else
        {
          if (_complexSelector == null)
          {
            _complexSelector = new ComplexSelector();
          }

          _complexSelector.AppendSelector(_currentSelector, _combinator);
          _combinator = Combinator.Descendent;
          _hasCombinator = false;
          _currentSelector = selector;
        }
      }
      else
      {
        if (_currentSelector == null && _complexSelector == null && _combinator == Combinator.Namespace)
        {
          _complexSelector = new ComplexSelector();
          _complexSelector.AppendSelector(new UnknownSelector(""), _combinator);
          _currentSelector = selector;
        }
        else
        {
          _combinator = Combinator.Descendent;
          _hasCombinator = false;
          _currentSelector = selector;
        }
      }
    }

    private void Insert(Combinator combinator)
    {
      _hasCombinator = true;

      if (combinator != Combinator.Descendent)
      {
        _combinator = combinator;
      }
    }

    private void ParseDelimiter(Block token)
    {
      switch (((DelimiterBlock)token).Value)
      {
        case Specification.Comma:
          InsertCommaDelimited();
          return;

        case Specification.GreaterThan:
          Insert(Combinator.Child);
          return;

        case Specification.PlusSign:
          Insert(Combinator.AdjacentSibling);
          return;

        case Specification.Tilde:
          Insert(Combinator.Sibling);
          return;

        case Specification.Asterisk:
          Insert(AllSelector.All());
          return;

        case Specification.Period:
          _selectorOperation = SelectorOperation.Class;
          return;

        case Specification.Pipe:
          Insert(Combinator.Namespace);
          return;
      }
    }

    private NthChildSelector GetChildSelector(PseudoTypes type)
    {
      var selector = new NthChildSelector(type);

      if (_attributeValue.Equals(PseudoSelectorPrefix.NthChildOdd, StringComparison.OrdinalIgnoreCase))
      {
        selector.Step = 2;
        selector.Offset = 1;
      }
      else if (_attributeValue.Equals(PseudoSelectorPrefix.NthChildEven, StringComparison.OrdinalIgnoreCase))
      {
        selector.Step = 2;
        selector.Offset = 0;
      }
      else if (!int.TryParse(_attributeValue, out selector.Offset))
      {
        var index = _attributeValue.IndexOf(PseudoSelectorPrefix.NthChildN, StringComparison.OrdinalIgnoreCase);

        if (_attributeValue.Length <= 0 || index == -1)
        {
          return selector;
        }

        var first = _attributeValue.Substring(0, index).Replace(" ", "");

        var second = "";

        if (_attributeValue.Length > index + 1)
        {
          second = _attributeValue.Substring(index + 1).Replace(" ", "");
        }

        if (first == string.Empty || (first.Length == 1 && first[0] == Specification.PlusSign))
        {
          selector.Step = 1;
        }
        else if (first.Length == 1 && first[0] == Specification.MinusSign)
        {
          selector.Step = -1;
        }
        else
        {
          int step;
          if (int.TryParse(first, out step))
          {
            selector.Step = step;
          }
        }

        if (second == string.Empty)
        {
          selector.Offset = 0;
        }
        else
        {
          int offset;
          if (int.TryParse(second, out offset))
          {
            selector.Offset = offset;
          }
        }
      }

      return selector;
    }

    private static BaseSelector GetPseudoSelector(Block token)
    {
      switch (((SymbolBlock)token).Value)
      {
        case PseudoSelectorPrefix.PseudoRoot:
          return new PseudoSelector(PseudoTypes.Root);

        case PseudoSelectorPrefix.PseudoFirstOfType:
          return new PseudoSelector(PseudoTypes.FirstOfType);

        case PseudoSelectorPrefix.PseudoLastoftype:
          return new PseudoSelector(PseudoTypes.Lastoftype);

        case PseudoSelectorPrefix.PseudoOnlychild:
          return new PseudoSelector(PseudoTypes.Onlychild);

        case PseudoSelectorPrefix.PseudoOnlyOfType:
          return new PseudoSelector(PseudoTypes.OnlyOfType);

        case PseudoSelectorPrefix.PseudoFirstchild:
          return new PseudoSelector(PseudoTypes.Firstchild);

        case PseudoSelectorPrefix.PseudoLastchild:
          return new PseudoSelector(PseudoTypes.Lastchild);

        case PseudoSelectorPrefix.PseudoEmpty:
          return new PseudoSelector(PseudoTypes.Empty);

        case PseudoSelectorPrefix.PseudoLink:
          return new PseudoSelector(PseudoTypes.Link);

        case PseudoSelectorPrefix.PseudoVisited:
          return new PseudoSelector(PseudoTypes.Visited);

        case PseudoSelectorPrefix.PseudoActive:
          return new PseudoSelector(PseudoTypes.Active);

        case PseudoSelectorPrefix.PseudoHover:
          return new PseudoSelector(PseudoTypes.Hover);

        case PseudoSelectorPrefix.PseudoFocus:
          return new PseudoSelector(PseudoTypes.Focus);

        case PseudoSelectorPrefix.PseudoTarget:
          return new PseudoSelector(PseudoTypes.Target);

        case PseudoSelectorPrefix.PseudoEnabled:
          return new PseudoSelector(PseudoTypes.Enabled);

        case PseudoSelectorPrefix.PseudoDisabled:
          return new PseudoSelector(PseudoTypes.Disabled);

        case PseudoSelectorPrefix.PseudoDefault:
          return new PseudoSelector(PseudoTypes.Default);

        case PseudoSelectorPrefix.PseudoChecked:
          return new PseudoSelector(PseudoTypes.Checked);

        case PseudoSelectorPrefix.PseudoIndeterminate:
          return new PseudoSelector(PseudoTypes.Indeterminate);

        case PseudoSelectorPrefix.PseudoUnchecked:
          return new PseudoSelector(PseudoTypes.Unchecked);

        case PseudoSelectorPrefix.PseudoValid:
          return new PseudoSelector(PseudoTypes.Valid);

        case PseudoSelectorPrefix.PseudoInvalid:
          return new PseudoSelector(PseudoTypes.Invalid);

        case PseudoSelectorPrefix.PseudoRequired:
          return new PseudoSelector(PseudoTypes.Required);

        case PseudoSelectorPrefix.PseudoReadonly:
          return new PseudoSelector(PseudoTypes.Readonly);

        case PseudoSelectorPrefix.PseudoReadwrite:
          return new PseudoSelector(PseudoTypes.Readwrite);

        case PseudoSelectorPrefix.PseudoInrange:
          return new PseudoSelector(PseudoTypes.Inrange);

        case PseudoSelectorPrefix.PseudoOutofrange:
          return new PseudoSelector(PseudoTypes.Outofrange);

        case PseudoSelectorPrefix.PseudoOptional:
          return new PseudoSelector(PseudoTypes.Optional);

        case PseudoSelectorPrefix.PseudoElementBefore:
          return new PseudoElementSelector(PseudoElements.Before);

        case PseudoSelectorPrefix.PseudoElementAfter:
          return new PseudoElementSelector(PseudoElements.After);

        case PseudoSelectorPrefix.PseudoElementFirstline:
          return new PseudoElementSelector(PseudoElements.Firstline);

        case PseudoSelectorPrefix.PseudoElementFirstletter:
          return new PseudoElementSelector(PseudoElements.Firstletter);

        case PseudoSelectorPrefix.PseudoElementSelection:
          return new PseudoElementSelector(PseudoElements.Selection);

        default:
          return new UnknownSelector(":" + token.ToString());
      }
    }
  }
}
