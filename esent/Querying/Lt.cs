using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> LT finder </summary>
    public class Lt<T> : RangePredicate<T>
        where T : IComparable<T>
    {
        /// <summary> LE clause </summary>
        public Lt(ISearchIndex index, T val)
            : base(index, val)
        {
        }

        /// <summary> Creates proper range </summary>
        protected override Range<T> CreateRange()
        {
            return new Range<T> {To = Val, InclusiveTo = false};
        }
    }
}