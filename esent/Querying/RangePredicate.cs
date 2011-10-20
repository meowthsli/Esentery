using System;
using System.Collections.Generic;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> LE finder </summary>
    public abstract class RangePredicate<T> : Predicate// where T : IComparable<T>
    {
        /// <summary> LE clause </summary>
        protected RangePredicate(ISearchIndex searchIndex, T val)
        {
            SearchIndex = searchIndex;
            Val = val;
        }

        /// <summary> Returns bookmarks generator </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            var cursor = table.OpenNativeCursor(SearchIndex);
            cursor.Restrict(CreateRange());

            // step back
            if (!doNotRewind)
                cursor.StepBack();

            return cursor;
        }

        /// <summary> Proper range creation </summary>
        /// <returns></returns>
        protected abstract Range<T> CreateRange();

        /// <summary> This search index </summary>
        public ISearchIndex SearchIndex { get; private set; }

        /// <summary> Value to compare </summary>
        public T Val { get; private set; }
    }
}