using System;
using System.Linq;
using System.Text;
using Pipes.Css.Model;
using Pipes.Css.Model.TextBlocks;
using System.Collections.Generic;
using Pipes.Sgml.Selector;

namespace Pipes.Css
{
  public partial class Parser
  {
    private bool ParseTokenBlock(Block token)
    {
      switch (_parsingContext)
      {
        case ParsingContext.DataBlock:
          return ParseSymbol(token);

        case ParsingContext.InSelector:
          return ParseSelector(token);

        case ParsingContext.InDeclaration:
          return ParseDeclaration(token);

        case ParsingContext.AfterProperty:
          return ParsePostProperty(token);

        case ParsingContext.BeforeValue:
          return ParseValue(token);

        case ParsingContext.InValuePool:
          return ParseValuePool(token);

        case ParsingContext.InValueList:
          return ParseValueList(token);

        case ParsingContext.InSingleValue:
          return ParseSingleValue(token);

        case ParsingContext.ValueImportant:
          return ParseImportant(token);

        case ParsingContext.AfterValue:
          return ParsePostValue(token);

        case ParsingContext.InMediaList:
          return ParseMediaList(token);

        case ParsingContext.InMediaValue:
          return ParseMediaValue(token);

        case ParsingContext.BeforeImport:
          return ParseImport(token);

        case ParsingContext.AfterInstruction:
          return ParsePostInstruction(token);

        case ParsingContext.BeforeCharset:
          return ParseCharacterSet(token);

        case ParsingContext.BeforeNamespacePrefix:
          return ParseLeadingPrefix(token);

        case ParsingContext.AfterNamespacePrefix:
          return ParseNamespace(token);

        case ParsingContext.InCondition:
          return ParseCondition(token);

        case ParsingContext.InUnknown:
          return ParseUnknown(token);

        case ParsingContext.InKeyframeText:
          return ParseKeyframeText(token);

        case ParsingContext.BeforePageSelector:
          return ParsePageSelector(token);

        case ParsingContext.BeforeDocumentFunction:
          return ParsePreDocumentFunction(token);

        case ParsingContext.InDocumentFunction:
          return ParseDocumentFunction(token);

        case ParsingContext.AfterDocumentFunction:
          return ParsePostDocumentFunction(token);

        case ParsingContext.BetweenDocumentFunctions:
          return ParseDocumentFunctions(token);

        case ParsingContext.BeforeKeyframesName:
          return ParseKeyframesName(token);

        case ParsingContext.BeforeKeyframesData:
          return ParsePreKeyframesData(token);

        case ParsingContext.KeyframesData:
          return ParseKeyframesData(token);

        case ParsingContext.BeforeFontFace:
          return ParseFontface(token);

        case ParsingContext.InHexValue:
          return ParseHexValue(token);

        case ParsingContext.InFunction:

          return ParseValueFunction(token);
        default:
          return false;
      }
    }

    private bool ParseSymbol(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.AtRule)
      {
        var value = ((SymbolBlock)token).Value;
        switch (value)
        {
          case RuleTypes.Media:
            {
              AddRuleSet(new MediaRule());
              SetParsingContext(ParsingContext.InMediaList);
              break;
            }
          case RuleTypes.Page:
            {
              AddRuleSet(new PageRule());
              //SetParsingContext(ParsingContext.InSelector);
              SetParsingContext(ParsingContext.BeforePageSelector);
              break;
            }
          case RuleTypes.Import:
            {
              AddRuleSet(new ImportRule());
              SetParsingContext(ParsingContext.BeforeImport);
              break;
            }
          case RuleTypes.FontFace:
            {
              AddRuleSet(new FontFaceRule());
              //SetParsingContext(ParsingContext.InDeclaration);
              SetParsingContext(ParsingContext.BeforeFontFace);
              break;
            }
          case RuleTypes.CharacterSet:
            {
              AddRuleSet(new CharacterSetRule());
              SetParsingContext(ParsingContext.BeforeCharset);
              break;
            }
          case RuleTypes.Namespace:
            {
              AddRuleSet(new NamespaceRule());
              SetParsingContext(ParsingContext.BeforeNamespacePrefix);
              break;
            }
          case RuleTypes.Supports:
            {
              _buffer = new StringBuilder();
              AddRuleSet(new SupportsRule());
              SetParsingContext(ParsingContext.InCondition);
              break;
            }
          case BrowserPrefixes.Microsoft + RuleTypes.Keyframes:
          case BrowserPrefixes.Mozilla + RuleTypes.Keyframes:
          case BrowserPrefixes.Opera + RuleTypes.Keyframes:
          case BrowserPrefixes.Webkit + RuleTypes.Keyframes:
          case RuleTypes.Keyframes:
            {
              AddRuleSet(new KeyframesRule(value));
              SetParsingContext(ParsingContext.BeforeKeyframesName);
              break;
            }
          case RuleTypes.Document:
            {
              AddRuleSet(new DocumentRule());
              SetParsingContext(ParsingContext.BeforeDocumentFunction);
              break;
            }
          default:
            {
              _buffer = new StringBuilder();
              AddRuleSet(new GenericRule());
              SetParsingContext(ParsingContext.InUnknown);
              ParseUnknown(token);
              break;
            }
        }

        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        return FinalizeRule();
      }

