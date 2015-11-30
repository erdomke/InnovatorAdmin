using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Search;
using System.Text.RegularExpressions;

namespace Aras.Tools.InnovatorAdmin.Editor
{
	internal class SearchResultBackgroundRenderer : IBackgroundRenderer
	{
    private TextSegmentCollection<TextSegment> currentResults = new TextSegmentCollection<TextSegment>();
		private Brush markerBrush;
		private Pen markerPen;
    public TextSegmentCollection<TextSegment> CurrentResults
		{
			get
			{
				return this.currentResults;
			}
		}
		public KnownLayer Layer
		{
			get
			{
				return KnownLayer.Selection;
			}
		}
		public Brush MarkerBrush
		{
			get
			{
				return this.markerBrush;
			}
			set
			{
				this.markerBrush = value;
				this.markerPen = new Pen(this.markerBrush, 1.0);
			}
		}
		public SearchResultBackgroundRenderer()
		{
			this.markerBrush = Brushes.LightGreen;
			this.markerPen = new Pen(this.markerBrush, 1.0);
		}
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
			{
				throw new ArgumentNullException("textView");
			}
			if (drawingContext == null)
			{
				throw new ArgumentNullException("drawingContext");
			}
			if (this.currentResults == null || !textView.VisualLinesValid)
			{
				return;
			}
			ReadOnlyCollection<VisualLine> visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
			{
				return;
			}
			int offset = visualLines.First<VisualLine>().FirstDocumentLine.Offset;
			int endOffset = visualLines.Last<VisualLine>().LastDocumentLine.EndOffset;
			foreach (var current in this.currentResults.FindOverlappingSegments(offset, endOffset - offset))
			{
				BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
				backgroundGeometryBuilder.AlignToMiddleOfPixels = true;
				backgroundGeometryBuilder.CornerRadius = 3.0;
				backgroundGeometryBuilder.AddSegment(textView, current);
				Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
				if (geometry != null)
				{
					drawingContext.DrawGeometry(this.markerBrush, this.markerPen, geometry);
				}
			}
		}
	}
}
