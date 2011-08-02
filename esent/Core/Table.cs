using System;
using System.Collections.Generic;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;
using System.Linq;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Table (DML) </summary>
    public class Table : HasJetHandleBase<JET_TABLEID>
    {
        /// <summary> Creates table from scratch </summary>
        internal Table(Database db, string tableName, JET_TABLEID handle)
        {
            Database = db;
            TableName = tableName;
            JetHandle = handle;
        }

        /// <summary> Table creation </summary>
        internal static Table Create(Database db, string tableName)
        {
            JET_TABLEID handle;
            Api.JetCreateTable(db.CurrentSession, db, tableName, 0, 100, out handle);
            return new Table(db, tableName, handle);
        }

        internal static Table Open(Database db, string tableName)
        {
            JET_TABLEID handle;
            Api.JetOpenTable(db.CurrentSession, db, tableName, new byte[] { 0 }, 0, OpenTableGrbit.None, out handle);
            return new Table(db, tableName, handle);
        }

        protected override void Dispose(bool dispose)
        {
            if (!Database.CurrentSession.Disposed)
            {
                Api.JetCloseTable(CurrentSession, this);
            }
        }

        public override Session CurrentSession
        {
            get { return Database.CurrentSession; }
        }

        /// <summary> Creates column </summary>
        public Column<T> AddColumn<T>(string columnName, ColumnOptions options)
        {
            JET_COLUMNID column;
            Api.JetAddColumn(CurrentSession, this, columnName, options.ColumnDef, null, 0, out column);
            return new Column<T>(this, columnName, options, column);
        }

        /// <summary> Creates Index </summary>
        public SingleColumnIndex<T> AddSearchIndex<T>(string indexName, Column<T> column)
            where T : IComparable<T>
        {
            if(column.Table != this)
                throw new ArgumentException("Column doesn't belong to this table");

            return SingleColumnIndex<T>.CreateSingleColumnIndex(this, indexName, column);
        }
        
        public NativeCursor<T> OpenNativeCursor<T>(SingleColumnIndex<T> singleColumnIndex)
            where T : IComparable<T>
        {
            return new NativeCursor<T>(this, singleColumnIndex);
        }

        public ICursor OpenCursor(Predicate predicate)
        {
            return predicate.GetCursor(this);
        }

        public SingleColumnIndex<T> GetIndex<T>(string indexName)
            where T : IComparable<T>
        {
            // TODO: actuall return real index
            return new SingleColumnIndex<T>(this, indexName, null);
        }

        /// <summary> Returns column id </summary>
        internal JET_COLUMNID GetColumnId(string columnName)
        {
            return GetColumns()
                .ToDictionary(c => c.ColumnName, c => c)[columnName]
                .Handle;
        }

        public Database Database { get; private set;}
        public string TableName { get; set; }

        /// <summary> Column list </summary>
        /// TODO: sync with database and refresh cache as needed
        public ICollection<Column> GetColumns()
        {
            return Api.GetTableColumns(CurrentSession, this).Select(
                c => new Column(this, c.Name, typeof(string),
                                new ColumnOptions(
                                    new JET_COLUMNDEF()), c.Columnid)).ToArray();
        }
    }
}