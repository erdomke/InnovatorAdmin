using System.Linq;
using Pipes.Css.Model.Extensions;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public class SupportsRule : ConditionalRule
    {
        private string _condition;

        public SupportsRule()
        {
            RuleType = RuleType.Supports;
            _condition = string.Empty;
        }

        public override string Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public bool IsSupported{ get; set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var join = friendlyFormat ? "".NewLineIndent(true, indentation + 1) : "";

            var declarationList = RuleSets.Select(d => d.ToString(friendlyFormat, indentation + 1).TrimFirstLine());
            var declarations = declarationList.GroupConcat(join);

            return ("@supports" + _condition + "{").NewLineIndent(friendlyFormat, indentation) +
                declarations.TrimFirstLine().NewLineIndent(friendlyFormat, indentation + 1) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
