using System;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> ESENT engine </summary>
    public sealed class Engine : HasJetHandleBase<JET_INSTANCE>
    {
        /// <summary> Creates engine insance </summary>
        public Engine(string engineInstanceName, EngineOptions options)
        {
            if (string.IsNullOrEmpty(engineInstanceName))
                engineInstanceName = "default";
            
            Api.JetCreateInstance(out JetHandle, engineInstanceName);
            Api.JetSetSystemParameter(this, JET_SESID.Nil, JET_param.CircularLog, 1, null);
            Api.JetInit(ref JetHandle);
        }

        /// <summary> Opens new ESENT session </summary>
        public Session OpenSession()
        {
            return new Session(this);
        }

        /// <summary> Terminating engine </summary>
        protected override void Dispose(bool dispose)
        {
            Api.JetTerm(this);
        }

        /// <summary> Session is not available on engine </summary>
        public override Session CurrentSession
        {
            get { throw new NotSupportedException(); }
        }
    }
}