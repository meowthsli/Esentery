using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> LE finder </summary>
    public class Le<T> : RangePredicate<T>
        where T : IComparable<T>
    {
        /// <summary> LE clause </summary>
        public Le(SingleColumnIndex<T> index, T val)
            : base(index, val)
        {
        }

        /// <summary> Creates proper range </summary>
        protected override Range<T> CreateRange()
        {
            return new Range<T> { To = Val, InclusiveTo = true };
        }
    }
}