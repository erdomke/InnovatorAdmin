// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Linq;
using System.Text;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgFlowSupport
    {
        public static bool IsEnabled(HgRepository hgRepository)
        {
            var hgFlow = GetHgFlow(hgRepository);
            return IsEnabled(hgRepository, hgFlow);
        }

        public static bool IsEnabled(HgRepository hgRepository, HgFlow hgFlow)
        {
            if(!hgFlow.Enabled) return false;

            //
            // Ensure that the "develop" branch is not closed
            var hgDevelopBranch = 
                hgRepository.
                    GetBranches().
                        FirstOrDefault(c => !c.Branch.Closed && string.Equals(c.Branch.Name, hgFlow.Development, StringComparison.InvariantCulture));

            return hgDevelopBranch != null;
        }

        public static HgFlow GetHgFlow(HgRepository hgRepository)
        {
            if(hgRepository.Changelog.Tip == null) return new HgFlow(false);

            //
            // Check for .hgflow or .flow on open branches
            foreach(var hgBranch in hgRepository.GetBranches().Where(c => !c.Branch.Closed))
            {
                var hgManifest = hgRepository.Manifest[hgBranch.ManifestNodeID];
                if(hgManifest == null) continue;

                var hgFlowFileEntry = hgManifest[".hgflow"] ?? hgManifest[".flow"];
                if(hgFlowFileEntry == null) continue;

                var hgFlowFile = hgRepository.GetFile(hgFlowFileEntry);
                if(hgFlowFile == null) continue;

                var content = Encoding.UTF8.GetString(hgFlowFile.Data);

                var hgFlow = ParseHgFlow(content);
                if(hgFlow.Enabled)
                    return hgFlow;
            } // foreach

            return new HgFlow(false);
        }

        public static HgFlowStream? GetStreamForBranch(HgFlow hgFlow, string branchName)
        {
            var prefix = branchName.Contains("/") ? branchName.SubstringBefore("/") + "/" : branchName;
            var stream = hgFlow.Where(s => s.Value == prefix).ToList();
            if(stream.Count == 0) return null;

            return stream[0].Key;
        }

        public static HgFlowStream[] GetMergeStreams(HgFlowStream stream)
        {
            var hgFlowStreamAttribute = GetFlowStreamAttribute(stream);
            return hgFlowStreamAttribute == null ?
                new HgFlowStream[] { } :
                hgFlowStreamAttribute.MergeStreams;
        }

        public static HgFlowStream? GetPrimaryMergeStream(HgFlowStream stream)
        {
            var mergeStreams = GetMergeStreams(stream);
            return mergeStreams.Length == 0 ? 
                (HgFlowStream?)null : 
                mergeStreams[0];
        }

        public static HgRevsetEntry GetPrimaryMergeStreamBranchHead(HgRepository hgRepository, HgFlow hgFlow, HgFlowStream stream)
        {
            var primaryMergeStream = GetPrimaryMergeStream(stream);

            var primaryMergeStreamBranchHead = 
                primaryMergeStream.HasValue ?
                    hgRepository.GetBranchmap().
                        Where(bm => bm.Branch == hgFlow[primaryMergeStream.Value].TrimEnd('/')).
                        SelectMany(bm => bm.Heads).
                        OrderByDescending(h => h.Revision).
                        FirstOrDefault() :
                null;
            
            return primaryMergeStreamBranchHead;
        }

        public static HgFlowStream? GetTrunkStream(HgFlowStream stream)
        {
            var hgFlowStreamAttribute = GetFlowStreamAttribute(stream);
            return hgFlowStreamAttribute == null ? 
                (HgFlowStream?)null : 
                hgFlowStreamAttribute.TrunkStream;
        }

        private static HgFlowStreamAttribute GetFlowStreamAttribute(HgFlowStream stream)
        {
            var hgFlowStreamAttribute =
                typeof(HgFlowStream).
                    GetField(stream.ToString()).
                        GetCustomAttributes(typeof(HgFlowStreamAttribute), false).
                        OfType<HgFlowStreamAttribute>().
                        SingleOrDefault();


            return hgFlowStreamAttribute;
        }

        private static HgFlow ParseHgFlow(string content)
        {
            var hgFlow = new HgFlow(true);

            foreach(var line in content.ReadLines().Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#")))
            {
                if(line.StartsWith("[") && line.EndsWith("]"))
                {
                    var sectionName = line.SubstringBetween("[", "]");
                    if(sectionName != "branchname") return new HgFlow(false);
                } // if
                else
                {
                    var propertyName = line.SubstringBefore("=").Trim();
                    var propertyValue = line.SubstringAfter("=").Trim();

                    switch(propertyName)
                    {
                        case "master":
                            hgFlow.Master = propertyValue;
                            break;
                        case "develop":
                        case "development":
                            hgFlow.Development = propertyValue;
                            break;
                        case "feature":
                            hgFlow.Feature = propertyValue;
                            break;
                        case "release":
                            hgFlow.Release = propertyValue;
                            break;
                        case "hotfix":
                            hgFlow.Hotfix = propertyValue;
                            break;
                        case "support":
                            hgFlow.Support = propertyValue;
                            break;
                    } // switch
                } // else
            } // foreach

            return hgFlow;
        }
    }
}
