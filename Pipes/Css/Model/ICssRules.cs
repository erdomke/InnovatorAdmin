using System.Collections.Generic;

namespace Pipes.Css.Model
{
    interface ISupportsRuleSets
    {
        List<RuleSet> RuleSets { get; }
    }
}