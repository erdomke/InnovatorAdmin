using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Sgml.Query;
using Pipes.Sgml.Selector;
using Pipes.Css.Model;
using Pipes.Xml;
using System.Threading;

namespace Pipes.Css
{
  public static class Utils
  {

    public static StyleRule DefaultStyleRule(string tagName, IEnumerable<string> parents)
    {
      return new StyleRule(DefaultProperties(tagName, parents));
    }
    public static IEnumerable<Property> DefaultProperties (string tagName, IEnumerable<string> parents)
    {
      // TODO: deal with embedded list styling

      switch (tagName.ToLowerInvariant())
      {
        case "address":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-style") { Term = new PrimitiveTerm(UnitType.Ident, "italic") };
          break;
        case "html":
        case "article":
        case "aside":
        case "div":
        case "footer":
        case "header":
        case "hgroup":
        case "layer":
        case "main":
        case "nav":
        case "section":
        case "figcaption":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          break;
        case "p":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          break;
        case "head":
        case "link":
        case "meta":
        case "script":
        case "style":
        case "title":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "none") };
          break;
        case "body":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin") { Term = new PrimitiveTerm(UnitType.Pixel, 8) };
          break;
        case "marquee":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "inline-block") };
          break;
        case "blockquote":
        case "figure":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Pixel, 40) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Pixel, 40) };
          break;
        case "center":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("text-align") { Term = new PrimitiveTerm(UnitType.Ident, "center") };
          break;
        case "h1":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ems, 2) };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 0.67F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 0.67F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "h2":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ems, 1.5F) };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 0.83F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 0.83F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "h3":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ems, 1.17F) };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "h4":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1.33F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1.33F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "h5":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ems, 0.83F) };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1.67F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1.67F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "h6":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ems, 0.67F) };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 2.33F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 2.33F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "table":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table") };
          yield return new Property("border-collapse") { Term = new PrimitiveTerm(UnitType.Ident, "separate") };
          yield return new Property("border-spacing") { Term = new PrimitiveTerm(UnitType.Pixel, 2) };
          yield return new Property("border-color") { Term = new HtmlColor(System.Drawing.Color.Gray) };
          break;
        case "thead":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-header-group") };
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "middle") };
          yield return new Property("border-color") { Term = PrimitiveTerm.Inherit };
          break;
        case "tbody":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-row-group") };
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "middle") };
          yield return new Property("border-color") { Term = PrimitiveTerm.Inherit };
          break;
        case "tfoot":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-footer-group") };
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "middle") };
          yield return new Property("border-color") { Term = PrimitiveTerm.Inherit };
          break;
        case "tr":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-row") };
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "middle") };
          yield return new Property("border-color") { Term = PrimitiveTerm.Inherit };
          break;
        case "td":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-cell") };
          yield return new Property("vertical-align") { Term = PrimitiveTerm.Inherit };
          break;
        case "th":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-cell") };
          yield return new Property("vertical-align") { Term = PrimitiveTerm.Inherit };
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          yield return new Property("text-align") { Term = new PrimitiveTerm(UnitType.Ident, "center") };
          break;
        case "col":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-column") };
          break;
        case "colgroup":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-column-group") };
          break;
        case "caption":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "table-caption") };
          yield return new Property("text-align") { Term = new PrimitiveTerm(UnitType.Ident, "center") };
          break;
        case "ul":
        case "menu":
        case "dir":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          switch (parents.Where(v => string.Compare(v, "ol", StringComparison.OrdinalIgnoreCase) == 0 ||
                                     string.Compare(v, "ul", StringComparison.OrdinalIgnoreCase) == 0).Count())
          {
            case 2:
              yield return new Property("list-style-type") { Term = new PrimitiveTerm(UnitType.Ident, "circle") };
              break;
            case 3:
              yield return new Property("list-style-type") { Term = new PrimitiveTerm(UnitType.Ident, "square") };
              break;
            default:
              yield return new Property("list-style-type") { Term = new PrimitiveTerm(UnitType.Ident, "disc") };
              break;

          }
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("-webkit-padding-start") { Term = new PrimitiveTerm(UnitType.Pixel, 40) };
          break;
        case "ol":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("list-style-type") { Term = new PrimitiveTerm(UnitType.Ident, "decimal") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("-webkit-padding-start") { Term = new PrimitiveTerm(UnitType.Pixel, 40) };
          break;
        case "li":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "list-item") };
          break;
        case "dd":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("-webkit-margin-start") { Term = new PrimitiveTerm(UnitType.Pixel, 40) };
          break;
        case "dl":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          break;
        case "dt":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          break;
        case "form":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 0F) };
          break;
        case "label":
          yield return new Property("cursor") { Term = new PrimitiveTerm(UnitType.Ident, "default") };
          break;
        case "legend":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Pixel, 2F) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Pixel, 2F) };
          yield return new Property("border") { Term = new PrimitiveTerm(UnitType.Ident, "none") };
          break;
        case "fieldset":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Pixel, 2F) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Pixel, 2F) };
          yield return new Property("-webkit-padding-before") { Term = new PrimitiveTerm(UnitType.Ems, 0.35F) };
          yield return new Property("padding-left") { Term = new PrimitiveTerm(UnitType.Ems, 0.75F) };
          yield return new Property("padding-right") { Term = new PrimitiveTerm(UnitType.Ems, 0.75F) };
          yield return new Property("-webkit-padding-after") { Term = new PrimitiveTerm(UnitType.Ems, 0.625F) };
          break;
        case "optgroup":
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bolder") };
          break;
        case "option":
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "normal") };
          break;
        case "output":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "inline") };
          break;
        case "u":
        case "ins":
          yield return new Property("text-decoration") { Term = new PrimitiveTerm(UnitType.Ident, "underline") };
          break;
        case "strong":
        case "b":
          yield return new Property("font-weight") { Term = new PrimitiveTerm(UnitType.Ident, "bold") };
          break;
        case "i":
        case "cite":
        case "em":
        case "var":
        case "dfn":
          yield return new Property("font-style") { Term = new PrimitiveTerm(UnitType.Ident, "italic") };
          break;
        case "tt":
        case "code":
        case "kbd":
        case "samp":
          yield return new Property("font-family") { Term = new PrimitiveTerm(UnitType.Ident, "monospace") };
          break;
        case "pre":
        case "xmp":
        case "plaintext":
        case "listing":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("font-family") { Term = new PrimitiveTerm(UnitType.Ident, "monospace") };
          yield return new Property("white-space") { Term = new PrimitiveTerm(UnitType.Ident, "pre") };
          yield return new Property("margin-top") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-bottom") { Term = new PrimitiveTerm(UnitType.Ems, 1F) };
          yield return new Property("margin-left") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          yield return new Property("margin-right") { Term = new PrimitiveTerm(UnitType.Ems, 0) };
          break;
        case "mark":
          yield return new Property("background-color") { Term = new HtmlColor(System.Drawing.Color.Yellow) };
          yield return new Property("color") { Term = new HtmlColor(System.Drawing.Color.Black) };
          break;
        case "big":
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ident, "larger") };
          break;
        case "small":
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ident, "smaller") };
          break;
        case "s":
        case "strike":
        case "del":
          yield return new Property("text-decoration") { Term = new PrimitiveTerm(UnitType.Ident, "line-through") };
          break;
        case "sub":
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "sub") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ident, "smaller") };
          break;
        case "sup":
          yield return new Property("vertical-align") { Term = new PrimitiveTerm(UnitType.Ident, "super") };
          yield return new Property("font-size") { Term = new PrimitiveTerm(UnitType.Ident, "smaller") };
          break;
        case "nobr":
          yield return new Property("white-space") { Term = new PrimitiveTerm(UnitType.Ident, "nowrap") };
          break;
        case "a":
          yield return new Property("color") { Term = new HtmlColor(System.Drawing.Color.Blue) };
          yield return new Property("text-decoration") { Term = new PrimitiveTerm(UnitType.Ident, "underline") };
          yield return new Property("cursor") { Term = new PrimitiveTerm(UnitType.Ident, "auto") };
          break;
        case "ruby":
          yield return new Property("text-indent") { Term = new PrimitiveTerm(UnitType.Pixel, 0) };
          break;
        case "rt":
          yield return new Property("text-indent") { Term = new PrimitiveTerm(UnitType.Pixel, 0) };
          yield return new Property("line-height") { Term = new PrimitiveTerm(UnitType.Ident, "normal") };
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          break;
        case "rp":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "none") };
          break;
        case "noframes":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "none") };
          break;
        case "frame":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          break;
        case "frameset":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          yield return new Property("border-color") { Term = PrimitiveTerm.Inherit };
          break;
        case "iframe":
          yield return new Property("border-width") { Term = new PrimitiveTerm(UnitType.Pixel, 2F) };
          yield return new Property("border-style") { Term = new PrimitiveTerm(UnitType.Ident, "inset") };
          break;
        case "details":
        case "summary":
          yield return new Property("display") { Term = new PrimitiveTerm(UnitType.Ident, "block") };
          break;
      }
    }

    public static bool IsInheritable(string prop)
    {
      switch (prop.ToLowerInvariant())
      {
        case "azimuth":
        case "border-collapse":
        case "border-spacing":
        case "caption-side":
        case "color":
        case "cursor":
        case "direction":
        case "elevation":
        case "empty-cells":
        case "font-family":
        case "font-size":
        case "font-variant":
        case "font-weight":
        case "font":
        case "letter-spacing":
        case "line-height":
        case "list-style-image":
        case "list-style-position":
        case "list-style-type":
        case "list-style":
        case "orphans":
        case "pitch-range":
        case "pitch":
        case "quotes":
        case "richness":
        case "speak-header":
        case "speak-numeral":
        case "speak-puntuation":
        case "speak":
        case "speech-rate":
        case "stress":
        case "text-align":
        case "text-indent":
        case "text-transform":
        case "visibility":
        case "voice-family":
        case "volume":
        case "white-space":
        case "widows":
        case "word-spacing":
          return true;
        default:
          return false;
      }

    }

    public static IEnumerable<Property> ApplicableProperties(this IEnumerable<StyleRule> rules, IQueryableNode node, IQueryEngine engine, StringComparison comparison, GlobalStyleContext settings)
    {
      var visitor = new MatchVisitor(engine);
      visitor.Comparison = comparison;
      var terms = new Dictionary<string, TermSpecificity>();
      TermSpecificity existing;
      int specificity = 0;
      int propSpecificity;

      foreach (var style in rules)
      {
        if (style.Selector != null)
        {
          visitor.Initialize(node);
          style.Selector.Visit(visitor);
        }
        if (style.Selector == null || visitor.IsMatch())
        {
          specificity = style.Selector == null ? 0 : (visitor.MatchSpecificity > 0 ? visitor.MatchSpecificity : style.Selector.GetSpecificity());
          foreach (var prop in style.Declarations.SelectMany(p => p.Expand(settings.LeftToRight)))
          {
            propSpecificity = specificity + (prop.Important ? (1 << 20) : 0);
            if (terms.TryGetValue(prop.Name, out existing))
            {
              if (propSpecificity >= existing.Specificity)
              {
                existing.Property = prop;
              }
            }
            else
            {
              terms[prop.Name] = new TermSpecificity() { Property = prop, Specificity = propSpecificity };
            }
          }
        }
      }

      return terms.Values.Select(v => v.Property);
    }

    private class ImportLoader
    {
      private IList<ImportRule> _imports;
      private ManualResetEvent[] _resets; // = new ManualResetEvent[imports.Count]
      private Func<string, string> _resourceLoader;
      private StyleSheet[] _sheets;

      public IEnumerable<StyleSheet> Sheets
      {
        get { return _sheets; }
      }

      public ImportLoader(IList<ImportRule> imports, Func<string, string> resourceLoader)
      {
        _imports = imports;
        _resourceLoader = resourceLoader;
        if (imports.Any() && resourceLoader != null)
        {
          _resets = new ManualResetEvent[imports.Count];
          _sheets = new StyleSheet[imports.Count];
        }
      }

      private void LoadSheet(object o)
      {
        var i = (int)o;
        try
        {
          var parser = new Parser();
          _sheets[i] = parser.Parse(_resourceLoader.Invoke(_imports[i].Href));
        }
        finally
        {
          _resets[i].Set();
        }
      }

      public void Start()
      {
        if (_resets != null)
        {
          Thread background;
          for (int i = 0; i < _imports.Count; i++)
          {
            _resets[i] = new ManualResetEvent(false);
            background = new Thread(LoadSheet);
            background.Start(i);
          }
        }
      }

      public void WaitAll()
      {
        if (_resets != null)
        {
          foreach (var reset in _resets)
          {
            reset.WaitOne();
          }
        }
      }

    }

    public static IEnumerable<StyleRule> GetStyleRules(this IEnumerable<RuleSet> rules, GlobalStyleContext settings)
    {
      StyleRule style;
      MediaRule media;

      while (rules.Any())
      {
        var loader = new ImportLoader(rules.OfType<ImportRule>()
                                           .Where(i => !i.Media.Any() || i.Media.Any(m => MediaMatches(m, settings)))
                                           .ToList(),
                                      settings.ResourceLoader);
        loader.Start();

        foreach (var rule in rules)
        {
          style = rule as StyleRule;
          media = rule as MediaRule;
          if (style != null)
          {
            yield return style;
          }
          else if (media != null)
          {
            if (media.Media.Any(m => MediaMatches(m, settings)))
            {
              foreach (var s in media.RuleSets.GetStyleRules(settings))
              {
                yield return s;
              }
            }
          }
        }

        loader.WaitAll();
        if (loader.Sheets.Any())
        {
          rules = loader.Sheets.SelectMany(s => s.Rules);
        }
        else
        {
          rules = Enumerable.Empty<RuleSet>();
        }
      }
    }

    private static bool MediaMatches(MediaDefinition d, GlobalStyleContext settings)
    {
      var context = new StyleContext();
      var result = (d.Modifier != MediaTypeModifier.Not && (d.Type & settings.Media) > 0) ||
                   (d.Modifier == MediaTypeModifier.Not && (d.Type & settings.Media) == 0);
      if (result)
      {
        var width = d.Properties.Where(p => string.Compare(p.Name, "width", StringComparison.OrdinalIgnoreCase) == 0 ||
                                            p.Name.EndsWith("-width", StringComparison.OrdinalIgnoreCase));
        var plainProps = width.OfType<MediaPropertyPlain>();
        double px;
        foreach (var plain in plainProps)
        {
          px = ToPx(plain.Value as PrimitiveTerm, context, false);
          if (plain.Name.StartsWith("max-", StringComparison.OrdinalIgnoreCase) && px > settings.MediaWidth) return false;
          if (plain.Name.StartsWith("min-", StringComparison.OrdinalIgnoreCase) && px < settings.MediaWidth) return false;
        }

        var rangeProps = width.OfType<MediaPropertyRange>();
        foreach (var range in rangeProps)
        {
          if (range.LowerBound != null)
          {
            px = ToPx(range.LowerBound as PrimitiveTerm, context, false);
            if (settings.MediaWidth < px || (settings.MediaWidth == px && !range.LowerCompare.EndsWith("="))) return false;
          }
          if (range.UpperBound != null)
          {
            px = ToPx(range.UpperBound as PrimitiveTerm, context, false);
            if (settings.MediaWidth > px || (settings.MediaWidth == px && !range.UpperCompare.EndsWith("="))) return false;
          }
        }
      }
      return result;
    }

    public static double ToPx(this PrimitiveTerm term, StyleContext context, bool isFont)
    {
      switch (term.PrimitiveType)
      {
        case UnitType.Centimeter:
          return Math.Round(((float)term.Value) / 2.54 * context.Global.Dpi);
        case UnitType.Ems:
          return Math.Round(((float)term.Value) * context.FontSizePx);
        case UnitType.Exs:
          return Math.Round(((float)term.Value) * context.FontSizePx * 0.447265);
        case UnitType.Inch:
          return Math.Round(((float)term.Value) * context.Global.Dpi);
        case UnitType.Millimeter:
          return Math.Round(((float)term.Value) / 25.4 * context.Global.Dpi);
        case UnitType.Percentage:
          return Math.Round(((float)term.Value) * (isFont ? context.FontSizePx : context.ContentWidth) / 100.0);
        case UnitType.Number:
        case UnitType.Pixel:
          return System.Convert.ToDouble(term.Value);
        case UnitType.Point:
          return Math.Round(((float)term.Value) * 96.0 / 72);
        case UnitType.ViewportHeight:
          return Math.Round(((float)term.Value) * context.Global.ViewportHeight / 100.0);
        case UnitType.ViewportMax:
          return Math.Round(((float)term.Value) * Math.Max(context.Global.ViewportWidth, context.Global.ViewportHeight) / 100.0);
        case UnitType.ViewportMin:
          return Math.Round(((float)term.Value) * Math.Min(context.Global.ViewportWidth, context.Global.ViewportHeight) / 100.0);
        case UnitType.ViewportWidth:
          return Math.Round(((float)term.Value) * context.Global.ViewportWidth / 100.0);
        default:
          throw new NotSupportedException();
      }
    }

    public static IEnumerable<Property> Expand(this Property style, bool leftToRight)
    {
      Dictionary<string, Property> children;
      PrimitiveTerm prim;
      Term current;
      TermList list;
      IList<Term> terms;

      switch(style.Name.ToLowerInvariant())
      {
        case "background":
          children = new Dictionary<string, Property>()
          {
            {"background-color",    new Property("background-color")    { Term = new InitialTerm() }},
            {"background-image",    new Property("background-image")    { Term = new InitialTerm() }},
            {"background-repeat",   new Property("background-repeat")   { Term = new InitialTerm() }},
            {"background-position", new Property("background-position") { Term = new InitialTerm() }}
          };

          foreach (var term in GetEnumerable(style.Term))
          {
            if (term is HtmlColor)
            {
              children["background-color"].Term = term;
            }
            else
            {
              prim = term as PrimitiveTerm;
              if (prim != null)
              {
                if (prim.PrimitiveType == UnitType.Uri)
                {
                  children["background-image"].Term = term;
                }
                else if (prim.PrimitiveType == UnitType.Ident)
                {
                  switch (prim.Value.ToString())
                  {
                    case "repeat-x":
                    case "repeat-y":
                    case "repeat":
                    case "space":
                    case "round":
                    case "no-repeat":
                      current = children["background-repeat"].Term;
                      list = current as TermList;
                      if (list != null)
                      {
                        list.AddTerm(prim);
                      }
                      else if (current is InitialTerm)
                      {
                        children["background-repeat"].Term = prim;
                      }
                      else
                      {
                        list = new TermList();
                        list.AddTerm(current);
                        list.AddTerm(prim);
                        children["background-repeat"].Term = list;
                      }
                      break;
                  }
                }
              }
            }
          }

          foreach (var val in children.Values) yield return val;
          break;
        case "font":
          children = new Dictionary<string, Property>()
          {
            {"font-style",    new Property("font-style")    { Term = new InitialTerm() }},
            {"font-weight",   new Property("font-weight")   { Term = new InitialTerm() }},
            {"font-size",     new Property("font-size")     { Term = new InitialTerm() }},
            {"line-height",   new Property("line-height")   { Term = new InitialTerm() }},
            {"font-family",   new Property("font-family")   { Term = new InitialTerm() }}
          };

          foreach (var term in GetEnumerable(style.Term))
          {
            prim = term as PrimitiveTerm;
            if (prim != null)
            {
              if (prim.PrimitiveType == UnitType.Number)
              {
                switch (prim.Value.ToString())
                {
                  case "100":
                  case "200":
                  case "300":
                  case "400":
                  case "500":
                  case "600":
                  case "700":
                  case "800":
                  case "900":
                    children["font-weight"].Term = term;
                    break;
                  default:
                    children["line-height"].Term = term;
                    break;
                }
              }
              else if (prim.PrimitiveType == UnitType.Ident)
              {
                switch (prim.Value.ToString())
                {
                  case "italic":
                  case "oblique":
                    children["font-style"].Term = term;
                    break;
                  case "bold":
                  case "bolder":
                  case "lighter":
                    children["font-weight"].Term = term;
                    break;
                  default:
                    list = term as TermList;
                    if (list != null)
                    {
                      list.AddTerm(prim);
                    }
                    else if (term is InitialTerm)
                    {
                      children["font-family"].Term = prim;
                    }
                    else
                    {
                      list = new TermList();
                      list.AddTerm(term);
                      list.AddTerm(prim);
                      children["font-family"].Term = list;
                    }
                    break;
                }
              }
              else
              {
                children["font-size"].Term = term;
              }
            }
          }

          foreach (var val in children.Values) yield return val;
          break;
        case "border":
          children = new Dictionary<string, Property>()
          {
            {"border-width",    new Property("border-width")    { Term = new InitialTerm() }},
            {"border-style",    new Property("border-style")    { Term = new InitialTerm() }},
            {"border-color",    new Property("border-color")    { Term = new InitialTerm() }},
          };

          foreach (var term in GetEnumerable(style.Term))
          {
            if (term is HtmlColor)
            {
              children["border-color"].Term = term;
            }
            else
            {
              prim = term as PrimitiveTerm;
              if (prim != null)
              {
                if (prim.PrimitiveType == UnitType.Ident)
                {
                  children["border-style"].Term = term;
                }
                else
                {
                  children["border-width"].Term = term;
                }
              }
            }
          }

          foreach (var val in children.Values) yield return val;
          break;
        case "margin":
          terms = GetEnumerable(style.Term).ToList();
          switch (terms.Count)
          {
            case 1:
              yield return new Property("margin-top")    { Term = terms[0] };
              yield return new Property("margin-right")  { Term = terms[0] };
              yield return new Property("margin-bottom") { Term = terms[0] };
              yield return new Property("margin-left")   { Term = terms[0] };
              break;
            case 2:
              yield return new Property("margin-top")    { Term = terms[0] };
              yield return new Property("margin-right")  { Term = terms[1] };
              yield return new Property("margin-bottom") { Term = terms[0] };
              yield return new Property("margin-left")   { Term = terms[1] };
              break;
            case 3:
              yield return new Property("margin-top")    { Term = terms[0] };
              yield return new Property("margin-right")  { Term = terms[1] };
              yield return new Property("margin-bottom") { Term = terms[2] };
              yield return new Property("margin-left")   { Term = terms[1] };
              break;
            case 4:
              yield return new Property("margin-top")    { Term = terms[0] };
              yield return new Property("margin-right")  { Term = terms[1] };
              yield return new Property("margin-bottom") { Term = terms[2] };
              yield return new Property("margin-left")   { Term = terms[3] };
              break;
            default:
              throw new InvalidOperationException();
          }

          break;
        case "padding":
          terms = GetEnumerable(style.Term).ToList();
          switch (terms.Count)
          {
            case 1:
              yield return new Property("padding-top")    { Term = terms[0] };
              yield return new Property("padding-right")  { Term = terms[0] };
              yield return new Property("padding-bottom") { Term = terms[0] };
              yield return new Property("padding-left")   { Term = terms[0] };
              break;
            case 2:
              yield return new Property("padding-top")    { Term = terms[0] };
              yield return new Property("padding-right")  { Term = terms[1] };
              yield return new Property("padding-bottom") { Term = terms[0] };
              yield return new Property("padding-left")   { Term = terms[1] };
              break;
            case 3:
              yield return new Property("padding-top")    { Term = terms[0] };
              yield return new Property("padding-right")  { Term = terms[1] };
              yield return new Property("padding-bottom") { Term = terms[2] };
              yield return new Property("padding-left")   { Term = terms[1] };
              break;
            case 4:
              yield return new Property("padding-top")    { Term = terms[0] };
              yield return new Property("padding-right")  { Term = terms[1] };
              yield return new Property("padding-bottom") { Term = terms[2] };
              yield return new Property("padding-left")   { Term = terms[3] };
              break;
            default:
              throw new InvalidOperationException();
          }

          break;
        case "-webkit-margin-start":
        case "-moz-margin-start":
          if (leftToRight) yield return new Property("margin-left") { Term = style.Term };
          if (!leftToRight) yield return new Property("margin-right") { Term = style.Term };
          break;
        case "-webkit-padding-start":
        case "-moz-padding-start":
          if (leftToRight) yield return new Property("padding-left") { Term = style.Term };
          if (!leftToRight) yield return new Property("padding-right") { Term = style.Term };
          break;
        case "-webkit-margin-end":
        case "-moz-margin-end":
          if (leftToRight) yield return new Property("margin-right") { Term = style.Term };
          if (!leftToRight) yield return new Property("margin-left") { Term = style.Term };
          break;
        case "-webkit-padding-end":
        case "-moz-padding-end":
          if (leftToRight) yield return new Property("padding-right") { Term = style.Term };
          if (!leftToRight) yield return new Property("padding-left") { Term = style.Term };
          break;
        default:
          yield return style;
          break;
      }
    }

    public static StyleRule ParseInline(string style)
    {
      if (string.IsNullOrEmpty(style)) return null;
      var parser = new Parser();
      var stylesheet = parser.Parse("a {" + style + "}");
      var result = stylesheet.StyleRules.FirstOrDefault();
      result.Selector = InlineSelector.Value;
      return result;
    }

    private static IEnumerable<Term> GetEnumerable(Term value)
    {
      var enumerable = value as IEnumerable<Term>;
      if (enumerable == null) return Enumerable.Repeat(value, 1);
      return enumerable;
    }

#if DEBUG
    public static void Test()
    {
      var parser = new Parser();
      //var stylesheet = parser.Parse(System.IO.File.ReadAllText(@"C:\Users\edomke\Documents\Local_Projects\Lumen\Editing\Lumen\css\lumen.posts.css"));
      var stylesheet = parser.Parse("p {font-family: Arial, 'Times New Roman', sans-serif}");
    }
#endif

    private class TermSpecificity
    {
      public Property Property { get; set; }
      public int Specificity { get; set; }
    }
  }
}
