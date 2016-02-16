using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public class ParameterSubstitution
  {
    private IServerContext _context;
    private Dictionary<string, object> _parameters = new Dictionary<string, object>();
    private int _itemCount = 0;

    public int ItemCount { get { return _itemCount; } }
    public int ParamCount { get { return _parameters.Count; } }
    public Action<string> ParameterAccessListener { get; set; }

    public ParameterSubstitution() { }

    public void AddParameter(string name, object value)
    {
      _parameters.Add(name, value);
    }

    public string Substitute(string query, IServerContext context)
    {
      _context = context;
      if (string.IsNullOrEmpty(query)) return query;
      var i = 0;
      while (i < query.Length && char.IsWhiteSpace(query[i])) i++;
      if (i >= query.Length) return query;

      if (query[i] == '<')
      {
        var builder = new StringBuilder(query.Length);
        using (var reader = new StringReader(query))
        using (var xmlReader = XmlReader.Create(reader))
        using (var writer = new StringWriter(builder))
        using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings() { OmitXmlDeclaration = true }))
        {
          string condition = null;
          var hasItem = false;
          var tagNames = new List<string>();
          string tagName;
          Parameter param;

          while (xmlReader.Read())
          {
            switch (xmlReader.NodeType)
            {
              case XmlNodeType.CDATA:
                param = RenderValue(condition, xmlReader.Value);
                if (param.IsRaw)
                  throw new InvalidOperationException("Can't have a raw parameter in a CDATA section");
                xmlWriter.WriteCData(param.Value);
                break;
              case XmlNodeType.Comment:
                xmlWriter.WriteComment(xmlReader.Value);
                break;
              case XmlNodeType.Element:
                xmlWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName
                  , xmlReader.NamespaceURI);
                tagName = xmlReader.LocalName;

                var isEmpty = xmlReader.IsEmptyElement;
                for (i = 0; i < xmlReader.AttributeCount; i++)
                {
                  xmlReader.MoveToAttribute(i);
                  param = RenderValue(xmlReader.LocalName, xmlReader.Value);
                  if (param.IsRaw)
                    throw new InvalidOperationException("Can't have a raw parameter in an attribute");
                  xmlWriter.WriteAttributeString(xmlReader.Prefix, xmlReader.LocalName
                    , xmlReader.NamespaceURI, param.Value);
                  if (xmlReader.LocalName == "condition")
                  {
                    condition = xmlReader.Value;
                  }
                }

                switch (tagName)
                {
                  case "Item":
                    hasItem = true;
                    if (!tagNames.Any(n => n == "Item")) _itemCount++;
                    break;
                  case "sql":
                  case "SQL":
                    if (!hasItem) condition = "sql";
                    break;
                }

                if (isEmpty)
                {
                  xmlWriter.WriteEndElement();
                }
                else
                {
                  tagNames.Add(tagName);
                }
                break;
              case XmlNodeType.EndElement:
                xmlWriter.WriteEndElement();
                tagNames.RemoveAt(tagNames.Count - 1);
                break;
              case XmlNodeType.SignificantWhitespace:
                xmlWriter.WriteWhitespace(xmlReader.Value);
                break;
              case XmlNodeType.Text:
                param = RenderValue(condition, xmlReader.Value);
                if (param.IsRaw)
                {
                  xmlWriter.WriteRaw(param.Value);
                }
                else
                {
                  xmlWriter.WriteValue(param.Value);
                }
                break;
            }

          }
        }
        return builder.ToString();
      }
      else
      {
        return SqlReplace(query);
      }
    }

    internal string RenderParameter(string name, IServerContext context)
    {
      _context = context;
      return RenderValue("", name).Value;
    }
    private Parameter RenderValue(string context, string content)
    {
      object value;
      var param = new Parameter();
      if (content.IsNullOrWhitespace())
      {
        return param.WithValue(content);
      }
      else if (TryFillParameter(content, param) && TryGetParamValue(param.Name, out value))
      {
        if (param.IsRaw) return param.WithValue((value ?? "").ToString());

        switch (context)
        {
          case "idlist":
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
          case "in":
          case "not in":
            return param.WithValue(RenderSqlEnum(value, true, o => _context.Format(o)));
          case "like":
          case "not like":
            // Do something useful with context
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
          default:
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
        }
      }
      else if (context == "between" || context == "not between")
      {
        // Do something useful here
        return param.WithValue(content);
      }
      else if (context == "sql"
        || context == "where")
      {
        return param.WithValue(SqlReplace(content));
      }
      else if ((context == "in" || context == "not in")
            && content.TrimStart()[0] == '(')
      {
        content = content.Trim();
        if (content.Length > 2 && content[1] == '@')
        {
          // Dapper is trying to be too helpful with parameter expansion
          return param.WithValue(SqlReplace(content.TrimStart('(').TrimEnd(')')));
        }
        else
        {
          return param.WithValue(SqlReplace(content));
        }
      }
      else
      {
        return param.WithValue(content);
      }
    }

    private bool TryGetNumericEnumerable(object value, out IEnumerable enumerable)
    {
      if (value is IEnumerable<short>
        || value is IEnumerable<int>
        || value is IEnumerable<long>
        || value is IEnumerable<ushort>
        || value is IEnumerable<uint>
        || value is IEnumerable<ulong>
        || value is IEnumerable<byte>
        || value is IEnumerable<decimal>)
      {
        enumerable = (IEnumerable)value;
        return true;
      }
      else if (value is IEnumerable<float>)
      {
        enumerable = ((IEnumerable<float>)value).Cast<decimal>();
        return true;
      }
      else if (value is IEnumerable<double>)
      {
        enumerable = ((IEnumerable<double>)value).Cast<decimal>();
        return true;
      }
      enumerable = null;
      return false;
    }

    private bool TryGetParamValue(string name, out object value)
    {
      if (ParameterAccessListener != null)
        ParameterAccessListener.Invoke(name);

      return _parameters.TryGetValue(name, out value);
    }

    private bool TryFillParameter(string value, Parameter param)
    {
      if (value == null || value.Length < 2) return false;
      if (value[0] != '@') return false;

      var end = value.Length;
      if (value[value.Length - 1] == '!')
      {
        param.IsRaw = true;
        end--;
      }
      for (var i = 1; i < end; i++)
      {
        if (!char.IsLetterOrDigit(value[i]) && value[i] != '_') return false;
      }
      param.Name = value.Substring(1, end - 1);
      return true;
    }

    private string RenderSqlEnum(object value, bool quoteStrings, Func<object, string> format)
    {
      IEnumerable enumerable = value as IEnumerable;
      bool first = true;
      var builder = new StringBuilder();
      if (value is string)
      {
        // Deal with strings which technically are enumerables
        builder.Append(format.Invoke(value));
      }
      else if ((!quoteStrings && enumerable != null) || TryGetNumericEnumerable(value, out enumerable))
      {
        foreach (var item in enumerable)
        {
          if (!first) builder.Append(",");
          builder.Append(format.Invoke(item));
          first = false;
        }
      }
      else
      {
        enumerable = value as IEnumerable;
        if (enumerable != null)
        {
          foreach (var item in enumerable)
          {
            if (!first) builder.Append(",");
            builder.Append("N'").Append(format.Invoke(item)).Append("'");
            first = false;
          }
        }
        else
        {
          builder.Append(format.Invoke(value));
        }
      }
      return builder.ToString();
    }

    private string SqlReplace(string query)
    {
      var builder = new StringBuilder(query.Length);
      var formatter = new SqlFormatter(_context);
      SqlReplace(query, '@', builder, p =>
      {
        object value;
        if (TryGetParamValue(p, out value))
        {
          IFormattable num;
          if (ServerContext.TryCastNumber(value, out num))
          {
            return formatter.Format(num);
          }
          else if (builder.ToString().EndsWith(" in ") && value is IEnumerable)
          {
            return "(" + RenderSqlEnum(value, true, o => formatter.Format(o)) + ")";
          }
          else
          {
            return "N'" + RenderSqlEnum(value, false, o => formatter.Format(o)) + "'";
          }
        }
        else
        {
          return "@" + p;
        }
      });
      return builder.ToString();
    }

    private void SqlReplace(string sql, char paramPrefix, StringBuilder builder, Func<string, string> replace)
    {
      char endChar = '\0';
      int i = 0;
      var paramName = new StringBuilder(32);
      int lastWrite = 0;

      while (i < sql.Length)
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
            builder.Append(sql.Substring(lastWrite, i - lastWrite));
            i++;
            paramName.Length = 0;
            while (i < sql.Length && (Char.IsLetterOrDigit(sql[i]) || sql[i] == '_'))
            {
              paramName.Append(sql[i]);
              i++;
            }
            builder.Append(replace.Invoke(paramName.ToString()));
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

      if ((i - lastWrite) > 0) builder.Append(sql.Substring(lastWrite, i - lastWrite));
    }

    private class Parameter
    {
      public string Name { get; set; }
      public bool IsRaw { get; set; }
      public string Value { get; set; }

      public Parameter WithValue(string value)
      {
        this.Value = value;
        return this;
      }
    }
  }
}
