using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class RopeWriter : TextWriter //, ITextSource
  {
    private Encoding _encoding;
    private Rope<char> _rope;

    public Rope<char> Rope
    {
      get { return _rope; }
    }
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

    public RopeWriter()
    {
      _rope = new Rope<char>();
    }
    public RopeWriter(Rope<char> rope)
    {
      _rope = rope;
    }

    public override void Write(char value)
    {
      _rope.Add(value);
    }

    public override void Write(char[] buffer, int index, int count)
    {
      _rope.AddRange(buffer, index, count);
    }

    public override void Write(string value)
    {
      _rope.AddText(value);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

    //public TextReader CreateReader(int offset, int length)
    //{
    //  return new RopeTextReader(_rope.GetRange(offset, length));
    //}

    //public TextReader CreateReader()
    //{
    //  return new RopeTextReader(_rope);
    //}

    //public ITextSource CreateSnapshot(int offset, int length)
    //{
    //  return new RopeTextSource(_rope.GetRange(offset, length));
    //}

    //public ITextSource CreateSnapshot()
    //{
    //  return this;
    //}

    //public char GetCharAt(int offset)
    //{
    //  return _rope[offset];
    //}

    //public string GetText(ISegment segment)
    //{
    //  return _rope.ToString(segment.Offset, segment.Length);
    //}

    //public string GetText(int offset, int length)
    //{
    //  return _rope.ToString(offset, length);
    //}

    //public int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
    //{
    //  return _rope.IndexOf(searchText, startIndex, count, comparisonType);
    //}

    //public int IndexOf(char c, int startIndex, int count)
    //{
    //  return _rope.IndexOf(c, startIndex, count);
    //}

    //public int IndexOfAny(char[] anyOf, int startIndex, int count)
    //{
    //  return _rope.IndexOfAny(anyOf, startIndex, count);
    //}

    //public int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType)
    //{
    //  return _rope.LastIndexOf(searchText, startIndex, count, comparisonType);
    //}

    //public int LastIndexOf(char c, int startIndex, int count)
    //{
    //  return _rope.LastIndexOf(c, startIndex, count);
    //}

    //public string Text
    //{
    //  get { return _rope.ToString(); }
    //}

    //public int TextLength
    //{
    //  get { return _rope.Length; }
    //}

    //public ITextSourceVersion Version
    //{
    //  get { return null; }
    //}

    //public void WriteTextTo(TextWriter writer, int offset, int length)
    //{
    //  _rope.WriteTo(writer, offset, length);
    //}

    //public void WriteTextTo(TextWriter writer)
    //{
    //  _rope.WriteTo(writer, 0, _rope.Length);
    //}
  }
}
