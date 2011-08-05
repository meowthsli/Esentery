using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Native cursor, representing actual non-managed ESENT cursor.
    /// Can be used directly to iterate thru index records </summary>
    internal interface INativeCursor
    {
        /// <summary> We need to use cursor handle </summary>
        JET_TABLEID CursorHandle { get; }
    }
}