using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using HgSharp.Core.Util;

using System.Linq;

namespace HgSharp.Core
{
    public class HgBookmarkManager
    {
        private readonly HgRepository repository;
        private readonly HgFileSystem fileSystem;
        private readonly string bookmarksPath;

        public HgBookmarkManager(HgRepository repository, HgFileSystem fileSystem)
        {
            this.repository = repository;
            this.fileSystem = fileSystem;

            bookmarksPath = Alphaleonis.Win32.Filesystem.Path.Combine(repository.BasePath, "bookmarks");
        }

        public void UpdateBookmark(string name, HgNodeID changesetNodeID)
        {
            var bookmarks = ReadBookmarks();

            bookmarks.Remove(bookmarks.SingleOrDefault(b => b.Name == name));
            bookmarks.Add(new HgBookmark(name, repository.Changelog[changesetNodeID]));

            WriteBookmarks(bookmarks);
        }

        public void DeleteBookmark(string name)
        {
            var bookmarks = ReadBookmarks();

            bookmarks.Remove(bookmarks.SingleOrDefault(b => b.Name == name));

            WriteBookmarks(bookmarks);
        }

        private void WriteBookmarks(IList<HgBookmark> bookmarks)
        {
            using(var booknmarksStream = fileSystem.CreateWrite(bookmarksPath))
            using(var streamWriter = new StreamWriter(booknmarksStream))
            {
                foreach(var bookmark in bookmarks)
                {
                    streamWriter.Write("{0} {1}\n", bookmark.Changeset.Metadata.NodeID.Long, bookmark.Name);
                } // foreach
            } // using
        }

        private IList<HgBookmark> ReadBookmarks()
        {
            var bookmarks = new List<HgBookmark>();

            if(!Alphaleonis.Win32.Filesystem.File.Exists(bookmarksPath)) 
                return bookmarks;

            using(var booknmarksStream = fileSystem.OpenRead(bookmarksPath))
            using(var streamReader = new StreamReader(booknmarksStream))
            {
                var line = "";
                while((line = streamReader.ReadLine()) != null)
                {
                    var changesetNodeID = new HgNodeID(line.SubstringBefore(" "));
                    var name = line.SubstringAfter(" ").Trim();

                    bookmarks.Add(new HgBookmark(name, repository.Changelog[changesetNodeID]));
                } // while
            } // using

            return bookmarks;
        }

        public ReadOnlyCollection<HgBookmark> GetBookmarks()
        {
            return new ReadOnlyCollection<HgBookmark>(ReadBookmarks());
        }
    }
}
