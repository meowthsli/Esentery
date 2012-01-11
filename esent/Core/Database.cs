using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Database handle </summary>
    public sealed class Database : HasJetHandleBase<JET_DBID>
    {
        #region Construction & disposition

        /// <summary> Creates database </summary>
        private Database(Session currentSession, string pathToDatabase, JET_DBID dbid)
        {
            JetHandle = dbid;
            _currentSession = currentSession;
            PathToDatabase = pathToDatabase;
        }

        protected override void Dispose(bool dispose)
        {
            if (!CurrentSession.Disposed)
            {
                Api.JetCloseDatabase(CurrentSession, this, CloseDatabaseGrbit.None);
                Api.JetDetachDatabase(CurrentSession, PathToDatabase);
            }
        }

        #endregion


        /// <summary> Opens existing table </summary>
        public Table OpenTable(string tableName)
        {
            return Table.Open(this, tableName);
        }

        /// <summary> Creates new table </summary>
        public Table CreateTable(string tableName)
        {
            return Table.Create(this, tableName);
        }

        /// <summary> Database location </summary>
        public string PathToDatabase { get; private set; }

        /// <summary> Session </summary>
        public Session Session { get { return CurrentSession; } }

        /// <summary> Creates new database </summary>
        internal static Database Create(Session session, string pathToDatabase)
        {
            JET_DBID jetDbId;
            Api.JetCreateDatabase(session, pathToDatabase, null, out jetDbId, CreateDatabaseGrbit.OverwriteExisting);
            return new Database(session, pathToDatabase, jetDbId);
        }

        /// <summary> Opens existing </summary>
        internal static Database Open(Session session, string pathToDatabase)
        {
            // TODO: analyse return code
			var res = Api.JetAttachDatabase(session, pathToDatabase, AttachDatabaseGrbit.None);
			if(res != JET_wrn.Success)
				throw new InvalidOperationException("Can't attach database, error " + Enum.GetName(typeof(JET_wrn), res));
			
			JET_DBID jetDbId;
            res = Api.JetOpenDatabase(session, pathToDatabase, null, out jetDbId, OpenDatabaseGrbit.None);
			if (res != JET_wrn.Success)
				throw new InvalidOperationException("Can't open database, error " + Enum.GetName(typeof(JET_wrn), res));
            return new Database(session, pathToDatabase, jetDbId);
        }

        /// <summary> Internal session link </summary>
        public override Session CurrentSession { get { return _currentSession; }}
        private readonly Session _currentSession;
    }
}