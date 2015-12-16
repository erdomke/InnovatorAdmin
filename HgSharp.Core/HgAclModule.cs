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
    public class HgAclModule : IDisposable
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly HgRepository hgRepository;
        private readonly Func<HgPrincipal> hgPrincipalProvider;

        public HgAclModule(HgRepository hgRepository, Func<HgPrincipal> hgPrincipalProvider)
        {
            this.hgRepository = hgRepository;
            this.hgPrincipalProvider = hgPrincipalProvider;
            this.hgRepository.BundleCommitting += BundleCommitting;
        }

        private void BundleCommitting(HgBundleCommittedEventArgs args)
        {
            log.Info("handling BundleCommitting");
            var acl = new HgAclReader().ReadAcl(hgRepository.Rc);
            if(acl == null)
            {
                log.Info("no acl defined for repository at '{0}'", hgRepository.FullPath);
                return;
            } // if

            if(!acl.Sources.Contains("serve"))
            {
                log.Info("[acl].sources does not have 'serve'");
                return;
            } // if


            //
            // Checking in order:
            // - acl.deny.branches
            // - acl.allow.branches
            // - acl.deny
            // - acl.allow
            var principal = NormalizeHgPrincipal(hgPrincipalProvider.Invoke());

            log.Info("verifying access for '{0}' ('{1}')", principal.Name, string.Join("', '", principal.Groups));

            var denyBranches = BuildBranchMatch(principal, "acl.deny.branches", acl.DenyBranches);
            var allowBranches = BuildBranchMatch(principal, "acl.allow.branches", acl.AllowBranches);
            var deny = BuildPathMatch(principal, "acl.deny", acl.Deny);
            var allow = BuildPathMatch(principal, "acl.allow", acl.Allow);

            foreach(var changeset in args.Changesets)
            {
                log.Info("verifying access in changeset {0}", changeset.Metadata.NodeID.Short);

                var branch = changeset.Branch.Name;
                if(denyBranches != null && denyBranches(branch))
                    throw new HgSecurityException("User '{0}' is denied on branch '{1}' (changeset {2})".FormatWith(
                        principal.Name, branch, changeset.Metadata.NodeID.Short));
            
                if(allowBranches != null && !allowBranches(branch))
                    throw new HgSecurityException("User '{0}' is not allowed on branch '{1}' (changeset {2})".FormatWith(
                        principal.Name, branch, changeset.Metadata.NodeID.Short));

                foreach(var file in changeset.Files)
                {
                    if(deny != null && deny(new HgPath(file)))
                        throw new HgSecurityException("User '{0}' is denied on '{1}' (changeset {2})".FormatWith(
                            principal.Name, file, changeset.Metadata.NodeID.Short));

                    if(allow != null && !allow(new HgPath(file)))
                        throw new HgSecurityException("User '{0}' is not allowed on '{1}' (changeset {2})".FormatWith(
                            principal.Name, file, changeset.Metadata.NodeID.Short));
                } // foreach

                log.Info("access granted in changeset {0}", changeset.Metadata.NodeID.Short);
            } // foreach
        }

        private HgPrincipal NormalizeHgPrincipal(HgPrincipal hgPrincipal)
        {
            return new HgPrincipal(hgPrincipal.Name.ToLowerInvariant(), hgPrincipal.Groups.Select(g => g.ToLowerInvariant()));
        }

        private Func<string, bool> BuildBranchMatch(HgPrincipal principal, string section, IEnumerable<HgAclEntry> hgAclEntries)
        {
            if(hgAclEntries == null) return null;

            var patterns = hgAclEntries.Where(e => Matches(principal, e)).Select(e => e.Path).ToList();
            
            if(patterns.Count == 0) 
                return s => false;
            
            return s => patterns.Contains(s);
        }

        private Func<HgPath, bool> BuildPathMatch(HgPrincipal principal, string section, IEnumerable<HgAclEntry> hgAclEntries)
        {
            if(hgAclEntries == null) return null;

            var matchers = hgAclEntries.Where(e => Matches(principal, e)).Select(e => HgPathPatternMatcher.Parse(e.Path)).ToList();
            
            if(matchers.Count == 0) return s => false;
            
            return s => matchers.Any(m => m.Matches(s));
        }

        private bool Matches(HgPrincipal principal, HgAclEntry entry)
        {
            foreach(var aclPrincipal in entry.Principals)
            {
                if(aclPrincipal.Name == "*") return true;

                if(aclPrincipal.Excludes)
                {
                    if(aclPrincipal.Type == HgAclPrincipalType.User && principal.Name != aclPrincipal.Name)
                        return true;
                    
                    if(aclPrincipal.Type == HgAclPrincipalType.Group && !principal.Groups.Contains(aclPrincipal.Name))
                        return true;
                } // if
                else
                {
                    if(aclPrincipal.Type == HgAclPrincipalType.User && principal.Name == aclPrincipal.Name)
                        return true;

                    if(aclPrincipal.Type == HgAclPrincipalType.Group && principal.Groups.Contains(aclPrincipal.Name))
                        return true;
                } // else
            } // foreach

            return false;
        }

        public void Dispose()
        {
            hgRepository.BundleCommitting -= BundleCommitting;
        }
    }
}
