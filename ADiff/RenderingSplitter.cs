using ADiff.Api;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ADiff
{
  public class RenderingSplitter : GridSplitter
  {
    private enum DiffType
    {
      Normal,
      Add,
      Delete,
      Change
    }

    private IList<ListCompare> _diffs;
    private TextEditor _editLeft;
    private TextEditor _editRight;

    public void Setup(TextEditor editLeft, TextEditor editRight, IList<ListCompare> diffs)
    {
      _editLeft = editLeft;
      _editRight = editRight;

      _editLeft.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
      _editRight.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

      _diffs = diffs;
    }

    void TextView_ScrollOffsetChanged(object sender, EventArgs e)
    {
      this.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      if (_diffs != null)
      {
        var viewLeft = _editLeft.TextArea.TextView;
        var viewRight = _editRight.TextArea.TextView;

        viewLeft.EnsureVisualLines();
        viewRight.EnsureVisualLines();
        var leftFirst = viewLeft.VisualLines.First().FirstDocumentLine.LineNumber;
        var leftLast = viewLeft.VisualLines.Last().LastDocumentLine.LineNumber;
        var rightFirst = viewRight.VisualLines.First().FirstDocumentLine.LineNumber;
        var rightLast = viewRight.VisualLines.Last().LastDocumentLine.LineNumber;
        Func<ListCompare, bool> inRange = (c => Math.Abs(c.Base) >= leftFirst && Math.Abs(c.Base) <= leftLast
          || Math.Abs(c.Compare) >= rightFirst && Math.Abs(c.Compare) <= rightLast);

        var start = 0;
        while (start < _diffs.Count && !inRange(_diffs[start])) start++;
        if (start >= _diffs.Count) start--;
        var end = start;
        while (start >= 0 && GetDiffType(_diffs[start]) != DiffType.Normal) start--;
        while (end < _diffs.Count && inRange(_diffs[end])) end++;
        end--;
        while (end < _diffs.Count && GetDiffType(_diffs[end]) != DiffType.Normal) end++;

        var lastType = GetDiffType(_diffs[start]);
        DiffType currType;
        Geometry geom;
        drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), this.RenderSize)));
        for (var i = start + 1; i <= end; i++)
        {
          currType = GetDiffType(_diffs[i]);
          if (currType != lastType)
          {
            geom = GetPath(new Point(0, viewLeft.GetOrConstructVisualLine(viewLeft.Document.GetLineByNumber(Math.Abs(_diffs[i].Base))).VisualTop - viewLeft.VerticalOffset),
                           new Point(Width, viewRight.GetOrConstructVisualLine(viewRight.Document.GetLineByNumber(Math.Abs(_diffs[i].Compare))).VisualTop - viewRight.VerticalOffset));
            drawingContext.DrawGeometry(null, new Pen() { Thickness = 2, Brush = Brushes.Black }, geom);
          }
          lastType = currType;
        }
      }
    }

    private PathGeometry GetPath(Point start, Point end)
    {
      var pathFigure = new PathFigure() { StartPoint = start };
      var lineSegment = new LineSegment() { Point = end };
      var pathSegmentCollection = new PathSegmentCollection();
      pathSegmentCollection.Add(lineSegment);
      pathFigure.Segments = pathSegmentCollection;
      var pathFigureCollection = new PathFigureCollection();
      pathFigureCollection.Add(pathFigure);
      return new PathGeometry() { Figures = pathFigureCollection };
    }

    private DiffType GetDiffType(ListCompare compare)
    {
      if (compare.Base < 0) return DiffType.Add;
      if (compare.Compare < 0) return DiffType.Delete;
      if (compare.IsDifferent) return DiffType.Change;
      return DiffType.Normal;
    }
  }
}
