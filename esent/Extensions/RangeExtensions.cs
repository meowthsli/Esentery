using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Extensions
{
    /// <summary> Range extensions methods </summary>
    public static class RangeExtensions
    {
        /// <summary> Checks whether this range normal or not </summary>
        public static bool IsNormal<T>(this Range<T> range)
            where T : IComparable<T>
        {
            return range.From.CompareTo(range.To) <= 0;
        }

        /// <summary> Normalizes ranges </summary>
        public static void Normalize<T>(this Range<T> range)
            where T : IComparable<T>
        {
            if(!range.IsNormal())
            {
                var t = range.From;
                range.From = range.To;
                range.To = t;

                var t2 = range.InclusiveFrom;
                range.InclusiveFrom = range.InclusiveTo;
                range.InclusiveTo = t2;

                var t3 = range.HasFrom;
                range.HasFrom = range.HasTo;
                range.HasTo = t3;
            }
        }
    }
}