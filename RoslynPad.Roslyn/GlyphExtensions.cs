using System.Windows;
using System.Windows.Media;
using RoslynPad.Roslyn.Completion;
using System;

namespace RoslynPad.Roslyn
{
  public static class GlyphExtensions
  {
    private static Application _currentApp;

    public static ImageSource ToImageSource(this Glyph glyph)
    {
      if (_currentApp == null && Application.Current != null)
        _currentApp = Application.Current;
      return (Application.Current ?? _currentApp).TryFindResource(glyph) as ImageSource;
    }
  }
}
