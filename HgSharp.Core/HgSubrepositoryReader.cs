using System;
using System.Collections.Generic;

using System.Linq;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgSubrepositoryReader
    {
        private readonly HgRepository hgRepository;

        public HgSubrepositoryReader(HgRepository hgRepository)
        {
            this.hgRepository = hgRepository;
        }

        public IEnumerable<HgSubrepository> ReadSubrepositories(HgManifestEntry hgManifestEntry)
        {
            var hgsub = hgManifestEntry[".hgsub"];
            var hgsubstate = hgManifestEntry[".hgsubstate"];

            if(hgsub == null || hgsubstate == null) yield break;

            var hgsubText = hgRepository.Encoder.DecodeAsUtf8(hgRepository.GetFile(hgsub).Data);
            var hgsubstateText = hgRepository.Encoder.DecodeAsUtf8(hgRepository.GetFile(hgsubstate).Data);

            var subrepositories = 
                hgsubText.
                    Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).
                    Select(s => s.Split('=')).
                    Where(s => s.Length == 2).
                    Select(s => new {
                        path = new HgPath(s[0].Trim()),
                        repositoryPath = s[1].Trim()
                    }).
                    ToDictionary(s => s.path, s => s.repositoryPath);

            var subrepositoryStates = 
                hgsubstateText.
                    Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).
                    Select(s => new {
                        nodeID = new HgNodeID(s.SubstringBefore(" ").Trim()),
                        path = new HgPath(s.SubstringAfter(" ").Trim())
                    }).
                    ToDictionary(s => s.path, s => s.nodeID);

            foreach(var subrepository in subrepositories)
            {
                if(!subrepositoryStates.ContainsKey(subrepository.Key))
                    yield return new HgSubrepository(subrepository.Key, subrepository.Value, HgNodeID.Null);
                else
                    yield return new HgSubrepository(subrepository.Key, subrepository.Value, subrepositoryStates[subrepository.Key]);
            } // foreach
        }
    }
}