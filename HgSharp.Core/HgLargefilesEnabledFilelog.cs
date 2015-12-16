namespace HgSharp.Core
{
    public class HgLargefilesEnabledFilelog : HgFilelog
    {
        public HgLargefilesEnabledFilelog(HgRevlog revlog, HgPath path, HgRevlogReader revlogReader) :
            base(revlog, path, revlogReader)
        {
        }
    }
}