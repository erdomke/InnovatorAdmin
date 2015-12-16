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
using System.Diagnostics;
using System.Linq;

namespace HgSharp.Core
{
    [DebuggerDisplay("{Name,nq}")]
    public class HgRcSection
    {
        private readonly IList<HgRcSectionEntry> entries;

        public string Name { get; private set; }

        public string this[string propertyName]
        {
            get
            {
                var property = GetProperties().SingleOrDefault(p => p.Property.Item1 == propertyName);
                return property == null ? null : property.Property.Item2;
            }
            set
            {
                var property = GetProperties().SingleOrDefault(p => p.Property.Item1 == propertyName);
                if(property != null)
                    entries.Remove(property);

                entries.Add(new HgRcSectionEntry { Property = Tuple.Create(propertyName, value) });
            }
        }

        public ReadOnlyCollection<string> Properties
        {
            get { return new ReadOnlyCollection<string>(GetProperties().Select(p => p.Property.Item1).ToList()); }
        }

        internal HgRcSection(string name, IEnumerable<HgRcSectionEntry> properties)
        {
            Name = name;
            this.entries = new List<HgRcSectionEntry>(properties ?? new List<HgRcSectionEntry>());
        }

        private IEnumerable<HgRcSectionEntry> GetProperties()
        {
            return entries.Where(e => e.Property != null);
        } 

        internal void Add(HgRcSectionEntry entry)
        {
            entries.Add(entry);
        }
    }
}