using System;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Insertion to database </summary>
    public class RowModification : IDisposable
    {
        /// <summary> Prepares insertion </summary>
        internal RowModification(Table table, HasJetHandleBase<JET_TABLEID> cursor, JET_prep preparationType)
        {
            Table = table;
            Cursor = cursor;
            Api.JetPrepareUpdate(Session, cursor, preparationType);
        }

        private HasJetHandleBase<JET_TABLEID> Cursor { get; set; }
       
        /// <summary> Owner </summary>
        public Table Table { get; private set; }

        /// <summary> Saving </summary>
        public void Save()
        {
            Api.JetUpdate(Session, Cursor);
        }

        /// <summary> Fills field with value </summary>
        public void SetField<T>(Column<T> column, string value)
        {
            Api.SetColumn(Session, Cursor, column.Handle, value, Encoding.Unicode);
        }

        internal void SetField(Column column, string value)
        {
            // Conversion to value and set up
            Api.SetColumn(Session, Cursor, column.Handle, value, Encoding.Unicode);
        }

        public void Dispose() { }

        private Session Session { get { return Table.Database.CurrentSession; } }
    }
}