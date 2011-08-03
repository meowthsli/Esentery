using System;
using System.Linq;
using Meowth.Esentery.Core;

namespace Meowth.Esentery.Extensions
{
    /// <summary> Returns typed column by name </summary>
    public static class TableExtensons
    {
        /// <summary> Returns typed column </summary>
        public static Column<T> GetColumn<T>(this Table table, string name)
        {
            var c = GetColumn(table, name);
            if(c.ColumnType != typeof(T))
                throw new ArgumentException("Column has another type");

            return (Column<T>) c;
        }

        /// <summary> Returns untyped column </summary>
        public static Column GetColumn(this Table table, string name)
        {
            var cols = table.GetColumns().Where(c => c.ColumnName == name);
            if (cols.Count() == 0)
                throw new ArgumentException("Column with such name and type not found");

            return cols.First();
        }
    }
}