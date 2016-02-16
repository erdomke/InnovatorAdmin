using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Diff
{
  public class KeyedListDiff
  {
    public IEnumerable<IDiffRecord> GetDiff<T>(IEnumerable<T> baseData, IEnumerable<T> compareData, 
                                               Func<T, object> keyGetter, Func<T, T, IEnumerable<IDiffRecord>> itemDiffGetter)
    {
      var baseList = baseData.ToList();
      baseList.Sort((x,y) => Compare(keyGetter(x), keyGetter(y), StringComparison.InvariantCultureIgnoreCase));
      var compareList = compareData.ToList();
      compareList.Sort((x,y) => Compare(keyGetter(x), keyGetter(y), StringComparison.InvariantCultureIgnoreCase));

      var basePtr = 0;
      var comparePtr = 0;
      IEnumerable<IDiffRecord> itemDiffs;
      var result = new List<IDiffRecord>();
      bool isEqual;
      DiffRecord<T> diff;

      while (basePtr < baseList.Count && comparePtr < compareList.Count) 
      {
        diff = new DiffRecord<T>();
        switch (Compare(keyGetter(baseList[basePtr]), keyGetter(compareList[comparePtr]), StringComparison.InvariantCultureIgnoreCase)) 
        {
          case -1:
            diff.Action = DiffAction.Delete;
            diff.Base = baseList[basePtr];
            basePtr++;
            break;
          case 1:
            diff.Action = DiffAction.Add;
            diff.Compare = compareList[comparePtr];
            comparePtr++;
            break;
          default:
            isEqual = true;
            itemDiffs = itemDiffGetter(baseList[basePtr],compareList[comparePtr]);
            if (itemDiffs != null) {
              foreach (var itemDiff in itemDiffs) {
                if (itemDiff.Action != DiffAction.None) {
                  isEqual = false;
                  break;
                }
              }
            }

            diff.Action = (isEqual ? DiffAction.None : DiffAction.Change);
            diff.Base = baseList[basePtr];
            diff.Compare = compareList[comparePtr];
            diff.AddChanges(itemDiffs);
            basePtr++;
            comparePtr++;
            break;
        }
        result.Add(diff);  
      }
      while (basePtr < baseList.Count) {
        diff = new DiffRecord<T>();
        diff.Action = DiffAction.Delete;
        diff.Base = baseList[basePtr];
        result.Add(diff);
        basePtr++;
      }
      while (comparePtr < compareList.Count) {
        diff = new DiffRecord<T>();
        diff.Action = DiffAction.Add;
        diff.Compare = compareList[comparePtr];
        result.Add(diff);
        comparePtr++;
      }

      return result;
    }

    private int Compare(object x, object y, StringComparison compare)
    {
      if (x == null && y == null)
      {
        return 0;
      }
      else if (x == null)
      {
        return -1;
      }
      else if (y == null)
      {
        return 1;
      }
      else
      {
        var xString = x as string;
        var yString = y as string;

        if (xString != null && yString != null)
        {
          return string.Compare(xString, yString, compare);
        }
        else
        {
          var xCompare = x as IComparable;
          var yCompare = y as IComparable;
          if (xCompare != null && yCompare != null)
          {
            return xCompare.CompareTo(yCompare);
          }
          else
          {
            return string.Compare(x.ToString(), y.ToString(), compare);
          }
        }
      }
    }

    public static IEnumerable<IDiffRecord> ObjectDiff(object x, object y)
    {
      var diff = new DiffRecord<object>();
      diff.Action = (Extension.IsEqual(x, y) ? DiffAction.None : DiffAction.Change);
      diff.Base = x;
      diff.Compare = y;
      return new List<IDiffRecord>() { diff };
    }
  }
}
