using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;
using S = DocumentFormat.OpenXml.Spreadsheet;

namespace Pipes.Data.Table
{
  public class ExcelTableWriter : Pipes.Data.Table.BaseFormattedTableWriter, IWriter<Stream>, IDisposable
  {
    private int _cellIndex = 0;
    private List<Table.IColumn> _columns = new List<Table.IColumn>();
    private int _rowCount = 0;
    private string _rowSpan;
    private SharedStrings _sharedStrings = new SharedStrings();
    private TableWriterSettings _settings;
    private bool _settingsHandled;

    private BordersHelper _borders = new BordersHelper();
    private FontsHelper _fonts = new FontsHelper();
    private FillsHelper _fills = new FillsHelper();
    private StylesHelper _styles;
    private string _sheetName;

    private OpenXmlWriter _writer;
    private SpreadsheetDocument _spreadsheetDoc;
    private WorkbookPart _workbookpart;
    private WorksheetPart _worksheetPart;
    private ImageHelper _images;
    private HyperlinkHelper _hyperlinks;

    public ExcelTableWriter() { }
    public ExcelTableWriter(Stream stream)
    {
      Initialize(stream);
    }

    public Action<OpenXmlWriter> WorksheetCallback { get; set; }
    public object Parent { get; set; }

    public T Initialize<T>(T coreWriter) where T : Stream
    {
      // Add a WorkbookPart to the document.
      _spreadsheetDoc = SpreadsheetDocument.Create(coreWriter, SpreadsheetDocumentType.Workbook);
      _workbookpart = _spreadsheetDoc.AddWorkbookPart();
      _workbookpart.Workbook = new Workbook();

      // Add a WorksheetPart to the WorkbookPart.
      _worksheetPart = _workbookpart.AddNewPart<WorksheetPart>("rId1");
      _images = new ImageHelper(_worksheetPart.AddNewPart<DrawingsPart>("rId1"), _columns);
      _hyperlinks = new HyperlinkHelper(_worksheetPart);
      _writer = OpenXmlWriter.Create(_worksheetPart);
      _writer.WriteStartElement(new Worksheet());

      InitializeSettingData();
      return coreWriter;
    }

    public override void InitializeSettings(TableWriterSettings settings)
    {
      _settings = settings;

      InitializeSettingData();
    }

    private void InitializeSettingData()
    {
      if (_settings != null && _writer != null && !_settingsHandled)
      {
        _settingsHandled = true;

        _styles = new StylesHelper(_borders, _fonts, _fills, _settings.DefaultStyle);
        if (_settings.RepeatHeader)
        {
          _writer.WriteStartElement(new SheetViews());
          _writer.WriteStartElement(new SheetView(), new OpenXmlAttribute[] {
              new OpenXmlAttribute("tabSelected", null, "1"),
              new OpenXmlAttribute("workbookViewId", null, "0")
            });
          _writer.WriteStartElement(new Pane(), new OpenXmlAttribute[] {
              new OpenXmlAttribute("ySplit", null, "1"),
              new OpenXmlAttribute("topLeftCell", null, "A2"),
              new OpenXmlAttribute("activePane", null, "bottomLeft"),
              new OpenXmlAttribute("state", null, "frozenSplit")
            });
          _writer.WriteEndElement();
          _writer.WriteEndElement();
          _writer.WriteEndElement();
        }
      }
    }

