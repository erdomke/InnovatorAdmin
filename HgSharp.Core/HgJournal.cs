// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace HgSharp.Core
{
    internal class HgJournal
    {
        private readonly HgStore hgStore;
        private readonly StreamWriter journalWriter;

        public IList<HgJournalEntry> Entries { get; private set; }
        
        public string FullPath { get; private set; }

        public HgJournal(HgStore hgStore)
        {
            this.hgStore = hgStore;
            
            Entries = new List<HgJournalEntry>();
            FullPath = Path.Combine(hgStore.StoreBasePath, "journal");

            journalWriter = new StreamWriter(new FileStream(FullPath, FileMode.Create, FileAccess.Write));
        }

        public void Add(HgJournalEntry hgJournalEntry)
        {
            journalWriter.Write(hgJournalEntry.Name);
            journalWriter.Write((char)0);
            journalWriter.Write(hgJournalEntry.Length.ToString(CultureInfo.InvariantCulture));
            journalWriter.Write(new [] { (char)13, (char)10 });
            
            journalWriter.Flush();

            Entries.Add(hgJournalEntry);
        }

        public void Commit()
        {
            journalWriter.Flush();
            journalWriter.Close();
            journalWriter.Dispose();

            var undoPath = Path.Combine(hgStore.StoreBasePath, "undo");
            
            if(File.Exists(undoPath)) File.Delete(undoPath);
            File.Move(FullPath, undoPath);
        }

        public void Rollback()
        {
            journalWriter.Flush();
            journalWriter.Close();
            journalWriter.Dispose();

            File.Delete(FullPath);
        }
    }
}
