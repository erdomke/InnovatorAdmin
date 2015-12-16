namespace HgSharp.Core
{
  public struct HgRevision
  {
    public uint Revision { get; private set; }

    public HgNodeID NodeID { get; private set; }

    public HgRevision(uint revision, HgNodeID nodeID)
      : this()
    {
      Revision = revision;
      NodeID = nodeID;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is HgRevision)) return false;
      return Equals((HgRevision)obj);
    }
    public bool Equals(HgRevision rev)
    {
      return this.Revision == rev.Revision && this.NodeID.Equals(rev.NodeID);
    }

    public override int GetHashCode()
    {
      return this.NodeID.GetHashCode() ^ this.Revision.GetHashCode();
    }
  }
}
