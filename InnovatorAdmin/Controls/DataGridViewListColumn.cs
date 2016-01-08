using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public class DataGridViewListColumn : DataGridViewColumn
  {
    private BindingSource _binding;

    public DataGridViewListColumn()
      : base(new DataGridViewListCell())
    {
    }

    public object DataSource
    {
      get { return _binding; }
      set
      {
        if (value == null)
        {
          _binding = null;
        }
        else
        {
          _binding = new BindingSource(value, null);
        }
      }
    }
    public string DisplayMember { get; set; }
    public string ValueMember { get; set; }

    public override DataGridViewCell CellTemplate
    {
      get
      {
        return base.CellTemplate;
      }
      set
      {
        // Ensure that the cell used for the template is a CalendarCell.
        if (value != null &&
            !value.GetType().IsAssignableFrom(typeof(DataGridViewListCell)))
        {
          throw new InvalidCastException("Must be a CalendarCell");
        }
        base.CellTemplate = value;
      }
    }


    public class DataGridViewListCell : DataGridViewTextBoxCell
    {

      public DataGridViewListCell()
        : base() { }

      public override void InitializeEditingControl(int rowIndex, object
          initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
      {
        // Set the value of the editing control to the current cell value.
        base.InitializeEditingControl(rowIndex, initialFormattedValue,
            dataGridViewCellStyle);

        var ctrl = (ListEditingControl)DataGridView.EditingControl;
        ctrl.Text = this.Value == null ? null : this.Value.ToString();
        ctrl.DisplayMember = ((DataGridViewListColumn)this.OwningColumn).DisplayMember;
        ctrl.ValueMember = ((DataGridViewListColumn)this.OwningColumn).ValueMember;
        ctrl.DataSource = ((DataGridViewListColumn)this.OwningColumn).DataSource;

        var idx = FindIndexByProp(ctrl.ValueMember, this.Value as string);
        if (idx >= 0)
          ctrl.SelectedIndex = idx;
      }

      protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
      {
        var col = ((DataGridViewListColumn)this.OwningColumn);
        var idx = FindIndexByProp(col.ValueMember, value as string);
        if (idx >= 0)
        {
          var bs = (BindingSource)col.DataSource;
          return bs.GetItemProperties(null)[col.DisplayMember].GetValue(bs[idx]);
        }

        return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
      }

      public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
      {
        var col = ((DataGridViewListColumn)this.OwningColumn);
        var idx = FindIndexByProp(col.DisplayMember, formattedValue as string);
        if (idx >= 0)
        {
          var bs = (BindingSource)col.DataSource;
          return bs.GetItemProperties(null)[col.ValueMember].GetValue(bs[idx]);
        }

        return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
      }

      private int FindIndexByProp(string propName, string value)
      {
        var col = ((DataGridViewListColumn)this.OwningColumn);
        var bs = col.DataSource as BindingSource;
        if (bs != null
          && !string.IsNullOrEmpty(col.ValueMember)
          && !string.IsNullOrEmpty(col.DisplayMember))
        {
          var prop = bs.GetItemProperties(null)[propName];
          for (var i = 0; i < bs.Count; i++)
          {
            if (string.Equals(prop.GetValue(bs[i]) as string, value, StringComparison.OrdinalIgnoreCase))
              return i;
          }
        }
        return -1;
      }

      public override Type EditType
      {
        get
        {
          // Return the type of the editing control that CalendarCell uses.
          return typeof(ListEditingControl);
        }
      }

      public override Type ValueType
      {
        get
        {
          // Return the type of the value that CalendarCell contains.

          return typeof(string);
        }
      }

      public override object DefaultNewRowValue
      {
        get
        {
          // Use the current date and time as the default value.
          return string.Empty;
        }
      }
    }


    class ListEditingControl : ComboBox, IDataGridViewEditingControl
    {
      DataGridView dataGridView;
      private bool valueChanged = false;
      int rowIndex;

      public ListEditingControl()
      {
        this.DropDownStyle = ComboBoxStyle.DropDown;
      }

      // Implements the IDataGridViewEditingControl.EditingControlFormattedValue
      // property.
      public object EditingControlFormattedValue
      {
        get
        {
          return this.Text;
        }
        set
        {
          if (value == null)
          {
            this.Text = string.Empty;
          }
          else
          {
            this.Text = value.ToString();
          }
        }
      }

      // Implements the
      // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
      public object GetEditingControlFormattedValue(
          DataGridViewDataErrorContexts context)
      {
        return EditingControlFormattedValue;
      }

      // Implements the
      // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
      public void ApplyCellStyleToEditingControl(
          DataGridViewCellStyle dataGridViewCellStyle)
      {
        this.Font = dataGridViewCellStyle.Font;
        this.ForeColor = dataGridViewCellStyle.ForeColor;
        this.BackColor = dataGridViewCellStyle.BackColor;
      }

      // Implements the IDataGridViewEditingControl.EditingControlRowIndex
      // property.
      public int EditingControlRowIndex
      {
        get
        {
          return rowIndex;
        }
        set
        {
          rowIndex = value;
        }
      }

      // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey
      // method.
      public bool EditingControlWantsInputKey(
          Keys key, bool dataGridViewWantsInputKey)
      {
        // Let the DateTimePicker handle the keys listed.
        switch (key & Keys.KeyCode)
        {
          case Keys.Left:
          case Keys.Up:
          case Keys.Down:
          case Keys.Right:
          case Keys.Home:
          case Keys.End:
          case Keys.PageDown:
          case Keys.PageUp:
            return true;
          default:
            return !dataGridViewWantsInputKey;
        }
      }

      // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit
      // method.
      public void PrepareEditingControlForEdit(bool selectAll)
      {
        // No preparation needs to be done.
      }

      // Implements the IDataGridViewEditingControl
      // .RepositionEditingControlOnValueChange property.
      public bool RepositionEditingControlOnValueChange
      {
        get
        {
          return false;
        }
      }

      // Implements the IDataGridViewEditingControl
      // .EditingControlDataGridView property.
      public DataGridView EditingControlDataGridView
      {
        get
        {
          return dataGridView;
        }
        set
        {
          dataGridView = value;
        }
      }

      // Implements the IDataGridViewEditingControl
      // .EditingControlValueChanged property.
      public bool EditingControlValueChanged
      {
        get
        {
          return valueChanged;
        }
        set
        {
          valueChanged = value;
        }
      }

      // Implements the IDataGridViewEditingControl
      // .EditingPanelCursor property.
      public Cursor EditingPanelCursor
      {
        get
        {
          return base.Cursor;
        }
      }

      protected override void OnTextChanged(EventArgs e)
      {
        // Notify the DataGridView that the contents of the cell
        // have changed.
        valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        base.OnTextChanged(e);
      }
      protected override void OnSelectedIndexChanged(EventArgs e)
      {
        // Notify the DataGridView that the contents of the cell
        // have changed.
        valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        base.OnSelectedIndexChanged(e);
      }

      //protected override void OnValueChanged(EventArgs eventargs)
      //{
      //  // Notify the DataGridView that the contents of the cell
      //  // have changed.
      //  valueChanged = true;
      //  this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      //  base.OnValueChanged(eventargs);
      //}
    }
  }
}
