using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class TableWriterSettings
  {
    /// <summary>
    /// Whether or not to display the auto filter UI.
    /// </summary>
    [DisplayName("Auto Filter"), Description("Whether or not to display the auto filter UI.")]
    public bool AutoFilter { get; set; }

    /// <summary>
    /// Default style applied to cells in the table
    /// </summary>
    [DisplayName("Default Style"), Description("Default style applied to cells in the table.")]
    public ICellStyle DefaultStyle { get; set; }

    /// <summary>
    /// How to handle hidden columns in the export
    /// </summary>
    [DisplayName("Hidden Column Handling"), Description("How to handle hidden columns in the export.")]
    public HiddenColumnOptions HiddenColumnHandling { get; set; }

    /// <summary>
    /// Indicates whether or not to include column headers in the result
    /// </summary>
    [DisplayName("Include Headers"), Description("Indicates whether or not to include column headers in the result.")]
    public bool IncludeHeaders { get; set; }

    /// <summary>
    /// Whether to render the page in landscape.
    /// </summary>
    [DisplayName("Landscape"), Description("Whether to render the page in landscape.")]
    public bool Landscape { get; set; }

    /// <summary>
    /// Footer height in inches.
    /// </summary>
    [DisplayName("Footer Height"), Description("Footer height in inches.")]
    public double PageFooterHeight { get; set; }

    /// <summary>
    /// Header height in inches.
    /// </summary>
    [DisplayName("Header Height"), Description("Header height in inches.")]
    public double PageHeaderHeight { get; set; }

    public bool RepeatHeader { get; set; }

    public TableWriterSettings()
    {
      this.AutoFilter = true;
      this.RepeatHeader = true;
      this.HiddenColumnHandling = HiddenColumnOptions.IncludeHidden;
      this.IncludeHeaders = true;
    }
  }
}
