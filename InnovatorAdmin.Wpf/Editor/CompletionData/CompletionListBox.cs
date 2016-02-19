using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows.Controls;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// The list box used inside the CompletionList.
  /// </summary>
  public class CompletionListBox : ListBox
  {
    internal ScrollViewer scrollViewer;
    /// <summary>
    /// Gets the number of the first visible item.
    /// </summary>
    public int FirstVisibleItem
    {
      get
      {
        int result;
        if (this.scrollViewer == null || this.scrollViewer.ExtentHeight == 0.0)
        {
          result = 0;
        }
        else
        {
          result = checked((int)(unchecked((double)base.Items.Count * this.scrollViewer.VerticalOffset) / this.scrollViewer.ExtentHeight));
        }
        return result;
      }
      set
      {
        value = Math.Min(Math.Max(value, 0), checked(base.Items.Count - this.VisibleItemCount));
        if (this.scrollViewer != null)
        {
          this.scrollViewer.ScrollToVerticalOffset((double)value / (double)base.Items.Count * this.scrollViewer.ExtentHeight);
        }
      }
    }
    /// <summary>
    /// Gets the number of visible items.
    /// </summary>
    public int VisibleItemCount
    {
      get
      {
        int result;
        if (this.scrollViewer == null || this.scrollViewer.ExtentHeight == 0.0)
        {
          result = 10;
        }
        else
        {
          result = Math.Max(3, checked((int)Math.Ceiling(unchecked((double)base.Items.Count * this.scrollViewer.ViewportHeight) / this.scrollViewer.ExtentHeight)));
        }
        return result;
      }
    }
    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.scrollViewer = null;
      if (this.VisualChildrenCount > 0)
      {
        Border border = this.GetVisualChild(0) as Border;
        if (border != null)
        {
          this.scrollViewer = (border.Child as ScrollViewer);
        }
      }
    }
    /// <summary>
    /// Removes the selection.
    /// </summary>
    public void ClearSelection()
    {
      base.SelectedIndex = -1;
    }
    /// <summary>
    /// Selects the item with the specified index and scrolls it into view.
    /// </summary>
    public void SelectIndex(int index)
    {
      if (index >= base.Items.Count)
      {
        index = checked(base.Items.Count - 1);
      }
      if (index < 0)
      {
        index = 0;
      }
      base.SelectedIndex = index;
      base.ScrollIntoView(base.SelectedItem);
    }
    /// <summary>
    /// Centers the view on the item with the specified index.
    /// </summary>
    public void CenterViewOn(int index)
    {
      this.FirstVisibleItem = checked(index - this.VisibleItemCount / 2);
    }
  }
}
