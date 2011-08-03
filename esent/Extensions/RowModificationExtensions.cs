using Meowth.Esentery.Core;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Extensions
{
    /// <summary> Extensions </summary>
    public static class RowModificationExtensions
    {
        /// <summary> Fills field with typed value </summary>
        public static void SetField<T>(this RowModification rowModification, Column<T> column, string value)
        {
            Api.SetColumn(rowModification.CurrentSession, rowModification.Cursor, column.Handle, value, 
                column.Options.Encoding);
        }
    }
}