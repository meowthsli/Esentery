using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Meowth.Esentery.Extensions;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT NativeCursor </summary>
    /// <remarks> This cursor directly maps to ESENT internal cursor, so it reads data directly
    /// by iterating thru Index without calculations. Can be used as bookmark source for
    /// complex processing </remarks>
    public sealed class NativeCursor : HasJetHandleBase<JET_TABLEID>, ICursor, INativeCursor, IEnumerable<Bookmark>
    {
        #region Construction & Disposing

        /// <summary> Opens existing table </summary>
        /// <param name="searchIndex">Can be null</param>
        internal NativeCursor(Table table, ISearchIndex searchIndex)
        {
            Table = table;
            SearchIndex = searchIndex;

            Api.JetDupCursor(CurrentSession, Table, out JetHandle, DupCursorGrbit.None);

            var idx = (SearchIndex != null) ? SearchIndex.Name : null;
            Api.JetSetCurrentIndex(CurrentSession, JetHandle, idx);

            StepBack();
        }

        /// <summary> Disposes table </summary>
        protected override void Dispose(bool dispose)
        {
            if (!CurrentSession.Disposed)
                Api.JetCloseTable(CurrentSession, this);
        }

        public override Session CurrentSession
        {
            get { return Table.CurrentSession; }
        }

        #endregion

        internal void StepBack()
        {
            Api.TryMovePrevious(CurrentSession, this);
        }
        
        /// <summary> Table of this NativeCursor </summary>
        public Table Table { get; private set; }

        /// <summary> Index reference on this </summary>
        public ISearchIndex SearchIndex { get; private set; }

        /// <summary> Appends row </summary>
        public RowModification AddRow()
        {
            return new RowModification(Table, this, JET_prep.Insert);
        }

        /// <summary> Edits row </summary>
        public RowModification EditRow()
        {
            return new RowModification(Table, this, JET_prep.Replace);
        }

        /// <summary> Moved to next record </summary>
        public bool MoveNext()
        {
            return Api.TryMoveNext(CurrentSession, this);
        }

        /// <summary>  </summary>
        public IEnumerator<Bookmark> GetEnumerator()
        {
            while (MoveNext())
                yield return new Bookmark(Api.GetBookmark(CurrentSession, this));
        }

        /// <summary> </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>  </summary>
        public JET_TABLEID CursorHandle
        {
            get { return this; }
        }

        #region Milestones

        /// <summary> Returns milestone of current position on index </summary>
        public Milestone GetMilestone()
        {
            return new Milestone(SearchIndex, 
                Api.RetrieveKey(CurrentSession, this, RetrieveKeyGrbit.None),
                new KeyOptions()
                );
        }

        /// <summary> Jumps to milestone on that index </summary>
        public void GotoMilestone(Milestone ms)
        {
            Api.JetMakeKey(CurrentSession, this, ms.Data, ms.Data.Length, MakeKeyGrbit.NormalizedKey);
        }

        /// <summary> Removes current row </summary>
        public void DeleteRow()
        {
            Api.JetDelete(CurrentSession, this);
        }

        /// <summary> Returns value at column </summary>
        public object GetValue(Column column)
        {
            return Converters.GetGetter(column.ColumnType)
                (CurrentSession, this, column);
        }

        /// <summary> Returns stream at column </summary>
        public ColumnStream OpenStream(Column column)
        {
            return new ColumnStream(CurrentSession, this, column);
        }

        #endregion

        /// <summary> Restricts range on equality of current </summary>
        public void Restrict<T>(Range<T> range)
            //where T :IComparable<T>
        {
            range.Normalize();

            if (range.HasFrom & range.HasTo)
            {
                var keyFrom = Converters.Convert(range.From);
                Api.JetMakeKey(CurrentSession, this, keyFrom, keyFrom.Length, MakeKeyGrbit.NewKey);

                Api.TrySeek(CurrentSession, this, range.InclusiveFrom ? SeekGrbit.SeekGE : SeekGrbit.SeekGT);

                var keyTo = Converters.Convert(range.To);
                Api.JetMakeKey(CurrentSession, this, keyTo, keyTo.Length, MakeKeyGrbit.NewKey);

                Api.TrySetIndexRange(CurrentSession, this,
                                     range.InclusiveTo
                                         ? SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive
                                         : SetIndexRangeGrbit.RangeUpperLimit);
            }
            else if(!range.HasTo)
            {
                var keyFrom = Converters.Convert(range.From);
                Api.JetMakeKey(CurrentSession, this, keyFrom, keyFrom.Length, MakeKeyGrbit.NewKey);

                Api.TrySeek(CurrentSession, this, range.InclusiveFrom ? SeekGrbit.SeekGE : SeekGrbit.SeekGT);
            }
            else if(!range.HasFrom)
            {
                var keyTo = Converters.Convert(range.To);
                Api.JetMakeKey(CurrentSession, this, keyTo, keyTo.Length, MakeKeyGrbit.NewKey);

                Api.TrySetIndexRange(CurrentSession, this,
                                     range.InclusiveTo
                                         ? SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive
                                         : SetIndexRangeGrbit.RangeUpperLimit);
            }
        }
        
        /// <summary> Returns count of records </summary>
        public bool HasRecords()
        {
            var res = Api.TryMoveNext(CurrentSession, this);
            if (res)
                Api.TryMovePrevious(CurrentSession, this);

            return res;
        }
    }
}