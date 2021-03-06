using System;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Column (untyped version) </summary>
    public class Column : IHasJetHandle<JET_COLUMNID>, IColumn
    {
        /// <summary> Table </summary>
        public Table Table { get; private set; }
        /// <summary> Column </summary>
        public string ColumnName { get; private set; }
        public Type ColumnType { get; private set; }

        public ColumnOptions Options { get; private set; }

        /// <summary> Column </summary>
        internal Column(Table table, 
            string columnName,
            ColumnOptions options, 
            JET_COLUMNID handle)
        {

            Table = table;
            ColumnName = columnName;
            ColumnType = options.ColumnType;
            Options = options;
            Handle = handle;
        }

        /// <summary> Column native handle </summary>
        public JET_COLUMNID Handle { get; private set; }

        /// <summary> ���������� � ���������� ���� </summary>
        public static implicit operator JET_COLUMNID(Column column) { return column.Handle; }
    }
}