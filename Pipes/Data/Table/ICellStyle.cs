using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Pipes.Data.Table
{
  public interface ICellStyle
  {
    /// <summary>
    /// Gets or sets a value indicating the position of the cell content within a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Alignment"), Description("Gets or sets a value indicating the position of the cell content within a cell.")]
    ContentAlignment Alignment { get; set; }

    /// <summary>
    /// Gets or sets the background color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Back Color"), Description("Gets or sets the background color of a cell.")]
    Color BackColor { get; set; }

    /// <summary>
    /// Gets or sets the border color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Border Color"), Description("Gets or sets the border color of a cell.")]
    Color BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the sides on which a border is drawn.
    /// </summary>
    [DisplayName("Border sides"), Description("Gets or sets the sides on which a border is drawn.")]
    BorderSides BorderSides { get; set; }

    /// <summary>
    /// Gets or sets the border width of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Border Width"), Description("Gets or sets the border width of a cell.")]
    int BorderWidth { get; set; }

    /// <summary>
    /// Gets or sets the font applied to the textual content of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Font"), Description("Gets or sets the font applied to the textual content of a cell.")]
    IFontStyle Font { get; set; }

    /// <summary>
    /// Gets or sets the foreground color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Foreground Color"), Description("Gets or sets the foreground color of a cell.")]
    Color ForeColor { get; set; }
    
    /// <summary>
    /// Gets or sets the format string applied to the textual content of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Format"), Description("Gets or sets the format string applied to the textual content of a cell.")]
    string Format { get; set; }
    
    /// <summary>
    /// Gets or sets the number of indents for a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Intent"), Description("Gets or sets the number of indents for a cell.")]
    int Indent { get; set; }

    /// <summary>
    /// Gets or sets the space between the edge of a <see cref="ICell"/> and its content.
    /// </summary>
    [DisplayName("Padding"), Description("Gets or sets the space between the edge of a cell and its content.")]
    Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether textual content in a <see cref="ICell"/> is wrapped to subsequent lines or truncated when it is too long to fit on a single line.
    /// </summary>
    [DisplayName("Wrap Mode"), Description("Gets or sets a value indicating whether textual content in a cell is wrapped to subsequent lines or truncated when it is too long to fit on a single line.")]
    Conversion.TriState WrapMode { get; set; }

    bool IsEmpty();
  }
}
