using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Read-write cursor </summary>
    public interface ICursor : IReadonlyCursor
    {
        /// <summary> Creates insert/update buffer </summary>
        RowModification AddRow();

        /// <summary> Edits row </summary>
        RowModification EditRow();

        /// <summary> Removes row </summary>
        void DeleteRow();

        /// <summary> Returns value at column </summary>
        object GetValue(Column column);

        /// <summary> Returns stream at column </summary>
        ColumnStream OpenStream(Column column);

        /// <summary> Cursor's table </summary>
        Table Table { get; }
    }

    /// <summary> Access to row data (get set values by index
    /// or by name) </summary>
    public interface IRowAccess
    {
        /// <summary> Returns value at column </summary>
        object GetValue(Column column);

        /// <summary> Sets new value </summary>
        void SetValue(Column column, object value);

        /// <summary> Returns stream at column </summary>
        ColumnStream OpenStream(Column column);
    }
}