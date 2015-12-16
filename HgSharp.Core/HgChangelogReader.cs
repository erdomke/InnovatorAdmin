// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgChangelogReader : HgRevlogReaderBase
    {
        private readonly HgEncoder hgEncoder;

        public HgChangelogReader(HgEncoder hgEncoder)
        {
            this.hgEncoder = hgEncoder;
        }

        public IEnumerable<HgChangeset> ReadChangesets(IEnumerable<HgRevlogEntryData> revlogEntries)
        {
            foreach(var revlogEntry in revlogEntries)
                yield return ReadChangeset(revlogEntry);
        }

        public HgChangeset ReadChangeset(HgRevlogEntryData revlogEntryData)
        {
            try
            {
                return ReadChangesetInternal(revlogEntryData);
            }
            catch(Exception e)
            {
                throw;
            }
        }

        private HgChangeset ReadChangesetInternal(HgRevlogEntryData revlogEntryData)
        {
            var filesStart = revlogEntryData.Data.IndexOfNth((byte)'\n', 3);
            var filesEnd = revlogEntryData.Data.IndexOf((byte)'\n', (byte)'\n');

            var headerString = hgEncoder.DecodeAsUtf8(revlogEntryData.Data, 0, filesStart);
            
            var filesString = 
                filesStart == filesEnd ? 
                    "" :
                    hgEncoder.DecodeAsLocal(
                        revlogEntryData.Data, 
                        filesStart + 1, // The \n 
                        filesEnd - filesStart - 1);
            
            var commentString = hgEncoder.DecodeAsUtf8(
                revlogEntryData.Data, 
                filesEnd + 2, // These \n\n bytes 
                revlogEntryData.Data.Length - filesEnd - 2);

            var header = headerString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var manifestNodeID = new HgNodeID(header[0]);
            var committedBy = HgAuthor.Parse(header[1]);

            var timeParts = header[2].Split(' ');
            var time = int.Parse(timeParts[0]);
            var timeZone = int.Parse(timeParts[1]);

            var branchData = header[2].SubstringAfterNth(" ", 1).Or("branch:default");

            var b = branchData.Split('\0').Select(s => new { key = s.Split(':')[0], value = s.Split(':')[1] }).ToDictionary(s => s.key, s => s.value).DefaultableWith(v => "");
            var branchName = new HgBranch(b["branch"].Or("default"), b["close"] == "1");
            var sourceNodeID = new HgNodeID(b["source"].Or(HgNodeID.Null.Long));
            
            var dateTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1).AddSeconds(time).AddSeconds(-1 * timeZone), DateTimeKind.Local);
            var committedAt = new DateTimeOffset(dateTime);

            var files = filesString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            var metadata = GetRevlogEntryMetadata(revlogEntryData.Entry);

            return new HgChangeset(metadata, manifestNodeID, committedBy, committedAt, branchName, sourceNodeID, files, commentString);
        }
    }
}