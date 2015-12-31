using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
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
    private Rope<char> _rope;

    public SqlResultObject(DataSet dataSet, string text)
    {
      _dataSet = dataSet;
      _rope = new Rope<char>(text);
    }

    public int ItemCount
    {
      get { return _dataSet.Tables.OfType<DataTable>().Sum(t => t.Rows.Count); }
    }

    public string GetText()
    {
      return _rope.ToString();
    }
    public ITextSource GetDocument()
    {
      return new RopeTextSource(_rope);
    }
    public void SetText(string value)
    {
      _rope = new Rope<char>(value);
    }

    public DataSet GetDataSet()
    {
      return _dataSet;
    }

    public string Title
    {
      get { return ""; }
    }

    public OutputType PreferredMode
    {
      get { return _dataSet.Tables.Count > 0 && _dataSet.Tables[0].Rows.Count > 0 ? OutputType.Table : OutputType.Text; }
    }

    public string Html
    {
      get { return string.Empty; }
    }
  }
}
