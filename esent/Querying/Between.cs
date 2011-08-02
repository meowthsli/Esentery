using System;
using System.Collections.Generic;
using System.Linq;
using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;
using Enumerable = System.Linq.Enumerable;
using Table = Meowth.Esentery.Core.Table;

namespace Meowth.Esentery.Querying
{
    /// <summary> BETWEEN predicate </summary>
    public class Between<T> : Predicate
        where T : IComparable<T>
    {
        /// <summary> BETWEEN clause </summary>
        public Between(SingleColumnIndex<T> singleColumnIndex, T valueFrom, bool inclusiveFrom, T valueTo, bool inclusiveTo)
        {
            SearchIndex = singleColumnIndex;
            
            ValueFrom = valueFrom;
            InclusiveFrom = inclusiveFrom;
            ValueTo = valueTo;
            InclusiveTo = inclusiveTo;

            if (ValueFrom.CompareTo(ValueTo) <= 0) 
                return;

            var t1 = ValueFrom;
            ValueFrom = ValueTo;
            ValueTo = t1;

            var t2 = InclusiveFrom;
            InclusiveFrom = InclusiveTo;
            InclusiveTo = t2;
        }

        /// <summary> Returns bookmarks generator </summary>
        internal override IEnumerable<Bookmark> GetBookmarksGenerator(Table table, bool doNotRewind = false)
        {
            var cursor = table.OpenNativeCursor(SearchIndex);

            var session = table.Database.CurrentSession;

            // seek to value
            var keyFrom = Converters.Convert(ValueFrom);
            var keyTo = Converters.Convert(ValueTo);

            Api.JetMakeKey(session, cursor, keyFrom, keyFrom.Length, MakeKeyGrbit.NewKey);
            Api.TrySeek(session, cursor,
                        InclusiveFrom
                            ? SeekGrbit.SeekGE
                            : SeekGrbit.SeekGT);
            
            Api.JetMakeKey(session, cursor, keyTo, keyTo.Length, MakeKeyGrbit.NewKey);
            Api.TrySetIndexRange(session, cursor,
                                 InclusiveTo
                                     ? (SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive)
                                     : SetIndexRangeGrbit.RangeUpperLimit);
            // step back
            if (!doNotRewind)
                cursor.StepBack();

            return cursor;
        }

        public SingleColumnIndex<T> SearchIndex { get; private set; }
        public T ValueFrom { get; private set; }
        public bool InclusiveFrom { get; private set; }
        public T ValueTo { get; private set; }
        public bool InclusiveTo { get; private set; }
    }

    /// <summary> All byte converters </summary>
    /// <remarks> Переписать на LCG </remarks>
    static class Converters
    {
        /// <summary> Converts value </summary>
        public static byte[] Convert<T>(T value)
        {
            return GetConverter<T>()(value);
        }
        
        /// <summary> Returns suitable converter </summary>
        public static Func<T, byte[]> GetConverter<T>()
        {
            if (typeof(T) == typeof(string))
                return x => ConvertFromString(x as string);
            
            if (typeof(T) == typeof(int))
                return x => ConvertFromInt32((int) GetBoxed(x));

            if (typeof(T) == typeof(uint))
                return x => ConvertFromUInt32((uint)GetBoxed(x));

            if (typeof(T) == typeof(long))
                return x => ConvertFromInt64((long)GetBoxed(x));

            if (typeof(T) == typeof(ulong))
                return x => ConvertFromUInt64((ulong)GetBoxed(x));

            throw new NotSupportedException("This type is not supported");
        }

        private static object GetBoxed<T>(T t)
        {
            object o = t;
            return o;
        }

        /// <summary> From string </summary>
        private static byte[] ConvertFromString(string value)
        {
            return value.ToCharArray().SelectMany(BitConverter.GetBytes).ToArray();
        }

        /// <summary> From string </summary>
        private static byte[] ConvertFromInt32(int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary> From string </summary>
        private static byte[] ConvertFromUInt32(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary> From string </summary>
        private static byte[] ConvertFromInt64(long value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary> From string </summary>
        private static byte[] ConvertFromUInt64(ulong value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}