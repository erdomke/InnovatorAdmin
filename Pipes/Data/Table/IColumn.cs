using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IColumn : IColumnMetadata
  {
    /// <summary>
    /// Style for the header cell
    /// </summary>
    [DisplayName("Style"), Description("Style for the header cell")]
    ICellStyle Style { get; set; }
    /// <summary>
    /// Whether or not the column is visible
    /// </summary>
    [DisplayName("Visible"), Description("Whether or not the column is visible")]
    bool Visible { get; set; }
    /// <summary>
    /// Width of the column in pixels
    /// </summary>
    [DisplayName("Width"), Description("Width of the column in pixels")]
    int Width { get; set; }
  }
}
