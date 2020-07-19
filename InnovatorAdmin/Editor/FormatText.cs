using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static Run Bold(string value)
    {
      return Invoke(() => new Run(value)
      {
        FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(700)
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

    public static T Invoke<T>(Func<T> factory)
    {
      if (System.Windows.Forms.Application.OpenForms[0].InvokeRequired)
      {
        return (T)System.Windows.Forms.Application.OpenForms[0].Invoke(factory);
      }
      else
      {
        return factory.Invoke();
      }
    }
  }
}
