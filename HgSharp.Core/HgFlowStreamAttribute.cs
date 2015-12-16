using System;

namespace HgSharp.Core
{
    public sealed class HgFlowStreamAttribute : Attribute
    {
        public HgFlowStream TrunkStream { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Order is important here.</remarks>
        public HgFlowStream[] MergeStreams { get; set; }
    }
}