    public override void Flush()
    {
      if (_spreadsheetDoc != null)
      {
        /*_writer.WriteStartElement(new Dimension(), new OpenXmlAttribute[] { new OpenXmlAttribute("ref", null, "A1:C2") });
        _writer.WriteEndElement();*/

        _writer.WriteEndElement(); //Finish the sheetData tag

        // Add Sheets to the Workbook (if they haven't been previously added)
        AddSheetName("Sheet1");

        if (_settings != null)
        {
          if (_settings.AutoFilter)
          {
            _writer.WriteStartElement(new AutoFilter(), new OpenXmlAttribute[] {
              new OpenXmlAttribute("ref", null, "A1:" + CellAddress.GetColumnDesignator(_cellIndex - 1) + _rowCount.ToString())
            });
            _writer.WriteEndElement();

            var defNames = _workbookpart.Workbook.AppendChild(new DefinedNames());
            var defName = defNames.AppendChild(new DefinedName() {
              Hidden = true,
              LocalSheetId = 0,
              Name = "_xlnm._FilterDatabase",
              Text = "'" + _sheetName + "'!$A$1:$" + CellAddress.GetColumnDesignator(_cellIndex - 1) + "$" + _rowCount.ToString()
            });
          }
        }

        if (WorksheetCallback != null)
          WorksheetCallback.Invoke(_writer);

        if (_hyperlinks.Count > 0)
        {
          _hyperlinks.Render(_writer);
        }
        if (_images.Count > 0)
        {
          _writer.WriteStartElement(new Drawing(), new OpenXmlAttribute[] {
            new OpenXmlAttribute("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1")
          });
          _writer.WriteEndElement();
        }

        _writer.WriteEndElement();

        _writer.Close();

        // Add the Styles
        var workbookStylesPart = _workbookpart.AddNewPart<WorkbookStylesPart>("rId3");
        var styleSheet = new Stylesheet();

        styleSheet.Append(_fonts.Part);
        styleSheet.Append(_fills.Part);
        styleSheet.Append(_borders.Part);

        var cellStyleFormats = new CellStyleFormats() { Count = (UInt32Value)1U };
        var cellFormat = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyNumberFormat = false, ApplyBorder = false, ApplyAlignment = false };
        cellStyleFormats.Append(cellFormat);
        styleSheet.Append(cellStyleFormats);

        styleSheet.Append(_styles.Part);

        var cellStyles = new CellStyles() { Count = (UInt32Value)1U };
        var cellStyle = new S.CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
        cellStyles.Append(cellStyle);
        styleSheet.Append(cellStyles);

        workbookStylesPart.Stylesheet = styleSheet;

        // Deal with the shared strings
        var sharedStringPart = _workbookpart.AddNewPart<SharedStringTablePart>("rId4");
        sharedStringPart.SharedStringTable = _sharedStrings.Part;

        _workbookpart.Workbook.Save();

        // Close the document.
        _spreadsheetDoc.Close();
        _spreadsheetDoc.Dispose();
        _spreadsheetDoc = null;
      }
    }

    private void AddSheetName(string name)
    {
      if (_sheetName == null)
      {
        _sheetName = GetSheetName(name);


        var views = _workbookpart.Workbook.AppendChild(new BookViews());
        var view = new WorkbookView { WindowHeight = 6855, WindowWidth = 14055, XWindow = 510, YWindow = 555 };
        views.Append(view);

        var sheets = _workbookpart.Workbook.AppendChild(new Sheets());
        var sheet = new Sheet() { Id = _spreadsheetDoc.WorkbookPart.GetIdOfPart(_worksheetPart), SheetId = 1, Name = _sheetName };
        sheets.Append(sheet);
      }
    }

    public static string GetSheetName(string name)
    {
      var result = new char[name.Length];
      var j = 0;
      for (var i = 0; i < name.Length; i++)
      {
        switch (name[i])
        {
          case '*':
          case '[':
          case ']':
          case '\\':
          case '/':
          case ':':
          case '?':
            // Ignore these items;
            break;
          case '\'':
            if (j != 0)
            {
              result[j] = name[i];
              j++;
            }
            break;
          default:
            result[j] = name[i];
            j++;
            break;
        }
      }

      if (j > 31)
      {
        // Configure an ellipsis
        result[28] = '.';
        result[29] = '.';
        result[30] = '.';
      }

      return new string(result, 0, Math.Min(j, 31));
    }

    private static string Ellipsis(string value, int charCount)
    {
      if (string.IsNullOrEmpty(value) || charCount < 4 || value.Length <= charCount)
      {
        return value;
      }
      else
      {
        return value.Substring(0, charCount - 3) + "...";
      }
    }

    public override Table.IFormattedTableWriter Title(string name)
    {
      AddSheetName(name);
      return this;
    }

