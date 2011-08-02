using System;
using System.Collections.Generic;
using System.Linq;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;
using Table = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> Finder by string equality </summary>
    /// <remarks> Implements simple scheme, with natural cursor</remarks>
    public class Eq<T> : Predicate
        where T : IComparable<T>
    {
        /// <summary> EQ clause </summary>
        public Eq(SingleColumnIndex<T> singleColumnIndex, T val)
        {
            SearchIndex = singleColumnIndex;
            Val = val;
        }

        /// <summary> Returns bookmarks generator </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            var cursor = table.OpenNativeCursor(SearchIndex);
            cursor.Restrict(Range<T>.CreateEquality(Val));

            // step back
            if(!doNotRewind)
                cursor.StepBack();

            return cursor;
        }

        /// <summary> This search index </summary>
        public SingleColumnIndex<T> SearchIndex { get; private set; }

        /// <summary> Value to compare </summary>
        public T Val { get; private set; }
    }

    /// <summary> All records </summary>
    public class All<T> : Predicate
        where T : IComparable<T>
    {
        /// <summary> Opens cursor on default Index </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool notRewind)
        {
            return table.OpenNativeCursor<T>(null);
        }
    }
}