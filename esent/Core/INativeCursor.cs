using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Natural cursor </summary>
    internal interface INativeCursor
    {
        /// <summary> We need to use cursor handle </summary>
        JET_TABLEID CursorHandle { get; }
    }
}