    public override Table.ITableWriter Row()
    {
      if (_part == ReportParts.Data)
      {
        _rowCount++;
        _cellIndex = 0;

        _writer.WriteStartElement(new S.Row(), new OpenXmlAttribute[] {
          new OpenXmlAttribute("r", null, _rowCount.ToString()),
          new OpenXmlAttribute("spans", null, _rowSpan)
        });
      }
      return this;
    }
    public override Table.ITableWriter RowEnd()
    {
      if (_part == ReportParts.Data)
      {
        _writer.WriteEndElement();
      }
      return this;
    }

    private void Cell(object value, Table.ICellStyle style, object formattedValue)
    {
      var numFmtId = -1;
      var cellRef = CellAddress.GetColumnDesignator(_cellIndex) + _rowCount.ToString();
      string strVal = "";
      var attr = new List<OpenXmlAttribute>() {
        new OpenXmlAttribute("r", null, cellRef)
      };

      if (value == null)
      {
        // Do Nothing
      }
      else if (value is Int16 || value is Int32 || value is Int64 ||
        value is UInt16 || value is UInt32 || value is UInt64 ||
        value is double || value is float || value is decimal)
      {
        numFmtId = 0;
        attr.Add(new OpenXmlAttribute("t", null, "n"));
        strVal = value.ToString();
      }
      else if (value is bool)
      {
        attr.Add(new OpenXmlAttribute("t", null, "b"));
        strVal = ((bool)value ? "1": "0");
      }
      else if (value is DateTime)
      {
        var dateVal = (DateTime)value;
        numFmtId = (dateVal.Date == dateVal ? 14: 22);
        strVal = dateVal.ToOADate().ToString();
      }
      else if (value is System.Drawing.Image)
      {
        _images.AddImage((System.Drawing.Image)value, _cellIndex, _rowCount - 1, 0, 0);
        _writer.WriteStartElement(new S.Cell(), attr);
        _writer.WriteEndElement();
        _cellIndex++;
        return;
      }
      else if (value is Table.IHyperlink)
      {
        var hyperLink = value as Table.IHyperlink;
        attr.Add(new OpenXmlAttribute("t", null, "s"));
        strVal = _sharedStrings.Get(formattedValue == null ?
                                    hyperLink.Text :
                                    formattedValue.ToString()).ToString();
        _hyperlinks.Get(hyperLink.Target, cellRef);
      }
      else
      {
        attr.Add(new OpenXmlAttribute("t", null, "s"));
        strVal = _sharedStrings.Get(formattedValue == null ?
                                    value.ToString() :
                                    formattedValue.ToString()).ToString();
        var uri = value as Uri;
        if (uri != null) _hyperlinks.Get(uri, cellRef);
      }
      attr.Add(new OpenXmlAttribute("s", null, _styles.Get(style, numFmtId).ToString()));

      _writer.WriteStartElement(new S.Cell(), attr);
      _writer.WriteStartElement(new CellValue());
      _writer.WriteString(strVal);
      _writer.WriteEndElement();
      _writer.WriteEndElement();

      _cellIndex++;
    }

    public override Table.IFormattedTableWriter Cell(Table.ICell cell)
    {
      if (_part == ReportParts.Data)
      {
        Cell(cell.Value, cell.Style, cell.FormattedValue);
      }
      return this;
    }

    public override Table.IFormattedTableWriter Column(Table.IColumn column)
    {
      if (_part == ReportParts.Data)
      {
        _columns.Add(column);

        var attr = new List<OpenXmlAttribute>() {
          new OpenXmlAttribute("min", null, _columns.Count.ToString()),
          new OpenXmlAttribute("max", null, _columns.Count.ToString()),
          new OpenXmlAttribute("width", null, (column.Width / 7.0).ToString()),
          new OpenXmlAttribute("customWidth", null, "1")
        };
        if (!column.Visible) attr.Add(new OpenXmlAttribute("hidden", null, "1"));
        _writer.WriteStartElement(new S.Column(), attr);
        _writer.WriteEndElement();
      }
      return this;
    }
    public override Table.ITableWriter Head()
    {
      if (_part == ReportParts.Data)
      {
        _writer.WriteStartElement(new Columns());
      }
      return this;
    }
    public override Table.ITableWriter HeadEnd()
    {
      if (_part == ReportParts.Data)
      {
        _writer.WriteEndElement();
        _writer.WriteStartElement(new SheetData());

        _rowSpan = "1:" + _columns.Count.ToString();
        if (_settings.IncludeHeaders)
        {
          Row();
          foreach (var column in _columns)
          {
            Cell(column.Label, column.Style, null);
          }
          RowEnd();
        }
      }
      return this;
    }

