using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Sgml;
using System.Web.UI;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pipes.Data.Table
{
  public class HtmlTableWriter : BaseFormattedTableWriter, IWriter<ISgmlWriter>
  {
    private HtmlWriterWrapper _writer;
    private TableWriterSettings _settings = new TableWriterSettings();
    private Dictionary<string, Table.IColumn> _columns = new Dictionary<string,Table.IColumn>();
    private bool _started;

    public object Parent { get; set; }

    public T Initialize<T>(T coreWriter) where T : ISgmlWriter
    {
      _writer = new HtmlWriterWrapper(coreWriter);
      return coreWriter;
    }
    
    public override Table.IFormattedTableWriter Cell(Table.ICell cell)
    {
      Table.IColumn column = null;
      if (_part == ReportParts.Data && (!_columns.TryGetValue(cell.Name, out column) || 
                                        column.Visible || 
                                        _settings.HiddenColumnHandling != HiddenColumnOptions.Exclude)) {
        _writer.Element(HtmlTextWriterTag.Td);
        
        FormatCell(_writer, cell.Style);
        if (column != null && !column.Visible && _settings.HiddenColumnHandling == HiddenColumnOptions.IncludeHidden)
        {
          _writer.Style(HtmlTextWriterStyle.Display, "none");
        }

        var render = (cell.FormattedValue ?? cell.Value).ToString();
        if (cell.Value is Image)
        {
          _writer.Element(HtmlTextWriterTag.Img);
          using (var stream = new MemoryStream())
          {
            ((Image)cell.Value).Save(stream, ImageFormat.Png);
            _writer.Attribute(HtmlTextWriterAttribute.Src, "data:image/png;base64," + Convert.ToBase64String(stream.ToArray()));
          }
          _writer.ElementEnd();
        }
        else if (cell.Value is Table.IHyperlink)
        {
          var hyperlink = cell.Value as Table.IHyperlink;
          _writer.Element(HtmlTextWriterTag.A).Attribute(HtmlTextWriterAttribute.Href, hyperlink.Target.ToString());
          render = (cell.FormattedValue ?? (hyperlink.Text ?? cell.Value)).ToString();
          if (string.IsNullOrEmpty(render))
          {
            _writer.Raw("&#160;");
          }
          else
          {
            _writer.Value(render);
          }
          _writer.ElementEnd();
        }
        else if (cell.Value is Uri)
        {
          var uri = cell.Value as Uri;
          _writer.Element(HtmlTextWriterTag.A).Attribute(HtmlTextWriterAttribute.Href, uri.ToString());
          if (string.IsNullOrEmpty(render))
          {
            _writer.Raw("&#160;");
          }
          else
          {
            _writer.Value(render);
          }
          _writer.ElementEnd();
        }
        else
        {
          if (cell.Value == null)
          {
            // Do nothing for now
          }
          else if (cell.Value is Int16 || cell.Value is Int32 || cell.Value is Int64 ||
            cell.Value is UInt16 || cell.Value is UInt32 || cell.Value is UInt64 ||
            cell.Value is double || cell.Value is float || cell.Value is decimal)
          {
            _writer.Attribute("num", "urn:schemas-microsoft-com:office:excel", cell.Value);
          }
          else if (cell.Value is bool)
          {
            _writer.Attribute("bool", "urn:schemas-microsoft-com:office:excel", 
              ((bool)cell.Value ? "TRUE" : "FALSE" )
            );
          }
          else if (cell.Value is DateTime)
          {
            // Do nothing for now
          }
          else
          {
            _writer.Attribute("str", "urn:schemas-microsoft-com:office:excel", null);
          }

          if (string.IsNullOrEmpty(render))
          {
            _writer.Raw("&#160;");
          }
          else
          {
            _writer.Value(render);
          }
        }
      
        _writer.ElementEnd();
      }
      
      
      return this;
    }

    public override Table.IFormattedTableWriter Column(Table.IColumn column)
    {
      if (_part == ReportParts.Data)
      {
        _columns[column.Name] = column;
        if (_settings.IncludeHeaders)
        {
          if (column.Visible || _settings.HiddenColumnHandling != HiddenColumnOptions.Exclude)
          {
            _writer.Element(HtmlTextWriterTag.Th);
            FormatCell(_writer, column.Style);
            if (!column.Visible && _settings.HiddenColumnHandling == HiddenColumnOptions.IncludeHidden)
            {
              _writer.Style(HtmlTextWriterStyle.Display, "none");
            }
            _writer.Value(column.Label);
            _writer.ElementEnd();
          }
        }
      }
      return this;
    }

    public override void InitializeSettings(TableWriterSettings settings)
    {
      _settings = settings ?? new TableWriterSettings();
    }

    public override void Flush()
    {
      _writer.ElementEnd();
      _writer.ElementEnd();
      _writer.Flush();
    }

    public override Table.ITableWriter Head()
    {
      if (_part == ReportParts.Data)
      {
        EnsureStart();
        if (_settings.IncludeHeaders)
        {
          _writer.Element(HtmlTextWriterTag.Thead);
          _writer.Element(HtmlTextWriterTag.Tr);
        }
      }
      return this;
    }

    public override Table.ITableWriter HeadEnd()
    {
      if (_part == ReportParts.Data)
      {
        if (_settings.IncludeHeaders)
        {
          _writer.ElementEnd();
          _writer.ElementEnd();
        }
        _writer.Element(HtmlTextWriterTag.Tbody);
      }
      return this;
    }

    public override Table.ITableWriter Row()
    {
      if (_part == ReportParts.Data) _writer.Element(HtmlTextWriterTag.Tr);
      return this;
    }

    public override Table.ITableWriter RowEnd()
    {
      if (_part == ReportParts.Data) _writer.ElementEnd();
      return this;
    }

    private void EnsureStart()
    {
      if (!_started && _part == ReportParts.Data)
      {
        _started = true;
        _writer.Element(HtmlTextWriterTag.Table)
             .Attribute(HtmlTextWriterAttribute.Cellspacing, "0")
             .Attribute(HtmlTextWriterAttribute.Border, "0")
             .Attribute("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
      }
    }

    private void FormatCell(Pipes.Sgml.HtmlWriterWrapper writer, Table.ICellStyle style)
    {
      style = CellStyle.GetMerged(_settings.DefaultStyle, style);

      switch (style.Alignment)
      {
        case ContentAlignment.BottomCenter:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "bottom");
          writer.Style(HtmlTextWriterStyle.TextAlign, "center");
          break;
        case ContentAlignment.BottomLeft:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "bottom");
          writer.Style(HtmlTextWriterStyle.TextAlign, "left");
          break;
        case ContentAlignment.BottomRight:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "bottom");
          writer.Style(HtmlTextWriterStyle.TextAlign, "right");
          break;
        case ContentAlignment.MiddleCenter:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "middle");
          writer.Style(HtmlTextWriterStyle.TextAlign, "center");
          break;
        case ContentAlignment.MiddleLeft:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "middle");
          writer.Style(HtmlTextWriterStyle.TextAlign, "left");
          break;
        case ContentAlignment.MiddleRight:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "middle");
          writer.Style(HtmlTextWriterStyle.TextAlign, "right");
          break;
        case ContentAlignment.TopCenter:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "top");
          writer.Style(HtmlTextWriterStyle.TextAlign, "center");
          break;
        case ContentAlignment.TopRight:
          writer.Style(HtmlTextWriterStyle.VerticalAlign, "top");
          writer.Style(HtmlTextWriterStyle.TextAlign, "center");
          break;
        default:
          break;
        //htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top")
        //htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "left")
      }

      if (style.BackColor != System.Drawing.Color.Empty)
      {
        writer.Style(HtmlTextWriterStyle.BackgroundColor, HtmlColor(style.BackColor));
      }

      if (style.BorderWidth > 0 && style.BorderColor != System.Drawing.Color.Empty)
      {
        if ((style.BorderSides & BorderSides.All) == BorderSides.All)
        {
          writer.Style("border", string.Format("{0}px solid {1}", style.BorderWidth, HtmlColor(style.BorderColor)));
        }
        else
        {
          if ((style.BorderSides & BorderSides.Left) == BorderSides.Left)
            writer.Style("border-left", string.Format("{0}px solid {1}", style.BorderWidth, HtmlColor(style.BorderColor)));
          if ((style.BorderSides & BorderSides.Right) == BorderSides.Right)
            writer.Style("border-right", string.Format("{0}px solid {1}", style.BorderWidth, HtmlColor(style.BorderColor)));
          if ((style.BorderSides & BorderSides.Top) == BorderSides.Top)
            writer.Style("border-top", string.Format("{0}px solid {1}", style.BorderWidth, HtmlColor(style.BorderColor)));
          if ((style.BorderSides & BorderSides.Bottom) == BorderSides.Bottom)
            writer.Style("border-bottom", string.Format("{0}px solid {1}", style.BorderWidth, HtmlColor(style.BorderColor)));
        }
      }
      if (style.Font != null)
      {
        if (!string.IsNullOrEmpty(style.Font.Name))
          writer.Style(HtmlTextWriterStyle.FontFamily, style.Font.Name);
        if (style.Font.SizeInPoints > 0)
          writer.Style(HtmlTextWriterStyle.FontSize, string.Format("{0}pt", style.Font.SizeInPoints));
        if (style.Font.Italic)
          writer.Style(HtmlTextWriterStyle.FontStyle, "italic");
        if (style.Font.Bold)
          writer.Style(HtmlTextWriterStyle.FontWeight, "bold");
        if (style.Font.Underline)
        {
          writer.Style(HtmlTextWriterStyle.TextDecoration, "underline");
        }
        else if (style.Font.Strikeout)
        {
          writer.Style(HtmlTextWriterStyle.TextDecoration, "line-through");
        }
      }
      if (style.ForeColor != System.Drawing.Color.Empty)
      {
        writer.Style(HtmlTextWriterStyle.Color, HtmlColor(style.ForeColor));
      }
      if (style.Indent > 0 || style.Padding.All > 0)
      {
        writer.Style(HtmlTextWriterStyle.Padding, string.Format("{0}px {1}px {2}px {3}px", style.Padding.Top, style.Padding.Right, style.Padding.Bottom, style.Padding.Left + style.Indent * 5));
      }
    }
    private string HtmlColor(System.Drawing.Color color)
    {
      return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
    }

    public override Table.IFormattedTableWriter Title(string name)
    {
      EnsureStart();
      //_writer.Element(HtmlTextWriterTag.Caption, name);
      return this;
    }
  }
}
