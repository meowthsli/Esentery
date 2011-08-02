using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT session </summary>
    public sealed class Session : HasJetHandleBase<JET_SESID>
    {
        #region Construction & disposition

        /// <summary> ESENT session </summary>
        internal Session(Engine engine)
        {
            Engine = engine;
            Api.JetBeginSession(engine, out JetHandle, null, null);
        }

        protected override void Dispose(bool dispose)
        {
            if (!Engine.Disposed)
            {
                Api.JetEndSession(this, EndSessionGrbit.None);
            }
        }

        #endregion

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

        /// <summary> This engine </summary>
        public Engine Engine { get; private set; }

        /// <summary> Begins transaction </summary>
        public Transaction BeginTransaction()
        {
            return new Transaction(this);
        }

        /// <summary> Current session </summary>
        public override Session CurrentSession
        {
            get { return this; }
        }
    }
}