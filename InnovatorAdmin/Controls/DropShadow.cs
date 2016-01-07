using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class DropShadow : Panel
  {
    public int ShadowExtent { get; set; }

    public DropShadow() : base()
    {
      DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      var max = ShadowExtent <= 0 ? int.MaxValue : ShadowExtent;
      var extent = Math.Min(this.Height, max);
      using (var linGrBrush = new LinearGradientBrush(
         new Point(0, 0),
         new Point(0, extent),
         Color.DarkGray,   // Opaque red
         Color.White))
      {
        e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
        var newClip = Rectangle.Intersect(new Rectangle(0, 0, this.Width, extent), e.ClipRectangle);
        e.Graphics.FillRectangle(linGrBrush, newClip);
      }
    }
  }
}
