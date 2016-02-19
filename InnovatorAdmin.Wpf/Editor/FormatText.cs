using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  public static class FormatText
  {
    public static Span Span(params Run[] runs)
    {
      return Invoke(() =>
      {
        var result = new Span();
        result.Inlines.AddRange(runs);
        return result;
      });
    }
    public static Run ColorText(string value, Brush color)
    {
      return Invoke(() => new Run(value)
      {
        Foreground = color
      });
    }
    public static Run MutedText(string value)
    {
      return Invoke(() => new Run(value)
      {
        Foreground = Brushes.Gray
      });
    }
    public static Run Text(string value)
    {
      return Invoke(() => new Run(value));
    }

    private static T Invoke<T>(Func<T> factory)
    {
      return Application.Current.Dispatcher.Invoke(factory);
    }
  }
}
