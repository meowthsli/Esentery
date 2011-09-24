using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> A cursor over sequence of bookmarks </summary>
    public sealed class SynthesizedCursor : HasJetHandleBase<JET_TABLEID>, ICursor
    {
        /// <summary> Table object </summary>
        public Table Table { get; private set; }

        /// <summary> Moves to next data </summary>
        public bool MoveNext()
        {
            var res = BookmarkGenerator.MoveNext();
            if (res)
            {
                var bookmark = (byte[])BookmarkGenerator.Current;
                Api.JetGotoBookmark(CurrentSession, this, bookmark, bookmark.Length);
            }

            return res;
        }

        /// <summary> Appends new row </summary>
        public RowModification AddRow()
        {
            return new RowModification(Table, this, JET_prep.Insert);
        }

        /// <summary> Edits row </summary>
        public RowModification EditRow()
        {
            return new RowModification(Table, this, JET_prep.Replace);
        }

        /// <summary> Removes current row </summary>
        public void DeleteRow()
        {
            Api.JetDelete(CurrentSession, this);
        }

        /// <summary> Create from bookmark sequence </summary>
        internal SynthesizedCursor(IEnumerable<Bookmark> bookmarkGenerator, Table table)
        {
            BookmarkGenerator = bookmarkGenerator.GetEnumerator();
            Table = table;
            
            // dups table handle and sets up primary index
            Api.JetDupCursor(CurrentSession, Table, out JetHandle, DupCursorGrbit.None);
            Api.MoveBeforeFirst(CurrentSession, this);
        }

        public override Session CurrentSession
        {
            get { return Table.Database.CurrentSession; }
        }

        protected override void Dispose(bool dispose)
        {
            if (!CurrentSession.Disposed)
            {
                Api.JetCloseTable(CurrentSession, this);
            }
        }

        /// <summary> Returns stream at column </summary>
        public ColumnStream OpenStream(Column column)
        {
            return new ColumnStream(CurrentSession, this, column);
        }

        /// <summary> Returns value at column </summary>
        public object GetValue(Column column)
        {
            return Converters.GetGetter(column.ColumnType)
                (CurrentSession, this, column);
        }

        private IEnumerator<Bookmark> BookmarkGenerator { get; set; }
    }
}