    #region "Helper Classes"
    private static string ColorToString(System.Drawing.Color value)
    {
      return "FF" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2");
    }

    private class HyperlinkHelper
    {
      private Dictionary<Uri, string> _values = new Dictionary<Uri, string>();
      private List<KeyValuePair<string, string>> _refs = new List<KeyValuePair<string, string>>();
      private WorksheetPart _worksheetPart;

      public int Count { get { return _values.Count; } }

      public HyperlinkHelper(WorksheetPart worksheetPart)
      {
        _worksheetPart = worksheetPart;
      }

      public string Get(Uri value, string cellRef)
      {
        string id;
        if (!_values.TryGetValue(value, out id))
        {
          id = "uri" + _values.Count.ToString();
          _values[value] = id;
          _worksheetPart.AddHyperlinkRelationship(value, true, id);
        }
        _refs.Add(new KeyValuePair<string,string>(cellRef, id));
        return id;
      }

      public void Render(OpenXmlWriter writer)
      {
        writer.WriteStartElement(new Hyperlinks());
        foreach (var link in _refs)
        {
          writer.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Hyperlink(), new OpenXmlAttribute[] {
            new OpenXmlAttribute("ref", null, link.Key),
            new OpenXmlAttribute("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", link.Value)
          });
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }

    private class ImageHelper
    {
      private Dictionary<string, string> _values = new Dictionary<string, string>();
      private DrawingsPart _drawings;
      private DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing _worksheetDrawing;
      private IEnumerable<Table.IColumn> _columns;
      private int _instanceCount = 0;

      public int Count { get { return _values.Count; } }

