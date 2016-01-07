using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  /// <summary>
  /// Based on http://customerborderform.codeplex.com/
  /// </summary>
  public class FormBase : Form
  {
    public void DecorationMouseDown(HitTestValues hit, Point p)
    {
      NativeMethods.ReleaseCapture();
      var pt = new POINTS { X = (short)p.X, Y = (short)p.Y };
      NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_NCLBUTTONDOWN, (int)hit, pt);
    }

    public void DecorationMouseDown(HitTestValues hit)
    {
      DecorationMouseDown(hit, MousePosition);
    }

    public void DecorationMouseUp(HitTestValues hit, Point p)
    {
      NativeMethods.ReleaseCapture();
      var pt = new POINTS { X = (short)p.X, Y = (short)p.Y };
      NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_NCLBUTTONUP, (int)hit, pt);
    }

    public void DecorationMouseUp(HitTestValues hit)
    {
      DecorationMouseUp(hit, MousePosition);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);

      if (!DesignMode)
        SetWindowRegion(Handle, 0, 0, Width, Height);
    }

    protected void ShowSystemMenu(MouseButtons buttons)
    {
      ShowSystemMenu(buttons, MousePosition);
    }

    protected static int MakeLong(short lowPart, short highPart)
    {
      return (int)(((ushort)lowPart) | (uint)(highPart << 16));
    }

    protected void ShowSystemMenu(MouseButtons buttons, Point pos)
    {
      NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_SYSMENU, 0, MakeLong((short)pos.X, (short)pos.Y));
    }

    protected override void WndProc(ref Message m)
    {
      if (DesignMode)
      {
        base.WndProc(ref m);
        return;
      }

      switch (m.Msg)
      {
        case (int)WindowMessages.WM_NCCALCSIZE:
          {
            // Provides new coordinates for the window client area.
            WmNCCalcSize(ref m);
            break;
          }
        case (int)WindowMessages.WM_NCPAINT:
          {
            // Here should all our painting occur, but...
            WmNCPaint(ref m);
            break;
          }
        case (int)WindowMessages.WM_NCACTIVATE:
          {
            // ... WM_NCACTIVATE does some painting directly
            // without bothering with WM_NCPAINT ...
            WmNCActivate(ref m);
            break;
          }
        case (int)WindowMessages.WM_SETTEXT:
          {
            // ... and some painting is required in here as well
            WmSetText(ref m);
            break;
          }
        case (int)WindowMessages.WM_WINDOWPOSCHANGED:
          {
            WmWindowPosChanged(ref m);
            break;
          }
        case 174: // ignore magic message number
          {
            break;
          }
        default:
          {
            base.WndProc(ref m);
            break;
          }
      }
    }

    private void SetWindowRegion(IntPtr hwnd, int left, int top, int right, int bottom)
    {
      var hrg = new HandleRef((object)this, NativeMethods.CreateRectRgn(0, 0, 0, 0));
      var r = NativeMethods.GetWindowRgn(hwnd, hrg.Handle);
      RECT box;
      NativeMethods.GetRgnBox(hrg.Handle, out box);
      if (box.left != left || box.top != top || box.right != right || box.bottom != bottom)
      {
        var hr = new HandleRef((object)this, NativeMethods.CreateRectRgn(left, top, right, bottom));
        NativeMethods.SetWindowRgn(hwnd, hr.Handle, NativeMethods.IsWindowVisible(hwnd));
      }
    }

    public FormWindowState MinMaxState
    {
      get
      {
        var s = NativeMethods.GetWindowLong(Handle, NativeConstants.GWL_STYLE);
        var max = (s & (int)WindowStyle.WS_MAXIMIZE) > 0;
        if (max) return FormWindowState.Maximized;
        var min = (s & (int)WindowStyle.WS_MINIMIZE) > 0;
        if (min) return FormWindowState.Minimized;
        return FormWindowState.Normal;
      }
    }

    private void WmWindowPosChanged(ref Message m)
    {
      DefWndProc(ref m);
      UpdateBounds();
      var pos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));
      SetWindowRegion(m.HWnd, 0, 0, pos.cx, pos.cy);
      m.Result = NativeConstants.TRUE;
    }

    private void WmNCCalcSize(ref Message m)
    {
      // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowmessages/wm_nccalcsize.asp
      // http://groups.google.pl/groups?selm=OnRNaGfDEHA.1600%40tk2msftngp13.phx.gbl

      var r = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
      var max = MinMaxState == FormWindowState.Maximized;

      if (max)
      {
        var x = NativeMethods.GetSystemMetrics(NativeConstants.SM_CXSIZEFRAME);
        var y = NativeMethods.GetSystemMetrics(NativeConstants.SM_CYSIZEFRAME);
        var p = NativeMethods.GetSystemMetrics(NativeConstants.SM_CXPADDEDBORDER);
        var w = x + p;
        var h = y + p;

        r.left += w;
        r.top += h;
        r.right -= w;
        r.bottom -= h;

        var appBarData = new APPBARDATA();
        appBarData.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
        var autohide = (NativeMethods.SHAppBarMessage(NativeConstants.ABM_GETSTATE, ref appBarData) & NativeConstants.ABS_AUTOHIDE) != 0;
        if (autohide) r.bottom -= 1;

        Marshal.StructureToPtr(r, m.LParam, true);
      }

      m.Result = IntPtr.Zero;
    }

    private void WmNCPaint(ref Message msg)
    {
      // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/pantdraw_8gdw.asp
      // example in q. 2.9 on http://www.syncfusion.com/FAQ/WindowsForms/FAQ_c41c.aspx#q1026q

      // The WParam contains handle to clipRegion or 1 if entire window should be repainted
      //PaintNonClientArea(msg.HWnd, (IntPtr)msg.WParam);

      // we handled everything
      msg.Result = NativeConstants.TRUE;
    }

    private void WmSetText(ref Message msg)
    {
      // allow the system to receive the new window title
      DefWndProc(ref msg);

      // repaint title bar
      //PaintNonClientArea(msg.HWnd, (IntPtr)1);
    }

    private void WmNCActivate(ref Message msg)
    {
      // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowmessages/wm_ncactivate.asp

      bool active = (msg.WParam == NativeConstants.TRUE);

      if (MinMaxState == FormWindowState.Minimized)
        DefWndProc(ref msg);
      else
      {
        // repaint title bar
        //PaintNonClientArea(msg.HWnd, (IntPtr)1);

        // allow to deactivate window
        msg.Result = NativeConstants.TRUE;
      }
    }

    #region Styling

    private Color hoverTextColor = Color.FromArgb(62, 109, 181);

    public Color HoverTextColor
    {
      get { return hoverTextColor; }
      set { hoverTextColor = value; }
    }

    private Color downTextColor = Color.FromArgb(25, 71, 138);

    public Color DownTextColor
    {
      get { return downTextColor; }
      set { downTextColor = value; }
    }

    private Color hoverBackColor = Color.FromArgb(213, 225, 242);

    public Color HoverBackColor
    {
      get { return hoverBackColor; }
      set { hoverBackColor = value; }
    }

    private Color downBackColor = Color.FromArgb(163, 189, 227);

    public Color DownBackColor
    {
      get { return downBackColor; }
      set { downBackColor = value; }
    }

    private Color normalBackColor = Color.Transparent;

    public Color NormalBackColor
    {
      get { return normalBackColor; }
      set { normalBackColor = value; }
    }

    private Color activeTextColor = Color.FromArgb(68, 68, 68);

    public Color ActiveTextColor
    {
      get { return activeTextColor; }
      set
      {
        activeTextColor = value;
        foreach (var control in new[] { MinimizeLabel, MaximizeLabel, CloseLabel, TitleLabel }.Where(c => c != null))
        {
          SetLabelColors(control, MouseState.Normal);
        }
      }
    }

    private Color inactiveTextColor = Color.FromArgb(177, 177, 177);

    public Color InactiveTextColor
    {
      get { return inactiveTextColor; }
      set { inactiveTextColor = value; }
    }

    private Color activeBorderColor = Color.FromArgb(43, 87, 154);

    public Color ActiveBorderColor
    {
      get { return activeBorderColor; }
      set { activeBorderColor = value; }
    }

    private Color inactiveBorderColor = Color.FromArgb(131, 131, 131);

    public Color InactiveBorderColor
    {
      get { return inactiveBorderColor; }
      set { inactiveBorderColor = value; }
    }

    public enum MouseState
    {
      Normal,
      Hover,
      Down
    }

    protected void SetLabelColors(Control control, MouseState state)
    {
      if (!ContainsFocus) return;

      var textColor = ActiveTextColor;
      var backColor = NormalBackColor;

      switch (state)
      {
        case MouseState.Hover:
          textColor = HoverTextColor;
          backColor = HoverBackColor;
          break;
        case MouseState.Down:
          textColor = DownTextColor;
          backColor = DownBackColor;
          break;
      }

      control.ForeColor = textColor;
      control.BackColor = backColor;
    }

    protected Label MaximizeLabel { get; set; }
    protected Label MinimizeLabel { get; set; }
    protected Label CloseLabel { get; set; }
    protected Label TitleLabel { get; set; }
    protected Label SystemLabel { get; set; }
    protected Panel TopLeftPanel { get; set; }
    protected Panel TopRightPanel { get; set; }
    protected Panel TopLeftCornerPanel { get; set; }
    protected Panel TopRightCornerPanel { get; set; }
    protected Panel BottomLeftCornerPanel { get; set; }
    protected Panel BottomRightCornerPanel { get; set; }
    protected Panel TopBorderPanel { get; set; }
    protected Panel LeftBorderPanel { get; set; }
    protected Panel RightBorderPanel { get; set; }
    protected Panel BottomBorderPanel { get; set; }

    private FormWindowState previousWindowState;
    private bool _themeInitialize = false;

    protected void InitializeTheme()
    {
      _themeInitialize = true;
      foreach (var control in new[] { MinimizeLabel, MaximizeLabel, CloseLabel }.Where(c => c != null))
      {
        control.MouseEnter += (s, e) => SetLabelColors((Control)s, MouseState.Hover);
        control.MouseLeave += (s, e) => SetLabelColors((Control)s, MouseState.Normal);
        control.MouseDown += (s, e) => SetLabelColors((Control)s, MouseState.Down);
      }

      TopLeftCornerPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPLEFT);
      TopRightCornerPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPRIGHT);
      BottomLeftCornerPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMLEFT);
      BottomRightCornerPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMRIGHT);

      TopBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOP);
      LeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTLEFT);
      if (TopLeftPanel != null) TopLeftPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTLEFT);
      RightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTRIGHT);
      if (TopRightPanel != null) TopRightPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTRIGHT);
      BottomBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOM);

      TopLeftCornerPanel.Cursor = Cursors.SizeNWSE;
      TopRightCornerPanel.Cursor = Cursors.SizeNESW;
      BottomLeftCornerPanel.Cursor = Cursors.SizeNESW;
      BottomRightCornerPanel.Cursor = Cursors.SizeNWSE;

      TopBorderPanel.Cursor = Cursors.SizeNS;
      LeftBorderPanel.Cursor = Cursors.SizeWE;
      if (TopLeftPanel != null) TopLeftPanel.Cursor = Cursors.SizeWE;
      RightBorderPanel.Cursor = Cursors.SizeWE;
      if (TopRightPanel != null) TopRightPanel.Cursor = Cursors.SizeWE;
      BottomBorderPanel.Cursor = Cursors.SizeNS;

      if (SystemLabel != null)
      {
        SystemLabel.MouseDown += SystemLabel_MouseDown;
        SystemLabel.MouseUp += SystemLabel_MouseUp;
      }

      TitleLabel.MouseDown += TitleLabel_MouseDown;
      TitleLabel.MouseUp += (s, e) => { if (e.Button == MouseButtons.Right && TitleLabel.ClientRectangle.Contains(e.Location)) ShowSystemMenu(MouseButtons); };
      TitleLabel.Text = Text;

      var marlett = new Font("Marlett", 8.5f);

      MinimizeLabel.Font = marlett;
      MaximizeLabel.Font = marlett;
      CloseLabel.Font = marlett;
      if (SystemLabel != null) SystemLabel.Font = marlett;

      MinimizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) WindowState = FormWindowState.Minimized; };
      MaximizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) ToggleMaximize(); };
      previousWindowState = MinMaxState;
      CloseLabel.MouseClick += (s, e) => Close(e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      if (!_themeInitialize) return;
      TitleLabel.Text = Text;
    }

    protected void SystemLabel_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right) ShowSystemMenu(MouseButtons);
    }

    private DateTime systemClickTime = DateTime.MinValue;
    private DateTime systemMenuCloseTime = DateTime.MinValue;

    protected void SystemLabel_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var clickTime = (DateTime.Now - systemClickTime).TotalMilliseconds;
        if (clickTime < SystemInformation.DoubleClickTime)
          Close();
        else
        {
          systemClickTime = DateTime.Now;
          if ((systemClickTime - systemMenuCloseTime).TotalMilliseconds > 200)
          {
            if (SystemLabel != null) SetLabelColors(SystemLabel, MouseState.Normal);
            var ctrl = ((Control)sender);
            var pt = new Point(ctrl.Left, ctrl.Bottom);
            ShowSystemMenu(MouseButtons, ctrl.PointToScreen(pt));
            systemMenuCloseTime = DateTime.Now;
          }
        }
      }
    }

    void Close(MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left) Close();
    }

    void DecorationMouseDown(MouseEventArgs e, HitTestValues h)
    {
      if (e.Button == MouseButtons.Left) DecorationMouseDown(h);
    }

    protected override void OnDeactivate(EventArgs e)
    {
      base.OnDeactivate(e);
      if (!_themeInitialize) return;
      //SetBorderColor(InactiveBorderColor);
      SetTextColor(InactiveTextColor);
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
      if (!_themeInitialize) return;
      //SetBorderColor(ActiveBorderColor);
      SetTextColor(ActiveTextColor);
    }

    //protected void SetBorderColor(Color color)
    //{
    //  TopLeftCornerPanel.BackColor = color;
    //  TopBorderPanel.BackColor = color;
    //  TopRightCornerPanel.BackColor = color;
    //  LeftBorderPanel.BackColor = color;
    //  RightBorderPanel.BackColor = color;
    //  BottomLeftCornerPanel.BackColor = color;
    //  BottomBorderPanel.BackColor = color;
    //  BottomRightCornerPanel.BackColor = color;
    //}

    protected void SetTextColor(Color color)
    {
      if (SystemLabel != null) SystemLabel.ForeColor = color;
      TitleLabel.ForeColor = color;
      MinimizeLabel.ForeColor = color;
      MaximizeLabel.ForeColor = color;
      CloseLabel.ForeColor = color;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      if (!_themeInitialize) return;

      var maximized = MinMaxState == FormWindowState.Maximized;
      MaximizeLabel.Text = maximized ? "2" : "1";

      //var panels = new[] { TopLeftCornerPanel, TopRightCornerPanel, BottomLeftCornerPanel, BottomRightCornerPanel,
      //          TopBorderPanel, LeftBorderPanel, RightBorderPanel, BottomBorderPanel, TopLeftPanel, TopRightPanel };

      //foreach (var panel in panels)
      //{
      //  panel.Visible = !maximized;
      //}
    }

    private FormWindowState ToggleMaximize()
    {
      return WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
    }

    private DateTime titleClickTime = DateTime.MinValue;
    private Point titleClickPosition = Point.Empty;

    void TitleLabel_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var clickTime = (DateTime.Now - titleClickTime).TotalMilliseconds;
        if (clickTime < SystemInformation.DoubleClickTime && e.Location == titleClickPosition)
          ToggleMaximize();
        else
        {
          titleClickTime = DateTime.Now;
          titleClickPosition = e.Location;
          DecorationMouseDown(HitTestValues.HTCAPTION);
        }
      }
    }
    #endregion
  }
}
