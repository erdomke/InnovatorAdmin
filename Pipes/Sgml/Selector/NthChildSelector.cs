
// ReSharper disable once CheckNamespace
namespace Pipes.Sgml.Selector
{
  public class NthChildSelector : PseudoSelector
  {
    public int Step;
    public int Offset;

    public NthChildSelector(PseudoTypes type) : base(type) { }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}