using System;
using System.Collections.Generic;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;
using Enumerable = System.Linq.Enumerable;
using Table = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> BETWEEN predicate </summary>
    public class Between<T> : Predicate
        where T : IComparable<T>
    {
        /// <summary> BETWEEN clause </summary>
        public Between(SingleColumnIndex<T> singleColumnIndex, T valueFrom, bool inclusiveFrom, T valueTo, bool inclusiveTo)
        {
            SearchIndex = singleColumnIndex;
            
            ValueFrom = valueFrom;
            InclusiveFrom = inclusiveFrom;
            ValueTo = valueTo;
            InclusiveTo = inclusiveTo;

            if (ValueFrom.CompareTo(ValueTo) <= 0) 
                return;

            var t1 = ValueFrom;
            ValueFrom = ValueTo;
            ValueTo = t1;

            var t2 = InclusiveFrom;
            InclusiveFrom = InclusiveTo;
            InclusiveTo = t2;
        }

        /// <summary> Returns bookmarks generator </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            var cursor = table.OpenNativeCursor(SearchIndex);

            var session = table.Database.CurrentSession;

            // seek to value
            var keyFrom = Converters.Convert(ValueFrom);
            var keyTo = Converters.Convert(ValueTo);

            Api.JetMakeKey(session, cursor, keyFrom, keyFrom.Length, MakeKeyGrbit.NewKey);
            Api.TrySeek(session, cursor,
                        InclusiveFrom
                            ? SeekGrbit.SeekGE
                            : SeekGrbit.SeekGT);
            
            Api.JetMakeKey(session, cursor, keyTo, keyTo.Length, MakeKeyGrbit.NewKey);
            Api.TrySetIndexRange(session, cursor,
                                 InclusiveTo
                                     ? (SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive)
                                     : SetIndexRangeGrbit.RangeUpperLimit);
            // step back
            if (!doNotRewind)
                cursor.StepBack();

            return cursor;
        }

        public SingleColumnIndex<T> SearchIndex { get; private set; }
        public T ValueFrom { get; private set; }
        public bool InclusiveFrom { get; private set; }
        public T ValueTo { get; private set; }
        public bool InclusiveTo { get; private set; }
    }
}