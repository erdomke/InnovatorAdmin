using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ADiff
{
  public class MergeView
  {
    private MergeDocument _doc;
    private TextEditor _editor;
    private TextView _view;
    private MergeLocation _location;
    private IList<Alignment> _starts;
    private IList<double> _blockBoundaries;

    public event EventHandler VerticalOffsetChanged;

    public MergeDocument Doc { get { return _doc; } }
    public MergeLocation Location { get { return _location; } }

    public VerticalOffset VertOffset
    {
      get
      {
        for (var i = _blockBoundaries.Count - 2; i >= 0; i--)
        {
          if (_blockBoundaries[i] < _view.VerticalOffset)
          {
            return new VerticalOffset(i, Math.Min((_view.VerticalOffset - _blockBoundaries[i]) / (_blockBoundaries[i + 1] - _blockBoundaries[i]), 1.0));
          }
        }
        return new VerticalOffset();
      }
      set
      {
        SetScrollOffset(value.BlockIndex, value.PercentOffset);
      }
    }

    public MergeView(TextEditor editor)
    {
      _editor = editor;
      _view = _editor.TextArea.TextView;
      _view.ScrollOffsetChanged += _view_ScrollOffsetChanged;
    }

    void _view_ScrollOffsetChanged(object sender, EventArgs e)
    {
      OnVerticalOffsetChanged(EventArgs.Empty);
    }

    public IScrollInfo GetScrollInfo()
    {
      return (IScrollInfo)_view;
    }

    public MergeView SetDocument(MergeDocument doc, MergeLocation location)
    {
      _doc = doc;
      _location = location;
      _editor.Text = GetDocument();
      ConfigureLineStarts();
      _view.BackgroundRenderers.Add(new DiffBackground(this));
      _view.InvalidateLayer(KnownLayer.Background);
      return this;
    }

    public IList<double> GetScrollBoundaries()
    {
      return _blockBoundaries;
    }
    public void SetScrollOffset(int block, double percentOffset)
    {
      _editor.ScrollToVerticalOffset(percentOffset * (_blockBoundaries[block + 1] - _blockBoundaries[block]) + _blockBoundaries[block]);
    }

    protected virtual void OnVerticalOffsetChanged(EventArgs e)
    {
      if (this.VerticalOffsetChanged != null) this.VerticalOffsetChanged.Invoke(this, e);
    }

    private string GetDocument()
    {
      bool showAll;
      var builder = new StringBuilder();
      foreach (var block in _doc.Blocks)
      {
        showAll = _location == MergeLocation.Output && !block.Alternates.Any(a => (a.Location & _location) != 0);
        foreach (var alt in block.Alternates.Where(a => (a.Location & _location) != 0 || showAll))
        {
          builder.Append(alt.Text);
        }
      }
      return builder.ToString();
    }

    private void ConfigureLineStarts()
    {
      _starts = new List<Alignment>();
      var lastStart = 1;
      _blockBoundaries = new List<double>();

      bool showAll;
      foreach (var block in _doc.Blocks)
      {
        _blockBoundaries.Add(_view.GetOrConstructVisualLine(_view.Document.GetLineByNumber(lastStart)).GetVisualPosition(1, VisualYPosition.LineTop).Y);
        showAll = _location == MergeLocation.Output && !block.Alternates.Any(a => (a.Location & _location) != 0);
        foreach (var alt in block.Alternates.Where(a => (a.Location & _location) != 0 || showAll))
        {
          _starts.Add(new Alignment() {
            LineNumber = lastStart,
            Alternate = alt
          });
          lastStart += alt.LineCount;
        }
      }

      var lastLine = _view.GetOrConstructVisualLine(_view.Document.GetLineByNumber(lastStart-1));
      _blockBoundaries.Add(lastLine.GetVisualPosition(1, VisualYPosition.LineBottom).Y);
    }

    private class Alignment
    {
      public int LineNumber { get; set; }
      public MergeAlternate Alternate { get; set; }
    }

    private class DiffBackground : IBackgroundRenderer
    {
      private MergeView _view;

      public DiffBackground(MergeView view)
      {
        _view = view;
      }

      private Color GetColor(MergeLocation location)
      {
        switch (location)
        {
          case MergeLocation.Left:
            return Colors.Violet;
          case MergeLocation.Parent:
            return Colors.LightGoldenrodYellow;
          case MergeLocation.Right:
            return Colors.LightGreen;
          default:
            throw new ArgumentException();
        }
      }

      public void Draw(TextView textView, DrawingContext drawingContext)
      {
        if (textView.Document == null) return;

        textView.EnsureVisualLines();

        var firstLine = textView.VisualLines.First().FirstDocumentLine.LineNumber;
        int nextBlockLine = int.MaxValue;
        int blockIndex;
        Color backColor;
        MergeAlternate alt;
        MergeLocation colorLocation;

        for (blockIndex = 1; blockIndex < _view._starts.Count; blockIndex++)
        {
          if (_view._starts[blockIndex].LineNumber > firstLine)
          {
            nextBlockLine = _view._starts[blockIndex].LineNumber;
            blockIndex--;
            break;
          }
        }
        if (blockIndex >= _view._starts.Count) blockIndex = _view._starts.Count - 1;

        foreach (var visLine in textView.VisualLines)
        {
          if (visLine.FirstDocumentLine.LineNumber >= nextBlockLine)
          {
            blockIndex++;
            nextBlockLine = (blockIndex < (_view._starts.Count - 1) ? _view._starts[blockIndex + 1].LineNumber : int.MaxValue);
          }

          alt = _view._starts[blockIndex].Alternate;
          if (_view._starts[blockIndex].Alternate.Parent.Alternates.Count > 1
            && (_view.Location == MergeLocation.Output
              || _view.Location == MergeLocation.Parent
              || (alt.Location & MergeLocation.Parent) == 0))
          {

            colorLocation = alt.Location & ~MergeLocation.Output;
            if (_view.Location == MergeLocation.Output)
            {
              if ((colorLocation & MergeLocation.Right) != 0)
              {
                backColor = GetColor(MergeLocation.Right);
              }
              else if ((colorLocation & MergeLocation.Parent) != 0)
              {
                backColor = GetColor(MergeLocation.Parent);
              }
              else
              {
                backColor = GetColor(MergeLocation.Left);
              }
            }
            else
            {
              backColor = GetColor(_view.Location);
              if (colorLocation == (MergeLocation.Left | MergeLocation.Right)) backColor = GetColor(MergeLocation.Right);
            }

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
    }

    public static IList<double> GetScrollBoundaries(params MergeView[] views)
    {
      var result = new List<double>();
      result.Add(0);
      var boundaryLists = views.Select(v => v.GetScrollBoundaries()).ToArray();
      double currChange;
      double maxChange;

      for (var i = 1; i < views[0].GetScrollBoundaries().Count; i++)
      {
        maxChange = double.MinValue;
        foreach (var list in boundaryLists)
        {
          currChange = list[i] - list[i - 1];
          if (currChange > maxChange) maxChange = currChange;
        }
        result.Add(result.Last() + maxChange);
      }

      return result;
    }
    //public static double MinViewportHeight(params MergeView[] views)
    //{
    //  return views.Select(v => ((IScrollInfo)v._view).ScrollOwner.ViewportHeight).Min();
    //}
  }
}
