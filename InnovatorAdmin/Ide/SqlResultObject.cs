using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class SqlResultObject : IResultObject
  {
    private DataTable _table;
    private TextDocument _doc;

    public SqlResultObject(DataTable table, string text)
    {
      _table = table;
      _doc = new TextDocument(text);
    }

    public int ItemCount
    {
      get { return _table.Rows.Count; }
    }

    public string GetText()
    {
      return _doc.Text;
    }
    public TextDocument GetDocument()
    {
      return _doc;
    }
    public void SetText(string value)
    {
      _doc.Text = value;
    }

    public DataTable GetTable()
    {
      return _table;
    }

    public bool PreferTable
    {
      get { return _table.Rows.Count > 0; }
    }
  }
}
