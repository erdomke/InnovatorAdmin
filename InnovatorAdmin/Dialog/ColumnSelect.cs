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
  public partial class ColumnSelect : Form
  {
    private DataGridView _dataSource;

    public DataGridView DataSource
    {
      get { return _dataSource; }
      set
      {
        _dataSource = value;
        grid.DataSource = _dataSource.Columns
          .OfType<DataGridViewColumn>()
          .OrderBy(c => c.HeaderText)
          .ToList();
      }
    }

    public ColumnSelect()
    {
      InitializeComponent();
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
      grid.AutoGenerateColumns = false;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }


  }
}
