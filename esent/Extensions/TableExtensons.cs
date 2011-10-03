using System;
using System.Linq;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Extensions
{
    /// <summary> Returns typed column by name </summary>
    public static class TableExtensons
    {
        /// <summary> Returns typed column </summary>
        public static Column GetColumn<T>(this Table table, string name)
        {
            var c = GetColumn(table, name);
            if(c.ColumnType != typeof(T))
                throw new ArgumentException("Column has another type");

            return c;
        }

        /// <summary> Removes all rows from table </summary>
        public static void RemoveAllRows(this Table table)
        {
            // TODO:
        }

        /// <summary> Returns untyped column </summary>
        public static Column GetColumn(this Table table, string columnName)
        {
            var col = table.GetColumns().FirstOrDefault(c => c.ColumnName == columnName);
            if (col == null)
                throw new ArgumentException("Column with such name and type not found");

            return col;
        }

        /// <summary> Returns untyped index </summary>
        public static ISearchIndex GetSearchIndexOfColumn(this Table table, string columnName)
        {
            var column = table.GetColumn(columnName);
            var indices = table.GetIndexes()
                .OfType<ISearchIndex>()
                .Where(i => i.Column == column)
                .ToList();

            if (indices.Count() == 0)
                throw new ArgumentException("There is no search index on given column");
            if (indices.Count() > 1)
                throw new ArgumentException("There is too many search indices on given column");

            return indices.First();
        }

        /// <summary> Counts records on table </summary>
        public static long GetRecordCount(this Table table)
        {
            var count = 0;
            using (var cursor = table.OpenPrimaryCursor())
                while (cursor.MoveNext())
                    count++;

            return count;
        }

        /// <summary> Check is any records presents </summary>
        public static bool HasRecords(this Table table)
        {
            using (var cursor = table.OpenPrimaryCursor())
                return cursor.MoveNext();
        }
    }
}