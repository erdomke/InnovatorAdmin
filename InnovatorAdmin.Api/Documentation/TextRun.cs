namespace InnovatorAdmin.Documentation
{
  public class TextRun : IElement
  {
    public RunStyle Style { get; }
    public string Text { get; }

    public TextRun(string text)
    {
      Text = text;
    }

    public TextRun(string text, RunStyle style)
    {
      Text = text;
      Style = style;
    }

    public T Visit<T>(IElementVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
  }
}
