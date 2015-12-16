// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Text;

namespace HgSharp.Core
{
    public class HgChangelogWriter
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public byte[] WriteChangeset(HgChangeset hgChangeset)
        {
            var changeset = new StringBuilder();
            changeset.AppendFormat("{0}\n", hgChangeset.ManifestNodeID.Long);
            changeset.AppendFormat("{0} <{1}>\n", hgChangeset.CommittedBy.FullName, hgChangeset.CommittedBy.EmailAddress);
            changeset.AppendFormat("{0} {1}", (int)(hgChangeset.CommittedAt.ToUniversalTime() - Epoch).TotalSeconds, (int)-hgChangeset.CommittedAt.Offset.TotalSeconds);

            if(hgChangeset.Branch != HgBranch.Default)
            {
                changeset.AppendFormat(" branch:{0}", hgChangeset.Branch.Name);
                if(hgChangeset.Branch.Closed)
                    changeset.AppendFormat("\0close:1");
            } // if

            changeset.Append('\n');

            foreach(var file in hgChangeset.Files)
                changeset.AppendFormat("{0}\n", file.TrimStart('/'));

            changeset.Append('\n');
            changeset.Append(hgChangeset.Comment);

            var changesetData = Encoding.UTF8.GetBytes(changeset.ToString());
            return changesetData;
        }
    }
} 