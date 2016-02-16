using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Css.Model
{
  public class MediaProperty : IToString
  {
    public string Name { get; set; }

  
    public override string ToString()
    {
 	     return ToString(false);
    }
    public virtual string ToString(bool friendlyFormat, int indentation = 0)
    {
      return "(" + this.Name + ")";
    }
  }

  public class MediaPropertyPlain : MediaProperty
  {
    public Term Value { get; set; }

    public override string ToString(bool friendlyFormat, int indentation = 0)
    {
      return "(" + this.Name + (friendlyFormat ? " : " : ":") + this.Value + ")";
    }
  }
  
  public class MediaPropertyRange : MediaProperty
  {
    public Term LowerBound { get; set; }
    public string LowerCompare { get; set; }
    public Term UpperBound { get; set; }
    public string UpperCompare { get; set; }

    public override string ToString(bool friendlyFormat, int indentation = 0)
    {
      var builder = new StringBuilder();

      builder.Append('(');
      if (LowerBound != null)
      {
        builder.Append(LowerBound);
        if (friendlyFormat) builder.Append(' ');
        builder.Append(LowerCompare);
        if (friendlyFormat) builder.Append(' ');
      }
      builder.Append(Name);
      if (UpperBound != null)
      {
        if (friendlyFormat) builder.Append(' ');
        builder.Append(UpperCompare);
        if (friendlyFormat) builder.Append(' ');
        builder.Append(UpperBound);
      }
      builder.Append(')');
      return builder.ToString();
    }
  }
}
