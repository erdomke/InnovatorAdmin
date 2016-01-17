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
  public partial class ParameterDialog : FormBase
  {
    private IList<QueryParameter> _orig;
    private BindingSource _source;

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }

    public ParameterDialog(IList<QueryParameter> parameters)
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

      _orig = parameters;

      var paramGroup = new QueryParamGroup(parameters.Select(p => p.Clone()));
      var list = new List<QueryParamGroup>() { paramGroup };
      _source = new BindingSource() { DataSource = list };
      paramGrid.SetDataSource(_source);
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

    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.ValidateChildren())
        {
          var builder = new StringBuilder();
          var parameters = ((IList<QueryParamGroup>)_source.DataSource)[0].Items;
          foreach (var param in parameters)
          {
            try
            {
              param.GetValue();
            }
            catch (FormatException)
            {
              builder.AppendFormat("Value `{0}` for parameter `{1}` is invalid for type `{2}`", param.TextValue, param.Name, param.Type).AppendLine();
            }
          }

          if (builder.Length > 0)
          {
            MessageBox.Show(builder.ToString());
          }
          else
          {
            for (var i = 0; i < _orig.Count; i++)
            {
              _orig[i].Overwrite(parameters[i]);
            }
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
