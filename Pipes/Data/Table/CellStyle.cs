using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Pipes.Data.Table
{
  public class CellStyle : ICellStyle, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private ContentAlignment _alignment = ContentAlignment.NotSet;
    private Color _backColor = Color.Empty;
    private Color _borderColor = Color.Empty;
    private BorderSides _borderSides = BorderSides.NotSet;
    private int _borderWidth = -1;
    private IFontStyle _font;
    private Color _foreColor = Color.Empty;
    private string _format;
    private int _indent = -1;
    private Padding _padding;
    private Conversion.TriState _wrapMode = Conversion.TriState.Indeterminate;

    /// <summary>
    /// Gets or sets a value indicating the position of the cell content within a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Alignment"), Description("Gets or sets a value indicating the position of the cell content within a cell.")]
    public ContentAlignment Alignment {
      get { return _alignment; }
      set {
        _alignment = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Alignment"));
      }
    }

    /// <summary>
    /// Gets or sets the background color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Back Color"), Description("Gets or sets the background color of a cell.")]
    public Color BackColor {
      get { return _backColor; }
      set {
        _backColor = value;
        OnPropertyChanged(new PropertyChangedEventArgs("BackColor"));
      }
    }

    /// <summary>
    /// Gets or sets the border color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Border Color"), Description("Gets or sets the border color of a cell.")]
    public Color BorderColor {
      get { return _borderColor; }
      set {
        _borderColor = value;
        OnPropertyChanged(new PropertyChangedEventArgs("BorderColor"));
      }
    }

    /// <summary>
    /// Gets or sets the sides on which a border is drawn.
    /// </summary>
    [DisplayName("Border sides"), Description("Gets or sets the sides on which a border is drawn.")]
    public BorderSides BorderSides {
      get { return _borderSides; }
      set {
        _borderSides = value;
        OnPropertyChanged(new PropertyChangedEventArgs("BorderSides"));
      }
    }

    /// <summary>
    /// Gets or sets the border width of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Border Width"), Description("Gets or sets the border width of a cell.")]
    public int BorderWidth {
      get { return _borderWidth; }
      set {
        _borderWidth = value;
        OnPropertyChanged(new PropertyChangedEventArgs("BorderWidth"));
      }
    }

    /// <summary>
    /// Gets or sets the font applied to the textual content of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Font"), Description("Gets or sets the font applied to the textual content of a cell.")]
    public IFontStyle Font {
      get { return _font; }
      set {
        _font = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Font"));
      }
    }

    /// <summary>
    /// Gets or sets the foreground color of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Foreground Color"), Description("Gets or sets the foreground color of a cell.")]
    public Color ForeColor {
      get { return _foreColor; }
      set {
        _foreColor = value;
        OnPropertyChanged(new PropertyChangedEventArgs("ForeColor"));
      }
    }

    /// <summary>
    /// Gets or sets the format string applied to the textual content of a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Format"), Description("Gets or sets the format string applied to the textual content of a cell.")]
    public string Format {
      get { return _format; }
      set {
        _format = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Format"));
      }
    }

    /// <summary>
    /// Gets or sets the number of indents for a <see cref="ICell"/>.
    /// </summary>
    [DisplayName("Intent"), Description("Gets or sets the number of indents for a cell.")]
    public int Indent {
      get { return _indent; }
      set {
        _indent = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Indent"));
      }
    }

    /// <summary>
    /// Gets or sets the space between the edge of a <see cref="ICell"/> and its content.
    /// </summary>
    [DisplayName("Padding"), Description("Gets or sets the space between the edge of a cell and its content.")]
    public Padding Padding {
      get { return _padding; }
      set {
        _padding = value;
        OnPropertyChanged(new PropertyChangedEventArgs("Padding"));
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether textual content in a <see cref="ICell"/> is wrapped to subsequent lines or truncated when it is too long to fit on a single line.
    /// </summary>
    [DisplayName("Wrap Mode"), Description("Gets or sets a value indicating whether textual content in a cell is wrapped to subsequent lines or truncated when it is too long to fit on a single line.")]
    public Conversion.TriState WrapMode
    {
      get { return _wrapMode; }
      set {
        _wrapMode = value;
        OnPropertyChanged(new PropertyChangedEventArgs("WrapMode"));
      }
    }

    public CellStyle()
    {
      //Do Nothing
    }
    public CellStyle(ICellStyle clone)
    {
      this.Alignment = clone.Alignment;
      this.BackColor = clone.BackColor;
      this.BorderColor = clone.BorderColor;
      this.BorderSides = clone.BorderSides;
      this.BorderWidth = clone.BorderWidth;
      this.Font = clone.Font;
      this.ForeColor = clone.ForeColor;
      this.Format = clone.Format;
      this.Indent = clone.Indent;
      this.Padding = clone.Padding;
      this.WrapMode = clone.WrapMode;
    }

    public override int GetHashCode()
    {
      int returnVal = (int)this.Alignment ^ this.BackColor.GetHashCode() ^ this.BorderColor.GetHashCode() ^ (int)this.BorderSides ^ this.BorderWidth ^ this.ForeColor.GetHashCode() ^ this.Indent ^ this.Padding.GetHashCode() ^ (int)this.WrapMode;
      if (this.Font != null)
        returnVal = returnVal ^ this.Font.GetHashCode();
      if (this.Format != null)
        returnVal = returnVal ^ this.Format.GetHashCode();
      return returnVal;
    }
    public override bool Equals(object obj)
    {
      if (obj is CellStyle) {
        return Equals((CellStyle)obj);
      } else {
        return false;
      }
    }
    public bool Equals(CellStyle obj)
    {
      return this.Alignment == obj.Alignment && ColorAreEqual(this.BackColor, obj.BackColor) && ColorAreEqual(this.BorderColor, obj.BorderColor) && this.BorderSides == obj.BorderSides && this.BorderWidth == obj.BorderWidth && this.Font.Equals(obj.Font) && ColorAreEqual(this.ForeColor, obj.ForeColor) && this.Format == obj.Format && this.Indent == obj.Indent && this.Padding == obj.Padding && this.WrapMode == obj.WrapMode;
    }
    public bool IsEmpty()
    {
      return this.Alignment == ContentAlignment.NotSet 
        && (this.BackColor.IsEmpty || ColorAreEqual(this.BackColor, System.Drawing.Color.White)) 
        && (this.BorderColor.IsEmpty || ColorAreEqual(this.BorderColor, System.Drawing.Color.White)) 
        && (this.BorderSides == BorderSides.NotSet)
        && this.BorderWidth <= 0 
        && this.Font == null 
        && (this.ForeColor.IsEmpty || ColorAreEqual(this.ForeColor, System.Drawing.Color.White)) 
        && string.IsNullOrEmpty(this.Format) 
        && this.Indent <= 0 
        && this.Padding.All == 0 
        && this.WrapMode == Conversion.TriState.Indeterminate;
    }

    protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null) {
        PropertyChanged(this, e);
      }
    }

    public static bool operator ==(CellStyle a, CellStyle b)
    {
      return a.Equals(b);
    }
    public static bool operator !=(CellStyle a, CellStyle b)
    {
      return !a.Equals(b);
    }


    public static bool ColorAreEqual(Color color1, Color color2) {
      return color1.A == color2.A && color1.R == color2.R && color1.G == color2.G && color1.B == color2.B;
    }

    public static ICellStyle GetMerged(ICellStyle baseStyle, ICellStyle style)
    {
      if (baseStyle == null) return style ?? new CellStyle();
      if (style == null) return baseStyle;

      var result = new CellStyle(baseStyle);
      if (style.Alignment != ContentAlignment.NotSet) result.Alignment = style.Alignment;
      if (style.BackColor != Color.Empty) result.BackColor = style.BackColor;
      if (style.BorderColor != Color.Empty) result.BorderColor = style.BorderColor;
      if (style.BorderSides != BorderSides.NotSet) result.BorderSides = style.BorderSides;
      if (style.BorderWidth >= 0) result.BorderWidth = style.BorderWidth;
      if (style.Font != null) result.Font = style.Font;
      if (style.ForeColor != Color.Empty) result.ForeColor = style.ForeColor;
      if (!string.IsNullOrEmpty(style.Format)) result.Format = style.Format;
      if (style.Indent >= 0) result.Indent = style.Indent;
      if (style.Padding != Padding.Empty) result.Padding = style.Padding;
      if (style.WrapMode != Conversion.TriState.Indeterminate) result.WrapMode = style.WrapMode;
      return result;
    }

  }
}
