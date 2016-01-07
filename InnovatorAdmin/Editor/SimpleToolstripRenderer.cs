using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class SimpleToolstripRenderer
    : System.Windows.Forms.ToolStripProfessionalRenderer
  {

    public Color BaseColor
    {
      get { return ((SimpleColorTable)this.ColorTable).BaseColor; }
      set { ((SimpleColorTable)this.ColorTable).BaseColor = value; }
    }

    private class SimpleColorTable : ProfessionalColorTable
    {
      private Color baseColor;
      private Color pressedColor;
      private Color selectedColor;
      private Color separatorColor;
      private Color borderColor;

      public Color BaseColor
      {
        get { return baseColor; }
        set
        {
          baseColor = value;

          var lab = ColorTheory.ColorSpaceHelper.RGBtoLab(value);
          var lightLight = ColorTheory.ColorSpaceHelper.LabtoRGB((100 - lab.L) * 2.0 / 3 + lab.L, lab.A, lab.B);
          var light = ColorTheory.ColorSpaceHelper.LabtoRGB((100 - lab.L) * 1.0 / 3 + lab.L, lab.A, lab.B);
          var dark = ColorTheory.ColorSpaceHelper.LabtoRGB(lab.L * 2.0 / 3, lab.A, lab.B);
          var darkDark = ColorTheory.ColorSpaceHelper.LabtoRGB(lab.L * 1.0 / 3, lab.A, lab.B);

          selectedColor = light.ToColor();
          pressedColor = lightLight.ToColor();
          borderColor = dark.ToColor();

          if (lab.L < 50)
          {
            separatorColor = Color.White;
          }
          else
          {
            separatorColor = Color.Black;
          }
        }
      }
      public Color SeparatorOverride { get; set; }

      public SimpleColorTable()
      {
        this.BaseColor = Color.FromArgb(183, 28, 28);
      }

      public override Color ButtonCheckedGradientBegin { get { return selectedColor; } }
      public override Color ButtonCheckedGradientEnd { get { return selectedColor; } }
      public override Color ButtonCheckedGradientMiddle { get { return selectedColor; } }
      public override Color ButtonCheckedHighlight { get { return pressedColor; } }
      public override Color ButtonCheckedHighlightBorder { get { return borderColor; } }
      public override Color ButtonPressedBorder { get { return borderColor; } }
      public override Color ButtonPressedGradientBegin { get { return pressedColor; } }
      public override Color ButtonPressedGradientEnd { get { return pressedColor; } }
      public override Color ButtonPressedGradientMiddle { get { return pressedColor; } }
      public override Color ButtonPressedHighlight { get { return pressedColor; } }
      public override Color ButtonPressedHighlightBorder { get { return borderColor; } }
      public override Color ButtonSelectedBorder { get { return borderColor; } }
      public override Color ButtonSelectedGradientBegin { get { return selectedColor; } }
      public override Color ButtonSelectedGradientEnd { get { return selectedColor; } }
      public override Color ButtonSelectedGradientMiddle { get { return selectedColor; } }
      public override Color ButtonSelectedHighlight { get { return pressedColor; } }
      public override Color ButtonSelectedHighlightBorder { get { return borderColor; } }
      public override Color CheckBackground { get { return baseColor; } }
      public override Color CheckPressedBackground { get { return pressedColor; } }
      public override Color CheckSelectedBackground { get { return selectedColor; } }
      public override Color GripDark { get { return separatorColor; } }
      public override Color GripLight { get { return baseColor; } }
      public override Color ImageMarginGradientBegin { get { return Color.White; } }
      public override Color ImageMarginGradientEnd { get { return Color.White; } }
      public override Color ImageMarginGradientMiddle { get { return Color.White; } }
      public override Color ImageMarginRevealedGradientBegin { get { return Color.White; } }
      public override Color ImageMarginRevealedGradientEnd { get { return Color.White; } }
      public override Color ImageMarginRevealedGradientMiddle { get { return Color.White; } }
      public override Color MenuBorder { get { return Color.FromArgb(240, 240, 240); } }
      public override Color MenuItemBorder { get { return borderColor; } }
      public override Color MenuItemPressedGradientBegin { get { return pressedColor; } }
      public override Color MenuItemPressedGradientEnd { get { return pressedColor; } }
      public override Color MenuItemPressedGradientMiddle { get { return pressedColor; } }
      public override Color MenuItemSelected { get { return selectedColor; } }
      public override Color MenuItemSelectedGradientBegin { get { return selectedColor; } }
      public override Color MenuItemSelectedGradientEnd { get { return selectedColor; } }
      public override Color OverflowButtonGradientBegin { get { return baseColor; } }
      public override Color OverflowButtonGradientEnd { get { return baseColor; } }
      public override Color OverflowButtonGradientMiddle { get { return baseColor; } }
      public override Color SeparatorDark { get { return SeparatorOverride == Color.Empty ? separatorColor : SeparatorOverride; } }
      public override Color SeparatorLight { get { return baseColor; } }
      public override Color ToolStripDropDownBackground { get { return Color.White; } }
      public override Color ToolStripContentPanelGradientBegin { get { return baseColor; } }
      public override Color ToolStripContentPanelGradientEnd { get { return baseColor; } }
      public override Color ToolStripGradientBegin { get { return baseColor; } }
      public override Color ToolStripGradientEnd { get { return baseColor; } }
      public override Color ToolStripGradientMiddle { get { return baseColor; } }
      public override Color ToolStripBorder { get { return baseColor; } }
      public override Color ToolStripPanelGradientBegin { get { return baseColor; } }
      public override Color ToolStripPanelGradientEnd { get { return baseColor; } }
    }

    public SimpleToolstripRenderer()
      : base(new SimpleColorTable())
    {

    }

    //protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    //{
    //  RenderToolStripBackgroundInternal(e);
    //}
    //protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    //{
    //  if (!(e.ToolStrip is ToolStrip))
    //  {
    //    base.OnRenderToolStripBorder(e);
    //  }
    //}
    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      // Invert the text color if necessary
      if (!e.Item.IsOnDropDown && this.ColorTable.SeparatorDark == Color.White)
        e.TextColor = Color.FromArgb(255 - e.TextColor.R, 255 - e.TextColor.G, 255 - e.TextColor.B);
      //e.Item.Selected
      base.OnRenderItemText(e);
    }
    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
      try
      {
        if (e.Item.IsOnDropDown)
          ((SimpleColorTable)this.ColorTable).SeparatorOverride = Color.Black;
        base.OnRenderSeparator(e);
      }
      finally
      {
        ((SimpleColorTable)this.ColorTable).SeparatorOverride = Color.Empty;
      }
    }
  }
}
