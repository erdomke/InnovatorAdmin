using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlCompletionHelper
  {
    private ISqlMetadataProvider _provider;

    public TextArea CurrentTextArea { get; set; }

    public SqlCompletionHelper(ISqlMetadataProvider provider)
    {
      _provider = provider;
    }

    public IPromise<CompletionContext> Completions(string prefix, string all, int caret, string termCharacter)
    {
      try
      {
        var lastIndex = string.IsNullOrEmpty(termCharacter) ? -1 : all.IndexOf(termCharacter, caret);
        var sql = prefix + (lastIndex < 0 ? all.Substring(caret) : all.Substring(caret, lastIndex - caret));
        if (sql.StartsWith("(") && sql.EndsWith(")"))
          sql = sql.Substring(1, sql.Length - 2);
        var parseTree = new SqlTokenizer(sql).Parse();
        if (!parseTree.Any())
          return Promises.Resolved(new CompletionContext());

        var currNode = parseTree.NodeByOffset(prefix.Length);
        var literal = currNode as SqlLiteral;

        if (literal != null)
        {
          if (SqlTokenizer.KeywordPrecedesTable(literal.Text))
          {
            return Promises.Resolved(new CompletionContext()
            {
              Items = _provider.GetTableNames()
                .Concat(_provider.GetSchemaNames())
                .GetCompletions<SqlObjectCompletionData>()
                .OrderBy(i => i.Text),
              Overlap = 0
            });
          }
          else if (literal.Text == "(")
          {

            var prev = literal.PreviousLiteral();

            if (prev != null)
            {
              if (CurrentTextArea != null)
              {
                var overloads = from f in _coreFunctions
                                where string.Equals(f.Name, prev.Text, StringComparison.OrdinalIgnoreCase)
                                select new Overload(f.Usage, f.Description);
                if (overloads.Any())
                {
                  var overload = new OverloadInsightWindow(CurrentTextArea);
                  overload.StartOffset = caret;
                  overload.EndOffset = caret + 1;
                  overload.Provider = new OverloadList().AddRange(overloads);
                  overload.Show();
                }
              }

              switch (prev.Text.ToUpperInvariant())
              {
                case "DATEADD":
                case "DATEDIFF":
                case "DATEDIFF_BIG":
                case "DATEFROMPARTS":
                case "DATENAME":
                case "DATEPART":
                  return Promises.Resolved(new CompletionContext()
                  {
                    Items = _datePartNames.Select(n => new SqlGeneralCompletionData()
                      {
                        Text = n[0] + (n[1] == n[0] ? "" : " (" + n[1] + ")"),
                        Description = n[1],
                        Action = () => n[0]
                      })
                      .OrderBy(i => i.Text),
                    Overlap = 0
                  });
              }
            }
          }
          else if (literal.Text == ".")
          {
            var name = literal.Parent as SqlNameDefinition;
            if (name != null)
            {
              var idx = name.IndexOf(literal);
              var schema = name[idx - 1].Text;
              if (_provider.GetSchemaNames().Contains(schema, StringComparer.OrdinalIgnoreCase))
              {
                return Promises.Resolved(new CompletionContext()
                {
                  Items = _provider.GetTableNames()
                    .Where(t => t.StartsWith(schema + ".", StringComparison.OrdinalIgnoreCase))
                    .Select(t => t.Substring(schema.Length + 1))
                    .GetCompletions<SqlObjectCompletionData>()
                    .OrderBy(i => i.Text),
                  Overlap = 0
                });
              }
            }
            else
            {
              var group = literal.Parent as SqlGroup;
              if (group != null)
              {
                var idx = group.IndexOf(literal);
                var context = GetAliasContext(literal);
                string fullName;
                if (idx > 0 && group[idx - 1] is SqlLiteral
                  && context.TryGetValue(((SqlLiteral)group[idx - 1]).Text.ToLowerInvariant(), out fullName))
                {
                  return _provider.GetColumnNames(fullName)
                    .Convert(p => new CompletionContext()
                    {
                      Items = new PropertyCompletionFactory().GetCompletions(p)
                    });
                }
              }
            }
          }
          else if (SqlTokenizer.IsKeyword(literal.Text)
            || literal.Type == SqlType.Operator)
          {
            var group = literal.Parent as SqlGroup;
            if (group != null)
            {
              var others = new List<string>();

              switch (literal.Text.ToLowerInvariant())
              {
                case "select":
                  others.Add("*");
                  break;
              }

              var types = group.OfType<SqlNameDefinition>().Select(n => _provider.GetColumnNames(n.Name)).ToArray();
              return Promises.All(types)
                .Convert(l => new CompletionContext()
                {
                  Items = l.OfType<IEnumerable<IListValue>>()
                    .SelectMany(i => new PropertyCompletionFactory().GetCompletions(i))
                    .Distinct()
                    .Concat(_coreFunctions.Select(f => new SqlGeneralCompletionData()
                    {
                      Text = f.Name + "(",
                      Content = f.Name,
                      Description = f.Description
                    }))
                    .Concat(others.GetCompletions<SqlGeneralCompletionData>())
                    .OrderBy(i => i.Text)
                });
            }
          }
        }

        return Promises.Resolved(new CompletionContext());
      }
      catch (Exception ex)
      {
        return Promises.Rejected<CompletionContext>(ex);
      }
    }

    private IDictionary<string, string> GetAliasContext(SqlNode node)
    {
      var result = new Dictionary<string, string>();

      var group = node.Parent as SqlGroup;
      while (group != null)
      {
        foreach (var name in group.OfType<SqlNameDefinition>())
        {
          if (name.Alias != null && !result.ContainsKey(name.Alias.ToLowerInvariant()))
            result[name.Alias.ToLowerInvariant()] = name.FullName;
          result[name.FullName.ToLowerInvariant()] = name.FullName;
        }
        group = group.Parent as SqlGroup;
      }

      return result;
    }

    private static string[][] _datePartNames = new string[][] {
      new string[] { "year", "year"},
      new string[] { "yy", "year"},
      new string[] { "yyyy", "year"},
      new string[] { "quarter", "quarter"},
      new string[] { "qq", "quarter"},
      new string[] { "q", "quarter"},
      new string[] { "month", "month"},
      new string[] { "mm", "month"},
      new string[] { "m", "month"},
      new string[] { "dayofyear", "dayofyear"},
      new string[] { "dy", "dayofyear"},
      new string[] { "y", "dayofyear"},
      new string[] { "day", "day"},
      new string[] { "dd", "day"},
      new string[] { "d", "day"},
      new string[] { "week", "week"},
      new string[] { "wk", "week"},
      new string[] { "ww", "week"},
      new string[] { "weekday", "weekday"},
      new string[] { "dw", "weekday"},
      new string[] { "w", "weekday"},
      new string[] { "hour", "hour"},
      new string[] { "hh", "hour"},
      new string[] { "minute", "minute"},
      new string[] { "mi", "minute"},
      new string[] { "n", "minute"},
      new string[] { "second", "second"},
      new string[] { "ss", "second"},
      new string[] { "s", "second"},
      new string[] { "millisecond", "millisecond"},
      new string[] { "ms", "millisecond"},
      new string[] { "microsecond", "microsecond"},
      new string[] { "mcs", "microsecond"},
      new string[] { "nanosecond", "nanosecond"},
      new string[] { "ns", "nanosecond"},
      new string[] { "TZoffset", "TZoffset"},
      new string[] { "tz", "TZoffset"},
      new string[] { "ISO_WEEK", "ISO_WEEK"},
      new string[] { "ISOWK", "ISO_WEEK"},
      new string[] { "ISOWW", "ISO_WEEK"}
    };

    private static SqlCoreFunction[] _coreFunctions = new SqlCoreFunction[] {
      new SqlCoreFunction("ASCII","Returns the ASCII code value of the leftmost character of a character expression.","ASCII ( character_expression )"),
      new SqlCoreFunction("CHAR","Converts an int ASCII code to a character.","CHAR ( integer_expression )"),
      new SqlCoreFunction("CHARINDEX","Searches an expression for another expression and returns its starting position if found. ","CHARINDEX ( expressionToFind ,expressionToSearch [ , start_location ] ) "),
      new SqlCoreFunction("CONCAT","Returns a string that is the result of concatenating two or more string values.","CONCAT ( string_value1, string_value2 [, string_valueN ] )"),
      new SqlCoreFunction("DIFFERENCE","Returns an integer value that indicates the difference between the SOUNDEX values of two character expressions.","DIFFERENCE ( character_expression , character_expression )"),
      new SqlCoreFunction("FORMAT","Returns a value formatted with the specified format and optional culture in SQL Server 2016. Use the FORMAT function for locale-aware formatting of date/time and number values as strings. For general data type conversions, use CAST or CONVERT.","FORMAT ( value, format [, culture ] )"),
      new SqlCoreFunction("LEFT","Returns the left part of a character string with the specified number of characters. ","LEFT ( character_expression , integer_expression )"),
      new SqlCoreFunction("LEN","Returns the number of characters of the specified string expression, excluding trailing blanks. ","LEN ( string_expression )"),
      new SqlCoreFunction("LOWER","Returns a character expression after converting uppercase character data to lowercase.","LOWER ( character_expression )"),
      new SqlCoreFunction("LTRIM","Returns a character expression after it removes leading blanks.","LTRIM ( character_expression )"),
      new SqlCoreFunction("NCHAR","Returns the Unicode character with the specified integer code, as defined by the Unicode standard. ","NCHAR ( integer_expression )"),
      new SqlCoreFunction("PATINDEX","Returns the starting position of the first occurrence of a pattern in a specified expression, or zeros if the pattern is not found, on all valid text and character data types. ","PATINDEX ( '%pattern%' , expression )"),
      new SqlCoreFunction("QUOTENAME","Returns a Unicode string with the delimiters added to make the input string a valid SQL Server delimited identifier.","QUOTENAME ( 'character_string' [ , 'quote_character' ] ) "),
      new SqlCoreFunction("REPLACE","Replaces all occurrences of a specified string value with another string value.","REPLACE ( string_expression , string_pattern , string_replacement )"),
      new SqlCoreFunction("REPLICATE","Repeats a string value a specified number of times.","REPLICATE ( string_expression ,integer_expression ) "),
      new SqlCoreFunction("REVERSE","Returns the reverse order of a string value.","REVERSE ( string_expression )"),
      new SqlCoreFunction("RIGHT","Returns the right part of a character string with the specified number of characters. ","RIGHT ( character_expression , integer_expression )"),
      new SqlCoreFunction("RTRIM","Returns a character string after truncating all trailing blanks.","RTRIM ( character_expression )"),
      new SqlCoreFunction("SOUNDEX","Returns a four-character (SOUNDEX) code to evaluate the similarity of two strings.","SOUNDEX ( character_expression )"),
      new SqlCoreFunction("SPACE","Returns a string of repeated spaces.","SPACE ( integer_expression )"),
      new SqlCoreFunction("STR","Returns character data converted from numeric data.","STR ( float_expression [ , length [ , decimal ] ] )"),
      new SqlCoreFunction("STUFF","The STUFF function inserts a string into another string. It deletes a specified length of characters in the first string at the start position and then inserts the second string into the first string at the start position.","STUFF ( character_expression , start , length , replaceWith_expression )"),
      new SqlCoreFunction("SUBSTRING","Returns part of a character, binary, text, or image expression in SQL Server.","SUBSTRING ( expression ,start , length )"),
      new SqlCoreFunction("UNICODE","Returns the integer value, as defined by the Unicode standard, for the first character of the input expression. ","UNICODE ( 'ncharacter_expression' )"),
      new SqlCoreFunction("UPPER","Returns a character expression with lowercase character data converted to uppercase.","UPPER ( character_expression )"),
      new SqlCoreFunction("ABS","A mathematical function that returns the absolute (positive) value of the specified numeric expression.","ABS ( numeric_expression )"),
      new SqlCoreFunction("ACOS","A mathematical function that returns the angle, in radians, whose cosine is the specified float expression; also called arccosine.","ACOS ( float_expression )"),
      new SqlCoreFunction("ASIN","Returns the angle, in radians, whose sine is the specified float expression. This is also called arcsine.","ASIN ( float_expression )"),
      new SqlCoreFunction("ATAN","Returns the angle in radians whose tangent is a specified float expression. This is also called arctangent.","ATAN ( float_expression )"),
      new SqlCoreFunction("ATN2","Returns the angle, in radians, between the positive x-axis and the ray from the origin to the point (y, x), where x and y are the values of the two specified float expressions.","ATN2 ( float_expression , float_expression )"),
      new SqlCoreFunction("CEILING","Returns the smallest integer greater than, or equal to, the specified numeric expression.","CEILING ( numeric_expression )"),
      new SqlCoreFunction("COS","Is a mathematical function that returns the trigonometric cosine of the specified angle, in radians, in the specified expression.","COS ( float_expression )"),
      new SqlCoreFunction("COT","A mathematical function that returns the trigonometric cotangent of the specified angle, in radians, in the specified float expression.","COT ( float_expression )"),
      new SqlCoreFunction("DEGREES","Returns the corresponding angle in degrees for an angle specified in radians.","DEGREES ( numeric_expression )"),
      new SqlCoreFunction("EXP","Returns the exponential value of the specified float expression.","EXP ( float_expression )"),
      new SqlCoreFunction("FLOOR","Returns the largest integer less than or equal to the specified numeric expression.","FLOOR ( numeric_expression )"),
      new SqlCoreFunction("LOG","Returns the natural logarithm of the specified float expression in SQL Server.","LOG ( float_expression [, base ] )"),
      new SqlCoreFunction("LOG10","Returns the base-10 logarithm of the specified float expression.","LOG10 ( float_expression )"),
      new SqlCoreFunction("PI","Returns the constant value of PI.","PI ( )"),
      new SqlCoreFunction("POWER","Returns the value of the specified expression to the specified power.","POWER ( float_expression , y )"),
      new SqlCoreFunction("RADIANS","Returns radians when a numeric expression, in degrees, is entered.","RADIANS ( numeric_expression )"),
      new SqlCoreFunction("RAND","Returns a pseudo-random float value from 0 through 1, exclusive.","RAND ( [ seed ] )"),
      new SqlCoreFunction("ROUND","Returns a numeric value, rounded to the specified length or precision.","ROUND ( numeric_expression , length [ ,function ] )"),
      new SqlCoreFunction("SIGN","Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.","SIGN ( numeric_expression )"),
      new SqlCoreFunction("SIN","Returns the trigonometric sine of the specified angle, in radians, and in an approximate numeric, float, expression.","SIN ( float_expression )"),
      new SqlCoreFunction("SQRT","Returns the square root of the specified float value.","SQRT ( float_expression )"),
      new SqlCoreFunction("SQUARE","Returns the square of the specified float value.","SQUARE ( float_expression )"),
      new SqlCoreFunction("TAN","Returns the tangent of the input expression. ","TAN ( float_expression )"),
      new SqlCoreFunction("CHOOSE","Returns the item at the specified index from a list of values in SQL Server.","CHOOSE ( index, val_1, val_2 [, val_n ] )"),
      new SqlCoreFunction("IIF","Returns one of two values, depending on whether the Boolean expression evaluates to true or false in SQL Server.","IIF ( boolean_expression, true_value, false_value )"),
      new SqlCoreFunction("DATALENGTH","Returns the number of bytes used to represent any expression.","DATALENGTH ( expression ) "),
      new SqlCoreFunction("IDENT_CURRENT","Returns the last identity value generated for a specified table or view. The last identity value generated can be for any session and any scope. ","IDENT_CURRENT( 'table_name' )"),
      new SqlCoreFunction("IDENT_INCR","Returns the increment value (returned as numeric (@@MAXPRECISION,0)) specified during the creation of an identity column in a table or view that has an identity column. ","IDENT_INCR ( 'table_or_view' )"),
      new SqlCoreFunction("IDENT_SEED","Returns the original seed value (returned as numeric(@@MAXPRECISION,0)) that was specified when an identity column in a table or a view was created. Changing the current value of an identity column by using DBCC CHECKIDENT does not change the value returned by this function.","IDENT_SEED ( 'table_or_view' )"),
      new SqlCoreFunction("IDENTITY (Function)","Is used only in a SELECT statement with an INTO table clause to insert an identity column into a new table. Although similar, the IDENTITY function is not the IDENTITY property that is used with CREATE TABLE and ALTER TABLE. ","IDENTITY (data_type [ , seed , increment ] ) AS column_name"),
      new SqlCoreFunction("SQL_VARIANT_PROPERTY","Returns the base data type and other information about a sql_variant value.","SQL_VARIANT_PROPERTY ( expression , property )"),
      new SqlCoreFunction("CAST","Converts an expression of one data type to another in SQL Server 2016.","CAST ( expression AS data_type [ ( length ) ] )"),
      new SqlCoreFunction("CONVERT","Converts an expression of one data type to another in SQL Server 2016.","CONVERT ( data_type [ ( length ) ] , expression [ , style ] )"),
      new SqlCoreFunction("PARSE","Returns the result of an expression, translated to the requested data type in SQL Server.","PARSE ( string_value AS data_type [ USING culture ] )"),
      new SqlCoreFunction("TRY_CAST","Returns a value cast to the specified data type if the cast succeeds; otherwise, returns null.","TRY_CAST ( expression AS data_type [ ( length ) ] )"),
      new SqlCoreFunction("TRY_CONVERT","Returns a value cast to the specified data type if the cast succeeds; otherwise, returns null.","TRY_CONVERT ( data_type [ ( length ) ], expression [, style ] )"),
      new SqlCoreFunction("TRY_PARSE","Returns the result of an expression, translated to the requested data type, or null if the cast fails in SQL Server. Use TRY_PARSE only for converting from string to date/time and number types.","TRY_PARSE ( string_value AS data_type [ USING culture ] )"),
      new SqlCoreFunction("AVG","Returns the average of the values in a group. Null values are ignored. ","AVG ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  "),
      new SqlCoreFunction("CHECKSUM_AGG","Returns the checksum of the values in a group. Null values are ignored. Can be followed by the OVER clause.","CHECKSUM_AGG ( [ ALL | DISTINCT ] expression )"),
      new SqlCoreFunction("COUNT","Returns the number of items in a group. COUNT works like the COUNT_BIG function. The only difference between the two functions is their return values. COUNT always returns an int data type value. COUNT_BIG always returns a bigint data type value. ","COUNT ( { [ [ ALL | DISTINCT ] expression ] | * } ) OVER ( [ partition_by_clause ] order_by_clause )"),
      new SqlCoreFunction("COUNT_BIG","Returns the number of items in a group. COUNT_BIG works like the COUNT function. The only difference between the two functions is their return values. COUNT_BIG always returns a bigint data type value. COUNT always returns an int data type value. ","COUNT_BIG ( { [ ALL | DISTINCT ] expression } | * ) OVER ( [ partition_by_clause ] order_by_clause )"),
      new SqlCoreFunction("GROUPING","Indicates whether a specified column expression in a GROUP BY list is aggregated or not. GROUPING returns 1 for aggregated or 0 for not aggregated in the result set. GROUPING can be used only in the SELECT &lt;select&gt; list, HAVING, and ORDER BY clauses when GROUP BY is specified.","GROUPING ( <column_expression> )"),
      new SqlCoreFunction("GROUPING_ID","Is a function that computes the level of grouping. GROUPING_ID can be used only in the SELECT &lt;select&gt; list, HAVING, or ORDER BY clauses when GROUP BY is specified.","GROUPING_ID ( <column_expression> [ ,...n ] )"),
      new SqlCoreFunction("MAX","Returns the maximum value in the expression. ","MAX ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )   "),
      new SqlCoreFunction("MIN","Returns the minimum value in the expression. May be followed by the OVER clause.","MIN ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )"),
      new SqlCoreFunction("STDEV","Returns the statistical standard deviation of all values in the specified expression. ","STDEV ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  "),
      new SqlCoreFunction("STDEVP","Returns the statistical standard deviation for the population for all values in the specified expression. ","STDEVP ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  "),
      new SqlCoreFunction("SUM","Returns the sum of all the values, or only the DISTINCT values, in the expression. SUM can be used with numeric columns only. Null values are ignored. ","SUM ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  "),
      new SqlCoreFunction("VAR","Returns the statistical variance of all values in the specified expression. May be followed by the OVER clause.","VAR ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  "),
      new SqlCoreFunction("VARP","Returns the statistical variance for the population for all values in the specified expression. ","VARP ( [ ALL | DISTINCT ] expression ) OVER ( [ partition_by_clause ] order_by_clause )  nh"),
      new SqlCoreFunction("CURRENT_TIMESTAMP","Returns the current database system timestamp as a datetime value without the database time zone offset. This value is derived from the operating system of the computer on which the instance of SQL Server is running.","CURRENT_TIMESTAMP"),
      new SqlCoreFunction("DATEADD","Returns a specified date with the specified number interval (signed integer) added to a specified datepart of that date.","DATEADD (datepart , number , date )"),
      new SqlCoreFunction("DATEDIFF","Returns the count (signed integer) of the specified datepart boundaries crossed between the specified startdate and enddate.","DATEDIFF ( datepart , startdate , enddate )"),
      new SqlCoreFunction("DATEDIFF_BIG","Returns the count (signed big integer) of the specified datepart boundaries crossed between the specified startdate and enddate.","DATEDIFF_BIG ( datepart , startdate , enddate )"),
      new SqlCoreFunction("DATEFROMPARTS","Returns a date value for the specified year, month, and day.","DATEFROMPARTS ( year, month, day )"),
      new SqlCoreFunction("DATENAME","Returns a character string that represents the specified datepart of the specified date&nbsp;","DATENAME ( datepart , date )"),
      new SqlCoreFunction("DATEPART","Returns an integer that represents the specified datepart of the specified date.","DATEPART ( datepart , date )"),
      new SqlCoreFunction("DATETIME2FROMPARTS","Returns a datetime2 value for the specified date and time and with the specified precision.","DATETIME2FROMPARTS ( year, month, day, hour, minute, seconds, fractions, precision )"),
      new SqlCoreFunction("DATETIMEFROMPARTS","Returns a datetime value for the specified date and time.","DATETIMEFROMPARTS ( year, month, day, hour, minute, seconds, milliseconds )"),
      new SqlCoreFunction("DATETIMEOFFSETFROMPARTS","Returns a datetimeoffset value for the specified date and time and with the specified offsets and precision.","DATETIMEOFFSETFROMPARTS ( year, month, day, hour, minute, seconds, fractions, hour_offset, minute_offset, precision )"),
      new SqlCoreFunction("DAY","Returns an integer representing the day (day of the month) of the specified date.","DAY ( date )"),
      new SqlCoreFunction("EOMONTH","Returns the last day of the month that contains the specified date, with an optional offset.","EOMONTH ( start_date [, month_to_add ] )"),
      new SqlCoreFunction("GETDATE","Returns the current database system timestamp as a datetime value without the database time zone offset. This value is derived from the operating system of the computer on which the instance of SQL Server is running.","GETDATE ( )"),
      new SqlCoreFunction("GETUTCDATE","Returns the current database system timestamp as a datetime value. The database time zone offset is not included. This value represents the current UTC time (Coordinated Universal Time). This value is derived from the operating system of the computer on which the instance of SQL Server is running.","GETUTCDATE()"),
      new SqlCoreFunction("ISDATE","Returns 1 if the expression is a valid date, time, or datetime value; otherwise, 0.","ISDATE ( expression )"),
      new SqlCoreFunction("MONTH","Returns an integer that represents the month of the specified date.","MONTH ( date )"),
      new SqlCoreFunction("SMALLDATETIMEFROMPARTS","Returns a smalldatetime value for the specified date and time.","SMALLDATETIMEFROMPARTS ( year, month, day, hour, minute )"),
      new SqlCoreFunction("SWITCHOFFSET","Returns a datetimeoffset value that is changed from the stored time zone offset to a specified new time zone offset.","SWITCHOFFSET ( DATETIMEOFFSET, time_zone ) "),
      new SqlCoreFunction("SYSDATETIME","Returns a datetime2(7) value that contains the date and time of the computer on which the instance of SQL Server is running.","SYSDATETIME ( )"),
      new SqlCoreFunction("SYSDATETIMEOFFSET","Returns a datetimeoffset(7) value that contains the date and time of the computer on which the instance of SQL Server is running. The time zone offset is included.","SYSDATETIMEOFFSET ( )"),
      new SqlCoreFunction("SYSUTCDATETIME","Returns a datetime2 value that contains the date and time of the computer on which the instance of SQL Server is running. The date and time is returned as UTC&nbsp;time&nbsp;(Coordinated Universal Time). The fractional second precision specification has a range from 1 to 7 digits. The default precision is 7 digits.","SYSUTCDATETIME ( )"),
      new SqlCoreFunction("TIMEFROMPARTS","Returns a time value for the specified time and with the specified precision.","TIMEFROMPARTS ( hour, minute, seconds, fractions, precision )"),
      new SqlCoreFunction("TODATETIMEOFFSET","Returns a datetimeoffset value that is translated from a datetime2 expression. ","TODATETIMEOFFSET ( expression , time_zone )"),
      new SqlCoreFunction("YEAR","Returns an integer that represents the year of the specified date.","YEAR ( date )"),
      new SqlCoreFunction("ISNULL","Replaces NULL with the specified replacement value.","ISNULL ( check_expression , replacement_value )"),
      new SqlCoreFunction("COALESCE","Evaluates the arguments in order and returns the current value of the first expression that initially does not evaluate to NULL.","COALESCE ( expression [ ,...n ] )"),
      new SqlCoreFunction("NULLIF","Returns a null value if the two specified expressions are equal.","NULLIF ( expression , expression )"),
      new SqlCoreFunction("ISNUMERIC","Determines whether an expression is a valid numeric type.","ISNUMERIC ( expression )"),
      new SqlCoreFunction("NEWID","Creates a unique value of type uniqueidentifier.","NEWID ( )"),
      new SqlCoreFunction("PARSENAME","Returns the specified part of an object name. The parts of an object that can be retrieved are the object name, owner name, database name, and server name.","PARSENAME ( 'object_name' , object_piece ) "),
      new SqlCoreFunction("RANK","Returns the rank of each row within the partition of a result set. The rank of a row is one plus the number of ranks that come before the row in question.","RANK ( ) OVER ( [ partition_by_clause ] order_by_clause )"),
      new SqlCoreFunction("DENSE_RANK","Returns the rank of rows within the partition of a result set, without any gaps in the ranking. The rank of a row is one plus the number of distinct ranks that come before the row in question.","DENSE_RANK ( ) OVER ( [ <partition_by_clause> ] < order_by_clause > )"),
      new SqlCoreFunction("NTILE","Distributes the rows in an ordered partition into a specified number of groups. The groups are numbered, starting at one. For each row, NTILE returns the number of the group to which the row belongs.","NTILE (integer_expression) OVER ( [ <partition_by_clause> ] < order_by_clause > )"),
      new SqlCoreFunction("ROW_NUMBER","Returns the sequential number of a row within a partition of a result set, starting at 1 for the first row in each partition.","ROW_NUMBER ( ) OVER ( [ PARTITION BY value_expression , ... [ n ] ] order_by_clause )")
    };

    private class SqlCoreFunction
    {
      public string Name { get; set; }
      public string Description { get; set; }
      public string Usage { get; set; }

      public SqlCoreFunction(string name, string description, string usage)
      {
        this.Name = name;
        this.Description = description;
        this.Usage = usage;
      }
    }

  }
}
