using GotDotNet.XPath;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace InnovatorAdmin.Editor
{
  public class XPathSearchStrategy : ISearchStrategy
  {
    private string _xPath;

    public XPathSearchStrategy(string xPath)
    {
      _xPath = xPath;
    }


    public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
    {
      try
      {
        return FindAllValidating(document, offset, length).ToList();
      }
      catch (XmlException)
      {
        return FindAllForwardOnly(document, offset, length);
      }
      catch (Exception)
      {
        return Enumerable.Empty<ISearchResult>();
      }
    }

    public IEnumerable<ISearchResult> FindAllValidating(ITextSource document, int offset, int length)
    {
      using (var stream = document.CreateReader())
      {
        var doc = (IDocument)document;
        var xmlDoc = new XPathDocument(stream); ;
        var navigator = xmlDoc.CreateNavigator();

        XPathExpression expr = null;
        XPathNodeIterator iterator;
        try
        {
          expr = navigator.Compile(_xPath);
          iterator = navigator.Select(expr);
        }
        catch (System.Xml.XPath.XPathException)
        {
          yield break;
        }

        while (iterator.MoveNext())
        {
          var current = iterator.Current;
          var segment = XmlSegment(doc, ((IXmlLineInfo)current).LineNumber, ((IXmlLineInfo)current).LinePosition);
          if (segment != null && segment.Offset >= offset && segment.EndOffset <= (offset + length))
          {
            yield return new XPathSearchResult()
            {
              StartOffset = segment.Offset,
              Length = segment.Length
            };
          }
        }
      }
    }

    public IEnumerable<ISearchResult> FindAllForwardOnly(ITextSource document, int offset, int length)
    {
      var xc = new XPathCollection();
      try
      {
        xc.Add(_xPath);
      }
      catch (Exception)
      {
        yield break;
      }
      using (var reader = new XmlTextReader(document.CreateReader()))
      using (var xpathReader = new XPathReader(reader, xc))
      {
        var lineInfo = xpathReader as IXmlLineInfo;
        var doc = (IDocument)document;
        ISegment segment;

        while (Read(xpathReader))
        {
          if (xpathReader.Match(0) && xpathReader.NodeType != XmlNodeType.EndElement)
          {
            segment = null;
            try
            {
              segment = XmlSegment(doc, lineInfo.LineNumber, lineInfo.LinePosition);
            }
            catch (Exception) { }
            if (segment != null && segment.Offset >= offset && segment.EndOffset <= (offset + length))
            {
              yield return new XPathSearchResult()
              {
                StartOffset = segment.Offset,
                Length = segment.Length
              };
            }
          }
        }
      }
    }

    private bool Read(XmlReader reader)
    {
      try
      {
        return reader.Read();
      }
      catch (Exception)
      {
        return false;
      }
    }

    private ISegment XmlSegment(IDocument doc, int lineNumber, int linePosition)
    {
      if (lineNumber < 1) return null;
      var offset = doc.GetOffset(lineNumber, linePosition) - 1;
      using (var stream = new StringReader(doc.GetText(offset, doc.TextLength - offset)))
      using (var reader = new XmlTextReader(stream))
      {
        while (reader.Read())
        {
          try
          {
            reader.ReadOuterXml();
          }
          catch (XmlException)
          {
            return null;
          }

          if (reader.LineNumber == 1)
          {
            return new TextSegment()
            {
              StartOffset = offset,
              Length = reader.LinePosition
            };
          }
          else
          {
            return new TextSegment()
            {
              StartOffset = offset,
              Length = doc.GetOffset(reader.LineNumber + lineNumber - 1, reader.LinePosition) - offset
            };
          }
        }
      }

      return null;
    }

    public ISearchResult FindNext(ITextSource document, int offset, int length)
    {
      return this.FindAll(document, offset, length).FirstOrDefault<ISearchResult>();
    }

    public bool Equals(ISearchStrategy other)
    {
      var xPath = other as XPathSearchStrategy;
      return xPath != null && xPath._xPath == this._xPath;
    }

    private class XPathSearchResult : TextSegment, ISearchResult
    {
      public string ReplaceWith(string replacement)
      {
        return replacement;
      }
    }
  }
}