      AddRuleSet(new StyleRule());
      SetParsingContext(ParsingContext.InSelector);
      ParseSelector(token);
      return true;

    }

    private bool ParseUnknown(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.Semicolon:
          CastRuleSet<GenericRule>().SetInstruction(_buffer.ToString());
          SetParsingContext(ParsingContext.DataBlock);

          return FinalizeRule();

        case GrammarSegment.CurlyBraceOpen:
          CastRuleSet<GenericRule>().SetCondition(_buffer.ToString());
          SetParsingContext(ParsingContext.DataBlock);
          break;

        default:
          _buffer.Append(token);
          break;
      }

      return true;
    }

    private bool ParseSelector(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.CurlyBraceOpen:
          {
            var rule = CurrentRule as ISupportsSelector;

            if (rule != null)
            {
              rule.Selector = _selectorFactory.GetSelector();
            }

            SetParsingContext(CurrentRule is StyleRule
                ? ParsingContext.InDeclaration
                : ParsingContext.DataBlock);
          }
          break;

        case GrammarSegment.CurlyBracketClose:
          return false;

        default:
          _selectorFactory.Apply(token);
          break;
      }

      return true;
    }

    private bool ParseDeclaration(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        FinalizeProperty();
        SetParsingContext(CurrentRule is KeyframeRule ? ParsingContext.KeyframesData : ParsingContext.DataBlock);
        return FinalizeRule();
      }

      if (token.GrammarSegment != GrammarSegment.Ident)
      {
        return false;
      }

      AddProperty(new Property(((SymbolBlock)token).Value));
      SetParsingContext(ParsingContext.AfterProperty);
      return true;
    }

    private bool ParsePostInstruction(Block token)
    {
      if (token.GrammarSegment != GrammarSegment.Semicolon)
      {
        return false;
      }

      SetParsingContext(ParsingContext.DataBlock);

      return FinalizeRule();
    }

    private bool ParseCondition(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.CurlyBraceOpen:
          CastRuleSet<SupportsRule>().Condition = _buffer.ToString();
          SetParsingContext(ParsingContext.DataBlock);
          break;

        default:
          _buffer.Append(token);
          break;
      }

      return true;
    }

    private bool ParseLeadingPrefix(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Ident)
      {
        CastRuleSet<NamespaceRule>().Prefix = ((SymbolBlock)token).Value;
        SetParsingContext(ParsingContext.AfterNamespacePrefix);

        return true;
      }

      if (token.GrammarSegment == GrammarSegment.String || token.GrammarSegment == GrammarSegment.Url)
      {
        CastRuleSet<NamespaceRule>().Uri = ((StringBlock)token).Value;
        return true;
      }

      SetParsingContext(ParsingContext.AfterInstruction);

      return ParsePostInstruction(token);
    }

    private bool ParsePostProperty(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Colon)
      {
        //_isFraction = false;
        SetParsingContext(ParsingContext.BeforeValue);
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.Semicolon || token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        ParsePostValue(token);
      }

      return false;
    }

    private bool ParseValue(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.Semicolon:
          SetParsingContext(ParsingContext.InDeclaration);
          break;

        case GrammarSegment.CurlyBracketClose:
          ParseDeclaration(token);
          break;

        default:
          SetParsingContext(ParsingContext.InSingleValue);
          return ParseSingleValue(token);
      }

      return false;
    }

    private bool ParseSingleValue(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.Dimension: // "3px"
          return AddTerm(new PrimitiveTerm(((UnitBlock)token).Unit, ((UnitBlock)token).Value));

        case GrammarSegment.Hash:// "#ffffff"
          return ParseSingleValueHexColor(((SymbolBlock)token).Value);

        case GrammarSegment.Delimiter: // "#"
          if (token.ToString() == ".")
          {
            _terms.AddSeparator(GrammarSegment.Delimiter);
            return true;
          }
          return ParseValueDelimiter((DelimiterBlock)token);

        case GrammarSegment.Ident: // "auto"
          return ParseSingleValueIdent((SymbolBlock)token);

        case GrammarSegment.String:// "'some value'"
          return AddTerm(new PrimitiveTerm(UnitType.String, ((StringBlock)token).Value));

        case GrammarSegment.Url:// "url('http://....')"
          return AddTerm(new PrimitiveTerm(UnitType.Uri, ((StringBlock)token).Value));

        case GrammarSegment.Percentage: // "10%"
          return AddTerm(new PrimitiveTerm(UnitType.Percentage, ((UnitBlock)token).Value));

        case GrammarSegment.Number: // "123"
          return AddTerm(new PrimitiveTerm(UnitType.Number, ((NumericBlock)token).Value));

        case GrammarSegment.Whitespace: // " "
          _terms.AddSeparator(GrammarSegment.Whitespace);
          SetParsingContext(ParsingContext.InValueList);
          return true;

        case GrammarSegment.Function: // rgba(...)
          _functionBuffers.Push(new FunctionBuffer(((SymbolBlock)token).Value));
          SetParsingContext(ParsingContext.InFunction);
          return true;

        case GrammarSegment.Comma: // ","
          _terms.AddSeparator(GrammarSegment.Comma);
          SetParsingContext(ParsingContext.InValuePool);
          return true;

        case GrammarSegment.Colon: // ":"
          _terms.AddSeparator(GrammarSegment.Colon);
          return true;

        case GrammarSegment.Semicolon: // ";"
        case GrammarSegment.CurlyBracketClose: // "}"
          return ParsePostValue(token);

        case GrammarSegment.ParenClose: // ")"
          SetParsingContext(ParsingContext.AfterValue);
          return true;

        default:
          return false;
      }
    }

    private bool ParseValueFunction(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.ParenClose:
          {
            var functionBuffer = _functionBuffers.Pop().Done();
            if (_functionBuffers.Any()) return AddTerm(functionBuffer);

            SetParsingContext(ParsingContext.InSingleValue);
            return AddTerm(functionBuffer);
          }
        case GrammarSegment.Whitespace:
          {
            if (!_functionBuffers.Any()) return AddTerm(new Whitespace());

            var functionBuffer = _functionBuffers.Peek();
            var lastTerm = functionBuffer.TermList.LastOrDefault();

            if (lastTerm is Comma || lastTerm is Whitespace)
              return true;

            return AddTerm(new Whitespace());
          }

        case GrammarSegment.Comma:
          return AddTerm(new Comma());

        case GrammarSegment.Delimiter:
          return AddTerm(new EqualSign());

        default:
          return ParseSingleValue(token);
      }
    }

    private bool ParseValueList(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.CurlyBracketClose:
        case GrammarSegment.Semicolon:
          ParsePostValue(token);
          break;

        case GrammarSegment.Comma:
          SetParsingContext(ParsingContext.InValuePool);
          break;

        default:
          SetParsingContext(ParsingContext.InSingleValue);
          return ParseSingleValue(token);
      }

      return true;
    }

    private bool ParseValuePool(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Semicolon || token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        ParsePostValue(token);
      }
      else
      {
        SetParsingContext(ParsingContext.InSingleValue);
        return ParseSingleValue(token);
      }

      return false;
    }

    private bool ParseHexValue(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.Number:
        case GrammarSegment.Dimension:
        case GrammarSegment.Ident:
          var rest = token.ToString();

          if (_buffer.Length + rest.Length <= 6)
          {
            _buffer.Append(rest);
            return true;
          }

          break;
      }

      ParseSingleValueHexColor(_buffer.ToString());
      SetParsingContext(ParsingContext.InSingleValue);
      return ParseSingleValue(token);
    }

    private bool ParsePostValue(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Semicolon)
      {
        FinalizeProperty();
        SetParsingContext(ParsingContext.InDeclaration);
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        return ParseDeclaration(token);
      }

      return false;
    }

    private bool ParseImportant(Block token)
    {
      if (token.GrammarSegment != GrammarSegment.Ident || ((SymbolBlock)token).Value != "important")
      {
        return ParsePostValue(token);
      }

      SetParsingContext(ParsingContext.AfterValue);
      _property.Important = true;

      return true;
    }

    private bool ParseValueDelimiter(DelimiterBlock token)
    {
      switch (token.Value)
      {
        case Specification.Em:
          SetParsingContext(ParsingContext.ValueImportant);
          return true;

        case Specification.Hash:
          _buffer = new StringBuilder();
          SetParsingContext(ParsingContext.InHexValue);
          return true;

        case Specification.Solidus:
          _terms.AddSeparator(GrammarSegment.Solidus);
          //_isFraction = true;
          return true;

        default:
          return false;
      }
    }

    private bool ParseSingleValueIdent(SymbolBlock token)
    {
      if (token.Value != "inherit")
      {
        return AddTerm(new PrimitiveTerm(UnitType.Ident, token.Value));
      }
      _terms.AddTerm(Term.Inherit);
      SetParsingContext(ParsingContext.AfterValue);
      return true;
    }

    private bool ParseSingleValueHexColor(string color)
    {
      HtmlColor htmlColor;

      if (HtmlColor.TryFromHex(color, out htmlColor))
        return AddTerm(htmlColor);
      return false;
    }

    #region Namespace
    private bool ParseNamespace(Block token)
    {
      SetParsingContext(ParsingContext.AfterInstruction);

      if (token.GrammarSegment != GrammarSegment.String)
      {
        return ParsePostInstruction(token);
      }

      CastRuleSet<NamespaceRule>().Uri = ((StringBlock)token).Value;

      return true;
    }
    #endregion

    #region Charset
    private bool ParseCharacterSet(Block token)
    {
      SetParsingContext(ParsingContext.AfterInstruction);

      if (token.GrammarSegment != GrammarSegment.String)
      {
        return ParsePostInstruction(token);
      }

      CastRuleSet<CharacterSetRule>().Encoding = ((StringBlock)token).Value;

      return true;
    }
    #endregion

    #region Import
    private bool ParseImport(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.String || token.GrammarSegment == GrammarSegment.Url)
      {
        CastRuleSet<ImportRule>().Href = ((StringBlock)token).Value;
        SetParsingContext(ParsingContext.InMediaList);
        return true;
      }

      SetParsingContext(ParsingContext.AfterInstruction);

      return false;
    }
    #endregion

    #region Font Face

    private bool ParseFontface(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.CurlyBraceOpen)
      {
        SetParsingContext(ParsingContext.InDeclaration);
        return true;
      }

      return false;
    }
    #endregion

    #region Keyframes
    private bool ParseKeyframesName(Block token)
    {
      //SetParsingContext(ParsingContext.BeforeKeyframesData);

      if (token.GrammarSegment == GrammarSegment.Ident)
      {
        CastRuleSet<KeyframesRule>().Identifier = ((SymbolBlock)token).Value;
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBraceOpen)
      {
        SetParsingContext(ParsingContext.KeyframesData);
        return true;
      }

      return false;
    }

    private bool ParsePreKeyframesData(Block token)
    {
      if (token.GrammarSegment != GrammarSegment.CurlyBraceOpen)
      {
        return false;
      }

      SetParsingContext(ParsingContext.BeforeKeyframesData);
      return true;
    }

    private bool ParseKeyframesData(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        SetParsingContext(ParsingContext.DataBlock);
        return FinalizeRule();
      }

      _buffer = new StringBuilder();

      return ParseKeyframeText(token);
    }

    private KeyframeRule _frame;
    private bool ParseKeyframeText(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.CurlyBraceOpen)
      {
        _frame = null;
        SetParsingContext(ParsingContext.InDeclaration);
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBracketClose)
      {
        ParseKeyframesData(token);
        return false;
      }

      if (token.GrammarSegment == GrammarSegment.Comma)
      {
        return true;
      }

      if (_frame == null)
      {
        _frame = new KeyframeRule();
        _frame.AddValue(token.ToString());

        CastRuleSet<KeyframesRule>().Declarations.Add(_frame);
        _activeRuleSets.Push(_frame);
      }
      else
      {
        _frame.AddValue(token.ToString());
      }

      return true;
    }
    #endregion

    #region Page

    private bool ParsePageSelector(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Colon || token.GrammarSegment == GrammarSegment.Whitespace)
      {
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.Ident)
      {
        CastRuleSet<PageRule>().Selector = ElementRestriction.LocalName(token.ToString());
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBraceOpen)
      {
        SetParsingContext(ParsingContext.InDeclaration);
        return true;
      }

      return false;
    }

    #endregion

    #region Document
    private bool ParsePreDocumentFunction(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.Url:
          CastRuleSet<DocumentRule>().Conditions.Add(new KeyValuePair<DocumentFunction, string>(DocumentFunction.Url, ((StringBlock)token).Value));
          break;

        case GrammarSegment.UrlPrefix:
          CastRuleSet<DocumentRule>().Conditions.Add(new KeyValuePair<DocumentFunction, string>(DocumentFunction.UrlPrefix, ((StringBlock)token).Value));
          break;

        case GrammarSegment.Domain:
          CastRuleSet<DocumentRule>().Conditions.Add(new KeyValuePair<DocumentFunction, string>(DocumentFunction.Domain, ((StringBlock)token).Value));
          break;

        case GrammarSegment.Function:
          if (string.Compare(((SymbolBlock)token).Value, "regexp", StringComparison.OrdinalIgnoreCase) == 0)
          {
            SetParsingContext(ParsingContext.InDocumentFunction);
            return true;
          }
          SetParsingContext(ParsingContext.AfterDocumentFunction);
          return false;

        default:
          SetParsingContext(ParsingContext.DataBlock);
          return false;
      }

      SetParsingContext(ParsingContext.BetweenDocumentFunctions);
      return true;
    }

    private bool ParseDocumentFunction(Block token)
    {
      SetParsingContext(ParsingContext.AfterDocumentFunction);

      if (token.GrammarSegment != GrammarSegment.String) return false;
      CastRuleSet<DocumentRule>().Conditions.Add(new KeyValuePair<DocumentFunction, string>(DocumentFunction.RegExp, ((StringBlock)token).Value));
      return true;
    }

    private bool ParsePostDocumentFunction(Block token)
    {
      SetParsingContext(ParsingContext.BetweenDocumentFunctions);
      return token.GrammarSegment == GrammarSegment.ParenClose;
    }

    private bool ParseDocumentFunctions(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Comma)
      {
        SetParsingContext(ParsingContext.BeforeDocumentFunction);
        return true;
      }

      if (token.GrammarSegment == GrammarSegment.CurlyBraceOpen)
      {
        SetParsingContext(ParsingContext.DataBlock);
        return true;
      }

      SetParsingContext(ParsingContext.DataBlock);
      return false;
    }
    #endregion

    #region Media
    private bool ParseMediaList(Block token)
    {
      if (token.GrammarSegment == GrammarSegment.Semicolon)
      {
        FinalizeRule();
        SetParsingContext(ParsingContext.DataBlock);
        return true;
      }

      _buffer = new StringBuilder();
      SetParsingContext(ParsingContext.InMediaValue);
      return ParseMediaValue(token);
    }

    private MediaDefinition _mediaDefn;
    private MediaProperty _mediaProp;
    private bool _nameFirst;
    private string _compare1;
    private string _compare2;

    private bool ParseMediaValue(Block token)
    {
      switch (token.GrammarSegment)
      {
        case GrammarSegment.CurlyBraceOpen:
        case GrammarSegment.Semicolon:
          {
            var container = CurrentRule as ISupportsMedia;

            if (container != null)
            {
              container.Media.AppendMedium(_mediaDefn);
            }

            if (CurrentRule is ImportRule)
            {
              return ParsePostInstruction(token);
            }

            SetParsingContext(ParsingContext.DataBlock);
            _mediaDefn = null;
            return token.GrammarSegment == GrammarSegment.CurlyBraceOpen;
          }
        case GrammarSegment.Comma:
          {
            var container = CurrentRule as ISupportsMedia;

            if (container != null)
            {
              container.Media.AppendMedium(_mediaDefn);
            }
            _mediaDefn = null;
            return true;
          }
        case GrammarSegment.Whitespace:
          {
            // Do Nothing
            return true;
          }
        default:
          {
            if (_mediaDefn == null) _mediaDefn = new MediaDefinition();
            switch (token.ToString())
            {
              case "only":
                _mediaDefn.Modifier = MediaTypeModifier.Only;
                break;
              case "not":
                _mediaDefn.Modifier = MediaTypeModifier.Not;
                break;
              case "screen":
                _mediaDefn.Type = MediaType.Screen;
                break;
              case "speech":
                _mediaDefn.Type = MediaType.Speech;
                break;
              case "print":
                _mediaDefn.Type = MediaType.Print;
                break;
              case "all":
                _mediaDefn.Type = MediaType.All;
                break;
              case "braille":
                _mediaDefn.Type = MediaType.Braille;
                break;
              case "embossed":
                _mediaDefn.Type = MediaType.Embossed;
                break;
              case "handheld":
                _mediaDefn.Type = MediaType.Handheld;
                break;
              case "projection":
                _mediaDefn.Type = MediaType.Projection;
                break;
              case "tty":
                _mediaDefn.Type = MediaType.Tty;
                break;
              case "tv":
                _mediaDefn.Type = MediaType.Tv;
                break;
              case "and":
                // do nothing
                break;
              case "(":
                _mediaProp = new MediaProperty();
                break;
              case ")":
                if (_mediaProp != null)
                {
                  var plain = _mediaProp as MediaPropertyPlain;
                  if (plain != null && _terms.Length == 1)
                  {
                    plain.Value = _terms[0];
                  }
                  else if (!string.IsNullOrEmpty(_compare1) && _terms.Length > 0)
                  {
                    var range = new MediaPropertyRange { Name = _mediaProp.Name };
                    if (_nameFirst)
                    {
                      if (_compare1.StartsWith("<"))
                      {
                        range.UpperBound = _terms[0];
                        range.UpperCompare = _compare1;
                      }
                      else
                      {
                        range.LowerBound = _terms[0];
                        range.LowerCompare = _compare1.Replace('>', '<');
                      }
                    }
                    else
                    {
                      if (_terms.Length == 1)
                      {
                        if (_compare1.StartsWith(">"))
                        {
                          range.UpperBound = _terms[0];
                          range.UpperCompare = _compare1.Replace('>', '<');
                        }
                        else
                        {
                          range.LowerBound = _terms[0];
                          range.LowerCompare = _compare1;
                        }
                      }
                      else
                      {
                        if (_compare1.StartsWith("<"))
                        {
                          range.LowerBound = _terms[0];
                          range.LowerCompare = _compare1;
                          range.UpperBound = _terms[1];
                          range.UpperCompare = _compare2;
                        }
                        else
                        {
                          range.UpperBound = _terms[0];
                          range.UpperCompare = _compare1.Replace('>', '<');
                          range.LowerBound = _terms[1];
                          range.LowerCompare = _compare2.Replace('>', '<');
                        } 
                      }
                    }
                    _mediaProp = range;
                  }
                  _mediaDefn.Properties.Add(_mediaProp);
                }
                _compare1 = null;
                _compare2 = null;
                _mediaProp = null;
                _terms = new TermList();
                break;
              case ":":
                if (_mediaProp != null) _mediaProp = new MediaPropertyPlain { Name = _mediaProp.Name };
                break;
              case "<":
              case ">":
                if (string.IsNullOrEmpty(_compare1))
                  _compare1 = token.ToString();
                else
                  _compare2 = token.ToString();
                break;
              case "=":
                if (string.IsNullOrEmpty(_compare1) || (string.IsNullOrEmpty(_compare2) && _compare1 != "="))
                  _compare1 = (_compare1 ?? "") + token.ToString();
                else
                  _compare2 = (_compare2 ?? "") + token.ToString();
                break;
              default:
                if (token.GrammarSegment == GrammarSegment.Ident && string.IsNullOrEmpty(_mediaProp.Name))
                {
                  _mediaProp.Name = token.ToString();
                  _nameFirst = string.IsNullOrEmpty(_compare1);
                }
                else
                {
                  ParseSingleValue(token);
                }
                break;
            }
            return true;
          }
      }
    }
    #endregion
  }
}
