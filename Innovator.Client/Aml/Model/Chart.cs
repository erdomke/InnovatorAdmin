using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Chart </summary>
  public class Chart : Item
  {
    protected Chart() { }
    public Chart(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>background_style</c> property of the item</summary>
    public IProperty_Text BackgroundStyle()
    {
      return this.Property("background_style");
    }
    /// <summary>Retrieve the <c>bar_spacing</c> property of the item</summary>
    public IProperty_Number BarSpacing()
    {
      return this.Property("bar_spacing");
    }
    /// <summary>Retrieve the <c>border_bottom</c> property of the item</summary>
    public IProperty_Number BorderBottom()
    {
      return this.Property("border_bottom");
    }
    /// <summary>Retrieve the <c>border_left</c> property of the item</summary>
    public IProperty_Number BorderLeft()
    {
      return this.Property("border_left");
    }
    /// <summary>Retrieve the <c>border_right</c> property of the item</summary>
    public IProperty_Number BorderRight()
    {
      return this.Property("border_right");
    }
    /// <summary>Retrieve the <c>border_top</c> property of the item</summary>
    public IProperty_Number BorderTop()
    {
      return this.Property("border_top");
    }
    /// <summary>Retrieve the <c>chart_type</c> property of the item</summary>
    public IProperty_Text ChartType()
    {
      return this.Property("chart_type");
    }
    /// <summary>Retrieve the <c>height</c> property of the item</summary>
    public IProperty_Number Height()
    {
      return this.Property("height");
    }
    /// <summary>Retrieve the <c>legend</c> property of the item</summary>
    public IProperty_Boolean Legend()
    {
      return this.Property("legend");
    }
    /// <summary>Retrieve the <c>legend_box_style</c> property of the item</summary>
    public IProperty_Text LegendBoxStyle()
    {
      return this.Property("legend_box_style");
    }
    /// <summary>Retrieve the <c>legend_height</c> property of the item</summary>
    public IProperty_Number LegendHeight()
    {
      return this.Property("legend_height");
    }
    /// <summary>Retrieve the <c>legend_text_style</c> property of the item</summary>
    public IProperty_Text LegendTextStyle()
    {
      return this.Property("legend_text_style");
    }
    /// <summary>Retrieve the <c>legend_width</c> property of the item</summary>
    public IProperty_Number LegendWidth()
    {
      return this.Property("legend_width");
    }
    /// <summary>Retrieve the <c>legend_x</c> property of the item</summary>
    public IProperty_Number LegendX()
    {
      return this.Property("legend_x");
    }
    /// <summary>Retrieve the <c>legend_y</c> property of the item</summary>
    public IProperty_Number LegendY()
    {
      return this.Property("legend_y");
    }
    /// <summary>Retrieve the <c>marker_size</c> property of the item</summary>
    public IProperty_Number MarkerSize()
    {
      return this.Property("marker_size");
    }
    /// <summary>Retrieve the <c>marker_style</c> property of the item</summary>
    public IProperty_Text MarkerStyle()
    {
      return this.Property("marker_style");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>radius</c> property of the item</summary>
    public IProperty_Number Radius()
    {
      return this.Property("radius");
    }
    /// <summary>Retrieve the <c>title</c> property of the item</summary>
    public IProperty_Text Title()
    {
      return this.Property("title");
    }
    /// <summary>Retrieve the <c>title_style</c> property of the item</summary>
    public IProperty_Text TitleStyle()
    {
      return this.Property("title_style");
    }
    /// <summary>Retrieve the <c>width</c> property of the item</summary>
    public IProperty_Number Width()
    {
      return this.Property("width");
    }
    /// <summary>Retrieve the <c>x_axis</c> property of the item</summary>
    public IProperty_Boolean XAxis()
    {
      return this.Property("x_axis");
    }
    /// <summary>Retrieve the <c>x_axis_label</c> property of the item</summary>
    public IProperty_Text XAxisLabel()
    {
      return this.Property("x_axis_label");
    }
    /// <summary>Retrieve the <c>x_axis_label_style</c> property of the item</summary>
    public IProperty_Text XAxisLabelStyle()
    {
      return this.Property("x_axis_label_style");
    }
    /// <summary>Retrieve the <c>x_axis_style</c> property of the item</summary>
    public IProperty_Text XAxisStyle()
    {
      return this.Property("x_axis_style");
    }
    /// <summary>Retrieve the <c>x_axis_value_label_style</c> property of the item</summary>
    public IProperty_Text XAxisValueLabelStyle()
    {
      return this.Property("x_axis_value_label_style");
    }
    /// <summary>Retrieve the <c>x_axis_value_labels</c> property of the item</summary>
    public IProperty_Boolean XAxisValueLabels()
    {
      return this.Property("x_axis_value_labels");
    }
    /// <summary>Retrieve the <c>x_grid</c> property of the item</summary>
    public IProperty_Boolean XGrid()
    {
      return this.Property("x_grid");
    }
    /// <summary>Retrieve the <c>x_grid_interval</c> property of the item</summary>
    public IProperty_Number XGridInterval()
    {
      return this.Property("x_grid_interval");
    }
    /// <summary>Retrieve the <c>x_grid_style</c> property of the item</summary>
    public IProperty_Text XGridStyle()
    {
      return this.Property("x_grid_style");
    }
    /// <summary>Retrieve the <c>x_max</c> property of the item</summary>
    public IProperty_Number XMax()
    {
      return this.Property("x_max");
    }
    /// <summary>Retrieve the <c>x_min</c> property of the item</summary>
    public IProperty_Number XMin()
    {
      return this.Property("x_min");
    }
    /// <summary>Retrieve the <c>y_axis</c> property of the item</summary>
    public IProperty_Boolean YAxis()
    {
      return this.Property("y_axis");
    }
    /// <summary>Retrieve the <c>y_axis_label</c> property of the item</summary>
    public IProperty_Text YAxisLabel()
    {
      return this.Property("y_axis_label");
    }
    /// <summary>Retrieve the <c>y_axis_label_style</c> property of the item</summary>
    public IProperty_Text YAxisLabelStyle()
    {
      return this.Property("y_axis_label_style");
    }
    /// <summary>Retrieve the <c>y_axis_style</c> property of the item</summary>
    public IProperty_Text YAxisStyle()
    {
      return this.Property("y_axis_style");
    }
    /// <summary>Retrieve the <c>y_axis_value_label_style</c> property of the item</summary>
    public IProperty_Text YAxisValueLabelStyle()
    {
      return this.Property("y_axis_value_label_style");
    }
    /// <summary>Retrieve the <c>y_axis_value_labels</c> property of the item</summary>
    public IProperty_Boolean YAxisValueLabels()
    {
      return this.Property("y_axis_value_labels");
    }
    /// <summary>Retrieve the <c>y_grid</c> property of the item</summary>
    public IProperty_Boolean YGrid()
    {
      return this.Property("y_grid");
    }
    /// <summary>Retrieve the <c>y_grid_interval</c> property of the item</summary>
    public IProperty_Number YGridInterval()
    {
      return this.Property("y_grid_interval");
    }
    /// <summary>Retrieve the <c>y_grid_style</c> property of the item</summary>
    public IProperty_Text YGridStyle()
    {
      return this.Property("y_grid_style");
    }
    /// <summary>Retrieve the <c>y_max</c> property of the item</summary>
    public IProperty_Number YMax()
    {
      return this.Property("y_max");
    }
    /// <summary>Retrieve the <c>y_min</c> property of the item</summary>
    public IProperty_Number YMin()
    {
      return this.Property("y_min");
    }
  }
}