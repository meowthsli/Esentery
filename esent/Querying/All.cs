using System;
using System.Collections.Generic;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> All records </summary>
    public class All<T> : Predicate
        where T : IComparable<T>
    {
        /// <summary> Opens cursor on default Index </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool notRewind)
        {
            return table.OpenNativeCursor(null);
        }
    }
}