using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoslynPad
{
  /// <summary>
  /// Interaction logic for MethodWindow.xaml
  /// </summary>
  public partial class MethodWindow : Window
  {
    private WindowViewModel _viewModel;

    public MethodWindow()
    {
      _viewModel = new WindowViewModel();
      _viewModel.CurrentOpenDocument = new MethodViewModel(_viewModel, null, _viewModel.MethodConfigId);
      DataContext = _viewModel;
      InitializeComponent();
      docView.DataContext = _viewModel.CurrentOpenDocument;
    }
  }
}
