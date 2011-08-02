using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Column (untyped version) </summary>
    public class Column : IHasJetHandle<JET_COLUMNID>, IColumn
    {
        public Table Table { get; private set; }
        public string ColumnName { get; private set; }
        public Type ColumnType { get; private set; }
        public ColumnOptions Options { get; private set; }

        /// <summary> Column </summary>
        internal Column(Table table, string columnName, Type columnType, ColumnOptions options, JET_COLUMNID handle)
        {
            Table = table;
            ColumnName = columnName;
            ColumnType = columnType;
            Options = options;
            Handle = handle;
        }

        /// <summary> Column native handle </summary>
        public JET_COLUMNID Handle { get; private set; }
    }
}