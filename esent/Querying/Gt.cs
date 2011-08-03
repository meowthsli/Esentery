using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Querying
{
    /// <summary> GE finder </summary>
    public class Gt<T> : RangePredicate<T>
        where T : IComparable<T>
    {
        /// <summary> GE clause </summary>
        public Gt(SearchIndex<T> index, T val)
            : base(index, val)
        {
        }

        /// <summary> Creates proper range </summary>
        protected override Range<T> CreateRange()
        {
            return new Range<T> { From = Val, InclusiveTo = false };
        }
    }
}