using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class XmlEditorHelper : IEditorHelper
  {
    protected IFoldingStrategy _foldingStrategy = new XmlFoldings();

    public virtual string BlockCommentEnd { get { return "-->"; } }
    public virtual string BlockCommentStart { get { return "<!--"; } }
    public virtual string LineComment { get { return string.Empty; } }

    public virtual ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighting()
    {
      return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".xml");
    }

    public virtual IEnumerable<string> GetParameterNames(string query)
    {
      return Enumerable.Empty<string>();
    }

    public IFoldingStrategy FoldingStrategy
    {
      get { return _foldingStrategy; }
    }

    public virtual void HandleTextEntered(EditorWinForm control, string insertText)
    {
      // Do nothing
    }

    public virtual string GetCurrentQuery(ITextSource text, int offset)
    {
      return text.Text;
    }

    public virtual Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      return Innovator.Client.Promises.Resolved(new CompletionContext());
    }

    public virtual void Format(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      RenderXml(reader, writer, true);
    }

    public virtual void Minify(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      RenderXml(reader, writer, false);
    }

    private void RenderXml(System.IO.TextReader reader, System.IO.TextWriter writer, bool indent)
    {
      var readerSettings = new XmlReaderSettings();
      readerSettings.IgnoreWhitespace = true;
      readerSettings.ConformanceLevel = ConformanceLevel.Fragment;

      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      if (indent) settings.IndentChars = "  ";
      settings.CheckCharacters = true;
      settings.CloseOutput = true;
      settings.ConformanceLevel = ConformanceLevel.Fragment;

      try
      {
        using (var xmlReader = XmlReader.Create(reader, readerSettings))
        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
          while (!xmlReader.EOF)
          {
            xmlWriter.WriteNode(xmlReader, true);
          }
          xmlWriter.Flush();
        }
      }
      catch (XmlException) { } // Eat it for now
    }

    private class XmlFoldings : IFoldingStrategy
    {
      private XmlFoldingStrategy _strategy = new XmlFoldingStrategy();

      /// <summary>
      /// Create <see cref="NewFolding"/>s for the specified document and updates the folding manager with them.
      /// </summary>
      public void UpdateFoldings(FoldingManager manager, TextDocument document)
      {
        int firstErrorOffset;
        IEnumerable<NewFolding> foldings = _strategy.CreateNewFoldings(document, out firstErrorOffset);
        var errors = foldings.Where(f => f.StartOffset < 0 || f.EndOffset > document.TextLength).ToArray();
        manager.UpdateFoldings(foldings, firstErrorOffset);
      }
    }


    public virtual IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset)
    {
      return Enumerable.Empty<IEditorScript>();
    }
  }
}
