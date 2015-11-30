
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
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;

namespace FindReplace
{

    /// <summary>
    /// Interaction logic for FindReplaceDialog.xaml
    /// </summary>
    public partial class FindReplaceDialog : Window
    {
        FindReplaceMgr TheVM;

        public FindReplaceDialog(FindReplaceMgr theVM)
        {            
            DataContext = TheVM = theVM;  
            InitializeComponent();
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            TheVM.FindNext();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            TheVM.Replace();
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            TheVM.ReplaceAll();
        }
  
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

    }
}
