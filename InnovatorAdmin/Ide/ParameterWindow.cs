using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class ParameterWindow : Form
  {
    private IList<QueryParameter> _orig;

    public ParameterWindow(IList<QueryParameter> parameters)
    {
      InitializeComponent();

      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      _orig = parameters;

      bs.DataSource = parameters.Select(p => p.Clone()).ToList();
      lstItems.DataSource = bs;
      lstItems.DisplayMember = "Name";

      cmbType.DataSource = Enum.GetValues(typeof(QueryParameter.DataType));

      txtName.DataBindings.Add("Text", bs, "Name");
      txtValue.DataBindings.Add("Text", bs, "TextValue");
      cmbType.DataBindings.Add("Text", bs, "Type");
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      txtValue.Focus();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        var builder = new StringBuilder();
        var parameters = (IList<QueryParameter>)bs.DataSource;
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
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      try
      {
        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.Close();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnIgnore_Click(object sender, EventArgs e)
    {
      try
      {
        this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
        this.Close();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }


  }
}
