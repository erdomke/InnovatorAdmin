using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADiff
{
  /// <summary>
  /// Interaction logic for MergeViewer.xaml
  /// </summary>
  public partial class MergeViewer : UserControl
  {
    private MergeDocument _doc;
    private MergeView _leftView;
    private MergeView _rightView;
    private MergeView _parentView;
    private MergeView _outputView;
    private IList<double> _scrollBoundaries;
    private IList<MergeView> _views;

    public MergeDocument Document
    {
      get { return _doc; }
      set 
      { 
        _doc = value;
        TryInitializeViews();
      }
    }

    void editor_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      RecalculateScrollBars();
    }

    public MergeViewer()
    {
      InitializeComponent();

      ConfigureEditor(editLeft);
      ConfigureEditor(editParent);
      ConfigureEditor(editOutput);
      ConfigureEditor(editRight);

      editLeft.SizeChanged += editor_SizeChanged;
      editParent.SizeChanged += editor_SizeChanged;
      editRight.SizeChanged += editor_SizeChanged;
      editOutput.SizeChanged += editor_SizeChanged;

      this.Loaded += MergeViewer_Loaded; 
    }

    void MergeViewer_Loaded(object sender, RoutedEventArgs e)
    {
      TryInitializeViews();
      RecalculateScrollBars();
    }

    private void TryInitializeViews()
    {
      if (_doc != null && this.IsLoaded)
      {
        _leftView = new MergeView(editLeft).SetDocument(_doc, MergeLocation.Left);
        _parentView = new MergeView(editParent).SetDocument(_doc, MergeLocation.Parent);
        _rightView = new MergeView(editRight).SetDocument(_doc, MergeLocation.Right);
        _outputView = new MergeView(editOutput).SetDocument(_doc, MergeLocation.Output);
        _scrollBoundaries = MergeView.GetScrollBoundaries(_leftView, _parentView, _rightView, _outputView);
        _views = new MergeView[] { _leftView, _parentView, _rightView, _outputView };

        vertScroll.Minimum = 0.0;
        vertScroll.Maximum = _scrollBoundaries.Last();
      }
    }

    private void RecalculateScrollBars()
    {
      if (_views != null)
      {
        vertScroll.ViewportSize = _views.Select(v => v.GetScrollInfo().ViewportHeight).Min();
        vertScroll.Maximum = _scrollBoundaries.Last();
      }
    }

    private void ConfigureEditor(TextEditor editor)
    {
      editor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
      editor.FontSize = 12.0;
      editor.Options.ConvertTabsToSpaces = true;
      editor.Options.EnableRectangularSelection = true;
      editor.Options.IndentationSize = 2;
      editor.ShowLineNumbers = true;
    }
    private void HandleVerticalScroll()
    {
      for (var i = _scrollBoundaries.Count - 2; i >= 0; i--)
      {
        if (_scrollBoundaries[i] < vertScroll.Value)
        {
          var offset = new VerticalOffset(i, Math.Min((vertScroll.Value - _scrollBoundaries[i]) / (_scrollBoundaries[i + 1] - _scrollBoundaries[i]), 1.0));
          _leftView.VertOffset = offset;
          _parentView.VertOffset = offset;
          _rightView.VertOffset = offset;
          _outputView.VertOffset = offset;
          break;
        }
      }
    }

    private void vertScroll_Scroll(object sender, ScrollEventArgs e)
    {
      HandleVerticalScroll();
    }

    private void horizScroll_Scroll(object sender, ScrollEventArgs e)
    {
      //editLeft.ScrollToHorizontalOffset(horizScroll.Value);
      //editRight.ScrollToHorizontalOffset(horizScroll.Value);
    }
  }
}
