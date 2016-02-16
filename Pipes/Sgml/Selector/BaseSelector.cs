
// ReSharper disable once CheckNamespace
namespace Pipes.Sgml.Selector
{
  public abstract class BaseSelector : ISelector
  {
    public sealed override string ToString()
    {
      using (var writer = new System.IO.StringWriter())
      {
        this.Visit(new TextRendererSelectorVisitor(writer));
        return writer.ToString();
      }
    }

    public abstract void Visit(ISelectorVisitor visitor);
  }
}