      public ImageHelper(DrawingsPart drawings, IEnumerable<Table.IColumn> columns)
      {
        _drawings = drawings;
        _columns = columns;

        _worksheetDrawing = new DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing();
        _worksheetDrawing.AddNamespaceDeclaration("xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
        _worksheetDrawing.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        drawings.WorksheetDrawing = _worksheetDrawing;
      }

      private string Get(System.Drawing.Image value)
      {
        string hash;
        string imgId;
        using (var md5 = MD5.Create())
        {
          using (var stream = new MemoryStream())
          {
            value.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            hash = BitConverter.ToString(md5.ComputeHash(stream.ToArray())).Replace("-", "").ToLowerInvariant();

            if (!_values.TryGetValue(hash, out imgId))
            {
              stream.Position = 0;
              imgId = "rId" + _values.Count.ToString();
              var part = _drawings.AddNewPart<ImagePart>("image/png", imgId);
              part.FeedData(stream);
              stream.Close();
              _values[hash] = imgId;
            }
          }
        }

        return imgId;
      }

      public void AddImage(System.Drawing.Image value, int column, int row, int xOffset, int yOffset)
      {
        var imgWidth = (long)(value.Width) * (long)(System.Math.Truncate(914400.0 / value.HorizontalResolution));
        var imgHeight = (long)(value.Height) * (long)(System.Math.Truncate(914400.0 / value.VerticalResolution));

        TwoCellAnchor twoCellAnchor = new TwoCellAnchor();

        var fromMarker = new DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker();
        fromMarker.Append(new ColumnId() { Text = column.ToString() });
        fromMarker.Append(new ColumnOffset() { Text = xOffset.ToString() });
        fromMarker.Append(new RowId() { Text = row.ToString() });
        fromMarker.Append(new RowOffset() { Text = yOffset.ToString() });

        var toMarker = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ToMarker();
        toMarker.Append(new ColumnId() { Text = column.ToString() });
        toMarker.Append(new ColumnOffset() { Text = (xOffset + imgWidth).ToString() });
        toMarker.Append(new RowId() { Text = row.ToString() });
        toMarker.Append(new RowOffset() { Text = (yOffset + imgHeight).ToString() });

        var picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
        var nonVisualPictureProperties = new NonVisualPictureProperties();
        _instanceCount++;
        var nonVisualDrawingProperties = new NonVisualDrawingProperties() {
          Id = 1024U + (uint)_instanceCount,
          Name = "Picture " + _instanceCount.ToString()
        };

        var nonVisualPictureDrawingProperties = new NonVisualPictureDrawingProperties();
        var pictureLocks = new A.PictureLocks() { NoChangeAspect = true };

        nonVisualPictureDrawingProperties.Append(pictureLocks);

        nonVisualPictureProperties.Append(nonVisualDrawingProperties);
        nonVisualPictureProperties.Append(nonVisualPictureDrawingProperties);

        var blipFill = new BlipFill();

        var blip = new A.Blip() { Embed = Get(value) };
        blip.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

        var blipExtensionList = new A.BlipExtensionList();

        var blipExtension = new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

        var useLocalDpi = new A14.UseLocalDpi() { Val = false };
        useLocalDpi.AddNamespaceDeclaration("a14", "http://schemas.microsoft.com/office/drawing/2010/main");

        blipExtension.Append(useLocalDpi);

        blipExtensionList.Append(blipExtension);

        blip.Append(blipExtensionList);

        var stretch = new A.Stretch();
        var fillRectangle1 = new A.FillRectangle();

        stretch.Append(fillRectangle1);

        blipFill.Append(blip);
        blipFill.Append(stretch);

        var shapeProperties = new ShapeProperties();

        var transform2D = new A.Transform2D();
        var offset = new A.Offset() {
          X = (column > 0 ? (long)_columns.Take(column).Select(c => c.Width * 9525.7).Sum() : 0) + xOffset,
          Y = 190500L * row + yOffset
        };
        var extents = new A.Extents() {
          Cx = (long)(value.Width) * (long)(System.Math.Truncate(914400.0 / value.HorizontalResolution)),
          Cy = (long)(value.Height) * (long)(System.Math.Truncate(914400.0 / value.VerticalResolution))
        };

        transform2D.Append(offset);
        transform2D.Append(extents);

        var presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
        var adjustValueList = new A.AdjustValueList();

        presetGeometry.Append(adjustValueList);

        shapeProperties.Append(transform2D);
        shapeProperties.Append(presetGeometry);

        picture.Append(nonVisualPictureProperties);
        picture.Append(blipFill);
        picture.Append(shapeProperties);

        twoCellAnchor.Append(fromMarker);
        twoCellAnchor.Append(toMarker);
        twoCellAnchor.Append(picture);
        twoCellAnchor.Append(new ClientData());

        _worksheetDrawing.Append(twoCellAnchor);
      }
    }

    private class SharedStrings
    {
      private uint _usage;
      private Dictionary<string, int> _values = new Dictionary<string,int>();
      private SharedStringTable _part = new SharedStringTable();

      public SharedStringTable Part
      {
        get {
          _part.Count = _usage;
          _part.UniqueCount = (uint)_values.Count;
          return _part;
        }
      }

      public int Get(string value)
      {
        int index;
        if (!_values.TryGetValue(value, out index))
        {
          index = _values.Count;
          _values[value] = index;
          var item = new SharedStringItem();
          item.Append(new Text() { Text = value });
          _part.Append(item);
        }
        _usage++;
        return index;
      }

    }

    private class FillsHelper
    {
      private Dictionary<System.Drawing.Color, int> _values = new Dictionary<System.Drawing.Color, int>();
      private int _index = 0;
      private Fills _part = new Fills();

      public Fills Part
      {
        get
        {
          _part.Count = (uint)_index;
          return _part;
        }
      }

      public FillsHelper()
      {
        this.Get(System.Drawing.Color.Empty);
        var item = new Fill();
        item.Append(new PatternFill() { PatternType = PatternValues.Gray125 });
        _part.Append(item);
        _index++;
      }

      public int Get(System.Drawing.Color value)
      {
        int index;
        if (!_values.TryGetValue(value, out index))
        {
          index = _index;
          _index++;
          _values[value] = index;
          var item = new Fill();
          if (value == System.Drawing.Color.Empty)
          {
            item.Append(new PatternFill() { PatternType = PatternValues.None });
          }
          else
          {
            var patternFill = new PatternFill() { PatternType = PatternValues.Solid };
            patternFill.Append(new ForegroundColor() { Rgb = ColorToString(value) });
            patternFill.Append(new BackgroundColor() { Rgb = ColorToString(value) });
            item.Append(patternFill);
          }
          _part.Append(item);
        }
        return index;
      }
    }

