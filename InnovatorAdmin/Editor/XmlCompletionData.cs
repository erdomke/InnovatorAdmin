// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace InnovatorAdmin.Editor
{
  /// <summary>
  /// Holds the text for  namespace, child element or attribute
  /// autocomplete (intellisense).
  /// </summary>
  public class XmlCompletionData : BasicCompletionData
  {
    string defaultValue;
    DataType dataType = DataType.XmlElement;
    bool forceDefault = false;

    /// <summary>
    /// The type of text held in this object.
    /// </summary>
    public enum DataType
    {
      XmlElement = 1,
      XmlAttribute = 2,
      NamespaceUri = 3,
      XmlAttributeValue = 4
    }

    public XmlCompletionData() { }

    public XmlCompletionData(string text)
      : this(text, String.Empty, DataType.XmlElement)
    {
    }

    public XmlCompletionData(string text, string description)
      : this(text, description, DataType.XmlElement)
    {
    }

    public XmlCompletionData(string text, DataType dataType)
      : this(text, String.Empty, dataType)
    {
    }

    public XmlCompletionData(string text, string description, DataType dataType)
    {
      this.Text = text;
      this.Description = description;
      this.dataType = dataType;
    }

    public XmlCompletionData(string text, string description, DataType dataType, string defaultValue)
    {
      this.Text = text;
      this.Description = description;
      this.dataType = dataType;
      this.defaultValue = defaultValue;
    }
    public XmlCompletionData(string text, string description, DataType dataType, string defaultValue, bool forceDefault)
    {
      this.Text = text;
      this.Description = description;
      this.dataType = dataType;
      this.defaultValue = defaultValue;
      this.forceDefault = forceDefault;
    }

    public string DefaultValue
    {
      get
      {
        return defaultValue;
      }
    }

    public override System.Windows.Media.ImageSource Image
    {
      get
      {
        if (base.Image == null)
        {
          switch (this.dataType)
          {
            case DataType.XmlElement:
              return WpfImages.XmlTag16;
            case DataType.XmlAttribute:
              return WpfImages.Attribute16;
            default:
              return WpfImages.EnumValue16;
          }
        }
        return base.Image;
      }
      set { base.Image = value; }
    }

    public double Priority
    {
      get
      {
        return 0;
      }
    }

    //public bool InsertAction(TextArea textArea, char ch)
    //{
    //  if ((dataType == DataType.XmlElement) || (dataType == DataType.XmlAttributeValue)) {
    //    textArea.InsertString(text);
    //  }
    //  else if (dataType == DataType.NamespaceUri) {
    //    textArea.InsertString(String.Concat("\"", text, "\""));
    //  } else {
    //    // Insert an attribute.
    //    Caret caret = textArea.Caret;

    //            if (string.IsNullOrEmpty(defaultValue))
    //            {
    //                textArea.InsertString(String.Concat(text, "=\"\""));
    //                // Move caret into the middle of the attribute quotes.
    //                caret.Position = textArea.Document.OffsetToPosition(caret.Offset - 1);
    //            }
    //            else
    //            {
    //                textArea.InsertString(String.Format("{0}=\"{1}\"", text, defaultValue));
    //                if (!forceDefault) {
    //                    caret.Position = textArea.Document.OffsetToPosition(caret.Offset - 1);
    //                    textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(caret.Offset - defaultValue.Length),
    //                        caret.Position);
    //                }
    //            }

    //            try
    //            {
    //                XmlEditorBase editor = textArea.Parent.Parent.Parent as XmlEditorBase;
    //                if (editor != null)
    //                {
    //                    editor.ShowCompletionWindow();
    //                }
    //            }
    //            catch
    //            {
    //                //Do Nothing
    //            }
    //  }
    //  return false;
    //}

    public int CompareTo(object obj)
    {
      if ((obj == null) || !(obj is XmlCompletionData))
      {
        return -1;
      }
      return this.Text.CompareTo(((XmlCompletionData)obj).Text);
    }
  }
}
