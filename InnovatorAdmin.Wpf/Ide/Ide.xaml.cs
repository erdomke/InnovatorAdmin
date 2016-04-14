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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InnovatorAdmin.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public RoutedUICommand NewDocument { get; private set; }
    public RoutedUICommand NewWindow { get; private set; }

    public MainWindow()
    {
      this.DataContext = this;
      //this.NewDocument = new RoutedUICommand("New Document", "New Document", typeof(MainWindow));
      //this.NewWindow = new Ide.Commands.NewWindowCommand();

      InitializeComponent();
    }
  }
}
