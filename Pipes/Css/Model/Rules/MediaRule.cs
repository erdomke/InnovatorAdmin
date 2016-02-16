using Pipes.Css.Model;
using Pipes.Css.Model.Extensions;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public class MediaRule : ConditionalRule, ISupportsMedia
    {
        private readonly MediaTypeList _media;

        public MediaRule() 
        {
            _media = new MediaTypeList();
            RuleType = RuleType.Media;
        }

        public override string Condition
        {
            get { return _media.ToString(); }
            set { }
        }

        public MediaTypeList Media
        {
            get { return _media; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var join = friendlyFormat ? "".NewLineIndent(true, indentation + 1) : "";

            var declarationList = RuleSets.Select(d => d.ToString(friendlyFormat, indentation + 1).TrimFirstLine());
            var declarations = declarationList.GroupConcat(join);

            return ("@media " + _media.ToString(friendlyFormat, indentation) + "{").NewLineIndent(friendlyFormat, indentation) +
                declarations.TrimFirstLine().NewLineIndent(friendlyFormat, indentation + 1) +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
