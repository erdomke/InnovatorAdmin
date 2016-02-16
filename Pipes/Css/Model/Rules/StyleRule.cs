using System;
using Pipes.Css.Model;
using Pipes.Css.Model.Extensions;
using Pipes.Sgml.Selector;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
  public class StyleRule : RuleSet, ISupportsSelector, ISupportsDeclarations
  {
    private string _value;
    private ISelector _selector;
    private readonly StyleDeclaration _declarations;

    public StyleRule()
      : this(new StyleDeclaration())
    { }

    public StyleRule(IEnumerable<Property> declarations)
    {
      RuleType = RuleType.Style;
      _declarations = declarations as StyleDeclaration;
      if (_declarations == null) _declarations = new StyleDeclaration(declarations);
    }

    public ISelector Selector
    {
      get { return _selector; }
      set
      {
        _selector = value;
        _value = value.ToString();
      }
    }

    public string Value
    {
      get { return _value; }
      set
      {
        _selector = Parser.ParseSelector(value);
        _value = value;
      }
    }

    public StyleDeclaration Declarations
    {
      get { return _declarations; }
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public override string ToString(bool friendlyFormat, int indentation = 0)
    {
      return _value.NewLineIndent(friendlyFormat, indentation) +
          "{" +
          _declarations.ToString(friendlyFormat, indentation) +
          "}".NewLineIndent(friendlyFormat, indentation);
    }
  }
}
