using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public static class DataExtensions
  {
    public static bool Assert<T>(this IDataRecord record, string name, Func<T, bool> predicate)
    {
      if (record.Status(name) == FieldStatus.FilledIn)
      {
        return predicate.Invoke((T)Conversion.BaseConverters.Convert(typeof(T), record.Item(name)));
      }
      return false;
    }
    public static T Item<T>(this IDataRecord record, string name)
    {
      return (T)Conversion.BaseConverters.Convert(typeof(T), record.Item(name));
    }
    public static T Item<T>(this IDataRecord record, string name, Func<Type, object, object> converter)
    {
      return (T)converter(typeof(T), record.Item(name));
    }
    public static T Val<T>(this IFieldValue value)
    {
      return (T)Conversion.BaseConverters.Convert(typeof(T), value.Value);
    }
    public static T Val<T>(this IFieldValue value, Func<Type, object, object> converter)
    {
      return (T)converter(typeof(T), value.Value);
    }
    public static IEnumerable<string> GetNames(this IEnumerable<IFieldValue> data)
    {
      return data.Select((d) => d.Name);
    }
    public static bool IsNullOrEmpty(this FieldStatus value)
    {
      return value != FieldStatus.FilledIn;
    }


    public static void ReplaceSqlParams(this Pipes.Code.ICodeTextWriter writer, string sql, char paramPrefix, Action<string> replace)
    {
      char endChar = '\0';
      int i = 0;
      var paramName = new StringBuilder();
      int lastWrite = 0;

      while(i < sql.Length)
      {
        if (endChar == '\0')
        {
          switch (sql[i])
          {
            case '\'':
              endChar = '\'';
              break;
            case '"':
              endChar = '"';
              break;
            case '[':
              endChar = ']';
              break;
            case '-':
              if (i + 1 < sql.Length && sql[i + 1] == '-')
              {
                endChar = '\n';
              }
              break;
            case '/':
              if (i + 1 < sql.Length && sql[i + 1] == '*')
              {
                endChar = '/';
              }
              break;
          }

          if (sql[i] == paramPrefix)
          {
            writer.Raw(sql.Substring(lastWrite, i - lastWrite));
            i++;
            paramName.Length = 0;
            while (i < sql.Length && (Char.IsLetterOrDigit(sql[i]) || sql[i] == '_')) {
              paramName.Append(sql[i]);
              i++;
            }
            replace.Invoke(paramName.ToString());
            lastWrite = i;
            i--;
          }
        }
        else if ((endChar == '\n' && sql[i] == '\r') ||
                 (endChar == '/' && sql[i] == '*' && i + 1 < sql.Length && sql[i + 1] == '/') ||
                 (sql[i] == endChar))
        {
          endChar = '\0';
        }
        i++;
      }

      if ((i - lastWrite) > 0) writer.Raw(sql.Substring(lastWrite, i - lastWrite));
    }
  }
}
