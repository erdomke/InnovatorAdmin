namespace HgSharp.Core
{
    public enum HgFlowStream
    {
        Development,

        [HgFlowStream(TrunkStream = Development, MergeStreams = new []{ Development })]
        Feature,
        
        [HgFlowStream(TrunkStream = Development, MergeStreams = new []{ Master, Development})]
        Release,

        Master,

        [HgFlowStream(TrunkStream = Master, MergeStreams = new []{ Master, Development })]
        Hotfix,

        [HgFlowStream(TrunkStream = Master, MergeStreams = new []{ Master })]
        Support
    }
}