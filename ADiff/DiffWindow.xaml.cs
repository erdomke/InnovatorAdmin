using ADiff.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
  /// Interaction logic for DiffWindow.xaml
  /// </summary>
  public partial class DiffWindow : Window
  {
    public string LeftText
    {
      get { return diff.Left; }
      set 
      { 
        diff.Left = value;
        RefreshDiff();
      }
    }
    public string RightText
    {
      get { return diff.Right; }
      set 
      { 
        diff.Right = value;
        RefreshDiff();
      }
    }

    public DiffWindow()
    {
      InitializeComponent();
#if DEBUG
      diff.Left = System.IO.File.ReadAllText(@"C:\Users\edomke\Exports\Issue_DEV\ItemType\Issue.xml");
      diff.Right = System.IO.File.ReadAllText(@"C:\Users\edomke\Exports\Issue_PROD\ItemType\Issue.xml");
      RefreshDiff();
#endif
    }

    private void btnCalcDiff_Click(object sender, RoutedEventArgs e)
    {
      RefreshDiff();
    }

    private void RefreshDiff()
    {
      if (!string.IsNullOrEmpty(diff.Left) && !string.IsNullOrEmpty(diff.Right))
      {
        string newLeft;
        string newRight;
        var changes = AmlDiff.TwoWayDiff(diff.Left, diff.Right, out newLeft, out newRight);
        diff.Left = newLeft;
        diff.Right = newRight;
        diff.SetDiffs(changes);
      }
    }
  }
}
