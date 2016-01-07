using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public class SplitContainerTheme : SplitContainer
  {
    private const int PadDist = 3;

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      using (var brush = new SolidBrush(Color.FromArgb(235, 235, 235)))
      using (var pen = new Pen(brush, 1))
      {
        // Draw gripper dots in center
        var rect = this.SplitterRectangle;
        if (Orientation == System.Windows.Forms.Orientation.Horizontal)
        {
          e.Graphics.DrawLine(pen, rect.Left + PadDist, rect.Top + rect.Height / 2
            , rect.Right - PadDist, rect.Top + rect.Height / 2);
        }
        else
        {
          e.Graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + PadDist
            , rect.Left + rect.Width / 2, rect.Bottom - PadDist);
        }

      }
    }
  }
}
