using System;
using System.Text;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Column options </summary>
    public class ColumnOptions
    {
        /// <summary> Creates from columndef </summary>
        public static ColumnOptions CreateFrom(ColumnInfo cd)
        {
            return new ColumnOptions
                       {
                           ColumnType = Converters.GetClrType(cd),
                           IsNullable = ((cd.Grbit & ColumndefGrbit.ColumnNotNULL) == ColumndefGrbit.ColumnNotNULL),
                           Length = cd.MaxLength,
                       };
        }

        /// <summary> Creates column definition </summary>
        /// <returns></returns>
        public JET_COLUMNDEF GetColumnDef()
        {
            return new JET_COLUMNDEF
                       {
                           cbMax = Length ?? 0,
                           coltyp = Converters.GetEsentType(this),
                           grbit = CreateGrBit()
                       };
        }

        /// <summary> </summary>
        public ColumndefGrbit CreateGrBit()
        {
            var bit = ColumndefGrbit.None;
            bit |= IsNullable ? ColumndefGrbit.ColumnNotNULL : 0;
            return bit;
        }

        /// <summary> Is this nullable </summary>
        public bool IsNullable { get; set; }

        /// <summary> Type of column </summary>
        public Type ColumnType { get; set; }

        /// <summary> Length </summary>
        public int? Length { get; set; }

        /// <summary> Encoding </summary>
        public Encoding Encoding { get; set; }
    }
}