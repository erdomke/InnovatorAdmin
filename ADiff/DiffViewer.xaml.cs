using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ADiff
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class DiffViewer : UserControl
  {
    private enum DiffType
    {
      Normal,
      Add,
      Change,
      Delete
    }

    private List<ScrollAlignment> _alignments = new List<ScrollAlignment>();
    private IList<ListCompare> _compares;
    private double _lastLeftVertOffset;
    private double _lastRightVertOffset;
    private DrawingGroup _splitterDrawing;

    public string Left
    {
      get { return editLeft.Text; }
      set { editLeft.Text = value; }
    }
    public string Right
    {
      get { return editRight.Text; }
      set { editRight.Text = value; }
    }

    public DiffViewer()
    {
      InitializeComponent();

      ConfigureXmlEditor(editLeft);
      ConfigureXmlEditor(editRight);

      editLeft.TextArea.TextView.BackgroundRenderers.Add(new DiffBackground()
      {
        AddColor = Color.FromRgb(0xff, 0xcc, 0xcc),
        GetDiffType = i => GetLineDiff(c => c.Base == i, c => c.Compare)
      });
      editRight.TextArea.TextView.BackgroundRenderers.Add(new DiffBackground()
      {
        AddColor = Color.FromRgb(0xcc, 0xff, 0xcc),
        GetDiffType = i => GetLineDiff(c => c.Compare == i, c => c.Base)
      });

      _splitterDrawing = new DrawingGroup();
      splitter.Background = new DrawingBrush() { Drawing = _splitterDrawing };
    }

    public void SetDiffs(IEnumerable<ListCompare> compares)
    {
      if (!this.IsLoaded)
      {
        this.Loaded += (o, e) =>
        {
          SetDiffs(compares);
        };
        return;
      }

      _compares = compares.ToList();
      ListCompare compare;
      var lastBase = 1;
      var lastCompare = 1;

      _alignments.Clear();
      _alignments.Add(new ScrollAlignment());
      var lastAligned = true;
      bool currAligned;
      var leftView = editLeft.TextArea.TextView;
      var rightView = editRight.TextArea.TextView;
      VisualLine leftLine, rightLine;

      for (int i = 0; i < _compares.Count; i++)
      {
        currAligned = _compares[i].Base >= 0 && _compares[i].Compare >= 0;
        if (currAligned != lastAligned && i > 0)
        {
          if (currAligned)
          {
            _alignments.Add(new ScrollAlignment(
              leftView.GetOrConstructVisualLine(leftView.Document.GetLineByNumber(_compares[i].Base)).VisualTop,
              rightView.GetOrConstructVisualLine(rightView.Document.GetLineByNumber(_compares[i].Compare)).VisualTop,
              _alignments.Last()
            ));
          }
          else
          {
            var offset = (_compares[i].Base < 0 ? _compares[i].Compare - _compares[i - 1].Compare : _compares[i].Base - _compares[i - 1].Base) - 1;
            leftLine = leftView.GetOrConstructVisualLine(leftView.Document.GetLineByNumber(_compares[i - 1].Base + offset));
            rightLine = rightView.GetOrConstructVisualLine(rightView.Document.GetLineByNumber(_compares[i - 1].Compare + offset));
            _alignments.Add(new ScrollAlignment(
              leftLine.VisualTop + leftLine.Height,
              rightLine.VisualTop + rightLine.Height,
              _alignments.Last()
            ));
          }
        }
        lastAligned = currAligned;

        if (_compares[i].Base < 0)
        {
          if (i > 0 && _compares[i - 1].Base >= 0 && _compares[i - 1].Compare >= 0)
          {
            var offset = _compares[i].Compare - _compares[i - 1].Compare;
            lastBase += offset;
            lastCompare += offset;
          }
          compare = _compares[i]; // Create a clone
          compare.Base = -1 * lastBase;
          _compares[i] = compare;
        }
        else
        {
        }
        if (_compares[i].Compare < 0)
        {
          if (i > 0 && _compares[i - 1].Base >= 0 && _compares[i - 1].Compare >= 0)
          {
            var offset = _compares[i].Base - _compares[i - 1].Base;
            lastBase += offset;
            lastCompare += offset;
          }
          compare = _compares[i]; // Create a clone
          compare.Compare = -1 * lastCompare;
          _compares[i] = compare;
        }

        if (_compares[i].Base >= 0 && _compares[i].Compare >= 0)
        {
          lastBase = _compares[i].Base;
          lastCompare = _compares[i].Compare;
        }
      }

      if (_compares.Count > 0)
      {
        leftLine = leftView.GetOrConstructVisualLine(leftView.Document.GetLineByNumber(_compares.Last().Base));
        rightLine = rightView.GetOrConstructVisualLine(rightView.Document.GetLineByNumber(_compares.Last().Compare));
        _alignments.Add(new ScrollAlignment(
          leftLine.VisualTop + leftLine.Height,
          rightLine.VisualTop + rightLine.Height,
          _alignments.Last()
        ));
      }

      vertScroll.Minimum = 0;
      vertScroll.Maximum = _alignments.Last().Master;
      var leftScrollView = ((IScrollInfo)leftView).ScrollOwner;
      var rightScrollView = ((IScrollInfo)rightView).ScrollOwner;
      vertScroll.Visibility = (leftScrollView.ExtentHeight > leftScrollView.ViewportHeight || rightScrollView.ExtentHeight > rightScrollView.ViewportHeight)
        ? System.Windows.Visibility.Visible
        : System.Windows.Visibility.Hidden;
      horizScroll.Visibility = (leftScrollView.ExtentWidth > leftScrollView.ViewportWidth || rightScrollView.ExtentWidth > rightScrollView.ViewportWidth)
        ? System.Windows.Visibility.Visible
        : System.Windows.Visibility.Hidden;
      horizScroll.Minimum = 0;
      horizScroll.Maximum = Math.Max(leftScrollView.ExtentWidth, rightScrollView.ExtentWidth);
      UpdateScrollProperties(false);

      leftView.InvalidateLayer(KnownLayer.Background);
      rightView.InvalidateLayer(KnownLayer.Background);


    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);
      if (this.IsLoaded) UpdateScrollProperties(false, sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height, sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width);
    }

    private void ConfigureXmlEditor(TextEditor editor)
    {
      editor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
      editor.FontSize = 12.0;
      editor.Options.ConvertTabsToSpaces = true;
      editor.Options.EnableRectangularSelection = true;
      editor.Options.IndentationSize = 2;
      editor.ShowLineNumbers = true;
      editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".xml");

      editor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

      editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
    }

    private DiffType GetLineDiff(Func<ListCompare, bool> predicate, Func<ListCompare, int> other)
    {
      if (_compares == null) return DiffType.Normal;
      var compare = _compares.FirstOrDefault(predicate);
      if (other(compare) < 0) return DiffType.Add;
      if (compare.IsDifferent) return DiffType.Change;
      return DiffType.Normal;
    }
    private void HandleVerticalScroll()
    {
      var i = 0;
      while (i < _alignments.Count && vertScroll.Value > _alignments[i].Master) i++;
      if (i == 0)
      {
        _lastLeftVertOffset = 0;
        _lastRightVertOffset = 0;
      }
      else if (i >= _alignments.Count)
      {
        _lastLeftVertOffset = _alignments.Last().Left;
        _lastRightVertOffset = _alignments.Last().Right;
      }
      else
      {
        var prev = _alignments[i - 1];
        var curr = _alignments[i];

        var interpolate = (vertScroll.Value - prev.Master) / (curr.Master - prev.Master);
        _lastLeftVertOffset = prev.Left + interpolate * (curr.Left - prev.Left);
        _lastRightVertOffset = prev.Right + interpolate * (curr.Right - prev.Right);
      }

      editLeft.ScrollToVerticalOffset(_lastLeftVertOffset);
      editRight.ScrollToVerticalOffset(_lastRightVertOffset);
      UpdateScrollProperties(true);
    }

    private void UpdateScrollProperties(bool horizontalOnly, double vertFactor = 1.0, double horizFactor = 1.0)
    {
      var leftScrollView = ((IScrollInfo)editLeft.TextArea.TextView).ScrollOwner;
      var rightScrollView = ((IScrollInfo)editLeft.TextArea.TextView).ScrollOwner;
      horizScroll.ViewportSize = Math.Min(leftScrollView.ViewportWidth, rightScrollView.ViewportWidth) * horizFactor;
      horizScroll.Maximum = Math.Max(leftScrollView.ScrollableWidth, rightScrollView.ScrollableWidth);

      if (!horizontalOnly)
      {
        vertScroll.ViewportSize = leftScrollView.ViewportHeight * vertFactor;
      }
    }

    void TextView_ScrollOffsetChanged(object sender, EventArgs e)
    {
      var view = (TextView)sender;
      double change;
      if (sender == editLeft.TextArea.TextView)
      {
        change = view.VerticalOffset - _lastLeftVertOffset;
      }
      else
      {
        change = view.VerticalOffset - _lastRightVertOffset;
      }
      if (change != 0.0)
      {
        vertScroll.Value += change;
        HandleVerticalScroll();
      }

      RenderBrush();
      splitter.InvalidateVisual();
    }

    private void horizScroll_Scroll(object sender, ScrollEventArgs e)
    {
      editLeft.ScrollToHorizontalOffset(horizScroll.Value);
      editRight.ScrollToHorizontalOffset(horizScroll.Value);
    }

    private void vertScroll_Scroll(object sender, ScrollEventArgs e)
    {
      HandleVerticalScroll();
    }

    private class DiffBackground : IBackgroundRenderer
    {
      public Func<int, DiffType> GetDiffType { get; set; }
      public Color AddColor { get; set; }

      public void Draw(TextView textView, DrawingContext drawingContext)
      {
        if (textView.Document == null) return;

        textView.EnsureVisualLines();

        Color backColor;
        foreach (var visLine in textView.VisualLines)
        {
          backColor = default(Color);
          switch (GetDiffType(visLine.FirstDocumentLine.LineNumber))
          {
            case DiffType.Add:
              backColor = this.AddColor;
              break;
            case DiffType.Change:
              backColor = Color.FromRgb(0xcc, 0xcc, 0xff);
              break;
          }

          if (backColor != default(Color))
          {
            var backgroundGeometryBuilder = new BackgroundGeometryBuilder();
            var y = visLine.VisualTop - textView.ScrollOffset.Y;
            backgroundGeometryBuilder.AddRectangle(textView, new Rect(0.0, y, textView.ActualWidth, visLine.Height));
            var geometry = backgroundGeometryBuilder.CreateGeometry();
            if (geometry != null)
            {
              drawingContext.DrawGeometry(new SolidColorBrush(backColor), null, geometry);
            }
          }
        }
      }

      public KnownLayer Layer
      {
        get { return KnownLayer.Background; }
      }

      private static IEnumerable<DocumentLine> DocumentLines(VisualLine visLine)
      {
        var line = visLine.FirstDocumentLine;
        while (line != visLine.LastDocumentLine)
        {
          yield return line;
          line = line.NextLine;
        }
        yield return line;
      }
    }

    private class ScrollAlignment
    {
      public double Master { get; set; }
      public double Left { get; set; }
      public double Right { get; set; }

      public ScrollAlignment()
      {
        this.Master = 0.0;
        this.Left = 0.0;
        this.Right = 0.0;
      }
      public ScrollAlignment(double left, double right, ScrollAlignment previous)
      {
        this.Left = left;
        this.Right = right;
        this.Master = Math.Max(right - previous.Right, left - previous.Left) + previous.Master;
      }

      public override string ToString()
      {
        return string.Format("M{0:0.00}, L{1:0.00}, R{2:0.00}", this.Master, this.Left, this.Right);
      }
    }


    private void RenderBrush()
    {
      _splitterDrawing.Children.Clear();
      if (_compares != null)
      {
        var viewLeft = editLeft.TextArea.TextView;
        var viewRight = editRight.TextArea.TextView;

        _splitterDrawing.ClipGeometry = new RectangleGeometry(new Rect(0, 0, splitter.ActualWidth, splitter.ActualHeight));

        var space = new GeometryDrawing();
        space.Brush = Brushes.White;
        space.Geometry = new RectangleGeometry(new Rect(0, 0, splitter.ActualWidth, splitter.ActualHeight));
        _splitterDrawing.Children.Add(space);

        viewLeft.EnsureVisualLines();
        viewRight.EnsureVisualLines();
        var leftFirst = viewLeft.VisualLines.First().FirstDocumentLine.LineNumber;
        var leftLast = viewLeft.VisualLines.Last().LastDocumentLine.LineNumber;
        var rightFirst = viewRight.VisualLines.First().FirstDocumentLine.LineNumber;
        var rightLast = viewRight.VisualLines.Last().LastDocumentLine.LineNumber;
        Func<ListCompare, bool> inRange = (c => Math.Abs(c.Base) >= leftFirst && Math.Abs(c.Base) <= leftLast
          || Math.Abs(c.Compare) >= rightFirst && Math.Abs(c.Compare) <= rightLast);

        var start = 0;
        while (start < _compares.Count && !inRange(_compares[start])) start++;
        if (start >= _compares.Count) start--;
        var end = start;
        while (start >= 0 && GetDiffType(_compares[start]) != DiffType.Normal) start--;
        while (end < _compares.Count && inRange(_compares[end])) end++;
        end--;
        while (end < _compares.Count && GetDiffType(_compares[end]) != DiffType.Normal) end++;

        var lastType = GetDiffType(_compares[start]);
        DiffType currType;
        Geometry geom;

        var lines = new GeometryDrawing();
        lines.Pen = new Pen() { Thickness = 2, Brush = Brushes.Black };
        _splitterDrawing.Children.Add(lines);
      
        var group = new GeometryGroup();
        for (var i = start + 1; i <= end; i++)
        {
          currType = GetDiffType(_compares[i]);
          if (currType != lastType)
          {
            geom = GetPath(new Point(0, viewLeft.GetOrConstructVisualLine(viewLeft.Document.GetLineByNumber(Math.Abs(_compares[i].Base))).VisualTop - viewLeft.VerticalOffset),
                           new Point(splitter.ActualWidth, viewRight.GetOrConstructVisualLine(viewRight.Document.GetLineByNumber(Math.Abs(_compares[i].Compare))).VisualTop - viewRight.VerticalOffset));
            group.Children.Add(geom);
          }
          lastType = currType;
        }

        lines.Geometry = group;
      }
    }

    private PathGeometry GetPath(Point start, Point end)
    {
      var pathFigure = new PathFigure() { StartPoint = start };
      var segment = new BezierSegment()
      {
        Point1 = new Point((end.X - start.X) * 0.6 + start.X, start.Y),
        Point2 = new Point((end.X - start.X) * 0.4 + start.X, end.Y),
        Point3 = end
      };
      //var segment = new LineSegment() { Point = end };
      var pathSegmentCollection = new PathSegmentCollection();
      pathSegmentCollection.Add(segment);
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
