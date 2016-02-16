using System.Collections.Generic;
using Pipes.Css.Model;
using Pipes.Css.Model.Extensions;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public class KeyframeRule : RuleSet, ISupportsDeclarations
    {
        private List<string> _values { get; set; }
        public KeyframeRule()
        {
            Declarations = new StyleDeclaration();
            RuleType = RuleType.Keyframe;
            _values = new List<string>();
        }

        public void AddValue(string value)
        {
            _values.Add(value);
        }

        public StyleDeclaration Declarations { get; private set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Empty.Indent(friendlyFormat, indentation) +
                _values.GroupConcat(",") + 
                "{" + 
                Declarations.ToString(friendlyFormat, indentation) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
