using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Innovator.Client;

namespace InnovatorAdmin
{
  public static class Extensions
  {
    public const string AmlTable_TypeName = "__type";
    public const string AmlTable_TypeId = "itemtype";

    /// <summary>
    /// Determines if a string is an Aras-styled GUID
    /// </summary>
    public static bool IsGuid(this string value)
    {
      if (value == null || value.Length != 32) return false;
      for (int i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
          case 'F':
            // Do Nothing
            break;
          default:
            return false;
        }
      }
      return true;
    }

    public static void WriteTo(this IEnumerable<InstallItem> exports, TextWriter writer)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";

      // Render the output
      using (var xml = XmlTextWriter.Create(writer, settings))
      {
        xml.WriteStartElement("AML");
        foreach (var item in exports.Where(i => i.Script != null))
        {
          item.Script.WriteTo(xml);
        }
        xml.WriteEndElement();
      }
    }
    public static string WriteToString(this IEnumerable<InstallItem> exports)
    {
      using (var writer = new StringWriter())
      {
        WriteTo(exports, writer);
        return writer.ToString();
      }
    }

    private class TypeProperties : IEnumerable<KeyValuePair<string, HashSet<string>>>
    {
      private Dictionary<string, HashSet<string>> _types = new Dictionary<string, HashSet<string>>();

      public int Count { get { return _types.Count; } }

      public void Add(string type, string property)
      {
        HashSet<string> props;
        if (!_types.TryGetValue(type, out props))
        {
          props = new HashSet<string>();
          _types.Add(type, props);
        }
        props.Add(property);
      }

      public IEnumerator<KeyValuePair<string, HashSet<string>>> GetEnumerator()
      {
        return _types.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public bool AllTypesTheSame()
      {
        if (_types.Count < 2) return true;
        var lists = _types.Values.ToArray();
        var counts = lists[0].ToDictionary(i => i, i => 1);
        int count;

        foreach (var list in lists.Skip(1))
        {
          foreach (var prop in list)
          {
            if (!counts.TryGetValue(prop, out count))
              return false;
            counts[prop] = count + 1;
          }
        }

        return counts.Values.All(v => v == _types.Count);
      }
    }

    public static bool IsUiVisible(this DataColumn column)
    {
      return !column.ExtendedProperties.ContainsKey("visible") || (bool)column.ExtendedProperties["visible"];
    }
    private static void IsUiVisible(this DataColumn column, bool value)
    {
      if (column == null) return;
      column.ExtendedProperties["visible"] = value;
    }
    public static Property PropMetadata(this DataColumn column)
    {
      if (column.ExtendedProperties.ContainsKey("property"))
        return (Property)column.ExtendedProperties["property"];
      return null;
    }
    private static void PropMetadata(this DataColumn column, Property value)
    {
      column.ExtendedProperties["property"] = value;
    }
    //public static int ColumnWidth(this DataColumn column)
    //{
    //  if (column.ExtendedProperties.ContainsKey("column_width"))
    //    return (int)column.ExtendedProperties["column_width"];
    //  return 100;
    //}
    //private static void ColumnWidth(this DataColumn column, int value)
    //{
    //  column.ExtendedProperties["column_width"] = value;
    //}
    //public static int SortOrder(this DataColumn column)
    //{
    //  if (column.ExtendedProperties.ContainsKey("sort_order"))
    //    return (int)column.ExtendedProperties["sort_order"];
    //  return 999999;
    //}
    //public static void SortOrder(this DataColumn column, int value)
    //{
    //  column.ExtendedProperties["sort_order"] = value;
    //}

    public static IEnumerable<AmlSelectColumn> SelectColumns(string select)
    {
      var result = new AmlSelectColumn();
      if (string.IsNullOrEmpty(select))
        return result.Children;

      var path = new Stack<AmlSelectColumn>();
      path.Push(result);
      var start = 0;
      for (var i = 0; i < select.Length; i++)
      {
        switch (select[i])
        {
          case ',':
            if (i - start > 0)
              path.Peek().Add(new AmlSelectColumn() { Name = select.Substring(start, i - start).Trim() });
            start = i + 1;
            break;
          case '(':
            if (i - start > 0)
            {
              var curr = new AmlSelectColumn() { Name = select.Substring(start, i - start).Trim() };
              path.Peek().Add(curr);
              path.Push(curr);
            }
            start = i + 1;
            break;
          case ')':
            if (i - start > 0)
              path.Peek().Add(new AmlSelectColumn() { Name = select.Substring(start, i - start).Trim() });
            path.Pop();
            start = i + 1;
            break;
        }
      }

      if (start < select.Length)
      {
        result.Add(new AmlSelectColumn() { Name = select.Substring(0, select.Length - start).Trim() });
      }
      return result.Children;
    }

    public static DataSet GetItemTable(IReadOnlyResult res, ArasMetadataProvider metadata, string select)
    {
      var ds = new DataSet();
      string mainType = null;
      string mainId = null;
      var selectedCols = SelectColumns(select);

      List<IReadOnlyItem> items;
      try
      {
        items = res.Items().ToList();
        if (items.Count == 1)
        {
          mainType = items[0].Type().Value;
          mainId = items[0].Id();
          items.AddRange(items[0].Relationships());
        }

        if (items.Count < 1)
          return ds;
      }
      catch (ServerException)
      {
        return ds;
      }

      var types = new TypeProperties();
      string type;
      bool idAdded;
      bool keyedNameAdded;

      foreach (var i in items)
      {
        type = (i.Type().Exists || i.Property("id").Type().Exists)
          ? i.Type().AsString(i.Property("id").Type().AsString(""))
          : string.Empty;

        if (!string.IsNullOrEmpty(type)) types.Add(type, AmlTable_TypeName);
        if (i.TypeId().Exists || i.Property("itemtype").Exists) types.Add(type, AmlTable_TypeId);

        idAdded = false;
        keyedNameAdded = false;
        foreach (var elem in i.Elements().OfType<IReadOnlyProperty>())
        {
          if (elem.Name != "config_id")
          {
            if (elem.Type().Exists) types.Add(type, elem.Name + "/type");
            if (elem.KeyedName().Exists) types.Add(type, elem.Name + "/keyed_name");
          }
          types.Add(type, elem.Name);
          idAdded = idAdded || elem.Name == "id";
          keyedNameAdded = keyedNameAdded || elem.Name == "keyed_name";
        }
        if (!string.IsNullOrEmpty(i.Id()) && !idAdded) types.Add(type, "id");
        if (i.Property("id").KeyedName().Exists && !keyedNameAdded) types.Add(type, "keyed_name");
      }


      ItemType itemType;
      Property pMeta;
      DataRow row;
      string propName;
      string propAddendum;
      int split;
      DataColumn newColumn;

      var typesEnum = types.Count > 1 && types.AllTypesTheSame()
        ? Enumerable.Repeat(new KeyValuePair<string, HashSet<string>>(string.Empty, types.First().Value), 1)
        : types;

      foreach (var kvp in typesEnum)
      {
        var result = new DataTable(kvp.Key);
        try
        {
          result.BeginLoadData();

          if (string.IsNullOrEmpty(kvp.Key)
              || metadata == null
              || !metadata.ItemTypeByName(kvp.Key, out itemType))
          {
            itemType = null;
          }
          else
          {
            result.RowChanging += (s, e) =>
            {
              if (e.Action == DataRowAction.Add)
              {
                var newId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                e.Row.Table.Columns["id"].DefaultValue = newId;
              }
            };
          }

          var allProps = new HashSet<string>(kvp.Value);
          if (itemType != null)
            allProps.UnionWith(metadata.GetProperties(itemType).Wait().Select(p => p.Name));

          foreach (var prop in allProps)
          {
            if (prop != AmlTable_TypeName
              && prop != AmlTable_TypeId
              && itemType != null)
            {
              result.TableName = itemType.TabLabel ?? (itemType.Label ?? itemType.Name);

              split = prop.IndexOf('/');
              propAddendum = string.Empty;
              propName = prop;
              if (split > 0)
              {
                propName = prop.Substring(0, split);
                propAddendum = prop.Substring(split);
              }

              try
              {
                pMeta = metadata.GetProperty(itemType, propName).Wait();

                switch (pMeta.Type)
                {
                  case PropertyType.boolean:
                    newColumn = new DataColumn(prop, typeof(bool));
                    if (pMeta.IsRequired && pMeta.DefaultValue == null)
                      newColumn.DefaultValue = false;
                    break;
                  case PropertyType.date:
                    newColumn = new DataColumn(prop, typeof(DateTime));
                    break;
                  case PropertyType.number:
                    if (string.Equals(pMeta.TypeName, "integer", StringComparison.OrdinalIgnoreCase))
                    {
                      newColumn = new DataColumn(prop, typeof(int));
                    }
                    else
                    {
                      newColumn = new DataColumn(prop, typeof(double));
                    }
                    break;
                  default:
                    newColumn = new DataColumn(prop, typeof(string));
                    if (pMeta.StoredLength > 0) newColumn.MaxLength = pMeta.StoredLength;
                    break;
                }
                newColumn.Caption = (pMeta.Label ?? pMeta.Name) + propAddendum;
                if (pMeta.DefaultValue != null) newColumn.DefaultValue = pMeta.DefaultValue;
                if (kvp.Key != mainType && !string.IsNullOrEmpty(mainId) && pMeta.Name == "source_id")
                  newColumn.DefaultValue = mainId;
                newColumn.ReadOnly = pMeta.ReadOnly;
                newColumn.AllowDBNull = selectedCols.Any() || !pMeta.IsRequired;
                // Ignore constraints on the following columns
                switch (pMeta.Name)
                {
                  case "config_id":
                  case "created_by_id":
                  case "created_on":
                  case "permission_id":
                    newColumn.AllowDBNull = true;
                    break;
                }
                if (selectedCols.Any())
                {
                  newColumn.IsUiVisible(selectedCols.Any(c => string.Equals(c.Name, propName)));
                }
                else
                {
                  newColumn.IsUiVisible(!string.IsNullOrEmpty(mainType) && itemType.Name != mainType
                    ? (pMeta.Visibility & PropertyVisibility.RelationshipGrid) > 0
                    : (pMeta.Visibility & PropertyVisibility.MainGrid) > 0);
                }
                newColumn.PropMetadata(pMeta);
              }
              catch (KeyNotFoundException)
              {
                newColumn = new DataColumn(prop, typeof(string));
                newColumn.IsUiVisible(string.IsNullOrEmpty(kvp.Key) || metadata == null);
              }
            }
            else
            {
              newColumn = new DataColumn(prop, typeof(string));
              newColumn.IsUiVisible(string.IsNullOrEmpty(kvp.Key) || metadata == null);
              if (prop == AmlTable_TypeName && !string.IsNullOrEmpty(kvp.Key))
                newColumn.DefaultValue = kvp.Key;
            }
            result.Columns.Add(newColumn);
          }

          foreach (var item in items
            .Where(i => string.IsNullOrEmpty(kvp.Key)
              || i.Type().AsString(i.Property("id").Type().Value) == kvp.Key))
          {
            row = result.NewRow();
            row.BeginEdit();
            foreach (var prop in kvp.Value)
            {
              switch (prop)
              {
                case "id":
                  row[prop] = item.Id();
                  break;
                case "keyed_name":
                  row[prop] = item.KeyedName().AsString(item.Property("id").KeyedName().Value);
                  break;
                case AmlTable_TypeName:
                  row[prop] = item.Type().AsString(item.Property("id").Type().Value);
                  break;
                case AmlTable_TypeId:
                  row[prop] = item.TypeId().AsString(item.Property("itemtype").Value);
                  break;
                default:
                  split = prop.IndexOf('/');
                  if (split > 0)
                  {
                    row[prop] = item.Property(prop.Substring(0, split)).Attribute(prop.Substring(split + 1)).Value;
                  }
                  else if (item.Property(prop).HasValue())
                  {
                    newColumn = result.Columns[prop];
                    if (newColumn.DataType == typeof(bool))
                    {
                      row[prop] = item.Property(prop).AsBoolean(false);
                    }
                    else if (newColumn.DataType == typeof(DateTime))
                    {
                      row[prop] = item.Property(prop).AsDateTime(DateTime.MinValue);
                    }
                    else if (newColumn.DataType == typeof(int))
                    {
                      row[prop] = item.Property(prop).AsInt(int.MinValue);
                    }
                    else if (newColumn.DataType == typeof(double))
                    {
                      row[prop] = item.Property(prop).AsDouble(double.MinValue);
                    }
                    else
                    {
                      row[prop] = item.Property(prop).Value;
                    }
                  }
                  else
                  {
                    row[prop] = DBNull.Value;
                  }
                  break;
              }
            }
            row.EndEdit();
            row.AcceptChanges();
            result.Rows.Add(row);
          }


          if (result.Rows.Count > 0
            && result.Columns.OfType<DataColumn>().Where(c => c.ColumnName == "source_id" || c.ColumnName == "related_id").Count() == 2)
          {
            var firstRelated = result.Rows[0]["related_id"];
            var firstSource = result.Rows[0]["source_id"];
            if (result.Rows.Count > 1
              && result.AsEnumerable().All(r => r["related_id"] == firstRelated)
              && !result.AsEnumerable().All(r => r["source_id"] == firstSource))
            {
              result.Columns["source_id/keyed_name"].IsUiVisible(true);
            }
            else
            {
              result.Columns["related_id/keyed_name"].IsUiVisible(true);
            }
          }
        }
        finally
        {
          try
          {
            result.EndLoadData();
          }
          catch (ConstraintException) { }
          result.AcceptChanges();
        }

        ds.Tables.Add(result);
      }

      return ds;
    }

    /// <summary>
    /// Groups a list of items and pages the results by batch size
    /// </summary>
    public static IEnumerable<IGrouping<TKey, T>> PagedGroupBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelector, int batchSize)
    {
      var elems = new Dictionary<TKey, Grouping<TKey, T>>();
      TKey key;
      Grouping<TKey, T> list;

      foreach (var item in items)
      {
        key = keySelector(item);
        if (!elems.TryGetValue(key, out list) || list == null)
        {
          list = new Grouping<TKey, T>();
          list.Key = key;
          elems[key] = list;
        }
        list.Values.Add(item);
        if (list.Values.Count >= batchSize)
        {
          yield return list;
          elems[key] = null;
        }
      }

      foreach (var kvp in elems.Where(k => k.Value != null))
      {
        yield return kvp.Value;
      }
    }

    private class Grouping<TKey, T> : IGrouping<TKey, T>
    {
      private List<T> _values = new List<T>();

      public TKey Key { get; internal set; }
      internal IList<T> Values { get { return _values; } }

      public IEnumerator<T> GetEnumerator()
      {
        return _values.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }
    }

    /// <summary>
    /// Whether the enumerable has more than one item or more than one item matching the criteria
    /// if criteria is provided
    /// </summary>
    public static bool HasMultiple<T>(this IEnumerable<T> items, Func<T, bool> predicate = null)
    {
      var count = 0;
      foreach (var item in items)
      {
        if (predicate == null || predicate(item))
        {
          count++;
          if (count > 1) return true;
        }
      }
      return false;
    }


    public static IEnumerable<XElement> Parents(this XElement x)
    {
      var curr = x.Parent;
      while (curr != null)
      {
        yield return curr;
        curr = curr.Parent;
      }
    }
    public static IEnumerable<XElement> ParentsAndSelf(this XElement x)
    {
      return Enumerable.Repeat(x, 1).Concat(Parents(x));
    }
    public static XElement ReplaceWithElement(this XElement x, XElement replacement)
    {
      x.ReplaceWith(replacement);
      return replacement;
    }

    public static void MergeSorted<T, TKey>(this IList<T> start, IList<T> dest,
      Func<T, TKey> keyGetter, Action<int, T, T> callback) where TKey : IComparable
    {
      var startPtr = 0;
      var destPtr = 0;
      int status;

      while (startPtr < start.Count && destPtr < dest.Count)
      {
        status = keyGetter(start[startPtr]).CompareTo(keyGetter(dest[destPtr]));
        switch (status)
        {
          case -1:
            callback(status, start[startPtr], default(T));
            startPtr++;
            break;
          case 1:
            callback(status, default(T), dest[destPtr]);
            destPtr++;
            break;
          default:
            callback(0, start[startPtr], dest[destPtr]);
            startPtr++;
            destPtr++;
            break;
        }
      }
      while (startPtr < start.Count)
      {
        callback(-1, start[startPtr], default(T));
        startPtr++;
      }
      while (destPtr < dest.Count)
      {
        callback(1, default(T), dest[destPtr]);
        destPtr++;
      }
    }
  }
}
