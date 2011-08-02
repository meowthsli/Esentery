using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT "Transaction" </summary>
    public sealed class Transaction : IDisposable
    {
        /// <summary> Creates new transaction </summary>
        public Transaction(Session session)
        {
            Session = session;
            Api.JetBeginTransaction(Session);
        }

        /// <summary> Commits </summary>
        public void Commit(bool lazy = true)
        {
            Api.JetCommitTransaction(Session, 
                                     lazy? CommitTransactionGrbit.LazyFlush : CommitTransactionGrbit.WaitLastLevel0Commit);
            _commited = true;
        }

        /// <summary> Rolls back </summary>
        public void Rollback()
        {
            Api.JetRollback(Session, RollbackTransactionGrbit.None);
        }

        public void Dispose()
        {
            if(!_commited)
                Api.JetRollback(Session, RollbackTransactionGrbit.None);
        }

        /// <summary> currentSession </summary>
        public Session Session { get; private set;}

        private bool _commited = false;
    }
}