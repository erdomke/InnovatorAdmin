using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class FileCompare
  {
    private MergeStatus _resolutionStatus = MergeStatus.UnresolvedConflict;


    public string Path { get; set; }
    public FileStatus InBase { get; set; }
    public FileStatus InLocal { get; set; }
    public FileStatus InRemote { get; set; }
    public bool IsModified { get { return _resolutionStatus != MergeStatus.UnresolvedConflict; } }
    public MergeStatus ResolutionStatus
    {
      get
      {
        return _resolutionStatus == MergeStatus.UnresolvedConflict
          ? this.Status : _resolutionStatus;
      }
      set { _resolutionStatus = value; }
    }
    public MergeStatus Status
    {
      get
      {
        return _status[(int)InBase + ((int)InLocal << 2) + ((int)InRemote << 4)];
      }
    }
    public string StatusDescription
    {
      get
      {
        switch (this.Status)
        {
          case MergeStatus.TakeLocal:
            if (InLocal == FileStatus.DoesntExist)
            {
              return "Local removed";
            }
            else
            {
              return "Local added / changed";
            }
          case MergeStatus.TakeRemote:
            if (InRemote == FileStatus.DoesntExist)
            {
              return "Remote removed";
            }
            else
            {
              return "Remote added / changed";
            }
          case MergeStatus.NoChange:
            return "No change";
          default:
            if (InBase == FileStatus.DoesntExist)
            {
              return "Both local and remote added";
            }
            else if (InLocal == FileStatus.DoesntExist)
            {
              return "Local removed / remote modified";
            }
            else if (InRemote == FileStatus.DoesntExist)
            {
              return "Local modified / remote removed";
            }
            else
            {
              return "Both local and remote changed";
            }
        }
      }
    }

    private static Dictionary<int, MergeStatus> _status = new Dictionary<int, MergeStatus>()
    {
      {(int)FileStatus.DoesntExist + ((int)FileStatus.DoesntExist << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.TakeRemote},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.DoesntExist << 2) + ((int)FileStatus.Modified << 4), MergeStatus.TakeRemote},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.DoesntExist << 4), MergeStatus.TakeLocal},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.Modified << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Modified << 2) + ((int)FileStatus.DoesntExist << 4), MergeStatus.TakeLocal},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Modified << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.DoesntExist + ((int)FileStatus.Modified << 2) + ((int)FileStatus.Modified << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.Unchanged + ((int)FileStatus.DoesntExist << 2) + ((int)FileStatus.DoesntExist << 4), MergeStatus.TakeLocal},
      {(int)FileStatus.Unchanged + ((int)FileStatus.DoesntExist << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.TakeLocal},
      {(int)FileStatus.Unchanged + ((int)FileStatus.DoesntExist << 2) + ((int)FileStatus.Modified << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.DoesntExist << 4), MergeStatus.TakeRemote},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.NoChange},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Unchanged << 2) + ((int)FileStatus.Modified << 4), MergeStatus.TakeRemote},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Modified << 2) + ((int)FileStatus.DoesntExist << 4), MergeStatus.UnresolvedConflict},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Modified << 2) + ((int)FileStatus.Unchanged << 4), MergeStatus.TakeLocal},
      {(int)FileStatus.Unchanged + ((int)FileStatus.Modified << 2) + ((int)FileStatus.Modified << 4), MergeStatus.UnresolvedConflict}
    };

    public override string ToString()
    {
      return InBase.ToString()[0].ToString()
        + InLocal.ToString()[0].ToString()
        + InRemote.ToString()[0].ToString()
        + " " + Path;
    }
  }
}
