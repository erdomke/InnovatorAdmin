using System.Drawing;

namespace Pipes.Data.Table
{
  public class FontStyle : IFontStyle
  {

    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public string Name { get; set; }
    public float SizeInPoints { get; set; }
    public bool Strikeout { get; set; }
    public bool Underline { get; set; }

    public FontStyle()
    {
      //Do Nothing
    }
    public FontStyle(Font font)
    {
      this.Bold = font.Bold;
      this.Italic = font.Italic;
      this.Name = font.Name;
      this.SizeInPoints = font.SizeInPoints;
      this.Strikeout = font.Strikeout;
      this.Underline = font.Underline;
    }

    public override bool Equals(object obj)
    {
      var fontStyle = obj as FontStyle;
      if (fontStyle == null)
      {
        return false;
      }
      else
      {
        return Equals(fontStyle);
      }
    }
    public bool Equals(FontStyle obj)
    {
      return this.Bold == obj.Bold && this.Italic == obj.Italic && this.Name == obj.Name && this.SizeInPoints == obj.SizeInPoints && this.Strikeout == obj.Strikeout && this.Underline == obj.Underline;
    }
    public override int GetHashCode()
    {
      return this.Bold.GetHashCode() ^ this.Italic.GetHashCode() ^ this.Name.GetHashCode() ^ this.SizeInPoints.GetHashCode() ^ this.Strikeout.GetHashCode() ^ this.Underline.GetHashCode();
    }
  }
}
