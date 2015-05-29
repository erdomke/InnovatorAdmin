using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ADiff.Api;

namespace Aras.Tools.InnovatorAdmin
{
  public enum DiffType
  {
    LeftOnly,
    RightOnly,
    Identical,
    Different
  }

  public class InstallItemDiff
  {
    private ItemReference _itemRef;
    private string _name;
    private InstallType _type;

    public InstallType Type
    {
      get { return _type; }
      set { _type = value; }
    }
    public ItemReference Reference { get { return _itemRef; } }
    public string Name
    {
      get
      {
        if (_name == null)
        {
          switch (_type)
          {
            case InstallType.Create:
              return "Install of " + this.Reference.ToString();
            case InstallType.DependencyCheck:
              return "Check of Dependency " + this.Reference.ToString();
            case InstallType.Script:
              return "Script: " + this.Reference.KeyedName;
          }
        }
        return _name;
      }
      set { _name = value; }
    }

    public DiffType DiffType { get; private set; }
    public bool LeftExists { get { return this.LeftScript != null; } }
    public bool RightExists { get { return this.RightScript != null; } }

    public XmlElement LeftScript { get; private set; }
    public XmlElement RightScript { get; private set; }

    public static IEnumerable<InstallItemDiff> GetDiffs(InstallScript left, InstallScript right)
    {
      return GetDiffs(left.Lines, right.Lines);
    }
    public static IEnumerable<InstallItemDiff> GetDiffs(IEnumerable<InstallItem> left, IEnumerable<InstallItem> right)
    {
      var leftList = left.Where(i => i.Type != InstallType.Warning).ToArray();
      var rightList = right.Where(i => i.Type != InstallType.Warning).ToArray();
      var compares = ListCompare.Create(leftList, rightList, i => i.Reference);
      var results = new List<InstallItemDiff>();
      
      foreach (var compare in compares)
      {
        if (compare.Base < 0)
        {
          results.Add(new InstallItemDiff()
          {
            _itemRef = rightList[compare.Compare].Reference,
            _type = rightList[compare.Compare].Type,
            DiffType = DiffType.RightOnly,
            RightScript = rightList[compare.Compare].Script
          });
        }
        else if (compare.Compare < 0)
        {
          results.Add(new InstallItemDiff()
          {
            _itemRef = leftList[compare.Base].Reference,
            _type = leftList[compare.Base].Type,
            DiffType = DiffType.LeftOnly,
            LeftScript = leftList[compare.Base].Script
          });
        }
        else
        {
          results.Add(new InstallItemDiff()
          {
            _itemRef = leftList[compare.Base].Reference,
            _type = leftList[compare.Base].Type,
            DiffType = (AmlDiff.IsDifferent(leftList[compare.Base].Script.OuterXml, rightList[compare.Compare].Script.OuterXml) ? 
                        DiffType.Different : DiffType.Identical),
            LeftScript = leftList[compare.Base].Script,
            RightScript = rightList[compare.Compare].Script
          });
        }
      }

      return results;
    }
  }
}
