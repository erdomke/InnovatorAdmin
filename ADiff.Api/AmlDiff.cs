using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ADiff
{
  public class AmlDiff
  {
    private enum KeyMode
    {
      Undefined,
      ItemId,
      ElementName
    }

    private class ChangeElement
    {
      public XElement Base { get; set; }
      public XElement Left { get; set; }
      public XElement Right { get; set; }
      public Change LeftChange { get; set; }
      public Change RightChange { get; set; }

      public ChangeElement()
      {
        this.LeftChange = Change.NoChange;
        this.RightChange = Change.NoChange;
      }
    }

    private enum ThreeWayStatus
    {
      LeftDeleteRightDelete = 0x00,
      LeftEqualRightDelete = 0x10,
      LeftAddRightDelete = 0x20,
      LeftDeleteRightEqual = 0x01,
      LeftEqualRightEqual = 0x11,
      LeftAddRightEqual = 0x21,
      LeftDeleteRightAdd = 0x02,
      LeftEqualRightAdd = 0x12,
      LeftAddRightAdd = 0x22
    }

    public static bool IsDifferent(string baseXml, string compareXml)
    {
      string baseFormatted;
      string compareFormatted;
      var diffs = TwoWayDiff(baseXml, compareXml, out baseFormatted, out compareFormatted);
      return diffs.Any(d => d.IsDifferent || d.Base < 0 || d.Compare < 0);
    }
    public static IList<ListCompare> TwoWayDiff(string baseXml, string compareXml, out string baseFormatted, out string compareFormatted)
    {
      var baseElem = GetFirstItemElem(XElement.Parse(baseXml));
      baseElem = baseElem.Parent ?? baseElem;
      var compareElem = GetFirstItemElem(XElement.Parse(compareXml));
      compareElem = compareElem.Parent ?? compareElem;
      var settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = "  ";
      settings.OmitXmlDeclaration = true;
      settings.NewLineOnAttributes = false;

      var baseWriter = new ElementWriter(settings);
      baseWriter.WriteStartElement(baseElem);

      var compareWriter = new ElementWriter(settings);
      compareWriter.WriteStartElement(compareElem);

      var results = new List<ListCompare>();
      if (!baseElem.Elements().Any() || !baseElem.Elements().Any())
      {
        var isDifferent = baseElem.Elements().Any()
          || baseElem.Elements().Any()
          || GetKeys(Enumerable.Repeat(baseElem, 1)).Single().Item1 != GetKeys(Enumerable.Repeat(compareElem, 1)).Single().Item1;
        results.Add(new ListCompare(baseWriter.CurrentLine + 1, compareWriter.CurrentLine + 1) { IsDifferent = isDifferent });
      }
      else
      {
        results.Add(new ListCompare(baseWriter.CurrentLine + 1, compareWriter.CurrentLine + 1) { IsDifferent = false });
        TwoWayDiff(baseElem, compareElem, baseWriter, compareWriter, results);
      }
      
      baseWriter.WriteEndElement();
      baseFormatted = baseWriter.ToString();
      compareWriter.WriteEndElement();
      compareFormatted = compareWriter.ToString();
      return results;
    }

    private static string GetRelatedId(XElement item)
    {
      var relatedId = item.Element("related_id");
      if (relatedId == null) return null;
      var relatedItem = relatedId.Element("Item");
      if (relatedItem == null) return relatedId.Value;
      var idAttr = relatedItem.Attribute("id");
      if (idAttr == null) return null;
      return idAttr.Value;
    }

    private static void TwoWayDiff(XElement baseElem, XElement compareElem, ElementWriter bWriter, ElementWriter cWriter, List<ListCompare> results)
    {
      var baseList = GetKeys(baseElem.Elements());
      var compareList = GetKeys(compareElem.Elements());

      var compares = ListCompare.Create(baseList, compareList, v => v.Item1);
      // Deal with the relationships of versionable items
      if (baseElem.Name.LocalName == "Relationships" && !compares.Any(c => c.Base >= 0 && c.Compare >= 0))
      {
        var baseRelateds = baseElem.Elements().Select(e => Tuple.Create(GetRelatedId(e), e)).ToList();
        var compareRelateds = compareElem.Elements().Select(e => Tuple.Create(GetRelatedId(e), e)).ToList();
        if (!baseRelateds.Any(t => string.IsNullOrEmpty(t.Item1)) 
          && !compareRelateds.Any(t => string.IsNullOrEmpty(t.Item1))
          && baseRelateds.Select(t => t.Item1).Distinct().Count() == baseRelateds.Count
          && compareRelateds.Select(t => t.Item1).Distinct().Count() == compareRelateds.Count)
        {
          compares = ListCompare.Create(baseRelateds, compareRelateds, v => v.Item1);
        }
      }

      int start;
      foreach (var tuple in compares)
      {
        if (tuple.Base < 0)
        {
          start = cWriter.CurrentLine + 1;
          cWriter.WriteElement(compareList[tuple.Compare].Item2);
          results.AddRange(Enumerable.Range(start, cWriter.CurrentLine - start + 1).Select(i => new ListCompare(-1, i)));
        }
        else if (tuple.Compare < 0)
        {
          start = bWriter.CurrentLine + 1;
          bWriter.WriteElement(baseList[tuple.Base].Item2);
          results.AddRange(Enumerable.Range(start, bWriter.CurrentLine - start + 1).Select(i => new ListCompare(i, -1)));
        }
        else if (baseList[tuple.Base].Item2.Elements().Any() &&
          compareList[tuple.Compare].Item2.Elements().Any())
        {
          results.Add(new ListCompare(bWriter.CurrentLine + 1, cWriter.CurrentLine + 1) { IsDifferent = false });
          bWriter.WriteStartElement(baseList[tuple.Base].Item2);
          cWriter.WriteStartElement(compareList[tuple.Compare].Item2);
          TwoWayDiff(baseList[tuple.Base].Item2, compareList[tuple.Compare].Item2, bWriter, cWriter, results);
          bWriter.WriteEndElement();
          cWriter.WriteEndElement();
        }
        else if (baseList[tuple.Base].Item2.Elements().Any() || compareList[tuple.Compare].Item2.Elements().Any() || baseList[tuple.Base].Item2.Value != compareList[tuple.Compare].Item2.Value)
        {
          results.Add(new ListCompare(bWriter.CurrentLine + 1, cWriter.CurrentLine + 1) { IsDifferent = true });
          for (int i = results.Last().Base; i < bWriter.CurrentLine; i++)
            results.Add(new ListCompare(i, -1));
          for (int i = results.Last().Compare; i < cWriter.CurrentLine; i++)
            results.Add(new ListCompare(-1, i));
          bWriter.WriteElement(baseList[tuple.Base].Item2);
          cWriter.WriteElement(compareList[tuple.Compare].Item2);
        }
        else
        {
          results.Add(new ListCompare(bWriter.CurrentLine + 1, cWriter.CurrentLine + 1));
          bWriter.WriteElement(baseList[tuple.Base].Item2);
          cWriter.WriteElement(compareList[tuple.Compare].Item2);
        }
      }
    }

    private static XElement GetFirstItemElem(XElement search)
    {
      var elementSearch = Enumerable.Repeat(search, 1);
      var item = elementSearch.FirstOrDefault(e => e.Name.LocalName == "Item");
      while (item == null && elementSearch.Any())
      {
        elementSearch = elementSearch.SelectMany(e => e.Elements());
        item = elementSearch.FirstOrDefault(e => e.Name.LocalName == "Item");
      }
      return item;
    }

    public void ThreeWayDiff(XElement baseFile, XElement left, XElement right)
    {
      var baseList = GetKeys(baseFile.Elements());
      var leftList = GetKeys(left.Elements());
      var rightList = GetKeys(right.Elements());

      var basePtr = 0;
      var leftPtr = 0;
      var rightPtr = 0;

      var result = new List<ChangeElement>();
      ChangeElement changeElem;
      ThreeWayStatus status;

      while (basePtr <= baseList.Count 
        && leftPtr <= leftList.Count 
        && rightPtr <= rightList.Count)
      {
        changeElem = new ChangeElement();
        status = (ThreeWayStatus)((baseList[basePtr].Item1.CompareTo(leftList[leftPtr].Item1) + 1) << 4
         + (baseList[basePtr].Item1.CompareTo(rightList[rightPtr].Item1) + 1));

        switch (status)
        {
          case ThreeWayStatus.LeftAddRightAdd:
            if (leftList[leftPtr].Item1 == rightList[rightPtr].Item1)
            {
              changeElem.Left = leftList[leftPtr].Item2;
              changeElem.LeftChange = Change.Add;
              changeElem.Right = rightList[rightPtr].Item2;
              changeElem.RightChange = Change.Add;
            }
            else 
            {
              changeElem.Left = leftList[leftPtr].Item2;
              changeElem.LeftChange = Change.Add;
              result.Add(changeElem);
              changeElem = new ChangeElement();
              changeElem.Right = rightList[rightPtr].Item2;
              changeElem.RightChange = Change.Add;
            }
            leftPtr++;
            rightPtr++;
            break;
          case ThreeWayStatus.LeftAddRightDelete:
          case ThreeWayStatus.LeftAddRightEqual:
            changeElem.Left = leftList[leftPtr].Item2;
            changeElem.LeftChange = Change.Add;
            leftPtr++;
            break;
          case ThreeWayStatus.LeftDeleteRightAdd:
          case ThreeWayStatus.LeftEqualRightAdd:
            changeElem.Right = rightList[rightPtr].Item2;
            changeElem.RightChange = Change.Add;
            rightPtr++;
            break;
          case ThreeWayStatus.LeftDeleteRightDelete:
            changeElem.Base = baseList[basePtr].Item2;
            changeElem.LeftChange = Change.Delete;
            changeElem.RightChange = Change.Delete;
            basePtr++;
            break;
          case ThreeWayStatus.LeftDeleteRightEqual:
            changeElem.Base = baseList[basePtr].Item2;
            changeElem.Right = rightList[rightPtr].Item2;
            changeElem.LeftChange = Change.Delete;
            changeElem.RightChange = Change.NoChange;
            basePtr++;
            rightPtr++;
            break;
          case ThreeWayStatus.LeftEqualRightDelete:
            changeElem.Base = baseList[basePtr].Item2;
            changeElem.Left = leftList[leftPtr].Item2;
            changeElem.RightChange = Change.Delete;
            changeElem.LeftChange = Change.NoChange;
            basePtr++;
            leftPtr++;
            break;
          default: // ThreeWayStatus.LeftEqualRightEqual
            changeElem.Base = baseList[basePtr].Item2;
            changeElem.Left = leftList[leftPtr].Item2;
            changeElem.Right = rightList[rightPtr].Item2;
            basePtr++;
            leftPtr++;
            rightPtr++;
            break;
        }


      }
    }

    private static IList<Tuple<string, XElement>> GetKeys(IEnumerable<XElement> elems)
    {
      var mode = KeyMode.Undefined;
      var results = new List<Tuple<string, XElement>>();

      foreach (var elem in elems)
      {
        if (mode == KeyMode.Undefined)
        {
          if (elem.Name.LocalName == "Item" 
            && (elem.Attribute("action") == null || elem.Attribute("action").Value != "get") 
            && elem.Attribute("type") != null
            && (elem.Attribute("id") != null || elem.Attribute("where") != null || elem.Attribute("idlist") != null))
          {
            mode = KeyMode.ItemId;
          }
          else 
          {
            mode = KeyMode.ElementName;
          }
        }

        if (mode == KeyMode.ItemId)
        {
          if (elem.Attribute("id") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("id").Value, elem));
          }
          else if (elem.Attribute("idlist") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("idlist").Value, elem));
          }
          else if (elem.Attribute("where") != null)
          {
            results.Add(Tuple.Create(elem.Attribute("where").Value, elem));
          }
        }
        else
        {
          results.Add(Tuple.Create(elem.Name.LocalName, elem));
        }
      }

      //results.Sort((x, y) => x.Item1.CompareTo(y.Item1));
      return results;
    }
  }
}
