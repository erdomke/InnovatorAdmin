using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Pipes.Conversion
{
  public static class BaseConverters
  {
    public static object Convert(Type type, object value)
    {
      try
      {
        int intValue = 0;
        if ((value != null && type.IsAssignableFrom(value.GetType())) || 
            (value == null && (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)))
        {
          return value;
        }
        else if (type == typeof(bool) && value is string)
        {
          if (value.Equals("1"))
          {
            return true;
          }
          else if (value.Equals("0"))
          {
            return false;
          }
        }
        else if (type == typeof(int) && IntegerTryParse(value as string, out intValue))
        {
          return intValue;
        }
        else if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(type) && !type.IsEnum)
        {
          //For base conversion not involving enumerables, try using the conversion methods.  Otherwise, use a type converter
          try
          {
            return System.Convert.ChangeType(value, type);
          }
          catch { }
        }

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFrom(value);
      }
      catch (NotSupportedException ex)
      {
        if (value == null)
        {
          throw new NotSupportedException(string.Format("Unable to convert {{nothing}} to {0}", type.FullName), ex);
        }
        else
        {
          throw new NotSupportedException(string.Format("Unable to convert {0} to {1}", value, type.FullName), ex);
        }
      }
    }

    /// <summary>
    /// Extend the default integer parsing by allowing the user to enter a string representing a double which is equivalent to an integer (e.g. 13.0)
    /// </summary>
    /// <param name="value">String value representing a number which can be converted to an integer with no data loss</param>
    /// <param name="intValue">Integer value that was parsed</param>
    /// <returns><c>true</c> if the conversion was successful, otherwise <c>false</c></returns>
    public static bool IntegerTryParse(string value, out int intValue)
    {
      intValue = 0;
      if (string.IsNullOrEmpty(value))
      {
        return false;
      }
      else if (int.TryParse(value, out intValue))
      {
        return true;
      }
      else
      {
        double dblValue = 0;
        if (double.TryParse(value, out dblValue))
        {
          intValue = System.Convert.ToInt32(dblValue);
          return System.Convert.ToDouble(intValue) == dblValue;
        }
        else
        {
          return false;
        }
      }
    }
  }
}
