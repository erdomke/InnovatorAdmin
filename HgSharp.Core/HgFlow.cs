// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Collections;
using System.Collections.Generic;

namespace HgSharp.Core
{
    public class HgFlow : IEnumerable<KeyValuePair<HgFlowStream, string>>
    {
        private readonly IDictionary<HgFlowStream, string> streams = new Dictionary<HgFlowStream, string>();
        private readonly bool enabled;

        public bool Enabled
        {
            get { return enabled; }
        }

        public string this[HgFlowStream stream]
        {
            get
            {
                if(!enabled) throw new InvalidOperationException();
                return streams[stream];
            }
            set
            {
                if(!enabled) throw new InvalidOperationException();
                streams[stream] = value;
            }
        }

        public string Master
        {
            get { return this[HgFlowStream.Master]; }
            set { this[HgFlowStream.Master] = value; }
        }

        public string Development
        {
            get { return this[HgFlowStream.Development]; }
            set { this[HgFlowStream.Development] = value; }
        }

        public string Support
        {
            get { return this[HgFlowStream.Support]; }
            set { this[HgFlowStream.Support] = value; }
        }

        public string Feature
        {
            get { return this[HgFlowStream.Feature]; }
            set { this[HgFlowStream.Feature] = value; }
        }

        public string Release
        {
            get { return this[HgFlowStream.Release]; }
            set { this[HgFlowStream.Release] = value; }
        }

        public string Hotfix
        {
            get { return this[HgFlowStream.Hotfix]; }
            set { this[HgFlowStream.Hotfix] = value; }
        }

        public IEnumerable<string> Prefixes
        {
            get
            {
                if(!enabled) throw new InvalidOperationException();
                return streams.Values;
            }
        }

        public HgFlow(bool enabled)
        {
            this.enabled = enabled;
        }

        public IEnumerator<KeyValuePair<HgFlowStream, string>> GetEnumerator()
        {
            if(!enabled) throw new InvalidOperationException();
            return streams.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if(!enabled) throw new InvalidOperationException();
            return GetEnumerator();
        }
    }
}