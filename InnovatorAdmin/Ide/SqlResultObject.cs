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
    private DataSet _dataSet;
    private TextDocument _doc;

    public SqlResultObject(DataSet dataSet, string text)
    {
      _dataSet = dataSet;
      _doc = new TextDocument(text);
    }

    public int ItemCount
    {
      get { return _dataSet.Tables.OfType<DataTable>().Sum(t => t.Rows.Count); }
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

    public DataSet GetDataSet()
    {
      return _dataSet;
    }

    public bool PreferTable
    {
      get { return _dataSet.Tables.Count > 0 && _dataSet.Tables[0].Rows.Count > 0; }
    }


    public string Title
    {
      get { return ""; }
    }
  }
}
