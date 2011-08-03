using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT untyped index </summary>
    public class SingleColumnIndex : ISearchIndex, IHasJetHandle<JET_INDEXID>
    {
        /// <summary> ESET Index </summary>
        internal SingleColumnIndex(Table table, string indexName, Column column)
        {
            Table = table;
            Name = indexName;
            Column = column;
        }

        public Table Table { get; private set; }

        /// <summary> Index name </summary>
        public string Name { get; private set; }

        /// <summary> Column of this index </summary>
        public Column Column { get; private set; }

        /// <summary> Return type of T </summary>
        public Type ColumnType { get { return Column.ColumnType; } }

        #region

        /// <summary> Currnt session holder </summary>
        /// <summary> Currnt session holder </summary>
        internal Session CurrentSession { get { return Table.CurrentSession; } }

        #endregion

        /// <summary> Handle of </summary>
        public JET_INDEXID Handle { get; private set; }
    }
}