using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class LightBox : Form
  {
    public LightBox()
    {
      InitializeComponent();

      FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      StartPosition = FormStartPosition.Manual;
      MaximizeBox = false;
      MinimizeBox = false;
      ShowInTaskbar = false;
      BackColor = Color.Black;
      TransparencyKey = Color.Magenta;
      Opacity = 0.5;
    }

    private Control _parent;
    private Control ParentCtrl
    {
      get { return _parent; }
      set
      {
        if (_parent != null)
        {
          _parent.Move -= _parent_Move;
          _parent.ParentChanged -= _parent_ParentChanged;
          _parent.Resize -= _parent_Resize;
          _parent.VisibleChanged -= _parent_VisibleChanged;
        }
        _parent = value;
        if (_parent != null)
        {
          _parent.Move += _parent_Move;
          _parent.ParentChanged += _parent_ParentChanged;
          _parent.Resize += _parent_Resize;
          _parent.VisibleChanged += _parent_VisibleChanged;
        }
      }
    }
    private Control _highlight;
    private Control Highlight
    {
      get { return _highlight; }
      set
      {
        if (_highlight != null)
        {
          _highlight.Move -= _highlight_Move;
          _highlight.Resize -= _highlight_Resize;
          _highlight.VisibleChanged -= _highlight_VisibleChanged;
        }
        _highlight = value;
        if (_highlight != null)
        {
          _highlight.Move += _highlight_Move;
          _highlight.Resize += _highlight_Resize;
          _highlight.VisibleChanged += _highlight_VisibleChanged;
        }
      }
    }
    private Control _window;
    private Control Window
    {
      get { return _window; }
      set
      {
        if (_window != null)
        {
          _window.Move -= _window_Move;
          _window.ParentChanged -= _window_ParentChanged;
        }
        _window = value;
        if (_window != null)
        {
          _window.Move += _window_Move;
          _window.ParentChanged += _window_ParentChanged;
        }
      }

    }

    public new void Show(Control parent, Control highlight)
    {
      base.Show(parent);
      ParentCtrl = parent;
      Highlight = highlight;
      Window = GetRootParent(ParentCtrl);
      MoveForm();
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
      base.OnPaint(e);
      try
      {
        if (Highlight != null)
        {
          var parentPt = LocationToScreen(ParentCtrl);
          var highlightPt = LocationToScreen(Highlight);
          var rect = Rectangle.Intersect(e.ClipRectangle, new Rectangle(highlightPt.X - parentPt.X, highlightPt.Y - parentPt.Y, Highlight.Width, Highlight.Height));
          e.Graphics.FillRectangle(Brushes.Magenta, rect);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void _window_Move(object sender, System.EventArgs e)
    {
      try
      {
        MoveForm();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _window_ParentChanged(object sender, System.EventArgs e)
    {
      try
      {
        var newWin = GetRootParent(Window);
        Window = null;
        //Try to clean up event handlers and any potential memory leaks
        Window = newWin;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private Control GetRootParent(Control ctrl)
    {
      if (ctrl == null)
        return null;
      var parent = ctrl;
      while (parent.Parent != null)
      {
        parent = parent.Parent;
      }
      return parent;
    }
    private void _parent_Move(object sender, System.EventArgs e)
    {
      try
      {
        MoveForm();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _parent_ParentChanged(object sender, System.EventArgs e)
    {
      try
      {
        Window = null;
        Window = GetRootParent(ParentCtrl);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _parent_Resize(object sender, System.EventArgs e)
    {
      try
      {
        MoveForm();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _parent_VisibleChanged(object sender, System.EventArgs e)
    {
      try
      {
        Visible = ParentCtrl.Visible;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _highlight_Move(object sender, System.EventArgs e)
    {
      try
      {
        this.Invalidate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _highlight_Resize(object sender, System.EventArgs e)
    {
      try
      {
        this.Invalidate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
    private void _highlight_VisibleChanged(object sender, System.EventArgs e)
    {
      try
      {
        this.Invalidate();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void MoveForm()
    {
      Location = LocationToScreen(ParentCtrl);
      Size = ParentCtrl.Size;
    }

    private Point LocationToScreen(Control ctrl)
    {
      if (ctrl.TopLevelControl == ctrl)
        return ctrl.Location;
      return ctrl.PointToScreen(ctrl.Location);
    }
  }
}