    private class FontsHelper
    {
      private Dictionary<XlFont, int> _values = new Dictionary<XlFont, int>();
      private Fonts _part = new Fonts();

      public Fonts Part
      {
        get
        {
          _part.Count = (uint)_values.Count;
          return _part;
        }
      }

      public int Get(XlFont value)
      {
        int index;
        if (!_values.TryGetValue(value, out index))
        {
          index = _values.Count;
          _values[value] = index;

          var item = new Font();
          if (value.Font != null)
          {
            if (value.Font.Underline) item.Append(new Underline());
            if (value.Font.Italic) item.Append(new Italic());
            if (value.Font.Bold) item.Append(new Bold());
            if (value.Font.Strikeout) item.Append(new Strike());
          }
          item.Append(new FontSize() { Val = (value.Font == null ? 11 : value.Font.SizeInPoints) });
          item.Append(new Color() { Rgb = ColorToString(value.ForeColor) });
          item.Append(new FontName() { Val = (value.Font == null || string.IsNullOrEmpty(value.Font.Name) ? "Calibri" : value.Font.Name) });
          item.Append(new FontFamilyNumbering() { Val = 2 });

          _part.Append(item);
        }
        return index;
      }
    }

    private class BordersHelper
    {
      private Dictionary<XlBorder, int> _values = new Dictionary<XlBorder, int>();
      private Borders _part = new Borders();

      public Borders Part
      {
        get
        {
          _part.Count = (uint)_values.Count;
          return _part;
        }
      }

      private BorderStyleValues GetStyle(int width)
      {
        switch (width)
        {
          case 0:
            return BorderStyleValues.None;
          case 1:
            return BorderStyleValues.Thin;
          case 2:
            return BorderStyleValues.Medium;
          default:
            return BorderStyleValues.Thick;
        }
      }

      public int Get(XlBorder value)
      {
        int index;
        if (!_values.TryGetValue(value, out index))
        {
          index = _values.Count;
          _values[value] = index;

          var item = new Border();
          if (value.BorderWidth < 1 || value.BorderSides == BorderSides.None || value.BorderSides == BorderSides.NotSet)
          {
            item.Append(new LeftBorder());
            item.Append(new RightBorder());
            item.Append(new TopBorder());
            item.Append(new BottomBorder());
          }
          else
          {
            if ((value.BorderSides & BorderSides.Left) > 0)
              item.Append(new LeftBorder()
              {
                Color = new Color() { Rgb = ColorToString(value.BorderColor) },
                Style = GetStyle(value.BorderWidth)
              });
            if ((value.BorderSides & BorderSides.Right) > 0)
              item.Append(new RightBorder()
              {
                Color = new Color() { Rgb = ColorToString(value.BorderColor) },
                Style = GetStyle(value.BorderWidth)
              });
            if ((value.BorderSides & BorderSides.Top) > 0)
              item.Append(new TopBorder()
              {
                Color = new Color() { Rgb = ColorToString(value.BorderColor) },
                Style = GetStyle(value.BorderWidth)
              });
            if ((value.BorderSides & BorderSides.Bottom) > 0)
              item.Append(new BottomBorder()
              {
                Color = new Color() { Rgb = ColorToString(value.BorderColor) },
                Style = GetStyle(value.BorderWidth)
              });
          }
          item.Append(new DiagonalBorder());
          _part.Append(item);
        }
        return index;
      }
    }

    private class StylesHelper
    {
      private Dictionary<XlStyle, int> _values = new Dictionary<XlStyle, int>();
      private CellFormats _part = new CellFormats();
      private BordersHelper _borders;
      private FillsHelper _fills;
      private FontsHelper _fonts;
      private ICellStyle _defaultStyle;

      public StylesHelper(BordersHelper borders, FontsHelper fonts, FillsHelper fills, ICellStyle defaultStyle)
      {
        _borders = borders;
        _fonts = fonts;
        _fills = fills;
        this.Get(new Table.CellStyle(), 0);
        _defaultStyle = defaultStyle;
      }

