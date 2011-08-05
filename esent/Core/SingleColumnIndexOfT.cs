using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Index </summary>
    public sealed class SearchIndex<T> : SearchIndex
    {
        /// <summary> Creates index </summary>
        internal static SearchIndex<T> CreateSingleColumnIndex(Table table, string name, Column<T> column)
        {
            var keyDescription = string.Format("+{0}\0\0", column.ColumnName);
            Api.JetCreateIndex(table.CurrentSession, table, name, CreateIndexGrbit.IndexLazyFlush | CreateIndexGrbit.None,
                keyDescription, keyDescription.Length, 100);

            return new SearchIndex<T>(table, name, column);
        }

        /// <summary> ESET Index </summary>
        internal SearchIndex(Table table, string indexName, Column<T> column)
            : base(table, indexName, column)
        {
        }
    }
}