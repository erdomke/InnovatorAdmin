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
    private string _text;

    public SqlResultObject(DataTable table, string text)
    {
      _table = table;
      _text = text;
    }

    public int ItemCount
    {
      get { return _table.Rows.Count; }
    }

    public string GetText()
    {
      return _text;
    }
    public void SetText(string value)
    {
      _text = value;
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
