using System.Collections.Generic;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;
using Table = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> Finder by beginning of string </summary>
    public class StartsWith : Predicate
    {
        internal SearchIndex<string> SearchIndex { get; private set; }
        internal string Val { get; private set; }

        public StartsWith(SearchIndex<string> searchIndex, string val)
        {
            SearchIndex = searchIndex;
            Val = val;
        }

        /// <summary> Before seek </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind)
        {
            var cursor = table.OpenNativeCursor(SearchIndex);

            var session = cursor.Table.Database.CurrentSession;

            // seek to value
            var key = Converters.Convert(Val);

            Api.JetMakeKey(session, cursor, key, key.Length, MakeKeyGrbit.NewKey);
            Api.TrySeek(session, cursor, SeekGrbit.SeekGE);

            Api.JetMakeKey(session, cursor, key, key.Length, MakeKeyGrbit.NewKey | MakeKeyGrbit.SubStrLimit);
            Api.TrySetIndexRange(session, cursor, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive);
            
            // step back
            if(!doNotRewind)
                cursor.StepBack();

            return cursor;
        }
    }
}