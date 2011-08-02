using System;
using Meowth.Esentery.Core;

namespace Meowth.Esentery
{
    /// <summary> Esent disposables root </summary>
    public abstract class HasJetHandleBase<THandle> 
        : IDisposable, 
        IHasJetHandle<THandle>,
        ISessionObject
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            Disposed = true;
        }

        /// <summary> Finalizer </summary>
        ~HasJetHandleBase()
        {
            Console.WriteLine("Finalizer called on type " + GetType().Name);
            Dispose(false);
            Disposed = true;
        }

        /// <summary> Disposing </summary>
        protected abstract void Dispose(bool dispose);

        /// <summary> Is disposed </summary>
        internal bool Disposed { get; private set; }

        /// <summary> JET Handle </summary>
        public THandle Handle { get { return JetHandle; }}

        /// <summary> Handle retriever </summary>
        public static implicit operator THandle(HasJetHandleBase<THandle> obj)
        {
            return obj.JetHandle;
        }

        public abstract Session CurrentSession { get; }

        internal THandle JetHandle;
    }

    /// <summary>  </summary>
    internal interface IHasJetHandle<T>
    {
        T Handle { get; }
    }

    internal interface ISessionObject
    {
        Session CurrentSession { get; }
    }
}