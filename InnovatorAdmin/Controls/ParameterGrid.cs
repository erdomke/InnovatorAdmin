using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public partial class ParameterGrid : UserControl
  {
    private List<int> _initialValues = new List<int>();
    private bool _isLoaded = false;

    public IPropertyFilter Filter { get; set; }

    public ParameterGrid()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // No clue why I need this, but it works.
      for (var i = 0; i < _initialValues.Count; i++)
      {
        var cmb = (ComboBox)tblMain.GetControlFromPosition(1, i);
        cmb.SelectedIndex = _initialValues[i];
      }
      _isLoaded = true;
    }

    public void SetDataSource(BindingSource source)
    {
      tblMain.Controls.Clear();
      tblMain.RowStyles.Clear();
      tblMain.RowCount = 0;

      var dataTypeArray = Enum.GetValues(typeof(QueryParameter.DataType)).OfType<QueryParameter.DataType>().ToArray();
      var props = source.GetItemProperties(null).OfType<PropertyDescriptor>()
        .Where(p => Filter == null || Filter.Contains(p))
        .OrderBy(p => p.DisplayName ?? p.Name)
        .ToArray();

      foreach (var prop in props)
      {
        tblMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblMain.RowCount += 1;
        var currRow = tblMain.RowCount - 1;

        var lbl = new Label();
        lbl.AutoSize = true;
        lbl.Text = prop.DisplayName ?? prop.Name;
        lbl.Anchor = AnchorStyles.Left;
        tblMain.Controls.Add(lbl, 0, currRow);

        var currProp = prop as QueryParamDescriptor;
        if (currProp != null)
        {
          var box = new ComboBox();
          box.Anchor = AnchorStyles.Left;
          box.DropDownStyle = ComboBoxStyle.DropDownList;
          box.DataSource = Enum.GetValues(typeof(QueryParameter.DataType));
          tblMain.Controls.Add(box, 1, currRow);
          _initialValues.Add(Array.IndexOf(dataTypeArray, currProp.Param.Type));
          box.SelectedIndexChanged += (s, e) =>
          {
            if (!_isLoaded)
              return;

            tblMain.Controls.Remove(tblMain.GetControlFromPosition(2, currRow));
            var newValue = (QueryParameter.DataType)((ComboBox)s).SelectedValue;
            currProp.Param.Type = newValue;
            switch (newValue)
            {
              case QueryParameter.DataType.Boolean:
                currProp.SetValue(source.Current, default(bool));
                AddControl(source, currProp, typeof(bool), currRow);
                break;
              case QueryParameter.DataType.DateTime:
                currProp.SetValue(source.Current, default(DateTime));
                AddControl(source, currProp, typeof(DateTime), currRow);
                break;
              case QueryParameter.DataType.Decimal:
                currProp.SetValue(source.Current, default(decimal));
                AddControl(source, currProp, typeof(decimal), currRow);
                break;
              case QueryParameter.DataType.Integer:
                currProp.SetValue(source.Current, default(long));
                AddControl(source, currProp, typeof(long), currRow);
                break;
              default:
                currProp.SetValue(source.Current, default(string));
                AddControl(source, currProp, typeof(string), currRow);
                break;
            }
          };
        }

        AddControl(source, prop, null, currRow);
      }

      System.Diagnostics.Debug.Print("{0}", this.Height);
    }

    private void AddControl(BindingSource source, PropertyDescriptor prop, Type type, int row)
    {
      var paramControl = prop.Attributes.OfType<ParamControlAttribute>().FirstOrDefault();

      type = type ?? prop.PropertyType;
      Control ctrl;
      if (paramControl != null && typeof(Control).IsAssignableFrom(paramControl.ControlType))
      {
        ctrl = (Control)Activator.CreateInstance(paramControl.ControlType);
        var optCtrl = ctrl as IOptionsControl;
        if (optCtrl != null)
          optCtrl.SetOptions(paramControl.Options);
        ctrl.DataBindings.Add("Text", source, prop.Name);
      }
      else if (type == typeof(bool))
      {
        ctrl = new CheckBox();
        ctrl.DataBindings.Add("Checked", source, prop.Name);
      }
      else if (type == typeof(DateTime))
      {
        var picker = new DateTimePicker();
        picker.ShowCheckBox = true;
        ctrl = picker;
        var binding = new Binding("Value", source, prop.Name, true);
        binding.Format += dateTimeBinding_Format;
        binding.Parse += dateTimeBinding_Parse;
        ctrl.DataBindings.Add(binding);
      }
      else
      {
        var txt = new TextBox();
        txt.Multiline = true;
        txt.Height = 45;
        txt.ScrollBars = ScrollBars.Vertical;
        ctrl = txt;
        ctrl.DataBindings.Add("Text", source, prop.Name);
      }
      ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      tblMain.Controls.Add(ctrl, 2, row);
    }

    void dateTimeBinding_Parse(object sender, ConvertEventArgs e)
    {
      var b = (Binding)sender;
      var dtp = (DateTimePicker)b.Control;

      if (dtp.Checked == false)
      {
        dtp.ShowCheckBox = true;
        dtp.Checked = false;
        e.Value = DateTime.MinValue;
      }
    }

    void dateTimeBinding_Format(object sender, ConvertEventArgs e)
    {
      var b = (Binding)sender;
      var dtp = (DateTimePicker)b.Control;

      if (e.Value == null || (DateTime)e.Value == DateTime.MinValue)
      {
        dtp.ShowCheckBox = true;
        dtp.Checked = false;
        // have to set e.Value to SOMETHING, since it’s coming in as NULL
        // if i set to DateTime.Today, and that’s DIFFERENT than the control’s current
        // value, then it triggers a CHANGE to the value, which CHECKS the box (not ok)
        // the trick – set e.Value to whatever value the control currently has.
        // This does NOT cause a CHANGE, and the checkbox stays OFF.
        e.Value = dtp.Value;
      }
      else
      {
        dtp.ShowCheckBox = true;
        dtp.Checked = true;
        // leave e.Value unchanged – it’s not null, so the DTP is fine with it.
      }
    }
  }
}
