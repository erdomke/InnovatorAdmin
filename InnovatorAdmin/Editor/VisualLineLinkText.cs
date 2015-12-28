using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// VisualLineElement that represents a piece of text and is a clickable link.
  /// </summary>
  public class VisualLineLinkText : VisualLineText
  {
    /// <summary>
    /// Gets/Sets the action that is performed when the link is clicked.
    /// </summary>
    public Action NavigateAction { get; set; }
    /// <summary>
    /// Gets/Sets the window name where the URL will be opened.
    /// </summary>
    public string TargetName { get; set; }
    /// <summary>
    /// Gets/Sets whether the user needs to press Control to click the link.
    /// The default value is true.
    /// </summary>
    public bool RequireControlModifierForClick { get; set; }
    /// <summary>
    ///  Creates a visual line text element with the specified length.
    ///  It uses the <see cref="P:ICSharpCode.AvalonEdit.Rendering.ITextRunConstructionContext.VisualLine" /> and its
    ///  <see cref="P:ICSharpCode.AvalonEdit.Rendering.VisualLineElement.RelativeTextOffset" /> to find the actual text string.
    ///  </summary>
    public VisualLineLinkText(VisualLine parentVisualLine, int length)
      : base(parentVisualLine, length)
    {
      this.RequireControlModifierForClick = true;
    }
    /// <summary>
    /// Creates the TextRun for this line element.
    /// </summary>
    /// <param name="startVisualColumn">
    /// The visual column from which the run should be constructed.
    /// Normally the same value as the <see cref="P:ICSharpCode.AvalonEdit.Rendering.VisualLineElement.VisualColumn" /> property is used to construct the full run;
    /// but when word-wrapping is active, partial runs might be created.
    /// </param>
    /// <param name="context">
    /// Context object that contains information relevant for text run creation.
    /// </param>
    public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
    {
      base.TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
      base.TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
      if (context.TextView.LinkTextUnderline)
      {
        base.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
      }
      return base.CreateTextRun(startVisualColumn, context);
    }
    /// <summary>
    /// Gets whether the link is currently clickable.
    /// </summary>
    protected bool LinkIsClickable()
    {
      return !(this.NavigateAction == null)
        && (!this.RequireControlModifierForClick
          || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
    }

    /// <summary>
    /// Queries the cursor over the visual line element.
    /// </summary>
    protected override void OnQueryCursor(QueryCursorEventArgs e)
    {
      if (this.LinkIsClickable())
      {
        e.Handled = true;
        e.Cursor = Cursors.Hand;
      }
    }
    /// <summary>
    /// Allows the visual line element to handle a mouse event.
    /// </summary>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left && !e.Handled && this.LinkIsClickable())
      {
        this.NavigateAction.Invoke();
      }
    }
    /// <summary>
    /// Override this method to control the type of new VisualLineText instances when
    /// the visual line is split due to syntax highlighting.
    /// </summary>
    protected override VisualLineText CreateInstance(int length)
    {
      return new VisualLineLinkText(base.ParentVisualLine, length)
      {
        NavigateAction = this.NavigateAction,
        TargetName = this.TargetName,
        RequireControlModifierForClick = this.RequireControlModifierForClick
      };
    }
  }
}
