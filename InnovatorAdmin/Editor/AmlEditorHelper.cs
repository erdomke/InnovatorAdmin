using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class AmlEditorHelper : CompletionHelper, IEditorHelper
  {
    private bool _isInitialized = false;

    public string SoapAction { get; set; }

    public AmlEditorHelper()
    {
      this.SoapAction = "ApplyAML";
    }

    public override void InitializeConnection(IAsyncConnection conn)
    {
      _isInitialized = true;
      base.InitializeConnection(conn);
    }

    public void HandleTextEntered(EditorControl control, string insertText)
    {
      if (_isInitialized)
      {
        switch (insertText)
        {
          case "'":
          case "\"":
          case " ":
          case "<":
          case ",":
          case "(":
          case ".":
            ShowCompletions(control)
              .Done(data =>
              {
                if (data != null && data.State == CompletionType.Attribute && !data.Items.Any()
                  && control.Editor.CaretOffset < control.Editor.Document.TextLength)
                {
                  var doc = control.Editor.TextArea.Document;
                  var quote = doc.GetCharAt(doc.LastIndexOf('=', 0, control.Editor.CaretOffset) + 1);
                  if (insertText[0] == quote && quote == doc.GetCharAt(control.Editor.CaretOffset))
                  {
                    doc.Remove(control.Editor.CaretOffset, 1);
                  }
                }
              });
            break;
          case ">":
            var endTag = this.LastOpenTag(control.Editor.Text.Substring(0, control.Editor.CaretOffset));
            if (!string.IsNullOrEmpty(endTag))
            {
              var insert = "</" + endTag + ">";
              if (!control.Editor.Text.Substring(control.Editor.CaretOffset).StartsWith(insert))
              {
                control.Editor.Document.Insert(control.Editor.CaretOffset, insert, AnchorMovementType.BeforeInsertion);
              }
            }
            break;
        }
      }
    }

    private IPromise<CompletionData> ShowCompletions(EditorControl control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;

      return this.GetCompletions(control.Editor.Text, control.Editor.CaretOffset, this.SoapAction)
        .UiPromise(control)
        .Convert(data => {
          if (length != control.Editor.Document.TextLength
            || caret != control.Editor.CaretOffset)
          {
            ShowCompletions(control);
            return null;
          }

          if (data.Items.Any())
            control.ShowCompletionWindow(data.Items.Select(c =>
            {
              switch (data.State)
              {
                case CompletionType.Attribute:
                  return new AttributeCompletionData(c, this, control);
                case CompletionType.AttributeValue:
                  return new AttributeValueCompletionData(c, data.MultiValueAttribute);
                case CompletionType.SqlGeneral:
                  return new SqlGeneral(c);
                case CompletionType.SqlObjectName:
                  return new SqlObjectCompletionData(c, this, control);
              }
              return new BasicCompletionData(c);
            }), data.Overlap);

          return data;
        });
    }

    public string GetCurrentQuery(string text, int offset)
    {
      return this.GetQuery(text, offset);
    }

    private class AttributeCompletionData : BasicCompletionData
    {
      private AmlEditorHelper _parent;
      private EditorControl _control;


      public AttributeCompletionData(string text, AmlEditorHelper parent, EditorControl control)
        : base(text)
      {
        _parent = parent;
        _control = control;
      }

      public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
      {
        textArea.Document.Replace(completionSegment, this.Text + "=''");
        textArea.Caret.Offset -= 1;
        _parent.ShowCompletions(_control);
      }
    }

    private class SqlObjectCompletionData : BasicCompletionData
    {
      private AmlEditorHelper _parent;
      private EditorControl _control;


      public SqlObjectCompletionData(string text, AmlEditorHelper parent, EditorControl control)
        : base(text)
      {
        _parent = parent;
        _control = control;
      }

      public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
      {
        if (this.Text.Equals("innovator", StringComparison.OrdinalIgnoreCase))
        {
          textArea.Document.Replace(completionSegment, "innovator.");
          _parent.ShowCompletions(_control);
        }
        else
        {
          if (textArea.Document.Text.Substring(0, completionSegment.Offset).EndsWith("innovator.", StringComparison.OrdinalIgnoreCase))
          {
            textArea.Document.Replace(completionSegment, "[" + this.Text + "]");
          }
          else
          {
            textArea.Document.Replace(completionSegment, "innovator.[" + this.Text + "]");
          }
        }
      }
    }

    internal class SqlGeneral : BasicCompletionData
    {
      public SqlGeneral(string text) : base(text) { }
    }

    private class AttributeValueCompletionData : BasicCompletionData
    {

      private bool _multiValue;

      public AttributeValueCompletionData(string text, bool multiValue) : base(text)
      {
        _multiValue = multiValue;
      }

      public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
      {
        textArea.Document.Replace(completionSegment, this.Text);

        if (!_multiValue)
        {
          var offset = completionSegment.Offset + this.Text.Length;
          var doc = textArea.Document;
          var quote = doc.GetCharAt(completionSegment.Offset - 1);
          if (doc.TextLength > offset && doc.GetCharAt(offset) == quote
            && (quote == '\'' || quote == '"'))
          {
            textArea.Caret.Offset += 1;
          }
        }
      }
    }
  }
}
