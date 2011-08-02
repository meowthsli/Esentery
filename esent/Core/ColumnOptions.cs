using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Column options </summary>
    public class ColumnOptions
    {
        public JET_COLUMNDEF ColumnDef { get; private set; }

        /// <summary> Creation </summary>
        public ColumnOptions(JET_COLUMNDEF columndef)
        {
            ColumnDef = columndef;
        }
    }
}