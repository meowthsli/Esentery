using System;
using System.Linq;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Querying
{
    /// <summary> Finder by string equality </summary>
    /// <remarks> Implements simple scheme, with natural cursor</remarks>
    public class Eq<T> : RangePredicate<T>
        where T : IComparable<T>
    {
        /// <summary> EQ clause </summary>
        public Eq(ISearchIndex searchIndex, T val)
            : base(searchIndex, val)
        {
        }

        /// <summary> Creates range for equality </summary>
        protected override Range<T> CreateRange()
        {
            return Range<T>.CreateEquality(Val);
        }
    }
}