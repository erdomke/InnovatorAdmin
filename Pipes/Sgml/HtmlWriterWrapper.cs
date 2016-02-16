using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Pipes.Sgml
{
  public class HtmlWriterWrapper : ISgmlWriter
  {
    private ISgmlWriter _writer;
    private StringBuilder _style = new StringBuilder();

    public HtmlWriterWrapper(ISgmlWriter writer)
    {
      _writer = writer;
    }

    public HtmlWriterWrapper Style(HtmlTextWriterStyle style, object value)
    {
      return Style(_styles[style], value);
    }
    public HtmlWriterWrapper Style(string style, object value)
    {
      _style.Append(style).Append(':').Append((value ?? "").ToString()).Append(';');
      return this;
    }

#region "ISgmlWriter Implementation"
    public ISgmlWriter Attribute(string name, object value)
    {
      CloseStyle();
      _writer.Attribute(name, value);
      return this;
    }
    public ISgmlWriter Attribute(string name, string ns, object value)
    {
      CloseStyle();
      _writer.Attribute(name, ns, value);
      return this;
    }
    public ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      CloseStyle();
      _writer.Attribute(prefix, name, ns, value);
      return this;
    }
    public ISgmlWriter Comment(string value)
    {
      CloseStyle();
      _writer.Comment(value);
      return this;
    }
    public ISgmlWriter Element(string name, object value)
    {
      CloseStyle();
      _writer.Element(name, value);
      return this;
    }
    public ISgmlWriter Element(string name)
    {
      CloseStyle();
      _writer.Element(name);
      return this;
    }
    public ISgmlWriter ElementEnd()
    {
      CloseStyle();
      _writer.ElementEnd();
      return this;
    }
    public void Flush()
    {
      CloseStyle();
      _writer.Flush();
    }
    public ISgmlWriter NsElement(string name, string ns)
    {
      CloseStyle();
      _writer.NsElement(name, ns);
      return this;
    }
    public ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      CloseStyle();
      _writer.NsElement(prefix, name, ns);
      return this;
    }
    public ISgmlWriter Raw(string value)
    {
      CloseStyle();
      _writer.Raw(value);
      return this;
    }
    public ISgmlWriter Value(object value)
    {
      CloseStyle();
      _writer.Value(value);
      return this;
    }
#endregion

    private void CloseStyle()
    {
      if (_style.Length > 0) _writer.Attribute("style", _style.ToString());
      _style.Length = 0;
    }
        
    private static Dictionary<HtmlTextWriterStyle, string> _styles;

    static HtmlWriterWrapper() {
      _styles = new Dictionary<HtmlTextWriterStyle, string>();
      _styles[HtmlTextWriterStyle.BackgroundColor] = "background-color";
      _styles[HtmlTextWriterStyle.BackgroundImage] = "background-image";
      _styles[HtmlTextWriterStyle.BorderCollapse] = "border-collapse";
      _styles[HtmlTextWriterStyle.BorderColor] = "border-color";
      _styles[HtmlTextWriterStyle.BorderStyle] = "border-style";
      _styles[HtmlTextWriterStyle.BorderWidth] = "border-width";
      _styles[HtmlTextWriterStyle.Color] = "color";
      _styles[HtmlTextWriterStyle.Cursor] = "cursor";
      _styles[HtmlTextWriterStyle.Direction] = "direction";
      _styles[HtmlTextWriterStyle.Display] = "display";
      _styles[HtmlTextWriterStyle.Filter] = "filter";
      _styles[HtmlTextWriterStyle.FontFamily] = "font-family";
      _styles[HtmlTextWriterStyle.FontSize] = "font-size";
      _styles[HtmlTextWriterStyle.FontStyle] = "font-style";
      _styles[HtmlTextWriterStyle.FontVariant] = "font-variant";
      _styles[HtmlTextWriterStyle.FontWeight] = "font-weight";
      _styles[HtmlTextWriterStyle.Height] = "height";
      _styles[HtmlTextWriterStyle.Left] = "left";
      _styles[HtmlTextWriterStyle.ListStyleImage] = "list-style-image";
      _styles[HtmlTextWriterStyle.ListStyleType] = "list-style-type";
      _styles[HtmlTextWriterStyle.Margin] = "margin";
      _styles[HtmlTextWriterStyle.MarginBottom] = "margin-bottom";
      _styles[HtmlTextWriterStyle.MarginLeft] = "margin-left";
      _styles[HtmlTextWriterStyle.MarginRight] = "margin-right";
      _styles[HtmlTextWriterStyle.MarginTop] = "margin-top";
      _styles[HtmlTextWriterStyle.OverflowX] = "overflow-x";
      _styles[HtmlTextWriterStyle.OverflowY] = "overflow-y";
      _styles[HtmlTextWriterStyle.Overflow] = "overflow";
      _styles[HtmlTextWriterStyle.Padding] = "padding";
      _styles[HtmlTextWriterStyle.PaddingBottom] = "padding-bottom";
      _styles[HtmlTextWriterStyle.PaddingLeft] = "padding-left";
      _styles[HtmlTextWriterStyle.PaddingRight] = "padding-right";
      _styles[HtmlTextWriterStyle.PaddingTop] = "padding-top";
      _styles[HtmlTextWriterStyle.Position] = "position";
      _styles[HtmlTextWriterStyle.TextAlign] = "text-align";
      _styles[HtmlTextWriterStyle.TextDecoration] = "text-decoration";
      _styles[HtmlTextWriterStyle.TextOverflow] = "text-overflow";
      _styles[HtmlTextWriterStyle.Top] = "top";
      _styles[HtmlTextWriterStyle.VerticalAlign] = "vertical-align";
      _styles[HtmlTextWriterStyle.Visibility] = "visibility";
      _styles[HtmlTextWriterStyle.Width] = "width";
      _styles[HtmlTextWriterStyle.WhiteSpace] = "white-space";
      _styles[HtmlTextWriterStyle.ZIndex] = "z-index";
    }
  }
}
