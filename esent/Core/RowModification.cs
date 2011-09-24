using System;
using System.Text;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Insertion to database </summary>
    public class RowModification : IDisposable, IRowAccess
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
            Updated = true;
        }
        
        public void Dispose()
        {
            if(!Updated)
                Api.JetPrepareUpdate(CurrentSession, Cursor, JET_prep.Cancel);
        }

        /// <summary> Was it update? </summary>
        protected bool Updated { get; set; }

        public Session CurrentSession { get { return Table.Database.CurrentSession; } }

        /// <summary> Returns value at column </summary>
        public object GetValue(Column column)
        {
            return Converters.GetGetter(column.ColumnType)
                (CurrentSession, Cursor, column);
        }

        /// <summary> Returns stream at column </summary>
        public ColumnStream OpenStream(Column column)
        {
            return new ColumnStream(CurrentSession, Cursor, column);
        }

        /// <summary> Sets new value </summary>
        public void SetValue(Column column, object value)
        {
            Converters.GetSetter(column.ColumnType)
                (CurrentSession, Cursor, column, value);
        }
    }
}