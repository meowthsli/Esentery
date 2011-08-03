using System;
using System.Text;
using Meowth.Esentery.Querying;
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
            Api.JetPrepareUpdate(CurrentSession, cursor, preparationType);
        }

        internal HasJetHandleBase<JET_TABLEID> Cursor { get; set; }
       
        /// <summary> Owner </summary>
        public Table Table { get; private set; }

        /// <summary> Saving </summary>
        public void Save()
        {
            Api.JetUpdate(CurrentSession, Cursor);
        }

        /// <summary> Field set up without types </summary>
        public void SetField(Column column, object value)
        {
            if(column.ColumnType != value.GetType())
                throw new ArgumentException("Column has another type");

            var data = Converters.Convert(column.ColumnType, value);
            Api.JetSetColumn(CurrentSession, Cursor, column.Handle, data, data.Length, SetColumnGrbit.None, 
                new JET_SETINFO());
        }

        public void Dispose() { }

        public Session CurrentSession { get { return Table.Database.CurrentSession; } }
    }
}