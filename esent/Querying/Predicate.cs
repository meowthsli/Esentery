using System;
using System.Collections.Generic;
using Meowth.Esentery.Core;
using TTable = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> Conditions builder </summary>
    public abstract class Predicate
    {
        /// <summary> Returns cursor to records, filtered by predicated </summary>
        public ICursor GetCursor(Table table)
        {
            var gen = GetBookmarksGenerator(table);
            if(gen is ICursor)
                return (ICursor) gen;

            return new SynthesizedCursor(gen, table);
        }

        /// <summary> Generates bookmarks </summary>
        internal abstract IEnumerable<Bookmark> GetBookmarksGenerator(TTable table, bool notRewind = false);
    }
}