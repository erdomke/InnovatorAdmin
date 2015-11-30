using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Aras.Tools.InnovatorAdmin
{
  public class SimpleToolstripRenderer
    : System.Windows.Forms.ToolStripProfessionalRenderer
  {
    protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
    {
      base.OnRenderToolStripBackground(e);
      using (var br = new SolidBrush(e.ToolStrip.BackColor))
      {
        e.Graphics.FillRectangle(br, e.AffectedBounds);
      }
    }
  }
}
