using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Index </summary>
    public sealed class SingleColumnIndex<T> : ISearchIndex, IHasJetHandle<JET_INDEXID>
        where T : IComparable<T>
    {
        /// <summary> Creates index </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal static SingleColumnIndex<T> CreateSingleColumnIndex(Table table, string name, Column<T> column)
        {
            var keyDescription = string.Format("+{0}\0\0", column.ColumnName);
            Api.JetCreateIndex(table.CurrentSession, table, name, CreateIndexGrbit.IndexLazyFlush | CreateIndexGrbit.None,
                keyDescription, keyDescription.Length, 100);

            return new SingleColumnIndex<T>(table, name, column);
        }

        /// <summary> ESET Index </summary>
        internal SingleColumnIndex(Table table, string indexName, Column<T> column)
        {
            Table = table;
            Name = indexName;
            Column = column;
            ColumnType = typeof (T);
        }

        public Table Table { get; private set; }

        /// <summary> Index name </summary>
        public string Name { get; private set; }

        /// <summary> Column of this index </summary>
        public Column<T> Column { get; private set; }

        /// <summary> Return type of T </summary>
        public Type ColumnType { get; private set; }

        #region

        /// <summary> Currnt session holder </summary>
        /// <summary> Currnt session holder </summary>
        internal Session CurrentSession { get { return Table.CurrentSession; }}

        #endregion

        // TODO:
        public JET_INDEXID Handle { get; private set; }
    }
}