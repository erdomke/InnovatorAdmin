using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public interface IRuleContainer
    {
        List<RuleSet> Declarations { get; }
    }
}