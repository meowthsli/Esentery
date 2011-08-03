using System;

namespace Meowth.Esentery.Core
{
    /// <summary> Effective range for the restriction </summary>
    public sealed class Range<T>
        where T : IComparable<T>
    {
        public T From { get; internal set;}
        public bool HasFrom { get; internal set; }
        public T To { get; internal set; }
        public bool HasTo { get; internal set; }

        public bool InclusiveFrom { get; internal set; }
        public bool InclusiveTo { get; internal set; }

        public static Range<T> CreateEquality(T val)
        {
            return new Range<T>
                       {
                           From = val,
                           HasFrom = true,
                           InclusiveFrom = true,
                           To = val,
                           HasTo = true,
                           InclusiveTo = true
                       };
        }

        /// <summary> </summary>
        public static Range<U> CreateRange<U>(U? from, U? to, 
            bool inclusiveFrom = true, 
            bool inclusiveTo = true)

            where U : struct, IComparable<U>
        {
            return new Range<U>
                       {
                           From = from.HasValue ? from.Value : default(U),
                           HasFrom = from.HasValue,
                           InclusiveFrom = inclusiveFrom,
                           To = to.HasValue ? to.Value : default(U),
                           HasTo = to.HasValue,
                           InclusiveTo = inclusiveTo
                       };
        }

        /// <summary> Creates new range </summary>
        public static Range<T> CreateRange(T from, T to,
            bool inclusiveFrom = true,
            bool inclusiveTo = true)
        {
            return new Range<T>
            {
                From = from,
                HasFrom = true,
                InclusiveFrom = inclusiveFrom,
                To = to,
                HasTo = true,
                InclusiveTo = inclusiveTo
            };
        }

        /// <summary> Type of range </summary>
        public Type ParametrizationType { get { return typeof (T); }}
    }
}