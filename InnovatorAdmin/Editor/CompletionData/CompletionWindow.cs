using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// The code completion window.
  /// </summary>
  public class CompletionWindow : CompletionWindowBase
  {
    private readonly CompletionList completionList = new CompletionList();
    private ToolTip toolTip = new ToolTip();
    /// <summary>
    /// Gets the completion list used in this completion window.
    /// </summary>
    public CompletionList CompletionList
    {
      get
      {
        return this.completionList;
      }
    }
    /// <summary>
    /// Gets/Sets whether the completion window should close automatically.
    /// The default value is true.
    /// </summary>
    public bool CloseAutomatically
    {
      get;
      set;
    }
    /// <inheritdoc />
    protected override bool CloseOnFocusLost
    {
      get
      {
        return this.CloseAutomatically;
      }
    }
    /// <summary>
    /// When this flag is set, code completion closes if the caret moves to the
    /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
    /// but not in dot-completion.
    /// Has no effect if CloseAutomatically is false.
    /// </summary>
    public bool CloseWhenCaretAtBeginning
    {
      get;
      set;
    }
    /// <summary>
    /// Creates a new code completion window.
    /// </summary>
    public CompletionWindow(TextArea textArea) : base(textArea)
    {
      this.CloseAutomatically = true;
      base.SizeToContent = SizeToContent.Height;
      base.MaxHeight = 300.0;
      base.Width = 175.0;
      base.Content = this.completionList;
      base.MinHeight = 15.0;
      base.MinWidth = 30.0;
      this.toolTip.PlacementTarget = this;
      this.toolTip.Placement = PlacementMode.Right;
      this.toolTip.Closed += new RoutedEventHandler(this.toolTip_Closed);
      this.AttachEvents();
    }
    private void toolTip_Closed(object sender, RoutedEventArgs e)
    {
      if (this.toolTip != null)
      {
        this.toolTip.Content = null;
      }
    }
    private void completionList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      ICompletionData item = this.completionList.SelectedItem;
      if (item != null)
      {
        object description = item.Description;
        if (description != null)
        {
          string descriptionText = description as string;
          if (descriptionText != null)
          {
            this.toolTip.Content = new TextBlock
            {
              Text = descriptionText,
              TextWrapping = TextWrapping.Wrap
            };
          }
          else
          {
            this.toolTip.Content = description;
          }
          this.toolTip.IsOpen = true;
        }
        else
        {
          this.toolTip.IsOpen = false;
        }
      }
    }
    private void completionList_InsertionRequested(object sender, EventArgs e)
    {
      base.Close();
      ICompletionData item = this.completionList.SelectedItem;
      if (item != null)
      {
        item.Complete(base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, checked(base.EndOffset - base.StartOffset)), e);
      }
    }
    private void AttachEvents()
    {
      this.completionList.InsertionRequested += new EventHandler(this.completionList_InsertionRequested);
      this.completionList.SelectionChanged += new SelectionChangedEventHandler(this.completionList_SelectionChanged);
      base.TextArea.Caret.PositionChanged += new EventHandler(this.CaretPositionChanged);
      base.TextArea.MouseWheel += new MouseWheelEventHandler(this.textArea_MouseWheel);
      base.TextArea.PreviewTextInput += new TextCompositionEventHandler(this.textArea_PreviewTextInput);
    }
    /// <inheritdoc />
    protected override void DetachEvents()
    {
      this.completionList.InsertionRequested -= new EventHandler(this.completionList_InsertionRequested);
      this.completionList.SelectionChanged -= new SelectionChangedEventHandler(this.completionList_SelectionChanged);
      base.TextArea.Caret.PositionChanged -= new EventHandler(this.CaretPositionChanged);
      base.TextArea.MouseWheel -= new MouseWheelEventHandler(this.textArea_MouseWheel);
      base.TextArea.PreviewTextInput -= new TextCompositionEventHandler(this.textArea_PreviewTextInput);
      base.DetachEvents();
    }
    /// <inheritdoc />
    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      if (this.toolTip != null)
      {
        this.toolTip.IsOpen = false;
        this.toolTip = null;
      }
    }
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (!e.Handled)
      {
        this.completionList.HandleKey(e);
      }
    }
    private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = CompletionWindowBase.RaiseEventPair(this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
    }
    private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      e.Handled = CompletionWindowBase.RaiseEventPair(this.GetScrollEventTarget(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
    }
    private UIElement GetScrollEventTarget()
    {
      UIElement result;
      if (this.completionList == null)
      {
        result = this;
      }
      else
      {
        result = (this.completionList.ScrollViewer ?? ((UIElement)this.completionList.ListBox ?? this.completionList));
      }
      return result;
    }
    private void CaretPositionChanged(object sender, EventArgs e)
    {
      int offset = base.TextArea.Caret.Offset;
      if (offset == base.StartOffset)
      {
        if (this.CloseAutomatically && this.CloseWhenCaretAtBeginning)
        {
          base.Close();
        }
        else
        {
          this.completionList.SelectItem(string.Empty);
        }
      }
      else
      {
        if (offset < base.StartOffset || offset > base.EndOffset)
        {
          if (this.CloseAutomatically)
          {
            base.Close();
          }
        }
        else
        {
          TextDocument document = base.TextArea.Document;
          if (document != null)
          {
            this.completionList.SelectItem(document.GetText(base.StartOffset, checked(offset - base.StartOffset)));
          }
        }
      }
    }
  }
}
