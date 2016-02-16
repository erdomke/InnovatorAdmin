using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface ICell : IFieldValue
  {
    /// <summary>
    /// Gets the value of the cell as formatted for display.
    /// </summary>
    [DisplayName("Formatted Value"), Description("Gets the value of the cell as formatted for display.")]
    object FormattedValue { get; set; }

    /// <summary>
    /// Gets or sets the style for the cell.
    /// </summary>
    [DisplayName("Style"), Description("Gets or sets the style for the cell.")]
    ICellStyle Style { get; set; }
  }
}
