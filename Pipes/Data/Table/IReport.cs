using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IReport
  {
    /// <summary>
    /// Data and formatting information for the footer.
    /// </summary>
    [DisplayName("Footer"), Description("Data and formatting information for the footer.")]
    IFormattedTable Footer { get; set; }
    
    /// <summary>
    /// Data and formatting information for the header.
    /// </summary>
    [DisplayName("Header"), Description("Data and formatting information for the header.")]
    IFormattedTable Header { get; set; }

    /// <summary>
    /// Name of the report (used as the sheet/file name).
    /// </summary>
    [DisplayName("Name"), Description("Name of the report (used as the sheet/file name).")]
    string Name { get; set; }

    /// <summary>
    /// Data and formatting information for the table.
    /// </summary>
    [DisplayName("Table"), Description("Data and formatting information for the table.")]
    IFormattedTable Table { get; set; }
  }
}
