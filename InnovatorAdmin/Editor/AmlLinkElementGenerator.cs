using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class AmlLinkElementGenerator : VisualLineElementGenerator
  {
    public event EventHandler<AmlLinkClickedEventArgs> AmlLinkClicked;

    /// <summary>
    /// Gets/Sets whether the user needs to press Control to click the link.
    /// The default value is true.
    /// </summary>
    public bool RequireControlModifierForClick { get; set; }

    /// <summary>
    /// Creates a new LinkElementGenerator.
    /// </summary>
    public AmlLinkElementGenerator()
    {
      this.RequireControlModifierForClick = true;
    }

    private AmlLinkClickedEventArgs InfoFromOffset(int startOffset)
    {
      var settings = new XmlReaderSettings();
      settings.ConformanceLevel = ConformanceLevel.Fragment;
      settings.DtdProcessing = DtdProcessing.Ignore;
      settings.IgnoreProcessingInstructions = true;
      var start = base.CurrentContext.VisualLine.FirstDocumentLine.Offset;
      var stop = base.CurrentContext.VisualLine.LastDocumentLine.EndOffset;
      var offset = startOffset - start;
      var text = base.CurrentContext.GetText(start, stop - start).Text;

      using (var strReader = new StringReader(text))
      using (var reader = XmlReader.Create(strReader, settings))
      {
        var lineInfo = reader as IXmlLineInfo;
        string type = null;
        try
        {
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                type = reader.IsEmptyElement ? null : reader.GetAttribute("type");
                break;
              case XmlNodeType.EndElement:
                type = null;
                break;
              case XmlNodeType.Text:
                if (!string.IsNullOrEmpty(type) && reader.Value.IsGuid() && lineInfo.LinePosition >= offset)
                  return new AmlLinkClickedEventArgs()
                  {
                    Id = reader.Value,
                    Type = type,
                    Offset = lineInfo.LinePosition + start - 1
                  };
                break;
            }
          }
        }
        catch (XmlException) { }
      }
      return new AmlLinkClickedEventArgs() { Offset = -1 };
    }

    /// <summary>
    /// Gets the first offset &gt;= startOffset where the generator wants to construct an element.
    /// Return -1 to signal no interest.
    /// </summary>
    public override int GetFirstInterestedOffset(int startOffset)
    {
      return InfoFromOffset(startOffset).Offset;
    }
    /// <summary>
    /// Constructs an element at the specified offset.
    /// May return null if no element should be constructed.
    /// </summary>
    public override VisualLineElement ConstructElement(int offset)
    {
      var info = InfoFromOffset(offset);
      if (info.Offset == offset)
      {
        return new VisualLineLinkText(base.CurrentContext.VisualLine, info.Id.Length)
        {
          NavigateAction = () => {
            if (AmlLinkClicked != null)
              AmlLinkClicked.Invoke(this, info);
          },
          RequireControlModifierForClick = this.RequireControlModifierForClick
        };
      }

      return null;
    }
  }

  public class AmlLinkClickedEventArgs : EventArgs
  {
    public string Type { get; set; }
    public string Id { get; set; }
    public int Offset { get; set; }
  }
}
