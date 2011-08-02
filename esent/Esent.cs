using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esent.Wrappers
{
    /// <summary> Esent disposables </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DisposableBase<T> : IDisposable
        where T : DisposableBase<T>
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableBase()
        {
            Dispose(false);
        }

        /// <summary> Disposing </summary>
        protected virtual void Dispose(bool dispose)
        {
            
        }
    }

    /// <summary> Engine start options </summary>
    public struct EngineOptions 
    {
        
    }

    /// <summary> ESENT engine </summary>
    public sealed class Engine : DisposableBase<Engine>
    {
        /// <summary> Creates engine insance </summary>
        public Engine(string engineInstanceName, EngineOptions options)
        {
            if (string.IsNullOrEmpty(engineInstanceName))
                engineInstanceName = "default";

            // Initialize ESENT. Setting JET_param.CircularLog to 1 means ESENT will automatically
            // delete unneeded logfiles. JetInit will inspect the logfiles to see if the last
            // shutdown was clean. If it wasn't (e.g. the application crashed) recovery will be
            // run automatically bringing the database to a consistent state.
            Api.JetCreateInstance(out _instance, engineInstanceName);
            Api.JetSetSystemParameter(_instance, JET_SESID.Nil, JET_param.CircularLog, 1, null);
            Api.JetInit(ref _instance);
        }

        /// <summary> Opens new ESENT session </summary>
        /// <returns></returns>
        public Session OpenSession()
        {
            return new Session(this);
        }

        /// <summary> Terminating engine </summary>
        protected override void Dispose(bool dispose)
        {
            Api.JetTerm(_instance);
            _instance = new JET_INSTANCE();
        }

        /// <summary> Instance handle </summary>
        internal JET_INSTANCE Instance {get { return _instance; }}

        /// <summary> Instance handle </summary>
        private JET_INSTANCE _instance;
    }

    /// <summary> ESENT session </summary>
    public sealed class Session : DisposableBase<Session>
    {
        /// <summary> Creates new database </summary>
        public Database CreateDatabase(string path)
        {
            return Database.Create(this, path);
        }

        /// <summary> Creates new database </summary>
        public Database OpenDatabase(string path)
        {
            return Database.Open(this, path);
        }


        /// <summary> Begins transaction </summary>
        public Transaction BeginTransaction()
        {
            return new Transaction(this);
        }

        /// <summary> ESENT session </summary>
        internal Session(Engine engine)
        {
            Api.JetBeginSession(engine.Instance, out _session, null, null);
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            Api.JetEndSession(_session, EndSessionGrbit.None);
        }

        /// <summary> Идентификатор сессии </summary>
        internal JET_SESID SessionId { get { return _session; } }
        
        /// <summary> Session id </summary>
        private readonly JET_SESID _session;
    }

    /// <summary> Database handle </summary>
    public sealed class Database : DisposableBase<Database>
    {
        /// <summary> Opens existing table </summary>
        public Cursor OpenTable(string tableName)
        {
            return new Cursor(this, tableName);
        }

        /// <summary> Creates new table </summary>
        public Table CreateTable(string tableName)
        {
            return new Table(this, tableName);
        }

        /// <summary> Session </summary>
        public Session Session { get { return _session; } }

        /// <summary> Creates new database </summary>
        internal static Database Create(Session session, string pathToDatabase)
        {
            // Create the database. To open an existing database use the JetAttachDatabase and 
            // JetOpenDatabase APIs.

            JET_DBID jetDbId;
            Api.JetCreateDatabase(session.SessionId, pathToDatabase, null, out jetDbId, CreateDatabaseGrbit.OverwriteExisting);
            return new Database(session, pathToDatabase, jetDbId);
        }

        /// <summary> Opens existing </summary>
        internal static Database Open(Session session, string pathToDatabase)
        {
            // TODO: analyse return code
            Api.JetAttachDatabase(session.SessionId, pathToDatabase, AttachDatabaseGrbit.None);
            JET_DBID jetDbId;
            Api.JetOpenDatabase(session.SessionId, pathToDatabase, null, out jetDbId, OpenDatabaseGrbit.None);
            return new Database(session, pathToDatabase, jetDbId);
        }

        /// <summary> Database id </summary>
        internal JET_DBID DatabaseId { get { return _database; }}

        /// <summary> Creates database </summary>
        private Database(Session session, string pathToDatabase, JET_DBID dbid)
        {
            _database = dbid;
            _session = session;
        }

        private readonly JET_DBID _database;
        private readonly Session _session;
    }

    /// <summary> ESENT "Transaction" </summary>
    public sealed class Transaction : DisposableBase<Transaction>
    {
        /// <summary> Creates new transaction </summary>
        public Transaction(Session session)
        {
            Api.JetBeginTransaction(session.SessionId);
            _session = session;
        }

        /// <summary> Commits </summary>
        public void Commit(bool lazy = true)
        {
            Api.JetCommitTransaction(_session.SessionId, 
                lazy? CommitTransactionGrbit.LazyFlush : CommitTransactionGrbit.None);
            _commited = true;
        }

        /// <summary> Rolls back </summary>
        public void Rollback()
        {
            Api.JetRollback(_session.SessionId, RollbackTransactionGrbit.None);
        }

        protected override void Dispose(bool dispose)
        {
            if(!_commited)
                Api.JetRollback(_session.SessionId, RollbackTransactionGrbit.None);
        }

        /// <summary> Session </summary>
        private readonly Session _session;

        private bool _commited = false;
    }

    /// <summary> ESENT Table (DML) </summary>
    public class Table : DisposableBase<Cursor>
    {
        /// <summary> Creates table from scratch </summary>
        internal Table(Database db, string tableName)
        {
            _database = db;
            Api.JetCreateTable(db.Session.SessionId, db.DatabaseId,
                tableName, 0, 100, out _tableId);
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            Api.JetCloseTable(_database.Session.SessionId, _tableId);
        }

        /// <summary> Creates column </summary>
        public void CreateColumn(string columnName, JET_COLUMNDEF columndef)
        {
            JET_COLUMNID column;
            Api.JetAddColumn(_database.Session.SessionId, _tableId, columnName, columndef, null, 0, out column);
        }

        private readonly JET_TABLEID _tableId;
        private readonly Database _database;
    }

    /// <summary> ESENT Cursor </summary>
    public sealed class Cursor : DisposableBase<Cursor>
    {
        /// <summary> Opens existing table </summary>
        internal Cursor (Database db, string tableName)
        {
            _database = db;
            Api.JetOpenTable(db.Session.SessionId, db.DatabaseId, 
                tableName, new byte[] {0}, 0,
                OpenTableGrbit.None, out _cursor);
        }

        /// <summary> Disposes table </summary>
        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            Api.JetCloseTable(_database.Session.SessionId, _cursor);
        }
        
        private readonly JET_TABLEID _cursor;
        private readonly Database _database;

        /// <summary> Appends row </summary>
        public void AddRow(string []columns, string[] values)
        {
            // Insert a record. This table only has one column but a table can have slightly over 64,000
            // columns defined. Unless a column is declared as fixed or variable it won't take any space
            // in the record unless set. An individual record can have several hundred columns set at one
            // time, the exact number depends on the database page size and the contents of the columns.
            var session = _database.Session;
            Api.JetPrepareUpdate(session.SessionId, _cursor, JET_prep.Insert);

            for (var i = 0; i < columns.Length; ++i)
            {
                var tableColumns = Api.GetColumnDictionary(session.SessionId, _cursor);
                var columnId = tableColumns[columns[i]];
                Api.SetColumn(session.SessionId, _cursor, columnId , values[i], Encoding.Unicode);
            }
            Api.JetUpdate(session.SessionId, _cursor);
        }

        /// <summary> Returns string value </summary>
        public string GetString(string columnName)
        {
            // Retrieve a column from the record. Here we move to the first record with JetMove. By using
            // JetMoveNext it is possible to iterate through all records in a table. Use JetMakeKey and
            // JetSeek to move to a particular record.
            var colId = Api.GetColumnDictionary(_database.Session.SessionId, _cursor)[columnName];
            return Api.RetrieveColumnAsString(_database.Session.SessionId, _cursor, colId, Encoding.Unicode);
        }
    }

    public class Column
    {
        
    }

    public class Index
    {
        
    }

    public class IndexRange
    {
        
    }
}
