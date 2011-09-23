using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT untyped single column (search) index </summary>
    public class SearchIndex : ISearchIndex, IHasJetHandle<JET_INDEXID>
    {
        /// <summary> ESET Index </summary>
        internal SearchIndex(Table table, string indexName, Column column)
        {
            Table = table;
            Name = indexName;
            Column = column;
        }

        /// <summary> Parent table </summary>
        public Table Table { get; private set; }

        /// <summary> Index name </summary>
        public string Name { get; private set; }

        /// <summary> Column of this index </summary>
        public Column Column { get; private set; }

        /// <summary> Return type of T </summary>
        public Type ColumnType { get { return Column.ColumnType; } }


        #region

        /// <summary> Currnt session holder </summary>
        internal Session CurrentSession { get { return Table.CurrentSession; } }

        /// <summary> Handle of </summary>
        public JET_INDEXID Handle { get; private set; }

        #endregion

        /// <summary> Creates index </summary>
        internal static SearchIndex CreateSingleColumnIndex(Table table, string name, Column column)
        {
            var keyDescription = string.Format("+{0}\0\0", column.ColumnName);
            Api.JetCreateIndex(table.CurrentSession, table, name, CreateIndexGrbit.IndexLazyFlush | CreateIndexGrbit.None,
                keyDescription, keyDescription.Length, 100);

            return new SearchIndex(table, name, column);
        }
    }
}