      public CellFormats Part
      {
        get
        {
          _part.Count = (uint)_values.Count;
          return _part;
        }
      }

      public int Get(Table.ICellStyle style, int numFtmId)
      {
        style = CellStyle.GetMerged(_defaultStyle, style);

        style = style ?? new Table.CellStyle();
        var value = new XlStyle()
        {
          NumFmtId = numFtmId,
          BorderId = _borders.Get(new XlBorder()
          {
            BorderColor = style.BorderColor,
            BorderSides = style.BorderSides,
            BorderWidth = style.BorderWidth
          }),
          FillId = _fills.Get(style.BackColor),
          FontId = _fonts.Get(new XlFont() { Font = style.Font, ForeColor = style.ForeColor}),
          Alignment = style.Alignment,
          Indent = style.Indent
        };

        int index;
        if (!_values.TryGetValue(value, out index))
        {
          index = _values.Count;
          _values[value] = index;

          var format = new CellFormat()
          {
            FormatId = 0U,
            NumberFormatId = (uint)(value.NumFmtId > 0 ? value.NumFmtId : 0),
            FontId = (uint)value.FontId,
            FillId = (uint)value.FillId,
            BorderId = (uint)value.BorderId,
            ApplyFill = true,
            ApplyAlignment = true,
            ApplyProtection = true,
            ApplyNumberFormat = (value.NumFmtId >= 0)
          };
          var alignment = new Alignment();
          switch (value.Alignment)
          {
            case ContentAlignment.BottomCenter:
              alignment.Horizontal = HorizontalAlignmentValues.Center;
              alignment.Vertical = VerticalAlignmentValues.Bottom;
              break;
            case ContentAlignment.BottomRight:
              alignment.Horizontal = HorizontalAlignmentValues.Right;
              alignment.Vertical = VerticalAlignmentValues.Bottom;
              break;
            case ContentAlignment.MiddleCenter:
              alignment.Horizontal = HorizontalAlignmentValues.Center;
              alignment.Vertical = VerticalAlignmentValues.Center;
              break;
            case ContentAlignment.MiddleLeft:
              alignment.Horizontal = HorizontalAlignmentValues.Left;
              alignment.Vertical = VerticalAlignmentValues.Center;
              break;
            case ContentAlignment.MiddleRight:
              alignment.Horizontal = HorizontalAlignmentValues.Right;
              alignment.Vertical = VerticalAlignmentValues.Center;
              break;
            case ContentAlignment.TopCenter:
              alignment.Horizontal = HorizontalAlignmentValues.Center;
              alignment.Vertical = VerticalAlignmentValues.Top;
              break;
            case ContentAlignment.TopLeft:
              alignment.Horizontal = HorizontalAlignmentValues.Left;
              alignment.Vertical = VerticalAlignmentValues.Top;
              break;
            case ContentAlignment.TopRight:
              alignment.Horizontal = HorizontalAlignmentValues.Right;
              alignment.Vertical = VerticalAlignmentValues.Top;
              break;
            default: //case DataGridViewContentAlignment.BottomLeft
              alignment.Horizontal = HorizontalAlignmentValues.Left;
              alignment.Vertical = VerticalAlignmentValues.Bottom;
              break;
          }
          if (value.Indent > 0) alignment.Indent = (uint)value.Indent;
          format.Append(alignment);
          _part.Append(format);
        }
        return index;
      }


    }

    private class XlStyle
    {
      public int NumFmtId { get; set; }
      public int FontId { get; set; }
      public int FillId { get; set; }
      public int BorderId { get; set; }
      public int Indent { get; set; }
      public ContentAlignment Alignment { get; set; }

      public override int GetHashCode()
      {
        return this.NumFmtId ^ this.FontId ^ this.FillId ^ this.BorderId ^ (int)this.Alignment ^ this.Indent;
      }

      public override bool Equals(object obj)
      {
        var style = obj as XlStyle;
        if (style == null) return false;
        return Equals(style);
      }
      public bool Equals(XlStyle obj)
      {
        return this.NumFmtId == obj.NumFmtId && this.FontId == obj.FontId &&
          this.FillId == obj.FillId && this.BorderId == obj.BorderId &&
          this.Alignment == obj.Alignment && this.Indent == obj.Indent;
      }
    }

