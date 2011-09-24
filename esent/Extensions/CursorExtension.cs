using System;
using Meowth.Esentery.Core;
using System.Linq;
namespace Meowth.Esentery.Extensions
{
    /// <summary> Extensions </summary>
    public static class CursorExtension
    {
        /// <summary> Appends row </summary>
        public static void AddRow<T>(this NativeCursor nativeReadonlyCursor, string[] columns, string[] values)
            where T : IComparable<T>
        {
            using (var insertion = nativeReadonlyCursor.AddRow())
            {
                for (var i = 0; i < columns.Length; ++i)
                    insertion.SetValue(nativeReadonlyCursor.Table.GetColumn(columns[i]), values[i]);

                insertion.Save();
            }
        }

        /// <summary> Opens Index by name </summary>
        public static ICursor OpenPrimaryCursor(this Table table)
        {
            return table.OpenNativeCursor(null);
        }

        /// <summary> Returns value of column </summary>
        public static object GetValue(this ICursor cursor, string columnName)
        {
            return cursor.GetValue(cursor.Table.GetColumn(columnName));
        }
    }
}