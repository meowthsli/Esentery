using System;
using System.Collections.Generic;
using System.Linq;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> OR claus </summary>
    public class Or : ComplexPredicate
    {
        public Or(params Predicate[] predicates) : base(predicates)
        {
        }
        
        /// <summary> Returns cursor over OR-connected cursors </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            var cursors = Subpredicates.Select(s => s.GetBookmarksGenerator(table, doNotRewind));

            // simple way
            // complex way must be built on the state machine
            var list = cursors.SelectMany(s => s).ToList();
            foreach (var disposableCursor in cursors.OfType<IDisposable>())
                disposableCursor.Dispose();

            return list;
        }
    }
}