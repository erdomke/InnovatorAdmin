using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Pipes.Google
{
  public class VisJsonTextWriter : Json.JsonTextWriter
  {
    public VisJsonTextWriter(TextWriter writer) : base(writer) { }

    public override Json.IJsonWriter Prop(string name)
    {
      if (_needsComma) _writer.Write(",");
      _writer.Write(name);
      _writer.Write(":");
      _needsComma = false;
      return this;
    }
    public Json.IJsonWriter Value(object obj, DateType type)
    {
      if (obj is DateTime)
      {
        if (_needsComma) _writer.Write(",");
        _needsComma = true;
        DateTime dt = (DateTime)obj;
        switch (type)
        {
          case DateType.Date:
            if (_useUTCDateTime) dt = TimeZoneInfo.ConvertTimeToUtc(dt);
            _writer.Write("new Date(");
            _writer.Write(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            _writer.Write(", ");
            _writer.Write((dt.Month - 1).ToString(NumberFormatInfo.InvariantInfo));
            _writer.Write(", ");
            _writer.Write(dt.Day.ToString(NumberFormatInfo.InvariantInfo));
            _writer.Write(")");
            break;
          case DateType.TimeOfDay:
            if (_useUTCDateTime) dt = TimeZoneInfo.ConvertTimeToUtc(dt);
            Array();
            base.Value(dt.Hour);
            base.Value(dt.Minute);
            base.Value(dt.Second);
            base.Value(dt.Millisecond);
            ArrayEnd();
            break;
          default:
            WriteDateTime(dt);
            break;
        }

        return this;
      }
      else
      {
        return base.Value(obj);
      }
    }
    protected override void WriteStringCore(string value, bool fast)
    {
      if (value == null)
      {
        WriteNull();
      }
      else
      {
        _writer.Write("'");
        if (fast)
        {
          _writer.Write(value);
        }
        else
        {
          WriteStringValueCore(value, '\'');
        }
        _writer.Write("'");
      }
    }
    protected override void WriteDateTime(DateTime dt)
    {
      // datetime format standard : yyyy-MM-dd HH:mm:ss
      if (_useUTCDateTime) dt = TimeZoneInfo.ConvertTimeToUtc(dt);

      _writer.Write("new Date(");
      _writer.Write(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
      _writer.Write(", ");
      _writer.Write((dt.Month -1).ToString(NumberFormatInfo.InvariantInfo));
      _writer.Write(", ");
      _writer.Write(dt.Day.ToString(NumberFormatInfo.InvariantInfo));
      _writer.Write(", ");
      _writer.Write(dt.Hour.ToString(NumberFormatInfo.InvariantInfo));
      _writer.Write(", ");
      _writer.Write(dt.Minute.ToString(NumberFormatInfo.InvariantInfo));
      _writer.Write(", ");
      _writer.Write(dt.Second.ToString(NumberFormatInfo.InvariantInfo));
      _writer.Write(")");
    }
  }
}
