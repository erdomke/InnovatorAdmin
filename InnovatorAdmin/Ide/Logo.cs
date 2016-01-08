using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Logo
  {
    private Icon _icon;
    private Bitmap _image;

    public Icon Icon { get { return _icon; } }
    public Image Image { get { return _image; } }

    public Logo(Color value)
    {
      var existing = Properties.Resources.logo_black_opaque;
      _image = new Bitmap(existing.Width, existing.Height);
      using (var g = Graphics.FromImage(_image))
      {
        var attr = new ImageAttributes();

        float[][] colorMatrixElements = {
            new float[] {1,  0,  0,  0, 0},        // red scaling factor of 1
            new float[] {0,  1,  0,  0, 0},        // green scaling factor of 1
            new float[] {0,  0,  1,  0, 0},        // blue scaling factor of 1
            new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
            new float[] {value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, 0, 1}};    // three translations

        var colorMatrix = new ColorMatrix(colorMatrixElements);

        attr.SetColorMatrix(
           colorMatrix,
           ColorMatrixFlag.Default,
           ColorAdjustType.Bitmap);

        var rect = new Rectangle(0, 0, _image.Width, _image.Height);
        g.DrawImage(existing, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
      };
      _icon = Icon.FromHandle(_image.GetHicon());
    }
  }
}
