using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Meowth.Esentery.Core
{
    /// <summary> Untyped column accessor </summary>
    public delegate object ColumnValueGetter(JET_SESID session, JET_TABLEID table, JET_COLUMNID column);

    /// <summary> Untyped column accessor </summary>
    public delegate void ColumnValueSetter(JET_SESID session, JET_TABLEID table, JET_COLUMNID column, object value);

    /// <summary> Tuple of values </summary>
    public sealed class ColumnValueAccessor
    {
        /// <summary> Tuple of values </summary>
        public ColumnValueAccessor(ColumnValueGetter getter, ColumnValueSetter setter)
        {
            Getter = getter;
            Setter = setter;
        }

        /// <summary> Value getter </summary>
        public ColumnValueGetter Getter { get; private set; }

        /// <summary> Value setter </summary>
        public ColumnValueSetter Setter { get; private set; }
    }

    /// <summary> All byte converters </summary>
    /// <remarks> Переписать на LCG </remarks>
    internal static class Converters
    {
        /// <summary> Checks the type is valid </summary>
        public static void AssertTypeIsValidEsentType(Type type)
        {
            if (!s_validEsentTypes.Contains(type))
                throw new ArgumentException(
                    string.Format("Type '{0}' is not supported. Valid types are {1}", type, TypesList));

        }

        /// <summary> Returns CLR type</summary>
        public static Type GetClrType(ColumnInfo columndef)
        {
            return GetClrType(columndef.Coltyp);
        }

        /// <summary> Returns CLR type</summary>
        public static Type GetClrType(JET_COLUMNDEF columndef)
        {
            return GetClrType(columndef.coltyp);
        }

        /// <summary> Returns CLR type</summary>
        public static Type GetClrType(JET_coltyp colType)
        {
            return MapJetTypeToClrType[colType];
        }

        /// <summary> Returns ESENT type </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static JET_coltyp GetEsentType(ColumnOptions options)
        {
            if (MapClrTypeToEsentType.ContainsKey(options.ColumnType))
                return MapClrTypeToEsentType[options.ColumnType];

            if (options.ColumnType == typeof (byte[]))
                if (!options.Length.HasValue)
                    return JET_coltyp.LongBinary;
                else
                    return options.Length < 256 ? JET_coltyp.Binary : JET_coltyp.LongBinary;

            if (options.ColumnType == typeof (string))
                if (!options.Length.HasValue)
                    return JET_coltyp.LongText;
                else
                    return options.Length < 256 ? JET_coltyp.Text : JET_coltyp.LongText;

            AssertTypeIsValidEsentType(options.ColumnType);

            return 0;
        }

        /// <summary> Converts value to sequence of bytes </summary>
        public static byte[] Convert<T>(T value)
        {
            return GetConverter(typeof (T))(value);
        }

        /// <summary> Converts value to sequence of bytes </summary>
        public static byte[] Convert(Type t, object value)
        {
            return GetConverter(t)(value);
        }

        /// <summary> Retrieves value of column </summary>
        public static ColumnValueGetter GetGetter(Type t)
        {
            return s_accessors[t].Getter;
        }

        /// <summary> Retrieves value of column </summary>
        public static ColumnValueSetter GetSetter(Type t)
        {
            return s_accessors[t].Setter;
        }

        /// <summary> Access to column values </summary>
        private static readonly Dictionary<Type, ColumnValueAccessor> s_accessors
            = new Dictionary<Type, ColumnValueAccessor>
                  {
                      {
                          typeof (int),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsInt32(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (int) value))
                          },

                      {
                          typeof (uint),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsUInt32(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (uint) value))
                          },

                      {
                          typeof (short),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsInt16(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (short) value))
                          },

                      {
                          typeof (ushort),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsUInt16(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (ushort) value))
                          },

                      {
                          typeof (byte),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsByte(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (byte) value))
                          },

                      {
                          typeof (long),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsInt64(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (long) value))
                          },


                      {
                          typeof (double),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsDouble(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (double) value))
                          },

                      {
                          typeof (float),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsFloat(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (float) value))
                          },


                      {
                          typeof (string),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsString(sess, table, column, Encoding.Unicode),
                          (sess, table, column, value) =>
                          Api.SetColumn(sess, table, column, (string) value, Encoding.Unicode))
                          },

                      {
                          typeof (ulong),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsUInt64(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (ulong) value))
                          },

                      {
                          typeof (bool),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsBoolean(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (bool) value))
                          },

                      {
                          typeof (byte[]),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumn(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (byte[]) value))
                          },

                    {
                          typeof (Guid),
                          new ColumnValueAccessor(
                          (sess, table, column) => Api.RetrieveColumnAsGuid(sess, table, column),
                          (sess, table, column, value) => Api.SetColumn(sess, table, column, (Guid) value))
                          },
                  };
        
        /// <summary> Returns suitable converter </summary>
        private static Func<object, byte[]> GetConverter(Type t)
        {
            if (s_convertersToByteArray.ContainsKey(t))
                return x => s_convertersToByteArray[t](x);

            throw new NotSupportedException("Dont' know how to convert this type to byte sequence");
        }
      
        /// <summary> All types known to be valid Esent types </summary>
        private static readonly HashSet<Type> s_validEsentTypes
           = new HashSet<Type>
                  {
                      typeof(int),
                      typeof(uint),
                      typeof(short),
                      typeof(ushort),

                      typeof(byte),
                      typeof(long),
                      typeof(double),
                      typeof(float),
                      typeof(string),
                      typeof(ulong),
                      
                      typeof(bool),
                      typeof(DateTime),
                      typeof(Guid),

                      typeof(byte[]),
                  };

        /// <summary> Списо тип </summary>
        private static readonly string TypesList = string.Join(",", s_validEsentTypes.Select(s => s.Name).ToArray());

        private static readonly Dictionary<JET_coltyp, Type> MapJetTypeToClrType
            = new Dictionary<JET_coltyp, Type>
                  {
                      {JET_coltyp.Bit, typeof(bool)},
                      {JET_coltyp.DateTime, typeof(DateTime)},
                      {JET_coltyp.IEEEDouble, typeof(double)},
                      {JET_coltyp.IEEESingle, typeof(float)},

                      {JET_coltyp.Long, typeof(int)},
                      {VistaColtyp.UnsignedLong, typeof(uint)},

                      {VistaColtyp.LongLong, typeof(long)},
                      {JET_coltyp.UnsignedByte, typeof(byte)},

                      {JET_coltyp.Short, typeof(short)},
                      {VistaColtyp.UnsignedShort, typeof(ushort)},

                      {JET_coltyp.Binary, typeof(byte[])},
                      {JET_coltyp.LongBinary, typeof(byte[])},
                      
                      {JET_coltyp.Text, typeof(string)},
                      {JET_coltyp.LongText, typeof(string)},

                      {VistaColtyp.GUID, typeof(Guid)},
                  };

        private static readonly Dictionary<Type, JET_coltyp> MapClrTypeToEsentType
            = new Dictionary<Type, JET_coltyp>
                  {
                      { typeof(bool), JET_coltyp.Bit },
                      { typeof(decimal), JET_coltyp.Currency },
                      { typeof(DateTime), JET_coltyp.DateTime },
                      { typeof(double), JET_coltyp.IEEEDouble },
                      { typeof(float), JET_coltyp.IEEESingle },

                      { typeof(int), JET_coltyp.Long },
                      { typeof(uint), VistaColtyp.UnsignedLong },

                      { typeof(short), JET_coltyp.Short },
                      { typeof(ushort), VistaColtyp.UnsignedShort },

                      { typeof(long), VistaColtyp.LongLong },
                      { typeof(byte), JET_coltyp.UnsignedByte },

                      {typeof(Guid), VistaColtyp.GUID},
                      
                      //{ typeof(byte[]), JET_coltyp.LongBinary },
                      //{ typeof(string), JET_coltyp.LongText },
                      //{ typeof(byte[]), JET_coltyp.Binary},
                  };

        /// <summary> Table of converter functions </summary>
        private static readonly Dictionary<Type, Func<object, byte[]>> s_convertersToByteArray
            = new Dictionary<Type, Func<object, byte[]>>
                  {
                      { typeof (char), x => BitConverter.GetBytes((char) x) },
                      { typeof(string), x => ((string)x).ToCharArray().SelectMany(BitConverter.GetBytes).ToArray()},
                      { typeof(int), x => BitConverter.GetBytes((int)x) },
                      { typeof(short), x => BitConverter.GetBytes((short)x) },
                      { typeof(sbyte), x => BitConverter.GetBytes((sbyte)x) },
                      { typeof(long), x => BitConverter.GetBytes((long)x) },
                      { typeof(uint), x => BitConverter.GetBytes((uint)x) },
                      { typeof(ushort), x => BitConverter.GetBytes((ushort)x) },
                      { typeof(byte), x => BitConverter.GetBytes((byte)x) },
                      { typeof(ulong), x => BitConverter.GetBytes((ulong)x) },

                      { typeof(double), x => BitConverter.GetBytes((double)x) },
                      { typeof(float), x => BitConverter.GetBytes((float)x) },

                      { typeof(bool), x => BitConverter.GetBytes((bool)x) },
                      { typeof(DateTime), x => BitConverter.GetBytes(((DateTime)x).ToOADate()) },
                      { typeof(Guid), x => ((Guid)x).ToByteArray()}
                  };
    }
}