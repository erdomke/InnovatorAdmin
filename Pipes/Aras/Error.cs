using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class Error : IError
  {
    private string _code;
    private Xml.IXmlElement _data;
    private string _detail;
    private string _message;

    public string Code
    {
      get { return _code; }
      set { _code = value;  }
    }
    public Xml.IXmlElement Data
    {
      get { return _data; }
      set { _data = value; }
    }
    public string Detail
    {
      get { return _detail; }
      set { _detail = value; }
    }
    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }

    public Error() {}
    public Error(string code, string message)
    {
      _code = code;
      _message = message;
    }
  }
}
