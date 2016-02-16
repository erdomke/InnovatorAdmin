
// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
    public abstract class ConditionalRule : AggregateRule
    {
        public virtual string Condition
        {
            get { return string.Empty; }
            set { }
        }
    }
}
