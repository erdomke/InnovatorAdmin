using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Pipes.Data.ParserMapping
{
  public class XmlDataSetParser : IDataSetParser
  {
    [DisplayName("Level"), Description("How many levels into the XML tree to pivot the data.  0 = root.")]
    public int Level { get; set; }
    [DisplayName("Tag Name"), Description("Tag name to pivot on if multiple are available at a given level.")]
    public string TagName { get; set; }

    public XmlDataSetParser()
    {
      this.Level = -1;
    }

    public bool TryGetDataSet(System.IO.Stream stream, out DataSet ds)
	  {
		  ds = new DataSet();
		  try {
			  var settings = new XmlReaderSettings();
			  settings.IgnoreWhitespace = true;
			  settings.ProhibitDtd = false;
			  settings.MaxCharactersFromEntities = 10000000L;
			  settings.XmlResolver = null;

			  using (var reader = XmlReader.Create(stream, settings)) {
				  var xml = XDocument.Load(reader);

				  xml.Root.AddAnnotation(1);
				  var calcLevel = AddIndices(xml.Root, 0);
				  if (this.Level < 0) {
					  this.Level = calcLevel;
				  }

				  IEnumerable<XElement> elems = null;
				  switch (this.Level) {
					  case 0:
              throw new ArgumentOutOfRangeException("Level cannot be zero.");
					  default:
						  elems = xml.Root.Elements();
						  for (var i = 2; i <= this.Level; i++) {
							  if (i == this.Level) {
                  elems = elems.SelectMany(n => GetElements(n.Elements(), FilterRepeats));
							  } else {
								  elems = elems.SelectMany(n => n.Elements());
							  }
						  }

						  break;
				  }

				  var dt = ds.Tables.Add("Data");
				  DataRow row = null;
				  IEnumerable<XElement> repeats = null;
				  IEnumerable<XText> textNodes = null;
				  ColumnStore colStore = new ColumnStore(dt);
				  string nodePath = null;

				  foreach (var elem in elems) {
					  if (elem.Parent == null) {
						  repeats = Enumerable.Empty<XElement>();
					  } else {
              repeats = elem.Ancestors().Reverse();
					  }

					  row = dt.NewRow();

					  foreach (var repeat in repeats) {
						  nodePath = GetPath(repeat);
						  foreach (var attr in repeat.Attributes()) {
							  row[colStore.GetTableColumn(nodePath + "/@" + attr.Name.ToString(), attr.Name.LocalName)] = attr.Value;
						  }
						  textNodes = repeat.Nodes().OfType<XText>();
						  if (textNodes.Any()) {
							  row[colStore.GetTableColumn(nodePath, repeat.Name.LocalName)] = (from t in textNodes 
                                                                                 select t.Value).Aggregate((p, c) => p + c);
						  }
					  }
            int index = 0;
            nodePath = GetPath(elem.Parent);
            foreach (var child in GetElements(elem.Parent.Elements(), (c, n) => !FilterRepeats(c, n)))
            {
              index = (int)child.Annotation(typeof(int));
              BuildRow(colStore, row, child, nodePath + "/" + child.Name.ToString() + (index > 1 ? string.Format("[{0}]", index) : ""));
            }

					  nodePath = GetPath(elem);
					  BuildRow(colStore, row, elem, nodePath);
					  dt.Rows.Add(row);
				  }
			  }

			  return true;
		  } catch (XmlException) {
			  return false;
		  }
	  }

    private int AddIndices(XElement parent, int level)
    {
      NameCounter counter = new NameCounter();
      int count = 0;
      int maxLevel = 0;
      foreach (var child in parent.Elements())
      {
        count = counter.GetCount(child.Name.LocalName);
        child.AddAnnotation(count);
        if (count > 1)
        {
          maxLevel = Math.Max(maxLevel, level + 1);
        }
        maxLevel = Math.Max(maxLevel, AddIndices(child, level + 1));
      }
      return maxLevel;
    }
    private void BuildRow(ColumnStore columns, DataRow row, XElement parent, string parentPath)
	  {
		  foreach (var attr in parent.Attributes()) {
			  row[columns.GetTableColumn(parentPath + "/@" + attr.Name.ToString(), attr.Name.LocalName)] = attr.Value;
		  }
		  var textNodes = parent.Nodes().OfType<XText>();
		  if (textNodes.Any()) {
			  row[columns.GetTableColumn(parentPath, parent.Name.LocalName)] = (from t in textNodes 
                                                                          select t.Value).Aggregate((p, c) => p + c);
		  }

		  int index = 0;
		  foreach (var child in parent.Elements()) {
			  index = (int)child.Annotation(typeof(int));
			  BuildRow(columns, row, child, parentPath + "/" + child.Name.ToString() + (index > 1 ? string.Format("[{0}]", index) : ""));
		  }
	  }
    private bool FilterRepeats(int count, string name)
    {
      return count > 1 && (string.IsNullOrEmpty(this.TagName) || name == this.TagName);
    }
    private IEnumerable<XElement> GetElements(IEnumerable<XElement> elems, Func<int, string, bool> filter)
    {
      var result = (from e in (from e in elems
                              group e by e.Name into Group
                              select Group)
                    where filter.Invoke(e.Count(), e.Key.LocalName)
                    select e).SelectMany(e => e);
      return result;
    }
    private string GetPath(XElement elem)
    {
      return (from a in elem.AncestorsAndSelf().Reverse()
              select a.Name.ToString()).Aggregate((p, c) => p + "/" + c);
    }

    private class NameCounter
    {

      private Dictionary<string, int> _counts = new Dictionary<string, int>();
      public int GetCount(string name)
      {
        int count = 0;
        if (!_counts.TryGetValue(name, out count))
        {
          count = 0;
        }
        count += 1;
        _counts[name] = count;
        return count;
      }
      public void Clear()
      {
        _counts.Clear();
      }
    }

    private class ColumnStore
    {
      private DataTable _table;
      private Dictionary<string, DataColumn> _columnMappings = new Dictionary<string, DataColumn>();

      private NameCounter _counter = new NameCounter();
      public ColumnStore(DataTable table)
      {
        _table = table;
      }

      public DataColumn GetTableColumn(string fullPath, string desiredName)
      {
        DataColumn result = null;
        if (!_columnMappings.TryGetValue(fullPath, out result))
        {
          var count = _counter.GetCount(desiredName);
          if (count > 1)
            desiredName += string.Format("[{0}]", count);
          result = _table.Columns.Add(fullPath, typeof(string));
          result.Caption = desiredName;
          _columnMappings.Add(fullPath, result);
        }
        return result;
      }
    }
  }
}
