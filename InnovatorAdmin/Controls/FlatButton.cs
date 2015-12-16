using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public enum FlatButtonTheme
  {
    Red,
    LightGray,
    DarkGray
  }

  public class FlatButton : Button
  {
    private bool _autoSize;
    private Orientation _orientation = Orientation.Horizontal;
    private FlatButtonTheme _theme = FlatButtonTheme.LightGray;

    public override bool AutoSize
    {
      get { return _autoSize; }
      set
      {
        _autoSize = value;
        SetSize();
      }
    }
    [Category("Appearance")]
    public Orientation Orientation
    {
      get { return _orientation; }
      set
      {
        _orientation = value;
        SetSize();
      }
    }
    [Category("Appearance")]
    public FlatButtonTheme Theme
    {
      get { return _theme; }
      set
      {
        switch (value)
        {
          case FlatButtonTheme.Red:
            this.BackColor = System.Drawing.Color.FromArgb(204, 0, 51);
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(119, 3, 32);
            this.ForeColor = System.Drawing.Color.White;
            break;
          case FlatButtonTheme.DarkGray:
            break;
          default:
            this.BackColor = System.Drawing.Color.FromArgb(221, 221, 221);
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(196, 196, 196);
            this.ForeColor = System.Drawing.Color.Black;
            break;
        }
        this.FlatAppearance.MouseDownBackColor = this.FlatAppearance.MouseOverBackColor;
        _theme = value;
      }
    }
    public override string Text
    {
      get { return base.Text; }
      set { base.Text = value; SetSize(); }
    }

    public FlatButton()
    {
      this.AutoSize = true;
      this.FlatAppearance.BorderSize = 0;
      this.Padding = new System.Windows.Forms.Padding(2);
      this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.UseVisualStyleBackColor = false;
      this.Theme = FlatButtonTheme.LightGray;
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
      base.OnEnabledChanged(e);
      if (this.Enabled)
      {
        this.Theme = _theme;
      }
      else
      {
        switch (_theme)
        {
          case FlatButtonTheme.Red:
            this.BackColor = System.Drawing.Color.FromArgb(234, 186, 198);
            break;
          case FlatButtonTheme.DarkGray:
            break;
          default:
            this.BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
            break;
        }
      }
    }
    protected override void OnPaint(PaintEventArgs pevent)
    {
      base.OnPaint(pevent);
      if (_orientation == Orientation.Vertical)
      {

        var backColor = ClientRectangle.Contains(PointToClient(Control.MousePosition))
          ? ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) > 0
            ? this.FlatAppearance.MouseDownBackColor
            : this.FlatAppearance.MouseOverBackColor)
          : this.BackColor;

        using (var back = new SolidBrush(backColor))
        using (var fore = new SolidBrush(this.ForeColor))
        {
          var fmt = new StringFormat();
          switch (this.TextAlign)
          {
            case ContentAlignment.BottomCenter:
              fmt.Alignment = StringAlignment.Center;
              fmt.LineAlignment = StringAlignment.Far;
              break;
            case ContentAlignment.BottomLeft:
              fmt.Alignment = StringAlignment.Near;
              fmt.LineAlignment = StringAlignment.Far;
              break;
            case ContentAlignment.BottomRight:
              fmt.Alignment = StringAlignment.Far;
              fmt.LineAlignment = StringAlignment.Far;
              break;
            case ContentAlignment.MiddleCenter:
              fmt.Alignment = StringAlignment.Center;
              fmt.LineAlignment = StringAlignment.Center;
              break;
            case ContentAlignment.MiddleLeft:
              fmt.Alignment = StringAlignment.Near;
              fmt.LineAlignment = StringAlignment.Center;
              break;
            case ContentAlignment.MiddleRight:
              fmt.Alignment = StringAlignment.Far;
              fmt.LineAlignment = StringAlignment.Center;
              break;
            case ContentAlignment.TopCenter:
              fmt.Alignment = StringAlignment.Center;
              fmt.LineAlignment = StringAlignment.Near;
              break;
            case ContentAlignment.TopLeft:
              fmt.Alignment = StringAlignment.Near;
              fmt.LineAlignment = StringAlignment.Near;
              break;
            case ContentAlignment.TopRight:
              fmt.Alignment = StringAlignment.Far;
              fmt.LineAlignment = StringAlignment.Near;
              break;
          }

          pevent.Graphics.FillRectangle(back, pevent.ClipRectangle);
          pevent.Graphics.TranslateTransform(Width, 0);
          pevent.Graphics.RotateTransform(90);
          pevent.Graphics.DrawString(this.Text, this.Font, fore, new Rectangle(0, 0, Height, Width), fmt);
        }
      }
    }

    private void SetSize()
    {
      if (_orientation == Orientation.Horizontal)
        base.AutoSize = _autoSize;
      else
        base.AutoSize = false;

      if (_autoSize && _orientation == Orientation.Vertical)
      {
        var content = TextRenderer.MeasureText(this.Text, this.Font);
        var vert = new Size(content.Height, content.Width);
        this.Size = Size.Add(vert, this.Padding.Size);
      }
    }
  }
}
