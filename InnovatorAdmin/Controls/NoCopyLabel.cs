using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
// Helper class to prevent copying the Title into the clipboard on double click / maximize
//
namespace InnovatorAdmin.Controls
{
  class NoCopyLabel : Label
  {
    private const int WM_LBUTTONDBLCLK = 0x203;

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == WM_LBUTTONDBLCLK)
      {
        string sSaved = Clipboard.GetText();
        System.Drawing.Image iSaved = Clipboard.GetImage();
        base.WndProc(ref m);
        if (iSaved != null) Clipboard.SetImage(iSaved);
        if (!string.IsNullOrEmpty(sSaved)) Clipboard.SetText(sSaved);
      }
      else
      {
        base.WndProc(ref m);
      }
    }
  }
}