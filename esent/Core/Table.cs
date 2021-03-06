using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;
using System.Linq;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Table (DML) </summary>
    public class Table : HasJetHandleBase<JET_TABLEID>
    {
        /// <summary> Creates column </summary>
        public Column AddColumn<T>(string columnName, ColumnOptions options)
        {
            JET_COLUMNID column;

            var typedOptions = options.OfType<T>();
            Api.JetAddColumn(CurrentSession, this, columnName, typedOptions.GetColumnDef(), null, 0, out column);
            var newColumns = new Column(this, columnName, typedOptions, column);
            _columns.Add(newColumns);
            return newColumns;
        }

        /// <summary> Creates column </summary>
        public Column AddColumn(string columnName, ColumnOptions options)
        {
            JET_COLUMNID column;

            Api.JetAddColumn(CurrentSession, this, columnName, options.GetColumnDef(), null, 0, out column);
            var newColumns = new Column(this, columnName, options, column);
            _columns.Add(newColumns);
            return newColumns;
        }

        /// <summary> Creates Index </summary>
        public SearchIndex AddSearchIndex(string indexName, Column column)
        {
            if (column.Table != this)
                throw new ArgumentException("Column doesn't belong to this table");

            if(_indexes.Any(i => i.Column == column))
                throw new ArgumentException("Index on specified column already exists");

            var idx = SearchIndex.CreateSingleColumnIndex(this, indexName, column);
            _indexes.Add(idx);
            return idx;
        }

        /// <summary> Opens cursor for the table </summary>
        public ICursor OpenCursor(Predicate predicate)
        {
            return predicate.GetCursor(this);
        }

        /// <summary> Returns index by name </summary>
        public SearchIndex GetIndex<T>(string indexName)
            where T : IComparable<T>
        {
            var index = _indexes.FirstOrDefault(i => i.Name == indexName);
            if(index == null)
                throw new ArgumentException("No index found by given name");

            if(index.Column.ColumnType != typeof(T))
                throw new ArgumentException(string.Format("Index type differs from specified; expected '{0}', found '{1}'", 
                    typeof(T).Name, index.Column.ColumnType.Name));

            return (SearchIndex) index;
        }
        
        /// <summary> Returns copy of column list </summary>
        public ICollection<Column> GetColumns()
        {
            return new List<Column>(_columns);
        }

        /// <summary> Returns copy of column list </summary>
        public ICollection<IIndex> GetIndexes()
        {
            return new List<IIndex>(_indexes);
        }
        
        /// <summary> Current database </summary>
        public Database Database { get; private set; }

        /// <summary> Name of table </summary>
        public string TableName { get; private set; }

        /// <summary> Reference to current session </summary>
        public override Session CurrentSession { get { return Database.CurrentSession; }}

        /// <summary> Creates table object over handle </summary>
        internal Table(Database db, string tableName, JET_TABLEID handle, bool initial = false)
        {
            Database = db;
            TableName = tableName;
            JetHandle = handle;
            if (initial)
            {
                LoadColumns();
                LoadIndexes();
            }
        }

        /// <summary> Creates new table </summary>
        internal static Table Create(Database db, string tableName)
        {
            JET_TABLEID handle;
            Api.JetCreateTable(db.CurrentSession, db, tableName, 0, 100, out handle);
            return new Table(db, tableName, handle, true);
        }

        /// <summary> Opens existing table </summary>
        internal static Table Open(Database db, string tableName)
        {
            JET_TABLEID handle;
            Api.JetOpenTable(db.CurrentSession, db, tableName, new byte[] { 0 }, 0, OpenTableGrbit.None, out handle);
            return new Table(db, tableName, handle, true);
        }

        /// <summary> Opens native cursor over this table </summary>
        /// <param name="searchIndex">Can be null</param>
        internal NativeCursor OpenNativeCursor(ISearchIndex searchIndex)
        {
            return new NativeCursor(this, searchIndex);
        }

        /// <summary> Returns column id </summary>
        internal JET_COLUMNID GetColumnId(string columnName)
        {
            return GetColumns()
                .ToDictionary(c => c.ColumnName, c => c)[columnName]
                .Handle;
        }

        /// <summary> Disposition </summary>
        protected override void Dispose(bool dispose)
        {
            if (!Database.CurrentSession.Disposed)
                Api.JetCloseTable(CurrentSession, this);

        }
        
        /// <summary> Loads columns of table </summary>
        private void LoadColumns()
        {
            foreach(var c in Api.GetTableColumns(CurrentSession, this))
            {
                var ops = ColumnOptions.From(c);
                var cc = new Column(this, c.Name, ops, c.Columnid);
                
                _columns.Add(cc);
            }
        }

        /// <summary> Loads columns of table </summary>
        private void LoadIndexes()
        {
            foreach (var idx in Api.GetTableIndexes(CurrentSession, this))
            {
                var ix = new SearchIndex(this, idx.Name,
                    _columns.First(c => c.ColumnName == idx.IndexSegments[0].ColumnName));
                _indexes.Add(ix);
            }
        }

        /// <summary> Columns cache </summary>
        private readonly ICollection<Column> _columns = new List<Column>();

        private readonly ICollection<ISearchIndex> _indexes = new List<ISearchIndex>();
    }
}