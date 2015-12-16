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
using System.Collections.ObjectModel;

using System.Linq;

using HgSharp.Core.Util;

using NLog;

namespace HgSharp.Core
{
    public class HgAclReader
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public HgAcl ReadAcl(HgRc hgRc)
        {
            if(hgRc == null)
            {
                log.Info("no hgrc");
                return null;
            } // if

            if(hgRc["acl"] == null)
            {
                log.Info("no [acl[ section in hgrc");
                return null;
            } // if

            var sources = hgRc["acl"]["sources"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var deny = ReadAclSection(hgRc, "acl.deny");
            var allow = ReadAclSection(hgRc, "acl.allow");
            var allowBranches = ReadAclSection(hgRc, "acl.allow.branches");
            var denyBranches = ReadAclSection(hgRc, "acl.deny.branches");

            return new HgAcl(new ReadOnlyCollection<string>(sources), allow, deny, allowBranches, denyBranches);
        }

        private ReadOnlyCollection<HgAclEntry> ReadAclSection(HgRc hgRc, string section)
        {
            if(hgRc[section] == null) return null;
            var aclEntries = new List<HgAclEntry>();

            foreach(var property in hgRc[section].Properties)
            {
                var hgAclEntry = new HgAclEntry(property, ReadAclPrincipals(hgRc[section][property]).ToList());
                aclEntries.Add(hgAclEntry);
            } // foreach

            return new ReadOnlyCollection<HgAclEntry>(aclEntries);
        }

        private IEnumerable<HgAclPrincipal> ReadAclPrincipals(string principals)
        {
            var names = principals.Split(',');
            foreach(var s in names)
            {
                var name = s.Trim();
                var type = HgAclPrincipalType.User;
                var excludes = false;

                if(name.StartsWith("!"))
                {
                    excludes = true;
                    name = name.SubstringAfter("!");
                } // if

                if(name.StartsWith("@"))
                {
                    type = HgAclPrincipalType.Group;
                    name = name.SubstringAfter("@");
                } // if

                yield return new HgAclPrincipal(excludes, type, name);
            }
        }
    }
}