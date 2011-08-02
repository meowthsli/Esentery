using System;
using System.Collections.Generic;
using System.Linq;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;
using Table = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> AND clause </summary>
    public class And : ComplexPredicate
    {
        /// <summary> AND clause </summary>
        /// <param name="predicates"></param>
        public And(params Predicate[] predicates) : base(predicates) {}
        
        /// <summary> Returns sequence of bookmarks </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            // retrieve all subcursors
            var allSubcursors = Subpredicates
                .Select(s => s.GetBookmarksGenerator(table, true))
                .ToArray();

            // can we oprimize it natively?
            if(allSubcursors.Count() > 0 && allSubcursors.All(t => t is INativeCursor))
            {
                // Optimized searching by index intersection
                var cursors = allSubcursors.OfType<INativeCursor>();
                try
                {
                    return Api.IntersectIndexes(
                        table.Database.CurrentSession, 
                        cursors.Select(s => s.CursorHandle).ToArray())
                        .Select(k => new Bookmark(k)).ToList();
                }
                catch(EsentNoCurrentRecordException ex)
                {
                    return Enumerable.Empty<Bookmark>();
                }
                finally
                {
                    foreach (var cursor in cursors.OfType<IDisposable>())
                        cursor.Dispose();
                }
            }
            
            // can't optimize by intersection, so do intersection of all
            // in memory
            return allSubcursors.Aggregate((acc, left) => acc.Intersect(left)).ToList();
        }
    }
}