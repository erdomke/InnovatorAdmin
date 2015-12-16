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
using System.IO;

using NLog;

namespace HgSharp.Core
{
    internal class HgTransaction : IDisposable
    {
        private readonly HgStore hgStore;
        private readonly HgJournal hgJournal;
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IList<IHgTransactionalAction> transactionalActions = new List<IHgTransactionalAction>();
        private HgTransactionState transactionState;

        internal HgTransaction(HgStore hgStore, HgJournal hgJournal)
        {
            // TODO: 
            /*         
            # abort here if the journal already exists
            if os.path.exists(self.sjoin("journal")):
                raise error.RepoError(
                    _("abandoned transaction found - run hg recover"))
            */
            
            this.hgStore = hgStore;
            this.hgJournal = hgJournal;

            log.Info("starting transaction");

            transactionState = HgTransactionState.Active;
        }

        public void Enlist(IHgTransactionalAction hgTransactionalAction)
        {
            transactionalActions.Add(hgTransactionalAction);
        }

        public void Enlist(HgManifest hgManifest)
        {
            hgStore.Enlist(hgManifest, this);
        }

        public void Enlist(HgChangelog hgChangelog)
        {
            hgStore.Enlist(hgChangelog, this);
        }

        public void Enlist(HgFilelog hgFilelog)
        {
            hgStore.Enlist(hgFilelog, this);
        }

        internal void AddJournalEntry(string name, string path)
        {
            var fileInfo = new FileInfo(path);
            hgJournal.Add(new HgJournalEntry(name, fileInfo.Exists ? fileInfo.Length : 0));
        }

        public void Commit()
        {
            if(transactionState != HgTransactionState.Active) return;

            log.Info("committing transaction");

            foreach(var transactionalAction in transactionalActions)
            {
                transactionalAction.Commit(); 
            } // foreach

            hgJournal.Commit();

            transactionState = HgTransactionState.Commit;
        }

        public void Rollback()
        {
            if(transactionState != HgTransactionState.Active) return;

            log.Info("rolling back transaction");

            foreach(var transactionalAction in transactionalActions)
            {
                transactionalAction.Rollback();
            } // foreach

            foreach(var entry in hgJournal.Entries)
            {
                var filePath = hgStore.ResolveFilePath(entry.Name);

                log.Info("rolling back '{0}'", filePath);

                if(entry.Length == 0)
                {
                    if(File.Exists(filePath))
                        File.Delete(filePath);
                } // if
                else
                {
                    using(var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None))
                        fileStream.SetLength(entry.Length);
                } // else
            } // foreach

            hgJournal.Rollback();

            transactionState = HgTransactionState.Rollback;
        }

        public void Dispose()
        {
            // if not committed
            //Rollback();
        }
    }
}