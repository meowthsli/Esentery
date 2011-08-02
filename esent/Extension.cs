using System;
using Meowth.Esentery.Core;
using Meowth.Esentery.Querying;
using System.Linq;
namespace Meowth.Esentery.Additions
{
    
    /// <summary> Extensions </summary>
    public static class CursorExtension
    {
        /// <summary> Appends row </summary>
        public static void AddRow<T>(this NativeCursor<T> nativeReadonlyCursor, string[] columns, string[] values)
            where T : IComparable<T>
        {
            var cols = nativeReadonlyCursor.Table.GetColumns().ToDictionary(c => c.ColumnName, c => c);
            // Insert a record. This table only has one column but a table can have slightly over 64,000
            // columns defined. Unless a column is declared as fixed or variable it won't take any space
            // in the record unless set. An individual record can have several hundred columns set at one
            // time, the exact number depends on the database page size and the contents of the columns.)
            using (var insertion = nativeReadonlyCursor.AddRow())
            {
                for (var i = 0; i < columns.Length; ++i)
                    insertion.SetField(cols[columns[i]], values[i]);

                insertion.Save();
            }
        }

        /// <summary> Opens Index by name </summary>
        public static ICursor OpenPrimaryCursor(this Table table)
        {
            return table.OpenNativeCursor((SingleColumnIndex<string>)null);
        }

        /// <summary> Opens cursor, building predicate </summary>
        public static ICursor OpenCursor(this Table table, string predicate)
        {
            return null;
        }
    }

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

    public static class RowModificationExtensions
    {
        /// <summary> Sets up string </summary>
        public static void SetString(this RowModification mod, string colName, string value)
        {
            var cols = mod.Table.GetColumns().ToDictionary(c => c.ColumnName, c => c);
            mod.SetField(cols[colName],  value);
        }
    }
}