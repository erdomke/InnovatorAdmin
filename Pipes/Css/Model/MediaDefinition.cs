using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Css.Model
{
  public class MediaDefinition : IToString
  {
    private List<MediaProperty> _properties = new List<MediaProperty>();

    public MediaTypeModifier Modifier { get; set; }
    public IList<MediaProperty> Properties { get { return _properties; }}
    public MediaType Type { get; set; }

    public MediaDefinition()
    {
      this.Modifier = MediaTypeModifier.None;
      this.Type = MediaType.All;
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool friendlyFormat, int indentation = 0)
    {
      var builder = new StringBuilder();
      switch (this.Modifier)
      {
        case MediaTypeModifier.Not:
          builder.Append("not");
          break;
        case MediaTypeModifier.Only:
          builder.Append("only");
          break;
      }
      switch (this.Type)
      {
        case MediaType.Print:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("print");
          break;
        case MediaType.Screen:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("screen");
          break;
        case MediaType.Speech:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("speech");
          break;
        case MediaType.Braille:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("braille");
          break;
        case MediaType.Embossed:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("embossed");
          break;
        case MediaType.Handheld:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("handheld");
          break;
        case MediaType.Projection:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("projection");
          break;
        case MediaType.Tty:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("tty");
          break;
        case MediaType.Tv:
          if (builder.Length > 0) builder.Append(' ');
          builder.Append("tv");
          break;
      }

      if (_properties.Any())
      {
        if (builder.Length > 0) builder.Append(" and ");
        _properties.GroupConcat(builder, " and ", p => p.ToString(friendlyFormat, indentation));
      }

      return builder.ToString();
    }
  }
}
