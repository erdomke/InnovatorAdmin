using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class ColorDialog : FormBase
  {
    private Color _selectedColor = Color.Empty;

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }
    public Color SelectedColor
    {
      get { return _selectedColor; }
    }

    public ColorDialog()
    {
      InitializeComponent();

      this.TitleLabel = lblTitle;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.LeftBorderPanel = pnlLeft;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      var rows = _colors.Length;
      tblColors.RowStyles.Clear();
      for (var i = 0; i < rows; i++)
      {
        tblColors.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f / rows));
      }
      tblColors.RowCount = rows;

      var cols = _colors.Max(c => c.Length);
      tblColors.ColumnStyles.Clear();
      for (var i = 0; i < cols; i++)
      {
        tblColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f / cols));
      }
      tblColors.ColumnCount = cols;

      for (var i = 0; i < _colors.Length; i++)
      {
        for (var j = 0; j < _colors[i].Length; j++)
        {
          var btn = new Button();
          btn.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
          btn.FlatStyle = FlatStyle.Flat;
          btn.FlatAppearance.BorderSize = 1;
          btn.FlatAppearance.BorderColor = Color.Gray;
          btn.BackColor = _colors[i][j];
          btn.Margin = new Padding(0);
          btn.Click += (s, e) =>
          {
            _selectedColor = ((Button)s).BackColor;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
          };
          tblColors.Controls.Add(btn, j, i);
        }
      }
    }


    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

    }

    public DialogResult ShowDialog(IWin32Window owner, Rectangle bounds)
    {
      if (bounds != default(Rectangle))
      {
        this.StartPosition = FormStartPosition.Manual;
        var screenDim = SystemInformation.VirtualScreen;
        var newX = Math.Min(Math.Max(bounds.X, 0), screenDim.Width - this.DesktopBounds.Width);
        var newY = Math.Min(Math.Max(bounds.Y - 30, 0), screenDim.Height - this.DesktopBounds.Height);
        this.DesktopLocation = new Point(newX, newY);
      }
      return this.ShowDialog(owner);
    }

    private Color[][] _colors = new Color[][]
    {
      new Color[] {
        Color.FromArgb(255, 235, 238), // Red 50
        Color.FromArgb(255, 205, 210), // Red 100
        Color.FromArgb(239, 154, 154), // Red 200
        Color.FromArgb(229, 115, 115), // Red 300
        Color.FromArgb(239, 83, 80), // Red 400
        Color.FromArgb(244, 67, 54), // Red 500
        Color.FromArgb(229, 57, 53), // Red 600
        Color.FromArgb(211, 47, 47), // Red 700
        Color.FromArgb(198, 40, 40), // Red 800
        Color.FromArgb(183, 28, 28), // Red 900
        Color.FromArgb(255, 138, 128), // Red A100
        Color.FromArgb(255, 82, 82), // Red A200
        Color.FromArgb(255, 23, 68), // Red A400
        Color.FromArgb(213, 0, 0) // Red A700
      },
      new Color[]
      {
        Color.FromArgb(252, 228, 236), // Pink 50
        Color.FromArgb(248, 187, 208), // Pink 100
        Color.FromArgb(244, 143, 177), // Pink 200
        Color.FromArgb(240, 98, 146), // Pink 300
        Color.FromArgb(236, 64, 122), // Pink 400
        Color.FromArgb(233, 30, 99), // Pink 500
        Color.FromArgb(216, 27, 96), // Pink 600
        Color.FromArgb(194, 24, 91), // Pink 700
        Color.FromArgb(173, 20, 87), // Pink 800
        Color.FromArgb(136, 14, 79), // Pink 900
        Color.FromArgb(255, 128, 171), // Pink A100
        Color.FromArgb(255, 64, 129), // Pink A200
        Color.FromArgb(245, 0, 87), // Pink A400
        Color.FromArgb(197, 17, 98) // Pink A700
      },
      new Color[]
      {
        Color.FromArgb(243, 229, 245), // Purple 50
        Color.FromArgb(225, 190, 231), // Purple 100
        Color.FromArgb(206, 147, 216), // Purple 200
        Color.FromArgb(186, 104, 200), // Purple 300
        Color.FromArgb(171, 71, 188), // Purple 400
        Color.FromArgb(156, 39, 176), // Purple 500
        Color.FromArgb(142, 36, 170), // Purple 600
        Color.FromArgb(123, 31, 162), // Purple 700
        Color.FromArgb(106, 27, 154), // Purple 800
        Color.FromArgb(74, 20, 140), // Purple 900
        Color.FromArgb(234, 128, 252), // Purple A100
        Color.FromArgb(224, 64, 251), // Purple A200
        Color.FromArgb(213, 0, 249), // Purple A400
        Color.FromArgb(170, 0, 255) // Purple A700
      },
      new Color[]
      {
        Color.FromArgb(237, 231, 246), // Deep Purple 50
        Color.FromArgb(209, 196, 233), // Deep Purple 100
        Color.FromArgb(179, 157, 219), // Deep Purple 200
        Color.FromArgb(149, 117, 205), // Deep Purple 300
        Color.FromArgb(126, 87, 194), // Deep Purple 400
        Color.FromArgb(103, 58, 183), // Deep Purple 500
        Color.FromArgb(94, 53, 177), // Deep Purple 600
        Color.FromArgb(81, 45, 168), // Deep Purple 700
        Color.FromArgb(69, 39, 160), // Deep Purple 800
        Color.FromArgb(49, 27, 146), // Deep Purple 900
        Color.FromArgb(179, 136, 255), // Deep Purple A100
        Color.FromArgb(124, 77, 255), // Deep Purple A200
        Color.FromArgb(101, 31, 255), // Deep Purple A400
        Color.FromArgb(98, 0, 234) // Deep Purple A700
      },
      new Color[]
      {
        Color.FromArgb(232, 234, 246), // Indigo 50
        Color.FromArgb(197, 202, 233), // Indigo 100
        Color.FromArgb(159, 168, 218), // Indigo 200
        Color.FromArgb(121, 134, 203), // Indigo 300
        Color.FromArgb(92, 107, 192), // Indigo 400
        Color.FromArgb(63, 81, 181), // Indigo 500
        Color.FromArgb(57, 73, 171), // Indigo 600
        Color.FromArgb(48, 63, 159), // Indigo 700
        Color.FromArgb(40, 53, 147), // Indigo 800
        Color.FromArgb(26, 35, 126), // Indigo 900
        Color.FromArgb(140, 158, 255), // Indigo A100
        Color.FromArgb(83, 109, 254), // Indigo A200
        Color.FromArgb(61, 90, 254), // Indigo A400
        Color.FromArgb(48, 79, 254) // Indigo A700
      },
      new Color[]
      {
        Color.FromArgb(227, 242, 253), // Blue 50
        Color.FromArgb(187, 222, 251), // Blue 100
        Color.FromArgb(144, 202, 249), // Blue 200
        Color.FromArgb(100, 181, 246), // Blue 300
        Color.FromArgb(66, 165, 245), // Blue 400
        Color.FromArgb(33, 150, 243), // Blue 500
        Color.FromArgb(30, 136, 229), // Blue 600
        Color.FromArgb(25, 118, 210), // Blue 700
        Color.FromArgb(21, 101, 192), // Blue 800
        Color.FromArgb(13, 71, 161), // Blue 900
        Color.FromArgb(130, 177, 255), // Blue A100
        Color.FromArgb(68, 138, 255), // Blue A200
        Color.FromArgb(41, 121, 255), // Blue A400
        Color.FromArgb(41, 98, 255) // Blue A700
      },
      new Color[]
      {
        Color.FromArgb(225, 245, 254), // Light Blue 50
        Color.FromArgb(179, 229, 252), // Light Blue 100
        Color.FromArgb(129, 212, 250), // Light Blue 200
        Color.FromArgb(79, 195, 247), // Light Blue 300
        Color.FromArgb(41, 182, 246), // Light Blue 400
        Color.FromArgb(3, 169, 244), // Light Blue 500
        Color.FromArgb(3, 155, 229), // Light Blue 600
        Color.FromArgb(2, 136, 209), // Light Blue 700
        Color.FromArgb(2, 119, 189), // Light Blue 800
        Color.FromArgb(1, 87, 155), // Light Blue 900
        Color.FromArgb(128, 216, 255), // Light Blue A100
        Color.FromArgb(64, 196, 255), // Light Blue A200
        Color.FromArgb(0, 176, 255), // Light Blue A400
        Color.FromArgb(0, 145, 234) // Light Blue A700
      },
      new Color[]
      {
        Color.FromArgb(224, 247, 250), // Cyan 50
        Color.FromArgb(178, 235, 242), // Cyan 100
        Color.FromArgb(128, 222, 234), // Cyan 200
        Color.FromArgb(77, 208, 225), // Cyan 300
        Color.FromArgb(38, 198, 218), // Cyan 400
        Color.FromArgb(0, 188, 212), // Cyan 500
        Color.FromArgb(0, 172, 193), // Cyan 600
        Color.FromArgb(0, 151, 167), // Cyan 700
        Color.FromArgb(0, 131, 143), // Cyan 800
        Color.FromArgb(0, 96, 100), // Cyan 900
        Color.FromArgb(132, 255, 255), // Cyan A100
        Color.FromArgb(24, 255, 255), // Cyan A200
        Color.FromArgb(0, 229, 255), // Cyan A400
        Color.FromArgb(0, 184, 212) // Cyan A700
      },
      new Color[]
      {
        Color.FromArgb(224, 242, 241), // Teal 50
        Color.FromArgb(178, 223, 219), // Teal 100
        Color.FromArgb(128, 203, 196), // Teal 200
        Color.FromArgb(77, 182, 172), // Teal 300
        Color.FromArgb(38, 166, 154), // Teal 400
        Color.FromArgb(0, 150, 136), // Teal 500
        Color.FromArgb(0, 137, 123), // Teal 600
        Color.FromArgb(0, 121, 107), // Teal 700
        Color.FromArgb(0, 105, 92), // Teal 800
        Color.FromArgb(0, 77, 64), // Teal 900
        Color.FromArgb(167, 255, 235), // Teal A100
        Color.FromArgb(100, 255, 218), // Teal A200
        Color.FromArgb(29, 233, 182), // Teal A400
        Color.FromArgb(0, 191, 165) // Teal A700
      },
      new Color[]
      {
        Color.FromArgb(232, 245, 233), // Green 50
        Color.FromArgb(200, 230, 201), // Green 100
        Color.FromArgb(165, 214, 167), // Green 200
        Color.FromArgb(129, 199, 132), // Green 300
        Color.FromArgb(102, 187, 106), // Green 400
        Color.FromArgb(76, 175, 80), // Green 500
        Color.FromArgb(67, 160, 71), // Green 600
        Color.FromArgb(56, 142, 60), // Green 700
        Color.FromArgb(46, 125, 50), // Green 800
        Color.FromArgb(27, 94, 32), // Green 900
        Color.FromArgb(185, 246, 202), // Green A100
        Color.FromArgb(105, 240, 174), // Green A200
        Color.FromArgb(0, 230, 118), // Green A400
        Color.FromArgb(0, 200, 83) // Green A700
      },
      new Color[]
      {
        Color.FromArgb(241, 248, 233), // Light Green 50
        Color.FromArgb(220, 237, 200), // Light Green 100
        Color.FromArgb(197, 225, 165), // Light Green 200
        Color.FromArgb(174, 213, 129), // Light Green 300
        Color.FromArgb(156, 204, 101), // Light Green 400
        Color.FromArgb(139, 195, 74), // Light Green 500
        Color.FromArgb(124, 179, 66), // Light Green 600
        Color.FromArgb(104, 159, 56), // Light Green 700
        Color.FromArgb(85, 139, 47), // Light Green 800
        Color.FromArgb(51, 105, 30), // Light Green 900
        Color.FromArgb(204, 255, 144), // Light Green A100
        Color.FromArgb(178, 255, 89), // Light Green A200
        Color.FromArgb(118, 255, 3), // Light Green A400
        Color.FromArgb(100, 221, 23) // Light Green A700
      },
      new Color[]
      {
        Color.FromArgb(249, 251, 231), // Lime 50
        Color.FromArgb(240, 244, 195), // Lime 100
        Color.FromArgb(230, 238, 156), // Lime 200
        Color.FromArgb(220, 231, 117), // Lime 300
        Color.FromArgb(212, 225, 87), // Lime 400
        Color.FromArgb(205, 220, 57), // Lime 500
        Color.FromArgb(192, 202, 51), // Lime 600
        Color.FromArgb(175, 180, 43), // Lime 700
        Color.FromArgb(158, 157, 36), // Lime 800
        Color.FromArgb(130, 119, 23), // Lime 900
        Color.FromArgb(244, 255, 129), // Lime A100
        Color.FromArgb(238, 255, 65), // Lime A200
        Color.FromArgb(198, 255, 0), // Lime A400
        Color.FromArgb(174, 234, 0) // Lime A700
      },
      new Color[]
      {
        Color.FromArgb(255, 253, 231), // Yellow 50
        Color.FromArgb(255, 249, 196), // Yellow 100
        Color.FromArgb(255, 245, 157), // Yellow 200
        Color.FromArgb(255, 241, 118), // Yellow 300
        Color.FromArgb(255, 238, 88), // Yellow 400
        Color.FromArgb(255, 235, 59), // Yellow 500
        Color.FromArgb(253, 216, 53), // Yellow 600
        Color.FromArgb(251, 192, 45), // Yellow 700
        Color.FromArgb(249, 168, 37), // Yellow 800
        Color.FromArgb(245, 127, 23), // Yellow 900
        Color.FromArgb(255, 255, 141), // Yellow A100
        Color.FromArgb(255, 255, 0), // Yellow A200
        Color.FromArgb(255, 234, 0), // Yellow A400
        Color.FromArgb(255, 214, 0) // Yellow A700
      },
      new Color[]
      {
        Color.FromArgb(255, 248, 225), // Amber 50
        Color.FromArgb(255, 236, 179), // Amber 100
        Color.FromArgb(255, 224, 130), // Amber 200
        Color.FromArgb(255, 213, 79), // Amber 300
        Color.FromArgb(255, 202, 40), // Amber 400
        Color.FromArgb(255, 193, 7), // Amber 500
        Color.FromArgb(255, 179, 0), // Amber 600
        Color.FromArgb(255, 160, 0), // Amber 700
        Color.FromArgb(255, 143, 0), // Amber 800
        Color.FromArgb(255, 111, 0), // Amber 900
        Color.FromArgb(255, 229, 127), // Amber A100
        Color.FromArgb(255, 215, 64), // Amber A200
        Color.FromArgb(255, 196, 0), // Amber A400
        Color.FromArgb(255, 171, 0) // Amber A700
      },
      new Color[]
      {
        Color.FromArgb(255, 243, 224), // Orange 50
        Color.FromArgb(255, 224, 178), // Orange 100
        Color.FromArgb(255, 204, 128), // Orange 200
        Color.FromArgb(255, 183, 77), // Orange 300
        Color.FromArgb(255, 167, 38), // Orange 400
        Color.FromArgb(255, 152, 0), // Orange 500
        Color.FromArgb(251, 140, 0), // Orange 600
        Color.FromArgb(245, 124, 0), // Orange 700
        Color.FromArgb(239, 108, 0), // Orange 800
        Color.FromArgb(230, 81, 0), // Orange 900
        Color.FromArgb(255, 209, 128), // Orange A100
        Color.FromArgb(255, 171, 64), // Orange A200
        Color.FromArgb(255, 145, 0), // Orange A400
        Color.FromArgb(255, 109, 0) // Orange A700
      },
      new Color[]
      {
        Color.FromArgb(251, 233, 231), // Deep Orange 50
        Color.FromArgb(255, 204, 188), // Deep Orange 100
        Color.FromArgb(255, 171, 145), // Deep Orange 200
        Color.FromArgb(255, 138, 101), // Deep Orange 300
        Color.FromArgb(255, 112, 67), // Deep Orange 400
        Color.FromArgb(255, 87, 34), // Deep Orange 500
        Color.FromArgb(244, 81, 30), // Deep Orange 600
        Color.FromArgb(230, 74, 25), // Deep Orange 700
        Color.FromArgb(216, 67, 21), // Deep Orange 800
        Color.FromArgb(191, 54, 12), // Deep Orange 900
        Color.FromArgb(255, 158, 128), // Deep Orange A100
        Color.FromArgb(255, 110, 64), // Deep Orange A200
        Color.FromArgb(255, 61, 0), // Deep Orange A400
        Color.FromArgb(221, 44, 0) // Deep Orange A700
      },
      new Color[]
      {
        Color.FromArgb(239, 235, 233), // Brown 50
        Color.FromArgb(215, 204, 200), // Brown 100
        Color.FromArgb(188, 170, 164), // Brown 200
        Color.FromArgb(161, 136, 127), // Brown 300
        Color.FromArgb(141, 110, 99), // Brown 400
        Color.FromArgb(121, 85, 72), // Brown 500
        Color.FromArgb(109, 76, 65), // Brown 600
        Color.FromArgb(93, 64, 55), // Brown 700
        Color.FromArgb(78, 52, 46), // Brown 800
        Color.FromArgb(62, 39, 35) // Brown 900
      },
      new Color[]
      {
        Color.FromArgb(250, 250, 250), // Grey 50
        Color.FromArgb(245, 245, 245), // Grey 100
        Color.FromArgb(238, 238, 238), // Grey 200
        Color.FromArgb(224, 224, 224), // Grey 300
        Color.FromArgb(189, 189, 189), // Grey 400
        Color.FromArgb(158, 158, 158), // Grey 500
        Color.FromArgb(117, 117, 117), // Grey 600
        Color.FromArgb(97, 97, 97), // Grey 700
        Color.FromArgb(66, 66, 66), // Grey 800
        Color.FromArgb(33, 33, 33), // Grey 900
        Color.FromArgb(255, 255, 255), // White
        Color.FromArgb(0, 0, 0) // Black
      },
      new Color[]
      {
        Color.FromArgb(236, 239, 241), // Blue Grey 50
        Color.FromArgb(207, 216, 220), // Blue Grey 100
        Color.FromArgb(176, 190, 197), // Blue Grey 200
        Color.FromArgb(144, 164, 174), // Blue Grey 300
        Color.FromArgb(120, 144, 156), // Blue Grey 400
        Color.FromArgb(96, 125, 139), // Blue Grey 500
        Color.FromArgb(84, 110, 122), // Blue Grey 600
        Color.FromArgb(69, 90, 100), // Blue Grey 700
        Color.FromArgb(55, 71, 79), // Blue Grey 800
        Color.FromArgb(38, 50, 56) // Blue Grey 900
      }
    };

    private void btnCustom_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new System.Windows.Forms.ColorDialog())
        {
          dialog.FullOpen = true;
          if (dialog.ShowDialog(this) == DialogResult.OK)
          {
            _selectedColor = dialog.Color;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
