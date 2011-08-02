using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    public interface IColumn
    {
        Table Table { get; }
        string ColumnName { get; }
        ColumnOptions Options { get; }
    }

    /// <summary> ESENT Table column </summary>
    public sealed class Column<T> : Column
    {
        /// <summary> Column </summary>
        internal Column(Table table, string columnName, ColumnOptions options, JET_COLUMNID handle)
            : base(table, columnName, options, handle)
        {
        }
    }

    /// <summary> Typed column </summary>
    public class Column : IHasJetHandle<JET_COLUMNID>, IColumn
    {
        public Table Table { get; private set; }
        public string ColumnName { get; private set; }
        public ColumnOptions Options { get; private set; }

        /// <summary> Column </summary>
        internal Column(Table table, string columnName, ColumnOptions options, JET_COLUMNID handle)
        {
            Table = table;
            ColumnName = columnName;
            Options = options;
            Handle = handle;
        }

        /// <summary> Column native handle </summary>
        public JET_COLUMNID Handle { get; private set; }
    }

    /// <summary> Column options </summary>
    public class ColumnOptions
    {
        public JET_COLUMNDEF ColumnDef { get; private set; }

        /// <summary> Creation </summary>
        public ColumnOptions(JET_COLUMNDEF columndef)
        {
            ColumnDef = columndef;
        }
    }
}