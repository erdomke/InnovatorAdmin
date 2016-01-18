using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class XmlFormatter : IFormatProvider, ICustomFormatter
  {
    public string Format(string format, params object[] args)
    {
      return string.Format(this, format, args);
    }

    object IFormatProvider.GetFormat(Type formatType)
    {
      if (formatType == typeof(ICustomFormatter))
        return this;
      else
        return null;
    }

    string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
    {
      if (arg == null) return null;
      if (!this.Equals(formatProvider)) return null;
      var formattable = arg as IFormattable;
      var value = formattable == null ?
        arg.ToString() :
        formattable.ToString(format, CultureInfo.CurrentCulture);
      return new StringBuilder(value.Length + 10).AppendEscapedXml(value).ToString();
    }
  }
}
