using System.Collections.Generic;
using Pipes.Css.Model;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public abstract class AggregateRule : RuleSet, ISupportsRuleSets
    {
        protected AggregateRule()
        {
            RuleSets = new List<RuleSet>();
        }

        public List<RuleSet> RuleSets { get; private set; }
    }
}
