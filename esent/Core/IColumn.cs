using System;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Table column </summary>
    public interface IColumn
    {
        /// <summary> Table of column </summary>
        Table Table { get; }

        /// <summary> Name of column </summary>
        string ColumnName { get; }
        
        /// <summary> Column type </summary>
        Type ColumnType { get; }

        /// <summary> Options of column </summary>
        ColumnOptions Options { get; }
    }
}