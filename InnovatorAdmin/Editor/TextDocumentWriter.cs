using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.AvalonEdit.Document;

namespace InnovatorAdmin
{
  public class TextDocumentWriter : TextWriter
  {
    private TextDocument _doc;
    private int _undoStackSize;
    private int _i;
    private Encoding _encoding;

    public override Encoding Encoding
    {
      get
      {
        if (_encoding == null)
        {
          _encoding = new UnicodeEncoding(false, false);
        }
        return _encoding;
      }
    }

    public TextDocumentWriter(TextDocument doc)
    {
      _doc = doc;
      _doc.BeginUpdate();
      _undoStackSize = doc.UndoStack.SizeLimit;
      _doc.UndoStack.SizeLimit = 0;
      _i = _doc.TextLength;
    }

    public override void Write(char value)
    {
      _doc.Insert(_i, value.ToString());
      _i += 1;
    }

    public override void Write(char[] buffer, int index, int count)
    {
      _doc.Insert(_i, new string(buffer, index, count));
      _i += count;
    }

    public override void Write(string value)
    {
      _doc.Insert(_i, value);
      _i += value.Length;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        _doc.EndUpdate();
        _doc.UndoStack.SizeLimit = _undoStackSize;
      }
    }
  }
}