    private class XlFont
    {
      public Table.IFontStyle Font { get; set; }
      public System.Drawing.Color ForeColor { get; set; }

      public override int GetHashCode()
      {
        return (this.Font == null ? 0 : this.Font.Bold.GetHashCode() ^ this.Font.Italic.GetHashCode() ^
          (this.Font.Name ?? "").GetHashCode() ^ this.Font.SizeInPoints.GetHashCode() ^
          this.Font.Strikeout.GetHashCode() ^ this.Font.Underline.GetHashCode()) ^
          this.ForeColor.GetHashCode();
      }

      public override bool Equals(object obj)
      {
        var style = obj as XlFont;
        if (style == null) return false;
        return Equals(style);
      }
      public bool Equals(XlFont obj)
      {
        return ((this.Font == null && obj.Font == null) ||
          (this.Font != null && obj.Font != null &&
          this.Font.Bold == obj.Font.Bold &&
          this.Font.Italic == obj.Font.Italic &&
          this.Font.Name == obj.Font.Name &&
          this.Font.SizeInPoints == obj.Font.SizeInPoints &&
          this.Font.Strikeout == obj.Font.Strikeout &&
          this.Font.Underline == obj.Font.Underline)) &&
          this.ForeColor == obj.ForeColor;
      }
    }

    private class XlBorder
    {
      public System.Drawing.Color BorderColor { get; set; }
      public BorderSides BorderSides { get; set; }
      public int BorderWidth { get; set; }

      public override int GetHashCode()
      {
        return this.BorderColor.GetHashCode() ^ (int)this.BorderSides ^ this.BorderWidth;
      }

      public override bool Equals(object obj)
      {
        var style = obj as XlBorder;
        if (style == null) return false;
        return Equals(style);
      }
      public bool Equals(XlBorder obj)
      {
        return this.BorderColor == obj.BorderColor &&
          this.BorderSides == obj.BorderSides &&
          this.BorderWidth == obj.BorderWidth;
      }
    }
    #endregion

    public void Dispose()
    {
      Flush();
      if (_spreadsheetDoc != null) _spreadsheetDoc.Dispose();
      _spreadsheetDoc = null;
    }

#if DEBUG
    public static void Test()
    {
      using (var stream = new FileStream(@"C:\test.xlsx", FileMode.Create, FileAccess.ReadWrite))
      {
        using (var writer = new ExcelTableWriter(stream))
        {
          var style = new Table.CellStyle()
          {
            BackColor = System.Drawing.Color.LightGray,
            Alignment = ContentAlignment.MiddleCenter
          };

          writer.InitializeSettings(new TableWriterSettings() { AutoFilter = true, RepeatHeader = true });

          writer.Head();
          writer.Column(new Table.Column("text") { Label = "String", Visible = true, Width = 200, Style = style });
          writer.Column(new Table.Column("date") { Label = "Date", Visible = true, Width = 200, Style = style });
          writer.Column(new Table.Column("number") { Label = "Number", Visible = true, Width = 200, Style = style });
          writer.Column(new Table.Column("number1") { Label = "Number", Visible = true, Width = 20, Style = style });
          writer.HeadEnd();

          writer.Row();
          writer.Cell(new Table.Cell("text", null) { Value = "a thing" });
          writer.Cell(new Table.Cell("date", null) { Value = DateTime.Now });
          writer.Cell(new Table.Cell("number", null) { Value = 52.3 });
          writer.Cell(new Table.Cell("number1", null) { Value = new System.Drawing.Bitmap(@"C:\Users\edomke\Documents\Local_Projects\ComponentTracker\Reference\ArasIcons\Final\Part_Released16.png") });
          writer.RowEnd();

          writer.Row();
          writer.Cell(new Table.Cell("text", null) { Value = "a second thing" });
          writer.Cell(new Table.Cell("date", null) { Value = DateTime.Today });
          writer.Cell(new Table.Cell("number", null) { Value = 15 });
          writer.Cell(new Table.Cell("number1", null) { Value = new Uri("http://teamlink.gentex.com") });
          writer.RowEnd();
        }
      }
    }
#endif

  }
}
