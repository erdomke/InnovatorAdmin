using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class AmlTypeDefinition
  {
    private IEnumerable<string> _values;

    public AmlDataType Type { get; private set; }
    public string Source { get; private set; }
    public IEnumerable<string> Values { get { return _values ?? Enumerable.Empty<string>(); } }

    public static AmlTypeDefinition FromConstants(params string[] constants)
    {
      return new AmlTypeDefinition()
      {
        Type = AmlDataType.Enum,
        _values = constants
      };
    }

    public static AmlTypeDefinition FromDefinition(AmlDataType dataType, params string[] values)
    {
      var typeDefn = new AmlTypeDefinition()
      {
        Type = dataType
      };

      if (values?.Length > 0)
      {
        switch (typeDefn.Type)
        {
          case AmlDataType.Enum:
            typeDefn._values = values;
            break;
          case AmlDataType.Inherit:
          case AmlDataType.Item:
          case AmlDataType.ItemName:
          case AmlDataType.List:
          case AmlDataType.MultiValueList:
          case AmlDataType.OrderBy:
          case AmlDataType.SelectList:
          case AmlDataType.WhereClause:
            typeDefn.Source = values[0];
            break;
        }
      }

      return typeDefn;
    }

    public static List<AmlTypeDefinition> FromType(string types)
    {
      var result = new List<AmlTypeDefinition>();
      if (string.IsNullOrEmpty(types))
        return result;

      var typeList = Regex.Match(types, @"^(?<type>\w+(\[[^]]+\])?)(\|(?<type>\w+(\[[^]]+\])?))*$")
        .Groups["type"].Captures.OfType<Capture>().Select(c => c.Value);

      foreach (var type in typeList)
      {
        var dataType = type;
        var values = default(string[]);
        if (type.EndsWith("]") && type.IndexOf('[') > 0)
        {
          var parts = type.Substring(0, type.Length - 1).Split('[');
          dataType = parts[0];
          values = parts[1].Split('|');
        }

        var amlDataType = AmlDataType.Unknown;
        if (!Enum.TryParse(dataType, true, out amlDataType))
        {
          switch (dataType.ToUpperInvariant())
          {
            case "COLOR":
            case "COLOR LIST":
            case "FORMATTED TEXT":
            case "IMAGE":
            case "MD5":
            case "ML_STRING":
            case "SEQUENCE":
            case "TEXT":
              amlDataType = AmlDataType.String;
              break;
            case "DECIMAL":
              amlDataType = AmlDataType.Float;
              break;
            case "FILTER LIST":
              amlDataType = AmlDataType.List;
              break;
            case "GLOBAL_VERSION":
            case "UBIGINT":
              amlDataType = AmlDataType.Integer;
              break;  
            case "ID":
              amlDataType = AmlDataType.Item;
              break;
            case "MV_LIST":
              amlDataType = AmlDataType.MultiValueList;
              break;
            default:
              amlDataType = AmlDataType.Unknown;
              break;
          }
        }

        result.Add(FromDefinition(amlDataType, values));
      }

      return result;
    }
  }
}
