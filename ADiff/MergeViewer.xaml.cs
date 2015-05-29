using ICSharpCode.AvalonEdit;
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
    public MergeViewer()
    {
      InitializeComponent();

      ConfigureEditor(editLeft);
      ConfigureEditor(editParent);
      ConfigureEditor(editResult);
      ConfigureEditor(editRight);
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

    private void vertScroll_Scroll(object sender, ScrollEventArgs e)
    {
      //HandleVerticalScroll();
    }

    private void horizScroll_Scroll(object sender, ScrollEventArgs e)
    {
      //editLeft.ScrollToHorizontalOffset(horizScroll.Value);
      //editRight.ScrollToHorizontalOffset(horizScroll.Value);
    }
  }
}
