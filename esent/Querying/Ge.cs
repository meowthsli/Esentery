using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> GE finder </summary>
    public class Ge<T> : RangePredicate<T>
        where T : IComparable<T>
    {
        /// <summary> GE clause </summary>
        public Ge(SearchIndex<T> index, T val)
            : base(index, val)
        {
        }

        /// <summary> Creates proper range </summary>
        protected override Range<T> CreateRange()
        {
            return new Range<T> { From = Val, InclusiveTo = true };
        }
    }
}