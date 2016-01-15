using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class ParameterPrompt : Form
  {
    private BindingSource _source = new BindingSource();

    public ParameterPrompt()
    {
      InitializeComponent();

      //var param = new ParamTest()
      //{
      //  ItemNumber = "905-1954-001",
      //  Name = "Finished Good",
      //  Description = "Never Used"
      //};
      //var list = new List<ParamTest>() { param };

      var obj = new QueryParamGroup();
      obj.Items.Add(new QueryParameter() { Name = "ItemNumber", TextValue = "905-1954-001" });
      obj.Items.Add(new QueryParameter() { Name = "Name", TextValue = "Finished Good" });
      obj.Items.Add(new QueryParameter() { Name = "IsDbParam", TextValue = "1", Type = QueryParameter.DataType.Boolean });
      obj.Items.Add(new QueryParameter() { Name = "DateValue", TextValue = "2016-01-14" });

      var list = new List<QueryParamGroup>() { obj };

      _source.DataSource = list;
      paramGrid.SetDataSource(_source);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
    }

    private class ParamTest
    {
      public string Name { get; set; }
      public string Description { get; set; }
      public string ItemNumber { get; set; }
      [DisplayName("Is Active Rev")]
      public bool IsActiveRev { get; set; }
      public DateTime CreatedOn { get; set; }
      public string CreatedBy { get; set; }
      public string Another { get; set; }
    }
  }
}
