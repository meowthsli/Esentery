using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Table column (typed version) </summary>
    public sealed class Column<T> : Column
    {
        /// <summary> Column </summary>
        internal Column(Table table, string columnName, ColumnOptions options, JET_COLUMNID handle)
            : base(table, columnName, typeof(T), options, handle)
        {
        }
    }
}